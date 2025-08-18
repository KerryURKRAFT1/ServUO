using System;

namespace Server.Items
{
	[Flipable( 0x1723, 0x1724)]
	public class StealableCoconutHalf : RaresBaseFood
	{
		[Constructable]
		public StealableCoconutHalf() : base( 0x1723 )
		{
			ItemID = Utility.RandomList( 5923, 5924 );
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableCoconutHalf( Serial serial ) : base( serial )
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