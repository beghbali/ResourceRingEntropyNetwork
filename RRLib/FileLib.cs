using System;
using System.IO;

namespace RRLib
{
	/// <summary>
	/// Library routines for accessing and manipulating files, directories and filesystem resources
	/// </summary>
	public abstract class FileLib
	{
		public static bool isASCIIDocument(FileInfo file)
		{
			foreach (string extension in Constants.ASCIIDOCS_EXTENSIONS)
			{
				if (file.Extension == extension)
					return true;
			}
			return false;
		}

		public static StreamReader openFile(FileInfo file)
		{
			StreamReader sr;

			sr = File.OpenText(file.FullName);
			if (sr == null)
				return null;
			return sr;
		}

		public static void closeFile(Stream handle)
		{
			handle.Close();
		}

		public static string readFile(FileInfo file)
		{
			StreamReader fileReader = openFile(file);
			if (fileReader == null)
				return null;
			
			string line, filetext = "";
			
			while( (line = fileReader.ReadLine()) != null)
			{
				filetext += line;
			}
			fileReader.Close();			

			return filetext;
		}

		public static int fileCountRecursive (DirectoryInfo root)
		{
			int count = 0;
			DirectoryInfo[] directories = root.GetDirectories();
			foreach (DirectoryInfo directory in directories)
			{
				count += fileCountRecursive(directory);
			}
			count += root.GetFiles().Length;
			return count;
		}

		public static string getFileExtention(Resource resource)
		{
			if (resource.data == null || !(resource.data is FileInfo))
				return resource.header.name.Substring(resource.header.name.LastIndexOf("."));

			FileInfo file = (FileInfo)resource.data;
			return file.Extension;
		}
	}
}
