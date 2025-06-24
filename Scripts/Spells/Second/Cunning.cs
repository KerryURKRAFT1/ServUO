using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
    public class CunningSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cunning", "Uus Wis",
            212,
            9061,
            Reagent.MandrakeRoot,
            Reagent.Nightshade);
        public CunningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Second;
            }
        }
        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
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
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

				SpellHelper.AddStatBonus(this.Caster, m, StatType.Int);
				int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, false) * 100);
				TimeSpan length = SpellHelper.GetDuration(this.Caster, m);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Cunning, 1075843, length, m, percentage.ToString()));

				m.FixedParticles(0x375A, 10, 15, 5011, EffectLayer.Head);
                m.PlaySound(0x1EB);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly CunningSpell m_Owner;
            public InternalTarget(CunningSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
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