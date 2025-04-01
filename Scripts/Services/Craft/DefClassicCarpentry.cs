using System;
using Server.Items;

namespace Server.Engines.Craft
{
    public class DefClassicCarpentry : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Carpentry; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClassicCarpentry();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefClassicCarpentry()
            : base(1, 1, 1.25) // base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x23D);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            // Example of Carpentry items excluding ML, SA, SE expansions.

            this.AddCraft(typeof(Board), 1044294, 1027127, 0.0, 0.0, typeof(Log), 1044466, 1, 1044465);
            this.AddCraft(typeof(BarrelStaves), 1044294, 1027857, 00.0, 25.0, typeof(Board), 1044041, 5, 1044351);
            this.AddCraft(typeof(BarrelLid), 1044294, 1027608, 11.0, 36.0, typeof(Board), 1044041, 4, 1044351);
            this.AddCraft(typeof(ShortMusicStand), 1044294, 1044313, 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            this.AddCraft(typeof(TallMusicStand), 1044294, 1044315, 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            this.AddCraft(typeof(Easle), 1044294, 1044317, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);

            // Add more carpentry items here excluding ML, SA, SE items as per the requirement.
        }
    }
}