using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
    public class ReactiveArmorSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Reactive Armor", "Flam Sanct",
            236,
            9011,
            Reagent.Garlic,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        private static readonly Hashtable m_Table = new Hashtable();

        public ReactiveArmorSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public static void EndArmor(Mobile m)
        {
            if (m_Table.Contains(m))
            {
                ResistanceMod[] mods = (ResistanceMod[])m_Table[m];
                if (mods != null)
                {
                    for (int i = 0; i < mods.Length; ++i)
                        m.RemoveResistanceMod(mods[i]);
                }
                m_Table.Remove(m);
                BuffInfo.RemoveBuff(m, BuffIcon.ReactiveArmor);
            }
        }

        public override bool CheckCast()
        {
            if (Core.AOS || Core.UOR)
                return true;

            if (this.Caster.MeleeDamageAbsorb > 0)
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                return false;
            }
            else if (!this.Caster.CanBeginAction(typeof(DefensiveSpell)))
            {
                this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                return false;
            }
            return true;
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
            if (this.Caster.MeleeDamageAbsorb > 0)
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
            }
            else if (!this.Caster.CanBeginAction(typeof(DefensiveSpell)))
            {
                this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
            }
            else if (this.CheckSequence())
            {
                if (HasAnyDefensiveSpell(this.Caster))
                {
                    this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                }
                else if (this.Caster.BeginAction(typeof(DefensiveSpell)))
                {
                    int value = (int)(this.Caster.Skills[SkillName.Magery].Value + this.Caster.Skills[SkillName.Meditation].Value + this.Caster.Skills[SkillName.Inscribe].Value);
                    value /= 3;

                    if (value < 0)
                        value = 1;
                    else if (value > 75)
                        value = 75;

                    this.Caster.MeleeDamageAbsorb = value;

                    this.Caster.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
                    this.Caster.PlaySound(0x1F2);
                }
                else
                {
                    this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                }
            }
            
            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private ReactiveArmorSpell m_Owner;

            public InternalTarget(ReactiveArmorSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
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

        private void ApplyArmor(Mobile targ)
        {
            ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

            if (mods == null)
            {
                targ.PlaySound(0x1E9);
                targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                mods = new ResistanceMod[5]
                {
                    new ResistanceMod(ResistanceType.Physical, 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20)),
                    new ResistanceMod(ResistanceType.Fire, -5),
                    new ResistanceMod(ResistanceType.Cold, -5),
                    new ResistanceMod(ResistanceType.Poison, -5),
                    new ResistanceMod(ResistanceType.Energy, -5)
                };

                m_Table[targ] = mods;

                for (int i = 0; i < mods.Length; ++i)
                    targ.AddResistanceMod(mods[i]);

                int physresist = 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
                string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", physresist, 5, 5, 5, 5);

                BuffInfo.AddBuff(targ, new BuffInfo(BuffIcon.ReactiveArmor, 1075812, 1075813, args.ToString()));
            }
            else
            {
                targ.PlaySound(0x1ED);
                targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                m_Table.Remove(targ);

                for (int i = 0; i < mods.Length; ++i)
                    targ.RemoveResistanceMod(mods[i]);

                BuffInfo.RemoveBuff(targ, BuffIcon.ReactiveArmor);
            }
        }

        public static bool HasAnyDefensiveSpell(Mobile m)
        {
            return HasArmor(m) || Server.Spells.Second.ProtectionSpell.HasProtection(m) || Server.Spells.Fifth.MagicReflectSpell.HasReflect(m);
        }

        #region SA
        public static bool HasArmor(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }
        #endregion
    }
}