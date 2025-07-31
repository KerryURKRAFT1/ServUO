using System;
using Server.Items;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	class Meditation
    {
        public static void Initialize()
        {
            SkillInfo.Table[46].Callback = new SkillUseCallback(OnUse);
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
            m.RevealingAction();

			double chance = (m.Skills[SkillName.Meditation].Base - 5.0) / 100;
					
			if (m.Target != null)
            {
                m.SendLocalizedMessage(501845); // You are busy doing something else and cannot focus.
            }
            else if (m.Mana >= m.ManaMax)
            {
                m.SendLocalizedMessage(501846); // You are at peace.
            }
			else if (m.Spell != null && m.Spell.IsCasting)
        	{
				m.SendLocalizedMessage(502642); // You are already casting a spell.
			}
            else if (m.Hits < (m.HitsMax / 10)) // Less than 10% health
            {
                m.SendLocalizedMessage(501849); // The mind is strong but the body is weak.
            }
            else if (chance > Utility.RandomDouble())
            {
	            m.CheckSkill(SkillName.Meditation, 0.0, 100.0);
	
	            m.SendLocalizedMessage(501851); // You enter a meditative trance.
	            m.Meditating = true;
	            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.ActiveMeditation, 1075657));
	
	            if (m.Player || m.Body.IsHuman)
	                m.PlaySound(0xF9);
		
	            new SkillTimer(m, TimeSpan.FromSeconds(75)).Start();

                return true;
            }
            else
            {
				m.SendLocalizedMessage(501850); // You cannot focus your concentration.
            }

            return false;
        }

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly long m_EndTime;

			public SkillTimer(Mobile owner, TimeSpan delay) : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0) )
			{
				m_Owner = owner;
				m_EndTime = Core.TickCount + (int)delay.TotalMilliseconds;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if (m_EndTime <= Core.TickCount || !m_Owner.Meditating)
                {
					if (m_Owner.Meditating)
					{
						m_Owner.SendLocalizedMessage(500134); // You stop meditating.
					}
					
                    SkillRegistry.Remove(m_Owner);

					Stop();
                }
			}
		}
    }
}