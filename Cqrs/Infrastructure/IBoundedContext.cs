using System;

namespace Kovi.Data
{
	/// <summary>
	/// Определяет связанный контекст
	/// </summary>
	public interface IBoundedContext
	{
		String BoundedContext { get; }
	}
}
