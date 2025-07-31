using System;
using System.Collections;
using Server.Network;
using Server.Regions;
using Server.SkillHandlers;

namespace Server.Items
{
    public class Kindling : Item
    {
        [Constructable]
        public Kindling()
            : this(1)
        {
        }

        [Constructable]
        public Kindling(int amount)
            : base(0xDE1)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public Kindling(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(this.GetWorldLocation(), 2))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
    		else if (!SkillRegistry.Contains(m))
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
        }

        public bool TriggerSkill(Mobile m)
        {
            if (!this.VerifyMove(m))
                return false;

            new SkillTimer(m, this, SkillRegistry.Delay).Start();
            
            m.SendMessage("You attempt to light a fire");

			return true;
        }

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly Kindling m_Kindling;

            public SkillTimer(Mobile owner, Kindling kindling, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Kindling = kindling;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
	            Point3D fireLocation = m_Kindling.GetFireLocation(m_Owner);
	
	            if (fireLocation == Point3D.Zero)
	            {
	                m_Owner.SendLocalizedMessage(501695); // There is not a spot nearby to place your campfire.
	            }
	            else if (!m_Owner.CheckSkill(SkillName.Camping, 0.0, 100.0))
	            {
	                m_Owner.SendLocalizedMessage(501696); // You fail to ignite the campfire.
	            }
	            else
	            {
	                m_Kindling.Consume();
	
	                if (!m_Kindling.Deleted && m_Kindling.Parent == null)
	                    m_Owner.PlaceInBackpack(m_Kindling);
	
	                new Campfire().MoveToWorld(fireLocation, m_Owner.Map);
	            }

				SkillRegistry.Remove(m_Owner);
			}
        }
        
        private Point3D GetFireLocation(Mobile from)
        {
            if (from.Region.IsPartOf(typeof(DungeonRegion)))
                return Point3D.Zero;

            if (this.Parent == null)
                return this.Location;

            ArrayList list = new ArrayList(4);

            this.AddOffsetLocation(from, 0, -1, list);
            this.AddOffsetLocation(from, -1, 0, list);
            this.AddOffsetLocation(from, 0, 1, list);
            this.AddOffsetLocation(from, 1, 0, list);

            if (list.Count == 0)
                return Point3D.Zero;

            int idx = Utility.Random(list.Count);
            return (Point3D)list[idx];
        }

        private void AddOffsetLocation(Mobile from, int offsetX, int offsetY, ArrayList list)
        {
            Map map = from.Map;

            int x = from.X + offsetX;
            int y = from.Y + offsetY;

            Point3D loc = new Point3D(x, y, from.Z);

            if (map.CanFit(loc, 1) && from.InLOS(loc))
            {
                list.Add(loc);
            }
            else
            {
                loc = new Point3D(x, y, map.GetAverageZ(x, y));

                if (map.CanFit(loc, 1) && from.InLOS(loc))
                    list.Add(loc);
            }
        }
    }
}