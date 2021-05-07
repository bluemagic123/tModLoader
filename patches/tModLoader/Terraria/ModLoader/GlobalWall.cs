using Microsoft.Xna.Framework.Graphics;

namespace Terraria.ModLoader
{
	/// <summary>
	/// This class allows you to modify the behavior of any wall in the game (although admittedly walls don't have much behavior). Create an instance of an overriding class then call Mod.AddGlobalWall to use this.
	/// </summary>
	public abstract class GlobalWall : ModType
	{
		protected sealed override void Register() {
			ModTypeLookup<GlobalWall>.Register(this);
			WallLoader.globalWalls.Add(this);
		}

		public sealed override void SetupContent() => SetStaticDefaults();

		/// <summary>
		/// Allows you to customize which sound you want to play when the wall at the given coordinates is hit. Return false to stop the game from playing its default sound for the wall. Returns true by default.
		/// </summary>
		public virtual bool KillSound(int i, int j, int type) {
			return true;
		}

		/// <summary>
		/// Allows you to change how many dust particles are created when the wall at the given coordinates is hit.
		/// </summary>
		public virtual void NumDust(int i, int j, int type, bool fail, ref int num) {
		}

		/// <summary>
		/// Allows you to modify the default type of dust created when the wall at the given coordinates is hit. Return false to stop the default dust (the dustType parameter) from being created. Returns true by default.
		/// </summary>
		public virtual bool CreateDust(int i, int j, int type, ref int dustType) {
			return true;
		}

		/// <summary>
		/// Allows you to customize which items the wall at the given coordinates drops. Return false to stop the game from dropping the wall's default item (the dropType parameter). Returns true by default.
		/// </summary>
		public virtual bool Drop(int i, int j, int type, ref int dropType) {
			return true;
		}

		/// <summary>
		/// Allows you to determine what happens when the wall at the given coordinates is killed or hit with a hammer. Fail determines whether the wall is mined (whether it is killed).
		/// </summary>
		public virtual void KillWall(int i, int j, int type, ref bool fail) {
		}

		/// <summary>
		/// Allows you to stop a wall from being placed at the given coordinates. Return false to stop the wall from being placed. Returns true by default.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool CanPlace(int i, int j, int type) {
			return true;
		}

		/// <summary>
		/// Whether or not the wall at the given coordinates can be killed by an explosion (ie. bombs). Returns true by default; return false to stop an explosion from destroying it.
		/// </summary>
		public virtual bool CanExplode(int i, int j, int type) {
			return true;
		}

		/// <summary>
		/// Allows you to determine how much light the wall emits. This can also let you light up the block in front of the wall.
		/// </summary>
		public virtual void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b) {
		}

		/// <summary>
		/// Called for every wall the world randomly decides to update in a given tick. Useful for things such as growing or spreading.
		/// </summary>
		public virtual void RandomUpdate(int i, int j, int type) {
		}

		/// <summary>
		/// Allows you to draw things behind the wall at the given coordinates. Return false to stop the game from drawing the wall normally. Returns true by default.
		/// </summary>
		public virtual bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch) {
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the wall at the given coordinates.
		/// </summary>
		public virtual void PostDraw(int i, int j, int type, SpriteBatch spriteBatch) {
		}

		/// <summary>
		/// Called after this wall type was placed in the world by way of the item provided.
		/// </summary>
		public virtual void PlaceInWorld(int i, int j, int type, Item item) {
		}
	}
}
