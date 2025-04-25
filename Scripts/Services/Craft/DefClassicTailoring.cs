using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Craft
{
  


    public class DefClassicTailoring : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Tailoring;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044005;
            }// <CENTER>TAILORING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClassicTailoring();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixtyToFourtyFive;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefClassicTailoring()
            : base(1, 1, 1.25)// base( 1, 1, 4.5 )
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
            from.PlaySound(0x248);
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


            #region Hats
            this.AddCraft(typeof(SkullCap), 1011375, 1025444, 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            this.AddCraft(typeof(Bandana), 1011375, 1025440, 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            this.AddCraft(typeof(FloppyHat), 1011375, 1025907, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(Cap), 1011375, 1025909, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(WideBrimHat), 1011375, 1025908, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(StrawHat), 1011375, 1025911, 6.2, 31.2, typeof(Cloth), 1044286, 10, 1044287);
            this.AddCraft(typeof(TallStrawHat), 1011375, 1025910, 6.7, 31.7, typeof(Cloth), 1044286, 13, 1044287);
            this.AddCraft(typeof(WizardsHat), 1011375, 1025912, 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);
            this.AddCraft(typeof(Bonnet), 1011375, 1025913, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(FeatheredHat), 1011375, 1025914, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(TricorneHat), 1011375, 1025915, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(JesterHat), 1011375, 1025916, 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);
                #endregion


            #region Shirts/Pants
            this.AddCraft(typeof(Doublet), 1111747, 1028059, 0, 25.0, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Shirt), 1111747, 1025399, 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(FancyShirt), 1111747, 1027933, 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Tunic), 1111747, 1028097, 00.0, 25.0, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(Surcoat), 1111747, 1028189, 8.2, 33.2, typeof(Cloth), 1044286, 14, 1044287);
            this.AddCraft(typeof(PlainDress), 1111747, 1027937, 12.4, 37.4, typeof(Cloth), 1044286, 10, 1044287);
            this.AddCraft(typeof(FancyDress), 1111747, 1027935, 33.1, 58.1, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(Cloak), 1111747, 1025397, 41.4, 66.4, typeof(Cloth), 1044286, 14, 1044287);
            this.AddCraft(typeof(Robe), 1111747, 1027939, 53.9, 78.9, typeof(Cloth), 1044286, 16, 1044287);
            this.AddCraft(typeof(JesterSuit), 1111747, 1028095, 8.2, 33.2, typeof(Cloth), 1044286, 24, 1044287);

            if (Core.AOS)
            {
                this.AddCraft(typeof(FurCape), 1111747, 1028969, 35.0, 60.0, typeof(Cloth), 1044286, 13, 1044287);
                this.AddCraft(typeof(GildedDress), 1111747, 1028973, 37.5, 62.5, typeof(Cloth), 1044286, 16, 1044287);
                this.AddCraft(typeof(FormalShirt), 1111747, 1028975, 26.0, 51.0, typeof(Cloth), 1044286, 16, 1044287);
            }




            this.AddCraft(typeof(ShortPants), 1111747, 1025422, 24.8, 49.8, typeof(Cloth), 1044286, 6, 1044287);
            this.AddCraft(typeof(LongPants), 1111747, 1025433, 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Kilt), 1111747, 1025431, 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Skirt), 1111747, 1025398, 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);
                #endregion



            #region Misc
            this.AddCraft(typeof(BodySash), 1015283, 1025441, 4.1, 29.1, typeof(Cloth), 1044286, 4, 1044287);
            this.AddCraft(typeof(HalfApron), 1015283, 1025435, 20.7, 45.7, typeof(Cloth), 1044286, 6, 1044287);
            this.AddCraft(typeof(FullApron), 1015283, 1025437, 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);
                #endregion


            if (Core.ML)
            {
                #region
                index = this.AddCraft(typeof(ElvenQuiver), 1015283, 1032657, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRecipe(index, (int)TailorRecipe.ElvenQuiver);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfFire), 1015283, 1073109, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(FireRuby), 1032695, 15, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfFire);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfIce), 1015283, 1073110, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(WhitePearl), 1032694, 15, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfIce);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfBlight), 1015283, 1073111, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(Blight), 1032675, 10, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfBlight);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfLightning), 1015283, 1073112, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(Corruption), 1032676, 10, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfLightning);
                this.SetNeededExpansion(index, Expansion.ML);
                #endregion
                
                #region Mondain's Legacy
                index = this.AddCraft(typeof(LeatherContainerEngraver), 1015283, 1072152, 75.0, 100.0, typeof(Bone), 1049064, 1, 1049063);
                this.AddRes(index, typeof(Leather), 1044462, 6, 1044463);
                this.AddRes(index, typeof(SpoolOfThread), 1073462, 2, 1073463);
                this.AddRes(index, typeof(Dyes), 1024009, 6, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
                #endregion
            }

            this.AddCraft(typeof(OilCloth), 1015283, 1041498, 74.6, 99.6, typeof(Cloth), 1044286, 1, 1044287);



            #region Footwear

            this.AddCraft(typeof(Sandals), 1015288, 1025901, 12.4, 37.4, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(Shoes), 1015288, 1025904, 16.5, 41.5, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(Boots), 1015288, 1025899, 33.1, 58.1, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(ThighBoots), 1015288, 1025906, 41.4, 66.4, typeof(Leather), 1044462, 10, 1044463);
            #endregion

            #region Leather Armor

            #region Mondain's Legacy
            this.AddCraft(typeof(LeatherGorget), 1015293, 1025063, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(LeatherCap), 1015293, 1027609, 6.2, 31.2, typeof(Leather), 1044462, 2, 1044463);
            this.AddCraft(typeof(LeatherGloves), 1015293, 1025062, 51.8, 76.8, typeof(Leather), 1044462, 3, 1044463);
            this.AddCraft(typeof(LeatherArms), 1015293, 1025061, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(LeatherLegs), 1015293, 1025067, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
            this.AddCraft(typeof(LeatherChest), 1015293, 1025068, 70.5, 95.5, typeof(Leather), 1044462, 12, 1044463);
            #endregion
            #endregion
            // LEATHER BAG MISC
            this.AddCraft(typeof(Pouch), 1015293, 1025061, 20, 50, typeof(Leather), 1044462, 2, 1044463);
            this.AddCraft(typeof(Bag), 1015293, 1025067, 50, 70, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(Backpack), 1015293, 1025068, 65, 80, typeof(Leather), 1044462, 10, 1044463);

            #region Studded Armor
            this.AddCraft(typeof(StuddedGorget), 1015300, 1025078, 78.8, 103.8, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(StuddedGloves), 1015300, 1025077, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(StuddedArms), 1015300, 1025076, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            this.AddCraft(typeof(StuddedLegs), 1015300, 1025082, 91.2, 116.2, typeof(Leather), 1044462, 12, 1044463);
            this.AddCraft(typeof(StuddedChest), 1015300, 1025083, 94.0, 119.0, typeof(Leather), 1044462, 14, 1044463);
            #endregion

            #region Female Armor
            this.AddCraft(typeof(LeatherShorts), 1015306, 1027168, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(LeatherSkirt), 1015306, 1027176, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(LeatherBustierArms), 1015306, 1027178, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(StuddedBustierArms), 1015306, 1027180, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(FemaleLeatherChest), 1015306, 1027174, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(FemaleStuddedChest), 1015306, 1027170, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            #endregion

            #region Bone Armor
            index = this.AddCraft(typeof(BoneHelm), 1049149, 1025206, 85.0, 110.0, typeof(Leather), 1044462, 4, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 2, 1049063);
			
            index = this.AddCraft(typeof(BoneGloves), 1049149, 1025205, 89.0, 114.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = this.AddCraft(typeof(BoneArms), 1049149, 1025203, 92.0, 117.0, typeof(Leather), 1044462, 8, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = this.AddCraft(typeof(BoneLegs), 1049149, 1025202, 95.0, 120.0, typeof(Leather), 1044462, 10, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 6, 1049063);
		
            index = this.AddCraft(typeof(BoneChest), 1049149, 1025199, 96.0, 121.0, typeof(Leather), 1044462, 12, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 10, 1049063);
            #endregion


            // Set the overridable material
            this.SetSubRes(typeof(Leather), 1049150);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            this.AddSubRes(typeof(Leather), 1049150, 00.0, 1044462, 1049311);
            this.AddSubRes(typeof(SpinedLeather), 1049151, 65.0, 1044462, 1049311);
            this.AddSubRes(typeof(HornedLeather), 1049152, 80.0, 1044462, 1049311);
            this.AddSubRes(typeof(BarbedLeather), 1049153, 99.0, 1044462, 1049311);

            this.MarkOption = true;
            this.Repair = Core.AOS;
            this.CanEnhance = Core.ML;
			this.CanAlter = Core.SA;

        }
    }
}