using System;

namespace Server.Items
{
	[Flipable( 0xA38, 0xA30)]
	public class StealableRedDresser : RaresBaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x48; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public StealableRedDresser() : base( 0xA38 )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableRedDresser( Serial serial ) : base( serial )
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