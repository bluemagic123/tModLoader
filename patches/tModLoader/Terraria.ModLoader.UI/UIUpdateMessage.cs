using Ionic.Zip;
using ReLogic.OS;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI.DownloadManager;
using Terraria.UI;

namespace Terraria.ModLoader.UI
{
	//TODO how is this different to UIInfoMessage?
	internal class UIUpdateMessage : UIState
	{
		private readonly UIMessageBox _message = new UIMessageBox("");
		private UIElement _area;
		private UITextPanel<string> _autoUpdateButton;
		private int _gotoMenu;
		private string _url;
		private string _autoUpdateUrl;

		public override void OnInitialize() {
			_area = new UIElement {
				Width = { Percent = 0.8f },
				Top = { Pixels = 200 },
				Height = { Pixels = -240, Percent = 1f },
				HAlign = 0.5f
			};

			_message.Width.Percent = 1f;
			_message.Height.Percent = 0.8f;
			_message.HAlign = 0.5f;
			_area.Append(_message);

			var button = new UITextPanel<string>("Ignore", 0.7f, true) {
				Width = { Pixels = -10, Percent = 1f / 3f },
				Height = { Pixels = 50 },
				VAlign = 1f,
				Top = { Pixels = -30 }
			};
			button.WithFadedMouseOver();
			button.OnClick += IgnoreClick;
			_area.Append(button);

			var button2 = new UITextPanel<string>("Download", 0.7f, true);
			button2.CopyStyle(button);
			button2.HAlign = 0.5f;
			button2.WithFadedMouseOver();
			button2.OnClick += OpenURL;
			_area.Append(button2);

			_autoUpdateButton = new UITextPanel<string>("Auto Update", 0.7f, true);
			_autoUpdateButton.CopyStyle(button);
			_autoUpdateButton.HAlign = 1f;
			_autoUpdateButton.WithFadedMouseOver();
			_autoUpdateButton.OnClick += AutoUpdate;

			Append(_area);
		}

		public override void OnActivate() {
			base.OnActivate();

			if (FrameworkVersion.Framework != Framework.Mono || FrameworkVersion.Version >= new Version(5, 20))
				_area.AddOrRemoveChild(_autoUpdateButton, !string.IsNullOrEmpty(_autoUpdateUrl));
		}

		internal void SetMessage(string text) {
			_message.SetText(text);
		}

		internal void SetGotoMenu(int gotoMenu) {
			_gotoMenu = gotoMenu;
		}

		internal void SetURL(string url) {
			_url = url;
		}

		internal void SetAutoUpdateURL(string autoUpdateURL) {
			_autoUpdateUrl = autoUpdateURL;
		}

		private void IgnoreClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.PlaySound(10);
			Main.menuMode = _gotoMenu;
		}

		private void OpenURL(UIMouseEvent evt, UIElement listeningElement) {
			Main.PlaySound(10);
			Process.Start(_url);
		}

		// Windows only. AutoUpdate will download the the latest zip, extract it, then launch a script that waits for this exe to finish
		// The script then replaces this exe and then launches tModLoader again.
		private void AutoUpdate(UIMouseEvent evt, UIElement listeningElement) {
			Main.PlaySound(SoundID.MenuOpen);

			string installDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string zipFileName = Path.GetFileName(new Uri(_autoUpdateUrl).LocalPath);
			string zipFilePath = Path.Combine(installDirectory, zipFileName);

			Logging.tML.Info($"AutoUpdate started: {_autoUpdateUrl} -> {zipFilePath}");
			var downloadFile = new DownloadFile(_autoUpdateUrl, zipFilePath, $"Auto update: {zipFileName}");
			downloadFile.OnComplete += () => OnAutoUpdateDownloadComplete(installDirectory, zipFilePath);
			Interface.downloadProgress.gotoMenu = Interface.modBrowserID;
			Interface.downloadProgress.HandleDownloads(downloadFile);
		}

		private void OnAutoUpdateDownloadComplete(string installDirectory, string zipFilePath) {
			string executableName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
			string extractDir = Path.Combine(installDirectory, "tModLoader_Update");
			string updateScriptName = Platform.IsWindows ? "update.bat" : "update.sh";
			string updateScript = Path.Combine(installDirectory, updateScriptName);

			Logging.tML.Info($"AutoUpdate Paths: installDirectory {installDirectory}, executableName {executableName},  extractDir {extractDir}, autoUpdateScript {updateScript}");
			try {
				if (Directory.Exists(extractDir))
					Directory.Delete(extractDir, true);
				Directory.CreateDirectory(extractDir);

				using (var zip = ZipFile.Read(zipFilePath)) 
					zip.ExtractAll(extractDir);
				File.Delete(zipFilePath);

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Terraria.ModLoader.Core.{updateScriptName}"))
				using (var fs = File.OpenWrite(updateScript))
					stream.CopyTo(fs);

				if (Platform.IsWindows) {
					File.Move(Path.Combine(extractDir, "Terraria.exe"), Path.Combine(extractDir, executableName));
					Process.Start(updateScript, $"\"{executableName}\" tModLoader_Update");
				}
				else {
					Process.Start("bash", $"-c 'chmod a+x \"{updateScript}\"'").WaitForExit();
					Process.Start(updateScript, $"{Process.GetCurrentProcess().Id} tModLoader_Update");
				}

				Logging.tML.Info("AutoUpdate script started. Exiting");
				// Exit on main thread to avoid crash
				Interface.downloadProgress.gotoMenu = Interface.exitID;
				Main.menuMode = Interface.exitID;
			}
			catch (Exception e) {
				Logging.tML.Error("Problem during autoupdate", e);
			}
		}
	}
}
