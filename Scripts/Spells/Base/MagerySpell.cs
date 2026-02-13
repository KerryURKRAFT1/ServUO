using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Spells
{
    public abstract class MagerySpell : Spell
    {
		private static readonly int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };
		
		private static readonly double[] m_SkillOffset = new double[] { 20.0, 20.0, 20.0, 20.0, 20.0, 20.0, 14.25, 10.0 };
        
		public MagerySpell(Mobile caster, Item scroll, SpellInfo info) : base(caster, scroll, info)
		{
		}

        public abstract SpellCircle Circle { get; }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds((3 + (int)this.Circle) * this.CastDelaySecondsPerTick);
            }
        }
        
		public override bool OnCasterMoving(Direction d)
		{
			return true;
		}

		public override bool ConsumeReagents()
        {
			//set up special exceptions
//			if (this.Caster.Region is <eg.DuellingRegion>)
//                return true;

			if (base.ConsumeReagents())
			{
				return true;
			}

            if (ArcaneGem.ConsumeCharges(this.Caster, (Core.SE ? 1 : 1 + (int)this.Circle)))
			{
				return true;
			}

            return false;
        }

		public override void GetCastSkills( out double min, out double max )
		{
			int circle = (int)Circle;

			if ( this.Scroll != null )
			{
				circle -= Math.Max (0, circle - 2);
			}

			double avg = (100.0 / 7) * circle;
			
			min = avg - 20;
			
			max = avg + m_SkillOffset[circle];
		}

		public override int GetMana()
        {
            if (this.Scroll is BaseWand)
			{
                return 0;
			}

            return m_ManaTable[(int)this.Circle];
        }

		public override int ScaleMana(int mana)
		{
			double scalar = 1.0;
		
			return (int)(mana * scalar);
		}
		
		public override double GetResistSkill(Mobile m)
        {
            int maxSkill = (1 + (int)this.Circle) * 10;
			
            maxSkill += (1 + ((int)this.Circle / 6)) * 25;

            if (m.Skills[SkillName.MagicResist].Value < maxSkill)
			{
                m.CheckSkill(SkillName.MagicResist, 0.0, m.Skills[SkillName.MagicResist].Cap);
			}

            return m.Skills[SkillName.MagicResist].Value;
        }

        public virtual bool CheckResisted(Mobile target)
        {
            double n = this.GetResistPercent(target);

            n /= 100.0;

            if (n <= 0.0)
			{
                return false;
			}

            if (n >= 1.0)
			{
                return true;
			}

            int maxSkill = (1 + (int)this.Circle) * 10;
			
            maxSkill += (1 + ((int)this.Circle / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
			{
                target.CheckSkill(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);
			}

            return (Caster != target && n >= Utility.RandomDouble());
        }

        public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
            if (Core.UOR)
            {
                // SPHERE 51-55 STYLE
                double resistSkill = target.Skills[SkillName.MagicResist].Value;
                double circleModifier = ((int)circle + 1) * 10.0;
                double resistChance = (resistSkill / circleModifier) * 100.0;
                
                if (resistChance > 70.0)
                    resistChance = 70.0;
                
                return resistChance;
            }


            double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			
            double secondPercent = target.Skills[SkillName.MagicResist].Value - (((this.Caster.Skills[this.CastSkill].Value - 20.0) / 5.0) + (1 + (int)circle) * 5.0);

            return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
        }

        public virtual double GetResistPercent(Mobile target)
        {
            return this.GetResistPercentForCircle(target, this.Circle);
        }

        public override TimeSpan GetCastDelay()
        {
            if (!Core.ML && this.Scroll is BaseWand)
			{
                return TimeSpan.Zero;
			}

            if (!Core.AOS)
            {
				return TimeSpan.FromSeconds(((int)this.Circle * 0.25) + 0.5); //according to UO.com/wiki circle 1:0.5secs circle 8:2.25secs
            }
	
            return base.GetCastDelay();
        } 
 	}
}