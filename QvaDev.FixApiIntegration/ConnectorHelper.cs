using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using QvaDev.Communication.FixApi;

namespace QvaDev.FixApiIntegration
{
	internal static class ConnectorHelper
	{
		private static readonly HashSet<Assembly> ConnectorAssemblies = new HashSet<Assembly>();

		internal static string GetAppDir() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		static ConnectorHelper()
		{
			LoadConnectorAssemblies();
		}

		public static void LoadConnectorAssemblies()
		{
			var names = ConfigurationManager.AppSettings["ConnectorAssemblies"]?.Split(';');
			if (names == null)
				return;

			var path = GetAppDir();
			foreach (var name in names)
			{
				var asm = Path.Combine(path, name);
				if (!File.Exists(asm))
					continue;

				ConnectorAssemblies.Add(Assembly.LoadFrom(asm));
			}
		}

		public static void LoadConnectorAssembly(string fileName)
		{
			var asm = Assembly.LoadFrom(fileName);
			ConnectorAssemblies.Add(asm);
		}

		public static Type[] GetConnectorTypes()
		{
			var result = new List<Type>();
			foreach (var asm in ConnectorAssemblies)
			{
				result.AddRange(asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FixConnectorBase))));
			}

			return result.ToArray();
		}

		public static Type[] GetConfigurationTypes()
		{
			var result = new List<Type>();
			foreach (var asm in ConnectorAssemblies)
			{
				result.AddRange(asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IFixConfiguration).IsAssignableFrom(t)));
			}

			return result.ToArray();
		}

		public static Type GetConfigurationType(Type connectorType)
		{
			return connectorType.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == "Configuration" && !p.PropertyType.IsInterface)?.PropertyType;
		}

		public static Type GetConfigurationType(string name)
		{
			return GetConfigurationTypes().FirstOrDefault(c => c.Name == name);
		}

		public static Type GetConnectorType(Type confType)
		{
			var connectorTypes = GetConnectorTypes();
			return connectorTypes.FirstOrDefault(t => t.BaseType.GetGenericArguments().Any(a => a == confType));
		}
	}
}
