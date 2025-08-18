using System;

namespace Server.Items
{
	[Flipable( 0xA2C, 0xA34)]
	public class StealableDresser : RaresBaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x51; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public StealableDresser() : base( 0xA2C )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableDresser( Serial serial ) : base( serial )
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