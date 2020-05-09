﻿using Ionic.Zip;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Terraria.ModLoader.Core
{
	/// <summary>
	/// Log archiving is performed after log initialization in a separate class to avoid loading Ionic.Zip before logging initialises and it can be patched
	/// Some CLRs will load all required assemblies when the class is entered, not necessarily just the method, so you've got to watch out
	/// </summary>
	internal static class LogArchiver
	{
		internal static void ArchiveLogs() {
			if (!Directory.Exists(Logging.LogArchiveDir)) {
				var pattern = new Regex(".*\\.zip");
				var existingOldLogs = Directory.GetFiles(Logging.LogDir).Where(s => pattern.IsMatch(Path.GetFileName(s))).ToList();

				foreach (string f in existingOldLogs) {
					try {
						File.Delete(f);
					}
					catch (IOException) { }
				}

				Directory.CreateDirectory(Logging.LogArchiveDir);
			}

			foreach (var logFile in Directory.GetFiles(Logging.LogDir, "*.old*"))
				Archive(logFile, Path.GetFileNameWithoutExtension(logFile));

			DeleteOldArchives();
		}

		private static void Archive(string logFile, string entryName) {
			var time = File.GetCreationTime(logFile);
			int n = 1;

			var pattern = new Regex($"{time:yyyy-MM-dd}-(\\d+)\\.zip");
			var existingLogs = Directory.GetFiles(Logging.LogArchiveDir).Where(s => pattern.IsMatch(Path.GetFileName(s))).ToList();
			if (existingLogs.Count > 0)
				n = existingLogs.Select(s => int.Parse(pattern.Match(Path.GetFileName(s)).Groups[1].Value)).Max() + 1;

			using (var zip = new ZipFile(Path.Combine(Logging.LogArchiveDir, $"{time:yyyy-MM-dd}-{n}.zip"), Encoding.UTF8)) {
				using (var stream = File.OpenRead(logFile)) {
					zip.AddEntry(entryName, stream);
					zip.Save();
				}
			}

			File.Delete(logFile);
		}

		private const int MAX_LOGS = 20;
		private static void DeleteOldArchives() {
			var pattern = new Regex(".*\\.zip");
			var existingLogs = Directory.GetFiles(Logging.LogArchiveDir).Where(s => pattern.IsMatch(Path.GetFileName(s))).OrderBy(File.GetCreationTime).ToList();
			foreach (var f in existingLogs.Take(existingLogs.Count - MAX_LOGS)) {
				try {
					File.Delete(f);
				}
				catch (IOException) { }
			}
		}
	}
}
