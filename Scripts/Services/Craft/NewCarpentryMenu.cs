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
        private readonly bool m_isPreAoS;

        public NewCarpentryMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            : base("Select a category to craft:", GetCraftCategories(from))
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_Message = message;
            m_isPreAoS = isPreAoS;

            if (m_Message != 0)
            {
                from.SendLocalizedMessage(m_Message);
            }


            if (m_CraftSystem.GetType() == typeof(DefCarpentry))
            {
                m_CraftSystem = DefClassicCarpentry.CraftSystem;
            }

        }

        private static ItemListEntry[] GetCraftCategories(Mobile from)
        {
            return new ItemListEntry[]
            {
                new ItemListEntry("Chair", 2866),
                new ItemListEntry("Container", 3650),
                new ItemListEntry("Table", 2868),
                new ItemListEntry("Staffs", 3568),
                new ItemListEntry("Shields", 7034),
                new ItemListEntry("Bar", 2921),
                new ItemListEntry("Multiple Skill Item", 5200)
            };
        }

        public override void OnResponse(NetState state, int index)
        {
            switch (index)
            {
                case 0: // Chair
                    m_From.SendMenu(new ChairMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 1: // Container
                    m_From.SendMenu(new ContainerMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 2: // Table
                    m_From.SendMenu(new TableMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 3: // Staffs
                    m_From.SendMenu(new StaffsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 4: // Shields
                    m_From.SendMenu(new ShieldsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 5: // Bar
                    m_From.SendMenu(new BarMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case 6: // Multiple Skill Item
                    m_From.SendMenu(new MultipleSkillItemMenu(m_From, m_CraftSystem, m_Tool));
                    break;
            }
        }


/// <summary>
/// /////////
/// </summary>
/// 


public class ChairMenu : ItemListMenu
{
    private readonly Mobile m_From;
    private readonly CraftSystem m_CraftSystem;
    private readonly BaseTool m_Tool;

    public ChairMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
        : base("Select a chair to craft:", GetChairItems(from, craftSystem))
    {
        m_From = from;
        m_CraftSystem = craftSystem;
        m_Tool = tool;
    }

    private static ItemListEntry[] GetChairItems(Mobile from, CraftSystem craftSystem)
    {
        List<ItemListEntry> entries = new List<ItemListEntry>();

        ItemListEntryWithType[] allChairs = new ItemListEntryWithType[]
        {
            new ItemListEntryWithType("Foot Stool", typeof(FootStool), 2910),
            new ItemListEntryWithType("Stool", typeof(Stool), 2602),
            new ItemListEntryWithType("Bamboo Chair", typeof(BambooChair), 4300),
            new ItemListEntryWithType("Wooden Chair", typeof(WoodenChair), 4301),
            new ItemListEntryWithType("Fancy Wooden Chair Cushion", typeof(FancyWoodenChairCushion), 4302),
            new ItemListEntryWithType("Wooden Chair Cushion", typeof(WoodenChairCushion), 4303),
            new ItemListEntryWithType("Wooden Bench", typeof(WoodenBench), 2860),
            new ItemListEntryWithType("Wooden Throne", typeof(WoodenThrone), 4304),
            new ItemListEntryWithType("Throne", typeof(Throne), 4305)
        };

        foreach (ItemListEntryWithType entry in allChairs)
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

        Console.WriteLine("Chair items count: " + entries.Count); // Debug message
        return entries.ToArray();
    }

    public override void OnResponse(NetState state, int index)
    {
        var itemType = ((ItemListEntryWithType)GetChairItems(m_From, m_CraftSystem)[index]).ItemType;
        CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

        if (craftItem != null)
        {
            Console.WriteLine("Crafting item: " + itemType.Name); // Debug message
            craftItem.Craft(m_From, m_CraftSystem, null, m_Tool);
        }
        else
        {
            Console.WriteLine("The selected item cannot be crafted."); // Debug message
            m_From.SendMessage("The selected item cannot be crafted.");
        }
    }
}



/// <summary>
/// ///////////////
/// </summary>
        public class ContainerMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ContainerMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a container to craft:", GetContainerItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetContainerItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allContainers = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Wooden Box", typeof(WoodenBox), 3709),
                    new ItemListEntryWithType("Small Crate", typeof(SmallCrate), 4309),
                    new ItemListEntryWithType("Medium Crate", typeof(MediumCrate), 4310),
                    new ItemListEntryWithType("Large Crate", typeof(LargeCrate), 4311),
                    new ItemListEntryWithType("Wooden Chest", typeof(WoodenChest), 3650),
                    new ItemListEntryWithType("Empty Bookcase", typeof(EmptyBookcase), 2718),
                    new ItemListEntryWithType("Fancy Armoire", typeof(FancyArmoire), 4312),
                    new ItemListEntryWithType("Armoire", typeof(Armoire), 2643)
                };

                foreach (ItemListEntryWithType entry in allContainers)
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
                var itemType = ((ItemListEntryWithType)GetContainerItems(m_From, m_CraftSystem)[index]).ItemType;
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
                public class TableMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public TableMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a table to craft:", GetTableItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetTableItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allTables = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Writing Table", typeof(WritingTable), 2890),
                    new ItemListEntryWithType("Yew Wood Table", typeof(YewWoodTable), 4307),
                    new ItemListEntryWithType("Large Table", typeof(LargeTable), 4308)
                };

                foreach (ItemListEntryWithType entry in allTables)
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
                var itemType = ((ItemListEntryWithType)GetTableItems(m_From, m_CraftSystem)[index]).ItemType;
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

        public class StaffsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public StaffsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a staff to craft:", GetStaffItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetStaffItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allStaffs = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Shepherd's Crook", typeof(ShepherdsCrook), 3713),
                    new ItemListEntryWithType("Quarter Staff", typeof(QuarterStaff), 3721),
                    new ItemListEntryWithType("Gnarled Staff", typeof(GnarledStaff), 5112),
                    new ItemListEntryWithType("Club", typeof(Club), 5043),
                    new ItemListEntryWithType("Black Staff", typeof(BlackStaff), 3568)
                };

                foreach (ItemListEntryWithType entry in allStaffs)
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
                var itemType = ((ItemListEntryWithType)GetStaffItems(m_From, m_CraftSystem)[index]).ItemType;
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

                public class ShieldsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ShieldsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a shield to craft:", GetShieldItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetShieldItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allShields = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Wooden Shield", typeof(WoodenShield), 7034)
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
            }

            public override void OnResponse(NetState state, int index)
            {
                var itemType = ((ItemListEntryWithType)GetShieldItems(m_From, m_CraftSystem)[index]).ItemType;
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

        public class BarMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public BarMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a bar to craft:", GetBarItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetBarItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allBars = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Board", typeof(Board), 7127),
                    new ItemListEntryWithType("Barrel Staves", typeof(BarrelStaves), 7857),
                    new ItemListEntryWithType("Barrel Lid", typeof(BarrelLid), 7608),
                    new ItemListEntryWithType("Short Music Stand", typeof(ShortMusicStand), 4313),
                    new ItemListEntryWithType("Tall Music Stand", typeof(TallMusicStand), 4315),
                    new ItemListEntryWithType("Easel", typeof(Easle), 4317)
                };

                foreach (ItemListEntryWithType entry in allBars)
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
                var itemType = ((ItemListEntryWithType)GetBarItems(m_From, m_CraftSystem)[index]).ItemType;
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

                public class MultipleSkillItemMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public MultipleSkillItemMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a multiple skill item to craft:", GetMultipleSkillItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetMultipleSkillItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> entries = new List<ItemListEntry>();

                ItemListEntryWithType[] allMultipleSkillItems = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Small Bed South Deed", typeof(SmallBedSouthDeed), 4321),
                    new ItemListEntryWithType("Small Bed East Deed", typeof(SmallBedEastDeed), 4322),
                    new ItemListEntryWithType("Large Bed South Deed", typeof(LargeBedSouthDeed), 4323),
                    new ItemListEntryWithType("Large Bed East Deed", typeof(LargeBedEastDeed), 4324),
                    new ItemListEntryWithType("Dart Board South Deed", typeof(DartBoardSouthDeed), 4325),
                    new ItemListEntryWithType("Dart Board East Deed", typeof(DartBoardEastDeed), 4326),
                    new ItemListEntryWithType("Ballot Box Deed", typeof(BallotBoxDeed), 4327),
                    new ItemListEntryWithType("Pentagram Deed", typeof(PentagramDeed), 4328),
                    new ItemListEntryWithType("Abbatoir Deed", typeof(AbbatoirDeed), 4329),
                    new ItemListEntryWithType("Dressform", typeof(Dressform), 4339),
                    new ItemListEntryWithType("Spinning Wheel East Deed", typeof(SpinningwheelEastDeed), 4341),
                    new ItemListEntryWithType("Spinning Wheel South Deed", typeof(SpinningwheelSouthDeed), 4342),
                    new ItemListEntryWithType("Loom East Deed", typeof(LoomEastDeed), 4343),
                    new ItemListEntryWithType("Loom South Deed", typeof(LoomSouthDeed), 4344),
                    new ItemListEntryWithType("Small Forge Deed", typeof(SmallForgeDeed), 4330),
                    new ItemListEntryWithType("Large Forge East Deed", typeof(LargeForgeEastDeed), 4331),
                    new ItemListEntryWithType("Large Forge South Deed", typeof(LargeForgeSouthDeed), 4332),
                    new ItemListEntryWithType("Anvil East Deed", typeof(AnvilEastDeed), 4333),
                    new ItemListEntryWithType("Anvil South Deed", typeof(AnvilSouthDeed), 4334),
                    new ItemListEntryWithType("Training Dummy East Deed", typeof(TrainingDummyEastDeed), 4335),
                    new ItemListEntryWithType("Training Dummy South Deed", typeof(TrainingDummySouthDeed), 4336),
                    new ItemListEntryWithType("Pickpocket Dip East Deed", typeof(PickpocketDipEastDeed), 4337),
                    new ItemListEntryWithType("Pickpocket Dip South Deed", typeof(PickpocketDipSouthDeed), 4338)
                };

                foreach (ItemListEntryWithType entry in allMultipleSkillItems)
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
                var itemType = ((ItemListEntryWithType)GetMultipleSkillItems(m_From, m_CraftSystem)[index]).ItemType;
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
    }
}