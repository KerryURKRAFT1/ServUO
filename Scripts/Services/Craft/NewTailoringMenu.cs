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
    public class NewTailoringMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;


        public NewTailoringMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message,  bool isPreAoS)
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

            if (m_CraftSystem.GetType() == typeof(DefTailoring))
            {
                m_CraftSystem = DefClassicTailoring.CraftSystem;
            }
            else
            {
                Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }

                        // Verifica dei materiali
            if (!HasRequiredMaterials(from, craftSystem))
            {
                from.SendMessage("You do not have the necessary materials to craft any items.");

                //return;
            }


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
                from.SendMenu(new NewTailoringMenu(from, craftSystem, tool, message, isPreAoS));
            }




        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> categories = new List<ItemListEntry>();

            if (ShirtMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Shirt", 5399));
            }
            if (PantsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Pants", 5433));
            }
            if (HatsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Hats", 5916));
            }
            if (MiscMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Misc", 5437));
            }
            if (LeatherMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Leather Armor", 5068));
            }
            if (StuddedMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Studded Armor", 5083));
            }
            if (FootMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("FootWwear", 5899));
            }
            if (MiscBagMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Misc Leather", 3701));
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
                    case "Shirt":
                        m_From.SendMenu(new ShirtMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Pants":
                        m_From.SendMenu(new PantsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Hats":
                        m_From.SendMenu(new HatsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Misc":
                        m_From.SendMenu(new MiscMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Leather Armor":
                        m_From.SendMenu(new LeatherMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Studded Armor":
                        m_From.SendMenu(new StuddedMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "FootWwear":
                        m_From.SendMenu(new FootMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Misc Leather":
                        m_From.SendMenu(new MiscBagMenu(m_From, m_CraftSystem, m_Tool));
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
/// // SHIRT MENU
/// </summary>

public class ShirtMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ShirtMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allShirt = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Doublet", typeof(Doublet), 8059),
                    new ItemListEntryWithType("Shirt", typeof(Shirt), 5399),
                    new ItemListEntryWithType("Fancy Shirt", typeof(FancyShirt), 7933),
                    new ItemListEntryWithType("Tunic", typeof(Tunic), 8097),
                    new ItemListEntryWithType("Surcoat", typeof(Surcoat), 8089),
                    new ItemListEntryWithType("Plain Dress", typeof(PlainDress), 7937),
                    new ItemListEntryWithType("Fancy Dress", typeof(FancyDress), 7936),
                    new ItemListEntryWithType("Cloak", typeof(Cloak), 5397),
                    new ItemListEntryWithType("Robe", typeof(Robe), 7939),
                    new ItemListEntryWithType("Jester Suit", typeof(JesterSuit), 8095)
                };

                foreach (ItemListEntryWithType entry in allShirt)
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
/// // PANTS MENU
/// </summary>

public class PantsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public PantsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allPants = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Short Pants", typeof(ShortPants), 5422),
                    new ItemListEntryWithType("Long Pants", typeof(LongPants), 5433),
                    new ItemListEntryWithType("Kilt", typeof(Kilt), 5431),
                    new ItemListEntryWithType("Skirt", typeof(Skirt), 5398)
                };

                foreach (ItemListEntryWithType entry in allPants)
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
/// // HATS MENU
/// </summary>
 public class HatsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public HatsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allHats = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("SkullCap", typeof(SkullCap), 5444),
                    new ItemListEntryWithType("Bandana", typeof(Bandana), 5440),
                    new ItemListEntryWithType("FloppyHat", typeof(FloppyHat), 5907),
                    new ItemListEntryWithType("Cap", typeof(Cap), 5909),
                    new ItemListEntryWithType("Wide Brim Hat", typeof(WideBrimHat), 5908),
                    new ItemListEntryWithType("Straw Hat", typeof(StrawHat), 5911),
                    new ItemListEntryWithType("Tall Straw Hat", typeof(TallStrawHat), 5910),
                    new ItemListEntryWithType("Wizards Hat", typeof(WizardsHat), 5912),
                    new ItemListEntryWithType("Bonnet", typeof(Bonnet), 5913),
                    new ItemListEntryWithType("Feathered Hat", typeof(FeatheredHat), 5914),
                    new ItemListEntryWithType("Tricorne Hat", typeof(TricorneHat), 5915),
                    new ItemListEntryWithType("JesterHat", typeof(JesterHat), 5916)
                };

                foreach (ItemListEntryWithType entry in allHats)
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
/// // MISC MENU
/// </summary>


 public class MiscMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public MiscMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allMiscCloth = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Body Sash", typeof(BodySash), 5441),
                    new ItemListEntryWithType("Half Apron", typeof(HalfApron), 5435),
                    new ItemListEntryWithType("Full Apron", typeof(FullApron), 5437)
                };

                foreach (ItemListEntryWithType entry in allMiscCloth)
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
/// // LEATHER MENU
/// </summary>

public class LeatherMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public LeatherMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allLeather = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("LeatherGorget", typeof(LeatherGorget), 5063),
                    new ItemListEntryWithType("LeatherCap", typeof(LeatherCap), 7609),
                    new ItemListEntryWithType("LeatherGloves", typeof(LeatherGloves), 5062),
                    new ItemListEntryWithType("LeatherArms", typeof(LeatherArms), 5069),
                    new ItemListEntryWithType("LeatherLegs", typeof(LeatherLegs), 5067),
                    new ItemListEntryWithType("LeatherChest", typeof(LeatherChest), 5068)
                };

                foreach (ItemListEntryWithType entry in allLeather)
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
/// // STUDDED MENU
/// </summary>
        public class StuddedMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public StuddedMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allStudded = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("StuddedGorget", typeof(StuddedGorget), 5078),
                    new ItemListEntryWithType("StuddedGloves", typeof(StuddedGloves), 5077),
                    new ItemListEntryWithType("StuddedArms", typeof(StuddedArms), 5084),
                    new ItemListEntryWithType("LeatherLegs", typeof(LeatherLegs), 5082),
                    new ItemListEntryWithType("StuddedChest", typeof(StuddedChest), 5083)
                };

                foreach (ItemListEntryWithType entry in allStudded)
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
/// // footwear MENU
/// </summary>


public class FootMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public FootMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allFootItems = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Sandals", typeof(Sandals), 5901),
                    new ItemListEntryWithType("Shoes", typeof(Shoes), 5903),
                    new ItemListEntryWithType("Boots", typeof(Boots), 5899),
                    new ItemListEntryWithType("ThighBoots", typeof(ThighBoots), 5905)
                };

                foreach (ItemListEntryWithType entry in allFootItems)
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
/// // MISC BAG LEATHER
/// </summary>


public class MiscBagMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public MiscBagMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allBagItem = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Pouch", typeof(Pouch), 3705),
                    new ItemListEntryWithType("Bag", typeof(Bag), 3702),
                    new ItemListEntryWithType("Backpack", typeof(Backpack), 3701),
                };

                foreach (ItemListEntryWithType entry in allBagItem)
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


            

        /////////
    }
}