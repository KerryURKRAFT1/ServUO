using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
    public class FeeblemindSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Feeblemind", "Rel Wis",
            212,
            9031,
            Reagent.Ginseng,
            Reagent.Nightshade);

        public FeeblemindSpell(Mobile caster, Item scroll)
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
            Target((Mobile)ObjectTargeted);
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

                // CheckReflect classico (AOS/Pre-AOS)
                if (!Core.UOR)
                    SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

                // Spell interruption
                if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                if (Core.UOR)
                {
                    // Effetti visivi e sonori PRIMA del check reflect
                    m.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                    m.PlaySound(0x1E4);

                    // PATCH UOR: riflesso diretto (DOPO gli effetti)
                    if (SpellHelper.CheckReflectUOR(this, this.Caster, ref m))
                    {
                        this.FinishSequence();
                        return;
                    }
                }

                // Applica l'effetto Feeblemind
                SpellHelper.AddStatCurse(this.Caster, m, StatType.Int);
                int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, true) * 100);
                TimeSpan length = SpellHelper.GetDuration(this.Caster, m);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.FeebleMind, 1075833, length, m, percentage.ToString()));

                m.Paralyzed = false;

                // Effetti visivi e sonori per AOS/Old (NON UOR)
                if (!Core.UOR)
                {
                    m.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                    m.PlaySound(0x1E4);
                }

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FeeblemindSpell m_Owner;

            public InternalTarget(FeeblemindSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
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

            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.Target = new InternalTarget(m_Owner);
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237); // Target can not be seen.
            }
        }
    }
}