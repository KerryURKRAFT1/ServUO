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
    public class NewAlchemyMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;


        public NewAlchemyMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message,  bool isPreAoS)
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

            if (m_CraftSystem.GetType() == typeof(DefAlchemy))
            {
                m_CraftSystem = DefClassicAlchemy.CraftSystem;

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
                from.SendMenu(new NewAlchemyMenu(from, craftSystem, tool, message, isPreAoS));
            }




        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> categories = new List<ItemListEntry>();

            if (RefreshPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Refresh Potion", 3851));
            }
            if (HealPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Heal Potion", 3852));
            }
            if (CurePotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Cure Potion", 3847));
            }
            if (AgilityPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Agility Potion", 3848));               
            }
            if (StrenghtPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Strength Potion", 3849));
            }
            if (ExplosionPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Explosion Potion", 3853));
            }
             if (PoisonPotionMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Poison Potion", 3850));
            }
            if (NightSightMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("NightSight Potion", 3846));
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
                    case "Refresh Potion":
                        m_From.SendMenu(new RefreshPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Heal Potion":
                        m_From.SendMenu(new HealPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Cure Potion":
                        m_From.SendMenu(new CurePotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Agility Potion":
                        m_From.SendMenu(new AgilityPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Strength Potion":
                        m_From.SendMenu(new StrenghtPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Explosion Potion":
                        m_From.SendMenu(new ExplosionPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                        case "Poison Potion":
                        m_From.SendMenu(new PoisonPotionMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                        case "NightSight Potion":
                        m_From.SendMenu(new NightSightMenu(m_From, m_CraftSystem, m_Tool));
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
/// // refresh MENU
/// </summary>

public class RefreshPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public RefreshPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allRefreshPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Refresh Potion", typeof(RefreshPotion), 3851),
                    new ItemListEntryWithType("Total Refresh Potion", typeof(TotalRefreshPotion), 3851)
                };

                foreach (ItemListEntryWithType entry in allRefreshPot)
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
/// // heal pot  MENU
/// </summary>

public class HealPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public HealPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allHealPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Lesser Heal Potion", typeof(LesserHealPotion), 3852),
                    new ItemListEntryWithType("Heal Potion", typeof(HealPotion), 3852),
                    new ItemListEntryWithType("Greater Heal Potion", typeof(GreaterHealPotion), 3852)
                };

                foreach (ItemListEntryWithType entry in allHealPot)
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
/// // CURE POT MENU
/// </summary>
 public class CurePotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public CurePotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allCurePot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Lesser Cure Potion", typeof(LesserCurePotion), 3847),
                    new ItemListEntryWithType("Cure Potion", typeof(CurePotion), 3847),
                    new ItemListEntryWithType("Greater Cure Potion", typeof(GreaterCurePotion), 3847)
                };

                foreach (ItemListEntryWithType entry in allCurePot)
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
/// // agility pot MENU
/// </summary>


 public class AgilityPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public AgilityPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allAgilityPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("AgilityPotion", typeof(AgilityPotion), 3848),
                    new ItemListEntryWithType("GreaterAgilityPotion", typeof(GreaterAgilityPotion), 3848)
                };

                foreach (ItemListEntryWithType entry in allAgilityPot)
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
/// // strenght MENU
/// </summary>

public class StrenghtPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public StrenghtPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allStrenghtPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Strength Potion", typeof(StrengthPotion), 3849),
                    new ItemListEntryWithType("Greater Strength Potion", typeof(GreaterStrengthPotion), 3849)
                };

                foreach (ItemListEntryWithType entry in allStrenghtPot)
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
/// // explosion MENU
/// </summary>
        public class ExplosionPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ExplosionPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allExploPotion = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Lesser Explosion Potion", typeof(LesserExplosionPotion), 3853),
                    new ItemListEntryWithType("Explosion Potion", typeof(ExplosionPotion), 3853),
                    new ItemListEntryWithType("Greater Explosion Potion", typeof(GreaterExplosionPotion), 3853)
                };

                foreach (ItemListEntryWithType entry in allExploPotion)
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
/// //  poison POT MENU
/// </summary>


        public class PoisonPotionMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public PoisonPotionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allPoisonPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Lesser Poison Potion", typeof(LesserPoisonPotion), 3850),
                    new ItemListEntryWithType("Poison Potion", typeof(PoisonPotion), 3850),
                    new ItemListEntryWithType("Greater Poison Potion", typeof(GreaterPoisonPotion), 3850),
                    new ItemListEntryWithType("Deadly Poison Potion", typeof(DeadlyPoisonPotion), 3850)
                };

                foreach (ItemListEntryWithType entry in allPoisonPot)
                {
                    CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);

                    if (craftItem != null)
                    {
                                   
    
                        bool hasAllRequiredSkills = true;
                        foreach (CraftSkill skill in craftItem.Skills)
                        {
                           
                            if (from.Skills[skill.SkillToMake].Value < skill.MinSkill ) //skill.MinSkill
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
/// //  NIGHTSIGHT POT MENU
/// </summary>


        public class NightSightMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public NightSightMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allNightPot = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("NightSightPotion", typeof(NightSightPotion), 3846)

                };

                foreach (ItemListEntryWithType entry in allNightPot)
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