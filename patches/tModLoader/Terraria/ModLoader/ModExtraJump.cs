using System;
using System.Collections.Generic;

namespace Terraria.ModLoader
{
	/// <summary>
	/// A vanilla jump to facilitate parenting, it's logic is never updated. The order is:
	/// <see cref="Mounts"/>,
	/// <see cref="Sandstorm"/>,
	/// <see cref="Blizzard"/>,
	/// <see cref="Fart"/>,
	/// <see cref="Sail"/>,
	/// <see cref="Cloud"/>
	/// </summary>
	public sealed class VanillaExtraJump : ModExtraJump
	{
		/// <summary>
		/// Represents all mount specific jumps, use that if you want to insert before <see cref="VanillaExtraJump.Sandstorm"/>
		/// </summary>
		public static readonly VanillaExtraJump Mounts = new VanillaExtraJump();
		public static readonly VanillaExtraJump Sandstorm = new VanillaExtraJump();
		public static readonly VanillaExtraJump Blizzard = new VanillaExtraJump();
		public static readonly VanillaExtraJump Fart = new VanillaExtraJump();
		public static readonly VanillaExtraJump Sail = new VanillaExtraJump();
		public static readonly VanillaExtraJump Cloud = new VanillaExtraJump();

		internal static readonly List<ModExtraJump> vanillaJumps = new List<ModExtraJump> { Mounts, Sandstorm, Blizzard, Fart, Sail, Cloud };
	}

	/// <summary>
	/// A ModExtraJump instance represents an addition to a Player instance that handles jumping logic. A ModExtraJump is tied to the player instance, use <see cref="Player.EnableExtraJump"/> to enable a jump like you would enable any other effect through an accessory or armor
	/// </summary>
	public class ModExtraJump : ModType
	{
		/// <summary>
		/// The Player instance that this ModExtraJump instance is attached to.
		/// </summary>
		public Player player {
			get;
			internal set;
		}

		internal int index;

		internal bool hasJumpOption;

		internal bool canJumpAgain;

		public bool IsPerformingJump {
			get;
			internal set;
		}

		internal ModExtraJump CreateFor(Player newPlayer) {
			var ModExtraJump = (ModExtraJump)Activator.CreateInstance(GetType());
			ModExtraJump.Mod = Mod;
			ModExtraJump.player = newPlayer;
			ModExtraJump.index = index;
			return ModExtraJump;
		}

		protected sealed override void Register() {
			ModTypeLookup<ModExtraJump>.Register(this);
			ModExtraJumpLoader.Add(this);
		}

		public bool TypeEquals(ModExtraJump other) => Mod == other.Mod && Name == other.Name;

		public override string ToString() => $"{nameof(hasJumpOption)}: {hasJumpOption}, {nameof(canJumpAgain)}: {canJumpAgain}, {GetType().Name}";

		/// <summary>
		/// Return a <see cref="VanillaExtraJump"/> so that your jump will be performed after it.
		/// Jumps with the same parent and from the same mod will be performed in reverse alphabetic order.
		/// </summary>
		public virtual VanillaExtraJump JumpAfter => VanillaExtraJump.Cloud;

		/// <summary>
		/// Allows you to set the jump height multiplier, disable default double jump sound, and add visuals when this jump is initiated.
		/// </summary>
		/// /// <param name="jumpHeight">Jump height multiplier, 1f by default</param>
		/// <param name="playSound">if left on true, will play the default double jump sound</param>
		public virtual void Jump(ref float jumpHeight, ref bool playSound) {

		}

		/// <summary>
		/// Allows you to create visuals while the jump is happening.
		/// </summary>
		public virtual void PerformingJump() {

		}

		/// <summary>
		/// Allows you to modify the horizontal acceleration and max speed during the jump (e.g. sandstorm uses 1.5f and 2f, blizzard uses 3f and 1.5f).
		/// </summary>
		/// <param name="runAccelerationMult">Horizontal acceleration multiplier</param>
		/// <param name="maxRunSpeedMult">Max speed multiplier</param>
		public virtual void HorizontalJumpSpeed(ref float runAccelerationMult, ref float maxRunSpeedMult) {

		}

		/// <summary>
		/// Allows you to decide if the same jump should be repeated.
		/// </summary>
		public virtual bool CanJumpAgain() => false;
	}
}
