using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class MetalBoxEast : RaresLockableContainer
	{
		[Constructable]
		public MetalBoxEast() : base( 0x9A8 )
		{
		}

		public MetalBoxEast( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class MetalBoxSouth : RaresLockableContainer
	{
		[Constructable]
		public MetalBoxSouth() : base( 0xE80 )
		{
		}

		public MetalBoxSouth( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}