using System;

namespace Server.Items
{
		public class StealableDriedOnions : RaresBaseItem
	{
		[Constructable]
		public StealableDriedOnions() : this( 1 )
		{
		}

		[Constructable]
		public StealableDriedOnions( int amount ) : base( 0xC40 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
	         Movable = false;
    	     ItemFlags.SetStealable(this,true);
		}		

		public StealableDriedOnions( Serial serial ) : base( serial )
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
