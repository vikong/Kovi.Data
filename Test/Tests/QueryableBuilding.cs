using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kovi.Data.Cqrs.NotUsed
{
	class Test
	{
		private MethodCallExpression BuiltMethodCall(IQueryable _query, string CustTypeID, Type _ParentObjType, Type _ChildObjType, string strChildObj, string strChildCol)
		{

			//This function will build a dynamic linq expression tree representing the ling calls of:
			//Customers.Where(c => c.CustomerDemographics.Any(cd => cd.CustomerTypeID = custTypeID))

			ConstantExpression value = Expression.Constant(CustTypeID);

			//Build the outer part of the Where clause
			ParameterExpression parameterOuter = Expression.Parameter(_ParentObjType, "c");
			MemberExpression propertyOuter = Expression.Property(parameterOuter, strChildObj);

			//Build the comparison inside of the Any clause
			ParameterExpression parameterInner = Expression.Parameter(_ChildObjType, "cd");
			MemberExpression propertyInner = Expression.Property(parameterInner, strChildCol);

			BinaryExpression comparison = Expression.Equal(propertyInner, value);
			LambdaExpression lambdaInner = Expression.Lambda(comparison, parameterInner);

			//Create Generic Any Method
			Boolean methodLambda(MethodInfo m) 
				=> m.Name == "Any" && m.GetParameters().Length == 2;

			MethodInfo method = typeof(Enumerable).GetMethods().Where(methodLambda).Single().MakeGenericMethod(_ChildObjType);

			//Create the Any Expression Tree and convert it to a Lambda
			MethodCallExpression callAny = Expression.Call(method, propertyOuter, lambdaInner);
			LambdaExpression lambdaAny = Expression.Lambda(callAny, parameterOuter);

			//Build the final Where Call
			MethodCallExpression whereCall = Expression.Call(typeof(Queryable), "Where", new Type[] { _query.ElementType }, new Expression[] {
	_query.Expression,
	Expression.Quote(lambdaAny)
});

			return whereCall;
		}
	}
}
