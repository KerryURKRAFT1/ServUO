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


    public class NewInscriptionMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;


        public NewInscriptionMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message,  bool isPreAoS)
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
            if (m_CraftSystem.GetType() == typeof(DefInscription) && (Core.AOS || Core.UOR))
            {
                m_CraftSystem = DefClassicInscription.CraftSystem;

            }
            else
            {
                //Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }

                        // Verifica dei materiali
            //if (!HasRequiredMaterials(from, craftSystem))
            //{
              //  from.SendMessage("You do not have the necessary materials to craft any items.");

                //return;
            //}


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
                from.SendMenu(new NewInscriptionMenu(from, craftSystem, tool, message, isPreAoS));
            }




        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> categories = new List<ItemListEntry>();

            if (FirstCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 1", 8384));
            }
            if (SecondCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 2", 8385));
            }
            if (ThirdCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 3", 8386));
            }
            if (FourthCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 4", 8387));               
            }
            if (FifthCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 5", 8388));
            }
            if (SixthCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 6", 8389));
            }
             if (SeventhCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 7", 8390));
            }
            if (EighthCirclenMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Spell Circle 8", 8391));
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
                    case "Spell Circle 1":
                        m_From.SendMenu(new FirstCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Spell Circle 2":
                        m_From.SendMenu(new SecondCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Spell Circle 3":
                        m_From.SendMenu(new ThirdCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Spell Circle 4":
                        m_From.SendMenu(new FourthCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Spell Circle 5":
                        m_From.SendMenu(new FifthCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Spell Circle 6":
                        m_From.SendMenu(new SixthCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                        case "Spell Circle 7":
                        m_From.SendMenu(new SeventhCirclenMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                        case "Spell Circle 8":
                        m_From.SendMenu(new EighthCirclenMenu(m_From, m_CraftSystem, m_Tool));
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

public class FirstCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public FirstCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Scroll Spell to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allFirstCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("ReactiveArmorScroll", typeof(ReactiveArmorScroll), 8326),
                    new ItemListEntryWithType("ClumsyScroll", typeof(ClumsyScroll), 8320),
                    new ItemListEntryWithType("CreateFoodScroll", typeof(CreateFoodScroll), 8321),
                    new ItemListEntryWithType("FeeblemindScroll", typeof(FeeblemindScroll), 8322),
                    new ItemListEntryWithType("HealScroll", typeof(HealScroll), 8323),
                    new ItemListEntryWithType("MagicArrowScroll", typeof(MagicArrowScroll), 8324),
                    new ItemListEntryWithType("NightSightScroll", typeof(NightSightScroll), 8325),
                    new ItemListEntryWithType("WeakenScroll", typeof(WeakenScroll), 8327)
                };

                foreach (ItemListEntryWithType entry in allFirstCircle)
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

public class SecondCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SecondCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allSecondCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("AgilityScroll", typeof(AgilityScroll), 8328),
                    new ItemListEntryWithType("CunningScroll", typeof(CunningScroll), 8329),
                    new ItemListEntryWithType("CureScroll", typeof(CureScroll), 8330),
                    new ItemListEntryWithType("HarmScroll", typeof(HarmScroll), 8331),
                    new ItemListEntryWithType("MagicTrapScroll", typeof(MagicTrapScroll), 8332),
                    new ItemListEntryWithType("MagicUnTrapScroll", typeof(MagicUnTrapScroll), 8333),
                    new ItemListEntryWithType("ProtectionScroll", typeof(ProtectionScroll), 8334),
                    new ItemListEntryWithType("StrengthScroll", typeof(StrengthScroll), 8335),
                };

                foreach (ItemListEntryWithType entry in allSecondCircle)
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
 public class ThirdCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ThirdCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allThirdCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("BlessScroll", typeof(BlessScroll), 8336),
                    new ItemListEntryWithType("FireballScroll", typeof(FireballScroll), 8337),
                    new ItemListEntryWithType("MagicLockScroll", typeof(MagicLockScroll), 8338),
                    new ItemListEntryWithType("PoisonScroll", typeof(PoisonScroll), 8339),
                    new ItemListEntryWithType("TelekinisisScroll", typeof(TelekinisisScroll), 8340),
                    new ItemListEntryWithType("TeleportScroll", typeof(TeleportScroll), 8341),
                    new ItemListEntryWithType("UnlockScroll", typeof(UnlockScroll), 8342),
                    new ItemListEntryWithType("WallOfStoneScroll", typeof(WallOfStoneScroll), 8343)
                };

                foreach (ItemListEntryWithType entry in allThirdCircle)
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
/// // FourthCirce  MENU
/// </summary>


 public class FourthCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public FourthCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allFourthCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("ArchCureScroll", typeof(ArchCureScroll), 8344),
                    new ItemListEntryWithType("ArchProtectionScroll", typeof(ArchProtectionScroll), 8345),
                    new ItemListEntryWithType("CurseScroll", typeof(CurseScroll), 8346),
                    new ItemListEntryWithType("FireFieldScroll", typeof(FireFieldScroll), 8347),
                    new ItemListEntryWithType("GreaterHealScroll", typeof(GreaterHealScroll), 8348),
                    new ItemListEntryWithType("LightningScroll", typeof(LightningScroll), 8349),
                    new ItemListEntryWithType("ManaDrainScroll", typeof(ManaDrainScroll), 8350),
                    new ItemListEntryWithType("RecallScroll", typeof(RecallScroll), 8351)
                };

                foreach (ItemListEntryWithType entry in allFourthCircle)
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

public class FifthCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public FifthCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allFifthCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("BladeSpiritsScroll", typeof(BladeSpiritsScroll), 8352),
                    new ItemListEntryWithType("DispelFieldScroll", typeof(DispelFieldScroll), 8353),
                    new ItemListEntryWithType("IncognitoScroll", typeof(IncognitoScroll), 8354),
                    new ItemListEntryWithType("MagicReflectScroll", typeof(MagicReflectScroll), 8355),
                    new ItemListEntryWithType("MindBlastScroll", typeof(MindBlastScroll), 8356),
                    new ItemListEntryWithType("ParalyzeScroll", typeof(ParalyzeScroll), 8357),
                    new ItemListEntryWithType("PoisonFieldScroll", typeof(PoisonFieldScroll), 8358),
                    new ItemListEntryWithType("SummonCreatureScroll", typeof(SummonCreatureScroll), 8359)
                };

                foreach (ItemListEntryWithType entry in allFifthCircle)
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
        public class SixthCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SixthCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allSixthCircle = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("DispelScroll", typeof(LesserExplosionPotion), 8360),
                    new ItemListEntryWithType("EnergyBoltScroll", typeof(EnergyBoltScroll), 8361),
                    new ItemListEntryWithType("ExplosionScroll", typeof(ExplosionScroll), 8362),
                    new ItemListEntryWithType("InvisibilityScroll", typeof(InvisibilityScroll), 8363),
                    new ItemListEntryWithType("MarkScroll", typeof(MarkScroll), 8364),
                    new ItemListEntryWithType("MassCurseScroll", typeof(MassCurseScroll), 8365),
                    new ItemListEntryWithType("ParalyzeFieldScroll", typeof(ParalyzeFieldScroll), 8366),
                    new ItemListEntryWithType("RevealScroll", typeof(RevealScroll), 8367)
                };

                foreach (ItemListEntryWithType entry in allSixthCircle)
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
/// //  SeventhCirclenMenu POT MENU
/// </summary>


        public class SeventhCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public SeventhCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Potion to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allSeventhCirclen = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("ChainLightningScroll", typeof(ChainLightningScroll), 8368),
                    new ItemListEntryWithType("EnergyFieldScroll", typeof(EnergyFieldScroll), 8369),
                    new ItemListEntryWithType("FlamestrikeScroll", typeof(FlamestrikeScroll), 8370),
                    new ItemListEntryWithType("GateTravelScroll", typeof(GateTravelScroll), 8371),
                    new ItemListEntryWithType("ManaVampireScroll", typeof(ManaVampireScroll), 8372),
                    new ItemListEntryWithType("MassDispelScroll", typeof(MassDispelScroll), 8373),
                    new ItemListEntryWithType("MeteorSwarmScroll", typeof(MeteorSwarmScroll), 8374),
                    new ItemListEntryWithType("PolymorphScroll", typeof(PolymorphScroll), 8375)
                };

                foreach (ItemListEntryWithType entry in allSeventhCirclen)
                {
                    CraftItem craftItem = craftSystem.CraftItems.SearchFor(entry.ItemType);

                    if (craftItem != null)
                    {
                                   
    
                        bool hasRequiredSkills = true;
                        foreach (CraftSkill skill in craftItem.Skills)
                        {
                           
                            if (from.Skills[skill.SkillToMake].Value < skill.MinSkill ) //skill.MinSkill
                            {
                                hasRequiredSkills = false;
                                break;
                            }
        
                        }

                        if (hasRequiredSkills)
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
/// //  EighthCirclenMenu POT MENU
/// </summary>


        public class EighthCirclenMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public EighthCirclenMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a scroll to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allEighthCirclen = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("EarthquakeScroll", typeof(EarthquakeScroll), 8376),
                    new ItemListEntryWithType("EnergyVortexScroll", typeof(EnergyVortexScroll), 8377),
                    new ItemListEntryWithType("ResurrectionScroll", typeof(ResurrectionScroll), 8378),
                    new ItemListEntryWithType("SummonAirElementalScroll", typeof(SummonAirElementalScroll), 8379),
                    new ItemListEntryWithType("SummonDaemonScroll", typeof(SummonDaemonScroll), 8380),
                    new ItemListEntryWithType("SummonEarthElementalScroll", typeof(SummonEarthElementalScroll), 8381),
                    new ItemListEntryWithType("SummonFireElementalScroll", typeof(SummonFireElementalScroll), 8382),
                    new ItemListEntryWithType("SummonWaterElementalScroll", typeof(SummonWaterElementalScroll), 8383)
                };

                foreach (ItemListEntryWithType entry in allEighthCirclen)
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

                            // Controlli di null prima di chiamare Craft
                            if (m_From == null || m_CraftSystem == null || m_Tool == null)
                            {
                                Console.WriteLine("DEBUG: Uno o più parametri sono null in OnResponse.");
                                m_From.SendMessage("Errore interno: impossibile eseguire il crafting.");
                                return;
                            }

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