using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Menus.ItemLists;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.SkillHandlers
{
    public delegate bool TrackTypeDelegate(Mobile m);

    public class Tracking
    {
        private static readonly Dictionary<Mobile, TrackingInfo> m_Table = new Dictionary<Mobile, TrackingInfo>();

        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Tracking].Callback = new SkillUseCallback(OnUse);
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
            m.SendLocalizedMessage(1011350); // What do you wish to track?

			m.SendMenu(new TrackWhatMenuUor(m));
			
            return true;
        }

        public static void AddInfo(Mobile tracker, Mobile target)
        {
            TrackingInfo info = new TrackingInfo(tracker, target);
            m_Table[tracker] = info;
        }

        public static double GetStalkingBonus(Mobile tracker, Mobile target)
        {
            m_Table.TryGetValue(tracker, out var info);

            if (info == null || info.m_Target != target || info.m_Map != target.Map)
                return 0.0;

            int xDelta = info.m_Location.X - target.X;
            int yDelta = info.m_Location.Y - target.Y;

            double bonus = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));

            m_Table.Remove(tracker);

            if (Core.ML)
                return Math.Min(bonus, 10 + tracker.Skills.Tracking.Value / 10);

            return bonus;
        }

        public static void ClearTrackingInfo(Mobile tracker)
        {
            m_Table.Remove(tracker);
        }

        public class TrackingInfo
        {
            public Mobile m_Tracker;
            public Mobile m_Target;
            public Point2D m_Location;
            public Map m_Map;
            public TrackingInfo(Mobile tracker, Mobile target)
            {
                this.m_Tracker = tracker;
                this.m_Target = target;
                this.m_Location = new Point2D(target.X, target.Y);
                this.m_Map = target.Map;
            }
        }

        // --------- MODERN UI (UOR) ---------

        public class TrackWhatMenuUor : ItemListMenu
        {
            private readonly Mobile m_From;
            public TrackWhatMenuUor(Mobile from)
                : base("What do you wish to track?", GetCategoryEntries())
            {
                m_From = from;
            }

            private static ItemListEntry[] GetCategoryEntries()
            {
                return new ItemListEntry[]
                {
                    new ItemListEntry("Animals", 9682),
                    new ItemListEntry("Monsters", 9607),
                    new ItemListEntry("Human NPCs", 8454),
                    new ItemListEntry("Players", 8455)
                };
            }

            public override void OnResponse(NetState state, int index)
            {
                bool success = m_From.CheckSkill(SkillName.Tracking, 0.0, 21.1);
   	            TrackWhoMenuUor.DisplayTo(success, m_From, index);
            }

			public override void OnCancel(NetState state)
			{ 
               	SkillRegistry.Remove(m_From);
			}
        }

        public class TrackWhoMenuUor : ItemListMenu
        {
            private readonly Mobile m_From;
            private readonly int m_Category;
            private readonly List<Mobile> m_Targets;
            private readonly int m_Range;

            private static readonly TrackTypeDelegate[] m_Delegates = new TrackTypeDelegate[]
            {
                IsAnimal, IsMonster, IsHumanNPC, IsPlayer
            };

            private TrackWhoMenuUor(Mobile from, List<Mobile> targets, int range, int category)
                : base("Select the one you would like to track.", GetTargetEntries(targets))
            {
                m_From = from;
                m_Targets = targets;
                m_Range = range;
                m_Category = category;
            }

            private static ItemListEntry[] GetTargetEntries(List<Mobile> targets)
            {
                var entries = new List<ItemListEntry>();
                foreach (var mob in targets)
                {
                    entries.Add(new ItemListEntry(
                        mob.Name ?? mob.GetType().Name,
                        ShrinkTable.Lookup(mob)
                    ));
                }
                return entries.ToArray();
            }

            public static void DisplayTo(bool success, Mobile from, int category)
            {
                if (!success)
                {
                    from.SendLocalizedMessage(1018092); // You see no evidence of those in the area.
	                SkillRegistry.Remove(from);
                    return;
                }

                Map map = from.Map;
                if (map == null)
                {
	                SkillRegistry.Remove(from);
                	return;
                }

                TrackTypeDelegate check = m_Delegates[category];
                from.CheckSkill(SkillName.Tracking, 21.1, 100.0);

                int range = 10 + (int)(from.Skills[SkillName.Tracking].Value / 10);
                List<Mobile> list = new List<Mobile>();
                foreach (Mobile m in from.GetMobilesInRange(range))
                {
                    if (m != from && (!Core.AOS || m.Alive) && (!m.Hidden || m.IsPlayer() || from.AccessLevel > m.AccessLevel) && check(m) && CheckDifficulty(from, m))
                        list.Add(m);
                }

                if (list.Count > 0)
                {
                    list.Sort(new InternalSorter(from));
                    from.SendMenu(new TrackWhoMenuUor(from, list, range, category));
                    from.SendLocalizedMessage(1018093); // Select the one you would like to track.
                }
                else
                {
                    if (category == 0)
                        from.SendLocalizedMessage(502991); // You see no evidence of animals in the area.
                    else if (category == 1)
                        from.SendLocalizedMessage(502993); // You see no evidence of creatures in the area.
                    else
                        from.SendLocalizedMessage(502995); // You see no evidence of people in the area.

	                SkillRegistry.Remove(from);
                }
            }

            public override void OnResponse(NetState state, int index)
            {
                if (index >= 0 && index < m_Targets.Count)
                {
                    Mobile mob = m_Targets[index];
                    m_From.QuestArrow = new TrackArrow(m_From, mob, m_Range * 2);

                    if (Core.SE)
                        Tracking.AddInfo(m_From, mob);
                }
            }

			public override void OnCancel(NetState state)
			{ 
               	SkillRegistry.Remove(m_From);
        	}

			private class InternalSorter : IComparer<Mobile>
            {
                private readonly Mobile m_From;
                public InternalSorter(Mobile from)
                {
                    m_From = from;
                }

                public int Compare(Mobile x, Mobile y)
                {
                    if (x == null && y == null)
                        return 0;
                    else if (x == null)
                        return -1;
                    else if (y == null)
                        return 1;

                    return m_From.GetDistanceToSqrt(x).CompareTo(m_From.GetDistanceToSqrt(y));
                }
            }
        }

        // --------- TRACKING LOGIC ---------

        private static bool CheckDifficulty(Mobile from, Mobile m)
        {
            if (!Core.AOS || !m.Player)
                return true;

            int tracking = from.Skills[SkillName.Tracking].Fixed;
            int detectHidden = from.Skills[SkillName.DetectHidden].Fixed;

            if (Core.ML && m.Race == Race.Elf)
                tracking /= 2;

            int hiding = m.Skills[SkillName.Hiding].Fixed;
            int stealth = m.Skills[SkillName.Stealth].Fixed;
            int divisor = hiding + stealth;

            if (TransformationSpellHelper.UnderTransformation(m, typeof(HorrificBeastSpell)))
                divisor -= 200;
            else if (TransformationSpellHelper.UnderTransformation(m, typeof(VampiricEmbraceSpell)) && divisor < 500)
                divisor = 500;
            else if (TransformationSpellHelper.UnderTransformation(m, typeof(WraithFormSpell)) && divisor <= 2000)
                divisor += 200;

            int chance;
            if (divisor > 0)
            {
                if (Core.SE)
                    chance = 50 * (tracking * 2 + detectHidden) / divisor;
                else
                    chance = 50 * (tracking + detectHidden + 10 * Utility.RandomMinMax(1, 20)) / divisor;
            }
            else
                chance = 100;

            return chance > Utility.Random(100);
        }

        private static bool IsAnimal(Mobile m)
        {
            return (!m.Player && m.Body.IsAnimal);
        }

        private static bool IsMonster(Mobile m)
        {
            return (!m.Player && m.Body.IsMonster);
        }

        private static bool IsHumanNPC(Mobile m)
        {
            return (!m.Player && m.Body.IsHuman);
        }

        private static bool IsPlayer(Mobile m)
        {
            return m.Player;
        }

        public class TrackArrow : QuestArrow
        {
            private readonly Timer m_Timer;
            private Mobile m_From;
         	private IEntity m_Target;
         	
           public TrackArrow(Mobile from, IEntity target, int range)
                : base(from, target)
            {
                m_From = from;
	            m_Target = target;
                m_Timer = new TrackTimer(from, target, range, this);
                m_Timer.Start();
            }

            public override void OnClick(bool rightClick)
            {
                if (rightClick)
                {
                    Tracking.ClearTrackingInfo(m_From);

                    m_From = null;

	               	SkillRegistry.Remove(m_From);
	               	
					Stop();
                }
            }

            public override void OnStop()
            {
                m_Timer.Stop();

                if (m_From != null)
                {
	                Tracking.ClearTrackingInfo(m_From);
					
	                if (!m_From.InRange(m_Target, 3))
	                {
	                	m_From.SendLocalizedMessage(503177); // You have lost your quarry.
	                }
	                else
	                {
	                	m_From.SendMessage("You have found your quarry");
	                }

					SkillRegistry.Remove(m_From);
                }
            }
        }

        public class TrackTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly IEntity m_Target;
            private readonly int m_Range;
            private readonly QuestArrow m_Arrow;
            private int m_LastX, m_LastY;
            public TrackTimer(Mobile from, IEntity target, int range, QuestArrow arrow)
                : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
            {
                m_From = from;
                m_Target = target;
                m_Range = range;
                m_Arrow = arrow;
            }

            protected override void OnTick()
            {
                if (!m_Arrow.Running)
                {
                    Stop();
					SkillRegistry.Remove(m_From);
                    return;
                }
                else if (m_From.NetState == null || m_From.Deleted || m_Target.Deleted || m_From.Map != m_Target.Map || !m_From.InRange(m_Target, m_Range) || m_Target is Mobile mob && (mob.Hidden && mob.AccessLevel > m_From.AccessLevel))
                {
                    m_Arrow.Stop();
                    Stop();
					SkillRegistry.Remove(m_From);
                    return;
                }

                if (m_LastX != m_Target.Location.X || m_LastY != m_Target.Location.Y)
                {
                    m_LastX = m_Target.Location.X;
                    m_LastY = m_Target.Location.Y;

                    m_Arrow.Update();
                }
            }
        }
    }
}