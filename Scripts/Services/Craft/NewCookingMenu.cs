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
    public class NewCookingMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;


        public NewCookingMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message,  bool isPreAoS)
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


            if (m_CraftSystem.GetType() == typeof(DefCooking) && (Core.AOS || Core.UOR))
            {
                m_CraftSystem = DefClassicCooking.CraftSystem;

            }
            else
            {
                //Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }



        }

        private static bool HasRequiredMaterials(Mobile from, CraftSystem craftSystem)
        {
            Console.WriteLine("ENTRA IN HASMATERIAL");
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




        public static void OpenMenuWithMaterialCheck(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
        {
            // Verifica dei materiali
            if (!HasRequiredMaterials(from, craftSystem))
            {
                
                from.SendMessage("You do not have the necessary materials to craft any items.");
                return;
            }

            // Se i materiali sono sufficienti, apri il menu
            from.SendMenu(new NewCookingMenu(from, craftSystem, tool, message, isPreAoS));
        }

        /// <summary>
        /// // Controlla dal tool i materiali
        /// </summary>


            public static void CreateMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            {

                var categories = GetCraftCategories(from, craftSystem);

                // Se non ci sono categorie disponibili, procedi comunque con l'apertura del menu
                if (categories.Length == 0)
                {
                    from.SendMessage("You do not have the necessary materials to craft any items, but the menu will still open.");
                    return;
                }

                // Apri il menu normalmente, indipendentemente dalle categorie disponibili
                from.SendMenu(new NewCookingMenu(from, craftSystem, tool, message, isPreAoS));
            }




        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> categories = new List<ItemListEntry>();


            if (IngredientsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Ingredients", 4157));
            }
            if (PreparationsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Preparations", 4162));
            }
            if (BakingMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Baking", 6513));
            }
            if (WorldMapMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("World Map", 6514));               
            }
           
            return categories.ToArray();
        }

        public override void OnResponse(NetState state, int index)
        {
            var categories = GetCraftCategories(m_From, m_CraftSystem);

            if (index >= 0 && index < categories.Length)
            {
                var category = categories[index].Name;



                switch (category)
                {
                    case "Ingredients":
                        m_From.SendMenu(new IngredientsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Preparations":
                        m_From.SendMenu(new PreparationsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Baking":
                        m_From.SendMenu(new BakingMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "World Map":
                        m_From.SendMenu(new WorldMapMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    default:
                        m_From.SendMessage("Invalid selection.");
                        break;
                }
            }
            else
            {
                m_From.SendMessage("Invalid selection.");
            }
        }


/// <summary>
/// // Local Map Menu
/// </summary>

public class IngredientsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public IngredientsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an ingredient to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allIngredients = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Dough", typeof(Dough), 4157),
                    new ItemListEntryWithType("SweetDough", typeof(SweetDough), 4157),
                    new ItemListEntryWithType("CakeMix", typeof(CakeMix), 4159),
                    new ItemListEntryWithType("CookieMix", typeof(CookieMix), 4159)

                };


                foreach (ItemListEntryWithType entry in allIngredients)
                {
                    Console.WriteLine("DEBUG: Checking ingredient: {0}");

                    CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);

                    if (craftItem != null)
                    {
                         Console.WriteLine("DEBUG: Found CraftItem for {0}.", entry.Name);
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
                                                    int availableAmount = from.Backpack.GetAmount(craftRes.ItemType);
                    Console.WriteLine("DEBUG: Checking material: {0}, Required: {1}, Available: {2}",
                        craftRes.ItemType.Name, craftRes.Amount, availableAmount);
                 
                                if (from.Backpack.GetAmount(craftRes.ItemType) < craftRes.Amount)
                                {
                                    hasMaterials = false;
                                    Console.WriteLine("DEBUG: Not enough materials for: {0}", craftRes.ItemType.Name);
                                    break;
                                }
                            }

                            if (hasMaterials)
                            {
                                Console.WriteLine("DEBUG: Ingredient {0} is craftable.");
                                items.Add(entry);
                            }
                        }
                    }
                }

                return items.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var items = GetCraftItems(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    var itemType = ((ItemListEntryWithType)items[index]).ItemType;
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

            public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
            {
                var items = GetCraftItems(from, craftSystem);
                return items.Length > 0;
            }
        }



/// <summary>
/// // Preparations Menu
/// </summary>

public class PreparationsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public PreparationsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a map to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allPreparations = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("UnbakedQuiche", typeof(UnbakedQuiche), 4162),
                    new ItemListEntryWithType("UnbakedMeatPie", typeof(UnbakedMeatPie), 4162),
                    new ItemListEntryWithType("UncookedSausagePizza", typeof(UncookedSausagePizza), 4227),
                    new ItemListEntryWithType("UncookedCheesePizza", typeof(UncookedCheesePizza), 4227),
                    new ItemListEntryWithType("UnbakedFruitPie", typeof(UnbakedFruitPie), 4162),
                    new ItemListEntryWithType("UnbakedPeachCobbler", typeof(UnbakedPeachCobbler), 4162),
                    new ItemListEntryWithType("UnbakedApplePie", typeof(UnbakedApplePie), 4162),
                    new ItemListEntryWithType("UnbakedPumpkinPie", typeof(UnbakedPumpkinPie), 4162)

                };

                foreach (ItemListEntryWithType entry in allPreparations)
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
                                items.Add(entry);
                            }
                        }
                    }
                }

                return items.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var items = GetCraftItems(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    var itemType = ((ItemListEntryWithType)items[index]).ItemType;
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

            public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
            {
                var items = GetCraftItems(from, craftSystem);
                return items.Length > 0;
            }
        }



/// 
/// <summary>
/// // Baking MENU
/// </summary>
 public class BakingMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public BakingMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select bred to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allBaking = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),










                };

                foreach (ItemListEntryWithType entry in allBaking)
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
                                items.Add(entry);
                            }
                        }
                    }
                }

                return items.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var items = GetCraftItems(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    var itemType = ((ItemListEntryWithType)items[index]).ItemType;
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

            public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
            {
                var items = GetCraftItems(from, craftSystem);
                return items.Length > 0;
            }
        }



/// <summary>
/// // WorldMapMenu  MENU
/// </summary>


 public class WorldMapMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public WorldMapMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a map to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allWorldmap = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("World Map", typeof(WorldMap), 6314),

                };

                foreach (ItemListEntryWithType entry in allWorldmap)
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
                                items.Add(entry);
                            }
                        }
                    }
                }

                return items.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var items = GetCraftItems(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    var itemType = ((ItemListEntryWithType)items[index]).ItemType;
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

            public static bool HasCraftableItems(Mobile from, CraftSystem craftSystem)
            {
                var items = GetCraftItems(from, craftSystem);
                return items.Length > 0;
            }
        }

/// <summary>
/// // FifthCircle  MENU
/// </summary>





/// <summary>
/// // explosion MENU
/// </summary>
        


/// <summary>
/// //  SeventhCirclenMenu POT MENU
/// </summary>


       



/// <summary>
/// //  EighthCirclenMenu POT MENU
/// </summary>


        

            

        /////////
    }
}