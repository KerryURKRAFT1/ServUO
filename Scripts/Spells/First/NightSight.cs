using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
    public class NightSightSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Night Sight", "In Lor",
            236,
            9031,
            Reagent.SulfurousAsh,
            Reagent.SpidersSilk);
        public NightSightSpell(Mobile caster, Item scroll)
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
            if (CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                if (m.BeginAction(typeof(LightCycle)))
                {
                    new LightCycle.NightSightTimer(m).Start();
                    int level = (int)(LightCycle.DungeonLevel * Caster.Skills[SkillName.Magery].Value) / 100;

                    if (level < 0)
                        level = 0;

                    m.LightLevel = level;

                    m.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
                    m.PlaySound(0x1E3);

                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.NightSight, 1075643));	//Night Sight/You ignore lighting effects
                }
                else
                {
                    m.SendMessage("{0} already have nightsight.", m == Caster ? "You" : "They");
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NightSightSpell m_Owner;
            
            public InternalTarget(NightSightSpell owner)
                : base(12, false, TargetFlags.Beneficial)
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