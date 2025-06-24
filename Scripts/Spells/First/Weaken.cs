using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
    public class WeakenSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Weaken", "Des Mani",
            212,
            9031,
            Reagent.Garlic,
            Reagent.Nightshade);
        public WeakenSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.First;
            }
        }

        public override bool Cast()
        {
        	if (this.Caster.Mana > (Mana = ScaleMana(GetMana())))
        	{
        		return (this.Caster.Target = new InternalTarget(this)) != null;
        	}

        	this.Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana
        	
        	return false;
        }

        public override void OnCast()
        {
        	Target ((Mobile)ObjectTargeted);
        }
        
        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

				SpellHelper.AddStatCurse(this.Caster, m, StatType.Str);
				int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, true) * 100);
				TimeSpan length = SpellHelper.GetDuration(this.Caster, m);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Weaken, 1075837, length, m, percentage.ToString()));

				if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5009, EffectLayer.Waist);
                m.PlaySound(0x1E6);

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly WeakenSpell m_Owner;
            public InternalTarget(WeakenSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
	             	if (!this.m_Owner.StartSequence(o))
	            	{
	            		this.m_Owner.FinishSequence();
	            	}
                }
                else
                {
	              	from.SendLocalizedMessage(1005213); // You can't do that
                }
            }
        }
    }
}