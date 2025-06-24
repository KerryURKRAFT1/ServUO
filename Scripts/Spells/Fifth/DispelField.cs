using System;
using Server.Items;
using Server.Misc;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
    public class DispelFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Dispel Field", "An Grav",
            206,
            9002,
            Reagent.BlackPearl,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh,
            Reagent.Garlic);
        public DispelFieldSpell(Mobile caster, Item scroll)
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
        	Target ((Item)ObjectTargeted);
        }

        public void Target(Item item)
        {
            Type t = item.GetType();

            if (!this.Caster.CanSee(item))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (!t.IsDefined(typeof(DispellableFieldAttribute), false))
            {
                this.Caster.SendLocalizedMessage(1005049); // That cannot be dispelled.
            }
            else if (item is Moongate && !((Moongate)item).Dispellable)
            {
                this.Caster.SendLocalizedMessage(1005047); // That magic is too chaotic
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, item);

                Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 20, 5042);
                Effects.PlaySound(item.GetWorldLocation(), item.Map, 0x201);

                item.Delete();
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly DispelFieldSpell m_Owner;
            public InternalTarget(DispelFieldSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Item)
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