﻿#nullable enable
using System.Collections.Generic;

namespace Terraria.ModLoader.Container
{
	public abstract class HookedItemStorage : ItemStorage
	{
		protected HookedItemStorage(int size) : base(size) {
		}

		protected HookedItemStorage(IEnumerable<Item> items) : base(items) {
		}

		internal new Item this[int index] {
			get => Items[index];
			set {
				if (Items[index].Equals(value) || Items[index].IsAir && value.IsAir) {
					return;
				}
				OnUpdateItem?.Invoke(index, Items[index], value);
				Items[index] = value;
			}
		}

		public delegate void UpdateItem(int slot, Item oldItem, Item newItem);

		/// <summary>
		/// Fired just before updating an item through the indexer.
		/// </summary>
		public event UpdateItem? OnUpdateItem;

		public delegate void CanInteractDelegate(int slot, Operation operation, object? user, ref bool result);
		public delegate void IsItemValidDelegate(int slot, Item item, ref bool result);
		public delegate void GetSlotSizeDelegate(int slot, Item item, ref int result);

		public event CanInteractDelegate? OnCanInteract;
		public event IsItemValidDelegate? OnIsItemValid;
		public event GetSlotSizeDelegate? OnGetSlotSize;

		public override bool CanInteract(int slot, Operation operation, object? user) {
			bool ret = base.CanInteract(slot, operation, user);
			OnCanInteract?.Invoke(slot, operation, user, ref ret);
			return ret;
		}

		public override bool IsItemValid(int slot, Item item) {
			bool ret = base.IsItemValid(slot, item);
			OnIsItemValid?.Invoke(slot, item, ref ret);
			return ret;
		}

		public override int GetSlotSize(int slot, Item item) {
			int ret = base.GetSlotSize(slot, item);
			OnGetSlotSize?.Invoke(slot, item, ref ret);
			return ret;
		}
	}
}
