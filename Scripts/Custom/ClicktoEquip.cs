using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Equip logic for all wearable items (UOR Sphere-style):
    /// - Supports every wearable layer (not just hands)
    /// - Swaps the equipped item into the same container the new item came from (if possible, otherwise fallback to backpack)
    /// - Handles equip from ground, backpack, nested containers
    /// - Console debug for every swap
    /// </summary>
    public static class ClickToEquip
    {
        public static void OnDoubleClick(Mobile from, Item item)
        {
            // Only allow players
            if (!from.Player)
                return;

            // Usa il container diretto (parent) come destinazione swap
            Container sourceContainer = GetDirectContainer(item);

            // DEBUG: stamp container of origin
            //Console.WriteLine($"[DEBUG] Equip {item.Name ?? item.GetType().Name} da container: {sourceContainer?.GetType().Name ?? "null"} (Serial: {sourceContainer?.Serial})");

            // If item is in backpack or any sub-container inside backpack
            if (IsAccessibleFromBackpack(item, from.Backpack))
            {
                RemoveLayerForEquip(from, item.Layer, sourceContainer);

                // Try to equip the item
                if (from.EquipItem(item))
                {
                    from.PlaySound(0x57);
                }
                else
                {
                    from.SendMessage("You cannot equip this item.");
                }
                return;
            }
            // If item is on the ground and can be picked up
            else if (item.Parent == null && item.Map == from.Map && from.InRange(item.GetWorldLocation(), 2) && item.Movable && from.CanSee(item) && from.InLOS(item))
            {
                if (from.Backpack != null && from.Backpack.TryDropItem(from, item, false))
                {
                    sourceContainer = from.Backpack;
                    RemoveLayerForEquip(from, item.Layer, sourceContainer);

                    if (from.EquipItem(item))
                    {
                        from.PlaySound(0x57);
                    }
                    else
                    {
                        from.SendMessage("You cannot equip this item.");
                    }
                }
                else
                {
                    from.SendMessage("You cannot pick up or equip this item.");
                }
                return;
            }
            // If already equipped, do nothing (handle harvesting etc here if needed)
            else if (from.FindItemOnLayer(item.Layer) == item)
            {
                return;
            }
            else
            {
                from.SendMessage("You must equip the item to use it.");
                return;
            }
        }

        /// <summary>
        /// Checks if item is inside the backpack or any nested container inside the backpack.
        /// </summary>
        private static bool IsAccessibleFromBackpack(Item item, Container backpack)
        {
            Item current = item;
            while (current.Parent is Container container)
            {
                if (container == backpack)
                    return true;
                current = container;
            }
            return item.Parent == backpack;
        }

        /// <summary>
        /// Returns the immediate parent container of the item (bag, pouch, etc).
        /// </summary>
        private static Container GetDirectContainer(Item item)
        {
            return item.Parent as Container;
        }

        /// <summary>
        /// Swap logic for any wearable layer. The item removed goes into the same container as the new item (if possible).
        /// Special handling for hands (OneHanded/TwoHanded/shield).
        /// </summary>
        private static void RemoveLayerForEquip(Mobile from, Layer itemLayer, Container targetContainer)
        {
            SwapItemToContainer(from, from.FindItemOnLayer(itemLayer), targetContainer);

            // Special logic for weapons/shields
            if (itemLayer == Layer.TwoHanded)
            {
                SwapItemToContainer(from, from.FindItemOnLayer(Layer.OneHanded), targetContainer);
                SwapItemToContainer(from, from.FindItemOnLayer(Layer.TwoHanded), targetContainer);
            }
            else if (itemLayer == Layer.OneHanded)
            {
                SwapItemToContainer(from, from.FindItemOnLayer(Layer.OneHanded), targetContainer);
                Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);
                if (twoHanded != null && !(twoHanded is BaseShield) && !(twoHanded is BaseEquipableLight))
                    SwapItemToContainer(from, twoHanded, targetContainer);
            }
        }

        /// <summary>
        /// Tries to move equipped item to targetContainer, fallback to backpack if not possible. Debugs all actions.
        /// </summary>
        private static void SwapItemToContainer(Mobile from, Item equipped, Container targetContainer)
        {
            if (equipped == null)
                return;

            Container destination = targetContainer ?? from.Backpack;
            bool moved = false;

            //Console.WriteLine($"[DEBUG] Swap {equipped.Name ?? equipped.GetType().Name} (Serial: {equipped.Serial}) in {destination?.GetType().Name ?? "null"} (Serial: {destination?.Serial})");

            // Try in the container of origin
            if (destination != null && destination != equipped.Parent && destination.CheckHold(from, equipped, false, true, 0, 0))
            {
                moved = destination.TryDropItem(from, equipped, false);
                //Console.WriteLine($"[DEBUG] Swap in {destination.GetType().Name}: {(moved ? "OK" : "FALLITO")}");
            }

            // If it fails, try the main backpack
            if (!moved && from.Backpack != null && from.Backpack != equipped.Parent)
            {
                moved = from.Backpack.TryDropItem(from, equipped, false);
                //Console.WriteLine($"[DEBUG] Fallback swap in Backpack: {(moved ? "OK" : "FALLITO")}");
            }

            // If all else fails, delete
            if (!moved)
            {
                equipped.Delete();
                //Console.WriteLine("[DEBUG] Delete swap fallito!");
            }
        }
    }
}