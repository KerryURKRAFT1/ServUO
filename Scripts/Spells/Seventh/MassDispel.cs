using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Seventh
{
    public class MassDispelSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mass Dispel", "Vas An Ort",
            263,
            9002,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.BlackPearl,
            Reagent.SulfurousAsh);

        public MassDispelSpell(Mobile caster, Item scroll)
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

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!this.Caster.CanSee(p))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                ArrayList targets = new ArrayList();

                Map map = this.Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 8);

                    foreach (Mobile m in eable)
                    {
                        if (m.BodyMod != 0) // Controlla se è sotto Polymorph
                        {
                            targets.Add(m); // Aggiungere il bersaglio per Polymorph
                        }
                        else if (m is BaseCreature)
                        {
                            BaseCreature bc = (BaseCreature)m;

                            if (bc.IsDispellable && (bc.SummonMaster == this.Caster || this.Caster.CanBeHarmful(m, false)))
                                targets.Add(m);
                        }
                    }

                    eable.Free();
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = (Mobile)targets[i];

                    if (m.BodyMod != 0) // Controlla se il bersaglio è sotto Polymorph
                    {
                        SpellHelper.Turn(this.Caster, m);

                        // Rimuove l'effetto Polymorph
                        PolymorphSpell.EndPolymorph(m);
                        Effects.PlaySound(m.Location, m.Map, 0x201); // Suono per Dispel riuscito
                        this.Caster.SendMessage("You dispel the polymorph effect!");
                    }
                    else if (m is BaseCreature)
                    {
                        BaseCreature bc = (BaseCreature)m;

                        double dispelChance = (50.0 + ((100 * (this.Caster.Skills.Magery.Value - bc.GetDispelDifficulty())) / (bc.DispelFocus * 2))) / 100;

                        // Skill Masteries
                        dispelChance -= ((double)SkillMasteries.MasteryInfo.EnchantedSummoningBonus(bc) / 100);

                        if (dispelChance > Utility.RandomDouble())
                        {
                            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                            Effects.PlaySound(m, m.Map, 0x201); // Suono per Dispel riuscito

                            m.Delete();
                        }
                        else
                        {
                            this.Caster.DoHarmful(m);
                            m.FixedEffect(0x3779, 10, 20);
                        }
                    }
                }
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly MassDispelSpell m_Owner;

            public InternalTarget(MassDispelSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    this.m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}