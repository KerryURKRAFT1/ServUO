using System;

namespace Server.Items
{
	[Flipable( 0x1E35, 0x1E34)]
	public class StealableScareCrow : RaresBaseItem
	{
		[Constructable]
		public StealableScareCrow() : base( 0x1E35 )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableScareCrow( Serial serial ) : base( serial )
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