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

            // Configurazione esistente
            #region Metal Armor
            #region Ringmail
            AddCraft(typeof(RingmailGloves), 1111704, 1025099, 12.0, 62.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(RingmailLegs), 1111704, 1025104, 19.4, 69.4, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(typeof(RingmailArms), 1111704, 1025103, 16.9, 66.9, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(RingmailChest), 1111704, 1025100, 21.9, 71.9, typeof(IronIngot), 1044036, 18, 1044037);
            #endregion

            #region Chainmail
            AddCraft(typeof(ChainCoif), 1111704, 1025051, 14.5, 64.5, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(ChainLegs), 1111704, 1025054, 36.7, 86.7, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(typeof(ChainChest), 1111704, 1025055, 39.1, 89.1, typeof(IronIngot), 1044036, 20, 1044037);
            #endregion

            #region Platemail
            AddCraft(typeof(PlateArms), 1111704, 1025136, 66.3, 116.3, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(typeof(PlateGloves), 1111704, 1025140, 58.9, 108.9, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(PlateGorget), 1111704, 1025139, 56.4, 106.4, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(PlateLegs), 1111704, 1025137, 68.8, 118.8, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(PlateChest), 1111704, 1046431, 75.0, 125.0, typeof(IronIngot), 1044036, 25, 1044037);
            AddCraft(typeof(FemalePlateChest), 1111704, 1046430, 44.1, 94.1, typeof(IronIngot), 1044036, 20, 1044037);
            #endregion
            #endregion

            #region Helmets
            AddCraft(typeof(Bascinet), 1011079, 1025132, 8.3, 58.3, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(CloseHelm), 1011079, 1025128, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Helmet), 1011079, 1025130, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(NorseHelm), 1011079, 1025134, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(PlateHelm), 1011079, 1025138, 62.6, 112.6, typeof(IronIngot), 1044036, 15, 1044037);
            #endregion

            // Continua a implementare la logica di crafting per altre categorie come Armi e Strumenti
        }
    }
}