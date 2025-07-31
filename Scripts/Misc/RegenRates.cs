using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.SkillMasteries;
using Server.SkillHandlers;

namespace Server.Misc
{
    public delegate Int32 RegenBonusHandler(Mobile from);

    public class RegenRates
    {
        public static List<RegenBonusHandler> HitsBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> StamBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> ManaBonusHandlers = new List<RegenBonusHandler>();

        [CallPriority(10)]
        public static void Configure()
        {
            Mobile.DefaultHitsRate = TimeSpan.FromSeconds(11.0);
            Mobile.DefaultStamRate = TimeSpan.FromSeconds(4.0);
            Mobile.DefaultManaRate = TimeSpan.FromSeconds(7.0);

            Mobile.ManaRegenRateHandler = new RegenRateHandler(Mobile_ManaRegenRate);

            if (Core.AOS)
            {
                Mobile.StamRegenRateHandler = new RegenRateHandler(Mobile_StamRegenRate);
                Mobile.HitsRegenRateHandler = new RegenRateHandler(Mobile_HitsRegenRate);
            }
        }

        public static double GetArmorOffset(Mobile from)
        {
            double rating = 0.0;

            if (!Core.AOS)
                rating += GetArmorMeditationValue(from.ShieldArmor as BaseArmor);

            rating += GetArmorMeditationValue(from.NeckArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HandArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HeadArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ArmsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.LegsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ChestArmor as BaseArmor);

            return rating / 4;
        }

        private static void CheckBonusSkill(Mobile m, int cur, int max, SkillName skill)
        {
            if (!m.Alive)
                return;

            double n = (double)cur / max;
            double v = Math.Sqrt(m.Skills[skill].Value * 0.005);

            n *= (1.0 - v);
            n += v;

            m.CheckSkill(skill, n);
        }

        private static TimeSpan Mobile_HitsRegenRate(Mobile from)
        {
            int points = AosAttributes.GetValue(from, AosAttribute.RegenHits);

            if (from is BaseCreature && !((BaseCreature)from).IsAnimatedDead)
                points += 4;

            if ((from is BaseCreature && ((BaseCreature)from).IsParagon) || from is Leviathan)
                points += 40;
          
            if (points < 0)
                points = 0;

            return TimeSpan.FromSeconds(1.0 / (0.1 * (1 + points)));
        }

        private static TimeSpan Mobile_StamRegenRate(Mobile from)
        {
    	    if (from.Meditating)		
    	    {
    	    	return SkillRegistry.MeditateStamRate;
    	    }

       		return Mobile.DefaultStamRate;
        }
       		
       	private static TimeSpan Mobile_ManaRegenRate(Mobile from)
        {
            if (from.Skills == null)
                return Mobile.DefaultManaRate;

            if (!from.Meditating)
                CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Meditation);

            double rate;
            double armorPenalty = GetArmorOffset(from);

            double medPoints = (from.Int + from.Skills[SkillName.Meditation].Value) * 0.5;

            if (medPoints <= 0)
                rate = 7.0;
            else if (medPoints <= 100)
                rate = 7.0 - (239 * medPoints / 2400) + (19 * medPoints * medPoints / 48000);
            else if (medPoints < 120)
                rate = 1.0;
            else
                rate = 0.75;

            rate += armorPenalty;

            if (from.Meditating)
            {
            	if (IsMoving(from))
            	{
            		new LockTimer(from, SkillRegistry.LongDelay).Start();
            	}
            	
            	if (from.MeditationLock)
            	{               		
            		rate *= SkillRegistry.MeditateSlowRate;
            	}
            	else
            	{
            		rate *= SkillRegistry.MeditateFastRate;
            	}
            }

            if (rate < 0.5)
                rate = 0.5;
            else if (rate > 7.0)
                rate = 7.0;

            return TimeSpan.FromSeconds(rate);
        }

		private static bool IsMoving(Mobile m)
		{
			return !m.MeditationLock && (Core.TickCount - m.LastMoveTime <= m.ComputeMovementSpeed(m.Direction));
		}

        private class LockTimer : Timer
		{
			private readonly Mobile m_Owner;

            public LockTimer(Mobile owner, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;

				Priority = TimerPriority.TwoFiftyMS;
				
				m_Owner.MeditationLock = true;
            }

			protected override void OnTick()
			{
				m_Owner.MeditationLock = false;
			}
		}
        
		private static double GetArmorMeditationValue(BaseArmor ar)
        {
            if (ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0)
                return 0.0;

            switch ( ar.MeditationAllowance )
            {
                default:
                case ArmorMeditationAllowance.None:
                    return ar.BaseArmorRatingScaled;
                case ArmorMeditationAllowance.Half:
                    return ar.BaseArmorRatingScaled / 2.0;
                case ArmorMeditationAllowance.All:
                    return 0.0;
            }
        }
    }
}