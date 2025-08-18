using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class RaresBarrel : RaresBaseContainer
	{
		[Constructable]
		public RaresBarrel() : base( 0xE77 )
		{
			Movable = false;
			Weight = 25.0;
		}

		public RaresBarrel( Serial serial ) : base( serial )
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

			if ( Weight == 0.0 )
				Weight = 25.0;
		}
	}
}