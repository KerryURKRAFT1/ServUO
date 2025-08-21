using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class DonaldTrumpDollGold : Item
	{
		[Constructable]
		public DonaldTrumpDollGold() : this( 1 )
		{
		}
		
		[Constructable]
		public DonaldTrumpDollGold( int amount ) : base( 0x2106 )
		{
			Name = "Golden DonaldTrump Doll";
            Weight = 1.0;
			Hue = 252;
		}

		public DonaldTrumpDollGold( Serial serial ) : base( serial )
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
        //return base.Dupe( new DonaldTrumpDollGold( amount ), amount );
        //}

        public override void OnDoubleClick( Mobile from ) 
		
		{ 
			switch ( Utility.Random( 8 ) )
			{
				default:
				case  0: this.PublicOverheadMessage( MessageType.Regular, 252, true, "I have a very good brain and I've said a lot of things."); break;
				case  1: this.PublicOverheadMessage( MessageType.Regular, 252, true, "We will make America proud again. We will make America safe again. And we will make America great again. ... No dream is too big. No challenge is too great."); break;	
				case  2: this.PublicOverheadMessage( MessageType.Regular, 252, true, "DonaldTrump"); break;
				case  3: this.PublicOverheadMessage( MessageType.Regular, 252, true, "I could stand in the middle of 5th Avenue and shoot somebody and I wouldn't lose voters.!"); break;
				case  4: this.PublicOverheadMessage( MessageType.Regular, 252, true, "Sorry losers and haters, but my I.Q. is one of the highest – and you all know it! Please don't feel so stupid or insecure, it's not your fault."); break;
				case  5: this.PublicOverheadMessage( MessageType.Regular, 252, true, "DonaldTrump Likes Bacon"); break;
				case  6: this.PublicOverheadMessage( MessageType.Regular, 252, true,"Make America Great Again!" ); break;
				case  7: this.PublicOverheadMessage( MessageType.Regular, 252, true,"Go Brandon!" ); break;
			}
		}
	}
}
