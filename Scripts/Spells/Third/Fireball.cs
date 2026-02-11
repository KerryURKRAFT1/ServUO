using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Third
{
    public class FireballSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Fireball", "Vas Flam",
            203,
            9041,
            Reagent.BlackPearl);
        
        public FireballSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
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
                    damage = this.GetNewAosDamage(19, 1, 5, m);
                }
                else if (Core.UOR)
                {
                    damage = Utility.Random(10, 6); // 10-6
                    
                    // Effetti visivi e sonori PRIMA del check reflect
                    if (mob != null)
                    {
                        this.Caster.MovingParticles(mob, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                        this.Caster.PlaySound(0x44B);
                    }
                    else
                    {
                        this.Caster.MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                        this.Caster.PlaySound(0x44B);
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
                    damage = Utility.Random(10, 7);

                    if (this.CheckResisted(mob))
                    {
                        damage *= 0.75;
                    }

                    damage *= this.GetDamageScalar(mob);
                }

                // Effetti visivi per AOS/Old (NON UOR)
                if (!Core.UOR)
                {
                    if (mob != null)
                    {
                        this.Caster.MovingParticles(mob, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                        this.Caster.PlaySound(Core.AOS ? 0x15E : 0x44B);
                    }
                    else
                    {
                        this.Caster.MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                        this.Caster.PlaySound(Core.AOS ? 0x15E : 0x44B);
                    }
                }

                // Danno
                if (damage > 0)
                {
                    SpellHelper.Damage(this, m, damage, 0, 100, 0, 0, 0);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FireballSpell m_Owner;
            
            public InternalTarget(FireballSpell owner)
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