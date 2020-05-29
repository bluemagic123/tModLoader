using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.DownloadManager;
using Terraria.ModLoader.UI.ModBrowser;

namespace Terraria.ModLoader.UI
{
	internal static class Interface
	{
		internal const int modsMenuID = 10000;
		internal const int modSourcesID = 10001;
		//set initial Main.menuMode to loadModsID
		internal const int loadModsID = 10002;
		internal const int buildModID = 10003;
		internal const int errorMessageID = 10005;
		internal const int reloadModsID = 10006;
		internal const int modBrowserID = 10007;
		internal const int modInfoID = 10008;
		//internal const int downloadModID = 10009;
		//internal const int modControlsID = 10010;
		internal const int managePublishedID = 10011;
		internal const int updateMessageID = 10012;
		internal const int infoMessageID = 10013;
		internal const int enterPassphraseMenuID = 10015;
		internal const int modPacksMenuID = 10016;
		internal const int tModLoaderSettingsID = 10017;
		internal const int enterSteamIDMenuID = 10018;
		internal const int extractModID = 10019;
		internal const int downloadProgressID = 10020;
		internal const int developerModeHelpID = 10022;
		internal const int progressID = 10023;
		internal const int modConfigID = 10024;
		internal const int createModID = 10025;
		internal const int exitID = 10026;
		internal static UIMods modsMenu = new UIMods();
		internal static UILoadMods loadMods = new UILoadMods();
		private static UIModSources modSources = new UIModSources();
		internal static UIBuildMod buildMod = new UIBuildMod();
		internal static UIErrorMessage errorMessage = new UIErrorMessage();
		internal static UIModBrowser modBrowser = new UIModBrowser();
		internal static UIModInfo modInfo = new UIModInfo();
		internal static UIManagePublished managePublished = new UIManagePublished();
		internal static UIUpdateMessage updateMessage = new UIUpdateMessage();
		internal static UIInfoMessage infoMessage = new UIInfoMessage();
		internal static UIEnterPassphraseMenu enterPassphraseMenu = new UIEnterPassphraseMenu();
		internal static UIModPacks modPacksMenu = new UIModPacks();
		internal static UIEnterSteamIDMenu enterSteamIDMenu = new UIEnterSteamIDMenu();
		internal static UIExtractMod extractMod = new UIExtractMod();
		internal static UIDeveloperModeHelp developerModeHelp = new UIDeveloperModeHelp();
		internal static UIModConfig modConfig = new UIModConfig();
		internal static UIModConfigList modConfigList = new UIModConfigList();
		internal static UICreateMod createMod = new UICreateMod();
		internal static UIProgress progress = new UIProgress();
		internal static UIDownloadProgress downloadProgress = new UIDownloadProgress();

		//add to Terraria.Main.DrawMenu in Main.menuMode == 0 after achievements
		//Interface.AddMenuButtons(this, this.selectedMenu, array9, array7, ref num, ref num3, ref num10, ref num5);
		internal static void AddMenuButtons(Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, ref int offY, ref int spacing, ref int buttonIndex, ref int numButtons) {
			buttonNames[buttonIndex] = Language.GetTextValue("tModLoader.MenuMods");
			if (selectedMenu == buttonIndex) {
				Main.PlaySound(10, -1, -1, 1);
				Main.menuMode = modsMenuID;
			}
			buttonIndex++;
			numButtons++;
			if (ModCompile.DeveloperMode) {
				buttonNames[buttonIndex] = Language.GetTextValue("tModLoader.MenuModSources");
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(10, -1, -1, 1);
					Main.menuMode = ModCompile.DeveloperModeReady(out var _) ? modSourcesID : developerModeHelpID;
				}
				buttonIndex++;
				numButtons++;
			}
			buttonNames[buttonIndex] = Language.GetTextValue("tModLoader.MenuModBrowser");
			if (selectedMenu == buttonIndex) {
				Main.PlaySound(10, -1, -1, 1);
				Main.menuMode = modBrowserID;
			}
			buttonIndex++;
			numButtons++;
			offY = 220;
			for (int k = 0; k < numButtons; k++) {
				buttonScales[k] = 0.82f;
			}
			spacing = 45;
		}

		internal static void ResetData() {
			modBrowser.Reset();
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		}

		//internal static void AddSettingsMenuButtons(Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] virticalSpacing, ref int offY, ref int spacing, ref int buttonIndex, ref int numButtons)
		//{
		//	buttonIndex++;
		//	numButtons++;
		//	buttonNames[buttonIndex] = "Mod " + Lang.menu[66];
		//	if (selectedMenu == buttonIndex)
		//	{
		//		Main.PlaySound(10, -1, -1, 1);
		//		Main.menuMode = modControlsID;
		//	}
		//	for (int k = 0; k < numButtons; k++)
		//	{
		//		buttonScales[k] = 0.73f;
		//		virticalSpacing[k] = 0;
		//	}
		//	virticalSpacing[numButtons - 1] = 8;
		//}

		//add to end of if else chain of Main.menuMode in Terraria.Main.DrawMenu
		//Interface.ModLoaderMenus(this, this.selectedMenu, array9, array7, array4, ref num2, ref num4, ref num5, ref flag5);
		internal static void ModLoaderMenus(Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] buttonVerticalSpacing, ref int offY, ref int spacing, ref int numButtons, ref bool backButtonDown) {
			if (Main.menuMode == loadModsID) {
				if (ModLoader.ShowFirstLaunchWelcomeMessage) {
					ModLoader.ShowFirstLaunchWelcomeMessage = false;
					infoMessage.Show(Language.GetTextValue("tModLoader.FirstLaunchWelcomeMessage"), Main.menuMode);
				}
				//else if (ModLoader.ShowWhatsNew) {
				//	// TODO: possibly pull from github
				//	ModLoader.ShowWhatsNew = false;
				//	infoMessage.Show(Language.GetTextValue("tModLoader.WhatsNewMessage"), Main.menuMode);
				//}
			}
			if (Main.menuMode == modsMenuID) {
				Main.MenuUI.SetState(modsMenu);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == modSourcesID) {
				Main.MenuUI.SetState(modSources);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == createModID) {
				Main.MenuUI.SetState(createMod);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == developerModeHelpID) {
				Main.MenuUI.SetState(developerModeHelp);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == loadModsID) {
				Main.menuMode = 888;
				Main.MenuUI.SetState(loadMods);
			}
			else if (Main.menuMode == buildModID) {
				Main.MenuUI.SetState(buildMod);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == errorMessageID) {
				Main.MenuUI.SetState(errorMessage);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == reloadModsID) {
				ModLoader.Reload();
			}
			else if (Main.menuMode == modBrowserID) {
				Main.MenuUI.SetState(modBrowser);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == modInfoID) {
				Main.MenuUI.SetState(modInfo);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == managePublishedID) {
				Main.menuMode = 888;
				Main.MenuUI.SetState(managePublished);
			}
			//else if (Main.menuMode == modControlsID)
			//{
			//	UIModControls.ModLoaderMenus(main, selectedMenu, buttonNames, buttonScales, buttonVerticalSpacing, ref offY, ref spacing, ref numButtons);
			//}
			else if (Main.menuMode == updateMessageID) {
				Main.MenuUI.SetState(updateMessage);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == infoMessageID) {
				Main.MenuUI.SetState(infoMessage);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == enterPassphraseMenuID) {
				Main.MenuUI.SetState(enterPassphraseMenu);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == enterSteamIDMenuID) {
				Main.MenuUI.SetState(enterSteamIDMenu);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == modPacksMenuID) {
				Main.MenuUI.SetState(modPacksMenu);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == extractModID) {
				Main.MenuUI.SetState(extractMod);
				Main.menuMode = 888;
			}
			else if(Main.menuMode == progressID) {
				Main.MenuUI.SetState(progress);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == downloadProgressID) {
				Main.MenuUI.SetState(downloadProgress);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == tModLoaderSettingsID) {
				offY = 210;
				spacing = 42;
				numButtons = 11;
				buttonVerticalSpacing[numButtons - 1] = 18;
				for (int i = 0; i < numButtons; i++) {
					buttonScales[i] = 0.75f;
				}
				int buttonIndex = 0;
				buttonNames[buttonIndex] = (ModNet.downloadModsFromServers ? Language.GetTextValue("tModLoader.DownloadFromServersYes") : Language.GetTextValue("tModLoader.DownloadFromServersNo"));
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModNet.downloadModsFromServers = !ModNet.downloadModsFromServers;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = (ModNet.onlyDownloadSignedMods ? Language.GetTextValue("tModLoader.DownloadSignedYes") : Language.GetTextValue("tModLoader.DownloadSignedNo"));
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModNet.onlyDownloadSignedMods = !ModNet.onlyDownloadSignedMods;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = (ModLoader.autoReloadAndEnableModsLeavingModBrowser ? Language.GetTextValue("tModLoader.AutomaticallyReloadAndEnableModsLeavingModBrowserYes") : Language.GetTextValue("tModLoader.AutomaticallyReloadAndEnableModsLeavingModBrowserNo"));
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModLoader.autoReloadAndEnableModsLeavingModBrowser = !ModLoader.autoReloadAndEnableModsLeavingModBrowser;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = (Main.UseExperimentalFeatures ? Language.GetTextValue("tModLoader.ExperimentalFeaturesYes") : Language.GetTextValue("tModLoader.ExperimentalFeaturesNo"));
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					Main.UseExperimentalFeatures = !Main.UseExperimentalFeatures;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = Language.GetTextValue($"tModLoader.RemoveForcedMinimumZoom{(ModLoader.removeForcedMinimumZoom ? "Yes" : "No")}");
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModLoader.removeForcedMinimumZoom = !ModLoader.removeForcedMinimumZoom;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = Language.GetTextValue($"tModLoader.ShowMemoryEstimates{(ModLoader.showMemoryEstimates ? "Yes" : "No")}");
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModLoader.showMemoryEstimates = !ModLoader.showMemoryEstimates;
				}

				buttonIndex++;
				buttonNames[buttonIndex] = Language.GetTextValue("tModLoader.ClearMBCredentials");
				if (selectedMenu == buttonIndex) {
					Main.PlaySound(SoundID.MenuTick);
					ModLoader.modBrowserPassphrase = "";
					ModLoader.SteamID64 = "";
				}

				buttonIndex++;
				buttonNames[buttonIndex] = Lang.menu[5].Value;
				if (selectedMenu == buttonIndex || backButtonDown) {
					backButtonDown = false;
					Main.menuMode = 11;
					Main.PlaySound(11, -1, -1, 1);
				}
			}
			else if (Main.menuMode == modConfigID)
			{
				Main.MenuUI.SetState(modConfig);
				Main.menuMode = 888;
			}
			else if (Main.menuMode == exitID) {
				Environment.Exit(0);
			}
		}

		internal static void ServerModMenu() {
			bool exit = false;
			while (!exit) {
				Console.WriteLine("Terraria Server " + Main.versionNumber2 + " - " + ModLoader.versionedName);
				Console.WriteLine();
				var mods = ModOrganizer.FindMods();
				for (int k = 0; k < mods.Length; k++) {
					Console.Write((k + 1) + "\t\t" + mods[k].DisplayName);
					Console.WriteLine(" (" + (ModLoader.IsEnabled(mods[k].Name) ? "enabled" : "disabled") + ")");
				}
				if (mods.Length == 0) {
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine($"No mods were found in: \"{ModLoader.ModPath}\"\nIf you are running a dedicated server, you may wish to use the 'modpath' command line switch or server config setting to specify a custom mods directory.\n");
					Console.ResetColor();
				}
				Console.WriteLine("e\t\tEnable All");
				Console.WriteLine("d\t\tDisable All");
				Console.WriteLine("r\t\tReload and return to world menu");
				Console.WriteLine("Type a number to switch between enabled/disabled");
				Console.WriteLine();
				Console.WriteLine("Type a command: ");
				string command = Console.ReadLine();
				if (command == null) {
					command = "";
				}
				command = command.ToLower();
				Console.Clear();
				if (command == "e") {
					foreach (var mod in mods) {
						mod.Enabled = true;
					}
				}
				else if (command == "d") {
					foreach (var mod in mods) {
						mod.Enabled = false;
					}
				}
				else if (command == "r") {
					ModLoader.Reload();
					exit = true;
				}
				else if (int.TryParse(command, out int value) && value > 0 && value <= mods.Length) {
					mods[value - 1].Enabled ^= true;
				}
			}
		}

		internal static void ServerModBrowserMenu() {
			bool exit = false;
			Console.Clear();
			while (!exit) {
				Console.WriteLine();
				Console.WriteLine("b\t\tReturn to world menu");
				Console.WriteLine();
				Console.WriteLine("Type an exact ModName to download: ");
				string command = Console.ReadLine();
				if (command == null) {
					command = "";
				}
				if (command == "b" || command == "B") {
					exit = true;
				}
				else {
					string modname = command;
					try {
						ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
						using (WebClient client = new WebClient()) {
							string downloadURL = client.DownloadString($"http://javid.ddns.net/tModLoader/tools/querymoddownloadurl.php?modname={modname}");
							if (downloadURL.StartsWith("Failed")) {
								Console.WriteLine(downloadURL);
							}
							else {
								string tempFile = ModLoader.ModPath + Path.DirectorySeparatorChar + "temporaryDownload" + DownloadFile.TEMP_EXTENSION;
								client.DownloadFile(downloadURL, tempFile);
								ModLoader.GetMod(modname)?.Close();
								File.Copy(tempFile, ModLoader.ModPath + Path.DirectorySeparatorChar + downloadURL.Substring(downloadURL.LastIndexOf("/")), true);
								File.Delete(tempFile);
							}
							while (Console.KeyAvailable)
								Console.ReadKey(true);
						}
					}
					catch (Exception e) {
						Console.WriteLine("Error: Could not download " + modname + " -- " + e.ToString());
					}
				}
			}
			Console.Clear();
		}

		internal static void MessageBoxShow(string text, string caption = null) {
			// MessageBox.Show fails on Mac, this method will open a text file to show a message.
			caption = caption ?? "Terraria: Error" + $" ({ModLoader.versionedName})";
			string message = $"{text}\n\nA client.log file containing error information has been generated in\n{Path.Combine(Main.SavePath, "Logs")}\n(You will need to share this file if asking for help)";
#if !MAC
			System.Windows.Forms.MessageBox.Show(message, caption);
#else
			File.WriteAllText("fake-messagebox.txt", $"{caption}\n\n{text}");
			Process.Start("fake-messagebox.txt");
#endif
		}

		internal static void MessageBoxShow(Exception e, string caption = null, bool generateTip = false) {
			string tip = "";

			if (generateTip) {
				if (e is OutOfMemoryException)
					tip = "Try running less mods at once, or determine if a mod is taking up too much memory. Optionally, install 64-bit tModLoader";
				else if (e is InvalidOperationException || e is NullReferenceException || e is IndexOutOfRangeException || e is ArgumentNullException)
					tip = "This is likely a mod's fault. Disable mods one by one and check if the issue persists";
				else if (e is IOException && e.Message.Contains("cloud file provider"))
					tip = "Try installing/enabling OneDrive. Right click your Documents folder and enable \"Always save on this device\"";
				else if (e is System.Threading.SynchronizationLockException)
					tip = "If you have an antivirus, try adding Terraria to its whitelist/exclusion list";
				else if (e is TypeInitializationException)
					tip = "Restart the game. If this issue persists, try uninstalling Terraria completely, then reinstalling through Steam, and then reinstalling tModLoader";
			}

			string message = e.ToString();
			if (tip != "")
				message += $"\n\nTip: {tip}";

			MessageBoxShow(message, caption);
		}
	}
}
