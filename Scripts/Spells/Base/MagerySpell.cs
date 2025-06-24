using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells
{
    public abstract class MagerySpell : Spell
    {
        private static readonly int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };
        private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;
        
        public MagerySpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
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
			if (this.Caster.AccessLevel > AccessLevel.Player)
                return true;

			//set up special exceptions
//			if (this.Caster.Region is <eg.DuellingRegion>)
//                return true;

			if (base.ConsumeReagents())
                return true;

            if (ArcaneGem.ConsumeCharges(this.Caster, (Core.SE ? 1 : 1 + (int)this.Circle)))
                return true;

            return false;
        }

		public override void GetCastSkills( out double min, out double max )
		{
			int circle = (int)Circle;

			if ( this.Scroll != null )
				circle -= 2;

			double avg = 100.0 * circle / 7;

			min = avg - 20;
			max = avg + 20;
		}

		public override int GetMana()
        {
            if (this.Scroll is BaseWand)
                return 0;

            return m_ManaTable[(int)this.Circle];
        }

        public override double GetResistSkill(Mobile m)
        {
            int maxSkill = (1 + (int)this.Circle) * 10;
            maxSkill += (1 + ((int)this.Circle / 6)) * 25;

            if (m.Skills[SkillName.MagicResist].Value < maxSkill)
                m.CheckSkill(SkillName.MagicResist, 0.0, m.Skills[SkillName.MagicResist].Cap);

            return m.Skills[SkillName.MagicResist].Value;
        }

        public virtual bool CheckResisted(Mobile target)
        {
            double n = this.GetResistPercent(target);

            n /= 100.0;

            if (n <= 0.0)
                return false;

            if (n >= 1.0)
                return true;

            int maxSkill = (1 + (int)this.Circle) * 10;
            maxSkill += (1 + ((int)this.Circle / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
                target.CheckSkill(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);

            return (n >= Utility.RandomDouble());
        }

        public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
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
                return TimeSpan.Zero;

            if (!Core.AOS)
            {
            	return TimeSpan.FromSeconds((((int)this.Circle + 1) * 0.25) + 0.5); //corrected UOR formula circle 1:0.75 circle 2:1.0 circle 3:1.25 etc circle 8:2.5
            }
	
            return base.GetCastDelay();
        }
 
		public override void CheckLOS()
		{
        	if(!this.Caster.Blessed || this.TravelSpell) //sanity
            {
        		return;
        	}

        	if( this.ObjectTargeted != null )
    		{
        		IPoint3D loc = this.ObjectTargeted as IPoint3D;

        		if( loc != null )
        		{
					if( !this.Caster.InLOS( new Point3D( loc )) || !this.Caster.CanSee( this.ObjectTargeted ))
					{
						this.Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
		           		DoFizzle();
					}
	        		else if( !this.Caster.InRange( new Point3D( loc ), 12 ))
					{
						this.Caster.SendLocalizedMessage( 1076203 ); // Target out of range.
		           		DoFizzle();
	                }
        		}
            }
		}
    }
}