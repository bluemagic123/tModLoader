using Terraria.ModLoader;

namespace ExampleMod.Backgrounds
{
	public class ExampleUgBgStyle : ModUgBgStyle
	{
		//TODO: This currently doesn't work
		public override void FillTextureArray(int[] textureSlots) {
			textureSlots[0] = Mod.GetBackgroundSlot("Assets/Textures/Backgrounds/ExampleBiomeUG0.rawimg");
			textureSlots[1] = Mod.GetBackgroundSlot("Assets/Textures/Backgrounds/ExampleBiomeUG1.rawimg");
			textureSlots[2] = Mod.GetBackgroundSlot("Assets/Textures/Backgrounds/ExampleBiomeUG2.rawimg");
			textureSlots[3] = Mod.GetBackgroundSlot("Assets/Textures/Backgrounds/ExampleBiomeUG3.rawimg");
		}
	}
}