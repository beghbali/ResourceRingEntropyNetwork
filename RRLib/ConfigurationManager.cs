using System;
using System.IO;
using System.Xml;

namespace RRLib
{
	public delegate void ConfigurationHandler(string confValue);

	public class ConfigurationNode
	{
		private bool _enable;
		public bool enable
		{
			get { return _enable; }
			set { _enable = value; }
		}
		
		private ConfigurationHandler _configure;
		public ConfigurationHandler configure
		{
			get { return _configure; }
			set { _configure = value; }
		}
	
		public ConfigurationNode(bool __enable, ConfigurationHandler __configure)
		{
			enable = __enable;
			configure = __configure;
		}
	}

	/// <summary>
	/// Takes care of saving and restoring configuration. Applying configuration and more.
	/// </summary>
	public class ConfigurationManager
	{
		private FileInfo startupConfig;
		private XmlParser xmlParser;
		private ConfigurationNode[] handlers;

		public ConfigurationManager(string startup_file)
		{
			startupConfig= new FileInfo(startup_file);
			xmlParser = new XmlParser(startup_file);
		}

		/// <summary>
		/// Loads the configuration from an XML configuration file and mark the entries in the 
		/// 
		/// </summary>
		/// <returns></returns>
		public bool LoadConfiguration()
		{
			if (xmlParser == null)
				return false;

			while (xmlParser.NextElement())
			{
				/* 
				 * need to go through the configuration tree, once at a leaf, take the handlerID and
				 * index into the handlers array to get the delegate to invoke for applying the 
				 * configuration
				 */
				break;
			}

			return true;
		}
	}
}
