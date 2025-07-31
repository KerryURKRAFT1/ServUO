using System;
using Server.Factions;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Server.Engines.VvV;
using Server.Network;

namespace Server.SkillHandlers
{
    public class DetectHidden
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback(OnUse);
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
            m.SendLocalizedMessage(500819);//Where will you search?
            m.Target = new InternalTarget();

            return true;
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile src, object targ)
            {			
	            new SkillTimer(src, targ, SkillRegistry.Delay).Start();
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
               	bool foundAnyone = false;
               	
                Point3D p;
                if (m_Targ is Mobile)
                    p = ((Mobile)m_Targ).Location;
                else if (m_Targ is Item)
                    p = ((Item)m_Targ).Location;
                else if (m_Targ is IPoint3D)
                    p = new Point3D((IPoint3D)m_Targ);
                else 
                    p = m_Owner.Location;

                double srcSkill = m_Owner.Skills[SkillName.DetectHidden].Value;
                int range = (int)(srcSkill / 10.0);

                if (!m_Owner.CheckSkill(SkillName.DetectHidden, 0.0, 100.0))
                    range /= 2;

                BaseHouse house = BaseHouse.FindHouseAt(p, m_Owner.Map, 16);

                bool inHouse = (house != null && house.IsFriend(m_Owner));

                if (inHouse)
                    range = 22;

                if (range > 0)
                {	                
                    IPooledEnumerable inRange = m_Owner.Map.GetMobilesInRange(p, range);

                    foreach (Mobile trg in inRange)
                    {
                        if (trg.Hidden && m_Owner != trg)
                        {
                            double ss = srcSkill + Utility.Random(21) - 10;
                            double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random(21) - 10;

                            if (m_Owner.AccessLevel >= trg.AccessLevel && (ss >= ts || (inHouse && house.IsInside(trg))))
                            {
                                if (trg is ShadowKnight && (trg.X != p.X || trg.Y != p.Y))
                                    continue;

                                trg.RevealingAction();
                                trg.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnyone = true;
                            }
                        }
                    }

                    inRange.Free();

                    bool faction = Faction.Find(m_Owner) != null;
                    bool vvv = ViceVsVirtueSystem.IsVvV(m_Owner);

                    if (faction || vvv)
                    {
                        IPooledEnumerable itemsInRange = m_Owner.Map.GetItemsInRange(p, range);

                        foreach (Item item in itemsInRange)
                        {
                            if (faction && item is BaseFactionTrap)
                            {
                                BaseFactionTrap trap = (BaseFactionTrap)item;

                                if (m_Owner.CheckTargetSkill(SkillName.DetectHidden, trap, 80.0, 100.0))
                                {
                                    m_Owner.SendLocalizedMessage(1042712, true, " " + (trap.Faction == null ? "" : trap.Faction.Definition.FriendlyName)); // You reveal a trap placed by a faction:

                                    trap.Visible = true;
                                    trap.BeginConceal();

                                    foundAnyone = true;
                                }
                            }
                            else if (vvv && (item is VvVSigil || item is VvVTrap) && Utility.Random(100) <= srcSkill)
                            {
                                if (item is VvVTrap && item.ItemID == VvVTrap.HiddenID)
                                    ((VvVTrap)item).OnRevealed(m_Owner);
                                else if (!item.Visible)
                                    item.Visible = true;
                            }
                        }

                        itemsInRange.Free();
                    }
                }

                if (!foundAnyone)
                {
                    m_Owner.SendLocalizedMessage(500817); // You can see nothing hidden there.
                }

				SkillRegistry.Remove(m_Owner);
			}
		}

        public static void DoPassiveDetect(Mobile src)
        {
			if (src == null || src.Map == null || src.Location == Point3D.Zero || src.IsStaff())
				return;

            double ss = src.Skills[SkillName.DetectHidden].Value;

            if (ss <= 0)
                return;

            IPooledEnumerable eable = src.Map.GetMobilesInRange(src.Location, 4);

			if (eable == null)
				return;

            foreach (Mobile m in eable)
            {
                if (m == null || m is ShadowKnight)
                    continue;

                int noto = Notoriety.Compute(src, m);

                if (m != src && noto != Notoriety.Innocent && noto != Notoriety.Ally && noto != Notoriety.Invulnerable)
                {
                    double ts = (m.Skills[SkillName.Hiding].Value + m.Skills[SkillName.Stealth].Value) / 2;

                    if (src.Race == Race.Elf)
                        ss += 20;

                    if (src.AccessLevel >= m.AccessLevel && Utility.Random(1000) < (ss - ts) + 1)
                    {
                        m.RevealingAction();
                        m.SendLocalizedMessage(500814); // You have been revealed!
                    }
                }
            }

            eable.Free();
        }
    }
}