using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells.Seventh; // Per accedere a PolymorphSpell.EndPolymorph()

namespace Server.Spells.Sixth
{
    public class DispelSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Dispel", "An Ort",
            218,
            9002,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);

        public DispelSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
            }
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public class InternalTarget : Target
        {
            private readonly DispelSpell m_Owner;

            public InternalTarget(DispelSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    Mobile m = (Mobile)o;
                    BaseCreature bc = m as BaseCreature;

                    if (!from.CanSee(m))
                    {
                        from.SendLocalizedMessage(500237); // Target can not be seen.
                    }
                    else if (m.BodyMod != 0) // Controlla se il bersaglio è sotto Polymorph
                    {
                        SpellHelper.Turn(from, m);

                        // Rimuove l'effetto Polymorph
                        PolymorphSpell.EndPolymorph(m);
                        Effects.PlaySound(m, m.Map, 0x201);
                        from.SendMessage("You dispel the polymorph effect!");

                    }
                    else if (bc == null || !bc.IsDispellable)
                    {
                        from.SendLocalizedMessage(1005049); // That cannot be dispelled.
                    }
                    else if (bc.SummonMaster == from || m_Owner.CheckHSequence(m))
                    {
                        SpellHelper.Turn(from, m);

                        double dispelChance = (50.0 + ((100 * (from.Skills.Magery.Value - bc.GetDispelDifficulty())) / (bc.DispelFocus * 2))) / 100;

                        // Skill Masteries
                        dispelChance -= ((double)SkillMasteries.MasteryInfo.EnchantedSummoningBonus(bc) / 100);

                        if (dispelChance > Utility.RandomDouble())
                        {
                            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                            Effects.PlaySound(m, m.Map, 0x201);

                            m.Delete();
                        }
                        else
                        {
                            m.FixedEffect(0x3779, 10, 20);
                            from.SendLocalizedMessage(1010084); // The creature resisted the attempt to dispel it!
                        }
                    }
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}