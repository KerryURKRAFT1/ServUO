using System;

namespace Server.Items
{
    public class GMRobe : Robe
    {
        private Mobile m_Wearer = null;
        private AccessLevel m_GMLevel = AccessLevel.Player;

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
    		
   			new AutoResTimer(m).Start();
		}

		private class AutoResTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public AutoResTimer(Mobile mob) : base(TimeSpan.FromSeconds(5.0))
			{
				m_Mobile = mob;
			}

			protected override void OnTick()
			{
				if (!m_Mobile.Alive)
				{					
		            m_Mobile.Resurrect();
	
		            m_Mobile.Hits =	m_Mobile.HitsMax;
					
					m_Mobile.SendMessage(48, "Your GM robe resurrects you");
				}
				
	            Stop();
			}
		}
    }
}
