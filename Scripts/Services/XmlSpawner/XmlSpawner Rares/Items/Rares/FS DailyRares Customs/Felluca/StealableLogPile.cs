using System;

namespace Server.Items
{
	public class StealableLogPile : RaresBaseItem
	{
		[Constructable]
		public StealableLogPile() : base( 0x1BE2 )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableLogPile( Serial serial ) : base( serial )
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