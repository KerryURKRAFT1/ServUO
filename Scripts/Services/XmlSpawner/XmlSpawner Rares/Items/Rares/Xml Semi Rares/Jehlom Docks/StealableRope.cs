using System;

namespace Server.Items 
{ 
	public class StealableRope : RaresBaseItem
	{
		[Constructable]
		public StealableRope() : this( 1 )
		{
		}

		[Constructable]
		public StealableRope( int amount ) : base( 0x14F8 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableRope( Serial serial ) : base( serial )
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
