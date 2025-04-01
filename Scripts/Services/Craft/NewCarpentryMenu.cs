using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Menus.ItemLists;

namespace Server.Engines.Craft
{
    public class ItemListEntryWithType : ItemListEntry
    {
        public Type ItemType { get; set; }

        public ItemListEntryWithType(string name, Type itemType, int itemID) : base(name, itemID)
        {
            ItemType = itemType;
        }
    }

    public class NewCarpentryMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool isPreAoS;

        public NewCarpentryMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            : base("Select a category to craft:", GetCraftCategories(from))
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_Message = message;
            this.isPreAoS = isPreAoS;

            // Forza l'uso di DefClassicCarpentry
            if (m_CraftSystem.GetType() == typeof(DefCarpentry))
            {
                m_CraftSystem = DefClassicCarpentry.CraftSystem;
            }

            if (m_Message != 0)
            {
                from.SendLocalizedMessage(m_Message);
            }
        }

        private static ItemListEntry[] GetCraftCategories(Mobile from)
        {
            return new ItemListEntry[]
            {
                new ItemListEntry("Furniture", 7026),
                new ItemListEntry("Containers", 5141),
                new ItemListEntry("Weapons", 5049),
                new ItemListEntry("Miscellaneous", 5384)
            };
        }

        public override void OnResponse(NetState state, int index)
        {
            switch (index)
            {
                case 0: // Furniture
                    m_From.SendMenu(new FurnitureMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case 1: // Containers
                    m_From.SendMenu(new ContainersMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case 2: // Weapons
                    m_From.SendMenu(new WeaponsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case 3: // Miscellaneous
                    m_From.SendMenu(new MiscellaneousMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
            }
        }

        // Define additional menus for each category following the same structure as in NewCraftingMenu.cs
        // Example for Furniture menu
        public class FurnitureMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS;

            public FurnitureMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a furniture item to craft:", GetFurnitureItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntry[] GetFurnitureItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allFurniture = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Wooden Chair", typeof(WoodenChair), 7026),
                    new ItemListEntryWithType("Fancy Wooden Chair", typeof(FancyWoodenChair), 7027),
                    new ItemListEntryWithType("Wooden Stool", typeof(WoodenStool), 7030)
                };

                foreach (ItemListEntryWithType entry in allFurniture)
                {
                    CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);

                    if (craftItem != null)
                    {
                        bool hasRequiredSkill = false;
                        foreach (CraftSkill skill in craftItem.Skills)
                        {
                            if (from.Skills[skill.SkillToMake].Value >= skill.MinSkill)
                            {
                                hasRequiredSkill = true;
                                break;
                            }
                        }

                        if (hasRequiredSkill)
                        {
                            bool hasMaterials = true;
                            foreach (CraftRes craftRes in craftItem.Resources)
                            {
                                if (from.Backpack.GetAmount(craftRes.ItemType) < craftRes.Amount)
                                {
                                    hasMaterials = false;
                                    break;
                                }
                            }

                            if (hasMaterials)
                            {
                                entries.Add(entry);
                            }
                        }
                    }
                }

                return entries.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var itemType = ((ItemListEntryWithType)GetFurnitureItems(m_From, m_CraftSystem)[index]).ItemType;
                CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                if (craftItem != null)
                {
                    craftItem.Craft(m_From, m_CraftSystem, null, m_Tool);
                }
                else
                {
                    m_From.SendMessage("The selected item cannot be crafted.");
                }
            }
        }

        // Similar classes for ContainersMenu, WeaponsMenu, MiscellaneousMenu can be added here
    }
}