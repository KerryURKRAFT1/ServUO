using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
    public class ItemIdentification
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback(OnUse);
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

        public static bool TriggerSkill(Mobile from)
        {
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
            from.Target = new InternalTarget();

            return true;
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
            	if (targeted is Item)
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
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1076203); // Target out of range.	
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
                if (m_Owner.CheckTargetSkill(SkillName.ItemID, m_Targ, 0, 100))
                {
                    if (m_Targ is BaseWeapon)
                        ((BaseWeapon)m_Targ).Identified = true;
                    else if (m_Targ is BaseArmor)
                        ((BaseArmor)m_Targ).Identified = true;

                    if (!Core.AOS)
                        ((Item)m_Targ).OnSingleClick(m_Owner);

	                Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(m_Owner, m_Targ);
                }
                else
                {
                    m_Owner.SendLocalizedMessage(500353); // You are not certain...
                }
					
                SkillRegistry.Remove(m_Owner);
			}
		}
    }
}