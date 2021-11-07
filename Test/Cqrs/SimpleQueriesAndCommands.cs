using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Cqrs.Test.Dapper;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;

namespace Data.Cqrs.Test
{
	#region Queries

	public class StringQriteria : IQriteria
	{
		public String Name { get; set; }
	}

	public class SimpleQuery : IQueryHandler<StringQriteria, QueryResult<String>>
	{
		public QueryResult<String> Handle(StringQriteria qrit)
		{
			string data = $"SimpleQuery:{qrit.Name}";

			var result = new QueryResult<String>(data);

			result.From(nameof(SimpleQuery));

			return result;
		}
	}

	public class IntQriteria : IQriteria
	{
		public int Id { get; set; }
	}

	public class SimpleIdQuery : IQueryHandler<IntQriteria, QueryResult<String>>
	{
		public QueryResult<String> Handle(IntQriteria qrit)
		{
			string data = $"SimpleIdQuery:{qrit.Id}";

			var result = new QueryResult<String>(data);

			result.From(nameof(SimpleIdQuery));

			return result;
		}
	}

	public class SimpleLinqQriteria : IQriteria
	{
		public String Name { get; set; }
	}

	public class SimpleLinqQuery : ILinqQuery<SimpleLinqQriteria, QueryResult<String>>
	{
		public static string Data = "SimpleLinqQuery";

		public QueryResult<String> Handle(SimpleLinqQriteria q, ILinqProvider linq)
		{
			var result = new QueryResult<String>($"{Data}:{q.Name}");

			result.From(Data);

			return result;
		}
	}


	public class SimpleDapperQriteria : IQriteria
	{
		public String Name { get; set; }
	}

	public class SimpleDapperQuery : IQueryHandler<SimpleDapperQriteria, QueryResult<String>>
	{
		public static string Data = "SimpleDapperQuery";
		public QueryResult<String> Handle(SimpleDapperQriteria qrit)
		{
			var result = new QueryResult<String>($"{Data}:{qrit.Name}");

			result.From(Data);

			return result;
		}
	}

	#endregion Queries

	#region Commands

	public class SimpleCommand: ICommand
	{
		public string Name { get; set; }
	}

	public class SimpleCommandHandler : ICommandHandler<SimpleCommand>
	{
		public static string Data = "SimpleCommandHandler";
		public Result Handle(SimpleCommand command)
		{
			List<string> data = new List<string>();
			data.Add($"{Data}:{command.Name}");
			return Result.Ok(data);
		}
	}

	#endregion Commands

	public class Entity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public class EntityDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public class QResult
	{
		public string From { get; set; }
		public IEnumerable<EntityDto> List { get; set; }

		public override string ToString()
		{
			return $"From:{From}\r\n{List.Aggregate("", (c, n) => c + n.Name + ",\r\n")}";
		}

	}
	public class Qrit2 : IQriteria
	{
		public int Id { get; set; }
	}
	public class DapperQuery : IQueryHandler<Qrit2, QResult>
	{
		protected readonly IDapperQueryHandler<Qrit2, QResult> QueryHandler;
		public DapperQuery(IDapperQueryHandler<Qrit2, QResult> queryHandler)
		{
			QueryHandler = queryHandler;
		}

		public QResult Handle(Qrit2 qrit)
		{
			return new QResult
			{
				From = nameof(DapperQuery),
				List = new EntityDto[]
				{
				new EntityDto { Id=1, Name="01"},
				new EntityDto { Id=1, Name="02"},
				}
			};

		}
	}

	public class DapperQueryHandler<TIn, TOut> : IDapperQueryHandler<TIn, TOut>
		where TIn : IQriteria
	{
		protected readonly IDapperQueryHandler<TIn, TOut> Decorated;

		public DapperQueryHandler(IDapperQueryHandler<TIn, TOut> decorated)
		{
			Decorated = decorated;
		}

		public TOut Handle(TIn input)
		{
			Debug.WriteLine("DapperHandler Handle");
			var result = Decorated.Handle(input);
			return result;
		}
	}

	//public class LinqQueryHandler<TIn, TOut> : IQueryHandler<TIn, TOut>
	//    where TIn: IQrit
	//{
	//    public LinqQueryHandler(ILinqQueryHandler<TIn, TOut> decorated)
	//    {
	//    }

	//    public TOut Handle(TIn input)
	//    {
	//        Debug.WriteLine("LinqHandler Handle");
	//        var result = this.Decorated.Handle(input);
	//        return result;
	//    }
	//}

	//public interface IQueryController<T>
	//{
	//    IEnumerable<T> Handle();
	//}

	//public class DecoratedList<T> : IQueryController<T>
	//{
	//    protected IQueryController<T> Decorated;

	//    public DecoratedList(IQueryController<T> decorated)
	//    {
	//        Decorated = decorated;
	//    }
	//    public IEnumerable<T> Handle()
	//    {
	//        Console.WriteLine("DecoratedList");
	//        return Decorated.Handle();
	//    }
	//}

	//public class ListString : IQueryController<String>
	//{
	//    public IEnumerable<string> Handle()
	//    {
	//        return new String[] { "1","2" };
	//    }
	//}

}
