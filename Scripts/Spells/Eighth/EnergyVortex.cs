using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Eighth
{
    public class EnergyVortexSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Energy Vortex", "Vas Corp Por",
            260,
            9032,
            false,
            Reagent.Bloodmoss,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.Nightshade);
        public EnergyVortexSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
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
                    duration = TimeSpan.FromSeconds(90.0);
                else
                    duration = TimeSpan.FromSeconds(Utility.Random(80, 40));

                BaseCreature.Summon(new EnergyVortex(), false, this.Caster, new Point3D(p), 0x212, duration);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private EnergyVortexSpell m_Owner;
            public InternalTarget(EnergyVortexSpell owner)
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