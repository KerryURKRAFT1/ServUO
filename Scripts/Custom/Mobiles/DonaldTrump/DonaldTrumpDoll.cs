using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class DonaldTrumpDoll : Item
	{
		[Constructable]
		public DonaldTrumpDoll() : this( 1 )
		{
		}
		
		[Constructable]
        public DonaldTrumpDoll( int amount ) : base( 0x2106 )
		{
			Name = "Donald Trump Doll";
            Weight = 1.0;
			Hue = 602;
		}

		public DonaldTrumpDoll( Serial serial ) : base( serial )
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

        //public override Item Dupe( int amount )
        //{
        //return base.Dupe( new DonaldTrumpDoll( amount ), amount );
        //}

        public override void OnDoubleClick( Mobile from )
		
		{ 
			switch ( Utility.Random( 5 ) )
			{
				default:
				case  0: this.PublicOverheadMessage( MessageType.Regular, 602, true, "I have a very good brain and I've said a lot of things."); break;
				case  1: this.PublicOverheadMessage( MessageType.Regular, 602, true, "We will make America proud again. We will make America safe again. And we will make America great again. ... No dream is too big. No challenge is too great."); break;	
				case  2: this.PublicOverheadMessage( MessageType.Regular, 602, true, "DonaldTrump"); break;
				case  3: this.PublicOverheadMessage( MessageType.Regular, 602, true, "I could stand in the middle of 5th Avenue and shoot somebody and I wouldn't lose voters."); break;
				case  4: this.PublicOverheadMessage( MessageType.Regular, 602, true, "Sorry losers and haters, but my I.Q. is one of the highest – and you all know it! Please don't feel so stupid or insecure, it's not your fault."); break;
			}
		}
	}
}
