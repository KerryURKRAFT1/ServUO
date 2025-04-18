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
/// <summary>
/// //////////////////
/// </summary>
    

public class NewCarpentryMenu : ItemListMenu
{
    private readonly Mobile m_From;
    private readonly CraftSystem m_CraftSystem;
    private readonly BaseTool m_Tool;
    private readonly int m_Message;
    private readonly bool m_isPreAoS;

    public NewCarpentryMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
        : base("Select a category to craft:", GetCraftCategories(from, craftSystem))
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
        // Verifica dei materiali

        if (!HasRequiredMaterials(from, craftSystem))
        {
            from.SendMessage("You do not have the necessary materials to craft any items.");
            return;
        }
        
    }


                /// <summary>
        /// Metodo wrapper per verificare i materiali e aprire il menu se i materiali sono sufficienti.
        /// </summary>
        public static void OpenMenuWithMaterialCheck(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
        {
            // Verifica dei materiali
            if (!HasRequiredMaterials(from, craftSystem))
            {
                from.SendMessage("You do not have the necessary materials to craft any items.");
                return;
            }

            // Se i materiali sono sufficienti, apri il menu
            from.SendMenu(new NewInscriptionMenu(from, craftSystem, tool, message, isPreAoS));
        }



    // class to check materials
    private static bool HasRequiredMaterials(Mobile from, CraftSystem craftSystem)
{
    foreach (CraftItem craftItem in craftSystem.CraftItems)
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
            return true;
        }
    }

    return false;
}


    private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
    {
        List<ItemListEntry> categories = new List<ItemListEntry>();

        if (ChairMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Chair", 2866));
            Console.WriteLine("Added category: Chair (index " + (categories.Count - 1) + ")");
        }
        if (ContainerMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Container", 3650));
            Console.WriteLine("Added category: Container (index " + (categories.Count - 1) + ")");
        }
        if (TableMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Table", 2868));
            Console.WriteLine("Added category: Table (index " + (categories.Count - 1) + ")");
        }
        if (StaffsMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Staffs", 3568));
            Console.WriteLine("Added category: Staffs (index " + (categories.Count - 1) + ")");
        }
        if (ShieldsMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Shields", 7034));
            Console.WriteLine("Added category: Shields (index " + (categories.Count - 1) + ")");
        }
        if (BarMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Bar", 2921));
            Console.WriteLine("Added category: Bar (index " + (categories.Count - 1) + ")");
        }
        if (MultipleSkillItemMenu.HasCraftableItems(from, craftSystem))
        {
            categories.Add(new ItemListEntry("Multiple Skill Item", 5200));
            Console.WriteLine("Added category: Multiple Skill Item (index " + (categories.Count - 1) + ")");
        }

        return categories.ToArray();
    }

    public override void OnResponse(NetState state, int index)
    {
        Console.WriteLine("Selected category index: " + index);

        var categories = GetCraftCategories(m_From, m_CraftSystem);
        if (index >= 0 && index < categories.Length)
        {
            var category = categories[index].Name;
            Console.WriteLine("Selected category: " + category);

            switch (category)
            {
                case "Chair":
                    Console.WriteLine("Opening ChairMenu");
                    m_From.SendMenu(new ChairMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Container":
                    Console.WriteLine("Opening ContainerMenu");
                    m_From.SendMenu(new ContainerMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Table":
                    Console.WriteLine("Opening TableMenu");
                    m_From.SendMenu(new TableMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Staffs":
                    Console.WriteLine("Opening StaffsMenu");
                    m_From.SendMenu(new StaffsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Shields":
                    Console.WriteLine("Opening ShieldsMenu");
                    m_From.SendMenu(new ShieldsMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Bar":
                    Console.WriteLine("Opening BarMenu");
                    m_From.SendMenu(new BarMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                case "Multiple Skill Item":
                    Console.WriteLine("Opening MultipleSkillItemMenu");
                    m_From.SendMenu(new MultipleSkillItemMenu(m_From, m_CraftSystem, m_Tool));
                    break;
                default:
                    Console.WriteLine("Invalid category: " + category);
                    m_From.SendMessage("Invalid selection.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid category index: " + index);
            m_From.SendMessage("Invalid selection.");
        }
    }




    


/// <summary>
/// //////////// chair menu
/// </summary>


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

        Console.WriteLine("ChairMenu initialized.");

        if (GetChairItems(from, craftSystem).Length == 0)
        {
            from.SendMessage("You do not have the necessary materials to craft any chairs.");
            from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
        }
    }

    private static ItemListEntry[] GetChairItems(Mobile from, CraftSystem craftSystem)
    {
        List<ItemListEntry> entries = new List<ItemListEntry>();

        ItemListEntryWithType[] allChairs = new ItemListEntryWithType[]
        {
            new ItemListEntryWithType("Foot Stool", typeof(FootStool), 2910),
            new ItemListEntryWithType("Stool", typeof(Stool), 2602),
            new ItemListEntryWithType("Bamboo Chair", typeof(BambooChair), 2907),
            new ItemListEntryWithType("Wooden Chair", typeof(WoodenChair), 2903),
            new ItemListEntryWithType("Fancy Wooden Chair Cushion", typeof(FancyWoodenChairCushion), 2895),
            new ItemListEntryWithType("Wooden Chair Cushion", typeof(WoodenChairCushion), 2899),
            new ItemListEntryWithType("Wooden Bench", typeof(WoodenBench), 2860),
            new ItemListEntryWithType("Wooden Throne", typeof(WoodenThrone), 4304),
            new ItemListEntryWithType("Throne", typeof(Throne), 2862)
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
                        Console.WriteLine("Added item: " + entry.ItemType.Name);
                    }
                }
            }
        }

        Console.WriteLine("GetChairItems found " + entries.Count + " items.");
        return entries.ToArray();
    }

    public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
    {
        var items = GetChairItems(from, craftSystem);
        return items.Length > 0;
    }

    public override void OnResponse(NetState state, int index)
    {
        var items = GetChairItems(m_From, m_CraftSystem);
        if (index >= 0 && index < items.Length)
        {
            var itemType = ((ItemListEntryWithType)items[index]).ItemType;
            Console.WriteLine("Selected item type: " + itemType.Name);

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
        else
        {
            m_From.SendMessage("Invalid selection.");
        }
    }
}




// Altri menu come ContainerMenu, TableMenu, StaffsMenu, ShieldsMenu, BarMenu, MultipleSkillItemMenu non modificati
    




/// <summary>
/// ///////////////
/// </summary>
/// CONTAINER MENU
/// 



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

            if (GetContainerItems(from, craftSystem).Length == 0)
            {
                from.SendMessage("You do not have the necessary materials to craft any containers.");
                from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
            }
        }

        private static ItemListEntry[] GetContainerItems(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> entries = new List<ItemListEntry>();

            ItemListEntryWithType[] allContainers = new ItemListEntryWithType[]
            {
                new ItemListEntryWithType("Wooden Box", typeof(WoodenBox), 2474),
                new ItemListEntryWithType("Small Crate", typeof(SmallCrate), 2473),
                new ItemListEntryWithType("Medium Crate", typeof(MediumCrate), 3647),
                new ItemListEntryWithType("Large Crate", typeof(LargeCrate), 3645),
                new ItemListEntryWithType("Wooden Chest", typeof(WoodenChest), 3651),
                new ItemListEntryWithType("Empty Bookcase", typeof(EmptyBookcase), 2717),
                new ItemListEntryWithType("Fancy Armoire", typeof(FancyArmoire), 2637),
                new ItemListEntryWithType("Armoire", typeof(Armoire), 2639)
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

        public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
        {
            var items = GetContainerItems(from, craftSystem);
            return items.Length > 0;
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



/// <summary>
/// /taBLE MENU
/// </summary>


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

            if (GetTableItems(from, craftSystem).Length == 0)
            {
                from.SendMessage("You do not have the necessary materials to craft any tables.");
                from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
            }
        }

        private static ItemListEntry[] GetTableItems(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> entries = new List<ItemListEntry>();

            ItemListEntryWithType[] allTables = new ItemListEntryWithType[]
            {
                new ItemListEntryWithType("Writing Table", typeof(WritingTable), 2890),
                new ItemListEntryWithType("Yew Wood Table", typeof(YewWoodTable), 2959),
                new ItemListEntryWithType("Large Table", typeof(LargeTable), 2960)
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

        public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
        {
            var items = GetTableItems(from, craftSystem);
            return items.Length > 0;
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



/// <summary>
/// //////////staff menu
/// </summary>
    


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

            if (GetStaffItems(from, craftSystem).Length == 0)
            {
                from.SendMessage("You do not have the necessary materials to craft any staffs.");
                from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
            }
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

        public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
        {
            var items = GetStaffItems(from, craftSystem);
            return items.Length > 0;
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




//////////////////
///// shield menu
//////
                

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

        if (GetShieldItems(from, craftSystem).Length == 0)
        {
            from.SendMessage("You do not have the necessary materials to craft any shields.");
            from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
        }
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

    public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
    {
        var items = GetShieldItems(from, craftSystem);
        return items.Length > 0;
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


/// <summary>
/// ////////// bar menu
/// </summary>


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

        Console.WriteLine("BarMenu initialized.");

        var items = GetBarItems(from, craftSystem);
        if (items.Length == 0)
        {
            from.SendMessage("You do not have the necessary materials to craft any bars.");
            from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
        }
        else
        {
            Console.WriteLine("BarMenu found " + items.Length + " items.");
        }
    }

    private static ItemListEntry[] GetBarItems(Mobile from, CraftSystem craftSystem)
    {
        List<ItemListEntry> entries = new List<ItemListEntry>();

        ItemListEntryWithType[] allBars = new ItemListEntryWithType[]
        {
            new ItemListEntryWithType("Board", typeof(Board), 7127),
            new ItemListEntryWithType("Barrel Staves", typeof(BarrelStaves), 7857),
            new ItemListEntryWithType("Barrel Lid", typeof(BarrelLid), 7608),
            new ItemListEntryWithType("Short Music Stand", typeof(ShortMusicStand), 3766),
            new ItemListEntryWithType("Tall Music Stand", typeof(TallMusicStand), 3771),
            new ItemListEntryWithType("Easel", typeof(Easle), 3945)
        };

        foreach (ItemListEntryWithType entry in allBars)
        {
            CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);
            Console.WriteLine("Checking item: " + entry.ItemType.Name);

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
                        Console.WriteLine("Added item: " + entry.ItemType.Name);
                    }
                }
            }
        }

        Console.WriteLine("GetBarItems found " + entries.Count + " items.");
        return entries.ToArray();
    }

    public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
    {
        var items = GetBarItems(from, craftSystem);
        return items.Length > 0;
    }

    public override void OnResponse(NetState state, int index)
    {
        var items = GetBarItems(m_From, m_CraftSystem);
        if (index >= 0 && index < items.Length)
        {
            var itemType = ((ItemListEntryWithType)items[index]).ItemType;
            Console.WriteLine("Selected item type: " + itemType.Name);

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
        else
        {
            m_From.SendMessage("Invalid selection.");
        }
    }
}


/// <summary>
/// //////////// multiple menu
/// 
/// </summary>
                


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

        if (GetMultipleSkillItems(from, craftSystem).Length == 0)
        {
            from.SendMessage("You do not have the necessary materials to craft any multiple skill items.");
            from.SendGump(new CraftGump(from, craftSystem, tool, 0)); // Torna al gump di crafting
        }
    }

    private static ItemListEntry[] GetMultipleSkillItems(Mobile from, CraftSystem craftSystem)
    {
        List<ItemListEntry> entries = new List<ItemListEntry>();

        ItemListEntryWithType[] allMultipleSkillItems = new ItemListEntryWithType[]
        {
            new ItemListEntryWithType("Small Bed South Deed", typeof(SmallBedSouthDeed), 5360),
            new ItemListEntryWithType("Small Bed East Deed", typeof(SmallBedEastDeed), 5360),
            new ItemListEntryWithType("Large Bed South Deed", typeof(LargeBedSouthDeed), 5360),
            new ItemListEntryWithType("Large Bed East Deed", typeof(LargeBedEastDeed), 5360),
            new ItemListEntryWithType("Dart Board South Deed", typeof(DartBoardSouthDeed), 5360),
            new ItemListEntryWithType("Dart Board East Deed", typeof(DartBoardEastDeed), 5360),
            new ItemListEntryWithType("Ballot Box Deed", typeof(BallotBoxDeed), 5360),
            new ItemListEntryWithType("Pentagram Deed", typeof(PentagramDeed), 5360),
            new ItemListEntryWithType("Abbatoir Deed", typeof(AbbatoirDeed), 5360),
            new ItemListEntryWithType("Dressform", typeof(Dressform), 3782),
            new ItemListEntryWithType("Spinning Wheel East Deed", typeof(SpinningwheelEastDeed), 5360),
            new ItemListEntryWithType("Spinning Wheel South Deed", typeof(SpinningwheelSouthDeed), 5360),
            new ItemListEntryWithType("Loom East Deed", typeof(LoomEastDeed), 5360),
            new ItemListEntryWithType("Loom South Deed", typeof(LoomSouthDeed), 5360),
            new ItemListEntryWithType("Small Forge Deed", typeof(SmallForgeDeed), 5360),
            new ItemListEntryWithType("Large Forge East Deed", typeof(LargeForgeEastDeed), 4331),
            new ItemListEntryWithType("Large Forge South Deed", typeof(LargeForgeSouthDeed), 4332),
            new ItemListEntryWithType("Anvil East Deed", typeof(AnvilEastDeed), 5360),
            new ItemListEntryWithType("Anvil South Deed", typeof(AnvilSouthDeed), 5360),
            new ItemListEntryWithType("Training Dummy East Deed", typeof(TrainingDummyEastDeed), 5360),
            new ItemListEntryWithType("Training Dummy South Deed", typeof(TrainingDummySouthDeed), 5360),
            new ItemListEntryWithType("Pickpocket Dip East Deed", typeof(PickpocketDipEastDeed), 5360),
            new ItemListEntryWithType("Pickpocket Dip South Deed", typeof(PickpocketDipSouthDeed), 5360)
        };

        foreach (ItemListEntryWithType entry in allMultipleSkillItems)
        {
            CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);

            if (craftItem != null)
            {
                bool hasAllRequiredSkills = true;
                foreach (CraftSkill skill in craftItem.Skills)
                {
                    if (from.Skills[skill.SkillToMake].Value < skill.MinSkill)
                    {
                        hasAllRequiredSkills = false;
                        break;
                    }
                }

                if (hasAllRequiredSkills)
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

    public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
    {
        var items = GetMultipleSkillItems(from, craftSystem);
        return items.Length > 0;
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

    




    ///////
    /// fine
    ///
    }
}