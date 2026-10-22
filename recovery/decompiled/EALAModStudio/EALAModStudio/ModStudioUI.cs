using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using EALABuild;
using Microsoft.CSharp;
using Microsoft.Win32;

namespace EALAModStudio;

public class ModStudioUI : Form, EALABuildGUI
{
	private delegate void FormInvokeDelegate();

	private delegate void WriteDataDelegate(string data);

	private IContainer components;

	private SplitContainer splitContainer1;

	private FlowLayoutPanel flowLayoutPanel1;

	private GroupBox grpModBuild;

	private FlowLayoutPanel flwModOptions;

	private ComboBox cmbMods;

	private GroupBox grpModBuildOptions;

	private Button buttonBuildMod;

	private SaveFileDialog saveBuildLog;

	private TabControl outputTabs;

	private TabPage tabPage1;

	private SplitContainer splitContainer2;

	private RichTextBox rtbGUIOutput;

	private Button buttonSaveBuildLog;

	private Button clearBuildLog;

	private TabPage tabPage2;

	private TreeView tvErrorView;

	private TabPage tabPage3;

	private WebBrowser brwsrDocumentation;

	private string buildscript = "defaultscript.cs";

	private string guiserializationextension = ".guidata";

	private EALABuildScript buildScriptInstance;

	private List<GUIElementData> inputModGUIData;

	private Dictionary<string, Control> generatedModGUIData = new Dictionary<string, Control>();

	private TreeNode tnCritical;

	private static int CRITICAL_IMAGE_INDEX = 0;

	private TreeNode tnError;

	private static int ERROR_IMAGE_INDEX = 1;

	private TreeNode tnWarning;

	private static int WARNING_IMAGE_INDEX = 2;

	private TableLayoutPanel modGUIPanel;

	private int currentStepId = -1;

	private Process currentRunningProcess;

	private Thread currentProcessOutputReader;

	private bool processRunning;

	private bool redirectOutput;

	private bool stepFailed;

	private int leftPanelWidth;

	private Color criticalColor;

	private Color errorColor;

	private Color warningColor;

	private long BuildTimeStart;

	private long StepTimeStart;

	private bool raiseDeleteErrors;

	private bool raiseCopyError;

	private bool overwrite = true;

	private static int MAX_READ_LINES = 15;

	private static int PROCESS_SLEEP_DELAY = 125;

	private static int MIN_LEFT_WIDTH = 170;

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EALAModStudio.ModStudioUI));
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
		this.grpModBuild = new System.Windows.Forms.GroupBox();
		this.flwModOptions = new System.Windows.Forms.FlowLayoutPanel();
		this.cmbMods = new System.Windows.Forms.ComboBox();
		this.grpModBuildOptions = new System.Windows.Forms.GroupBox();
		this.buttonBuildMod = new System.Windows.Forms.Button();
		this.outputTabs = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.splitContainer2 = new System.Windows.Forms.SplitContainer();
		this.rtbGUIOutput = new System.Windows.Forms.RichTextBox();
		this.buttonSaveBuildLog = new System.Windows.Forms.Button();
		this.clearBuildLog = new System.Windows.Forms.Button();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.tvErrorView = new System.Windows.Forms.TreeView();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.brwsrDocumentation = new System.Windows.Forms.WebBrowser();
		this.saveBuildLog = new System.Windows.Forms.SaveFileDialog();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.flowLayoutPanel1.SuspendLayout();
		this.grpModBuild.SuspendLayout();
		this.flwModOptions.SuspendLayout();
		this.outputTabs.SuspendLayout();
		this.tabPage1.SuspendLayout();
		this.splitContainer2.Panel1.SuspendLayout();
		this.splitContainer2.Panel2.SuspendLayout();
		this.splitContainer2.SuspendLayout();
		this.tabPage2.SuspendLayout();
		this.tabPage3.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
		this.splitContainer1.Panel2.Controls.Add(this.outputTabs);
		this.splitContainer1.Size = new System.Drawing.Size(481, 407);
		this.splitContainer1.SplitterDistance = 159;
		this.splitContainer1.TabIndex = 1;
		this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(splitContainer1_SplitterMoved);
		this.flowLayoutPanel1.Controls.Add(this.grpModBuild);
		this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(159, 407);
		this.flowLayoutPanel1.Name = "flowLayoutPanel1";
		this.flowLayoutPanel1.Size = new System.Drawing.Size(159, 407);
		this.flowLayoutPanel1.TabIndex = 0;
		this.grpModBuild.AutoSize = true;
		this.grpModBuild.Controls.Add(this.flwModOptions);
		this.grpModBuild.Location = new System.Drawing.Point(3, 3);
		this.grpModBuild.Name = "grpModBuild";
		this.grpModBuild.Size = new System.Drawing.Size(153, 158);
		this.grpModBuild.TabIndex = 7;
		this.grpModBuild.TabStop = false;
		this.grpModBuild.Text = "Mod Build";
		this.flwModOptions.Controls.Add(this.cmbMods);
		this.flwModOptions.Controls.Add(this.grpModBuildOptions);
		this.flwModOptions.Controls.Add(this.buttonBuildMod);
		this.flwModOptions.Location = new System.Drawing.Point(6, 19);
		this.flwModOptions.Name = "flwModOptions";
		this.flwModOptions.Size = new System.Drawing.Size(141, 120);
		this.flwModOptions.TabIndex = 9;
		this.cmbMods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbMods.FormattingEnabled = true;
		this.cmbMods.Location = new System.Drawing.Point(3, 3);
		this.cmbMods.Name = "cmbMods";
		this.cmbMods.Size = new System.Drawing.Size(138, 21);
		this.cmbMods.TabIndex = 11;
		this.grpModBuildOptions.Location = new System.Drawing.Point(3, 30);
		this.grpModBuildOptions.Name = "grpModBuildOptions";
		this.grpModBuildOptions.Size = new System.Drawing.Size(138, 30);
		this.grpModBuildOptions.TabIndex = 9;
		this.grpModBuildOptions.TabStop = false;
		this.grpModBuildOptions.Text = "Mod Build Options";
		this.buttonBuildMod.Location = new System.Drawing.Point(3, 66);
		this.buttonBuildMod.Name = "buttonBuildMod";
		this.buttonBuildMod.Size = new System.Drawing.Size(138, 30);
		this.buttonBuildMod.TabIndex = 10;
		this.buttonBuildMod.Text = "Build Mod";
		this.buttonBuildMod.UseVisualStyleBackColor = true;
		this.buttonBuildMod.Click += new System.EventHandler(buttonBuildMod_Click);
		this.outputTabs.Controls.Add(this.tabPage1);
		this.outputTabs.Controls.Add(this.tabPage2);
		this.outputTabs.Controls.Add(this.tabPage3);
		this.outputTabs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.outputTabs.Location = new System.Drawing.Point(0, 0);
		this.outputTabs.Name = "outputTabs";
		this.outputTabs.SelectedIndex = 0;
		this.outputTabs.Size = new System.Drawing.Size(318, 407);
		this.outputTabs.TabIndex = 0;
		this.tabPage1.Controls.Add(this.splitContainer2);
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(310, 381);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "Build Log";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
		this.splitContainer2.IsSplitterFixed = true;
		this.splitContainer2.Location = new System.Drawing.Point(3, 3);
		this.splitContainer2.Name = "splitContainer2";
		this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer2.Panel1.Controls.Add(this.rtbGUIOutput);
		this.splitContainer2.Panel2.Controls.Add(this.buttonSaveBuildLog);
		this.splitContainer2.Panel2.Controls.Add(this.clearBuildLog);
		this.splitContainer2.Size = new System.Drawing.Size(304, 375);
		this.splitContainer2.SplitterDistance = 340;
		this.splitContainer2.TabIndex = 8;
		this.rtbGUIOutput.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rtbGUIOutput.Location = new System.Drawing.Point(0, 0);
		this.rtbGUIOutput.Name = "rtbGUIOutput";
		this.rtbGUIOutput.ReadOnly = true;
		this.rtbGUIOutput.Size = new System.Drawing.Size(304, 340);
		this.rtbGUIOutput.TabIndex = 1;
		this.rtbGUIOutput.Text = "";
		this.buttonSaveBuildLog.Location = new System.Drawing.Point(36, 2);
		this.buttonSaveBuildLog.Name = "buttonSaveBuildLog";
		this.buttonSaveBuildLog.Size = new System.Drawing.Size(101, 28);
		this.buttonSaveBuildLog.TabIndex = 1;
		this.buttonSaveBuildLog.Text = "Save Build Log";
		this.buttonSaveBuildLog.UseVisualStyleBackColor = true;
		this.buttonSaveBuildLog.Click += new System.EventHandler(buttonSaveBuildLog_Click);
		this.clearBuildLog.Location = new System.Drawing.Point(181, 2);
		this.clearBuildLog.Name = "clearBuildLog";
		this.clearBuildLog.Size = new System.Drawing.Size(89, 28);
		this.clearBuildLog.TabIndex = 0;
		this.clearBuildLog.Text = "Clear Build Log";
		this.clearBuildLog.UseVisualStyleBackColor = true;
		this.clearBuildLog.Click += new System.EventHandler(clearBuildLog_Click);
		this.tabPage2.Controls.Add(this.tvErrorView);
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(310, 381);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "Error Log";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.tvErrorView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tvErrorView.Location = new System.Drawing.Point(3, 3);
		this.tvErrorView.Name = "tvErrorView";
		this.tvErrorView.Size = new System.Drawing.Size(304, 375);
		this.tvErrorView.TabIndex = 0;
		this.tvErrorView.DoubleClick += new System.EventHandler(tvErrorView_DoubleClick);
		this.tabPage3.Controls.Add(this.brwsrDocumentation);
		this.tabPage3.Location = new System.Drawing.Point(4, 22);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(310, 381);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "Documentation";
		this.tabPage3.UseVisualStyleBackColor = true;
		this.brwsrDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
		this.brwsrDocumentation.Location = new System.Drawing.Point(3, 3);
		this.brwsrDocumentation.MinimumSize = new System.Drawing.Size(20, 20);
		this.brwsrDocumentation.Name = "brwsrDocumentation";
		this.brwsrDocumentation.Size = new System.Drawing.Size(304, 375);
		this.brwsrDocumentation.TabIndex = 0;
		this.saveBuildLog.FileName = "buildlog.txt";
		this.saveBuildLog.Filter = "Text file|*.txt";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(481, 407);
		base.Controls.Add(this.splitContainer1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "ModStudioUI";
		this.Text = "Red Alert 3 Mod BuildStudio";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
		base.Load += new System.EventHandler(Form1_Load);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		this.splitContainer1.ResumeLayout(false);
		this.flowLayoutPanel1.ResumeLayout(false);
		this.flowLayoutPanel1.PerformLayout();
		this.grpModBuild.ResumeLayout(false);
		this.flwModOptions.ResumeLayout(false);
		this.outputTabs.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.splitContainer2.Panel1.ResumeLayout(false);
		this.splitContainer2.Panel2.ResumeLayout(false);
		this.splitContainer2.ResumeLayout(false);
		this.tabPage2.ResumeLayout(false);
		this.tabPage3.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	public ModStudioUI(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].Equals("-script", StringComparison.CurrentCultureIgnoreCase) && i + 1 < args.Length)
			{
				buildscript = args[++i];
			}
		}
		InitializeComponent();
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (buildScriptInstance != null)
			{
				Dictionary<string, object> graph = generateGUIResult(inputModGUIData, generatedModGUIData);
				string path = buildscript + guiserializationextension;
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				SHA1 sHA = new SHA1CryptoServiceProvider();
				FileStream fileStream = new FileStream(buildscript, FileMode.Open, FileAccess.Read);
				byte[] graph2 = sHA.ComputeHash(fileStream);
				fileStream.Close();
				FileStream fileStream2 = new FileStream(path, FileMode.Create);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream2, graph2);
				binaryFormatter.Serialize(fileStream2, graph);
				fileStream2.Close();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		Dictionary<string, object> storedGUIData = null;
		try
		{
			try
			{
				BuildHelper.setBuildGUI(this);
				loadBuildScript();
				string path = buildscript + guiserializationextension;
				if (File.Exists(path))
				{
					SHA1 sHA = new SHA1CryptoServiceProvider();
					FileStream fileStream = new FileStream(buildscript, FileMode.Open, FileAccess.Read);
					byte[] array = sHA.ComputeHash(fileStream);
					fileStream.Close();
					FileStream fileStream2 = new FileStream(path, FileMode.Open);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					byte[] array2 = (byte[])binaryFormatter.Deserialize(fileStream2);
					if (array2.Length == array.Length)
					{
						bool flag = true;
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i] != array[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							storedGUIData = (Dictionary<string, object>)binaryFormatter.Deserialize(fileStream2);
						}
					}
					fileStream2.Close();
				}
				buildScriptInstance.initialize();
			}
			catch (Exception ex)
			{
				rtbGUIOutput.AppendText(ex.Message);
			}
			try
			{
				ImageList imageList = new ImageList();
				Icon icon = new Icon(SystemIcons.Error, new Size(32, 32));
				imageList.Images.Add(icon.ToBitmap());
				Icon icon2 = new Icon(SystemIcons.Exclamation, new Size(32, 32));
				imageList.Images.Add(icon2.ToBitmap());
				Icon icon3 = new Icon(SystemIcons.Information, new Size(32, 32));
				imageList.Images.Add(icon3.ToBitmap());
				tvErrorView.ImageList = imageList;
				tnCritical = new TreeNode();
				tnCritical.Text = "Critical Errors";
				tnCritical.ImageIndex = CRITICAL_IMAGE_INDEX;
				tnCritical.SelectedImageIndex = CRITICAL_IMAGE_INDEX;
				tvErrorView.Nodes.Add(tnCritical);
				tnError = new TreeNode();
				tnError.Text = "Errors";
				tnError.ImageIndex = ERROR_IMAGE_INDEX;
				tnError.SelectedImageIndex = ERROR_IMAGE_INDEX;
				tvErrorView.Nodes.Add(tnError);
				tnWarning = new TreeNode();
				tnWarning.Text = "Warnings";
				tnWarning.ImageIndex = WARNING_IMAGE_INDEX;
				tnWarning.SelectedImageIndex = WARNING_IMAGE_INDEX;
				tvErrorView.Nodes.Add(tnWarning);
				leftPanelWidth = splitContainer1.Panel1.Width;
			}
			catch (Exception ex2)
			{
				rtbGUIOutput.AppendText(ex2.Message);
			}
			try
			{
				loadUserDataLeafName();
				loadModNames();
			}
			catch (Exception ex3)
			{
				rtbGUIOutput.AppendText(ex3.Message);
			}
			int num = 50;
			try
			{
				inputModGUIData = buildScriptInstance.GetModBuildGUIData();
				modGUIPanel = new TableLayoutPanel();
				modGUIPanel.ColumnCount = 1;
				modGUIPanel.Width = grpModBuildOptions.Width - 10;
				modGUIPanel.Left = 5;
				modGUIPanel.Top = 15;
				generatedModGUIData = generateGUIElements(modGUIPanel, inputModGUIData, storedGUIData);
				int num2 = 15;
				foreach (Control control in modGUIPanel.Controls)
				{
					num2 += control.Height + control.Margin.Vertical;
				}
				modGUIPanel.Height = num2;
				grpModBuildOptions.Height = num2 + modGUIPanel.Margin.Vertical + modGUIPanel.Top;
				flwModOptions.Height = grpModBuildOptions.Height + buttonBuildMod.Height + cmbMods.Height + 20;
				grpModBuildOptions.Controls.Add(modGUIPanel);
				num += grpModBuild.Height;
			}
			catch (Exception ex4)
			{
				rtbGUIOutput.AppendText(ex4.Message);
			}
			brwsrDocumentation.AllowNavigation = true;
			brwsrDocumentation.AllowWebBrowserDrop = false;
			brwsrDocumentation.Navigate("file://" + Environment.CurrentDirectory + "/Documentation/index.htm");
			if (num > 50)
			{
				base.Height = num;
			}
		}
		catch (Exception ex5)
		{
			MessageBox.Show("Uncaught Exception: " + ex5.Message);
		}
	}

	private Dictionary<string, Control> generateGUIElements(TableLayoutPanel panel, List<GUIElementData> data, Dictionary<string, object> storedGUIData)
	{
		Dictionary<string, Control> dictionary = new Dictionary<string, Control>();
		foreach (GUIElementData datum in data)
		{
			Control control = null;
			control = ((storedGUIData == null || !storedGUIData.ContainsKey(datum.DataKey)) ? generateControl(datum.LabelString, datum.ControlType) : generateControl(datum.LabelString, datum.ControlType, storedGUIData[datum.DataKey]));
			if (datum.ControlType != GUIElementData.LabelType)
			{
				dictionary.Add(datum.DataKey, control);
			}
			panel.Controls.Add(control);
		}
		return dictionary;
	}

	private Control generateControl(string name, Type controlType, object controlStatus)
	{
		Control control = generateControl(name, controlType);
		if (controlType == GUIElementData.CheckBoxType)
		{
			((CheckBox)control).Checked = (bool)controlStatus;
		}
		return control;
	}

	private Control generateControl(string name, Type controlType)
	{
		if (controlType == GUIElementData.CheckBoxType)
		{
			CheckBox checkBox = new CheckBox();
			checkBox.Text = name;
			checkBox.AutoSize = true;
			return checkBox;
		}
		if (controlType == GUIElementData.InputBoxType)
		{
			TextBox textBox = new TextBox();
			textBox.Text = name;
			textBox.AutoSize = true;
			return textBox;
		}
		if (controlType == GUIElementData.LabelType)
		{
			Label label = new Label();
			label.Text = name;
			label.AutoSize = true;
			return label;
		}
		return null;
	}

	private Dictionary<string, object> generateGUIResult(List<GUIElementData> inputData, Dictionary<string, Control> guiData)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (GUIElementData inputDatum in inputData)
		{
			object obj = retrieveGUIElementResult(inputDatum, guiData);
			if (obj != null)
			{
				dictionary.Add(inputDatum.DataKey, obj);
			}
		}
		return dictionary;
	}

	private object retrieveGUIElementResult(GUIElementData inputElement, Dictionary<string, Control> guiData)
	{
		if (inputElement.ControlType == GUIElementData.LabelType)
		{
			return null;
		}
		if (inputElement.ControlType == GUIElementData.CheckBoxType)
		{
			return ((CheckBox)guiData[inputElement.DataKey]).Checked;
		}
		if (inputElement.ControlType == GUIElementData.InputBoxType)
		{
			return ((TextBox)guiData[inputElement.DataKey]).Text;
		}
		return null;
	}

	private void loadBuildScript()
	{
		CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
		string[] assemblyNames = new string[2] { "System.dll", "EALABuild.dll" };
		CompilerParameters compilerParameters = new CompilerParameters(assemblyNames);
		compilerParameters.GenerateExecutable = false;
		compilerParameters.GenerateInMemory = true;
		CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(compilerParameters, buildscript);
		if (compilerResults.Errors.Count > 0)
		{
			foreach (CompilerError error in compilerResults.Errors)
			{
				displayLine(error.ErrorText);
			}
		}
		else
		{
			Type[] types = compilerResults.CompiledAssembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type[] interfaces = types[i].GetInterfaces();
				for (int j = 0; j < interfaces.Length; j++)
				{
					if (interfaces[j] == typeof(EALABuildScript))
					{
						buildScriptInstance = (EALABuildScript)types[i].GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
					}
				}
			}
		}
		if (buildScriptInstance == null)
		{
			throw new InvalidOperationException("No type found in " + buildscript + " which implements interface EALABuildScript");
		}
	}

	private void loadModNames()
	{
		if (Directory.Exists(StaticPaths.ModFolderPath))
		{
			string[] directories = Directory.GetDirectories(StaticPaths.ModFolderPath);
			string[] array = directories;
			foreach (string text in array)
			{
				string item = text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1);
				cmbMods.Items.Add(item);
			}
		}
	}

	private void loadUserDataLeafName()
	{
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Electronic Arts\\Electronic Arts\\Red Alert 3");
		if (registryKey != null)
		{
			string text = (string)registryKey.GetValue("UserDataLeafName", string.Empty);
			if (text != string.Empty)
			{
				BuildHelper.setUserDataLeafName(text);
			}
		}
	}

	public EALABuildScript getScriptInterface()
	{
		return buildScriptInstance;
	}

	public void runBuildStep(int stepId, StepType typeOfStep, object args)
	{
		if (BuildHelper.EnableBenchmarking)
		{
			StepTimeStart = DateTime.UtcNow.Ticks;
		}
		currentStepId = stepId;
		stepFailed = false;
		if (args == null)
		{
			displayLine("Invalid arguments to runBuildStep, null argument");
			onStepFailure();
		}
		switch (typeOfStep)
		{
		case StepType.RunExecutable:
		{
			RunExecutableArguments args2 = (RunExecutableArguments)args;
			if (!launchExecutable(args2))
			{
				onStepFailure();
			}
			break;
		}
		case StepType.CopyFile:
			copyFilesImpl((CopyFileArguments)args);
			break;
		case StepType.CopyFileList:
			copyFileListImpl((CopyFileListArguments)args);
			break;
		case StepType.DeleteFiles:
			deleteFilesImpl((DeleteFileArguments)args);
			break;
		case StepType.CreateDirectory:
			createDirectoryImpl((CreateDirectoryArguments)args);
			break;
		case StepType.WriteFile:
			writeFileImpl((WriteFileArguments)args);
			break;
		default:
			throw new InvalidOperationException("This operation has not been implemented.");
		}
	}

	public void displayLine(string text)
	{
		rtbGUIOutput.AppendText(text + "\n");
		rtbGUIOutput.ScrollToCaret();
	}

	public void displayGUILine(string text)
	{
		rtbGUIOutput.SelectionColor = Color.Green;
		rtbGUIOutput.AppendText(text + "\n");
		rtbGUIOutput.ScrollToCaret();
		rtbGUIOutput.SelectionColor = Color.Black;
	}

	public void resetBuildLog()
	{
		rtbGUIOutput.Clear();
		tnCritical.Nodes.Clear();
		tnWarning.Nodes.Clear();
		tnError.Nodes.Clear();
	}

	public void onBuildComplete()
	{
		displayGUILine("Build Completed");
		if (BuildHelper.EnableBenchmarking)
		{
			long ticks = DateTime.UtcNow.Ticks;
			long value = ticks - BuildTimeStart;
			displayGUILine("Build time taken: " + TimeSpan.FromTicks(value).ToString());
		}
	}

	public void setColors(Color critical, Color error, Color warning)
	{
		criticalColor = critical;
		errorColor = error;
		warningColor = warning;
	}

	public void createDirectoryImpl(CreateDirectoryArguments args)
	{
		try
		{
			if (!Directory.Exists(args.Path))
			{
				Directory.CreateDirectory(args.Path);
				onStepSuccess();
			}
		}
		catch (Exception ex)
		{
			displayLine(ex.Message);
			onStepFailure();
		}
	}

	private void deleteFilesImpl(DeleteFileArguments args)
	{
		raiseDeleteErrors = args.RaiseErrors;
		if (args.Recursive && args.Filter != string.Empty)
		{
			deleteFilesRecursiveFilteredImpl(args.Path, args.Filter);
		}
		else if (args.Recursive)
		{
			deleteFilesRecursiveImpl(args.Path);
		}
		else if (args.Filter != string.Empty)
		{
			deleteFilesFilteredImpl(args.Path, args.Filter);
		}
		else
		{
			deleteFileImpl(args.Path);
		}
		if (!stepFailed)
		{
			onStepSuccess();
		}
	}

	private void deleteFilesRecursiveFilteredImpl(string target, string filter)
	{
		displayDebugLine("Delete files recursive filtered: " + target + "," + filter);
		if (Directory.Exists(target))
		{
			try
			{
				deleteFilesFilteredImpl(target, filter);
				if (stepFailed)
				{
					return;
				}
				string[] directories = Directory.GetDirectories(target);
				string[] array = directories;
				foreach (string target2 in array)
				{
					deleteFilesFilteredImpl(target2, filter);
					if (stepFailed)
					{
						break;
					}
					deleteFilesRecursiveFilteredImpl(target2, filter);
					if (stepFailed)
					{
						break;
					}
				}
				return;
			}
			catch (Exception ex)
			{
				deleteFilesErrorImpl(ex.Message);
			}
		}
		deleteFilesErrorImpl("Invalid directory specified:" + target);
	}

	private void deleteFilesFilteredImpl(string target, string filter)
	{
		displayDebugLine("Delete files filtered: " + target + "," + filter);
		if (Directory.Exists(target))
		{
			try
			{
				string[] files = Directory.GetFiles(target, filter);
				string[] array = files;
				foreach (string text in array)
				{
					if (stepFailed)
					{
						break;
					}
					try
					{
						File.Delete(text);
						displayLine("Deleted File: " + text);
					}
					catch (Exception ex)
					{
						deleteFilesErrorImpl(ex.Message);
					}
				}
				return;
			}
			catch (Exception ex2)
			{
				deleteFilesErrorImpl(ex2.Message);
			}
		}
		deleteFilesErrorImpl("Invalid folder: " + target);
	}

	private void deleteFilesRecursiveImpl(string target)
	{
		displayDebugLine("Delete folder: " + target);
		if (Directory.Exists(target))
		{
			try
			{
				Directory.Delete(target, recursive: true);
				return;
			}
			catch (Exception ex)
			{
				deleteFilesErrorImpl(ex.Message);
				return;
			}
		}
		deleteFilesErrorImpl("Invalid directory specified:" + target);
	}

	private void deleteFileImpl(string target)
	{
		displayDebugLine("Delete file: " + target);
		if (File.Exists(target))
		{
			try
			{
				File.Delete(target);
				return;
			}
			catch (Exception ex)
			{
				deleteFilesErrorImpl(ex.Message);
			}
		}
		deleteFilesErrorImpl("File: " + target + "doesn't exist or is a directory");
	}

	private void deleteFilesErrorImpl(string message)
	{
		displayLine(message);
		if (raiseDeleteErrors)
		{
			onStepFailure();
		}
	}

	private void copyFileImpl(string file, string dest)
	{
		displayDebugLine("Copy file: " + file + " to: " + dest);
		if (File.Exists(file))
		{
			if (File.Exists(dest) && !overwrite)
			{
				copyFilesErrorImpl("Destination file exists and cannot be overwritten" + dest);
				return;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(dest);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
					displayLine("Directory created: " + directoryName);
				}
				File.Copy(file, dest, overwrite);
				displayLine("File copied: " + file + " to " + dest);
				return;
			}
			catch (Exception ex)
			{
				copyFilesErrorImpl(ex.Message);
			}
		}
		copyFilesErrorImpl("No file found: " + file);
	}

	private void copyFilesRecursiveImpl(string path, string dest)
	{
		displayDebugLine($"CopyFilesRecursive( {path} , {dest} )");
		if (Directory.Exists(path))
		{
			try
			{
				string[] directories = Directory.GetDirectories(path);
				string[] array = directories;
				foreach (string text in array)
				{
					string path2 = text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1);
					if (!Path.IsPathRooted(dest))
					{
						dest = Path.Combine(Environment.CurrentDirectory, dest);
					}
					string text2 = Path.Combine(dest, path2);
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
						displayLine("Created folder: " + text2);
					}
					copyFilesRecursiveImpl(text, text2);
					if (stepFailed)
					{
						return;
					}
				}
				string[] files = Directory.GetFiles(path);
				string[] array2 = files;
				foreach (string text3 in array2)
				{
					string dest2 = Path.Combine(dest, Path.GetFileName(text3));
					copyFileImpl(text3, dest2);
					if (stepFailed)
					{
						break;
					}
				}
				return;
			}
			catch (Exception ex)
			{
				copyFilesErrorImpl(ex.Message);
				return;
			}
		}
		copyFilesErrorImpl("Invalid directory specified:" + path);
	}

	private void copyFilesFilteredImpl(string path, string pattern, string dest)
	{
		displayDebugLine($"CopyFilesFiltered( {path} , {pattern} , {dest} )");
		if (Directory.Exists(path))
		{
			try
			{
				string[] files = Directory.GetFiles(path, pattern);
				string[] array = files;
				foreach (string text in array)
				{
					string dest2 = Path.Combine(dest, Path.GetFileName(text));
					copyFileImpl(text, dest2);
					if (stepFailed)
					{
						break;
					}
				}
				return;
			}
			catch (Exception ex)
			{
				copyFilesErrorImpl(ex.Message);
				return;
			}
		}
		copyFilesErrorImpl("Invalid directory specified:" + path);
	}

	private void copyFilesRecursiveFilteredImpl(string path, string pattern, string dest)
	{
		displayDebugLine($"CopyFilesRecursiveFiltered( {path} , {pattern} , {dest} )");
		if (Directory.Exists(path))
		{
			try
			{
				string[] directories = Directory.GetDirectories(path);
				string[] array = directories;
				foreach (string text in array)
				{
					string path2 = text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1);
					if (!Path.IsPathRooted(dest))
					{
						dest = Path.Combine(Environment.CurrentDirectory, dest);
					}
					string text2 = Path.Combine(dest, path2);
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
						displayLine("Created folder: " + text2);
					}
					copyFilesRecursiveFilteredImpl(text, pattern, text2);
					if (stepFailed)
					{
						return;
					}
				}
				string[] files = Directory.GetFiles(path, pattern);
				string[] array2 = files;
				foreach (string text3 in array2)
				{
					string dest2 = Path.Combine(dest, Path.GetFileName(text3));
					copyFileImpl(text3, dest2);
					if (stepFailed)
					{
						break;
					}
				}
				return;
			}
			catch (Exception ex)
			{
				copyFilesErrorImpl(ex.Message);
				return;
			}
		}
		copyFilesErrorImpl("Invalid directory specified:" + path);
	}

	private void copyFilesImpl(CopyFileArguments args)
	{
		raiseCopyError = args.RaiseErrors;
		overwrite = args.Overwrite;
		if (args.Recursive && args.Filter != string.Empty)
		{
			copyFilesRecursiveFilteredImpl(args.TargetFiles, args.Filter, args.DestinationPath);
		}
		else if (args.Recursive)
		{
			copyFilesRecursiveImpl(args.TargetFiles, args.DestinationPath);
		}
		else if (args.Filter != string.Empty)
		{
			copyFilesFilteredImpl(args.TargetFiles, args.Filter, args.DestinationPath);
		}
		else
		{
			copyFileImpl(args.TargetFiles, args.DestinationPath);
		}
		if (!stepFailed)
		{
			onStepSuccess();
		}
	}

	private void copyFileListImpl(CopyFileListArguments args)
	{
		overwrite = args.Overwrite;
		raiseCopyError = args.RaiseErrors;
		string[] fileList = args.FileList;
		foreach (string file in fileList)
		{
			copyFileImpl(file, args.DestinationPath);
			if (stepFailed)
			{
				return;
			}
		}
		if (!stepFailed)
		{
			onStepSuccess();
		}
	}

	private void copyFilesErrorImpl(string message)
	{
		displayLine(message);
		if (raiseCopyError)
		{
			onStepFailure();
		}
	}

	private bool launchExecutable(RunExecutableArguments args)
	{
		if (File.Exists(args.FileName))
		{
			currentRunningProcess = new Process();
			currentRunningProcess.StartInfo.FileName = args.FileName;
			currentRunningProcess.StartInfo.Arguments = args.Arguments;
			currentRunningProcess.StartInfo.CreateNoWindow = !args.CreateWindow;
			currentRunningProcess.StartInfo.UseShellExecute = args.CreateWindow;
			currentRunningProcess.StartInfo.RedirectStandardError = args.RedirectOutput;
			currentRunningProcess.StartInfo.RedirectStandardOutput = args.RedirectOutput;
			redirectOutput = args.RedirectOutput;
			currentRunningProcess.Exited += executableExited;
			currentRunningProcess.EnableRaisingEvents = true;
			currentRunningProcess.Start();
			processRunning = true;
			if (args.RedirectOutput)
			{
				currentProcessOutputReader = new Thread(readProcessOutputControl);
				currentProcessOutputReader.Start();
			}
			return true;
		}
		displayLine($"Failed to find executable: {args.FileName}");
		return false;
	}

	public void readProcessOutputControl()
	{
		while (processRunning)
		{
			readProcessOutput();
			Thread.Sleep(PROCESS_SLEEP_DELAY);
		}
	}

	public void readProcessOutput()
	{
		try
		{
			string text = "";
			string text2 = null;
			int num = 0;
			while ((text2 = currentRunningProcess.StandardOutput.ReadLine()) != null && num < MAX_READ_LINES && !breakOnInputLine(text2))
			{
				text += processInputLine(text2);
				text2 = null;
				num++;
			}
			Invoke(new WriteDataDelegate(writeDataDump), text);
			if (text2 != null)
			{
				processInputLine(text2);
			}
		}
		catch (Exception ex)
		{
			Invoke(new WriteDataDelegate(displayLine), ex.Message);
		}
	}

	public bool breakOnInputLine(string line)
	{
		if (line.Contains("Critical:") || line.Contains("Error:") || line.Contains("Warning:"))
		{
			return true;
		}
		return false;
	}

	public string processInputLine(string input)
	{
		if (input.Contains("Critical:"))
		{
			stepFailed = true;
			Invoke(new WriteDataDelegate(writeCriticalLine), input);
			return "";
		}
		if (input.Contains("Error:"))
		{
			Invoke(new WriteDataDelegate(writeErrorLine), input);
			return "";
		}
		if (input.Contains("Warning:"))
		{
			Invoke(new WriteDataDelegate(writeWarningLine), input);
			return "";
		}
		return input + "\r\n";
	}

	public void writeCriticalLine(string line)
	{
		int length = rtbGUIOutput.Text.Length;
		rtbGUIOutput.SelectionColor = criticalColor;
		rtbGUIOutput.AppendText(line + "\r\n");
		string text = line.Substring(line.IndexOf(']') + 1);
		TreeNode treeNode = new TreeNode(text);
		treeNode.Tag = length;
		tnCritical.Nodes.Add(treeNode);
		rtbGUIOutput.ScrollToCaret();
	}

	public void writeErrorLine(string line)
	{
		int length = rtbGUIOutput.Text.Length;
		rtbGUIOutput.SelectionColor = errorColor;
		rtbGUIOutput.AppendText(line + "\r\n");
		string text = line.Substring(line.IndexOf(']') + 1);
		TreeNode treeNode = new TreeNode(text);
		treeNode.Tag = length;
		treeNode.ImageIndex = ERROR_IMAGE_INDEX;
		treeNode.SelectedImageIndex = ERROR_IMAGE_INDEX;
		tnError.Nodes.Add(treeNode);
		rtbGUIOutput.ScrollToCaret();
	}

	public void writeWarningLine(string line)
	{
		int length = rtbGUIOutput.Text.Length;
		rtbGUIOutput.SelectionColor = warningColor;
		rtbGUIOutput.AppendText(line + "\r\n");
		string text = line.Substring(line.IndexOf(']') + 1);
		TreeNode treeNode = new TreeNode(text);
		treeNode.Tag = length;
		treeNode.ImageIndex = WARNING_IMAGE_INDEX;
		treeNode.SelectedImageIndex = WARNING_IMAGE_INDEX;
		tnWarning.Nodes.Add(treeNode);
		rtbGUIOutput.ScrollToCaret();
	}

	public void writeDataDump(string dump)
	{
		if (!stepFailed)
		{
			rtbGUIOutput.SelectionColor = Color.Black;
		}
		else
		{
			rtbGUIOutput.SelectionColor = Color.Red;
		}
		rtbGUIOutput.AppendText(dump);
		rtbGUIOutput.ScrollToCaret();
	}

	private void executableExited(object sender, EventArgs e)
	{
		processRunning = false;
		if (redirectOutput)
		{
			string text = "";
			string text2;
			while ((text2 = currentRunningProcess.StandardOutput.ReadLine()) != null)
			{
				if (breakOnInputLine(text2))
				{
					Invoke(new WriteDataDelegate(writeDataDump), text);
					text = "";
				}
				text += processInputLine(text2);
			}
			Invoke(new WriteDataDelegate(writeDataDump), text);
		}
		if (stepFailed)
		{
			Invoke(new FormInvokeDelegate(onStepFailure));
		}
		else
		{
			Invoke(new FormInvokeDelegate(onStepSuccess));
		}
	}

	private bool writeFileImpl(WriteFileArguments args)
	{
		try
		{
			displayDebugLine(args.Path);
			string directoryName = Path.GetDirectoryName(args.Path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
				displayLine("Directory created: " + directoryName);
			}
			File.WriteAllText(args.Path, args.Content);
			displayLine("File written: " + args.Path);
			onStepSuccess();
		}
		catch (Exception ex)
		{
			displayLine(ex.Message);
			onStepFailure();
		}
		return true;
	}

	private void onStepFailure()
	{
		stepFailed = true;
		buildScriptInstance.onStepFailure(currentStepId);
	}

	private void onStepSuccess()
	{
		if (BuildHelper.EnableBenchmarking)
		{
			long ticks = DateTime.UtcNow.Ticks;
			long value = ticks - StepTimeStart;
			displayGUILine("Step time taken: " + TimeSpan.FromTicks(value).ToString());
		}
		buildScriptInstance.onStepSuccess(currentStepId);
	}

	private void buttonBuildMod_Click(object sender, EventArgs e)
	{
		if (cmbMods.SelectedIndex != -1)
		{
			onBuildStart();
			Dictionary<string, object> gUIResults = generateGUIResult(inputModGUIData, generatedModGUIData);
			BuildHelper.StartBuild(BuildType.ModBuild, (string)cmbMods.SelectedItem);
			displayGUILine("Building Mod " + (string)cmbMods.SelectedItem);
			buildScriptInstance.buildMod((string)cmbMods.SelectedItem, gUIResults);
		}
	}

	private void onBuildStart()
	{
		if (BuildHelper.EnableBenchmarking)
		{
			BuildTimeStart = DateTime.UtcNow.Ticks;
		}
	}

	private void clearBuildLog_Click(object sender, EventArgs e)
	{
		rtbGUIOutput.Clear();
	}

	private void buttonSaveBuildLog_Click(object sender, EventArgs e)
	{
		string currentDirectory = Directory.GetCurrentDirectory();
		try
		{
			DialogResult dialogResult = saveBuildLog.ShowDialog();
			if (dialogResult.Equals(DialogResult.OK))
			{
				File.WriteAllText(saveBuildLog.FileName, rtbGUIOutput.Text);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Uncaught exception: " + ex.Message);
		}
		Directory.SetCurrentDirectory(currentDirectory);
	}

	private void tvErrorView_DoubleClick(object sender, EventArgs e)
	{
		TreeView treeView = (TreeView)sender;
		TreeNode selectedNode = treeView.SelectedNode;
		if (!selectedNode.Equals(tnCritical) && !selectedNode.Equals(tnError) && !selectedNode.Equals(tnWarning))
		{
			int selectionStart = (int)selectedNode.Tag;
			outputTabs.SelectedIndex = 0;
			rtbGUIOutput.SelectionStart = selectionStart;
			rtbGUIOutput.ScrollToCaret();
		}
	}

	private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
	{
		int splitterDistance = splitContainer1.SplitterDistance;
		int num = splitterDistance - leftPanelWidth;
		if (splitContainer1.Panel1.Width < MIN_LEFT_WIDTH)
		{
			splitContainer1.SplitterDistance = MIN_LEFT_WIDTH;
			return;
		}
		_ = grpModBuildOptions.Width;
		grpModBuildOptions.Width += num;
		flwModOptions.Width += num;
		if (modGUIPanel != null)
		{
			modGUIPanel.Width += num;
		}
		leftPanelWidth = splitterDistance;
		rtbGUIOutput.Refresh();
		rtbGUIOutput.ScrollToCaret();
	}

	public void displayDebugLine(string text)
	{
	}
}
