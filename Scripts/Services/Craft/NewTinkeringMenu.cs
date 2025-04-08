using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Menus.ItemLists;
using Server.Factions;

namespace Server.Engines.Craft
{


    public class NewTinkeringMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool m_isPreAoS;

        public NewTinkeringMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
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


            if (m_CraftSystem.GetType() == typeof(DefTinkering))
            {
                m_CraftSystem = DefClassictTinkering.CraftSystem;
            }
            else
            {
                Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }

          
           
            // Verifica dei materiali
            if (!HasRequiredMaterials(from, craftSystem))
            {
                from.SendMessage("You do not have the necessary materials to craft any items.");
                return;
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

        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            List<ItemListEntry> categories = new List<ItemListEntry>();

            if (UtensilsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Utensils", 7867));
            }
            if (PartsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Parts", 4175));
            }
            if (ToolsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Tools", 3897));
            }
            if (MultiMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Multi Component", 4171));
            }
            if (MiscMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Miscellaneous", 6225));
            }
            if (JewelryMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Jewelry", 4230));
            }
            if (TaverntypesMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Tavern Types", 4013));
            }
            if (WoodItemsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Wood Items", 4173));
            }
            if (TrapsMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Traps", 4407));
            }
            if (LightMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Lightsource", 3633));
            }
            if (DecoMenu.HasCraftableItems(from, craftSystem))
            {
                categories.Add(new ItemListEntry("Decorative", 5384));
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
                    case "Utensils":
                        m_From.SendMenu(new UtensilsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Parts":
                        m_From.SendMenu(new PartsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Tools":
                        m_From.SendMenu(new ToolsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Multi Component":
                        m_From.SendMenu(new MultiMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Miscellaneous":
                        m_From.SendMenu(new MiscMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Jewelry":
                        m_From.SendMenu(new JewelryMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Tavern Types":
                        m_From.SendMenu(new TaverntypesMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Wood Items":
                        m_From.SendMenu(new WoodItemsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Traps":
                        m_From.SendMenu(new TrapsMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Lightsource":
                        m_From.SendMenu(new LightMenu(m_From, m_CraftSystem, m_Tool));
                        break;
                    case "Decorative":
                        m_From.SendMenu(new DecoMenu(m_From, m_CraftSystem, m_Tool));
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

        // Definizione delle classi interne per ogni categoria
    
/// <summary>
/// /////// UTENSIL
/// </summary>


        public class UtensilsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public UtensilsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select an utensil to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allUtensils = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Butcher Knife", typeof(ButcherKnife), 5110),
                    new ItemListEntryWithType("Spoon Left", typeof(SpoonLeft), 2552),
                    new ItemListEntryWithType("Spoon Right", typeof(SpoonRight), 2553),
                    new ItemListEntryWithType("Plate", typeof(Plate), 2519),
                    new ItemListEntryWithType("Fork Left", typeof(ForkLeft), 2548),
                    new ItemListEntryWithType("Fork Right", typeof(ForkRight), 2549),
                    new ItemListEntryWithType("Cleaver", typeof(Cleaver), 3779),
                    new ItemListEntryWithType("Knife Left", typeof(KnifeLeft), 2550),
                    new ItemListEntryWithType("Knife Right", typeof(KnifeRight), 2551),
                    new ItemListEntryWithType("Goblet", typeof(Goblet), 2458),
                    new ItemListEntryWithType("Skinning Knife", typeof(SkinningKnife), 3780)
                };

                foreach (ItemListEntryWithType entry in allUtensils)
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
        /// ///////// part part
        /// 
        /// </summary>



        public class PartsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public PartsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a part to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allParts = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Gears", typeof(Gears), 4179),
                    new ItemListEntryWithType("Clock Parts", typeof(ClockParts), 4175),
                    new ItemListEntryWithType("Barrel Tap", typeof(BarrelTap), 4100),
                    new ItemListEntryWithType("Springs", typeof(Springs), 4189),
                    new ItemListEntryWithType("Sextant Parts", typeof(SextantParts), 4185),
                    new ItemListEntryWithType("Barrel Hoops", typeof(BarrelHoops), 7607),
                    new ItemListEntryWithType("Hinge", typeof(Hinge), 4181),
                    new ItemListEntryWithType("Keg", typeof(Keg), 3711),

                    //new ItemListEntryWithType("Bola Ball", typeof(BolaBall), 3699),
                    //new ItemListEntryWithType("Jeweled Filigree", typeof(JeweledFiligree), 2894)
                };

                foreach (ItemListEntryWithType entry in allParts)
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



////////////////
//////  TOOL PART
///


        public class ToolsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public ToolsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a tool to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allTools = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Scissors", typeof(Scissors), 3998),
                    new ItemListEntryWithType("Mortar Pestle", typeof(MortarPestle), 3739),
                    new ItemListEntryWithType("Scorp", typeof(Scorp), 4327),
                    new ItemListEntryWithType("Tinker Tools", typeof(TinkerTools), 4164),
                    new ItemListEntryWithType("Hatchet", typeof(Hatchet), 3907),
                    new ItemListEntryWithType("Draw Knife", typeof(DrawKnife), 4324),
                    new ItemListEntryWithType("Sewing Kit", typeof(SewingKit), 3997),
                    new ItemListEntryWithType("Saw", typeof(Saw), 4148),
                    new ItemListEntryWithType("Dovetail Saw", typeof(DovetailSaw), 4136),
                    new ItemListEntryWithType("Froe", typeof(Froe), 4325),
                    new ItemListEntryWithType("Shovel", typeof(Shovel), 3898),
                    new ItemListEntryWithType("Hammer", typeof(Hammer), 4138),
                    new ItemListEntryWithType("Tongs", typeof(Tongs), 4028),
                    new ItemListEntryWithType("Smith Hammer", typeof(SmithHammer), 5091),
                    new ItemListEntryWithType("Sledge Hammer", typeof(SledgeHammer), 4021),
                    new ItemListEntryWithType("Inshave", typeof(Inshave), 4326),
                    new ItemListEntryWithType("Pickaxe", typeof(Pickaxe), 3718),
                    new ItemListEntryWithType("Lockpick", typeof(Lockpick), 5372),
                    //new ItemListEntryWithType("Skillet", typeof(Skillet), 4567),
                    new ItemListEntryWithType("Flour Sifter", typeof(FlourSifter), 4158),
                    new ItemListEntryWithType("Fletcher Tools", typeof(FletcherTools), 4130),
                    new ItemListEntryWithType("Mapmaker's Pen", typeof(MapmakersPen), 4167),
                    new ItemListEntryWithType("Scribe's Pen", typeof(ScribesPen), 4031),
                    //new ItemListEntryWithType("Clippers", typeof(Clippers), 1112117)
                };

                foreach (ItemListEntryWithType entry in allTools)
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
/// /////////
///multi component item
/// </summary>
/// 

    
        public class MultiMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public MultiMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a miscellaneous item to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allMulti = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("AxleGears", typeof(AxleGears), 4177),
                    new ItemListEntryWithType("ClockRight", typeof(ClockRight), 4171),
                    new ItemListEntryWithType("ClockLeft", typeof(ClockLeft), 4172),
                    new ItemListEntryWithType("PotionKeg", typeof(PotionKeg), 6464)

                };

                foreach (ItemListEntryWithType entry in allMulti)
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
/// ////////// MISC PART
/// </summary>



        public class MiscMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public MiscMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a miscellaneous item to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allMisc = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Key Ring", typeof(KeyRing), 4113),
                    new ItemListEntryWithType("Candelabra", typeof(Candelabra), 2599),
                    new ItemListEntryWithType("Scales", typeof(Scales), 6225),
                    new ItemListEntryWithType("Key", typeof(Key), 4112),
                    new ItemListEntryWithType("Globe", typeof(Globe), 4167),
                    new ItemListEntryWithType("Spyglass", typeof(Spyglass), 5365),
                };

                foreach (ItemListEntryWithType entry in allMisc)
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
/// ///////  JEWELS PART
/// </summary>

        public class JewelryMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public JewelryMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select jewelry to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allJewelry = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Ecru Citrine Ring", typeof(EcruCitrineRing), 4231),
                    new ItemListEntryWithType("Blue Diamond Ring", typeof(BlueDiamondRing), 4232),
                    new ItemListEntryWithType("Fire Ruby Bracelet", typeof(FireRubyBracelet), 4230),
                    new ItemListEntryWithType("Perfect Emerald Ring", typeof(PerfectEmeraldRing), 4230),
                    new ItemListEntryWithType("Turqouise Ring", typeof(TurqouiseRing), 4230),
                    new ItemListEntryWithType("White Pearl Bracelet", typeof(WhitePearlBracelet), 4230)
                };

                foreach (ItemListEntryWithType entry in allJewelry)
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
/// //tavern types
/// 
/// </summary>

        public class TaverntypesMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public TaverntypesMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a items to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allTavern = new ItemListEntryWithType[]
                {
                    // inserire item
                    new ItemListEntryWithType("Pewter Mug", typeof(PewterMug), 4095),
                    new ItemListEntryWithType("Pitcher", typeof(Pitcher), 4086),
                    new ItemListEntryWithType("Spitton", typeof(DecoSpittoon), 4099),
                    new ItemListEntryWithType("Backgammon", typeof(Backgammon), 4013),
                    new ItemListEntryWithType("Backgammon", typeof(Backgammon), 3612),
                    new ItemListEntryWithType("Chessboard", typeof(Chessboard), 4006),
                    new ItemListEntryWithType("Dices", typeof(Dices), 4007),
                };

                foreach (ItemListEntryWithType entry in allTavern)
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
/// //////// WOOD ITEMS
/// </summary>
/// 

        public class WoodItemsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public WoodItemsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a items to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allWoodItems = new ItemListEntryWithType[]
                {
                    // inserire item
                    new ItemListEntryWithType("Jointing Plane", typeof(JointingPlane), 4144),
                    new ItemListEntryWithType("Moulding Plane", typeof(MouldingPlane), 4140),
                    new ItemListEntryWithType("Smoothing Plane", typeof(SmoothingPlane), 4146),
                    new ItemListEntryWithType("Clock Frame", typeof(ClockFrame), 4173),
                    new ItemListEntryWithType("Axle", typeof(Axle), 4187),
                    new ItemListEntryWithType("Rolling Pin", typeof(RollingPin), 4163),
                };

                foreach (ItemListEntryWithType entry in allWoodItems)
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
/// //////// traps menu
/// </summary>
/// 

        public class TrapsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public TrapsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a trap to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allTraps = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Dart Trap", typeof(DartTrapCraft), 4398),
                    new ItemListEntryWithType("Poison Trap", typeof(PoisonTrapCraft), 4414),
                    new ItemListEntryWithType("Explosion Trap", typeof(ExplosionTrapCraft), 4347),
                    //new ItemListEntryWithType("Faction Gas Trap Deed", typeof(FactionGasTrapDeed), 4598),
                    //new ItemListEntryWithType("Faction Explosion Trap Deed", typeof(FactionExplosionTrapDeed), 4599),
                    //new ItemListEntryWithType("Faction Saw Trap Deed", typeof(FactionSawTrapDeed), 4600),
                    //new ItemListEntryWithType("Faction Spike Trap Deed", typeof(FactionSpikeTrapDeed), 4601),
                    //new ItemListEntryWithType("Faction Trap Removal Kit", typeof(FactionTrapRemovalKit), 6445)
                };

                foreach (ItemListEntryWithType entry in allTraps)
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
/// ///// light menu
/// </summary>
/// 
/// 
               
               
         public class LightMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public LightMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Light Source to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allLight = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Lantern", typeof(Lantern), 2597),
                    new ItemListEntryWithType("Heating Stand", typeof(HeatingStand), 6217),
                    new ItemListEntryWithType("Brazier", typeof(Brazier), 3633),
                    new ItemListEntryWithType("Brazier Tall", typeof(BrazierTall), 6570),
                };

                foreach (ItemListEntryWithType entry in allLight)
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
/// /// deco menu
/// </summary>

                public class DecoMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;

            public DecoMenu(Mobile from, CraftSystem craftSystem, BaseTool tool)
                : base("Select a Decorative Item to craft:", GetCraftItems(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                

            }

            private static ItemListEntry[] GetCraftItems(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntry> items = new List<ItemListEntry>();

                ItemListEntryWithType[] allDeco = new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Decorative Axe North", typeof(DecorativeAxeNorth), 5473),
                    new ItemListEntryWithType("Decorative Axe West", typeof(DecorativeAxeWest), 5474),
                    new ItemListEntryWithType("Decorative D Axe North", typeof(DecorativeDAxeNorth), 5480),
                    new ItemListEntryWithType("Decorative D Axe West", typeof(DecorativeDAxeWest), 5482),
                    new ItemListEntryWithType("Decorative SwordNorth", typeof(DecorativeSwordNorth), 5477),
                    new ItemListEntryWithType("Decorative SwordWest", typeof(DecorativeSwordWest), 5478)

                };

                foreach (ItemListEntryWithType entry in allDeco)
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


////////


    }
}

