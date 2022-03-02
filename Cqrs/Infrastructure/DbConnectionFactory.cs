using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Kovi.Data.Cqrs
{
	/// <summary>
	/// Фабрика для создания IDbConnection по строке соединения или имени
	/// </summary>
	public static class DbConnectionFactory
	{
		private const string Provider = "provider";

		/// <summary>
		/// Поиск строки соединения в секции конфига
		/// </summary>
		/// <param name="connectionString">имя | строка соединения</param>
		/// <returns><see cref="ConnectionStringSettings"/> или null</returns>
		public static ConnectionStringSettings GetFromConfiguration(String connectionString)
		{
			ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[connectionString];

			if (css == null)
			{
				css = ConfigurationManager.ConnectionStrings
					.Cast<ConnectionStringSettings>()
					.FirstOrDefault(s => s.ConnectionString == connectionString);
			}

			return css;
		}

		/// <summary>
		/// Генерирует <see cref="IDbConnection"/> имени или строке соединения
		/// </summary>
		/// <param name="connection">имя | строка соединения</param>
		/// <returns>Новый экземпляр <see cref="IDbConnection"/></returns>
		public static IDbConnection Create(String connection)
		{
			// определяем провайдера
			string providerName = null;
			string connectionString = null;

			ConnectionStringSettings css = GetFromConfiguration(connection);

			if (css != null)
			{
				providerName = css.ProviderName;
				connectionString = css.ConnectionString;
			}
			else // пробуем разобрать как стандартную строку соединения
			{
				try
				{
					var csb = new DbConnectionStringBuilder { ConnectionString = connection };
					if (csb.ContainsKey(Provider))
					{
						providerName = csb[Provider].ToString();
						connectionString = connection;
					}
				}
				catch (Exception)
				{
					throw;
				}
			}

			// если ничего не получилось
			if (providerName == null)
			{
				throw new Exception("Не удалось определить поставщика данных для строки соединения");
			}

			// смотрим, был ли зарегистрирована фабрика провайдера
			var providerExists = DbProviderFactories
									 .GetFactoryClasses()
									 .Rows.Cast<DataRow>()
									 .Any(r => r[2].Equals(providerName));
			if (!providerExists)
			{
				throw new Exception($"Не определён или не зарегистрирован поставщик {providerName} для строки соединения");
			}

			// создаем коннекшн
			IDbConnection dbConn = null;
			try
			{
				DbProviderFactory factory =
					DbProviderFactories.GetFactory(providerName);

				dbConn = factory.CreateConnection();
				dbConn.ConnectionString = connectionString;
			}
			catch (Exception)
			{
				throw;
			}

			return dbConn;
		}
	}
}