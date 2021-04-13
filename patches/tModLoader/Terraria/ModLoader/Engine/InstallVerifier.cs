﻿using ReLogic.OS;
using Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria.ModLoader.Engine
{
	internal static class InstallVerifier
	{
		public const string TmlContentDirectory = "Content";
		public const bool RequireContentDirectory = false; // Not currently needed, due to tML matching vanilla's version.

		private static bool? isValid;
		public static bool IsValid => isValid ?? (isValid = InstallCheck()).Value;
		public static bool IsGoG = false;
		public static bool IsSteam = false;

		private static string steamAPIPath;
		private static byte[] steamAPIHash;
		private static byte[] gogHash;
		private static byte[] steamHash;

		static InstallVerifier() {
			if (Platform.IsWindows) {
				steamAPIPath = "steam_api.dll";
				steamAPIHash = ToByteArray("7B857C897BC69313E4936DC3DCCE5193");
				gogHash = ToByteArray("ff61b96a07894a9e65f880fb9608fb37"); // Don't forget to update CheckExe in CheckGoG
				steamHash = ToByteArray("4fd8072ca82ded3d9da1be577a478788");
			}
			else if (Platform.IsOSX) {
				steamAPIPath = "osx/libsteam_api.dylib";
				steamAPIHash = ToByteArray("4EECD26A0CDF89F90D4FF26ECAD37BE0");
				gogHash = ToByteArray("181c586d0fe64156adb0ecd4b9fabf9d");
				steamHash = ToByteArray("12c8d2ac5af6c8505bd1a9339dc75231");
			}
			else if (Platform.IsLinux) {
				steamAPIPath = "lib/libsteam_api.so";
				steamAPIHash = ToByteArray("7B74FD4C207D22DB91B4B649A44467F6");
				gogHash = ToByteArray("4a051352dd6ecc323c5a0a15e5b598fb");
				steamHash = ToByteArray("debcc318ca4e14295e3ac22e380e289b");
			}
			else {
				string message = Language.GetTextValue("tModLoader.UnknownVerificationOS");
				Logging.tML.Fatal(message);
				Exit(message, string.Empty);
			}
		}

		private static bool HashMatchesFile(string path, byte[] hash) {
			using (var md5 = MD5.Create())
			using (var stream = File.OpenRead(path))
				return hash.SequenceEqual(md5.ComputeHash(stream));
		}

		private static byte[] ToByteArray(string hexString) {
			byte[] retval = new byte[hexString.Length / 2];
			for (int i = 0; i < hexString.Length; i += 2)
				retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
			return retval;
		}
		private static void Exit(string errorMessage, string extraMessage) {
			errorMessage += $"\r\n\r\n{extraMessage}";
			Logging.tML.Fatal(errorMessage);
			UI.Interface.MessageBoxShow(errorMessage);
			Environment.Exit(1);
		}

		private static bool InstallCheck() {
#if CLIENT
			// Check if the content directory is present, if it's required
			if (RequireContentDirectory && !Directory.Exists(TmlContentDirectory)) {
				Exit(Language.GetTextValue("tModLoader.ContentFolderNotFoundInstallCheck", TmlContentDirectory), Language.GetTextValue("tModLoader.DefaultExtraMessage"));
				return false;
			}
#endif

			// Whether the steam_api file exists, indicating we'd have to check steam installation
			if (File.Exists(steamAPIPath))
				return CheckSteam();

			return CheckGoG();
		}

		// Check if steam installation is correct
		private static bool CheckSteam() {
			Logging.tML.Info("Checking Steam installation...");
			IsSteam = true;
#if CLIENT
			SocialAPI.LoadSteam();
			string terrariaInstallLocation = Steam.GetSteamTerrariaInstallDir();
			string terrariaContentLocation = Path.Combine(terrariaInstallLocation, TmlContentDirectory);
#if MAC
			terrariaContentLocation = Path.Combine(terrariaInstallLocation, "../Resources/Content");
#endif

			if (!Directory.Exists(terrariaContentLocation)) {
				Exit(Language.GetTextValue("tModLoader.VanillaSteamInstallationNotFound"), Language.GetTextValue("tModLoader.DefaultExtraMessage"));
				return false;
			}
#endif
			if (!HashMatchesFile(steamAPIPath, steamAPIHash)) {
				Process.Start(@"https://terraria.org");
				Exit(Language.GetTextValue("tModLoader.SteamAPIHashMismatch"), string.Empty);
				return false;
			}

			Logging.tML.Info("Steam installation OK.");
			return true;
		}

		// Check if GOG install or manual install is correct
		private static bool CheckGoG() {
			Logging.tML.Info("Checking GOG or manual installation...");
			IsGoG = true;

			const string DefaultExe = "Terraria.exe";
			string CheckExe = $"Terraria_1.4.2.1.exe"; // This should match the hashes. {Main.versionNumber}
			string vanillaPath = File.Exists(CheckExe) ? CheckExe : DefaultExe;

			// If .exe not present, check Terraria directory (Side-by-Side Manual Install)
			if (!File.Exists(vanillaPath)) {
				vanillaPath = Path.Combine("..", "Terraria");
#if MAC
				// GOG installs to /Applications/Terraria.app, Steam installs to /Applications/Terraria/Terraria.app
				// working directory is /Applications/tModLoader/tModLoader.app/Contents/MacOS/ for steam manual installs
				// working directory is /Applications/tModLoader.app/Contents/MacOS/ for GOG installs
				// Vanilla .exe files are in /Contents/Resources/, not /Contents/MacOS/
				if (Directory.Exists("../../../../Terraria/Terraria.app/")) {
					vanillaPath = "../../../../Terraria/Terraria.app/Contents/Resources/";
					Logging.tML.Info($"Mac installation location found at {vanillaPath}, assuming Steam manual install");
				}
				else if (Directory.Exists("../../../Terraria.app/")) {
					vanillaPath = "../../../Terraria.app/Contents/Resources/";
					Logging.tML.Info($"Mac installation location found at {vanillaPath}, assuming GOG manual install");
				}
				else {
					Logging.tML.Info($"Mac installation location not found.");
				}
#endif
				string defaultExe = Path.Combine(vanillaPath, DefaultExe);
				string checkExe = Path.Combine(vanillaPath, CheckExe);
				vanillaPath = File.Exists(checkExe) ? checkExe : defaultExe;
			}
			// If .exe not present check parent directory (Nested Manual Install)
			if (!File.Exists(vanillaPath)) {
				string defaultExe = Path.Combine("..", DefaultExe);
				string checkExe = Path.Combine("..", CheckExe);
				vanillaPath = File.Exists(checkExe) ? checkExe : defaultExe;
			}

			if (!File.Exists(vanillaPath)) {
#if SERVER
				return false;
#else
				Exit(Language.GetTextValue("tModLoader.VanillaGOGNotFound", vanillaPath, CheckExe), string.Empty);
				return false;
#endif
			}

			if (!HashMatchesFile(vanillaPath, gogHash) && !HashMatchesFile(vanillaPath, steamHash)) {
				Exit(Language.GetTextValue("tModLoader.GOGHashMismatch", vanillaPath), string.Empty);
				return false;
			}

			if (Path.GetFileName(vanillaPath) != CheckExe) {
				string pathToCheckExe = Path.Combine(Path.GetDirectoryName(vanillaPath), CheckExe);
				Logging.tML.Info($"Backing up {Path.GetFileName(vanillaPath)} to {CheckExe}");
				File.Copy(vanillaPath, pathToCheckExe);
			}

			Logging.tML.Info("GOG or manual installation OK.");
			return true;
		}
	}
}