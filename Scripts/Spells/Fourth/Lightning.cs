using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fourth
{
    public class LightningSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Lightning", "Por Ort Grav",
            239,
            9021,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        
        public LightningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
            }
        }
        
        public override bool DelayedDamage
        {
            get
            {
                return false;
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
            Target((IDamageable)ObjectTargeted);
        }
        
        public void Target(IDamageable m)
        {
            Mobile mob = m as Mobile;

            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
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
                    damage = this.GetNewAosDamage(23, 1, 4, m);
                }
                else if (Core.UOR)
                {
                    damage = Utility.Random(20, 5); // 20-24
                    
                    // Effetti visivi PRIMA del check reflect
                    Effects.SendBoltEffect(m, true, 0);
                    
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
                    damage = Utility.Random(12, 9);

                    if (this.CheckResisted(mob))
                    {
                        damage *= 0.75;
                    }

                    damage *= this.GetDamageScalar(mob);
                }

                // Effetti visivi per AOS/Old (NON UOR)
                if (!Core.UOR)
                {
                    Effects.SendBoltEffect(m, true, 0);
                }

                // Danno
                if (damage > 0)
                {
                    SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly LightningSpell m_Owner;
            
            public InternalTarget(LightningSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
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