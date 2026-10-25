using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using EALAModStudio;

namespace EALABuild;

public class BuildHelper
{
	private class BABProjGenerator
	{
		private static string xmlns = "urn:xmlns:ea.com:babproject";

		private static string assetXMLNS = "uri:ea.com:eala:asset";

		private static string[] streamReferences = new string[3] { "audio.xml", "global.xml", "static.xml" };

		public static string GenerateBABProjFile(string modName)
		{
			string text = StaticPaths.convertModNameToModXMLPath(modName);
			if (!File.Exists(text))
			{
				return string.Empty;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode newChild = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", "");
			xmlDocument.AppendChild(newChild);
			XmlNode xmlNode = xmlDocument.CreateNode("element", "BinaryAssetBuilderProject", xmlns);
			xmlDocument.AppendChild(xmlNode);
			CreateStreamNode(text, xmlDocument, xmlNode);
			string[] folderNames = getFolderNames(StaticPaths.convertModNametoUIFolderPath(modName));
			if (folderNames != null)
			{
				string[] array = folderNames;
				foreach (string folder in array)
				{
					AddUIStream(xmlDocument, xmlNode, folder, modName);
				}
			}
			string filename = StaticPaths.convertModNameToBABProjPath(modName);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, Encoding.UTF8);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.WriteTo(xmlTextWriter);
			xmlTextWriter.Close();
			return string.Empty;
		}

		private static XmlNode CreateStreamNode(string streamPath, XmlDocument babproj, XmlNode baseNode)
		{
			XmlNode xmlNode = babproj.CreateNode("element", "Stream", xmlns);
			XmlAttribute xmlAttribute = babproj.CreateAttribute("Source");
			xmlAttribute.Value = streamPath;
			xmlNode.Attributes.Append(xmlAttribute);
			baseNode.AppendChild(xmlNode);
			XmlNode xmlNode2 = babproj.CreateNode("element", "Configuration", xmlns);
			XmlAttribute node = babproj.CreateAttribute("Name");
			xmlNode2.Attributes.Append(node);
			XmlAttribute xmlAttribute2 = babproj.CreateAttribute("Default");
			xmlAttribute2.Value = "true";
			xmlNode2.Attributes.Append(xmlAttribute2);
			xmlNode.AppendChild(xmlNode2);
			string[] array = streamReferences;
			foreach (string text in array)
			{
				xmlNode2.AppendChild(CreateStreamReference(babproj, text, StaticPaths.convertStreamNameToManifestPath(text)));
			}
			return xmlNode;
		}

		private static XmlNode CreateStreamReference(XmlDocument babproj, string streamReferenceName, string streamReferencePath)
		{
			XmlNode xmlNode = babproj.CreateNode("element", "StreamReference", xmlns);
			XmlAttribute xmlAttribute = babproj.CreateAttribute("ReferenceName");
			xmlAttribute.Value = streamReferenceName;
			xmlNode.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = babproj.CreateAttribute("ReferenceManifest");
			xmlAttribute2.Value = streamReferencePath;
			xmlNode.Attributes.Append(xmlAttribute2);
			XmlAttribute node = babproj.CreateAttribute("ReferenceConfiguration");
			xmlNode.Attributes.Append(node);
			return xmlNode;
		}

		public static bool AddUIStream(XmlDocument babproj, XmlNode baseNode, string folder, string modName)
		{
			if (!CreateUIProxyInclude(folder, modName))
			{
				return false;
			}
			string text = folder + ".xml";
			if (!File.Exists(text))
			{
				return false;
			}
			XmlNode newChild = CreateUIStreamNode(babproj, text);
			baseNode.AppendChild(newChild);
			return true;
		}

		private static XmlNode CreateUIStreamNode(XmlDocument babproj, string streamXMLPath)
		{
			XmlNode xmlNode = babproj.CreateNode("element", "Stream", xmlns);
			XmlAttribute xmlAttribute = babproj.CreateAttribute("Source");
			xmlAttribute.Value = streamXMLPath;
			xmlNode.Attributes.Append(xmlAttribute);
			XmlNode xmlNode2 = babproj.CreateNode("element", "Configuration", xmlns);
			XmlAttribute node = babproj.CreateAttribute("Name");
			xmlNode2.Attributes.Append(node);
			xmlNode.AppendChild(xmlNode2);
			return xmlNode;
		}

		private static bool CreateUIProxyInclude(string folder, string modName)
		{
			string text = folder + ".xml";
			if (File.Exists(text))
			{
				return true;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode newChild = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", "");
			xmlDocument.AppendChild(newChild);
			XmlNode xmlNode = xmlDocument.CreateNode("element", "AssetDeclaration", assetXMLNS);
			xmlDocument.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlDocument.CreateNode("element", "Includes", assetXMLNS);
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlDocument.CreateNode("element", "Include", assetXMLNS);
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("type");
			xmlAttribute.Value = "all";
			xmlNode3.Attributes.Append(xmlAttribute);
			string[] array = folder.Split(Path.DirectorySeparatorChar);
			string text2 = array[array.Length - 1];
			string text3 = StaticPaths.convertModNameToRelativeUIFolderPath(modName);
			XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("source");
			xmlAttribute2.Value = "DATA:" + text3 + Path.DirectorySeparatorChar + text2 + Path.DirectorySeparatorChar + text2 + ".xml";
			xmlNode3.Attributes.Append(xmlAttribute2);
			xmlNode2.AppendChild(xmlNode3);
			try
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(text, Encoding.UTF8);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlDocument.WriteTo(xmlTextWriter);
				xmlTextWriter.Close();
			}
			catch (Exception ex)
			{
				DisplayLine(ex.Message);
				return false;
			}
			return true;
		}
	}

	private static int currentStep;

	private static BuildType currentType;

	private static bool resetBuildHelper = true;

	private static bool resetBuildLog = true;

	private static string buildTarget = "";

	public static bool enableBenchmarking = true;

	private static int currentBuildSteps = 0;

	private static EALABuildGUI parent;

	private static string userDataLeafName;

	private static Color criticalColor = Color.Red;

	public static Color errorColor = Color.DarkMagenta;

	public static Color warningColor = Color.DarkGoldenrod;

	public static int CurrentStep => currentStep;

	public static BuildType CurrentBuildType
	{
		get
		{
			return currentType;
		}
		set
		{
			currentType = value;
		}
	}

	public static bool AutomaticallyResetBuildHelper
	{
		get
		{
			return resetBuildHelper;
		}
		set
		{
			resetBuildHelper = value;
		}
	}

	public static bool AutomaticallyResetBuildLog
	{
		get
		{
			return resetBuildLog;
		}
		set
		{
			resetBuildLog = value;
		}
	}

	public static string BuildTarget
	{
		get
		{
			return buildTarget;
		}
		set
		{
			buildTarget = value;
		}
	}

	public static bool EnableBenchmarking
	{
		get
		{
			return enableBenchmarking;
		}
		set
		{
			enableBenchmarking = value;
		}
	}

	public static int CurrentBuildSteps
	{
		get
		{
			return currentBuildSteps;
		}
		set
		{
			currentBuildSteps = value;
		}
	}

	public static string UserDataLeafName => userDataLeafName;

	public static int[] CriticalColor
	{
		set
		{
			if (value.Length != 3)
			{
				throw new ArgumentException("Invalid color argument");
			}
			criticalColor = Color.FromArgb(value[0], value[1], value[2]);
			parent.setColors(criticalColor, errorColor, warningColor);
		}
	}

	public static int[] ErrorColor
	{
		set
		{
			if (value.Length != 3)
			{
				throw new ArgumentException("Invalid color argument");
			}
			errorColor = Color.FromArgb(value[0], value[1], value[2]);
			parent.setColors(criticalColor, errorColor, warningColor);
		}
	}

	public static int[] WarningColor
	{
		set
		{
			if (value.Length != 3)
			{
				throw new ArgumentException("Invalid color argument");
			}
			warningColor = Color.FromArgb(value[0], value[1], value[2]);
			parent.setColors(criticalColor, errorColor, warningColor);
		}
	}

	public static void StartBuild(BuildType typeOfBuild, string target)
	{
		if (AutomaticallyResetBuildHelper)
		{
			Reset();
			CurrentBuildType = typeOfBuild;
			buildTarget = target;
		}
		if (AutomaticallyResetBuildLog)
		{
			parent.resetBuildLog();
		}
		BABProjGenerator.GenerateBABProjFile(target);
	}

	public static void DisplayLine(string line)
	{
		parent.displayLine(line);
	}

	public static void Reset()
	{
		currentStep = 1;
	}

	public static void RunStep(StepType typeOfStep, object args)
	{
		parent.runBuildStep(currentStep++, typeOfStep, args);
	}

	public static void SkipStep()
	{
		currentStep++;
	}

	public static void OnBuildComplete()
	{
		parent.onBuildComplete();
	}

	public static int GetNextExecutableStep()
	{
		while (!parent.getScriptInterface().shouldExecuteStep(currentStep))
		{
			currentStep++;
			if (IsBuildComplete())
			{
				currentStep = -1;
				break;
			}
		}
		if (IsBuildComplete())
		{
			currentStep = -1;
		}
		return currentStep;
	}

	public static bool IsBuildComplete()
	{
		if (currentStep > currentBuildSteps)
		{
			return true;
		}
		return false;
	}

	public static void setBuildGUI(EALABuildGUI gui)
	{
		if (gui != null && parent == null)
		{
			parent = gui;
			parent.setColors(criticalColor, errorColor, warningColor);
		}
	}

	public static void setUserDataLeafName(string folder)
	{
		if (userDataLeafName == null && folder != null)
		{
			userDataLeafName = folder;
			StaticPaths.UserDataLeaf = folder;
		}
	}

	public static string[] getFolderNames(string directory)
	{
		if (Directory.Exists(directory))
		{
			return Directory.GetDirectories(directory);
		}
		return null;
	}
}
