using System;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class UnlockSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Unlock Spell", "Ex Por",
            215,
            9001,
            Reagent.Bloodmoss,
            Reagent.SulfurousAsh);
        public UnlockSpell(Mobile caster, Item scroll)
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
        	Target ((LockableContainer)ObjectTargeted);
        }

        public void Target(LockableContainer cont)
        {
            IPoint3D loc = cont as IPoint3D;

            if (loc == null)
            {
            	return;
            }

            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, cont);

                if (Multis.BaseHouse.CheckSecured(cont)) 
                {
                	this.Caster.SendLocalizedMessage(503098); // You cannot cast this on a secure item.`
                }
                else if (!cont.Locked)
                {
                	this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101); // That did not need to be unlocked.
                }
                else if (cont.LockLevel == 0)
                {
                	this.Caster.SendLocalizedMessage(501666); // You can't unlock that!
                }
                else
                {
                    int level = (int)(this.Caster.Skills[SkillName.Magery].Value * 0.8) - 4;

                    if (level >= cont.RequiredSkill && !(cont is TreasureMapChest && ((TreasureMapChest)cont).Level > 2))
                    {
                        cont.Locked = false;

                        if (cont.LockLevel == -255)
                            cont.LockLevel = cont.RequiredSkill - 10;
                    }
                    else
                    {
                    	this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503099); // My spell does not seem to have an effect on that lock.
                    }

	                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc), this.Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5024);
	
	                Effects.PlaySound(loc, this.Caster.Map, 0x1FF);
                }
            }

            this.FinishSequence();
        }


        private class InternalTarget : Target
        {
            private readonly UnlockSpell m_Owner;
            public InternalTarget(UnlockSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is LockableContainer)
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