using System;

namespace Server.Items
{
	public class StealableDung : RaresBaseItem
	{
		[Constructable]
		public StealableDung() : base( 0xF3B )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableDung( Serial serial ) : base( serial )
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