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
            //int index = -1;
             int index;

            // Example of Carpentry items excluding ML, SA, SE expansions.

            this.AddCraft(typeof(Board), 1044294, 1027127, 0.0, 0.0, typeof(Log), 1044466, 1, 1044465);
            this.AddCraft(typeof(BarrelStaves), 1044294, 1027857, 00.0, 25.0, typeof(Board), 1044041, 5, 1044351);
            this.AddCraft(typeof(BarrelLid), 1044294, 1027608, 11.0, 36.0, typeof(Board), 1044041, 4, 1044351);
            this.AddCraft(typeof(ShortMusicStand), 1044294, 1044313, 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            this.AddCraft(typeof(TallMusicStand), 1044294, 1044315, 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            this.AddCraft(typeof(Easle), 1044294, 1044317, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);


            // CHAIR 
            this.AddCraft(typeof(FootStool), 1044291, 1022910, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            this.AddCraft(typeof(Stool), 1044291, 1022602, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            this.AddCraft(typeof(BambooChair), 1044291, 1044300, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            this.AddCraft(typeof(WoodenChair), 1044291, 1044301, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            this.AddCraft(typeof(FancyWoodenChairCushion), 1044291, 1044302, 42.1, 67.1, typeof(Board), 1044041, 15, 1044351);
            this.AddCraft(typeof(WoodenChairCushion), 1044291, 1044303, 42.1, 67.1, typeof(Board), 1044041, 13, 1044351);
            this.AddCraft(typeof(WoodenBench), 1044291, 1022860, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            this.AddCraft(typeof(WoodenThrone), 1044291, 1044304, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            this.AddCraft(typeof(Throne), 1044291, 1044305, 73.6, 98.6, typeof(Board), 1044041, 19, 1044351);

            // Containers
            this.AddCraft(typeof(WoodenBox), 1044292, 1023709, 21.0, 46.0, typeof(Board), 1044041, 10, 1044351);
            this.AddCraft(typeof(SmallCrate), 1044292, 1044309, 10.0, 35.0, typeof(Board), 1044041, 8, 1044351);
            this.AddCraft(typeof(MediumCrate), 1044292, 1044310, 31.0, 56.0, typeof(Board), 1044041, 15, 1044351);
            this.AddCraft(typeof(LargeCrate), 1044292, 1044311, 47.3, 72.3, typeof(Board), 1044041, 18, 1044351);
            this.AddCraft(typeof(WoodenChest), 1044292, 1023650, 73.6, 98.6, typeof(Board), 1044041, 20, 1044351);
            this.AddCraft(typeof(EmptyBookcase), 1044292, 1022718, 31.5, 56.5, typeof(Board), 1044041, 25, 1044351);
            this.AddCraft(typeof(FancyArmoire), 1044292, 1044312, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);
            this.AddCraft(typeof(Armoire), 1044292, 1022643, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);

            // TABLE
            this.AddCraft(typeof(WritingTable), 1044291, 1022890, 63.1, 88.1, typeof(Board), 1044041, 17, 1044351);
            this.AddCraft(typeof(YewWoodTable), 1044291, 1044307, 63.1, 88.1, typeof(Board), 1044041, 23, 1044351);
            this.AddCraft(typeof(LargeTable), 1044291, 1044308, 84.2, 109.2, typeof(Board), 1044041, 27, 1044351);


            //STAFFS
            this.AddCraft(typeof(ShepherdsCrook), 1044566, 1023713, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            this.AddCraft(typeof(QuarterStaff), 1044566, 1023721, 73.6, 98.6, typeof(Board), 1044041, 6, 1044351);
            this.AddCraft(typeof(GnarledStaff), 1044566, 1025112, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            this.AddCraft(typeof(Club), 1044566, 1025043, 65.0, 115.0, typeof(Board), 1044041, 9, 1044351);
            this.AddCraft(typeof(BlackStaff), 1044566, 1023568, 81.5, 141.8, typeof(Board), 1044041, 9, 1044351);


            //shield
            this.AddCraft(typeof(WoodenShield), 1062760, 1027034, 52.6, 77.6, typeof(Board), 1044041, 9, 1044351);


            // Instruments
            index = this.AddCraft(typeof(LapHarp), 1044293, 1023762, 63.1, 88.1, typeof(Board), 1044041, 20, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = this.AddCraft(typeof(Harp), 1044293, 1023761, 78.9, 103.9, typeof(Board), 1044041, 35, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            index = this.AddCraft(typeof(Drums), 1044293, 1023740, 57.8, 82.8, typeof(Board), 1044041, 20, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = this.AddCraft(typeof(Lute), 1044293, 1023763, 68.4, 93.4, typeof(Board), 1044041, 25, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = this.AddCraft(typeof(Tambourine), 1044293, 1023741, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = this.AddCraft(typeof(TambourineTassel), 1044293, 1044320, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            this.AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            this.AddRes(index, typeof(Cloth), 1044286, 15, 1044287);


            /// items with other sills
            /// Tailoring
            /// 
            index = this.AddCraft(typeof(SmallBedSouthDeed), 1044290, 1044321, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
            index = this.AddCraft(typeof(SmallBedEastDeed), 1044290, 1044322, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
            index = this.AddCraft(typeof(LargeBedSouthDeed), 1044290, 1044323, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            this.AddRes(index, typeof(Cloth), 1044286, 150, 1044287);
            index = this.AddCraft(typeof(LargeBedEastDeed), 1044290, 1044324, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            this.AddRes(index, typeof(Cloth), 1044286, 150, 1044287);
            this.AddCraft(typeof(DartBoardSouthDeed), 1044290, 1044325, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            this.AddCraft(typeof(DartBoardEastDeed), 1044290, 1044326, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            this.AddCraft(typeof(BallotBoxDeed), 1044290, 1044327, 47.3, 72.3, typeof(Board), 1044041, 5, 1044351);
            index = this.AddCraft(typeof(PentagramDeed), 1044290, 1044328, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            this.AddSkill(index, SkillName.Magery, 75.0, 80.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);
            index = this.AddCraft(typeof(AbbatoirDeed), 1044290, 1044329, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            this.AddSkill(index, SkillName.Magery, 50.0, 55.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);


            // Tailoring and Cooking
            index = this.AddCraft(typeof(Dressform), 1044298, 1044339, 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            this.AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = this.AddCraft(typeof(SpinningwheelEastDeed), 1044298, 1044341, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            this.AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = this.AddCraft(typeof(SpinningwheelSouthDeed), 1044298, 1044342, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            this.AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = this.AddCraft(typeof(LoomEastDeed), 1044298, 1044343, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            this.AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = this.AddCraft(typeof(LoomSouthDeed), 1044298, 1044344, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            this.AddRes(index, typeof(Cloth), 1044286, 25, 1044287);


            // FORGE
            index = this.AddCraft(typeof(SmallForgeDeed), 1111809, 1044330, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            this.AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 75, 1044037);

            index = this.AddCraft(typeof(LargeForgeEastDeed), 1111809, 1044331, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            this.AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = this.AddCraft(typeof(LargeForgeSouthDeed), 1111809, 1044332, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            this.AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = this.AddCraft(typeof(AnvilEastDeed), 1111809, 1044333, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            this.AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = this.AddCraft(typeof(AnvilSouthDeed), 1111809, 1044334, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            this.AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            this.AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            // Training
            index = this.AddCraft(typeof(TrainingDummyEastDeed), 1044297, 1044335, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            this.AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = this.AddCraft(typeof(TrainingDummySouthDeed), 1044297, 1044336, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            this.AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = this.AddCraft(typeof(PickpocketDipEastDeed), 1044297, 1044337, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            this.AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = this.AddCraft(typeof(PickpocketDipSouthDeed), 1044297, 1044338, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            this.AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            this.AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            // Add more carpentry items here excluding ML, SA, SE items as per the requirement.
        }
    }
}