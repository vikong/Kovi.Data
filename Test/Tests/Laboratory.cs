using System;
using System.Diagnostics;
using System.Linq;
using Data.Cqrs.Test.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kovi.LinqExtensions;
using System.Linq.Expressions;
using System.Collections.Generic;
using Kovi.Data.Cqrs.Infrastructure;
using Kovi.Data.Cqrs.Linq;
using System.Data.Entity;
using System.Collections;
using System.Data.OleDb;
using Dapper;
using VfpClient;
using System.Data;

namespace Data.Cqrs.Test
{
	public partial class Car
	{

		#region prop
		public string Guid { get; set; }

		public string CarNumber { get; protected set; }
		public string CargoName { get; set; }
		public string Tir { get; set; }
		public DateTime? Crgdateoff { get; set; }
		public string Driver { get; set; }
		public DateTime? Entry { get; set; }
		public DateTime? Realcomin { get; set; }

		public string Licid { get; set; }
		public string Altapropid { get; set; }
		public DateTime? Billdate { get; set; }
		public string Billtypeid { get; set; }
		public Int32? PlombCount { get; set; }
		public string Info { get; set; }
		public string Comment { get; set; }
		public string Reason { get; set; }
		public DateTime? Stamp { get; set; }
		public string TermId { get; set; }
		public string Ucn { get; protected set; }
		public string Vehicle { get; set; }
		public string Whostateid { get; set; }

		public string CarPlaceId { get; set; }

		public int DocCount { get; set; }

		#endregion prop

		[Obsolete("For EF only", true)]
		public Car()
		{ }


	}

	[TestClass]
	public class Laboratory
	{
		[TestMethod]
		public void OleDbFoxPro_Sql_Test()
		{
			string efsql =
@"SELECT 
P2.Guid1 AS Guid, 
P2.Guid2 AS Guid1, 
P2.C1, 
P2.Cargoname, 
P2.Carnumber, 
P2.Entry, 
P2.Guid AS Guid2, 
P2.Realcomin, 
P2.Tir, 
P2.Vehicle, 
P2.C2, 
P2.Monitorid, 
P2.C3, 
P2.Guid3, 
P2.Inn, 
P2.Name
FROM ( SELECT 
	Limit1.Guid, 
	Limit1.Carnumber, 
	Limit1.Cargoname, 
	Limit1.Tir, 
	Limit1.Entry, 
	Limit1.Realcomin, 
	Limit1.Vehicle, 
	Limit1.Guid1, 
	Limit1.Guid2, 
	Limit1.C1, 
	Limit1.C2, 
	Limit1.Monitorid, 
	E5.Guid AS Guid3, 
	E5.Inn, 
	E5.Name, 
	 CAST( ICASE(ISNULL(E5.Guid),CAST(NULL AS i),1) AS i) AS C3
	FROM   (SELECT TOP 10 P11.Guid Guid, P11.Carnumber Carnumber, P11.Cargoname Cargoname, P11.Tir Tir, P11.Entry Entry, P11.Realcomin Realcomin, P11.Vehicle Vehicle, P11.Guid1 Guid1, P11.Buyerid Buyerid, P11.Guid2 Guid2, P11.C1 C1, P11.C2 C2, P11.Monitorid Monitorid
		FROM (select *, recno() as row_number from ( SELECT P1.Guid Guid, P1.Carnumber Carnumber, P1.Cargoname Cargoname, P1.Tir Tir, P1.Entry Entry, P1.Realcomin Realcomin, P1.Vehicle Vehicle, P1.Guid1 Guid1, P1.Buyerid Buyerid, P1.Guid2 Guid2, P1.C1 C1, P1.C2 C2, P1.Monitorid Monitorid
			FROM ( SELECT 
				E1.Guid, 
				E1.Carnumber, 
				E1.Cargoname, 
				E1.Tir, 
				E1.Entry, 
				E1.Realcomin, 
				E1.Vehicle, 
				E2.Guid AS Guid1, 
				E2.Buyerid, 
				E3.Guid AS Guid2, 
				1 AS C1, 
				ICASE(!ISNULL(G1.K1),G1.A1,0) AS C2, 
				G1.K1 AS Monitorid
				FROM    Monitor E1
				INNER JOIN Buyerpac E2 ON E1.Guid = E2.Carid
				INNER JOIN Buser E3 ON E2.Buyerid = E3.Buyerid
				LEFT JOIN  (SELECT 
					E4.K1 AS K1, 
					CAST(Count(E4.A1) as i) AS A1
					FROM ( SELECT 
						E4.Monitorid AS K1, 
						1 AS A1
						FROM Scandoc E4
					)  E4
					GROUP BY K1 ) G1 ON E1.Guid = G1.K1
				WHERE (  ((E1.Carplaceid = '        ') AND ((ICASE((E1.Carplaceid) IS NULL,.T.,.F.)) = (ICASE(('        ') IS NULL,.T.,.F.))))) AND (E3.Userid = @UserId) and (E1.entry>=@From and E1.entry<@To)
			)  P1
			ORDER BY P1.Realcomin DESC, P1.Carnumber
		) P1)  P11
		WHERE P11.row_number > 0
		ORDER BY P11.Realcomin DESC, P11.Carnumber ) Limit1
	LEFT JOIN Buyer E5 ON Limit1.Buyerid = E5.Guid
)  P2
ORDER BY P2.Realcomin DESC, P2.Carnumber, P2.Guid1, P2.Guid2, P2.Guid, P2.Monitorid, P2.C3";
			
			string sql =
@"select
mn.guid, mn.CarNumber, mn.CargoName, mn.Tir, mn.Entry, mn.realComin, mn.Vehicle, 
b.guid as buyerId, b.name as buyerName, b.inn, 
count(sd.monitorId) as docCount 
from monitor mn 
inner join buyerPac bp on mn.guid=bp.carId 
inner join buyer b on bp.buyerId=b.guid 
inner join bUser bu on bp.buyerId=bu.buyerId 
left join scandoc sd on mn.guid=sd.monitorId 
where (mn.CarPlaceId=='        ') 
and (bu.UserId = @UserId) 
and (mn.RealComin>=@From and mn.RealComin<@To) 
group by mn.guid, mn.CarNumber, mn.CargoName, mn.Tir, mn.Entry, mn.realComin, mn.Vehicle, 
b.guid, b.name, b.inn
order by mn.RealComin";

			string dataPath = @"\\ntbkfs\prog\Piramida\Krk2a\bases\BORODKI\";
			//string dataPath = @"E:\monitor_client\BASES\BORODKI\";
			Stopwatch stopwatch = new Stopwatch();
			List<Car>  result;
			using (IDbConnection conn = new VfpConnection($"Provider=VFPOLEDB.1;Data Source={dataPath};"))
			{
				stopwatch.Start();
				conn.Open();

				result = conn.Query<Car>(sql, 
					new { UserId = "k_3ш+By`", From=new DateTime(2021,10,1), To = new DateTime(2021, 11, 1) })
					.ToList();

				//result = conn.Query<Car>(sql, new { CarPlaceId="        ", UserId= "+{kЯ$Дж+" }).ToList();
				//result = conn.Query<Car>(sql,new { p__linq__0="        ", p__linq__1 = "+{kЯ$Дж+" }).ToList();

				conn.Close();
				stopwatch.Stop();
			}
			Console.WriteLine($"Selected {result.Count} rows at {stopwatch.Elapsed}");
			foreach (var item in result)
			{
				Console.WriteLine($"Id:'{item.Guid}' CarNumber:{item.CarNumber.Trim()} Entry:{item.Entry} Doc(s):{item.DocCount}");
			}
		}

		[TestMethod]
		public void TakePage()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4"),
				new Author(5, "a5"),
				new Author(6, "a6"),
				new Author(7, "a7"),
				new Author(8, "a8"),
				new Author(9, "a9")
			};

			IQueryable queryableData = authors.AsQueryable().OrderBy(a => a.Id);
			int page = 2, pageSize = 3;
			
			var actual = queryableData.TakePage(page, pageSize).Cast<Author>().ToList();
			var expected = authors.AsQueryable<Author>()
				.OrderBy(a => a.Id)
				.Skip(pageSize*(page-1))
				.Take(pageSize).ToList();

			CollectionAssert.AreEqual(expected, actual);
			


		}


		[TestMethod]
		public void ConventionsTest()
		{
			var idQrit = new EF.IdQriteria { Id = 1};
			var authorConv = Conventions<AuthorDto>.IdSpec;
			var authorPageConv = Conventions<AuthorDto>.PageSpec();

			Assert.ThrowsException<InvalidOperationException>(() => Conventions<PublisherDto>.IdSpec);
		}

		[TestMethod]
		public void ContextExperiments()
		{
			//System.Data.Entity.Database.SetInitializer(new BooksInitializer());
			using (BookContext ctx = new BookContext())
			{
				Debug.WriteLine($"Database: {ctx.Database.Connection.ConnectionString}");
				ctx.Database.Log = s => Debug.WriteLine(s);
				ctx.Database.Initialize(true);

				Assert.IsNotNull(ctx.Books);
				Assert.IsTrue(ctx.Books.Any());

				Book newBook = new Book("new book");
				newBook.AddAuthor(1);
				newBook.AddGenre(1);
				foreach (var a in newBook.BookAuthors)
				{
					var entry = ctx.Entry(a);
					Debug.WriteLine($"{a.Author.Id}:{a.ToString()}, {entry.State}");
					entry.State = System.Data.Entity.EntityState.Unchanged;
				}

				ctx.Books.Add(newBook);
				foreach (var e in ctx.ChangeTracker.Entries())
					Debug.WriteLine($"{e.Entity.GetType().Name}: {e.State}");
				ctx.SaveChanges();
			}
		}

		[TestMethod]
		public void ManuallyCreateQuery()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4")
			};
			IQueryable queryableData = authors.AsQueryable();

			var entityType = typeof(Author);

			ParameterExpression pe = Expression.Parameter(entityType, "a");

			Expression left = Expression.Property(pe, "Id");

			Expression right = Expression.Constant(2);

			Expression myExpr = Expression.Equal(left, right);

			var etype = typeof(Func<,>).MakeGenericType(entityType, typeof(Boolean));

			//var predicate = Expression.Lambda<Func<Author, Boolean>>(myExpr, new ParameterExpression[] { pe });
			var predicate = Expression.Lambda(etype, myExpr, new ParameterExpression[] { pe });

			MethodCallExpression whereCallExpression = Expression.Call(
				typeof(Queryable),
				"Where",
				new Type[] { entityType }, //{ queryableData.ElementType },
				queryableData.Expression,
				predicate);

			IQueryable results = queryableData.Provider.CreateQuery(whereCallExpression);

			foreach (var a in results)
				Console.WriteLine(a);

		}

		[TestMethod]
		public void ManuallyCreateQuery1()
		{
			Author[] authors = {
				new Author(1, "a1"),
				new Author(2, "a2"),
				new Author(3, "a3"),
				new Author(4, "a4")
			};
			IQueryable queryableData = authors.AsQueryable();

			var entityType = typeof(Author);

			//ParameterExpression pe = Expression.Parameter(entityType, "a");

			//Expression left = Expression.Property(pe, "Id");

			//Expression right = Expression.Constant(2);

			//Expression myExpr = Expression.Equal(left, right);

			//var etype = typeof(Func<,>).MakeGenericType(entityType, typeof(Boolean));

			//var predicate = Expression.Lambda<Func<Author, Boolean>>(myExpr, new ParameterExpression[] { pe });
			//var predicate = Expression.Lambda(etype, myExpr, new ParameterExpression[] { pe });

			//MethodCallExpression whereCallExpression = Expression.Call(
			//	typeof(Queryable),
			//	"Where",
			//	new Type[] { entityType }, //{ queryableData.ElementType },
			//	queryableData.Expression,
			//	predicate);

			MethodCallExpression countCallExpr = Expression.Call(
				typeof(Queryable),
				"Count",
				new Type[] { entityType }, 
				queryableData.Expression);

			//IQueryable results = queryableData.Provider.CreateQuery(whereCallExpression);
			var res = queryableData.Provider.Execute<Int32>(countCallExpr);

			Console.WriteLine(res);
			//foreach (var a in results)
			//	Console.WriteLine(a);

		}


	}
}
