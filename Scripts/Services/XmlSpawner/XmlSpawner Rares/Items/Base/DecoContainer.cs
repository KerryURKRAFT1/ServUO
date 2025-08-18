using System;
using Server;

namespace Server.Items
{
	public class DecoContainer : Container
	{
		public DecoContainer() : base( 0x80 )
		{
			Movable = false;
		}

		[Constructable]
		public DecoContainer( int itemID ) : base( itemID )
		{
			Movable = false;
		}

		[Constructable]
		public DecoContainer( int itemID, int count ) : this( Utility.Random( itemID, count ) )
		{
		}

		public DecoContainer( Serial serial ) : base( serial )
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