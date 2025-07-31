using System;
using Server.Multis;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Hiding
    {
        private static bool m_CombatOverride;
        
        public static bool CombatOverride
        {
            get
            {
                return m_CombatOverride;
            }
            set
            {
                m_CombatOverride = value;
            }
        }

        public static void Initialize()
        {
            SkillInfo.Table[21].Callback = new SkillUseCallback(OnUse);
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
        	if (m.Spell == null)
            {
		        TimeSpan delay = SkillRegistry.Delay;
	
	            if (Server.Engines.VvV.ManaSpike.UnderEffects(m))
	            {
		            delay = TimeSpan.FromMilliseconds(SkillRegistry.Delay.TotalMilliseconds * 0.75);
	            }
	
                m.SendMessage("You attempt to hide.");
                
	            new SkillTimer(m, delay).Start();
	
	            if (Core.ML && m.Target != null)
	            {
	                Targeting.Target.Cancel(m);
	            }
						
	            return true;
	        }
            else
            {
                m.SendLocalizedMessage(501238); // You are busy doing something else and cannot hide.

                SkillRegistry.Remove(m);

				return  false;
            }
        }
 
        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;

            public SkillTimer(Mobile owner, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;

				Priority = TimerPriority.TwoFiftyMS;

                m_Owner.RevealingAction();
            }

			protected override void OnTick()
			{
	            double bonus = 0.0;
	
	            BaseHouse house = BaseHouse.FindHouseAt(m_Owner);
	
	            if (house != null && house.IsFriend(m_Owner))
	            {
	                bonus = 100.0;
	            }
	            else if (!Core.AOS)
	            {
	                if (house == null)
	                    house = BaseHouse.FindHouseAt(new Point3D(m_Owner.X - 1, m_Owner.Y, 127), m_Owner.Map, 16);
	
	                if (house == null)
	                    house = BaseHouse.FindHouseAt(new Point3D(m_Owner.X + 1, m_Owner.Y, 127), m_Owner.Map, 16);
	
	                if (house == null)
	                    house = BaseHouse.FindHouseAt(new Point3D(m_Owner.X, m_Owner.Y - 1, 127), m_Owner.Map, 16);
	
	                if (house == null)
	                    house = BaseHouse.FindHouseAt(new Point3D(m_Owner.X, m_Owner.Y + 1, 127), m_Owner.Map, 16);
	
	                if (house != null)
	                    bonus = 50.0;
	            }
	
	            int range = Math.Min((int)((100 - m_Owner.Skills[SkillName.Hiding].Value) / 2) + 8, 18);	//Cap of 18 not OSI-exact, intentional difference
	
	            bool badCombat = (!m_CombatOverride && m_Owner.Combatant is Mobile && m_Owner.InRange(m_Owner.Combatant.Location, range) && ((Mobile)m_Owner.Combatant).InLOS(m_Owner.Combatant));
	           
	            bool ok = (!badCombat);
	
	            if (ok)
	            {
	                if (!m_CombatOverride)
	                {
	                    foreach (Mobile check in m_Owner.GetMobilesInRange(range))
	                    {
	                        if (check.InLOS(m_Owner) && check.Combatant == m_Owner)
	                        {
	                            badCombat = true;
	                            ok = false;
	                            break;
	                        }
	                    }
	                }
	
	                ok = (!badCombat && m_Owner.CheckSkill(SkillName.Hiding, 0.0 - bonus, 100.0 - bonus));
	            }
	
	            if (badCombat)
	            {
	                m_Owner.RevealingAction();
	
	                m_Owner.LocalOverheadMessage(MessageType.Regular, 0x22, 501237); // You can't seem to hide right now.
	            }
	            else 
	            {
	                if (ok)
	                {
	                    m_Owner.Hidden = true;
	                    m_Owner.Warmode = false;
						Server.Spells.Sixth.InvisibilitySpell.RemoveTimer(m_Owner);
	                    Server.Items.InvisibilityPotion.RemoveTimer(m_Owner);
	                    m_Owner.LocalOverheadMessage(MessageType.Regular, 0x1F4, 501240); // You have hidden yourself well.

		                Server.SkillHandlers.Stealth.Start(m_Owner);
	                }
	                else
	                {
	                    m_Owner.RevealingAction();
	
	                    m_Owner.LocalOverheadMessage(MessageType.Regular, 0x22, 501241); // You can't seem to hide here.
	                }
	            }

				SkillRegistry.Remove(m_Owner);
			}
		}
    }
}