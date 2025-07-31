using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class GMRobe : Robe
    {
        private Mobile m_Wearer = null;
        private AccessLevel m_GMLevel = AccessLevel.Player;

        public Mobile Wearer { get {return m_Wearer;} }
        public AccessLevel GMLevel { get {return m_GMLevel;} }

        [Constructable]
        public GMRobe() : base(9859)
        {
            LootType = LootType.Blessed;
            Name = "GM Robe";
            ItemID = 9859;
        }
                
        public override bool OnEquip(Mobile m)
        {        	
    		if (m_Wearer == null)
    		{
	        	if (m.IsStaff())
	            {
	                m_Wearer = m;
	
	                Name = $"{m.Name}'s GM Robe";
	                	        	
	                m_GMLevel = m.AccessLevel;
	        	}
    		}

    		if (m_Wearer == m)
        	{
	            DoHue(m);
	    		return true;
        	}
    		
    		return false;
        }

        public override void OnRemoved(object parent)
        {
        	if (Hue > 0)            
        	{
        		base.OnRemoved(parent);
        	}
        }

        public override void OnDoubleClick(Mobile m)
        {
        	if(m_Wearer == null)
        	{
        		if (m.IsStaff())
            	{
	                m_Wearer = m;
	                
	                m_GMLevel = m.AccessLevel;
	            
	                m.SendMessage(48, "This robe has been assigned to you.");
	
		            Name = $"{m.Name}'s GM Robe";
	            }
        	}

        	if (m_Wearer == m)
            {
	        	if (m.IsStaff())
	            {
	                m.SendMessage(48, "You are now a player");
	                
	                m.AccessLevel = AccessLevel.Player;
	
	                m.Blessed = false;
	            }
	        	else
	            {
	                m.SendMessage(48, "You are now staff");
	                
	                m.AccessLevel = m_GMLevel;
	                
	                m.Blessed = true;
	        	}
	
	            DoHue(m);
            }
            else
            {
            	m.SendMessage(48, $"{m_Wearer.Name}");
            }
        }
		    		
		private void DoHue(Mobile m)
        {
            switch (m.AccessLevel)
            {
            	default:
                    break;
                case AccessLevel.Owner:
                    Hue = 0x497;
                    break;
                case AccessLevel.CoOwner:
                    Hue = 0x481;
                    break;
                case AccessLevel.Developer:
                    Hue = 0x498;
                    break;
                case AccessLevel.Administrator:
                    Hue = 0x47E;
                    break;
                case AccessLevel.Seer:
                    Hue = 0x494;
                    break;
                case AccessLevel.GameMaster:
                    Hue = 0x5B5;
                    break;
                case AccessLevel.Spawner:
                    Hue = 0x493;
                    break;
                case AccessLevel.Decorator:
                    Hue = 0x493;
                    break;
                case AccessLevel.Counselor:
                    Hue = 0x5B6;
                    break;
            }
        }

        public GMRobe(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            
            //version 0
            writer.Write(m_Wearer);
            writer.WriteEncodedInt((int)m_GMLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            switch (version)
            {
                case 0:
            	{
                    m_Wearer = reader.ReadMobile();
                    
                    m_GMLevel = (AccessLevel)reader.ReadEncodedInt();
                    
                    break;
            	}
            }
            
           	ItemID = 9859;
        }

        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);

            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            Mobile m = args.Mobile;
                
            if (m.IsStaff())
            {
	            GMRobe robe = m.FindItemOnLayer(Layer.Backpack) as GMRobe;
	            	
		        if (robe == null)
	            {
	            	robe = m.FindItemOnLayer(Layer.OuterTorso) as GMRobe;
	            }
	
		        if (robe == null)
		        {
		        	m.Backpack.DropItem(new GMRobe());
		        }
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
		{
    		Mobile m = e.Mobile;
    		
            List<Item> items = new List<Item>(m.Backpack.Items);

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];

                if (item is GMRobe robe && robe.Wearer == m)
                {
                	m.AccessLevel = robe.GMLevel;
                	
		    		new AutoResTimer(robe).Start();

		    		break;
                }
            }
        }

		private class AutoResTimer : Timer
		{
			private readonly GMRobe m_Robe;

			public AutoResTimer(GMRobe robe) : base(TimeSpan.FromSeconds(5.0))
			{
				m_Robe = robe;
			}

			protected override void OnTick()
			{
				if (!m_Robe.Wearer.Alive)
				{					
		            m_Robe.Wearer.Resurrect();
	
		            m_Robe.Wearer.Hits = m_Robe.Wearer.HitsMax;
					
					m_Robe.Wearer.SendMessage(48, "Your GM robe resurrects you");

					Item item = m_Robe.Wearer.FindItemOnLayer(Layer.OuterTorso);
					
					if (item != null && m_Robe.Wearer.Backpack.TryDropItem(m_Robe.Wearer, item, true))
					{
			            m_Robe.Wearer.EquipItem(m_Robe);
			            
			            if (item is DeathRobe dr)
			            {
			            	dr.Delete();
			            }
					}
				}
				
	            Stop();
			}
		}
    }
}
