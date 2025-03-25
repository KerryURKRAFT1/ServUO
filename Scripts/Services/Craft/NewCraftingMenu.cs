using System;
using Server.Gumps;
using Server.Network;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Menus.ItemLists;

namespace Server.Engines.Craft
{
    public class NewCraftingMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;

        public NewCraftingMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            : base("Select a category to craft:", GetCraftCategories())
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_Message = message;
            m.isPreAoS = isPreAoS;

            if (m_Message != 0)
            {
                from.SendLocalizedMessage(m_Message);
            }
        }

        private static ItemListEntry[] GetCraftCategories()
        {
            return new ItemListEntry[]
            {
                new ItemListEntry("Repair", 4015),
                new ItemListEntry("Smelt", 4017),
                new ItemListEntry("Shields", 7026),
                new ItemListEntry("Armor", 5141),
                new ItemListEntry("Weapons", 5049),
                new ItemListEntry("Special Armor", 5384),
                new ItemListEntry("Special Weapon", 5183)
            };
        }

        public override void OnResponse(NetState state, int index)
        {
            switch (index)
            {
                case 0: // Repair
                    //Repair.Do(m_From, m_CraftSystem, m_Tool);
                    // AGGIORNATO PER RENAISSANCE
                    Repair.Do(from, craftSystem, tool, isPreAoS); // Aggiungi isPreAoS come parametro appropriato
                    break;
                case 1: // Smelt
                    //Resmelt.Do(m_From, m_CraftSystem, m_Tool);
                    // AGGIORNATO PER RENAISSANCE
                    Resmelt.Do(from, craftSystem, tool, isPreAoS); // Aggiungi isPreAoS come parametro appropriato
                    break;
                case 2: // Shields
                    m_From.SendMenu(new ShieldsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 3: // Armor
                    m_From.SendMenu(new ArmorMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 4: // Weapons
                    m_From.SendMenu(new WeaponsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 5: // Special Armor
                    m_From.SendMenu(new SpecialArmorMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 6: // Special Weapon
                    m_From.SendMenu(new SpecialWeaponsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
            }
        }

        public class ShieldsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ShieldsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a shield to craft:", GetShields())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetShields()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Buckler", 7026),
                    new ItemListEntry("Bronze Shield", 7030),
                    new ItemListEntry("Heater Shield", 7029),
                    new ItemListEntry("Metal Shield", 7031),
                    new ItemListEntry("Metal Kite Shield", 7028),
                    new ItemListEntry("Wooden Kite Shield", 7032),
                    new ItemListEntry("Chaos Shield", 7033),
                    new ItemListEntry("Order Shield", 7034)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                // Implement the logic to craft the selected shield
            }
        }

        public class ArmorMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an armor category:", GetArmorCategories())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetArmorCategories()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Ringmail", 5141),
                    new ItemListEntry("Chainmail", 5055),
                    new ItemListEntry("Platemail", 5142)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                // Implement the logic to open the submenus for each armor category
            }
        }

        public class WeaponsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public WeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a weapon to craft:", GetWeapons())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetWeapons()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Sword", 5049),
                    new ItemListEntry("Axe", 5050)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                // Implement the logic to craft the selected weapon
            }
        }

        public class SpecialArmorMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SpecialArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a special armor to craft:", GetSpecialArmors())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetSpecialArmors()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Dragon Armor", 5141)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                // Implement the logic to craft the selected special armor
            }
        }

        public class SpecialWeaponsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SpecialWeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a special weapon to craft:", GetSpecialWeapons())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetSpecialWeapons()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Magical Sword", 5049)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                // Implement the logic to craft the selected special weapon
            }
        }
    }
}