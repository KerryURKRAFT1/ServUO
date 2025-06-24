using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
    public class ProtectionSpell : MagerySpell
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Protection", "Uus Sanct",
            236,
            9011,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.SulfurousAsh);
        private static readonly Hashtable m_Table = new Hashtable();

        public ProtectionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public static Hashtable Registry
        {
            get { return m_Registry; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public static void Toggle(Mobile caster, Mobile target, bool archprotection)
        {
            // BLOCCO: se il target ha già una spell difensiva attiva, non applicare la spell e mostra il messaggio
            if (HasAnyDefensiveSpell(target))
            {
                if (caster != null)
                    caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                return;
            }

            object[] mods = (object[])m_Table[target];

            if (mods == null)
            {
                target.PlaySound(0x1E9);
                target.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);

                mods = new object[2]
                {
                    new ResistanceMod(ResistanceType.Physical, -15 + Math.Min((int)(caster.Skills[SkillName.Inscribe].Value / 20), 15)),
                    new DefaultSkillMod(SkillName.MagicResist, true, -35 + Math.Min((int)(caster.Skills[SkillName.Inscribe].Value / 20), 35))
                };

                m_Table[target] = mods;
                Registry[target] = 100.0;

                target.AddResistanceMod((ResistanceMod)mods[0]);
                target.AddSkillMod((SkillMod)mods[1]);

                int physloss = -15 + (int)(caster.Skills[SkillName.Inscribe].Value / 20);
                int resistloss = -35 + (int)(caster.Skills[SkillName.Inscribe].Value / 20);
                string args = String.Format("{0}\t{1}", physloss, resistloss);
                BuffInfo.AddBuff(target, new BuffInfo(archprotection ? BuffIcon.ArchProtection : BuffIcon.Protection, archprotection ? 1075816 : 1075814, 1075815, args.ToString()));
            }
            else
            {
                target.PlaySound(0x1ED);
                target.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);

                m_Table.Remove(target);
                Registry.Remove(target);

                target.RemoveResistanceMod((ResistanceMod)mods[0]);
                target.RemoveSkillMod((SkillMod)mods[1]);

                BuffInfo.RemoveBuff(target, BuffIcon.Protection);
                BuffInfo.RemoveBuff(target, BuffIcon.ArchProtection);
            }
        }

        public static void EndProtection(Mobile m)
        {
            if (m_Table.Contains(m))
            {
                object[] mods = (object[])m_Table[m];

                m_Table.Remove(m);
                Registry.Remove(m);

                m.RemoveResistanceMod((ResistanceMod)mods[0]);
                m.RemoveSkillMod((SkillMod)mods[1]);

                BuffInfo.RemoveBuff(m, BuffIcon.Protection);
                BuffInfo.RemoveBuff(m, BuffIcon.ArchProtection);
            }
        }

        public override bool CheckCast()
        {
            // Solo la restrizione classica per self-cast (non targeting)
            if (Core.AOS || Core.UOR)
                return true;

            if (m_Registry.ContainsKey(this.Caster))
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
            if (m_Registry.ContainsKey(this.Caster))
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
                    double value = (int)(this.Caster.Skills[SkillName.EvalInt].Value + this.Caster.Skills[SkillName.Meditation].Value + this.Caster.Skills[SkillName.Inscribe].Value);
                    value /= 4;

                    if (value < 0)
                        value = 0;
                    else if (value > 75)
                        value = 75.0;

                    Registry.Add(this.Caster, value);
                    new InternalTimer(this.Caster).Start();

                    this.Caster.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);
                    this.Caster.PlaySound(0x1ED);
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
            private ProtectionSpell m_Owner;

            public InternalTarget(ProtectionSpell owner)
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

        public static bool HasAnyDefensiveSpell(Mobile m)
        {
            return HasProtection(m) || Server.Spells.First.ReactiveArmorSpell.HasArmor(m) || Server.Spells.Fifth.MagicReflectSpell.HasReflect(m);
        }

        #region SA
        public static bool HasProtection(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }
        #endregion

        private class InternalTimer : Timer
        {
            private Mobile m_Caster;
            public InternalTimer(Mobile caster)
                : base(TimeSpan.FromSeconds(0))
            {
                double val = caster.Skills[SkillName.Magery].Value * 2.0;
                if (val < 15)
                    val = 15;
                else if (val > 240)
                    val = 240;

                this.m_Caster = caster;
                this.Delay = TimeSpan.FromSeconds(val);
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                ProtectionSpell.Registry.Remove(this.m_Caster);
                DefensiveSpell.Nullify(this.m_Caster);
            }
        }
    }
}