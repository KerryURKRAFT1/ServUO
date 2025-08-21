using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Regions;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public class DonaldTrump : BaseCreature
	{		
		[Constructable]
		public DonaldTrump() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			AddItem( new TrumpHair() );

			Body = 0x190;
			
			Hue = 1359;

			Name = "Donald Trump";

			SetStr( 250, 255 );
			SetDex( 100, 125 );
			SetInt( 61, 75 );

			SetDamage( 150, 230 );

			SetHits( 25000, 35000 );

			SetSkill( SkillName.Fencing, 88.8, 97.5 );
			SetSkill( SkillName.Macing, 99.9, 110.0 );
			SetSkill( SkillName.MagicResist, 25.0, 47.5 );
			SetSkill( SkillName.Swords, 65.0, 87.5 );
			SetSkill( SkillName.Tactics, 99.9, 110.0 );
			SetSkill( SkillName.Wrestling, 15.0, 37.5 );
			SetSkill( SkillName.Anatomy, 80.0, 90.1 );
			SetSkill( SkillName.Parry, 80.0, 100.0 );
			SetSkill( SkillName.Lumberjacking, 80.0, 100.0 );

			Fame = 10000;
			Karma = -10000;

			switch ( Utility.Random( 10 )) 
			{ 
				case 0: PackItem( new DonaldTrumpDollGold() ); break;
				case 1: PackItem( new Gold(2000) ); break;
				case 2: PackItem( new Gold(2000) ); break;
				case 3: PackItem( new Gold(2000) ); break;
				case 4: PackItem( new Gold(2000) ); break;
				case 5: PackItem( new Gold(2000) ); break;
				case 6: PackItem( new Gold(5000) ); break;
				case 7: PackItem( new Gold(10000) ); break;
				case 8: PackItem( new DonaldTrumpDoll() ); break;
				case 9: PackItem( new DonaldTrumpVengance() ); break;
			}

            AddItem( new DonaldTrumpPants() );
			AddItem( new DonaldTrumpShirt() );
			AddItem( new DonaldTrumpBelt() );
			AddItem( new DonaldTrumpShoes() );		
		}
									
		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss );
		}
		
        public override bool IsEnemy(Mobile m)
        {
			GuardedRegion region = Region.Find (m.Location, m.Map).GetRegion(typeof(GuardedRegion)) as GuardedRegion;
		
			if (region != null)
			{
				if (IsInitialInnocent)
				{
		        	if (m is PlayerMobile pm && pm.Combatant != this)
		    	    	return false;
		        	
		        	if (m is BaseCreature bc && (bc.Controlled || bc.Summoned) && bc.Combatant != this)
		    			return false;
				}
				else
				{
					if (m is PlayerMobile pm && pm.Karma >= 0 && !pm.Criminal && pm.Kills < 5)
		    	    	return false;
		        	
					if (m is BaseCreature bc)
					{
					    if (bc.Summoned)
		    				return true;

		        		if (bc.ControlMaster is PlayerMobile cm && (cm.Karma < 0 || cm.Criminal || cm.Kills >= 5 || bc.Karma < 0))
		    				return true;
					}
				}
			}
			
            return base.IsEnemy(m);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        // Temporary 
        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);
             
            if (e.Speech.ToLower().StartsWith("don"))
            {
            	Say( TrumpQuotes[Utility.Random(TrumpQuotes.Count)] );
            }
        }        

        public override void OnWarmodeChanged()
		{ 
			if (this.Spell != null && this.Spell.OnWarModeChange())
        	{
				((Spell)this.Spell).Disturb(DisturbType.EquipRequest);
			}

			if (!Warmode && InitialInnocent)
			{
				Timer.DelayCall( TimeSpan.FromSeconds(10.0), () => {IsInitialInnocent  = true;} );
			}
		}

		private bool m_SpeechLock = false;

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);
			
            Spam (TimeSpan.FromSeconds(5.0));
        }

        private void Spam (TimeSpan delay)
        {
            if (!m_SpeechLock)
            {
            	Say( TrumpQuotes[Utility.Random(TrumpQuotes.Count)] );
            		
            	m_SpeechLock = true;
            	
	            Timer.DelayCall( delay, () => {m_SpeechLock = false;} ); //spam, spam, less spam
            }
        }

        private List<string> TrumpQuotes = new List<string>()
        {
        	"Fake News",
        	"Everybody loves me",
        	"I demand a recount",
        	"I don't like losers",
        	"The point is that you can't be too greedy",
        	"Our country is being run by incompetent people",
        	"I am the least racist person there is",
        	"The budget was unlimited, but I exceeded it",
        	"Show me someone without an ego, and I'll show you a loser",
        	"I have great judgment",
        	"The beauty of me is that I'm very rich",
        	"I am the only one who can make America truly great again!",
        	"It's always good to be underestimated",
        	"I have a great body. I really do",
        	"Do you mind if I sit back a little? Because your breath is very bad",
        	"I think temperament is my single greatest asset",
        	"I have a winning temperament. I know how to win",
        	"The real excitement is playing the game",
        	"We have the greatest people on Earth, but they're depleted",
        	"I won't take no for an answer",
        	"Our leaders are stupid. Our politicians are stupid",
        };

		public override bool ClickTitle{ get{ return false; } }

		public override bool InitialInnocent{ get{ return true; } }

		public DonaldTrump( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version			
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	    public class TrumpHair : Hair
	    {
			[Constructable]
	        public TrumpHair() : base(0x203B, 1169)
	        {
	        }
	
	        public TrumpHair(Serial serial) : base(serial)
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
	    }
	}
}