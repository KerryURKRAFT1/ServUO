using System;

namespace Server.Items
{
	[Flipable( 0xA6C, 0xA6D, 0xA6E, 0xA6F)]
	public class StealableBlanket : RaresBaseItem
	{
		[Constructable]
		public StealableBlanket() : base( 0xA6C )
		{
			Hue = Utility.RandomNondyedHue();
		    Movable = false; 
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableBlanket( Serial serial ) : base( serial )
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
	}
}