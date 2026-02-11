using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Seventh
{
    public class FlameStrikeSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Flame Strike", "Kal Vas Flam",
            245,
            9042,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);

        public FlameStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
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

        public void Target(IDamageable m)
        {
            Mobile mob = m as Mobile;

            if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                // CheckReflect classico (AOS/Pre-AOS)
                if (mob != null && !Core.UOR)
                    SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref mob);

                // Spell interruption
                if (mob != null && mob.Spell != null)
                    mob.Spell.OnCasterHurt();

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(48, 1, 5, m);
                }
                else if (Core.UOR)
                {
                    damage = Utility.Random(58, 7); // 58-64

                    // Effetti visivi e sonori PRIMA del check reflect
                    if (mob != null)
                    {
                        mob.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                        mob.PlaySound(0x208);
                    }
                    else
                    {
                        Effects.SendLocationParticles(m, 0x3709, 10, 30, 5052);
                        Effects.PlaySound(m.Location, m.Map, 0x208);
                    }

                    // PATCH UOR: riflesso diretto (DOPO gli effetti)
                    if (mob != null && SpellHelper.CheckReflectUOR(this, this.Caster, mob, damage))
                    {
                        this.FinishSequence();
                        return;
                    }

                    // Magic Resistance (DOPO il check reflection)
                    if (mob != null && this.CheckResisted(mob))
                    {
                        damage /= 2.0;
                        mob.SendMessage(0x22, "You resist the spell!");
                    }
                }
                else if (mob != null)
                {
                    damage = Utility.Random(27, 22);

                    if (this.CheckResisted(mob))
                    {
                        damage *= 0.6;
                    }

                    damage *= this.GetDamageScalar(mob);
                }

                // Effetti visivi e sonori per AOS/Old (NON UOR)
                if (!Core.UOR)
                {
                    if (mob != null)
                    {
                        mob.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                        mob.PlaySound(0x208);
                    }
                    else
                    {
                        Effects.SendLocationParticles(m, 0x3709, 10, 30, 5052);
                        Effects.PlaySound(m.Location, m.Map, 0x208);
                    }
                }

                if (damage > 0)
                {
                    SpellHelper.Damage(this, m, damage, 0, 100, 0, 0, 0);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FlameStrikeSpell m_Owner;

            public InternalTarget(FlameStrikeSpell owner)
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