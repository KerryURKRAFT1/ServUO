using System;
using Server.Items;
using Server.Spells;

namespace Server.Engines.Craft
{

    public class DefClassicInscription : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Inscribe;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044009;
            }// <CENTER>INSCRIPTION MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefClassicInscription();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefClassicInscription()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            if (typeItem != null)
            {
                object o = Activator.CreateInstance(typeItem);

                if (o is SpellScroll)
                {
                    SpellScroll scroll = (SpellScroll)o;
                    Spellbook book = Spellbook.Find(from, scroll.SpellID);

                    bool hasSpell = (book != null && book.HasSpell(scroll.SpellID));

                    scroll.Delete();

                    return (hasSpell ? 0 : 1042404); // null : You don't have that spell!
                }
                else if (o is Item)
                {
                    ((Item)o).Delete();
                }
            }

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x249);
        }

        private static readonly Type typeofSpellScroll = typeof(SpellScroll);

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) //  not a scroll
            {
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
            else
            {
                if (failed)
                    return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
                else
                    return 501629; // You inscribe the spell and put the scroll in your backpack.
            }
        }

        private int m_Circle, m_Mana;

        private enum Reg { BlackPearl, Bloodmoss, Garlic, Ginseng, MandrakeRoot, Nightshade, SulfurousAsh, SpidersSilk, BatWing, GraveDust, DaemonBlood, NoxCrystal, PigIron, Bone, DragonBlood, FertileDirt, DaemonBone }

        private readonly Type[] m_RegTypes = new Type[]
        {
            typeof( BlackPearl ),
			typeof( Bloodmoss ),
			typeof( Garlic ),
			typeof( Ginseng ),
			typeof( MandrakeRoot ),
			typeof( Nightshade ),
			typeof( SulfurousAsh ),	
			typeof( SpidersSilk ),
            typeof( BatWing ),
            typeof( GraveDust ),
            typeof( DaemonBlood ),
            typeof( NoxCrystal ),
            typeof( PigIron ),
			typeof( Bone ),
			typeof( DragonBlood ),
			typeof( FertileDirt ),
			typeof( DaemonBone )			
        };

        private int m_Index;

        private void AddSpell(Type type, params Reg[] regs)
        {
            double minSkill, maxSkill;
            int cliloc;

            switch (m_Circle)
            {
                default:
                case 0: minSkill = -25.0; maxSkill = 25.0; cliloc = 1111691; break;
                case 1: minSkill = -10.8; maxSkill = 39.2; cliloc = 1111691; break;
                case 2: minSkill = 30.5; maxSkill = 53.5; cliloc = 1111692; break;
                case 3: minSkill = 40.8; maxSkill = 67.8; cliloc = 1111692; break;
                case 4: minSkill = 50.1; maxSkill = 82.1; cliloc = 1111693; break;
                case 5: minSkill = 60.4; maxSkill = 90.4; cliloc = 1111693; break;
                case 6: minSkill = 70.0; maxSkill = 95.0; cliloc = 1111694; break;
                case 7: minSkill = 90.0; maxSkill = 100.0; cliloc = 1111694; break;
            }

            int index = AddCraft(type, cliloc, 1044381 + m_Index++, minSkill, maxSkill, m_RegTypes[(int)regs[0]], 1044353 + (int)regs[0], 1, 1044361 + (int)regs[0]);

            for (int i = 1; i < regs.Length; ++i)
                AddRes(index, m_RegTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, m_Mana);
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Reg[] regs)
        {
            int id = GetRegLocalization(regs[0]);
            int index = AddCraft(type, 1061677, 1060509 + spell, minSkill, minSkill + 1.0, m_RegTypes[(int)regs[0]], id, 1, 501627);

            for (int i = 1; i < regs.Length; ++i)
            {
                id = GetRegLocalization(regs[i]);
                AddRes(index, m_RegTypes[(int)regs[i]], id, 1, 501627);
            }

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        private void AddMysticSpell(int id, int mana, double minSkill, Type type, params Reg[] regs)
        {
            int index = AddCraft(type, 1111671, id, minSkill, minSkill + 1.0, m_RegTypes[(int)regs[0]], GetRegLocalization(regs[0]), 1, 501627);	//Yes, on OSI it's only 1.0 skill diff'.  Don't blame me, blame OSI.

            for (int i = 1; i < regs.Length; ++i)
                AddRes(index, m_RegTypes[(int)regs[0]], GetRegLocalization(regs[i]), 1, 501627);

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        private int GetRegLocalization(Reg reg)
        {
            int loc = 0;

            switch (reg)
            {
                case Reg.BatWing: loc = 1023960; break;
                case Reg.GraveDust: loc = 1023983; break;
                case Reg.DaemonBlood: loc = 1023965; break;
                case Reg.NoxCrystal: loc = 1023982; break;
                case Reg.PigIron: loc = 1023978; break;
                case Reg.Bone: loc = 1023966; break;
                case Reg.DragonBlood: loc = 1023970; break;
                case Reg.FertileDirt: loc = 1023969; break;
                case Reg.DaemonBone: loc = 1023968; break;
            }

            if (loc == 0)
                loc = 1044353 + (int)reg;

            return loc;
        }


        public override void InitCraftList()
        {
            int index = -1;

            index = this.AddCraft(typeof(ReactiveArmorScroll), 1116350, 1044548, -25.0, 25.0, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ClumsyScroll), 1116350, 1044548, -25.0, 25.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            
            index = this.AddCraft(typeof(CreateFoodScroll), 1116350, 1044548, -25.0, 25.0, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            
            index = this.AddCraft(typeof(FeeblemindScroll), 1116350, 1044548, -25.0, 25.0, typeof(Nightshade), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
                  
            index = this.AddCraft(typeof(HealScroll), 1116350, 1044548, -25.0, 25.0, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MagicArrowScroll), 1116350, 1044548, -25.0, 25.0, typeof(SulfurousAsh), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);

            index = this.AddCraft(typeof(NightSightScroll), 1116350, 1044548, -25.0, 25.0, typeof(SpidersSilk), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(WeakenScroll), 1116350, 1044548, -25.0, 25.0, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 4);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);

            // 2 circle

            index = this.AddCraft(typeof(AgilityScroll), 1116350, 1044548, -10.8, 39.2, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(CunningScroll), 1116350, 1044548, -10.8, 39.2, typeof(Nightshade), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(CureScroll), 1116350, 1044548, -10.8, 39.2, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);

            index = this.AddCraft(typeof(HarmScroll), 1116350, 1044548, -10.8, 39.2, typeof(Nightshade), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MagicTrapScroll), 1116350, 1044548, -10.8, 39.2, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MagicUnTrapScroll), 1116350, 1044548, -10.8, 39.2, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ProtectionScroll), 1116350, 1044548, -10.8, 39.2, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);
            
            index = this.AddCraft(typeof(StrengthScroll), 1116350, 1044548, -10.8, 39.2, typeof(Nightshade), 1044358, 1, 1044366);
            this.SetManaReq(index, 6);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);


            // 3 circle



            index = this.AddCraft(typeof(BlessScroll), 1116350, 1044548, 30.5, 53.5, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(FireballScroll), 1116350, 1044548, 30.5, 53.5, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);

            index = this.AddCraft(typeof(MagicLockScroll), 1116350, 1044548, 30.5, 53.5, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(PoisonScroll), 1116350, 1044548, 30.5, 53.5, typeof(Nightshade), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);

            index = this.AddCraft(typeof(TelekinisisScroll), 1116350, 1044548, 30.5, 53.5, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(TeleportScroll), 1116350, 1044548, 30.5, 53.5, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(UnlockScroll), 1116350, 1044548, 30.5, 53.5, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(WallOfStoneScroll), 1116350, 1044548, 30.5, 53.5, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 9);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);

            // 4 circle

            index = this.AddCraft(typeof(ArchCureScroll), 1116350, 1044548, 40.8, 67.8, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ArchProtectionScroll), 1116350, 1044548, 40.8, 67.8, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(CurseScroll), 1116350, 1044548, 40.8, 67.8, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(FireFieldScroll), 1116350, 1044548, 40.8, 67.8, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(GreaterHealScroll), 1116350, 1044548, 40.8, 67.8, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);

            index = this.AddCraft(typeof(LightningScroll), 1116350, 1044548, 40.8, 67.8, typeof(MandrakeRoot), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ManaDrainScroll), 1116350, 1044548, 40.8, 67.8, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(RecallScroll), 1116350, 1044548, 40.8, 67.8, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 11);
            this.AddRes(index, typeof(Bloodmoss), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            // 5 circle

            index = this.AddCraft(typeof(BladeSpiritsScroll), 1116350, 1044548, 50.1, 82.1, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(DispelFieldScroll), 1116350, 1044548, 50.1, 82.1, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(IncognitoScroll), 1116350, 1044548, 50.1, 82.1, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MagicReflectScroll), 1116350, 1044548, 50.1, 82.1, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MindBlastScroll), 1116350, 1044548, 50.1, 82.1, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ParalyzeScroll), 1116350, 1044548, 50.1, 82.1, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(PoisonFieldScroll), 1116350, 1044548, 50.1, 82.1, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonCreatureScroll), 1116350, 1044548, 50.1, 82.1, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 14);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);


            // 6 circle

            index = this.AddCraft(typeof(DispelScroll), 1116350, 1044548, 60.4, 90.4, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(EnergyBoltScroll), 1116350, 1044548, 60.4, 90.4, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ExplosionScroll), 1116350, 1044548, 60.4, 90.4, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(InvisibilityScroll), 1116350, 1044548, 60.4, 90.4, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MarkScroll), 1116350, 1044548, 60.4, 90.4, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(BlackPearl), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MassCurseScroll), 1116350, 1044548, 60.4, 90.4, typeof(Garlic), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ParalyzeFieldScroll), 1116350, 1044548, 60.4, 90.4, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(RevealScroll), 1116350, 1044548, 60.4, 90.4, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 20);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);


            // 7 circle

            index = this.AddCraft(typeof(ChainLightningScroll), 1116350, 1044548, 70.0, 95.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(Bloodmoss), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(EnergyFieldScroll), 1116350, 1044548, 70.0, 95.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(FlamestrikeScroll), 1116350, 1044548, 70.0, 95.0, typeof(SpidersSilk), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(GateTravelScroll), 1116350, 1044548, 70.0, 95.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ManaVampireScroll), 1116350, 1044548, 70.0, 95.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(Bloodmoss), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MassDispelScroll), 1116350, 1044548, 70.0, 95.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(MeteorSwarmScroll), 1116350, 1044548, 70.0, 95.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(PolymorphScroll), 1116350, 1044548, 70.0, 95.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 40);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);


            // 8 circle

            index = this.AddCraft(typeof(EarthquakeScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(EnergyVortexScroll), 1116350, 1044548, 90.0, 100.0, typeof(BlackPearl), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(Bloodmoss), 1044529, 1, 500315);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(Nightshade), 1044529, 1, 500315);

            index = this.AddCraft(typeof(ResurrectionScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(Garlic), 1044529, 1, 500315);
            this.AddRes(index, typeof(Ginseng), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonAirElementalScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonDaemonScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonEarthElementalScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonFireElementalScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);
            this.AddRes(index, typeof(SulfurousAsh), 1044529, 1, 500315);

            index = this.AddCraft(typeof(SummonWaterElementalScroll), 1116350, 1044548, 90.0, 100.0, typeof(Bloodmoss), 1044358, 1, 1044366);
            this.SetManaReq(index, 50);
            this.AddRes(index, typeof(MandrakeRoot), 1044529, 1, 500315);
            this.AddRes(index, typeof(SpidersSilk), 1044529, 1, 500315);

            // end

            this.MarkOption = true;
        }



        /*
        public override void InitCraftList()
        {
            m_Circle = 0;
			m_Mana = 4;

			AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
			AddSpell( typeof( CreateFoodScroll ), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot );
			AddSpell( typeof( FeeblemindScroll ), Reg.Nightshade, Reg.Ginseng );
			AddSpell( typeof( HealScroll ), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk );
			AddSpell( typeof( MagicArrowScroll ), Reg.SulfurousAsh );
			AddSpell( typeof( NightSightScroll ), Reg.SpidersSilk, Reg.SulfurousAsh );
            AddSpell( typeof( WeakenScroll ), Reg.Garlic, Reg.Nightshade );

			m_Circle = 1;
			m_Mana = 6;

			AddSpell( typeof( AgilityScroll ), Reg.Bloodmoss, Reg.MandrakeRoot );
			AddSpell( typeof( CunningScroll ), Reg.Nightshade, Reg.MandrakeRoot );
			AddSpell( typeof( CureScroll ), Reg.Garlic, Reg.Ginseng );
			AddSpell( typeof( HarmScroll ), Reg.Nightshade, Reg.SpidersSilk );
			AddSpell( typeof( MagicTrapScroll ), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( MagicUnTrapScroll ), Reg.Bloodmoss, Reg.SulfurousAsh );
			AddSpell( typeof( ProtectionScroll ), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh );
			AddSpell( typeof( StrengthScroll ), Reg.Nightshade, Reg.MandrakeRoot );

			m_Circle = 2;
			m_Mana = 9;

			AddSpell( typeof( BlessScroll ), Reg.Garlic, Reg.MandrakeRoot );
			AddSpell( typeof( FireballScroll ), Reg.BlackPearl );
			AddSpell( typeof( MagicLockScroll ), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh );
			AddSpell( typeof( PoisonScroll ), Reg.Nightshade );
			AddSpell( typeof( TelekinisisScroll ), Reg.Bloodmoss, Reg.MandrakeRoot );
			AddSpell( typeof( TeleportScroll ), Reg.Bloodmoss, Reg.MandrakeRoot );
			AddSpell( typeof( UnlockScroll ), Reg.Bloodmoss, Reg.SulfurousAsh );
			AddSpell( typeof( WallOfStoneScroll ), Reg.Bloodmoss, Reg.Garlic );

			m_Circle = 3;
			m_Mana = 11;

			AddSpell( typeof( ArchCureScroll ), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot );
			AddSpell( typeof( ArchProtectionScroll ), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( CurseScroll ), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh );
			AddSpell( typeof( FireFieldScroll ), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( GreaterHealScroll ), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng );
			AddSpell( typeof( LightningScroll ), Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( ManaDrainScroll ), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot );
			AddSpell( typeof( RecallScroll ), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot );

			m_Circle = 4;
			m_Mana = 14;

			AddSpell( typeof( BladeSpiritsScroll ), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot );
			AddSpell( typeof( DispelFieldScroll ), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( IncognitoScroll ), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade );
			AddSpell( typeof( MagicReflectScroll ), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk );
			AddSpell( typeof( MindBlastScroll ), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh );
			AddSpell( typeof( ParalyzeScroll ), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk );
			AddSpell( typeof( PoisonFieldScroll ), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk );
			AddSpell( typeof( SummonCreatureScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );

			m_Circle = 5;
			m_Mana = 20;

			AddSpell( typeof( DispelScroll ), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( EnergyBoltScroll ), Reg.BlackPearl, Reg.Nightshade );
			AddSpell( typeof( ExplosionScroll ), Reg.Bloodmoss, Reg.MandrakeRoot );
			AddSpell( typeof( InvisibilityScroll ), Reg.Bloodmoss, Reg.Nightshade );
			AddSpell( typeof( MarkScroll ), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot );
			AddSpell( typeof( MassCurseScroll ), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh );
			AddSpell( typeof( ParalyzeFieldScroll ), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk );
			AddSpell( typeof( RevealScroll ), Reg.Bloodmoss, Reg.SulfurousAsh );

			m_Circle = 6;
			m_Mana = 40;

			AddSpell( typeof( ChainLightningScroll ), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( EnergyFieldScroll ), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( FlamestrikeScroll ), Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( GateTravelScroll ), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( ManaVampireScroll ), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );
			AddSpell( typeof( MassDispelScroll ), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh );
			AddSpell( typeof( MeteorSwarmScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk );
			AddSpell( typeof( PolymorphScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );

			m_Circle = 7;
			m_Mana = 50;

			AddSpell( typeof( EarthquakeScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh );
			AddSpell( typeof( EnergyVortexScroll ), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade );
			AddSpell( typeof( ResurrectionScroll ), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng );
			AddSpell( typeof( SummonAirElementalScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );
			AddSpell( typeof( SummonDaemonScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( SummonEarthElementalScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );
			AddSpell( typeof( SummonFireElementalScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh );
			AddSpell( typeof( SummonWaterElementalScroll ), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk );


            int index;
			
            if (Core.ML)
            {
                index = this.AddCraft(typeof(EnchantedSwitch), 1044294, 1072893, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
                this.AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
                this.AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
                this.AddRes(index, typeof(SwitchItem), 1073464, 1, 1044253);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
				
                index = this.AddCraft(typeof(RunedPrism), 1044294, 1073465, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
                this.AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
                this.AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
                this.AddRes(index, typeof(HollowPrism), 1072895, 1, 1044253);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
            }
			
            // Runebook
            index = this.AddCraft(typeof(Runebook), 1044294, 1041267, 45.0, 95.0, typeof(BlankScroll), 1044377, 8, 1044378);
            this.AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
            this.AddRes(index, typeof(GateTravelScroll), 1044446, 1, 1044253);


            if (Core.AOS)
            {
                this.AddCraft(typeof(Engines.BulkOrders.BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 10, 1044378);
            }

            if (Core.SE)
            {
                this.AddCraft(typeof(Spellbook), 1044294, 1023834, 50.0, 126, typeof(BlankScroll), 1044377, 10, 1044378);
            }
			

            this.MarkOption = true;
        }
        */
    }
}