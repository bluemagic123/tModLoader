using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace Terraria.ModLoader
{
	public abstract partial class Mod
	{
		public virtual void ChatInput(string text, ref bool broadcast)
		{
		}

		public virtual void UpdateMusic(ref int music)
		{
		}

		public virtual void HotKeyPressed(string name)
		{
		}

		public virtual void HandlePacket(BinaryReader reader, int whoAmI)
		{
		}

		public virtual bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
		{
			return false;
		}

		public virtual Matrix ModifyTransformMatrix(Matrix Transform)
		{
			return Transform;
		}

		public virtual void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
		}

		public virtual void PostDrawInterface(SpriteBatch spriteBatch)
		{
		}

		public virtual void PostDrawFullscreenMap(ref string mouseText)
		{
		}

		public virtual void PostUpdateInput()
		{
		}
	}

	internal static class ModHooks
	{
		//in Terraria.Main.do_Update after processing chat input call ModHooks.ChatText(Main.chatText);
		//in Terraria.Main.do_Update for if statement checking whether chat can be opened remove Main.netMode == 1
		internal static void ChatInput(string text, ref bool broadcast)
		{
			if (text.Length <= 0) return;
			foreach (var mod in ModLoader.mods.Values)
			{
				mod.ChatInput(text, ref broadcast);
			}
		}
        
		//in Terraria.Main.UpdateMusic before updating music boxes call ModHooks.UpdateMusic(ref this.newMusic);
		internal static void UpdateMusic(ref int music)
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				mod.UpdateMusic(ref music);
			}
		}

		// Pretty much deprecated. 
		internal static void HotKeyPressed()
		{
			foreach (var modHotkey in ModLoader.modHotKeys)
			{
				if (PlayerInput.Triggers.Current.KeyStatus[modHotkey.Value.displayName])
				{
					modHotkey.Value.mod.HotKeyPressed(modHotkey.Value.name);
				}
			}
		}

		internal static Matrix ModifyTransformMatrix(Matrix Transform)
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				Transform = mod.ModifyTransformMatrix(Transform);
			}
			return Transform;
		}

		internal static void PostDrawFullscreenMap(ref string mouseText)
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				mod.PostDrawFullscreenMap(ref mouseText);
			}
		}

		internal static void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				mod.ModifyInterfaceLayers(layers);
			}
		}

		internal static void PostDrawInterface(SpriteBatch spriteBatch)
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				mod.PostDrawInterface(spriteBatch);
			}
		}

		internal static void PostUpdateInput()
		{
			foreach (Mod mod in ModLoader.mods.Values)
			{
				mod.PostUpdateInput();
			}
		}
	}
}
