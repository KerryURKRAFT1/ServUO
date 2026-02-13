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
                Explode((BaseExplosionPotion)ObjectTargeted);
            }
            else
            {
                Target((IDamageable)ObjectTargeted);
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

                // CheckReflect classico (AOS/Pre-AOS)
                if (m != null && !Core.UOR)
                    SpellHelper.CheckReflect((int)this.Circle, source, ref m);

                // Spell interruption
                if (m != null && m.Spell != null)
                    m.Spell.OnCasterHurt();

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(10, 1, 4, d);
                }
                else if (Core.UOR) 
                {
                    damage = Utility.Random(6, 5); // 6-10
                    
                    // Effetti visivi e sonori PRIMA del check reflect
                    if (m != null)
                    {
                        source.MovingParticles(m, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                        source.PlaySound(0x1E5);
                    }
                    else
                    {
                        source.MovingParticles(d, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                        source.PlaySound(0x1E5);
                    }
                    
                    // PATCH UOR: riflesso diretto (DOPO gli effetti)
                    if (m != null && SpellHelper.CheckReflectUOR(this, source, m, damage))
                    {
                        this.FinishSequence();
                        return;
                    } 
                    
                    // Magic Resistance (DOPO il check reflection)
                    if (m != null && this.CheckResisted(m))
                    {
                        damage /= 2.0;
                        m.SendMessage(0x22, "You resist the spell!");
                    }
                }
                else if (m != null)
                {
                    damage = Utility.Random(4, 4);

                    if (this.CheckResisted(m))
                    {
                        damage *= 0.75;
                    }

                    damage *= this.GetDamageScalar(m);
                }
                
                // Effetti visivi e sonori per AOS/Old (NON UOR)
                if (!Core.UOR)
                {
                    if (m != null)
                    {
                        source.MovingParticles(m, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                        source.PlaySound(0x1E5);
                    }
                    else
                    {
                        source.MovingParticles(d, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                        source.PlaySound(0x1E5);
                    }
                }

                if (damage > 0)
                {
                    SpellHelper.Damage(this, d, damage, 0, 0, 0, 0, 100);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private MagicArrowSpell m_Owner;
            
            public InternalTarget(MagicArrowSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
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
            
            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.Target = new InternalTarget(m_Owner);
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237); // Target can not be seen.
            }
        }
    }
}