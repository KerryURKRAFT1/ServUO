using System;
using Server.Gumps;
using Server.Network;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Menus;

namespace Server.Engines.Craft
{
    public class NewCraftingMenu : Gump, IMenu // Assicurati che IMenu sia definito nel namespace corretto
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;

        public NewCraftingMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
            : base(50, 50)
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;

            AddPage(0);
            AddBackground(0, 0, 300, 400, 5054);

            AddLabel(100, 20, 1152, "Select a category to craft:");

            var categories = GetCraftCategories();
            for (int i = 0; i < categories.Length; i++)
            {
                AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                AddLabel(85, 50 + (i * 30), 1152, categories[i]);
            }
        }

        private static string[] GetCraftCategories()
        {
            return new string[]
            {
                "Repair",
                "Smelt",
                "Shields",
                "Armor",
                "Weapons",
                "Special Armor",
                "Special Weapon"
            };
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID - 1)
            {
                case 0: // Repair
                    //Repair.Do(m_From, m_CraftSystem, m_Tool, this);
                    break;
                case 1: // Smelt
                   // Resmelt.Do(m_From, m_CraftSystem, m_Tool, this);
                    break;
                case 2: // Shields
                    m_From.SendGump(new ShieldsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 3: // Armor
                    m_From.SendGump(new ArmorMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 4: // Weapons
                    m_From.SendGump(new WeaponsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 5: // Special Armor
                    m_From.SendGump(new SpecialArmorMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 6: // Special Weapon
                    m_From.SendGump(new SpecialWeaponsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
            }
        }

        public void OnCancel(NetState state)
        {
            // Implement cancel logic if needed
        }

        public int EntryLength
        {
            get { return 7; }
        }

        void IMenu.OnResponse(NetState state, int index)
        {
            //OnResponse(state, new RelayInfo(index, null));
        }

        public class ShieldsMenu : Gump
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ShieldsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base(50, 50)
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 5054);

                AddLabel(100, 20, 1152, "Select a shield to craft:");

                var shields = GetShields(craftSystem);
                for (int i = 0; i < shields.Length; i++)
                {
                    AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    AddLabel(85, 50 + (i * 30), 1152, shields[i]);
                }
            }

            private static string[] GetShields(CraftSystem craftSystem)
            {
                return new string[]
                {
                    "Buckler",
                    "Bronze Shield",
                    "Heater Shield",
                    "Metal Shield",
                    "Metal Kite Shield",
                    "Wooden Kite Shield",
                    "Chaos Shield",
                    "Order Shield"
                };
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                int index = info.ButtonID - 1;
                if (index >= 0 && index < GetShields(m_CraftSystem).Length)
                {
                    // Implement the logic to craft the selected shield
                }
            }
        }

        public class ArmorMenu : Gump
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base(50, 50)
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 5054);

                AddLabel(100, 20, 1152, "Select an armor category:");

                var armors = GetArmorCategories();
                for (int i = 0; i < armors.Length; i++)
                {
                    AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    AddLabel(85, 50 + (i * 30), 1152, armors[i]);
                }
            }

            private static string[] GetArmorCategories()
            {
                return new string[]
                {
                    "Ringmail",
                    "Chainmail",
                    "Platemail"
                };
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                // Implement the logic to open the submenus for each armor category
            }
        }

        public class WeaponsMenu : Gump
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public WeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base(50, 50)
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 5054);

                AddLabel(100, 20, 1152, "Select a weapon to craft:");

                var weapons = GetWeapons(craftSystem);
                for (int i = 0; i < weapons.Length; i++)
                {
                    AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    AddLabel(85, 50 + (i * 30), 1152, weapons[i]);
                }
            }

            private static string[] GetWeapons(CraftSystem craftSystem)
            {
                return new string[]
                {
                    "Sword",
                    "Axe"
                };
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                // Implement the logic to craft the selected weapon
            }
        }

        public class SpecialArmorMenu : Gump
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SpecialArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base(50, 50)
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 5054);

                AddLabel(100, 20, 1152, "Select a special armor to craft:");

                var armors = GetSpecialArmors(craftSystem);
                for (int i = 0; i < armors.Length; i++)
                {
                    AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    AddLabel(85, 50 + (i * 30), 1152, armors[i]);
                }
            }

            private static string[] GetSpecialArmors(CraftSystem craftSystem)
            {
                return new string[]
                {
                    "Dragon Armor"
                };
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                // Implement the logic to craft the selected special armor
            }
        }

        public class SpecialWeaponsMenu : Gump
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SpecialWeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base(50, 50)
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 5054);

                AddLabel(100, 20, 1152, "Select a special weapon to craft:");

                var weapons = GetSpecialWeapons(craftSystem);
                for (int i = 0; i < weapons.Length; i++)
                {
                    AddButton(50, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                    AddLabel(85, 50 + (i * 30), 1152, weapons[i]);
                }
            }

            private static string[] GetSpecialWeapons(CraftSystem craftSystem)
            {
                return new string[]
                {
                    "Magical Sword"
                };
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                // Implement the logic to craft the selected special weapon
            }
        }
    }
}