using System;

namespace Server.Items
{
	[Flipable( 0x976, 0x977)]
	public class StealableBaconSlab : RaresBaseFood
	{
		[Constructable]
		public StealableBaconSlab() : this( 1 )
		{
		}

		[Constructable]
		public StealableBaconSlab( int amount ) : base( 0x976 )
		{
			Amount = amount;
			ItemID = Utility.RandomList( 2422, 2423 );
			Stackable = true;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableBaconSlab( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}