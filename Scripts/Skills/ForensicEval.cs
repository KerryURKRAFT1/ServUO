using System;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.SkillHandlers
{
    public class ForensicEvaluation
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Forensics].Callback = new SkillUseCallback(OnUse);
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
            m.Target = new ForensicTarget();
            
            m.RevealingAction();

            m.SendLocalizedMessage(501000); // Show me the crime.

            return true;
        }

        public class ForensicTarget : Target
        {
            public ForensicTarget()
                : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                new SkillTimer(from, target, SkillRegistry.Delay).Start();
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
                if (m_Targ is Mobile)
                {
                    if (m_Owner.CheckTargetSkill(SkillName.Forensics, m_Targ, 40.0, 100.0))
                    {
                        if (m_Targ is PlayerMobile && ((PlayerMobile)m_Targ).NpcGuild == NpcGuild.ThievesGuild)
                            m_Owner.SendLocalizedMessage(501004);//That individual is a thief!
                        else
                            m_Owner.SendLocalizedMessage(501003);//You notice nothing unusual.
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(501001);//You cannot determain anything useful.
                    }
                }
                else if (m_Targ is Corpse)
                {
                    if (m_Owner.CheckTargetSkill(SkillName.Forensics, m_Targ, 0.0, 100.0))
                    {
                        Corpse c = (Corpse)m_Targ;

                        if (c.m_Forensicist != null)
                            m_Owner.SendLocalizedMessage(1042750, c.m_Forensicist) ; // The forensicist  ~1_NAME~ has already discovered that:
                        else
                            c.m_Forensicist = m_Owner.Name;

                        if (((Body)c.Amount).IsHuman)
                            m_Owner.SendLocalizedMessage(1042751, (c.Killer == null ? "no one" : c.Killer.Name));//This person was killed by ~1_KILLER_NAME~

                        if (c.Looters.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < c.Looters.Count; i++)
                            {
                                if (i > 0)
                                    sb.Append(", ");
                                sb.Append(((Mobile)c.Looters[i]).Name);
                            }

                            m_Owner.SendLocalizedMessage(1042752, sb.ToString());//This body has been distrubed by ~1_PLAYER_NAMES~
                        }
                        else
                        {
                            m_Owner.SendLocalizedMessage(501002);//The corpse has not be desecrated.
                        }
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(501001);//You cannot determain anything useful.
                    }
                }
                else if (m_Targ is ILockpickable)
                {
                    ILockpickable p = (ILockpickable)m_Targ;
                    if (p.Picker != null)
                        m_Owner.SendLocalizedMessage(1042749, p.Picker.Name);//This lock was opened by ~1_PICKER_NAME~
                    else
                        m_Owner.SendLocalizedMessage(501003);//You notice nothing unusual.
                }
                else if (m_Targ is Item)
                {
                    Item item = (Item)m_Targ;

                    if (item.HonestyItem && item.HonestyOwner != null)
                    {
                        string region = item.HonestyRegion == null ? "an unknown place" : item.HonestyRegion;

                        if (m_Owner.Skills.Forensics.Value >= 65)
                        {
                            m_Owner.SendLocalizedMessage(1151521, String.Format("{0}\t{1}", item.HonestyOwner.Name, region)); // This item belongs to ~1_val~ who lives in ~2_val~.
                        }
                        else if (m_Owner.Skills.Forensics.Value >= 40)
                        {
                            m_Owner.SendLocalizedMessage(1151522, region); // You find seeds from a familiar plant stuck to the item which suggests that this item is from ~1_val~.
                        }
                    }
                }

				SkillRegistry.Remove(m_Owner);
			}
		}
    }
}