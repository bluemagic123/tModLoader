﻿using Microsoft.Xna.Framework;
using System;

namespace Terraria.ModLoader
{
	/// <summary>
	/// This class serves as a way to store information about a menu button, which is the text buttons that appear on the main menu, options screen, etc. <br />
	/// You will create and manipulate objects of this class if you use the ModifyMenuButtons hook.
	/// </summary>
	public class MenuButton
	{
		/// <summary>
		/// Allows you to tell the button what to do when it's clicked.
		/// </summary>
		public Action OnClick;

		/// <summary>
		/// Allows you to tell the button what to do when it's being hovered on.
		/// </summary>
		public Action OnHover;
		
		/// <summary>
		/// The name of the mod that added this menu button. For vanilla buttons, the mod name will be "Terraria".
		/// </summary>
		public readonly string Mod;

		/// <summary>
		/// The name of this menu button, which is used to help identify its function.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The text this menu button displays.
		/// </summary>
		public string text;

		/// <summary>
		/// The button's scale.
		/// </summary>
		public float scale = 1f;

		/// <summary>
		/// Offset position on the y-axis.
		/// </summary>
		public int yOffsetPos = 0;

		/// <summary>
		/// Offset position on the x-axis.
		/// </summary>
		public int xOffsetPos = 0;

		/// <summary>
		/// If set to true, this text will be colored to white and the player will be unable to click on or hover over the button.
		/// </summary>
		public bool readonlyText = true;

		/// <summary>
		/// Similar to <see cref="readonlyText"/>, but just disallows clicking and hovering while still keeping the menu button color the same.
		/// </summary>
		public bool unhoverableText = true;

		/// <summary>
		/// The color of the button text.
		/// </summary>
		public Color color = Color.White;

		internal bool loweredAlpha = false;

		internal bool noCenterOffset = false;

		internal byte colorByte = 0;

		public MenuButton(Mod mod, string name, string text) {
			Mod = mod.Name;
			Name = name;
			this.text = text;
		}

		internal MenuButton(string name, string text) {
			Mod = "Terraria";
			Name = name;
			this.text = text;
		}
	}
}