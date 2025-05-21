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


    public class NewBlacksmithyMenu : ItemListMenu
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly int m_Message;
        private readonly bool isPreAoS;

        public NewBlacksmithyMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, int message, bool isPreAoS)
            : base("Select a category to craft:", GetCraftCategories(from, craftSystem))
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_Message = message;
            this.isPreAoS = isPreAoS;

            if (m_Message != 0)
            {
                from.SendLocalizedMessage(m_Message);
            }

            // Forza l'uso di DefClassicBlacksmithy
            if (m_CraftSystem.GetType() == typeof(DefBlacksmithy))
            {
                m_CraftSystem = DefClassicBlacksmithy.CraftSystem;
            }
            else
            {
                //Console.WriteLine("Using CraftSystem alternatio of type: " + m_CraftSystem.GetType().Name);
            }

            // Verifica dei materiali
            //if (!HasRequiredMaterials(from, craftSystem))
            //{
            //    from.SendMessage("You do not have the necessary materials or Skill to craft any items aaaaaaaaa.");

            //    return;
            //}


        }

        private static void SetSelectedResource(Mobile from, CraftSystem craftSystem, Type materialType)
        {
                var context = craftSystem.GetContext(from);
                if (context != null && materialType != null)
                {
                for (int i = 0; i < craftSystem.CraftSubRes.Count; i++)
                {
                    if (craftSystem.CraftSubRes.GetAt(i).ItemType == materialType)
                    {
                        context.LastResourceIndex = i;
                        return;
                    }
                }
            }
        }


        private static bool HasRequiredMaterials(Mobile from, CraftSystem craftSystem)
        {
            // Recupera il materiale selezionato dal context (se esiste)
            CraftContext context = craftSystem.GetContext(from);
            Type selectedMaterialType = null;
            if (context != null && context.LastResourceIndex >= 0 && context.LastResourceIndex < craftSystem.CraftSubRes.Count)
                selectedMaterialType = craftSystem.CraftSubRes.GetAt(context.LastResourceIndex).ItemType;

            foreach (CraftItem craftItem in craftSystem.CraftItems)
            {
                bool hasMaterials = true;
                foreach (CraftRes craftRes in craftItem.Resources)
                {
                    Type requiredType = craftRes.ItemType;
                    if (requiredType == typeof(IronIngot) && selectedMaterialType != null)
                        requiredType = selectedMaterialType;

                    if (from.Backpack.GetAmount(requiredType, true) < craftRes.Amount)
                    {
                        hasMaterials = false;
                        break;
                    }
                }
                if (hasMaterials)
                    return true;
            }
            return false;
        }
        /*
                        private static bool HasRequiredMaterials(Mobile from, CraftSystem craftSystem)
                        {
                            foreach (CraftItem craftItem in craftSystem.CraftItems)
                            {
                                bool hasMaterials = true;
                                foreach (CraftRes craftRes in craftItem.Resources)
                                {
                                    if (from.Backpack.GetAmount(craftRes.ItemType, true) < craftRes.Amount)
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
                        }*/



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
            from.SendMenu(new NewBlacksmithyMenu(from, craftSystem, tool, message, isPreAoS));
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
            from.SendMenu(new NewBlacksmithyMenu(from, craftSystem, tool, message, isPreAoS));
        }




        private static ItemListEntry[] GetCraftCategories(Mobile from, CraftSystem craftSystem)
        {
            bool hasOnlyIron = HasOnlyIronIngot(from);
            bool hasIron = false;
            bool hasOtherMaterials = false;

            // Verifica se il giocatore ha sia ferro che altri materiali
            Container pack = from.Backpack;
            if (pack != null)
            {
                var items = pack.FindItemsByType<Item>(true); // Ricorsivo

                foreach (Item item in items)
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

            var entries = new List<ItemListEntry>();

            if (hasOnlyIron)
            {
                entries.Add(new ItemListEntry("Repair", 4015));
                entries.Add(new ItemListEntry("Smelt", 4017));
                entries.Add(new ItemListEntry("Shields", 7026));
                entries.Add(new ItemListEntry("Armor", 5141));
                entries.Add(new ItemListEntry("Weapons", 5049));
            }
            else if (hasIron && hasOtherMaterials)
            {
                entries.Add(new ItemListEntry("Repair", 4015));
                entries.Add(new ItemListEntry("Smelt", 4017));
                entries.Add(new ItemListEntry("Shields", 7026));
                entries.Add(new ItemListEntry("Armor", 5141));
                entries.Add(new ItemListEntry("Weapons", 5049));
                if (HasSkillForAnySpecialArmor(from, craftSystem))
                    entries.Add(new ItemListEntry("Special Armor", 5384));
                if (HasSkillForAnySpecialWeapon(from, craftSystem))
                    entries.Add(new ItemListEntry("Special Weapon", 5183));
            }
            else if (hasOtherMaterials)
            {
                entries.Add(new ItemListEntry("Repair", 4015));
                entries.Add(new ItemListEntry("Smelt", 4017));
                if (HasSkillForAnySpecialArmor(from, craftSystem))
                    entries.Add(new ItemListEntry("Special Armor", 5384));
                if (HasSkillForAnySpecialWeapon(from, craftSystem))
                    entries.Add(new ItemListEntry("Special Weapon", 5183));
            }

            // Default case if nessuna delle condizioni sopra è soddisfatta
            return entries.ToArray();
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




        private static bool HasSkillForAnySpecialArmor(Mobile from, CraftSystem craftSystem)
        {
            Type[] specialIngots = new Type[]
            {
        typeof(DullCopperIngot),
        typeof(ShadowIronIngot),
        typeof(CopperIngot),
        typeof(BronzeIngot),
        typeof(GoldIngot),
        typeof(AgapiteIngot),
        typeof(VeriteIngot),
        typeof(ValoriteIngot)
            };

            CraftSubResCol subRes = craftSystem.CraftSubRes;
            double playerSkill = from.Skills[SkillName.Blacksmith].Value;

            foreach (Type ingotType in specialIngots)
            {
                if (from.Backpack.FindItemByType(ingotType) != null)
                {
                    foreach (CraftSubRes res in subRes)
                    {
                        if (res.ItemType == ingotType)
                        {
                            if (playerSkill >= res.RequiredSkill)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool HasSkillForAnySpecialWeapon(Mobile from, CraftSystem craftSystem)
        {
            Type[] specialIngots = new Type[]
            {
        typeof(DullCopperIngot),
        typeof(ShadowIronIngot),
        typeof(CopperIngot),
        typeof(BronzeIngot),
        typeof(GoldIngot),
        typeof(AgapiteIngot),
        typeof(VeriteIngot),
        typeof(ValoriteIngot)
            };

            CraftSubResCol subRes = craftSystem.CraftSubRes;
            double playerSkill = from.Skills[SkillName.Blacksmith].Value;

            foreach (Type ingotType in specialIngots)
            {
                if (from.Backpack.FindItemByType(ingotType) != null)
                {
                    foreach (CraftSubRes res in subRes)
                    {
                        if (res.ItemType == ingotType)
                        {
                            if (playerSkill >= res.RequiredSkill)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        //OLD CODE
        /*
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
        */

        public override void OnResponse(NetState state, int index)
        {
            var categories = GetCraftCategories(m_From, m_CraftSystem);
            if (index < 0 || index >= categories.Length)
            {
                m_From.SendMessage("Categoria non valida.");
                return;
            }

            string category = categories[index].Name;

            switch (category)
            {
                case "Repair":
                    Repair.Do(m_From, m_CraftSystem, m_Tool, isPreAoS);
                    break;
                case "Smelt":
                    Resmelt.Do(m_From, m_CraftSystem, m_Tool, isPreAoS);
                    break;
                case "Shields":
                    m_From.SendMenu(new ShieldsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case "Armor":
                    m_From.SendMenu(new ArmorMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case "Weapons":
                    m_From.SendMenu(new WeaponsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case "Special Armor":
                    m_From.SendMenu(new SpecialArmorMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                case "Special Weapon":
                    m_From.SendMenu(new SpecialWeaponsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                    break;
                default:
                    m_From.SendMessage("Categoria non supportata.");
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
                                if (from.Backpack.GetAmount(craftRes.ItemType, true) < craftRes.Amount)
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
                var itemType = ((ItemListEntryWithType)GetShields(m_From, m_CraftSystem)[index]).ItemType;
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

                // PLATEMAIL MENU        
                public PlatemailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a platemail item to craft:", GetPlatemailItems(from, craftSystem))
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetPlatemailItems(Mobile from, CraftSystem craftSystem)
                {
                    List<ItemListEntry> entries = new List<ItemListEntry>();

                    ItemListEntryWithType[] allPlatemail = new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Plate Arms", typeof(PlateArms), 5143),
                new ItemListEntryWithType("Plate Gloves", typeof(PlateGloves), 5144),
                new ItemListEntryWithType("Plate Gorget", typeof(PlateGorget), 5139),
                new ItemListEntryWithType("Plate Legs", typeof(PlateLegs), 5146),
                new ItemListEntryWithType("Plate Chest", typeof(PlateChest), 5142),
                new ItemListEntryWithType("Female Plate Chest", typeof(FemalePlateChest), 7172)
                    };

                    foreach (ItemListEntryWithType entry in allPlatemail)
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
                    var itemType = ((ItemListEntryWithType)GetPlatemailItems(m_From, m_CraftSystem)[index]).ItemType;
                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        craftItem.Craft(m_From, m_CraftSystem, null, m_Tool); // Updated to use 4 parameters
                    }
                    else
                    {
                        m_From.SendMessage("The selected item cannot be crafted.");
                    }
                }
            }
            // CHAINMAIL MENU


            private class ChainmailMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public ChainmailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a chainmail item to craft:", GetChainmailItems(from, craftSystem))
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetChainmailItems(Mobile from, CraftSystem craftSystem)
                {
                    List<ItemListEntry> entries = new List<ItemListEntry>();

                    ItemListEntryWithType[] allChainmail = new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Chain Coif", typeof(ChainCoif), 5051),
                new ItemListEntryWithType("Chain Legs", typeof(ChainLegs), 5054),
                new ItemListEntryWithType("Chain Chest", typeof(ChainChest), 5055)
                    };

                    foreach (ItemListEntryWithType entry in allChainmail)
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
                    var itemType = ((ItemListEntryWithType)GetChainmailItems(m_From, m_CraftSystem)[index]).ItemType;
                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        craftItem.Craft(m_From, m_CraftSystem, null, m_Tool); // Updated to use 4 parameters
                    }
                    else
                    {
                        m_From.SendMessage("The selected item cannot be crafted.");
                    }
                }
            }

            // RINGMAIL MENU

            private class RingmailMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public RingmailMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a ringmail item to craft:", GetRingmailItems(from, craftSystem))
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetRingmailItems(Mobile from, CraftSystem craftSystem)
                {
                    List<ItemListEntry> entries = new List<ItemListEntry>();

                    ItemListEntryWithType[] allRingmail = new ItemListEntryWithType[]
                    {
                        new ItemListEntryWithType("Ringmail Gloves", typeof(RingmailGloves), 5099),
                        new ItemListEntryWithType("Ringmail Legs", typeof(RingmailLegs), 5104),
                        new ItemListEntryWithType("Ringmail Arms", typeof(RingmailArms), 5103),
                        new ItemListEntryWithType("Ringmail Chest", typeof(RingmailChest), 5100)
                    };

                    foreach (ItemListEntryWithType entry in allRingmail)
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
                    var itemType = ((ItemListEntryWithType)GetRingmailItems(m_From, m_CraftSystem)[index]).ItemType;
                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        craftItem.Craft(m_From, m_CraftSystem, null, m_Tool); // Updated to use 4 parameters
                    }
                    else
                    {
                        m_From.SendMessage("The selected item cannot be crafted.");
                    }
                }
            }

            // HELMETS MENU

            private class HelmetsMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public HelmetsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a helmet to craft:", GetHelmetsItems(from, craftSystem))
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntry[] GetHelmetsItems(Mobile from, CraftSystem craftSystem)
                {
                    List<ItemListEntry> entries = new List<ItemListEntry>();

                    ItemListEntryWithType[] allHelmets = new ItemListEntryWithType[]
                    {
                        new ItemListEntryWithType("Bascinet", typeof(Bascinet), 5132),
                        new ItemListEntryWithType("CloseHelm", typeof(CloseHelm), 5128),
                        new ItemListEntryWithType("Helmet", typeof(Helmet), 5130),
                        new ItemListEntryWithType("NorseHelm", typeof(NorseHelm), 5134),
                        new ItemListEntryWithType("PlateHelm", typeof(PlateHelm), 5138)
                    };


                    foreach (ItemListEntryWithType entry in allHelmets)
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
                    var itemType = ((ItemListEntryWithType)GetHelmetsItems(m_From, m_CraftSystem)[index]).ItemType;
                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        craftItem.Craft(m_From, m_CraftSystem, null, m_Tool); // Updated to use 4 parameters
                    }
                    else
                    {
                        m_From.SendMessage("The selected item cannot be crafted.");
                    }
                }
            }
        }

        // inizio sezione armi
        /// <summary>
        /// ////////////////
        /// </summary>
        /// 
        // Classe per il menu delle armi

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
                        m_From.SendMenu(new SwordsAndBladesMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 1: // Axes
                        m_From.SendMenu(new AxesMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 2: // Maces and Hammers
                        m_From.SendMenu(new MacesAndHammersMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 3: // Spears and Forks
                        m_From.SendMenu(new SpearsAndForksMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                    case 4: // Pole Arms
                        m_From.SendMenu(new PolearmsMenu(m_From, m_CraftSystem, m_Tool, isPreAoS));
                        break;
                }
            }

            private class SwordsAndBladesMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public SwordsAndBladesMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a sword or blade to craft:", GetSwordsAndBladesItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntryWithType[] GetSwordsAndBladesItems()
                {
                    return new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Broadsword", typeof(Broadsword), 3934),
                new ItemListEntryWithType("Cutlass", typeof(Cutlass), 5185),
                new ItemListEntryWithType("Dagger", typeof(Dagger), 3921),
                new ItemListEntryWithType("Katana", typeof(Katana), 5119),
                new ItemListEntryWithType("Kryss", typeof(Kryss), 5121),
                new ItemListEntryWithType("Longsword", typeof(Longsword), 3937),
                new ItemListEntryWithType("Scimitar", typeof(Scimitar), 5046),
                new ItemListEntryWithType("Viking Sword", typeof(VikingSword), 5049)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    var items = GetSwordsAndBladesItems();
                    if (index >= 0 && index < items.Length)
                    {
                        var itemType = items[index].ItemType;
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

            private class AxesMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public AxesMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select an axe to craft:", GetAxesItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntryWithType[] GetAxesItems()
                {
                    return new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Axe", typeof(Axe), 3913),
                new ItemListEntryWithType("Battle Axe", typeof(BattleAxe), 3911),
                new ItemListEntryWithType("Double Axe", typeof(DoubleAxe), 3915),
                new ItemListEntryWithType("Executioner's Axe", typeof(ExecutionersAxe), 3909),
                new ItemListEntryWithType("Large Battle Axe", typeof(LargeBattleAxe), 5115),
                new ItemListEntryWithType("Two-Handed Axe", typeof(TwoHandedAxe), 5187),
                new ItemListEntryWithType("War Axe", typeof(WarAxe), 5040)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    var items = GetAxesItems();
                    if (index >= 0 && index < items.Length)
                    {
                        var itemType = items[index].ItemType;
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

            private class MacesAndHammersMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public MacesAndHammersMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a mace or hammer to craft:", GetMacesAndHammersItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntryWithType[] GetMacesAndHammersItems()
                {
                    return new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Hammer Pick", typeof(HammerPick), 5181),
                new ItemListEntryWithType("Mace", typeof(Mace), 3932),
                new ItemListEntryWithType("Maul", typeof(Maul), 5179),
                new ItemListEntryWithType("Smith Hammer", typeof(SmithHammer), 5092),
                new ItemListEntryWithType("War Hammer", typeof(WarHammer), 5177),
                new ItemListEntryWithType("War Mace", typeof(WarMace), 5127)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    var items = GetMacesAndHammersItems();
                    if (index >= 0 && index < items.Length)
                    {
                        var itemType = items[index].ItemType;
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

            private class SpearsAndForksMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public SpearsAndForksMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a spear or fork to craft:", GetSpearsAndForksItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntryWithType[] GetSpearsAndForksItems()
                {
                    return new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Short Spear", typeof(ShortSpear), 5123),
                new ItemListEntryWithType("Spear", typeof(Spear), 3938),
                new ItemListEntryWithType("War Fork", typeof(WarFork), 5125)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    var items = GetSpearsAndForksItems();
                    if (index >= 0 && index < items.Length)
                    {
                        var itemType = items[index].ItemType;
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

            private class PolearmsMenu : ItemListMenu
            {
                private readonly Mobile m_From;
                private readonly CraftSystem m_CraftSystem;
                private readonly BaseTool m_Tool;
                private readonly bool isPreAoS;

                public PolearmsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                    : base("Select a polearm to craft:", GetPolearmsItems())
                {
                    m_From = from;
                    m_CraftSystem = craftSystem;
                    m_Tool = tool;
                    this.isPreAoS = isPreAoS;
                }

                private static ItemListEntryWithType[] GetPolearmsItems()
                {
                    return new ItemListEntryWithType[]
                    {
                new ItemListEntryWithType("Bardiche", typeof(Bardiche), 3917),
                new ItemListEntryWithType("Halberd", typeof(Halberd), 5182)
                    };
                }

                public override void OnResponse(NetState state, int index)
                {
                    var items = GetPolearmsItems();
                    if (index >= 0 && index < items.Length)
                    {
                        var itemType = items[index].ItemType;
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




        /// <summary>
        /// /INIZIO CODICE SPECIAL ARMOR
        /// </summary>
        /// <returns></returns>

        // sezione special armor



        public class SpecialArmorMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS;

            public SpecialArmorMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a special armor material:", GetSpecialArmors(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntryWithType[] GetSpecialArmors(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntryWithType> entries = new List<ItemListEntryWithType>();
                Container pack = from.Backpack;

                // test
                double playerSkill = from.Skills[SkillName.Blacksmith].Value;

                ItemListEntryWithType[] allSpecialArmor = new ItemListEntryWithType[]
                {
            new ItemListEntryWithType("Dull Copper Armor", typeof(DullCopperIngot), 5384),
            new ItemListEntryWithType("Shadow Iron Armor", typeof(ShadowIronIngot), 5384),
            new ItemListEntryWithType("Copper Armor", typeof(CopperIngot), 5384),
            new ItemListEntryWithType("Bronze Armor", typeof(BronzeIngot), 5384),
            new ItemListEntryWithType("Gold Armor", typeof(GoldIngot), 5384),
            new ItemListEntryWithType("Agapite Armor", typeof(AgapiteIngot), 5384),
            new ItemListEntryWithType("Verite Armor", typeof(VeriteIngot), 5384),
            new ItemListEntryWithType("Valorite Armor", typeof(ValoriteIngot), 5384)
                };

                CraftSubResCol subRes = craftSystem.CraftSubRes;

                foreach (ItemListEntryWithType entry in allSpecialArmor)
                {
                    if (pack.FindItemByType(entry.ItemType) != null)
                    {
                        //entries.Add(entry);
                        foreach (CraftSubRes res in subRes)
                        {
                            if (res.ItemType == entry.ItemType && playerSkill >= res.RequiredSkill)
                            {
                                entries.Add(entry);
                                break;
                            }
                        }
                    }
                }

                return entries.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                var items = GetSpecialArmors(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    string selectedMaterial = items[index].Name;
                    m_From.SendMenu(new ArmorItemsMenu(m_From, m_CraftSystem, m_Tool, selectedMaterial, isPreAoS));
                }
                else
                {
                    m_From.SendMessage("Invalid selection.");
                }
            }
        }





        /// <summary>
        /// /SPECIAL WEAPON MENU
        /// </summary>



        public class SpecialWeaponsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly bool isPreAoS;

            public SpecialWeaponsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, bool isPreAoS)
                : base("Select a special weapon material:", GetSpecialWeaponMaterials(from, craftSystem))
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                this.isPreAoS = isPreAoS;
                Console.WriteLine("SpecialWeaponsMenu instantiated with material.");
            }

            private static ItemListEntryWithType[] GetSpecialWeaponMaterials(Mobile from, CraftSystem craftSystem)
            {
                List<ItemListEntryWithType> entries = new List<ItemListEntryWithType>();
                Container pack = from.Backpack;

                ItemListEntryWithType[] allSpecialWeapons = new ItemListEntryWithType[]
                {
            new ItemListEntryWithType("Dull Copper Weapon", typeof(DullCopperIngot), 5183),
            new ItemListEntryWithType("Shadow Iron Weapon", typeof(ShadowIronIngot), 5183),
            new ItemListEntryWithType("Copper Weapon", typeof(CopperIngot), 5183),
            new ItemListEntryWithType("Bronze Weapon", typeof(BronzeIngot), 5183),
            new ItemListEntryWithType("Gold Weapon", typeof(GoldIngot), 5183),
            new ItemListEntryWithType("Agapite Weapon", typeof(AgapiteIngot), 5183),
            new ItemListEntryWithType("Verite Weapon", typeof(VeriteIngot), 5183),
            new ItemListEntryWithType("Valorite Weapon", typeof(ValoriteIngot), 5183)
                };

                foreach (ItemListEntryWithType entry in allSpecialWeapons)
                {
                    if (pack.FindItemByType(entry.ItemType) != null)
                    {
                        entries.Add(entry);
                    }
                }

                return entries.ToArray();
            }

            public override void OnResponse(NetState state, int index)
            {
                Console.WriteLine("SpecialWeaponsMenu.OnResponse called with index: " + index);
                var items = GetSpecialWeaponMaterials(m_From, m_CraftSystem);
                if (index >= 0 && index < items.Length)
                {
                    string selectedMaterial = items[index].Name;
                    Console.WriteLine("Selected Material: " + selectedMaterial);
                    m_From.SendMenu(new WeaponItemsMenu(m_From, m_CraftSystem, m_Tool, selectedMaterial, isPreAoS));
                    Console.WriteLine("Sent WeaponItemsMenu with selected material: " + selectedMaterial);
                }
                else
                {
                    m_From.SendMessage("Invalid selection.");
                }
            }
        }




        ///////// 
        ///




        public class ArmorItemsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly string m_Material;
            private readonly bool isPreAoS;


            public ArmorItemsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, string material, bool isPreAoS)
                                : base("Select " + material + " item to craft:", GetArmorItems())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                m_Material = material;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntryWithType[] GetArmorItems()
            {
                return new ItemListEntryWithType[]
                {
                    new ItemListEntryWithType("Plate Chest", typeof(PlateChest), 5142),
                    new ItemListEntryWithType("Plate Legs", typeof(PlateLegs), 5146),
                    new ItemListEntryWithType("Helmet", typeof(Helmet), 5130),
                    new ItemListEntryWithType("Plate Arms", typeof(PlateArms), 5143),
                    new ItemListEntryWithType("Plate Gloves", typeof(PlateGloves), 5144),
                    new ItemListEntryWithType("Plate Gorget", typeof(PlateGorget), 5139),
                    new ItemListEntryWithType("Female Plate Chest", typeof(FemalePlateChest), 7172),
                    new ItemListEntryWithType("Heater Shield", typeof(HeaterShield), 7030),
                    //new ItemListEntryWithType("Shield", typeof(MetalKiteShield), 7028),
                    //new ItemListEntryWithType("Bronze Shield", typeof(BronzeShield), 7026)
                };
            }

            private static Type GetMaterialType(string material)
            {
                switch (material)
                {
                    case "Dull Copper Armor":
                        return typeof(DullCopperIngot);
                    case "Shadow Iron Armor":
                        return typeof(ShadowIronIngot);
                    case "Copper Armor":
                        return typeof(CopperIngot);
                    case "Bronze Armor":
                        return typeof(BronzeIngot);
                    case "Gold Armor":
                        return typeof(GoldIngot);
                    case "Agapite Armor":
                        return typeof(AgapiteIngot);
                    case "Verite Armor":
                        return typeof(VeriteIngot);
                    case "Valorite Armor":
                        return typeof(ValoriteIngot);
                    default:
                        return null;
                }


            }


            public override void OnResponse(NetState state, int index)
            {
                var items = GetArmorItems();
                if (index >= 0 && index < items.Length)
                {
                    var itemType = items[index].ItemType;
                    var materialType = GetMaterialType(m_Material);
                    if (materialType == null)
                    {
                        m_From.SendMessage("Invalid material type.");
                        return;
                    }

                    // Aggiorna il context con il materiale selezionato
                    SetSelectedResource(m_From, m_CraftSystem, materialType);

                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        // Check materiali, tenendo conto del materiale selezionato
                        bool hasMaterials = true;
                        /*
                        foreach (CraftRes res in craftItem.Resources)
                        {
                            Type requiredType = res.ItemType;
                            if (requiredType == typeof(IronIngot))
                                requiredType = materialType;

                            if (m_From.Backpack.GetAmount(requiredType) < res.Amount)
                            {
                                hasMaterials = false;
                                break;
                            }
                        }
                        */

                        if (hasMaterials)
                        {
                            // Chiama SOLO craft, lascia che il core consumi i materiali!
                            craftItem.Craft(m_From, m_CraftSystem, materialType, m_Tool);
                        }
                        else
                        {
                            m_From.SendMessage("You do not have the necessary materials.");
                        }
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








        /////////   WEAPON MENU
        ///


        public class WeaponItemsMenu : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly string m_Material;
            private readonly bool isPreAoS;

            public WeaponItemsMenu(Mobile from, CraftSystem craftSystem, BaseTool tool, string material, bool isPreAoS)
                : base("Select " + material + " item to craft:", GetWeaponItems())
            {
                m_From = from;
                m_CraftSystem = craftSystem;
                m_Tool = tool;
                m_Material = material;
                this.isPreAoS = isPreAoS;
            }

            private static ItemListEntryWithType[] GetWeaponItems()
            {
                return new ItemListEntryWithType[]
                {
                        new ItemListEntryWithType("Broadsword", typeof(Broadsword), 3934),
                        new ItemListEntryWithType("Cutlass", typeof(Cutlass), 5185),
                        new ItemListEntryWithType("Dagger", typeof(Dagger), 3921),
                        new ItemListEntryWithType("Katana", typeof(Katana), 5119),
                        new ItemListEntryWithType("Kryss", typeof(Kryss), 5121),
                        new ItemListEntryWithType("Longsword", typeof(Longsword), 3937),
                        new ItemListEntryWithType("Scimitar", typeof(Scimitar), 5046),
                        new ItemListEntryWithType("Viking Sword", typeof(VikingSword), 5049)
                };
            }

            private static Type GetMaterialType(string material)
            {
                switch (material)
                {
                    case "Dull Copper Weapon":
                        return typeof(DullCopperIngot);
                    case "Shadow Iron Weapon":
                        return typeof(ShadowIronIngot);
                    case "Copper Weapon":
                        return typeof(CopperIngot);
                    case "Bronze Weapon":
                        return typeof(BronzeIngot);
                    case "Gold Weapon":
                        return typeof(GoldIngot);
                    case "Agapite Weapon":
                        return typeof(AgapiteIngot);
                    case "Verite Weapon":
                        return typeof(VeriteIngot);
                    case "Valorite Weapon":
                        return typeof(ValoriteIngot);
                    default:
                        return null;
                }
            }


            public override void OnResponse(NetState state, int index)
            {
                var items = GetWeaponItems();
                if (index >= 0 && index < items.Length)
                {
                    var itemType = items[index].ItemType;
                    var materialType = GetMaterialType(m_Material);

                    if (materialType == null)
                    {
                        m_From.SendMessage("Invalid material type.");
                        return;
                    }

                    // (Consigliato) Aggiorna il context con il materiale selezionato, se hai questa funzione
                    SetSelectedResource(m_From, m_CraftSystem, materialType);

                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem != null)
                    {
                        // Controlla solo se hai i materiali, NON consumarli manualmente!
                        bool hasMaterials = true;
                        foreach (CraftRes res in craftItem.Resources)
                        {
                            Type requiredType = res.ItemType;
                            if (requiredType == typeof(IronIngot))
                                requiredType = materialType;

                            if (m_From.Backpack.GetAmount(requiredType) < res.Amount)
                            {
                                hasMaterials = false;
                                break;
                            }
                        }

                        if (hasMaterials)
                        {
                            // PATCH: Chiama SOLO craft, lascia che il core consumi i materiali!
                            craftItem.Craft(m_From, m_CraftSystem, materialType, m_Tool);
                        }
                        else
                        {
                            m_From.SendMessage("You do not have the necessary materials.");
                        }
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


        /////// DA QUI CI SONO LE PARENTESI FINALI
    }
}


