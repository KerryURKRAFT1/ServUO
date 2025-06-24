using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Sixth
{
    public class RevealSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Reveal", "Wis Quas",
            206,
            9002,
            Reagent.Bloodmoss,
            Reagent.SulfurousAsh);
        public RevealSpell(Mobile caster, Item scroll)
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
        	Target ((IPoint3D)ObjectTargeted);
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

                List<Mobile> targets = new List<Mobile>();

                Map map = this.Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 1 + (int)(this.Caster.Skills[SkillName.Magery].Value / 20.0));

                    foreach (Mobile m in eable)
                    {
                        if (m is Mobiles.ShadowKnight && (m.X != p.X || m.Y != p.Y))
                            continue;

                        if (m.Hidden && (m.IsPlayer() || this.Caster.AccessLevel > m.AccessLevel) && CheckDifficulty(this.Caster, m))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    m.RevealingAction();

                    m.FixedParticles(0x375A, 9, 20, 5049, EffectLayer.Head);
                    m.PlaySound(0x1FD);
                }
            }

            this.FinishSequence();
        }

        // Reveal uses magery and detect hidden vs. hide and stealth 
        private static bool CheckDifficulty(Mobile from, Mobile m)
        {
            // Reveal always reveals vs. invisibility spell 
            if (!Core.AOS || InvisibilitySpell.HasTimer(m))
                return true;

            int magery = from.Skills[SkillName.Magery].Fixed;
            int detectHidden = from.Skills[SkillName.DetectHidden].Fixed;

            int hiding = m.Skills[SkillName.Hiding].Fixed;
            int stealth = m.Skills[SkillName.Stealth].Fixed;
            int divisor = hiding + stealth;

            int chance;
            if (divisor > 0)
                chance = 50 * (magery + detectHidden) / divisor;
            else
                chance = 100;

            return chance > Utility.Random(100);
        }

        public class InternalTarget : Target
        {
            private readonly RevealSpell m_Owner;
            public InternalTarget(RevealSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
            	if (o is IPoint3D)
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