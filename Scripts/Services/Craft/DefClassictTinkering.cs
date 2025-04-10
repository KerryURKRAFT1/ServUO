using System;
using Server.Factions;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public class DefClassictTinkering : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Tinkering;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044007; // <CENTER>TINKERING MENU</CENTER>
            }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClassictTinkering();

                return m_CraftSystem;
            }
        }

        private DefClassictTinkering()
            : base(1, 1, 1.25)
        {
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1044258 || item.NameNumber == 1046445) // potion keg and faction trap removal kit
                return 0.5; // 50%

            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.
            else if (itemType != null && (itemType.IsSubclassOf(typeof(BaseFactionTrapDeed)) || itemType == typeof(FactionTrapRemovalKit)) && Faction.Find(from) == null)
                return 1044573; // You have to be in a faction to do that.
            else if (itemType == typeof(ModifiedClockworkAssembly) && !(from is PlayerMobile && ((PlayerMobile)from).MechanicalLife))
                return 1113034; // You haven't read the Mechanical Life Manual. Talking to Sutek might help!

            return 0;
        }


                public override void PlayCraftEffect(Mobile from)
        {
            // no sound
            from.PlaySound( 0x241 );
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

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(Silver))
                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }


        public void AddJewelrySet(GemType gemType, Type itemType)
        {
            int offset = (int)gemType - 1;

            int index = this.AddCraft(typeof(GoldRing), 1044049, 1044176 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(SilverBeadNecklace), 1044049, 1044185 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldNecklace), 1044049, 1044194 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldEarrings), 1044049, 1044203 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldBeadNecklace), 1044049, 1044212 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldBracelet), 1044049, 1044221 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);     
             
        }

        /// <summary>
        /// ///////////LISTA
        /// </summary>

        public override void InitCraftList()
        {
            int index = 0;

            #region Jewelry
            AddJewelrySet(GemType.StarSapphire, typeof(StarSapphire));
            AddJewelrySet(GemType.Emerald, typeof(Emerald));
            AddJewelrySet(GemType.Sapphire, typeof(Sapphire));
            AddJewelrySet(GemType.Ruby, typeof(Ruby));
            AddJewelrySet(GemType.Citrine, typeof(Citrine));
            AddJewelrySet(GemType.Amethyst, typeof(Amethyst));
            AddJewelrySet(GemType.Tourmaline, typeof(Tourmaline));
            AddJewelrySet(GemType.Amber, typeof(Amber));
            AddJewelrySet(GemType.Diamond, typeof(Diamond));
            #endregion

            #region Wooden Items
            AddCraft(typeof(JointingPlane), 1044042, 1024144, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(MouldingPlane), 1044042, 1024140, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(SmoothingPlane), 1044042, 1024146, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(ClockFrame), 1044042, 1024173, 0.0, 50.0, typeof(Board), 1044041, 6, 1044351);
            AddCraft(typeof(Axle), 1044042, 1024187, 0.0, 25.0, typeof(Board), 1044041, 2, 1044351);
            AddCraft(typeof(RollingPin), 1044042, 1024163, 0.0, 50.0, typeof(Board), 1044041, 5, 1044351);
            //KEG
            index = this.AddCraft(typeof(Keg), 1044047, 1023699, 65.0, 100.0, typeof(Board), 1044036, 12, 1044037);
            AddRes(index, typeof(BarrelStaves), 1044250, 2, 1044253);
            AddRes(index, typeof(BarrelLid), 1044250, 1, 1044253);
            #endregion

            #region Tools
            AddCraft(typeof(Scissors), 1044046, 1023998, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(MortarPestle), 1044046, 1023739, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(Scorp), 1044046, 1024327, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(TinkerTools), 1044046, 1044164, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Hatchet), 1044046, 1023907, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(DrawKnife), 1044046, 1024324, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(SewingKit), 1044046, 1023997, 10.0, 70.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Saw), 1044046, 1024148, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(DovetailSaw), 1044046, 1024136, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Froe), 1044046, 1024325, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Shovel), 1044046, 1023898, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Hammer), 1044046, 1024138, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Tongs), 1044046, 1024028, 35.0, 85.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(SmithHammer), 1044046, 1025091, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(SledgeHammer), 1044046, 1024021, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Inshave), 1044046, 1024326, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Pickaxe), 1044046, 1023718, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Lockpick), 1044046, 1025371, 45.0, 95.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Skillet), 1044046, 1044567, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(FlourSifter), 1044046, 1024158, 50.0, 100.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(FletcherTools), 1044046, 1044166, 35.0, 85.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(MapmakersPen), 1044046, 1044167, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(ScribesPen), 1044046, 1044168, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Clippers), 1044046, 1112117, 50.0, 50.0, typeof(IronIngot), 1044036, 4, 1044037);
            #endregion


            #region Parts
            AddCraft(typeof(Gears), 1044047, 1024179, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(ClockParts), 1044047, 1024175, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(BarrelTap), 1044047, 1024100, 35.0, 85.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Springs), 1044047, 1024189, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(SextantParts), 1044047, 1024185, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(BarrelHoops), 1044047, 1024321, -15.0, 35.0, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(Hinge), 1044047, 1024181, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(BolaBall), 1044047, 1023699, 45.0, 95.0, typeof(IronIngot), 1044036, 10, 1044037);


            #endregion

            #region Utensils
            AddCraft(typeof(ButcherKnife), 1044048, 1025110, 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(SpoonLeft), 1044048, 1044158, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(SpoonRight), 1044048, 1044159, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Plate), 1044048, 1022519, 0.0, 50.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(ForkLeft), 1044048, 1044160, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(ForkRight), 1044048, 1044161, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Cleaver), 1044048, 1023778, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(KnifeLeft), 1044048, 1044162, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(KnifeRight), 1044048, 1044163, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(typeof(Goblet), 1044048, 1022458, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(SkinningKnife), 1044048, 1023781, 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            #endregion

            #region Misc
            AddCraft(typeof(MetalBox), 1044050, 1024113, 10.0, 60.0, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(MetalChest), 1044050, 1022599, 55.0, 105.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(KeyRing), 1044050, 1024113, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Candelabra), 1044050, 1022599, 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Scales), 1044050, 1026225, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Key), 1044050, 1024112, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(Globe), 1044050, 1024167, 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Spyglass), 1044050, 1025365, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Lantern), 1044050, 1022597, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(HeatingStand), 1044050, 1026217, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            #endregion



            #region Tavern Types
            AddCraft(typeof(PewterMug), 1044048, 1024097, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Pitcher), 1044048, 1024097, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(DecoSpittoon), 1044048, 1024097, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Backgammon), 1044050, 1024113, 15.0, 60.0, typeof(Board), 1044036, 5, 1044037);
            AddCraft(typeof(Chessboard), 1044050, 1022599, 15.0, 60.0, typeof(Board), 1044036, 3, 1044037);
            AddCraft(typeof(Dices), 1044050, 1022599, 5.0, 30.0, typeof(Board), 1044036, 2, 1044037);
            #endregion

            #region Light Source
            AddCraft(typeof(Brazier), 1044048, 1024097, 30.0, 70.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(BrazierTall), 1044048, 1024097, 40.0, 70.0, typeof(IronIngot), 1044036, 15, 1044037);
            #endregion



            #region Traps
            // Dart Trap
            index = this.AddCraft(typeof(DartTrapCraft), 1044052, 1024396, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(Bolt), 1044570, 1, 1044253);

            // Poison Trap
            index = this.AddCraft(typeof(PoisonTrapCraft), 1044052, 1044593, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(BasePoisonPotion), 1044571, 1, 1044253);

            // Explosion Trap
            index = this.AddCraft(typeof(ExplosionTrapCraft), 1044052, 1044597, 55.0, 105.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(BaseExplosionPotion), 1044569, 1, 1044253);
            // other trap
            //AddCraft(typeof(FactionGasTrapDeed), 1044052, 1044598, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            //AddCraft(typeof(FactionExplosionTrapDeed), 1044052, 1044599, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            //AddCraft(typeof(FactionSawTrapDeed), 1044052, 1044600, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            //AddCraft(typeof(FactionSpikeTrapDeed), 1044052, 1044601, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            //AddCraft(typeof(FactionTrapRemovalKit), 1044052, 1046445, 90.0, 115.0, typeof(Silver), 1044572, 500, 1044253);
            #endregion

            #region Multi-Component Items
            index = this.AddCraft(typeof(AxleGears), 1044051, 1024177, 0.0, 0.0, typeof(Axle), 1044169, 1, 1044253);
            AddRes(index, typeof(Gears), 1044569, 1, 1044253);   

            // CRAFT FOR CLOCK RIGHT 
            index = this.AddCraft(typeof(ClockRight), 1044051, 1044257, 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            AddRes(index, typeof(AxleGears), 1044569, 1, 1044253);   
            AddRes(index, typeof(RollingPin), 1044569, 1, 1044253);   
            AddRes(index, typeof(ClockParts), 1044569, 1, 1044253);    
            // CRAFT FOR CLOCK LEFT
            index = this.AddCraft(typeof(ClockLeft), 1044051, 1044256, 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            AddRes(index, typeof(AxleGears), 1044569, 1, 1044253);   
            AddRes(index, typeof(RollingPin), 1044569, 1, 1044253);   
            AddRes(index, typeof(ClockParts), 1044569, 1, 1044253);    

            /////

            //AddCraft(typeof(ClockParts), 1044051, 1024175, 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            AddCraft(typeof(SextantParts), 1044051, 1024185, 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            AddCraft(typeof(Sextant), 1044051, 1024183, 0.0, 0.0, typeof(SextantParts), 1044175, 1, 1044253);
            //AddCraft(typeof(Bola), 1044051, 1046441, 60.0, 80.0, typeof(BolaBall), 1046440, 4, 1042613);
            index = this.AddCraft(typeof(PotionKeg), 1044051, 1044258, 75.0, 100.0, typeof(Keg), 1044255, 1, 1044253);;
            AddRes(index, typeof(Bottle), 1044250, 10, 1044253);
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);
            AddRes(index, typeof(BarrelTap), 1044252, 1, 1044253);
            #endregion

            #region Miscellaneous
            AddCraft(typeof(Key), 1044050, 1024112, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(KeyRing), 1044050, 1024113, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(Spyglass), 1044050, 1025365, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(Scales), 1044050, 1026225, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(typeof(ClockFrame), 1044050, 1024173, 0.0, 50.0, typeof(Board), 1044041, 6, 1044351);
            AddCraft(typeof(Lantern), 1044050, 1022597, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(typeof(HeatingStand), 1044050, 1026217, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            #endregion


            #region Decorative Weapon Armor
            AddCraft(typeof(DecorativeAxeNorth), 1044052, 1024396, 70.0, 80.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(DecorativeAxeWest), 1044052, 1044593, 70.0, 80.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(DecorativeDAxeNorth), 1044052, 1024396, 80.0, 80.0, typeof(IronIngot), 1044036, 40, 1044037);
            AddCraft(typeof(DecorativeDAxeWest), 1044052, 1044593, 80.0, 80.0, typeof(IronIngot), 1044036, 40, 1044037);
            AddCraft(typeof(DecorativeSwordNorth), 1044052, 1024396, 80.0, 80.0, typeof(IronIngot), 1044036, 40, 1044037);
            AddCraft(typeof(DecorativeSwordWest), 1044052, 1044593, 80.0, 80.0, typeof(IronIngot), 1044036, 40, 1044037);
            #endregion


            #region Resources
            SetSubRes(typeof(IronIngot), 1044022);
            SetSubRes(typeof(Board), 1072643);


            AddSubRes(typeof(Board), 1072643, 00.0, 1044041, 1072652);

            AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044267);
            AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044268);
            AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044268);
            AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044268);
            AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044268);
            AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044268);
            AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044268);
            AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044268);
            AddSubRes(typeof(ValoriteIngot), 1044030, 99.0, 1044036, 1044268);
            #endregion








            MarkOption = true;
            Repair = true;
            CanEnhance = Core.AOS;
            CanAlter = Core.SA;
        }
    }

//////////////////
//////
////////////////////
///



    public abstract class TrapCraft : CustomCraft
    {




        private LockableContainer m_Container;
        private Mobile m_From;
        private CraftSystem m_CraftSystem;
        private BaseTool m_Tool;
        private int m_Message;
        private int m_num;

        public LockableContainer Container
        {
            get
            {
                return this.m_Container;
            }
        }

        public abstract TrapType TrapType { get; }

        public TrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {

            this.m_From = from;
            this.m_CraftSystem = craftSystem;
            this.m_Tool = tool;


        }

        private int Verify(LockableContainer container)
        {

            if (container == null || container.KeyValue == 0)
            {
                //return 1005638; // You can only trap lockable chests.
                m_num = 1005638;
                Console.WriteLine("Verify failed: container is null or KeyValue is 0");
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return -1 ;  
            }
            if (this.From.Map != container.Map || !this.From.InRange(container.GetWorldLocation(), 2))
            {
                //return 500446; // That is too far away.
                m_num = 500446;
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return 500446; // That is too far away.
            }
            if (!container.Movable)
            {
                //return 502944; // You cannot trap this item because it is locked down.
                m_num = 502944;
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return 502944; // You cannot trap this item because it is locked down.
            }
            if (!container.IsAccessibleTo(this.From))
            {
                m_num = 502946;
                //return 502946; // That belongs to someone else.
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return 502946; // That belongs to someone else.
            }
            if (container.Locked)
            {
                //return 502943; // You can only trap an unlocked object.
                m_num = 502943;
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return 502943; // You can only trap an unlocked object.
            }
            if (container.TrapType != TrapType.None)
            {
                m_num = 502945;
                m_From.SendMenu(new NewTinkeringMenu(m_From, m_CraftSystem, m_Tool, m_num, true)); // Passa true per isPreAoS
                return 502945; // You can only place one trap on an object at a time.
            }
            return 0;
        }

        private bool Acquire(object target, out int message)
        {
            LockableContainer container = target as LockableContainer;


            int verifyResult = Verify(container);

            if (verifyResult == -1)
            {
                message = m_num;
                
                Console.WriteLine("Acquire failed: Verify returned -1, message: " + message);

                return false;
                //return false;
            }

            if (verifyResult > 0)
            {
                message = verifyResult;
                Console.WriteLine("Acquire failed: Verify returned > 0, message: " + message);
                return false;
            }

            this.m_Container = container;
            message = 0;
            Console.WriteLine("Acquire succeeded, result: " + message);
            return true;

            /*
            message = this.Verify(container);

            if (message > 0)
            {
                return false;
            }
            else
            {
                this.m_Container = container;
                return true;
            }
            */
        }

        public override void EndCraftAction()
        {
            this.From.SendLocalizedMessage(502921); // What would you like to set a trap on?
            this.From.Target = new ContainerTarget(this);
        }

        private class ContainerTarget : Target
        {
            private readonly TrapCraft m_TrapCraft;

            public ContainerTarget(TrapCraft trapCraft)
                : base(-1, false, TargetFlags.None)
            {
                this.m_TrapCraft = trapCraft;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int message;

                if (this.m_TrapCraft.Acquire(targeted, out message))
                    this.m_TrapCraft.CraftItem.CompleteCraft(this.m_TrapCraft.Quality, false, this.m_TrapCraft.From, this.m_TrapCraft.CraftSystem, this.m_TrapCraft.TypeRes, this.m_TrapCraft.Tool, this.m_TrapCraft);
                else
                    this.Failure(message);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    this.Failure(0);
            }

            private void Failure(int message)
            {
                Mobile from = this.m_TrapCraft.From;
                BaseTool tool = this.m_TrapCraft.Tool;

                if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
                {
                    //from.SendGump(new CraftGump(from, this.m_TrapCraft.CraftSystem, tool, message));
                }
                else if (message > 0)
                {
                    from.SendLocalizedMessage(message);
                }
            }
        }

        public override Item CompleteCraft(out int message)
        {
            //message = this.Verify(this.Container);
            int verifyResult = Verify(this.Container);


            if (verifyResult == -1)
            {
                message = m_num; // Messaggio di errore dalla verifica
                Console.WriteLine("CompleteCraft failed: Verify returned -1, message: " + message);
                return null; // Restituisce immediatamente, evitando il consumo dei materiali
            }

            //if (message == 0)
            if (verifyResult == 0) // Successo della verifica
            {
                int trapLevel = (int)(this.From.Skills.Tinkering.Value / 10);

                this.Container.TrapType = this.TrapType;
                this.Container.TrapPower = trapLevel * 9;
                this.Container.TrapLevel = trapLevel;
                this.Container.TrapOnLockpick = true;

                message = 1005639; // Trap is disabled until you lock the chest.
            }
                else
            {
                message = verifyResult; // Messaggio di errore dalla verifica
                Console.WriteLine("CompleteCraft failed: Verify returned > 0, message: " + message);
                return null; // Restituisce immediatamente, evitando il consumo dei materiali
            }


            return null;
        }


    }

    [CraftItemID(0x1BFC)]
    public class DartTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.DartTrap;
            }
        }

        public DartTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality )
            : base(from, craftItem, craftSystem, typeRes, tool, quality )
           
        {
        }
    }

    [CraftItemID(0x113E)]
    public class PoisonTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.PoisonTrap;
            }
        }

        public PoisonTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }

    [CraftItemID(0x370C)]
    public class ExplosionTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.ExplosionTrap;
            }
        }

        public ExplosionTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality )
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }









    /////////////////
}