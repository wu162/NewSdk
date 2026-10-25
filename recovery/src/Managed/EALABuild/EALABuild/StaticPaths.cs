using System;
using System.IO;

namespace EALABuild;

public class StaticPaths
{
	private static string DATA_STRING = "data";

	private static string TOOLS_STRING = "tools";

	private static string BUILT_MODS_STRING = "BuiltMods";

	private static string SHADERS_STRING = "Shaders";

	private static string APT_UI_STRING = "aptui";

	private static string MOD_INCLUDE_FILE = "mod.xml";

	private static string SAGEXML = "sagexml";

	private static string MOD_FOLDER_PATH = "Mods";

	private static string MOD_PATH_FOLDER = Path.DirectorySeparatorChar + DATA_STRING;

	private static string MOD_PATH_ADDON = Path.DirectorySeparatorChar + "mod.babproj";

	private static string MAKE_BIG = TOOLS_STRING + Path.DirectorySeparatorChar + "Makebig.exe";

	private static string BINARY_ASSET_BUILDER = TOOLS_STRING + Path.DirectorySeparatorChar + "BinaryAssetBuilder.exe";

	private static string BUILTMODS_OUTPUT = Path.DirectorySeparatorChar + BUILT_MODS_STRING;

	private static string BIG_FILE_EXTENSION = ".big";

	private static string SKUDEF_EXTENSION = ".skudef";

	private static string MANIFEST_EXTENSION = ".manifest";

	private static string USER_DATA_LEAF = "";

	public static string ModFolderPath => MOD_FOLDER_PATH;

	public static string BinaryAssetBuilderPath => BINARY_ASSET_BUILDER;

	public static string MakeBigPath => MAKE_BIG;

	public static string BuiltModsPath => CurrentDirectory + BUILTMODS_OUTPUT;

	public static string CurrentDirectory => Environment.CurrentDirectory;

	public static string DirectorySeparator => Path.DirectorySeparatorChar.ToString();

	public static string UserDataLeaf
	{
		get
		{
			return USER_DATA_LEAF;
		}
		set
		{
			USER_DATA_LEAF = value;
		}
	}

	public static string convertStreamNameToManifestPath(string streamName)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(streamName);
		string currentDirectory = Environment.CurrentDirectory;
		object obj = currentDirectory;
		return string.Concat(obj, BUILTMODS_OUTPUT, Path.DirectorySeparatorChar, SAGEXML, Path.DirectorySeparatorChar, fileNameWithoutExtension, MANIFEST_EXTENSION);
	}

	public static string convertModNameToRelativeDataPath(string modName)
	{
		return modName + MOD_PATH_FOLDER;
	}

	public static string convertModNameToBaseDataPath(string modName)
	{
		string currentDirectory = Environment.CurrentDirectory;
		object obj = currentDirectory;
		return string.Concat(obj, Path.DirectorySeparatorChar, MOD_FOLDER_PATH, Path.DirectorySeparatorChar, modName);
	}

	public static string convertModNameToBABProjPath(string modName)
	{
		string text = convertModNameToBaseDataPath(modName);
		return text + MOD_PATH_ADDON;
	}

	public static string convertModNameToModXMLPath(string modName)
	{
		string text = convertModNameToFullDataPath(modName);
		return text + Path.DirectorySeparatorChar + MOD_INCLUDE_FILE;
	}

	public static string convertModNameToFullDataPath(string modName)
	{
		string text = convertModNameToBaseDataPath(modName);
		return text + MOD_PATH_FOLDER;
	}

	public static string convertModNametoUIFolderPath(string modName)
	{
		string text = convertModNameToFullDataPath(modName);
		return text + Path.DirectorySeparatorChar + APT_UI_STRING;
	}

	public static string convertModNameToRelativeUIFolderPath(string modName)
	{
		string text = convertModNameToRelativeDataPath(modName);
		return text + Path.DirectorySeparatorChar + APT_UI_STRING;
	}

	public static string getBuiltModPath(string modName)
	{
		string currentDirectory = CurrentDirectory;
		object obj = currentDirectory;
		return string.Concat(obj, BUILTMODS_OUTPUT, Path.DirectorySeparatorChar, MOD_FOLDER_PATH, Path.DirectorySeparatorChar, modName);
	}

	public static string getBuiltModShaderPath(string modName)
	{
		string builtModPath = getBuiltModPath(modName);
		return builtModPath + Path.DirectorySeparatorChar + SHADERS_STRING;
	}

	public static string getBuiltModDataPath(string modName)
	{
		string builtModPath = getBuiltModPath(modName);
		return builtModPath + Path.DirectorySeparatorChar + DATA_STRING;
	}

	public static string getBuiltModManifestPath(string modName)
	{
		string builtModPath = getBuiltModPath(modName);
		object obj = builtModPath;
		return string.Concat(obj, Path.DirectorySeparatorChar, DATA_STRING, Path.DirectorySeparatorChar, "mod.manifest");
	}

	public static string getBuiltModBigPath(string modName)
	{
		string builtModPath = getBuiltModPath(modName);
		return builtModPath + BIG_FILE_EXTENSION;
	}

	public static string getModInstallPath(string modName)
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		object obj = folderPath;
		return string.Concat(obj, Path.DirectorySeparatorChar, BuildHelper.UserDataLeafName, Path.DirectorySeparatorChar, MOD_FOLDER_PATH, Path.DirectorySeparatorChar, modName);
	}

	public static string getModInstallBigFilePath(string modName)
	{
		string modInstallPath = getModInstallPath(modName);
		object obj = modInstallPath;
		return string.Concat(obj, Path.DirectorySeparatorChar, modName, BIG_FILE_EXTENSION);
	}

	public static string getModInstallSKUDefPath(string modName, string modVersion)
	{
		string modInstallPath = getModInstallPath(modName);
		object obj = modInstallPath;
		return string.Concat(obj, Path.DirectorySeparatorChar, modName, "_", modVersion, SKUDEF_EXTENSION);
	}
}
