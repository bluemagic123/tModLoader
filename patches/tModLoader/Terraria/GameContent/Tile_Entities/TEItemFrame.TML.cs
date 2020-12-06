using System.IO;
using Terraria.ModLoader.IO;

namespace Terraria.GameContent.Tile_Entities
{
	public partial class TEItemFrame
	{
		public override TagCompound Save() => new TagCompound {
			{ "item", ItemIO.Save(item) }
		};

		public override void Load(TagCompound tag) => item = ItemIO.Load(tag.GetCompound("item"));

		public override void NetSend(BinaryWriter writer) => ItemIO.Send(item, writer, true);

		public override void NetReceive(BinaryReader reader) => item = ItemIO.Receive(reader, true);
	}
}
