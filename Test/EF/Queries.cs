using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kovi.Data.Cqrs;
using Kovi.Data.Cqrs.Linq;

namespace Data.Cqrs.Test.EF
{
    public class AllAuthorLinqQuery : ILinqQuery<LinqQriteria, IEnumerable<Author>>
    {
		public IEnumerable<Author> Handle(LinqQriteria qrit, ILinqProvider linq)
		{
            var result = linq.Query<Author>();
            return result.ToList();
        }
    }

    public class AuthorLinqQuery: ILinqQuery<NameQriteria, IEnumerable<Author>>
    {
		public IEnumerable<Author> Handle(NameQriteria qrit, ILinqProvider linq)
		{
            var result = linq.Query<Author>()
                .Where(a =>
                    a.Name.Contains(qrit.Name)
                );
            return result.ToList();
        }
    }
}
