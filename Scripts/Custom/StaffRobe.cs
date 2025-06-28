using System;

namespace Server.Items
{
    public class GMRobe : BaseSuit
    {
        private static Mobile m_Owner;
        private AccessLevel m_GMLevel;

        [Constructable]
        public GMRobe() : base(AccessLevel.Player, 0, 0x204F)
        {
            LootType = LootType.Blessed;
            Name = "GM Robe";
        }
        
        [Constructable]
        public GMRobe(Mobile m) : base(m.AccessLevel, 0, 0x204F)
        {
            m_Owner = m;
            m_GMLevel = m.AccessLevel;

        	LootType = LootType.Blessed;
        	Name = $"{m.Name}'s GM Robe";
        }
        
        public override bool OnEquip(Mobile m)
        {
        	if (m.IsStaff() && m_Owner == null)
            {
                m_Owner = m;

                Name = $"{m.Name}'s GM Robe";
                	        	
                m_GMLevel = m.AccessLevel;				
        	}
        	
            DoHue(m);
            
	       	return base.OnEquip(m);
        }

        public override void OnRemoved(object parent)
        {
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if(m_Owner == null && m.IsStaff())
            {
                m_Owner = m;
                
                m_GMLevel = m.AccessLevel;
            
                m.SendMessage(48, "This robe has been assigned to you.");

	            Name = $"{m.Name}'s GM Robe";
            }
            else if (m_Owner == m)
            {
	            GMRobe robe = m.FindItemOnLayer(Layer.Backpack) as GMRobe ?? m.FindItemOnLayer(Layer.OuterTorso) as GMRobe;

            	if (robe != null)
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
            	}
            }
            else
            {
            	Delete();
            }
	            
            DoHue(m);
        }
		    		
		private void DoHue(Mobile m)
        {
            switch (m.AccessLevel)
            {
            	default:
                    Hue = 0;
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
            writer.Write(m_Owner);
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
                    m_Owner = reader.ReadMobile();
                    
                    m_GMLevel = (AccessLevel)reader.ReadEncodedInt();
                    
                    break;
            	}
            }
            
            if (ItemID != 0x204F)
            {
            	ItemID = 0x204F;
            }
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
	            GMRobe robe = m.FindItemOnLayer(Layer.Backpack) as GMRobe ?? m.FindItemOnLayer(Layer.OuterTorso) as GMRobe;
	
		        if (robe == null)
		        {
		        	m.Backpack.DropItem(new GMRobe(m));
		        }
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
		{	
    		Mobile m = e.Mobile;
    		
    		if (m == m_Owner)
    		{
    			new AutoResTimer(m_Owner).Start();
    		}
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
				m_Mobile.Resurrect();
				
				m_Mobile.Hits =	m_Mobile.HitsMax;
				
				m_Mobile.SendMessage(48, "Your GM robe resurrects you");
				
				Stop();
			}
		}
    }
}
