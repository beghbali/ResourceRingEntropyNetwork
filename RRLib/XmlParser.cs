using System;
using System.Xml;
using System.Collections;
using System.IO;

namespace RRLib
{
	/// <summary>
	/// Summary description for XmlParser.
	/// </summary>
	public class XmlParser
	{
		#region private member variables
		private XmlTextReader xmlReader;
		private Hashtable attributes;
		private string nameSpace;
		private string tag;
		private string tagValue;
		#endregion

		#region constructors
		public XmlParser(string url)
		{
			xmlReader = new XmlTextReader(url);
		}

		public XmlParser(Stream stream)
		{
			xmlReader = new XmlTextReader(stream);
		}
		#endregion

		#region properties
		public Hashtable Attributes
		{
			get { return attributes; }
		}

		public string NameSpace
		{
			get { return nameSpace; }
		}

		public string Tag
		{
			get { return tag; }
		}

		public string Value
		{
			get { return tagValue; }
		}
		#endregion

		#region member methods
		/// <summary>
		/// Determines whether there are any more elements left
		/// </summary>
		/// <returns>true, if there is at least one element left to parse, false otherwise</returns>
		public bool HasElement()
		{
			return (!xmlReader.EOF);
		}

		/// <summary>
		/// Goes to the next XML element(tag). The caller can then get the value and attribute
		/// information for the element
		/// </summary>
		/// <returns>true, if there is at least one element left to parse, false otherwise</returns>
		public bool NextElement()
		{
			try
			{	
				if (xmlReader.Read())
				{
					switch (xmlReader.NodeType)
					{
						case XmlNodeType.Element:
							attributes = new Hashtable();
							nameSpace= xmlReader.NamespaceURI;
							tag= xmlReader.Name;
							tagValue = xmlReader.Value;
							if (xmlReader.HasAttributes)
							{
								for (int i = 0; i < xmlReader.AttributeCount; i++)
								{
									xmlReader.MoveToAttribute(i);
									attributes.Add(xmlReader.Name,xmlReader.Value);
								}
							}
							break;
						default:
							break;
					}
					return true;
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}

			return false;
		}

		/// <summary>
		/// Finds the XML tag name. The caller can then retreive the value and attributes
		/// </summary>
		/// <param name="name">Name of the XML tag to search</param>
		/// <returns>true if a tag designated name is found, false otherwise</returns>
		public bool FindElement(string name)
		{
			try
			{	
				while (xmlReader.Read())
				{
					switch (xmlReader.NodeType)
					{
						case XmlNodeType.Element:
							tag= xmlReader.Name;
							if (tag != name)
								break;
							attributes = new Hashtable();
							nameSpace= xmlReader.NamespaceURI;
							tagValue = xmlReader.Value;
							if (xmlReader.HasAttributes)
							{
								for (int i = 0; i < xmlReader.AttributeCount; i++)
								{
									xmlReader.MoveToAttribute(i);
									attributes.Add(xmlReader.Name,xmlReader.Value);
								}
							}
							return true;
							break;
						default:
							break;
					}
				}
			}
			catch (Exception e)
			{
				int x = 2;
			}

			attributes = null;
			nameSpace = null;
			tag = null;
			tagValue = null;

			return false;
		}
		#endregion
	}
}
