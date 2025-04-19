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
    public class NewCartographyMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;


        public NewCartographyMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message,  bool isPreAoS)
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

            //if (m_CraftSystem.GetType() == typeof(DefInscription))
            if (m_CraftSystem.GetType() == typeof(DefCartography) && (Core.AOS || Core.UOR))
            {
                m_CraftSystem = DefCartography.CraftSystem;

            }
            else
            {
                //Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }




                        // Verifica dei materiali
            if (!HasRequiredMaterials(from, craftSystem))
            {
                from.SendMessage("You do not have the necessary materials or Skill to craft any items aaaaaaaaa.");

                return;
            }


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
            from.SendMenu(new NewCartographyMenu(from, craftSystem, tool, message, isPreAoS));
        }

        /// <summary>
        /// // Controlla dal tool i materiali
        /// </summary>


            public static void CreateMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            {
                var categories = GetCraftCategories(from, craftSystem);

                // Se l'array delle categorie è vuoto, non creare il menu
                if (categories.Length == 0)
                {
                    from.SendMessage("You do not have the necessary materials to craft any items.");
                    return; // Non mostrare il menu
                }

                // Altrimenti, crea il menu normalmente
                from.SendMenu(new NewCartographyMenu(from, craftSystem, tool, message, isPreAoS));
            }
            

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

            if (LocalMapMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Local Map", 6511));
            }
            if (CityMapnMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("City Map", 6512));
            }
            if (SeaChartMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Sea Chart", 6513));
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
                    case "Local Map":
                        m_From.SendMenu(new LocalMapMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "City Map":
                        m_From.SendMenu(new CityMapnMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Sea Chart":
                        m_From.SendMenu(new SeaChartMenu(m_From, m_CraftSystem, m_Tool));
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

public class LocalMapMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public LocalMapMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Map to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allLocalMap = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Local Map", typeof(LocalMap), 6511)
                };

                foreach (ItemListEntryWithType entry in allLocalMap)
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
/// // City Mapn Menu
/// </summary>

public class CityMapnMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public CityMapnMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a map to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allCitymap = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("City Map", typeof(CityMap), 6312),
                };

                foreach (ItemListEntryWithType entry in allCitymap)
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
/// // SeaChartMenu MENU
/// </summary>
 public class SeaChartMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SeaChartMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select map to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allSeachart = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("SeaChart", typeof(SeaChart), 6513),

                };

                foreach (ItemListEntryWithType entry in allSeachart)
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