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

    public class NewCraftingMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool isPreAoS;

        public NewCraftingMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            : base("Select a category to craft:", GetCraftCategories(from))
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_Message = message;
            this.isPreAoS = isPreAoS;

            // Forza l'uso di DefClassicBlacksmithy
            if (m_CraftSystem.GetType() == typeof(DefBlacksmithy))
            {
                m_CraftSystem = DefClassicBlacksmithy.CraftSystem;
            }



            if (m_Message != 0)
            {
                from.SendLocalizedMessage(m_Message);
            }
        }

                private static ItemListEntry[] GetCraftCategories(Mobile from)
                {
                    bool hasOnlyIron = HasOnlyIronIngot(from);
                    bool hasIron = false;
                    bool hasOtherMaterials = false;

                    // Verifica se il giocatore ha sia ferro che altri materiali
                    Container pack = from.Backpack;
                    if (pack != null)
                    {
                        foreach (Item item in pack.Items)
                        {
                            if (item is IronIngot)
                            {
                                hasIron = true;
                            }
                            else if (item is DullCopperIngot || item is ShadowIronIngot || item is CopperIngot || 
                                    item is BronzeIngot || item is GoldIngot || item is AgapiteIngot || 
                                    item is VeriteIngot || item is ValoriteIngot)
                            {
                                hasOtherMaterials = true;
                            }

                            if (hasIron && hasOtherMaterials)
                            {
                                break;
                            }
                        }
                    }

                    if (hasOnlyIron)
                    {
                        return new ItemListEntry[]
                        {
                            new ItemListEntry("Repair", 4015),
                            new ItemListEntry("Smelt", 4017),
                            new ItemListEntry("Shields", 7026),
                            new ItemListEntry("Armor", 5141),
                            new ItemListEntry("Weapons", 5049)
                        };
                    }
                    else if (hasIron && hasOtherMaterials)
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
                    else if (hasOtherMaterials)
                    {
                        return new ItemListEntry[]
                        {
                            new ItemListEntry("Repair", 4015),
                            new ItemListEntry("Smelt", 4017),
                            new ItemListEntry("Special Armor", 5384),
                            new ItemListEntry("Special Weapon", 5183)
                        };
                    }

                    // Default case if none of the conditions are met
                    return new ItemListEntry[0];
                }


                private static bool HasOnlyIronIngot(Mobile from)
                {
                    Container pack = from.Backpack;
                    if (pack == null)
                        return false;

                    bool hasIron = false;
                    bool hasOtherMaterials = false;
                    foreach (Item item in pack.Items)
                    {
                        if (item is IronIngot)
                        {
                            hasIron = true;
                        }
                        else if (item is DullCopperIngot || item is ShadowIronIngot || item is CopperIngot || 
                                item is BronzeIngot || item is GoldIngot || item is AgapiteIngot || 
                                item is VeriteIngot || item is ValoriteIngot)
                        {
                            hasOtherMaterials = true;
                        }

                        if (hasIron && hasOtherMaterials)
                        {
                            break;
                        }
                    }

                    return hasIron && !hasOtherMaterials;
                }



            public override void OnResponse(NetState state, int index)
            {
                switch (index)
                {
                    case 0: // Repair
                        Repair.Do(m_From, m_CraftSystem, m_Tool, isPreAoS);
                        break;
                    case 1: // Smelt
                        Resmelt.Do(m_From, m_CraftSystem, m_Tool, isPreAoS);
                        break;
                    case 2: // Shields
                        m_From.SendMenu(new ShieldsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 3: // Armor
                        m_From.SendMenu(new ArmorMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 4: // Weapons
                        m_From.SendMenu(new WeaponsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 5: // Special Armor
                        m_From.SendMenu(new SpecialArmorMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 6: // Special Weapon
                        m_From.SendMenu(new SpecialWeaponsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                }
            }

        public class ShieldsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS; 

            public ShieldsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a shield to craft:", GetShields(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntry[] GetShields(Mobile from, CraftSystem craftSystem)
            {

                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allShields = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Bronze Shield", typeof(BronzeShield), 7026),
                    new ItemListEntryWithType("Buckler", typeof(Buckler), 7027),
                    new ItemListEntryWithType("Heater Shield", typeof(HeaterShield), 7030),
                    new ItemListEntryWithType("Metal Kite Shield", typeof(MetalKiteShield), 7028),
                    new ItemListEntryWithType("Wooden Kite Shield", typeof(WoodenKiteShield), 7032),
                    new ItemListEntryWithType("Metal Shield", typeof(MetalShield), 7035)
                };

                foreach (ItemListEntryWithType entry in allShields)
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



                /*
                return new ItemListEntry[]
                {
                    new ItemListEntryWithType("Bronze Shield", typeof(BronzeShield), 7026),
                    new ItemListEntryWithType("Buckler", typeof(Buckler), 7027),
                    new ItemListEntryWithType("Heater Shield", typeof(HeaterShield), 7030),
                    new ItemListEntryWithType("Metal Kite Shield", typeof(MetalKiteShield), 7028),
                    new ItemListEntryWithType("Wooden Kite Shield", typeof(WoodenKiteShield), 7032),
                    new ItemListEntryWithType("Metal Shield", typeof(MetalShield), 7035)
                };
                */
                    
            }


                    public override void OnResponse(NetState state, int index)
                    {
                        var itemType = ((ItemListEntryWithType)GetShields(m_From, m_CraftSystem)[index]).ItemType;
                        CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                        // Debugging output
                        Console.WriteLine("Crafting item: " + itemType);
                        Console.WriteLine("Using tool: " + m_Tool);
                        Console.WriteLine("Craft item found: " + (craftItem != null));
                        if (craftItem != null)
                        {
                            craftItem.Craft(m_From, m_CraftSystem, null, m_Tool); // Updated to use 4 parameters
                            // Reopen the correct crafting menu after crafting
                            //if (isPreAoS)
                            //{
                            //    m_From.SendMenu(new NewCraftingMenu(m_From, m_CraftSystem, m_Tool, 0, true));
                            //}
                            //else
                            //{
                            //    m_From.SendMenu(new NewCraftingMenu(m_From, m_CraftSystem, m_Tool, 0, false));
                           // }
                        }
                        else
                        {
                            m_From.SendMessage("The selected item cannot be crafted.");
                        }
                    }
        }

        public class ArmorMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS;

            public ArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select an armor category:", GetArmorCategories())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntry[] GetArmorCategories()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Platemail", 5141),
                    new ItemListEntry("Chainmail", 5060),
                    new ItemListEntry("Ringmail", 5101),
                    new ItemListEntry("Helmets", 5132)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                switch (index)
                {
                    case 0: // Platemail
                        m_From.SendMenu(new PlatemailMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 1: // Chainmail
                        m_From.SendMenu(new ChainmailMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 2: // Ringmail
                        m_From.SendMenu(new RingmailMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 3: // Helmets
                        m_From.SendMenu(new HelmetsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                }
            }

            private class PlatemailMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public PlatemailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a platemail item to craft:", GetPlatemailItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetPlatemailItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Plate Arms", 5143),
                        new ItemListEntry("Plate Gloves", 5144),
                        new ItemListEntry("Plate Gorget", 5139),
                        new ItemListEntry("Plate Legs", 5146),
                        new ItemListEntry("Plate Chest", 5142),
                        new ItemListEntry("Female Plate Chest", 7172)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected platemail item
                }
            }

            private class ChainmailMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public ChainmailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a chainmail item to craft:", GetChainmailItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetChainmailItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Chain Coif", 1025051),
                        new ItemListEntry("Chain Legs", 1025054),
                        new ItemListEntry("Chain Chest", 1025055)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected chainmail item
                }
            }

            private class RingmailMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public RingmailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a ringmail item to craft:", GetRingmailItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetRingmailItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Ringmail Gloves", 1025099),
                        new ItemListEntry("Ringmail Legs", 1025104),
                        new ItemListEntry("Ringmail Arms", 1025103),
                        new ItemListEntry("Ringmail Chest", 1025100)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected ringmail item
                }
            }

            private class HelmetsMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                private readonly bool isPreAoS;

                public HelmetsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a helmet to craft:", GetHelmetsItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetHelmetsItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Bascinet", 1025132),
                        new ItemListEntry("Close Helm", 1025128),
                        new ItemListEntry("Helmet", 1025130),
                        new ItemListEntry("Norse Helm", 1025134),
                        new ItemListEntry("Plate Helm", 1025138)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected helmet
                }
            }
        }

        public class WeaponsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            private readonly bool isPreAoS;

            public WeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a weapon category:", GetWeaponsCategories())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntry[] GetWeaponsCategories()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Swords and Blades", 5050),
                    new ItemListEntry("Axes", 3911),
                    new ItemListEntry("Maces and Hammers", 3932),
                    new ItemListEntry("Spears and Forks", 3719),
                    new ItemListEntry("Polearms", 5183)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                switch (index)
                {
                    case 0: // Swords and Blades
                        m_From.SendMenu(new SwordsAndBladesMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case 1: // Axes
                        m_From.SendMenu(new AxesMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case 2: // Maces and Hammers
                        m_From.SendMenu(new MacesAndHammersMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case 3: // Spears and Forks
                        m_From.SendMenu(new SpearsAndForksMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case 4: // Pole Arms
                        m_From.SendMenu(new PolearmsMenu(m_From, m_CraftSystem, m_Tool));
                        break;    
                }
            }

            private class SwordsAndBladesMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                public SwordsAndBladesMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                    : base("Select a sword or blade to craft:", GetSwordsAndBladesItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                }

                private static ItemListEntry[] GetSwordsAndBladesItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Broadsword", 3934),
                        new ItemListEntry("Cutlass", 5185),
                        new ItemListEntry("Dagger", 3921),
                        new ItemListEntry("Katana", 5119),
                        new ItemListEntry("Kryss", 5121),
                        new ItemListEntry("Longsword", 3937),
                        new ItemListEntry("Scimitar", 5046),
                        new ItemListEntry("Viking Sword", 5049)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected sword or blade
                }
            }

            private class AxesMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                public AxesMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                    : base("Select an axe to craft:", GetAxesItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                }

                private static ItemListEntry[] GetAxesItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Axe", 3913),
                        new ItemListEntry("Battle Axe", 3911),
                        new ItemListEntry("Double Axe", 3915),
                        new ItemListEntry("Executioner's Axe", 3909),
                        new ItemListEntry("Large Battle Axe", 5115),
                        new ItemListEntry("Two-Handed Axe", 5187),
                        new ItemListEntry("War Axe", 5040)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected axe
                }
            }

            private class MacesAndHammersMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                public MacesAndHammersMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                    : base("Select a mace or hammer to craft:", GetMacesAndHammersItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                }

                private static ItemListEntry[] GetMacesAndHammersItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Hammer Pick", 5181),
                        new ItemListEntry("Mace", 3932),
                        new ItemListEntry("Maul", 5179),
                        new ItemListEntry("Smith Hammer", 5092),
                        new ItemListEntry("War Hammer", 5177),
                        new ItemListEntry("War Mace", 5127)

                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected mace or hammer
                }
            }

            private class SpearsAndForksMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                public SpearsAndForksMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                    : base("Select a spear or fork to craft:", GetSpearsAndForksItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                }

                private static ItemListEntry[] GetSpearsAndForksItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Short Spear", 5123),
                        new ItemListEntry("Spear", 3938),
                        new ItemListEntry("War Fork", 5125)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected spear or fork
                }
            }
            private class PolearmsMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;

                public PolearmsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                    : base("Select a Polearms to craft:", GetPolearmsItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                }

                private static ItemListEntry[] GetPolearmsItems()
                {
                    return new ItemListEntry[]
                    {
                        new ItemListEntry("Bardiche", 3917),
                        new ItemListEntry("Halberd", 5182)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    // Implement the logic to craft the selected spear or fork
                }
            }













        }

        public class SpecialArmorMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS;

            public SpecialArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a special armor to craft:", GetSpecialArmors())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
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
            private readonly bool isPreAoS;

            public SpecialWeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a special weapon to craft:", GetSpecialWeapons())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
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