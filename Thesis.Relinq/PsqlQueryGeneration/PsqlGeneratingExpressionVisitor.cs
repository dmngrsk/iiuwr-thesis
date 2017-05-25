using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace Thesis.Relinq.PsqlQueryGeneration
{
    public class PsqlGeneratingExpressionVisitor : RelinqExpressionVisitor
    {
        private readonly StringBuilder _psqlExpression = new StringBuilder();
        private readonly NpgsqlParameterAggregator _parameterAggregator;
        private readonly PsqlGeneratingQueryModelVisitor _queryModelVisitor;
        private bool _conditionalStart = true;

        private readonly static Dictionary<ExpressionType, string> _binaryExpressionOperatorsToString = 
            new Dictionary<ExpressionType, string>()
            {
                { ExpressionType.Equal,                 " = " },
                { ExpressionType.NotEqual,              " != " },
                { ExpressionType.GreaterThan,           " > " },
                { ExpressionType.GreaterThanOrEqual,    " >= " },
                { ExpressionType.LessThan,              " < " },
                { ExpressionType.LessThanOrEqual,       " <= " },

                { ExpressionType.Add,                   " + " },
                { ExpressionType.AddChecked,            " + " }, 
                { ExpressionType.Subtract,              " - " }, 
                { ExpressionType.SubtractChecked,       " - " },
                { ExpressionType.Multiply,              " * " },
                { ExpressionType.MultiplyChecked,       " * " },
                { ExpressionType.Divide,                " / " },
                { ExpressionType.Modulo,                " % " },

                { ExpressionType.And,                   " & " },
                { ExpressionType.Or,                    " | " },
                { ExpressionType.ExclusiveOr,           " # " },
                { ExpressionType.LeftShift,             " << " },
                { ExpressionType.RightShift,            " >> " },

                { ExpressionType.AndAlso,               " AND " },
                { ExpressionType.OrElse,                " OR " }
            };

        private readonly static Dictionary<string, string> _methodCallNamesToString = 
            new Dictionary<string, string>()
            {
                { "Equals",                             "{0} = {1}" },

                { "ToLower",                            "LOWER({0})" },
                { "ToUpper",                            "UPPER({0})" },
                { "Reverse",                            "REVERSE({0})" },
                { "Length",                             "LENGTH({0})" },
                
                { "Concat",                             "CONCAT({0})" },

                { "Substring",                          "SUBSTRING({0} FROM {1}+1)" },
                { "SubstringFor",                       "SUBSTRING({0} FROM {1}+1 FOR {2})" },

                { "Replace",                            "REPLACE({0}, {1}, {2})" },

                { "Trim",                               "TRIM(both {1} from {0})" },
                { "TrimStart",                          "TRIM(leading {1} from {0})" },
                { "TrimEnd",                            "TRIM(trailing {1} from {0})" },

                { "Contains",                           "{0} LIKE '%' || {1} || '%'" },
                { "StartsWith",                         "{0} LIKE {1} || '%'" },
                { "EndsWith",                           "{0} LIKE '%' || {1}" }
            };

        private PsqlGeneratingExpressionVisitor(NpgsqlParameterAggregator parameterAggregator, 
            PsqlGeneratingQueryModelVisitor queryModelVisitor)
        {
            _parameterAggregator = parameterAggregator;
            _queryModelVisitor = queryModelVisitor;
        }

        public static string GetPsqlExpression(Expression linqExpression, 
            NpgsqlParameterAggregator parameterAggregator, 
            PsqlGeneratingQueryModelVisitor queryModelVisitor)
        {
            var visitor = new PsqlGeneratingExpressionVisitor(
                parameterAggregator, queryModelVisitor);
            visitor.Visit(linqExpression);
            return visitor.GetPsqlExpression();
        }

        private string GetPsqlExpression()
        {
            return _psqlExpression.ToString();
        }

        // RelinqExpressionVisitor override methods.
        protected override Expression VisitQuerySourceReference(
            QuerySourceReferenceExpression expression)
        {
            if (expression.Type.FullName.Contains("IGrouping"))
            {
                var groupResultOperator = ((expression.ReferencedQuerySource as MainFromClause)
                    .FromExpression as SubQueryExpression)
                    .QueryModel.ResultOperators[0] as GroupResultOperator;

                this.Visit(groupResultOperator.ElementSelector);
                _psqlExpression.Append(", ");
                this.Visit(groupResultOperator.KeySelector);

                // throw new NotImplementedException("This LINQ provider does not provide grouping yet.");
            }
            else
            {
                string fullType = expression.ReferencedQuerySource.ItemType.ToString();
                int index = fullType.LastIndexOf('.') + 1;
                string type = fullType.Substring(index);

                _psqlExpression.Append($"\"{_queryModelVisitor.DbSchema.GetTableName(type)}\"");
            }

            return expression;
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            _queryModelVisitor.OpenSubQuery();
            _queryModelVisitor.VisitQueryModel(expression.QueryModel);
            _queryModelVisitor.CloseSubQuery();
            return expression;
        }

        // ExpressionVisitor override methods.
        protected override Expression VisitBinary(BinaryExpression expression)
        {
            this.Visit(expression.Left);
    
            var isAddingStrings = expression.NodeType == ExpressionType.Add && 
                (expression.Left.Type == typeof(string)
                || expression.Right.Type == typeof(string));

            if (isAddingStrings)
                _psqlExpression.Append(" || ");
            else
                _psqlExpression.Append(_binaryExpressionOperatorsToString[expression.NodeType]);
    
            this.Visit(expression.Right);
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.BlockExpression.
        protected override Expression VisitBlock(BlockExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.ConditionalExpression.
        protected override Expression VisitConditional(ConditionalExpression expression)
        {
            if (_conditionalStart)
            {
                _psqlExpression.Append("CASE");
                _conditionalStart = false;
            }

            _psqlExpression.Append(" WHEN ");
            this.Visit(expression.Test);
            _psqlExpression.Append(" THEN ");
            this.Visit(expression.IfTrue);

            if (expression.IfFalse.NodeType == ExpressionType.Conditional)
            {
                this.Visit(expression.IfFalse);
            }
            else // If constant, then that means the switch block has ended.
            {
                _psqlExpression.Append(" ELSE ");
                this.Visit(expression.IfFalse);
                _psqlExpression.Append(" END");
                _conditionalStart = true;
            }

            return expression;
        }
        
        protected override Expression VisitConstant(ConstantExpression expression)
        {
            var parameterName = _parameterAggregator.AddParameter(expression.Value);
            _psqlExpression.Append($"@{parameterName}");
            return expression;
        }
        // Visits the System.Linq.Expressions.DebugInfoExpression.
        protected override Expression VisitDebugInfo(DebugInfoExpression expression)
        {
            return expression;
        }
        // Visits the System.Linq.Expressions.DefaultExpression.
        protected override Expression VisitDefault(DefaultExpression expression)
        {
            return expression;
        }
        // Visits the children of the extension expression.
        protected override Expression VisitExtension(Expression expression)
        {
            Console.WriteLine("Hello, world!");
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.GotoExpression.
        protected override Expression VisitGoto(GotoExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.IndexExpression.
        protected override Expression VisitIndex(IndexExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.InvocationExpression.
        protected override Expression VisitInvocation(InvocationExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.LabelExpression.
        protected override Expression VisitLabel(LabelExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.Expression`1.
        protected override Expression VisitLambda<T>(Expression<T> expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.ListInitExpression.
        protected override Expression VisitListInit(ListInitExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.LoopExpression.
        protected override Expression VisitLoop(LoopExpression expression)
        {
            return expression;
        }
        
        protected override Expression VisitMember(MemberExpression expression)
        {
            if (expression.Expression.NodeType == ExpressionType.MemberAccess &&
                expression.Member.Name == "Length")
            {
                _psqlExpression.Append("LENGTH(");
                this.Visit(expression.Expression);
                _psqlExpression.Append(")");
            }
            else
            {
                this.Visit(expression.Expression);
                var columnName = _queryModelVisitor.DbSchema.GetColumnName(expression.Member.Name);
                _psqlExpression.Append($".\"{columnName}\"");
            }

            return expression;
        }
        // Visits the children of the System.Linq.Expressions.MemberInitExpression.
        protected override Expression VisitMemberInit(MemberInitExpression expression)
        {
            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            var methodName = expression.Method.Name;

            if (_methodCallNamesToString.ContainsKey(methodName))
            {
                this.Visit(expression.Object);
                var expressionAccumulator = new List<object>(new object[] { _psqlExpression.ToString() });
                _psqlExpression.Clear();

                foreach (var argument in expression.Arguments)
                {
                    this.Visit(argument);
                    expressionAccumulator.Add(_psqlExpression.ToString());
                    _psqlExpression.Clear();
                }

                var expectedArguments = Regex.Matches(
                    _methodCallNamesToString[methodName], "\\{([^}]+)\\}"
                );

                while (expressionAccumulator.Count < expectedArguments.Count) 
                {
                    expressionAccumulator.Add(string.Empty);
                }
                
                switch (methodName)
                {
                    case "Concat":
                        expressionAccumulator.RemoveAt(0);
                        _psqlExpression.AppendFormat(
                            _methodCallNamesToString[methodName],
                            string.Join(", ", expressionAccumulator.Select(x => x.ToString()))
                        );
                        break;

                    case "Substring":
                        if (expressionAccumulator.Count == 3)
                            _psqlExpression.AppendFormat(
                                _methodCallNamesToString[methodName + "For"],
                                expressionAccumulator.ToArray()
                            );
                        else // if (expressionAccumulator.Count == 2)
                            _psqlExpression.AppendFormat(
                                _methodCallNamesToString[methodName],
                                expressionAccumulator.ToArray()
                            );
                        break;

                    default:
                        _psqlExpression.AppendFormat(
                            _methodCallNamesToString[methodName], 
                            expressionAccumulator.ToArray()
                        );
                        break;
                }

                return expression;
            }

            throw new NotImplementedException(
                $"This LINQ provider does not provide the {methodName} method.");
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            this.Visit(expression.Arguments[0]);

            if (expression.Members != null)
            {
                _psqlExpression.Append($" AS {expression.Members[0].Name}");
            
                for (int i = 1; i < expression.Members.Count; i++)
                {
                    _psqlExpression.Append(", ");
                    this.Visit(expression.Arguments[i]);
                    _psqlExpression.Append($" AS {expression.Members[i].Name}");
                }
            }

            return expression;
        }
        // Visits the children of the System.Linq.Expressions.NewArrayExpression.
        protected override Expression VisitNewArray(NewArrayExpression expression)
        {
            return expression;
        }
        // Visits the System.Linq.Expressions.ParameterExpression.
        protected override Expression VisitParameter(ParameterExpression expression)
        {
            return expression;
        }

        // Visits the children of the System.Linq.Expressions.RuntimeVariablesExpression.
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.SwitchExpression.
        protected override Expression VisitSwitch(SwitchExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.TryExpression.
        protected override Expression VisitTry(TryExpression expression)
        {
            return expression;
        }
        // Visits the children of the System.Linq.Expressions.TypeBinaryExpression.
        protected override Expression VisitTypeBinary(TypeBinaryExpression expression)
        {
            return expression;
        }
        
        protected override Expression VisitUnary(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Not)
            {
                _psqlExpression.Append("NOT (");
                this.Visit(expression.Operand);
                _psqlExpression.Append(")");
            }
            else
            {
                this.Visit(expression.Operand as MemberExpression);
            }
                
            return expression;
        }
    }
}