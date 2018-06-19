using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;
using System.Reflection;
using Terraria.Localization;

namespace Terraria.ModLoader
{
	//todo: further documentation
	/// <summary>
	/// This class consists of functions that write error messages to text files for you to read. It also lets you write logs to text files.
	/// </summary>
	public static class ErrorLogger
	{
		/// <summary>
		/// The file path to which logs are written and stored.
		/// </summary>
		public static readonly string LogPath = Path.Combine(Main.SavePath, "Logs");
		internal static string CompileErrorPath = Path.Combine(LogPath, "Compile Errors.txt");
		private static readonly string[] buildDllLines =
		{
			"Must have either All.dll or both of Windows.dll and Mono.dll",
			"All.dll must not have any references to Microsoft.Xna.Framework or FNA",
			"Windows.dll must reference the windows Terraria.exe and Microsoft.Xna.Framework.dll",
			"Mono.dll must reference a non-windows Terraria.exe and FNA.dll"
		};

		internal static void LogBuildError(string errorText)
		{
			Directory.CreateDirectory(LogPath);
			File.WriteAllText(CompileErrorPath, errorText);
			Console.WriteLine(errorText);

			Interface.errorMessage.SetMessage(errorText);
			Interface.errorMessage.SetGotoMenu(Interface.modSourcesID);
			Interface.errorMessage.SetFile(CompileErrorPath);
		}

		internal static void LogCompileErrors(CompilerErrorCollection errors, bool forWindows)
		{
			string errorHeader = Language.GetTextValue("tModLoader.BuildErrorCompilingError") + Environment.NewLine + Environment.NewLine;
			string badInstallHint = "";
			if (!forWindows && ModLoader.windows)
			{
				badInstallHint = Language.GetTextValue("tModLoader.BuildErrorModCompileFolderHint") + Environment.NewLine + Environment.NewLine;
			}
			Console.WriteLine(errorHeader + badInstallHint);
			Directory.CreateDirectory(LogPath);
			CompilerError displayError = null;
			using (var writer = File.CreateText(CompileErrorPath))
			{
				foreach (CompilerError error in errors)
				{
					writer.WriteLine(error + Environment.NewLine);
					Console.WriteLine(error);
					if (!error.IsWarning && displayError == null)
						displayError = error;
				}
			}
			Interface.errorMessage.SetMessage(errorHeader + badInstallHint + displayError);
			Interface.errorMessage.SetGotoMenu(Interface.modSourcesID);
			Interface.errorMessage.SetFile(CompileErrorPath);
		}

		internal static void LogDllBuildError(string modDir)
		{
			Directory.CreateDirectory(LogPath);
			var errorText = Language.GetTextValue("tModLoader.BuildErrorMissingDllFilesFor", Path.GetFileName(modDir)) + Environment.NewLine + Environment.NewLine +
							string.Join(Environment.NewLine, buildDllLines);
			File.WriteAllText(CompileErrorPath, errorText);
			Console.WriteLine(errorText);

			Interface.errorMessage.SetMessage(errorText);
			Interface.errorMessage.SetGotoMenu(Interface.modSourcesID);
			Interface.errorMessage.SetFile(CompileErrorPath);
		}

		internal static void LogDependencyError(string error)
		{
			Directory.CreateDirectory(LogPath);
			string file = Path.Combine(LogPath, "Loading Errors.txt");
			File.WriteAllText(file, error);
			Console.WriteLine(error);

			Interface.errorMessage.SetMessage(error);
			Interface.errorMessage.SetGotoMenu(Interface.reloadModsID);
			Interface.errorMessage.SetFile(file);
		}

		internal static void LogLoadingError(string modFile, Version modBuildVersion, Exception e, bool recipes = false)
		{
			Directory.CreateDirectory(LogPath);
			string file = LogPath + Path.DirectorySeparatorChar + "Loading Errors.txt";
			using (StreamWriter writer = File.CreateText(file))
			{
				writer.WriteLine(e.Message);
				writer.WriteLine(e.StackTrace);
				Exception inner = e.InnerException;
				while (inner != null)
				{
					writer.WriteLine();
					writer.WriteLine("Inner Exception:");
					writer.WriteLine(inner.Message);
					writer.WriteLine(inner.StackTrace);
					inner = inner.InnerException;
				}
			}
			string message;
			if (recipes)
			{
				message = Language.GetTextValue("tModLoader.LoadErrorRecipes", modFile);
			}
			else
			{
				message = Language.GetTextValue("tModLoader.LoadError", modFile);
			}
			if (modBuildVersion != ModLoader.version)
			{
				message += "\n" + Language.GetTextValue("tModLoader.LoadErrorVersionMessage", modBuildVersion, ModLoader.versionedName);
			}
			message += "\n" + Language.GetTextValue("tModLoader.LoadErrorDisabledSeeBelowForError");
			message += "\n\n" + e.Message + "\n" + e.StackTrace;
			if (Main.dedServ)
			{
				Console.WriteLine(message);
			}
			Interface.errorMessage.SetMessage(message);
			Interface.errorMessage.SetGotoMenu(Interface.reloadModsID);
			Interface.errorMessage.SetFile(file);
			if (!string.IsNullOrEmpty(e.HelpLink))
			{
				Interface.errorMessage.SetWebHelpURL(e.HelpLink);
			}
		}

		private static Object logExceptionLock = new Object();
		//add try catch to Terraria.WorldGen.worldGenCallBack
		//add try catch to Terraria.WorldGen.playWorldCallBack
		//add try catch to Terraria.Main.Update
		//add try catch to Terraria.Main.Draw
		internal static void LogException(Exception e, string msg = "The game has crashed!")
		{
			lock (logExceptionLock)
			{
				Directory.CreateDirectory(LogPath);
				string file = LogPath + Path.DirectorySeparatorChar + "Runtime Error.txt";
				using (StreamWriter writer = File.CreateText(file))
				{
					writer.WriteLine(e.Message);
					writer.WriteLine(e.StackTrace);
					Exception inner = e.InnerException;
					while (inner != null)
					{
						writer.WriteLine();
						writer.WriteLine("Inner Exception:");
						writer.WriteLine(inner.Message);
						writer.WriteLine(inner.StackTrace);
						inner = inner.InnerException;
					}
				}
				Interface.errorMessage.SetMessage(msg + "\n\n" + e.Message + "\n" + e.StackTrace);
				Interface.errorMessage.SetGotoMenu(0);
				Interface.errorMessage.SetFile(file);
				Main.gameMenu = true;
				Main.menuMode = Interface.errorMessageID;
			}
		}

		internal static void LogModBrowserException(Exception e) => LogException(e, "The game has crashed accessing Web Resources!");

		internal static void LogModPublish(string message)
		{
			string file = LogPath + Path.DirectorySeparatorChar + "Network Error.txt";
			using (StreamWriter writer = File.CreateText(file))
			{
				writer.WriteLine(message);
			}
			Interface.errorMessage.SetMessage(Language.GetTextValue("tModLoader.MBServerResonponse", message));
			Interface.errorMessage.SetGotoMenu(Interface.modSourcesID);
			Interface.errorMessage.SetFile(file);
			Main.gameMenu = true;
			Main.menuMode = Interface.errorMessageID;
		}

		internal static void LogModUnPublish(string message)
		{
			string file = LogPath + Path.DirectorySeparatorChar + "Network Error.txt";
			using (StreamWriter writer = File.CreateText(file))
			{
				writer.WriteLine(message);
			}
			Interface.errorMessage.SetMessage(Language.GetTextValue("tModLoader.MBServerResonponse", message));
			Interface.errorMessage.SetGotoMenu(Interface.managePublishedID);
			Interface.errorMessage.SetFile(file);
			Main.gameMenu = true;
			Main.menuMode = Interface.errorMessageID;
		}

		internal static void LogMissingMods(string msg)
		{
			Directory.CreateDirectory(LogPath);
			string file = Path.Combine(LogPath, "Missing Mods.txt");
			File.WriteAllText(file, msg);
			Console.WriteLine(msg);

			Interface.errorMessage.SetMessage(msg);
			Interface.errorMessage.SetGotoMenu(0);
			Interface.errorMessage.SetFile(file);
			Main.gameMenu = true;
			Main.menuMode = Interface.errorMessageID;
		}

		private static Object logLock = new Object();
		/// <summary>
		/// You can use this method for your own testing purposes. The message will be added to the Logs.txt file in the Logs folder.
		/// </summary>
		public static void Log(string message)
		{
			lock (logLock)
			{
				Directory.CreateDirectory(LogPath);
				using (StreamWriter writer = File.AppendText(LogPath + Path.DirectorySeparatorChar + "Logs.txt"))
				{
					writer.WriteLine(message);
				}
			}
		}

		/// <summary>
		/// Allows you to log an object for your own testing purposes. The message will be added to the Logs.txt file in the Logs folder. 
		/// </summary>
		/// <param name="param">The object to be logged.</param>
		/// <param name="alternateOutput">If true, the object's data will be manually retrieved and logged. If false, the object's ToString method is logged.</param>
		public static void Log(object param, bool alternateOutput = false)
		{
			lock (logLock)
			{
				Directory.CreateDirectory(LogPath);
				using (StreamWriter writer = File.AppendText(LogPath + Path.DirectorySeparatorChar + "Logs.txt"))
				{
					if (!alternateOutput)
					{
						writer.WriteLine(param.ToString());
					}
					else
					{
						writer.WriteLine("Object type: " + param.GetType());
						foreach (PropertyInfo property in param.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							writer.Write("PROPERTY " + property.Name + " = " + property.GetValue(param, null) + "\n");
						}

						foreach (FieldInfo field in param.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							writer.Write("FIELD " + field.Name + " = " + (field.GetValue(param).ToString() != "" ? field.GetValue(param) : "(Field value not found)") + "\n");
						}

						foreach (MethodInfo method in param.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							writer.Write("METHOD " + method.Name + "\n");
						}

						int temp = 0;

						foreach (ConstructorInfo constructor in param.GetType().GetConstructors(BindingFlags.Public | BindingFlags.NonPublic))
						{
							temp++;
							writer.Write("CONSTRUCTOR " + temp + " : " + constructor.Name + "\n");
						}
					}
				}
			}
		}

		/// <summary>
		/// Deletes all log files.
		/// </summary>
		public static void ClearLogs()
		{
			lock (logLock)
			{
				Directory.CreateDirectory(LogPath);
				string[] files = new string[] {
					LogPath + Path.DirectorySeparatorChar + "Logs.txt",
					LogPath + Path.DirectorySeparatorChar + "Network Error.txt",
					LogPath + Path.DirectorySeparatorChar + "Runtime Error.txt",
					LogPath + Path.DirectorySeparatorChar + "Loading Errors.txt",
					CompileErrorPath,
				};
				foreach (var file in files)
				{
					try
					{
						File.Delete(file);
					}
					catch (Exception)
					{
						// Don't worry about it, modder or player might have the file open in tail or notepad.
					}
				}
			}
		}

		//this is a terrible hack on an average system
		public static void LogMulti(IEnumerable<Action> logCalls)
		{
			if (Main.dedServ)
			{
				foreach (var call in logCalls)
					call();
				return;
			}

			var list = logCalls.ToArray();
			list[0]();
			if (list.Length > 1)
				Interface.errorMessage.OverrideContinueAction(() => LogMulti(list.Skip(1)));
		}
	}
}
