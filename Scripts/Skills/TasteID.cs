using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.SkillHandlers
{
    public class TasteID
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.TasteID].Callback = new SkillUseCallback(OnUse);
        }

    	public static TimeSpan OnUse(Mobile m)
        {
    		if (!SkillRegistry.Contains(m))
    		{
	    		SkillRegistry.Add(m);
	        	
	        	if (!TriggerSkill(m))
	        	{
		    		SkillRegistry.Remove(m);
	        	}
    		}
    		else if (SkillRegistry.WaitMsg)
    		{
                m.SendMessage("You must wait to perform another action.");
    		}
        	
        	return TimeSpan.Zero;
        }

        public static bool TriggerSkill(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(502807); // What would you like to taste?

            return true;
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(2, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
            	if (targeted is Food || targeted is BasePotion || targeted is PotionKeg)
            	{				
            		new SkillTimer(from, targeted, SkillRegistry.Delay).Start();
            		
            		return;
            	}

                from.SendLocalizedMessage(1046439); // That is not a valid target.

                SkillRegistry.Remove(from);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                SkillRegistry.Remove(from);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(502815); // You are too far away to taste that.
				SkillRegistry.Remove(from);
            }

	        protected override void OnTargetOutOfLOS(Mobile from, object o)
	        {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237);// Target can not be seen.
                SkillRegistry.Remove(from);
	        }
        }

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly object m_Targ;

			public SkillTimer(Mobile owner, object targ, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                if (m_Targ is Food)
                {
                    Food food = (Food)m_Targ;

                    if (m_Owner.CheckTargetSkill(SkillName.TasteID, food, 0, 100))
                    {
                        if (food.Poison != null)
                        {
                            food.SendLocalizedMessageTo(m_Owner, 1038284); // It appears to have poison smeared on it.
                        }
                        else
                        {
                            // No poison on the food
                            food.SendLocalizedMessageTo(m_Owner, 1010600); // You detect nothing unusual about this substance.
                        }
                    }
                    else
                    {
                        // Skill check failed
                        food.SendLocalizedMessageTo(m_Owner, 502823); // You cannot discern anything about this substance.
                    }
                }
                else if (m_Targ is BasePotion)
                {
                    BasePotion potion = (BasePotion)m_Targ;

                    potion.SendLocalizedMessageTo(m_Owner, 502813); // You already know what kind of potion that is.
                    potion.SendLocalizedMessageTo(m_Owner, potion.LabelNumber);
                }
                else if (m_Targ is PotionKeg)
                {
                    PotionKeg keg = (PotionKeg)m_Targ;

                    if (keg.Held <= 0)
                    {
                        keg.SendLocalizedMessageTo(m_Owner, 502228); // There is nothing in the keg to taste!
                    }
                    else
                    {
                        keg.SendLocalizedMessageTo(m_Owner, 502229); // You are already familiar with this keg's contents.
                        keg.SendLocalizedMessageTo(m_Owner, keg.LabelNumber);
                    }
                }
                else
                {
                    // The target is not food or potion or potion keg.
                    m_Owner.SendLocalizedMessage(502820); // That's not something you can taste.
                }
					
                SkillRegistry.Remove(m_Owner);
			}
		}
    }
}