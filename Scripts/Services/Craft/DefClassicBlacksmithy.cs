using System;
using Server.Items;

namespace Server.Engines.Craft
{
    public class DefClassicBlacksmithy : CraftSystem
    {
        public override SkillName MainSkill { get { return SkillName.Blacksmith; } }

        public override int GumpTitleNumber
        {
            get { return 1044002; } // <CENTER>BLACKSMITHY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem { get { return m_CraftSystem ?? (m_CraftSystem = new DefClassicBlacksmithy()); } }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefClassicBlacksmithy()
            : base(1, 1, 1.25)
        {
        }

        private static readonly Type typeofAnvil = typeof(AnvilAttribute);
        private static readonly Type typeofForge = typeof(ForgeAttribute);

        public static void CheckAnvilAndForge(Mobile from, int range, out bool anvil, out bool forge)
        {
            anvil = false;
            forge = false;

            Map map = from.Map;

            if (map == null)
            {
                return;
            }

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isAnvil = (type.IsDefined(typeofAnvil, false) || item.ItemID == 4015 || item.ItemID == 4016 ||
                                item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6);
                bool isForge = (type.IsDefined(typeofForge, false) || item.ItemID == 4017 ||
                                (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8);

                if (!isAnvil && !isForge)
                {
                    continue;
                }

                if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                {
                    continue;
                }

                anvil = anvil || isAnvil;
                forge = forge || isForge;

                if (anvil && forge)
                {
                    break;
                }
            }

            eable.Free();

            for (int x = -range; (!anvil || !forge) && x <= range; ++x)
            {
                for (int y = -range; (!anvil || !forge) && y <= range; ++y)
                {
                    var tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

                    for (int i = 0; (!anvil || !forge) && i < tiles.Length; ++i)
                    {
                        int id = tiles[i].ID;

                        bool isAnvil = (id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6);
                        bool isForge = (id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8);

                        if (!isAnvil && !isForge)
                        {
                            continue;
                        }

                        if ((from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z ||
                            !from.InLOS(new Point3D(from.X + x, from.Y + y, tiles[i].Z + (tiles[i].Height / 2) + 1)))
                        {
                            continue;
                        }

                        anvil = anvil || isAnvil;
                        forge = forge || isForge;
                    }
                }
            }
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
            {
                return 1044038; // You have worn out your tool!
            }

            if (!BaseTool.CheckTool(tool, from))
            {
                return 1048146; // If you have a tool equipped, you must use that tool.
            }

            if (!BaseTool.CheckAccessible(tool, from))
            {
                return 1044263; // The tool must be on your person to use.
            }

            bool anvil, forge;
            CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (anvil && forge)
            {
                return 0;
            }

            return 1044267; // You must be near an anvil and a forge to smith items.
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x2A);
        }

        public override int PlayEndingEffect(
            Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
            {
                from.SendLocalizedMessage(1044038); // You have worn out your tool
            }
                        // Inizio modifica per riaprire il menu pre-AoS in caso di fallimento
                        // Inizio modifica per riaprire il menu pre-AoS in caso di fallimento
                        if (this.GetType().Name == "DefClassicBlacksmithy")
                        {
                            from.SendMenu(new NewCraftingMenu(from, this, null, 0, true)); // Passa true per isPreAoS
                        }
                        // Fine modifica
                        
            if (failed)
            {
                if (lostMaterial)
                {
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                }

                return 1044157; // You failed to create the item, but no materials were lost.
            }

            if (quality == 0)
            {
                return 502785; // You were barely able to make this item.  It's quality is below average.
            }

            if (makersMark && quality == 2)
            {
                return 1044156; // You create an exceptional quality item and affix your maker's mark.
            }

            if (quality == 2)
            {
                return 1044155; // You create an exceptional quality item.
            }

            return 1044154; // You create the item.
        }

        public override void InitCraftList()
        {
            int index;

            // Configurazione delle ricette per vari scudi con IronIngot
            index = AddCraft(typeof(BronzeShield), "Shields", "Bronze Shield", 30.0, 80.0, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            index = AddCraft(typeof(Buckler), "Shields", "Buckler", 20.0, 70.0, typeof(IronIngot), "Iron Ingot", 8, "You do not have sufficient metal to make that.");
            index = AddCraft(typeof(HeaterShield), "Shields", "Heater Shield", 40.0, 90.0, typeof(IronIngot), "Iron Ingot", 12, "You do not have sufficient metal to make that.");
            index = AddCraft(typeof(MetalKiteShield), "Shields", "Metal Kite Shield", 35.0, 85.0, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            index = AddCraft(typeof(WoodenKiteShield), "Shields", "Wooden Kite Shield", 25.0, 75.0, typeof(IronIngot), "Iron Ingot", 6, "You do not have sufficient metal to make that.");
            index = AddCraft(typeof(MetalShield), "Shields", "Metal Shield", 45.0, 95.0, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");

            // Configurazione delle ricette per varie armi con IronIngot
            #region Bladed
            AddCraft(typeof(Broadsword), "Weapons", "Broadsword", 35.4, 85.4, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Cutlass), "Weapons", "Cutlass", 24.3, 74.3, typeof(IronIngot), "Iron Ingot", 8, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Dagger), "Weapons", "Dagger", -0.4, 49.6, typeof(IronIngot), "Iron Ingot", 3, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Katana), "Weapons", "Katana", 44.1, 94.1, typeof(IronIngot), "Iron Ingot", 8, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Kryss), "Weapons", "Kryss", 36.7, 86.7, typeof(IronIngot), "Iron Ingot", 8, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Longsword), "Weapons", "Longsword", 28.0, 78.0, typeof(IronIngot), "Iron Ingot", 12, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Scimitar), "Weapons", "Scimitar", 31.7, 81.7, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(VikingSword), "Weapons", "Viking Sword", 24.3, 74.3, typeof(IronIngot), "Iron Ingot", 14, "You do not have sufficient metal to make that.");
            #endregion

            #region Axes
            AddCraft(typeof(Axe), "Weapons", "Axe", 25.0, 75.0, typeof(IronIngot), "Iron Ingot", 14, "You do not have sufficient metal to make that.");
            AddCraft(typeof(BattleAxe), "Weapons", "Battle Axe", 30.0, 80.0, typeof(IronIngot), "Iron Ingot", 18, "You do not have sufficient metal to make that.");
            AddCraft(typeof(DoubleAxe), "Weapons", "Double Axe", 35.0, 85.0, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            AddCraft(typeof(ExecutionersAxe), "Weapons", "Executioner's Axe", 40.0, 90.0, typeof(IronIngot), "Iron Ingot", 22, "You do not have sufficient metal to make that.");
            AddCraft(typeof(LargeBattleAxe), "Weapons", "Large Battle Axe", 45.0, 95.0, typeof(IronIngot), "Iron Ingot", 24, "You do not have sufficient metal to make that.");
            AddCraft(typeof(TwoHandedAxe), "Weapons", "Two-Handed Axe", 50.0, 100.0, typeof(IronIngot), "Iron Ingot", 26, "You do not have sufficient metal to make that.");
            AddCraft(typeof(WarAxe), "Weapons", "War Axe", 55.0, 105.0, typeof(IronIngot), "Iron Ingot", 28, "You do not have sufficient metal to make that.");
            #endregion

            #region Maces and Hammers
            AddCraft(typeof(HammerPick), "Weapons", "Hammer Pick", 35.0, 85.0, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Mace), "Weapons", "Mace", 25.0, 75.0, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Maul), "Weapons", "Maul", 30.0, 80.0, typeof(IronIngot), "Iron Ingot", 18, "You do not have sufficient metal to make that.");
            AddCraft(typeof(SmithHammer), "Weapons", "Smith Hammer", 20.0, 70.0, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(WarHammer), "Weapons", "War Hammer", 40.0, 90.0, typeof(IronIngot), "Iron Ingot", 22, "You do not have sufficient metal to make that.");
            AddCraft(typeof(WarMace), "Weapons", "War Mace", 45.0, 95.0, typeof(IronIngot), "Iron Ingot", 24, "You do not have sufficient metal to make that.");
            #endregion

            #region Spears and Forks
            AddCraft(typeof(ShortSpear), "Weapons", "Short Spear", 35.0, 85.0, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Spear), "Weapons", "Spear", 40.0, 90.0, typeof(IronIngot), "Iron Ingot", 22, "You do not have sufficient metal to make that.");
            AddCraft(typeof(WarFork), "Weapons", "War Fork", 45.0, 95.0, typeof(IronIngot), "Iron Ingot", 24, "You do not have sufficient metal to make that.");
            #endregion

            #region Polearms
            AddCraft(typeof(Bardiche), "Weapons", "Bardiche", 50.0, 100.0, typeof(IronIngot), "Iron Ingot", 26, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Halberd), "Weapons", "Halberd", 55.0, 105.0, typeof(IronIngot), "Iron Ingot", 28, "You do not have sufficient metal to make that.");
            #endregion

            // Configurazione esistente
            // Configurazione esistente
            #region Metal Armor
            #region Ringmail
            AddCraft(typeof(RingmailGloves), "Armor", "Ringmail Gloves", 12.0, 62.0, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(RingmailLegs), "Armor", "Ringmail Legs", 19.4, 69.4, typeof(IronIngot), "Iron Ingot", 16, "You do not have sufficient metal to make that.");
            AddCraft(typeof(RingmailArms), "Armor", "Ringmail Arms", 16.9, 66.9, typeof(IronIngot), "Iron Ingot", 14, "You do not have sufficient metal to make that.");
            AddCraft(typeof(RingmailChest), "Armor", "Ringmail Chest", 21.9, 71.9, typeof(IronIngot), "Iron Ingot", 18, "You do not have sufficient metal to make that.");
            #endregion

            #region Chainmail
            AddCraft(typeof(ChainCoif), "Armor", "Chain Coif", 14.5, 64.5, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(ChainLegs), "Armor", "Chain Legs", 36.7, 86.7, typeof(IronIngot), "Iron Ingot", 18, "You do not have sufficient metal to make that.");
            AddCraft(typeof(ChainChest), "Armor", "Chain Chest", 39.1, 89.1, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            #endregion

            #region Platemail
            AddCraft(typeof(PlateArms), "Armor", "Plate Arms", 66.3, 116.3, typeof(IronIngot), "Iron Ingot", 18, "You do not have sufficient metal to make that.");
            AddCraft(typeof(PlateGloves), "Armor", "Plate Gloves", 58.9, 108.9, typeof(IronIngot), "Iron Ingot", 12, "You do not have sufficient metal to make that.");
            AddCraft(typeof(PlateGorget), "Armor", "Plate Gorget", 56.4, 106.4, typeof(IronIngot), "Iron Ingot", 10, "You do not have sufficient metal to make that.");
            AddCraft(typeof(PlateLegs), "Armor", "Plate Legs", 68.8, 118.8, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            AddCraft(typeof(PlateChest), "Armor", "Plate Chest", 75.0, 125.0, typeof(IronIngot), "Iron Ingot", 25, "You do not have sufficient metal to make that.");
            AddCraft(typeof(FemalePlateChest), "Armor", "Female Plate Chest", 44.1, 94.1, typeof(IronIngot), "Iron Ingot", 20, "You do not have sufficient metal to make that.");
            #endregion
            #endregion

            #region Helmets
            AddCraft(typeof(Bascinet), "Helmets", "Bascinet", 8.3, 58.3, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            AddCraft(typeof(CloseHelm), "Helmets", "Close Helm", 37.9, 87.9, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            AddCraft(typeof(Helmet), "Helmets", "Helmet", 37.9, 87.9, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            AddCraft(typeof(NorseHelm), "Helmets", "Norse Helm", 37.9, 87.9, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            AddCraft(typeof(PlateHelm), "Helmets", "Plate Helm", 62.6, 112.6, typeof(IronIngot), "Iron Ingot", 15, "You do not have sufficient metal to make that.");
            #endregion

            // Continua a implementare la logica di crafting per altre categorie come Armi e Strumenti
        }
    }
}