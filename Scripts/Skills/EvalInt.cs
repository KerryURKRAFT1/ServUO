using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class EvalInt
    {
        public static void Initialize()
        {
            SkillInfo.Table[16].Callback = new SkillUseCallback(OnUse);
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
            m.Target = new EvalInt.InternalTarget();

            m.SendLocalizedMessage(500906); // What do you wish to evaluate?

            return true;
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == targeted && from.AccessLevel == AccessLevel.Player)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500910); // Hmm, that person looks really silly.
                }
                else if (targeted is TownCrier)
                {
                    ((TownCrier)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500907, from.NetState); // He looks smart enough to remember the news.  Ask him about it.
                }
                else if (targeted is BaseVendor && ((BaseVendor)targeted).IsInvulnerable)
                {
                    ((BaseVendor)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500909, from.NetState); // That person could probably calculate the cost of what you buy from them.
                }
                else if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

		            new SkillTimer(from, targ, SkillRegistry.Delay).Start();
					
		            return;                    
                }
                else if (targeted is Item)
                {
                    ((Item)targeted).SendLocalizedMessageTo(from, 500908, ""); // It looks smarter than a rock, but dumber than a piece of wood.
                }

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
			private readonly Mobile m_Targ;
			private string m_Msg;

            public SkillTimer(Mobile owner, Mobile targ, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                int i = m_Targ.Int;

                if (i < 11)
                    m_Msg = "slightly less intelligent than a rock";
                else if (i < 21 && i > 10)
                    m_Msg = "fairly stupid";
                else if (i < 31 && i > 20)
                    m_Msg = "not the brightest";
                else if (i < 41 && i > 30)
                    m_Msg = "about average";
                else if (i < 51 && i > 40)
                    m_Msg = "moderately intelligent";
                else if (i < 61 && i > 50)
                    m_Msg = "very intelligent";
                else if (i < 71 && i > 60)
                    m_Msg = "extraordinarily intelligent";
                else if (i < 81 && i > 70)
                    m_Msg = "like a formidable intellect, well beyond the ordinary";
                else if (i < 91 && i > 80)
                    m_Msg = "like a definite genius";
                else
                    m_Msg = "superhumanly intelligent in a manner you cannot comprehend";

                int body;

                if (m_Targ.Body.IsHuman)
                    body = m_Targ.Female ? 11 : 0;
                else
                    body = 22;

                if (m_Owner.CheckTargetSkill(SkillName.EvalInt, m_Targ, 0.0, 120.0))
                {
                    m_Owner.SendMessage($"{m_Targ.Name} looks {m_Msg}.");
                }
                else 
                {
                    m_Targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038166 + (body / 11), m_Owner.NetState); // You cannot judge his/her/its mental abilities.
                }

                SkillRegistry.Remove(m_Owner);
			}
		}
    }
}