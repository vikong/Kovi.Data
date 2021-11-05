using System.Collections;

namespace Data.Cqrs.Test
{
    public class AuthorDtoComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			AuthorDto xa = (AuthorDto)x, ya = (AuthorDto)y;
			if (xa.Id < ya.Id)
				return 1;
			if (xa.Id > ya.Id)
				return -1;
			return xa.Name.CompareTo(ya.Name);
		}
	}
}
