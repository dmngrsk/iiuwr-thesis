using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace Thesis.Relinq.PsqlQueryGeneration
{
    public class PsqlGeneratingQueryModelVisitor : QueryModelVisitorBase
    {
        private readonly QueryPartsAggregator _queryParts = new QueryPartsAggregator();
        private readonly NpgsqlParameterAggregator _parameterAggregator = new NpgsqlParameterAggregator();
        private readonly NpgsqlDatabaseSchema _dbSchema;

        private readonly static Dictionary<Type, string> _resultOperatorsToString =
            new Dictionary<Type, string>()
            {
                { typeof(CountResultOperator),          "COUNT({0})" },
                { typeof(AverageResultOperator),        "AVG({0})" },
                { typeof(SumResultOperator),            "SUM({0})" },
                { typeof(MinResultOperator),            "MIN({0})" },
                { typeof(MaxResultOperator),            "MAX({0})" },
                { typeof(DistinctResultOperator),       "DISTINCT({0})" },
            };

        public PsqlGeneratingQueryModelVisitor(NpgsqlDatabaseSchema dbSchema) : base()
        {
            _dbSchema = dbSchema;
        }

        public static NpgsqlCommandData GeneratePsqlQuery(QueryModel queryModel, NpgsqlDatabaseSchema dbSchema)
        {
            var visitor = new PsqlGeneratingQueryModelVisitor(dbSchema);
            visitor.VisitQueryModel(queryModel);
            return visitor.GetPsqlCommand();
        }

        public NpgsqlCommandData GetPsqlCommand() =>
            new NpgsqlCommandData(_queryParts.BuildPsqlString(), _parameterAggregator.GetParameters());

        public override void VisitQueryModel(QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this, queryModel);
            queryModel.MainFromClause.Accept(this, queryModel);
            this.VisitBodyClauses(queryModel.BodyClauses, queryModel);
            this.VisitResultOperators(queryModel.ResultOperators, queryModel);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            var fromPart = fromClause.ItemType.ToString();
            fromPart = fromPart.Substring(fromPart.LastIndexOf('.') + 1);
            _queryParts.AddFromPart($"\"{_dbSchema.GetTableName(fromPart)}\"");

            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotImplementedException();
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            _queryParts.AddJoinPart(
                GetPsqlExpression(joinClause.OuterKeySelector),
                GetPsqlExpression(joinClause.InnerKeySelector));

            base.VisitJoinClause(joinClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            throw new NotImplementedException();
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            var fromPart = fromClause.ItemType.ToString();
            fromPart = fromPart.Substring(fromPart.LastIndexOf('.') + 1);
            _queryParts.AddFromPart($"\"{_dbSchema.GetTableName(fromPart)}\"");

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            _queryParts.AddOrderByPart(orderByClause.Orderings.Select(o => 
                new Tuple<string, OrderingDirection>(GetPsqlExpression(o.Expression), o.OrderingDirection)));
           
            base.VisitOrderByClause(orderByClause, queryModel, index);
        }


        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            // TODO: https://www.tutorialspoint.com/linq/linq_query_operators.htm

            var operatorType = resultOperator.GetType();

            if (_resultOperatorsToString.ContainsKey(operatorType))
            {
                _queryParts.SetSelectPartAsScalar(_resultOperatorsToString[operatorType]);
                base.VisitResultOperator(resultOperator, queryModel, index);
            }

            else if (operatorType == typeof(TakeResultOperator) || operatorType == typeof(SkipResultOperator))
            {
                var limitter = operatorType == typeof(TakeResultOperator) ? "LIMIT" : "OFFSET";
                var constExpression = operatorType == typeof(TakeResultOperator) ?
                    (resultOperator as TakeResultOperator).Count :
                    (resultOperator as SkipResultOperator).Count;
                
                _queryParts.AddPagingPart(limitter, GetPsqlExpression(constExpression));
                base.VisitResultOperator(resultOperator, queryModel, index);
            }
            
            else 
            {
                throw new NotImplementedException(
                    $"This LINQ provider does not provide the {resultOperator} result operator.");
            }
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            _queryParts.SetSelectPart(GetPsqlExpression(selectClause.Selector));
            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            _queryParts.AddWherePart(GetPsqlExpression(whereClause.Predicate));
            base.VisitWhereClause(whereClause, queryModel, index);
        }

        private string GetPsqlExpression(Expression expression) =>
            PsqlGeneratingExpressionVisitor.GetPsqlExpression(expression, _parameterAggregator, _dbSchema);
    }
}