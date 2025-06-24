using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Third
{
    public class BlessSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Bless", "Rel Sanct",
            203,
            9061,
            Reagent.Garlic,
            Reagent.MandrakeRoot);
        public BlessSpell(Mobile caster, Item scroll)
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
        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
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
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckBSequence(m))
            {

                SpellHelper.Turn(this.Caster, m);

                // UOR LOGIC
                if (Core.UOR)
                {
                    double magery = this.Caster.Skills[SkillName.Magery].Value;
                    double percent = magery * 0.003; // 0,3% per punto magery (100 magery = 30%)

                    int strBonus = (int)(m.RawStr * percent);
                    int dexBonus = (int)(m.RawDex * percent);
                    int intBonus = (int)(m.RawInt * percent);

                    TimeSpan length = SpellHelper.GetDuration(this.Caster, m);

                    m.AddStatMod(new StatMod(StatType.Str, "[Bless] Str", strBonus, length));
                    m.AddStatMod(new StatMod(StatType.Dex, "[Bless] Dex", dexBonus, length));
                    m.AddStatMod(new StatMod(StatType.Int, "[Bless] Int", intBonus, length));

                    int percentage = (int)(percent * 100);
                    string args = String.Format("{0}\t{1}\t{2}", percentage, percentage, percentage);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Bless, 1075847, 1075848, length, m, args.ToString()));

                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.PlaySound(0x1EA);

                    Timer.DelayCall(length, () => m.Delta(MobileDelta.Stat));
                }
                ////// OLD LOGIC
                else
                {

                    SpellHelper.AddStatBonus(this.Caster, m, StatType.Str);
                    SpellHelper.DisableSkillCheck = true;
                    SpellHelper.AddStatBonus(this.Caster, m, StatType.Dex);
                    SpellHelper.AddStatBonus(this.Caster, m, StatType.Int);
                    SpellHelper.DisableSkillCheck = false;
                    
                    int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, false) * 100);
                    TimeSpan length = SpellHelper.GetDuration(this.Caster, m);
                    string args = String.Format("{0}\t{1}\t{2}", percentage, percentage, percentage);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Bless, 1075847, 1075848, length, m, args.ToString()));

                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.PlaySound(0x1EA);

                    Timer.DelayCall(length, () => m.Delta(MobileDelta.Stat));
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly BlessSpell m_Owner;
            public InternalTarget(BlessSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
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