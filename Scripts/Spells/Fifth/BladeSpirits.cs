using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
    public class BladeSpiritsSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Blade Spirits", "In Jux Hur Ylem",
            266,
            9040,
            false,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.Nightshade);
        public BladeSpiritsSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fifth;
            }
        }

        public override TimeSpan GetCastDelay()
        {
            if (Core.AOS)
                return TimeSpan.FromTicks(base.GetCastDelay().Ticks * ((Core.SE) ? 3 : 5));

            return TimeSpan.FromSeconds(5.25);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((this.Caster.Followers + (Core.SE ? 2 : 1)) > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
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
        	Target ((IPoint3D)ObjectTargeted);
        }

        public void Target(IPoint3D p)
        {
            Map map = this.Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
            {
                TimeSpan duration;

                if (Core.AOS)
                    duration = TimeSpan.FromSeconds(120);
                else
                    duration = TimeSpan.FromSeconds(Utility.Random(80, 40));

                BaseCreature.Summon(new BladeSpirits(), false, this.Caster, new Point3D(p), 0x212, duration);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private BladeSpiritsSpell m_Owner;
            public InternalTarget(BladeSpiritsSpell owner)
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