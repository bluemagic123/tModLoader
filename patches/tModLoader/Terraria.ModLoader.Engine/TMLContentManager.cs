using System;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Terraria.ModLoader.Engine
{
	internal class TMLContentManager : ContentManager
	{
		private readonly TMLContentManager alternateContentManager;
		private int loadedAssets = 0;

		public TMLContentManager(IServiceProvider serviceProvider, string rootDirectory, TMLContentManager alternateContentManager) : base(serviceProvider, rootDirectory) {
			this.alternateContentManager = alternateContentManager;
		}

		protected override Stream OpenStream(string assetName) {
			if (!assetName.StartsWith("tmod:")) {
				if (alternateContentManager != null && File.Exists(Path.Combine(alternateContentManager.RootDirectory, assetName + ".xnb"))) { 
					try {
						return alternateContentManager.OpenStream(assetName);
					}
					catch {}
				}
				return base.OpenStream(assetName);
			}

			if (!assetName.EndsWith(".xnb"))
				assetName += ".xnb";

			return ModContent.OpenRead(assetName);
		}

		public override T Load<T>(string assetName) {

			// default Load implementation is just ReadAsset with a cache. Don't cache tML assets, because then we'd have to clear the cache on mod loading.
			// Mods use Mod.GetFont/GetEffect rather than ContentManager.Load directly anyway, so Load should only be called once per mod load by tML.
			if (assetName.StartsWith("tmod:"))
				return ReadAsset<T>(assetName, null);

			loadedAssets++;
			if (loadedAssets % 1000 == 0)
				Logging.Terraria.Info($"Loaded {loadedAssets} vanilla assets");

			return base.Load<T>(assetName);
		}

		public bool ImageExists(string assetName)
		{
			return File.Exists(Path.Combine(RootDirectory, "Image", assetName + ".xnb")) || alternateContentManager != null && File.Exists(Path.Combine(alternateContentManager.RootDirectory, "Image", assetName + ".xnb"));
		}
	}
}