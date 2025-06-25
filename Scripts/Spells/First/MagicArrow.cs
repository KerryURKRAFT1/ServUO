using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.First
{
    public class MagicArrowSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Arrow", "In Por Ylem",
            212,
            9041,
            Reagent.SulfurousAsh);

        public MagicArrowSpell(Mobile caster, Item scroll)
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

        public override bool DelayedDamageStacking
        {
            get
            {
                return !Core.AOS;
            }
        }

        public override bool DelayedDamage
        {
            get
            {
                return true;
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
        	if (ObjectTargeted is BaseExplosionPotion)
        	{
        		Explode ((BaseExplosionPotion)ObjectTargeted);
        	}
        	else
        	{
        		Target ((IDamageable)ObjectTargeted);
        	}
        }

        public void Target(IDamageable d)
        {
            Mobile m = d as Mobile;
            Mobile source = this.Caster;

            if (!source.CanSee(d))
            {
                source.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(d))
            {
                SpellHelper.Turn(source, d);

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(10, 1, 4, d);
                }
                else if (m != null)
                {
                    damage = Utility.Random(4, 4);

                    if (this.CheckResisted(m))
                    {
                        damage *= 0.75;
                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(m);
                }

                // PATCH UOR: riflesso diretto (solo se target è Mobile)
                //if (m != null && SpellHelper.CheckReflectUOR(this, ref source, ref m, damage))
                   // return;
                    if (m != null && SpellHelper.CheckReflectUOR(this, source, m, damage))
                    return;
                // FINE PATCH

                // Logica classica (compatibilità legacy)
                if (m != null)
                    SpellHelper.CheckReflect((int)this.Circle, ref source, ref m);

                if (damage > 0)
                {
                    source.MovingParticles(d, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                    source.PlaySound(0x1E5);

                    SpellHelper.Damage(this, m, damage);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private MagicArrowSpell m_Owner;
            public InternalTarget(MagicArrowSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable || o is BaseExplosionPotion)
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
