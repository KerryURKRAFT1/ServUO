using System;
using Server.Factions;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.VvV;
using Server.Guilds;

namespace Server.SkillHandlers
{
    public class RemoveTrap
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback(OnUse);
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
            if (m.Skills[SkillName.Lockpicking].Value < 50)
            {
                m.SendLocalizedMessage(502366); // You do not know enough about locks.  Become better at picking locks.
            }
            else if (m.Skills[SkillName.DetectHidden].Value < 50)
            {
                m.SendLocalizedMessage(502367); // You are not perceptive enough.  Become better at detect hidden.
            }
            else
            {
                m.Target = new InternalTarget();

                m.SendLocalizedMessage(502368); // Wich trap will you attempt to disarm?
                
                return true;
            }

            return false;
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
	            new SkillTimer(from, targeted, SkillRegistry.Delay).Start();
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
                    m_Owner.SendLocalizedMessage(502816); // You feel that such an action would be inappropriate
                }
                else if (m_Targ is TrapableContainer)
                {
                    TrapableContainer targ = (TrapableContainer)m_Targ;

                    m_Owner.Direction = m_Owner.GetDirectionTo(targ);

                    if (targ.TrapType == Server.Items.TrapType.None)
                    {
                        m_Owner.SendLocalizedMessage(502373); // That doesn't appear to be trapped
                        return;
                    }

                    m_Owner.PlaySound(0x241);
					
                    if (m_Owner.CheckTargetSkill(SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30))
                    {
                        targ.TrapPower = 0;
                        targ.TrapLevel = 0;
                        targ.TrapType = Server.Items.TrapType.None;
                        m_Owner.SendLocalizedMessage(502377); // You successfully render the trap harmless
                    }
                    else
                    {
                        m_Owner.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off
                    }
                }
                else if (m_Targ is BaseFactionTrap)
                {
                    BaseFactionTrap trap = (BaseFactionTrap)m_Targ;
                    Faction faction = Faction.Find(m_Owner);

                    FactionTrapRemovalKit kit = (m_Owner.Backpack == null ? null : m_Owner.Backpack.FindItemByType(typeof(FactionTrapRemovalKit)) as FactionTrapRemovalKit);

                    bool isOwner = (trap.Placer == m_Owner || (trap.Faction != null && trap.Faction.IsCommander(m_Owner)));

                    if (faction == null)
                    {
                        m_Owner.SendLocalizedMessage(1010538); // You may not disarm faction traps unless you are in an opposing faction
                    }
                    else if (faction == trap.Faction && trap.Faction != null && !isOwner)
                    {
                        m_Owner.SendLocalizedMessage(1010537); // You may not disarm traps set by your own faction!
                    }
                    else if (!isOwner && kit == null)
                    {
                        m_Owner.SendLocalizedMessage(1042530); // You must have a trap removal kit at the base level of your pack to disarm a faction trap.
                    }
                    else
                    {
                        if ((Core.ML && isOwner) || (m_Owner.CheckTargetSkill(SkillName.RemoveTrap, trap, 80.0, 100.0) && m_Owner.CheckTargetSkill(SkillName.Tinkering, trap, 80.0, 100.0)))
                        {
                            m_Owner.PrivateOverheadMessage(MessageType.Regular, trap.MessageHue, trap.DisarmMessage, m_Owner.NetState);

                            if (!isOwner)
                            {
                                int silver = faction.AwardSilver(m_Owner, trap.SilverFromDisarm);

                                if (silver > 0)
                                    m_Owner.SendLocalizedMessage(1008113, true, silver.ToString("N0")); // You have been granted faction silver for removing the enemy trap :
                            }

                            trap.Delete();
                        }
                        else
                        {
                            m_Owner.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off
                        }

                        if (!isOwner && kit != null)
                            kit.ConsumeCharge(m_Owner);
                    }
                }
                else if (m_Targ is VvVTrap)
                {
                    VvVTrap trap = m_Targ as VvVTrap;

                    if (!ViceVsVirtueSystem.IsVvV(m_Owner))
                    {
                        m_Owner.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                    }
                    else
                    {
                        if (m_Owner == trap.Owner || ((m_Owner.Skills[SkillName.RemoveTrap].Value - 80.0) / 20.0) > Utility.RandomDouble())
                        {
                            VvVTrapKit kit = new VvVTrapKit(trap.TrapType);
                            trap.Delete();

                            if (!m_Owner.AddToBackpack(kit))
                                kit.MoveToWorld(m_Owner.Location, m_Owner.Map);

                            if (trap.Owner != null && m_Owner != trap.Owner)
                            {
                                Guild fromG = m_Owner.Guild as Guild;
                                Guild ownerG = trap.Owner.Guild as Guild;

                                if (fromG != null && fromG != ownerG && !fromG.IsAlly(ownerG) && ViceVsVirtueSystem.Instance != null 
                                    && ViceVsVirtueSystem.Instance.Battle != null && ViceVsVirtueSystem.Instance.Battle.OnGoing)
                                {
                                    ViceVsVirtueSystem.Instance.Battle.Update(m_Owner, UpdateType.Disarm);
                                }
                            }

                            m_Owner.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155413, m_Owner.NetState);
                        }
                        else if (.1 > Utility.RandomDouble())
                        {
                            trap.Detonate(m_Owner);
                        }
                    }
                }
                else
                {
                    m_Owner.SendLocalizedMessage(502373); // That does'nt appear to be trapped
                }
 
                SkillRegistry.Remove(m_Owner);
			}
		}
    }
}