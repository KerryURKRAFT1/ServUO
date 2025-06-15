using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Fifth
{
    public class MagicReflectSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Reflection", "In Jux Sanct",
            242,
            9012,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);
        private static readonly Hashtable m_Table = new Hashtable();

        public MagicReflectSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Fifth; }
        }

        public static void EndReflect(Mobile m)
        {
            if (m == null)
                return;

            if (m_Table.Contains(m))
            {
                ResistanceMod[] mods = (ResistanceMod[])m_Table[m];
                if (mods != null)
                {
                    for (int i = 0; i < mods.Length; ++i)
                        m.RemoveResistanceMod(mods[i]);
                }
                m_Table.Remove(m);
                BuffInfo.RemoveBuff(m, BuffIcon.MagicReflection);
            }

            DefensiveSpell.Nullify(m);
            m.MagicDamageAbsorb = 0;

            m.FixedEffect(0x37B9, 10, 5);
            m.SendLocalizedMessage(1005558); // "The spell reflection spell is dissipated."
        }

        public override bool CheckCast()
        {
            if (Core.AOS)
                return true;

            return true;
        }

        public override void OnCast()
        {
            if (Core.UOR)
            {
                this.Caster.Target = new InternalTarget(this);
            }
            else if (Core.AOS)
            {
                if (this.CheckSequence())
                {
                    Mobile targ = this.Caster;

                    ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

                    if (mods == null)
                    {
                        targ.PlaySound(0x1E9);
                        targ.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        int physiMod = -25 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
                        int otherMod = 10;

                        mods = new ResistanceMod[5]
                        {
                            new ResistanceMod(ResistanceType.Physical, physiMod),
                            new ResistanceMod(ResistanceType.Fire, otherMod),
                            new ResistanceMod(ResistanceType.Cold, otherMod),
                            new ResistanceMod(ResistanceType.Poison,    otherMod),
                            new ResistanceMod(ResistanceType.Energy,    otherMod)
                        };

                        m_Table[targ] = mods;

                        for (int i = 0; i < mods.Length; ++i)
                            targ.AddResistanceMod(mods[i]);

                        string buffFormat = String.Format("{0}\t+{1}\t+{1}\t+{1}\t+{1}", physiMod, otherMod);

                        BuffInfo.AddBuff(targ, new BuffInfo(BuffIcon.MagicReflection, 1075817, buffFormat, true));
                    }
                    else
                    {
                        targ.PlaySound(0x1ED);
                        targ.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        m_Table.Remove(targ);

                        for (int i = 0; i < mods.Length; ++i)
                            targ.RemoveResistanceMod(mods[i]);

                        BuffInfo.RemoveBuff(targ, BuffIcon.MagicReflection);
                    }
                }

                this.FinishSequence();
            }
            else
            {
                if (HasAnyDefensiveSpell(this.Caster))
                {
                    this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                }
                else if (!this.Caster.CanBeginAction(typeof(DefensiveSpell)))
                {
                    this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                }
                else if (this.CheckSequence())
                {
                    if (this.Caster.BeginAction(typeof(DefensiveSpell)))
                    {
                        int value = (int)(this.Caster.Skills[SkillName.Magery].Value + this.Caster.Skills[SkillName.Inscribe].Value);
                        value = (int)(8 + (value / 200) * 7.0);

                        this.Caster.MagicDamageAbsorb = value;

                        this.Caster.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                        this.Caster.PlaySound(0x1E9);
                    }
                    else
                    {
                        this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                    }
                }

                this.FinishSequence();
            }
        }

        private class InternalTarget : Target
        {
            private MagicReflectSpell m_Owner;

            public InternalTarget(MagicReflectSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                Mobile target = o as Mobile;
                if (target != null)
                    m_Owner.Target(target);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }

        public void Target(Mobile target)
        {
            if (!this.Caster.CanSee(target))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckBSequence(target))
            {
                if (HasAnyDefensiveSpell(target))
                {
                    // Mandalo sia a chi casta che a chi riceve
                    this.Caster.SendLocalizedMessage(1005559);  // Caster: "This spell is already in effect."
                    if (target != this.Caster)
                        target.SendLocalizedMessage(1005559);   // Target: "This spell is already in effect."
                    target.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                    target.PlaySound(0x1E9);
                }
                else
                {
                    if (target.BeginAction(typeof(DefensiveSpell)))
                    {
                        int value = (int)(this.Caster.Skills[SkillName.Magery].Value + this.Caster.Skills[SkillName.Inscribe].Value);
                        value = (int)(8 + (value / 200) * 7.0);

                        target.MagicDamageAbsorb = value;

                        target.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                        target.PlaySound(0x1E9);
                    }
                }
            }

            this.FinishSequence();
        }

        public static bool HasAnyDefensiveSpell(Mobile m)
        {
            return Server.Spells.First.ReactiveArmorSpell.HasArmor(m)
                || Server.Spells.Second.ProtectionSpell.HasProtection(m)
                || HasReflect(m)
                || (m != null && m.MagicDamageAbsorb > 0);
        }

        #region SA
        public static bool HasReflect(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }
        #endregion
    }
}