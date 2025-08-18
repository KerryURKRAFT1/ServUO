using System;
using Server;

namespace Server.Items
{
	public class StealableBasketOfFruit : RaresBaseFood
	{
		[Constructable]
		public StealableBasketOfFruit() : base( 1, 0x993 )
		{
			Weight = 2.0;
			FillFactor = 5;
			Stackable = false;
		    Movable = false; 
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableBasketOfFruit( Serial serial ) : base( serial )
		{
		}

		public override bool Eat( Mobile from )
		{
			if ( !base.Eat( from ) )
				return false;

			from.AddToBackpack( new Basket() );
			return true;
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
	}
}