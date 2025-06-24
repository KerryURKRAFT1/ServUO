using System;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Third
{
    public class TelekinesisSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Telekinesis", "Ort Por Ylem",
            203,
            9031,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public TelekinesisSpell(Mobile caster, Item scroll)
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
        	if (ObjectTargeted is ITelekinesisable)
        	{
        		Target ((ITelekinesisable)ObjectTargeted);
        	}
			else
			{
        		Target ((Container)ObjectTargeted);
			}
        }

        public void Target(ITelekinesisable obj)
        {
            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, obj);

                obj.OnTelekinesis(this.Caster);
            }

            this.FinishSequence();
        }

        public void Target(Container item)
        {
            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, item);

                object root = item.RootParent;

                if (!item.IsAccessibleTo(this.Caster))
                {
                    item.OnDoubleClickNotAccessible(this.Caster);
                }
                else if (!item.CheckItemUse(this.Caster, item))
                {
                }
                else if (root != null && root is Mobile && root != this.Caster)
                {
                    item.OnSnoop(this.Caster);
                }
                else if (item is Corpse && !((Corpse)item).CheckLoot(this.Caster, null))
                {
                }
                else if (this.Caster.Region.OnDoubleClick(this.Caster, item))
                {
                    Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
                    Effects.PlaySound(item.Location, item.Map, 0x1F5);

                    item.OnItemUsed(this.Caster, item);
                }
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly TelekinesisSpell m_Owner;
            public InternalTarget(TelekinesisSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is ITelekinesisable || o is Container)
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

namespace Server
{
    public interface ITelekinesisable : IPoint3D
    {
        void OnTelekinesis(Mobile from);
    }
}