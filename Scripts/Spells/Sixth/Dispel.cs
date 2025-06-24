using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells.Seventh; // Per accedere a PolymorphSpell.EndPolymorph()
using Server.Network;

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
            BaseCreature bc = m as BaseCreature;

            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (m.BodyMod != 0) // Controlla se il bersaglio è sotto Polymorph
            {
                SpellHelper.Turn(this.Caster, m);

                // Rimuove l'effetto Polymorph
                PolymorphSpell.EndPolymorph(m);
                Effects.PlaySound(m, m.Map, 0x201);
                this.Caster.SendMessage("You dispel the polymorph effect!");

            }
            else if (bc == null || !bc.IsDispellable)
            {
                this.Caster.SendLocalizedMessage(1005049); // That cannot be dispelled.
            }
            else if (bc.SummonMaster == this.Caster || this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                double dispelChance = (50.0 + ((100 * (this.Caster.Skills.Magery.Value - bc.GetDispelDifficulty())) / (bc.DispelFocus * 2))) / 100;

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
                    this.Caster.SendLocalizedMessage(1010084); // The creature resisted the attempt to dispel it!
                }
            }

            this.FinishSequence();
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
    }
}