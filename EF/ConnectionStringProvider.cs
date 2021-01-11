using System;
using System.Collections.Generic;
using System.Configuration;

namespace Kovi.Data.EF
{
	public interface IConnectionStringProvider
	{
		String GetConnectionString(String name);
	}


	public class ConfigConnectionStringProvider
		:
		IConnectionStringProvider
	{

		protected readonly Dictionary<String, String> _connectionStrings;

		public String GetConnectionString(String connectionName)
		{
			if (!_connectionStrings.ContainsKey(connectionName))
				return null;

			return _connectionStrings[connectionName];
		}

		private void AppendConnectionStrings(ConnectionStringSettingsCollection connStringsCollection)
		{
			foreach (ConnectionStringSettings connString in connStringsCollection)
			{
				_connectionStrings.Add(connString.Name, connString.ConnectionString);
			}

		}
		public void DefineConnections(String configFile)
		{
			var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
			Configuration config = ConfigurationManager
				.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
			AppendConnectionStrings(config.ConnectionStrings.ConnectionStrings);
		}

		public ConfigConnectionStringProvider()
		{
			_connectionStrings = new Dictionary<String, String>();
			AppendConnectionStrings(ConfigurationManager.ConnectionStrings);
		}

		public ConfigConnectionStringProvider(String configFile)
		{
			_connectionStrings = new Dictionary<String, String>();
			DefineConnections(configFile);
		}

	}

}
