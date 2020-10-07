﻿using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;

namespace Terraria.ModLoader.Default.Developer.Jofairden
{
	internal class AndromedonHeadShader : AndromedonShader
	{
		private static Asset<Texture2D> _shaderTexture;

		public override bool IsHeadLayer => true;

		public override DrawDataInfo GetData(PlayerDrawSet info) {
			_shaderTexture ??= ModContent.GetTexture("ModLoader/Developer.Jofairden.PowerRanger_Head_Head_Shader");

			return GetHeadDrawDataInfo(info, _shaderTexture.Value);
		}

		public override void GetDefaults(Player drawPlayer, out bool visible, out LayerConstraint constraint) {
			base.GetDefaults(drawPlayer, out visible, out _);

			constraint = new LayerConstraint(Head, true);
		}
	}
}
