﻿namespace Terraria.ModLoader.Default.Patreon
{
	internal class toplayz_Head : PatreonItem
	{
		public override string PatreonName => "toplayz";
		public override EquipType PatreonEquipType => EquipType.Head;

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.width = 28;
			item.height = 26;
		}
	}

	internal class toplayz_Body : PatreonItem
	{
		public override string PatreonName => "toplayz";
		public override EquipType PatreonEquipType => EquipType.Body;

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.width = 30;
			item.height = 20;
		}
	}

	internal class toplayz_Legs : PatreonItem
	{
		public override string PatreonName => "toplayz";
		public override EquipType PatreonEquipType => EquipType.Legs;

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.width = 22;
			item.height = 18;
		}
	}
}
