using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using System.Collections.Generic;

namespace Server.Spells.Sixth
{
    public class ExplosionSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Explosion", "Vas Ort Flam",
            230,
            9041,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public ExplosionSpell(Mobile caster, Item scroll)
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

        private List<Mobile> Targets = new List<Mobile>();

        public override void OnCast()
        {
        	if (ObjectTargeted is BaseExplosionPotion)
        	{
        		Explode ((BaseExplosionPotion)ObjectTargeted);
        	}
        	else
        	{
        		Target ((Mobile)ObjectTargeted);
        	}
        }

        public void Target(Mobile defender) //changed to mobile and now has area damage also removed damage delay
        {
            if (CheckHSequence(defender))
            {
            	Targets.Add(defender);
            	
	            foreach (Mobile targ in this.Caster.Map.GetMobilesInRange(defender.Location, 2)) //2? maybe 3
	            {
	            	if (SpellHelper.ValidIndirectTarget(this.Caster, targ) && this.Caster.CanBeHarmful(targ, false))
	                {
	                    Targets.Add(targ);
	                }
	            }
	
                for (int i = 0; i < Targets.Count; ++i)
                {
                    Mobile target = Targets[i];

		            if (CheckHSequence(target))
		            {
	                    double damage = Utility.Random(23, 22);
	
	                    if (CheckResisted(target))
	                    {
	                        damage *= 0.75;
	
	                        target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
	                    }
	
	                    damage *= GetDamageScalar(target);
	
                        this.Caster.DoHarmful(target);
	                    Effects.SendLocationParticles(target, 0x36BD, 20, 10, 5044);
	                    Effects.PlaySound(target.Location, target.Map, 0x307);
		
		                if (damage > 0)
		                {
		                    SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);
		                }		
	                }	                
	            }

                Targets.Clear();
            }
            
            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly ExplosionSpell m_Owner;
            public InternalTarget(ExplosionSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile || o is BaseExplosionPotion)
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
