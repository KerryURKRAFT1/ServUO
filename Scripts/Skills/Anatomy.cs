using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class Anatomy
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Anatomy].Callback = new SkillUseCallback(OnUse);
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
            m.Target = new Anatomy.InternalTarget();

            m.SendLocalizedMessage(500321); // Whom shall I examine?

            return true;
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile m, object targeted)
            {
                if (m == targeted && m.AccessLevel == AccessLevel.Player)
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500324); // You know yourself quite well enough already.
                }
                else if (targeted is TownCrier)
                {
                    ((TownCrier)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500322, m.NetState); // This person looks fine to me, though he may have some news...
                }
                else if (targeted is BaseVendor && ((BaseVendor)targeted).IsInvulnerable)
                {
                    ((BaseVendor)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500326, m.NetState); // That can not be inspected.
                }
                else if (targeted is Item)
                {
                    ((Item)targeted).SendLocalizedMessageTo(m, 500323, ""); // Only living things have anatomies!
                }
                else if (targeted is Mobile)
                {
                	Mobile targ = (Mobile)targeted;

		            new SkillTimer(m, targ, SkillRegistry.Delay).Start();
		            
		            return;
                }
                
                SkillRegistry.Remove(m);
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
			private readonly Mobile m_Targ;

            public SkillTimer(Mobile owner, Mobile targ, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				int str=m_Targ.Str, dex=m_Targ.Dex, stm=m_Targ.Stam;
				
				try
				{
					int marginOfError = Math.Max(0, 25 - (int)(m_Owner.Skills[SkillName.Anatomy].Value / 4));
	
	                str = m_Targ.Str + Utility.RandomMinMax(-marginOfError, +marginOfError);
	                dex = m_Targ.Dex + Utility.RandomMinMax(-marginOfError, +marginOfError);
	                stm = ((m_Targ.Stam * 100) / Math.Max(m_Targ.StamMax, 1)) + Utility.RandomMinMax(-marginOfError, +marginOfError);
				}
				catch{}

                int strMod = str / 10;
                int dexMod = dex / 10;
                int stmMod = stm / 10;

                if (strMod < 0)
                    strMod = 0;
                else if (strMod > 10)
                    strMod = 10;

                if (dexMod < 0)
                    dexMod = 0;
                else if (dexMod > 10)
                    dexMod = 10;

                if (stmMod > 10)
                    stmMod = 10;
                else if (stmMod < 0)
                    stmMod = 0;

                if (m_Owner.CheckTargetSkill(SkillName.Anatomy, m_Targ, 0, 100))
                {
                    m_Targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038045 + (strMod * 11) + dexMod, m_Owner.NetState); // That looks [strong] and [dexterous].

                    if (m_Owner.Skills[SkillName.Anatomy].Base >= 65.0)
                    {
                    	m_Targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038303 + stmMod, m_Owner.NetState); // That being is at [10,20,...] percent endurance.
                    }
                }
                else
                {
                    m_Targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042666, m_Owner.NetState); // You can not quite get a sense of their physical characteristics.
                }

                SkillRegistry.Remove(m_Owner);                    
			}
		}
    }
}