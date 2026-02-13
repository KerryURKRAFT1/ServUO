using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Sixth
{
    public class ExplosionSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Explosion", "Vas Ort Flam",
            230,
            9041,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
            }
        }

        public ExplosionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
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
                Target((Mobile)ObjectTargeted);
            }
        }

        public void Target(Mobile defender)
        {
            if (!this.Caster.CanSee(defender))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(defender))
            {
                SpellHelper.Turn(this.Caster, defender);

                Mobile originalTarget = defender;

                // CheckReflect classico (AOS/Pre-AOS)
                if (!Core.UOR)
                    SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref defender);

                // Spell interruption
                if (defender != null && defender.Spell != null)
                    defender.Spell.OnCasterHurt();

                // Delay timer (2.5 sec per UOR/Pre-AOS, 3.0 sec per AOS)
                InternalTimer t = new InternalTimer(this, this.Caster, defender, originalTarget);
                t.Start();
            }

            FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly ExplosionSpell m_Spell;
            private readonly Mobile m_Caster;
            private Mobile m_Target;
            private readonly Mobile m_OriginalTarget;

            public InternalTimer(ExplosionSpell spell, Mobile caster, Mobile target, Mobile originalTarget)
                : base(TimeSpan.FromSeconds(Core.AOS ? 3.0 : 2.5))
            {
                m_Spell = spell;
                m_Caster = caster;
                m_Target = target;
                m_OriginalTarget = originalTarget;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Caster == null || m_Target == null)
                    return;

                if (m_Caster.HarmfulCheck(m_Target))
                {
                    double damage = 0;

                    if (Core.AOS)
                    {
                        damage = Utility.Random(23, 22);
                    }
                    else if (Core.UOR)
                    {
                        damage = Utility.Random(33, 18); // 33-45

                        // Effetti visivi e sonori PRIMA del check reflect
                        m_Caster.DoHarmful(m_Target);
                        Effects.SendLocationParticles(
                            EffectItem.Create(m_Target.Location, m_Target.Map, EffectItem.DefaultDuration),
                            0x36BD, 20, 10, 5044);
                        Effects.PlaySound(m_Target.Location, m_Target.Map, 0x307);

                        // PATCH UOR: riflesso diretto (DOPO gli effetti)
                        if (m_Target != null && SpellHelper.CheckReflectUOR(m_Spell, m_Caster, m_Target, damage))
                        {
                            // Riflessa o neutralizzata
                            return;
                        }

                        // Magic Resistance (DOPO il check reflection)
                        if (m_Target != null && m_Spell.CheckResisted(m_Target))
                        {
                            damage /= 2.0;
                            m_Target.SendMessage(0x22, "You resist the spell!");
                        }
                    }
                    else
                    {
                        damage = Utility.Random(23, 22);

                        if (m_Spell.CheckResisted(m_Target))
                        {
                            damage *= 0.75;
                        }

                        damage *= m_Spell.GetDamageScalar(m_Target);
                    }

                    // Effetti visivi per AOS/Pre-AOS (NON UOR)
                    if (!Core.UOR)
                    {
                        m_Caster.DoHarmful(m_Target);
                        Effects.SendLocationParticles(
                            EffectItem.Create(m_Target.Location, m_Target.Map, EffectItem.DefaultDuration),
                            0x36BD, 20, 10, 5044);
                        Effects.PlaySound(m_Target.Location, m_Target.Map, 0x307);
                    }

                    // Danno
                    if (damage > 0)
                    {
                        SpellHelper.Damage(m_Spell, m_Target, damage, 0, 100, 0, 0, 0);
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly ExplosionSpell m_Owner;

            public InternalTarget(ExplosionSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile || o is BaseExplosionPotion)
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