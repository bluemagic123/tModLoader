using System;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Terraria.ModLoader
{
	internal struct MapEntry
	{
		internal Color color;
		internal LocalizedText name;
		internal ModTranslation translation;
		internal Func<LocalizedText, int, int, LocalizedText> getName;

		internal MapEntry(Color color, LocalizedText name = null)
		{
			if (name == null)
			{
				name = LocalizedText.Empty;
			}
			this.color = color;
			this.name = name;
			this.translation = null;
			this.getName = sameName;
		}

		internal MapEntry(Color color, ModTranslation name)
		{
			this.color = color;
			this.name = null;
			this.translation = name;
			this.getName = sameName;
		}

		internal MapEntry(Color color, LocalizedText name, Func<LocalizedText, int, int, LocalizedText> getName)
		{
			this.color = color;
			this.name = name;
			this.translation = null;
			this.getName = getName;
		}

		private static LocalizedText sameName(LocalizedText name, int x, int y)
		{
			return name;
		}
	}
}
