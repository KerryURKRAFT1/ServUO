using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class MetalChestEast : RaresLockableContainer
	{
		[Constructable]
		public MetalChestEast() : base( 0xE7C )
		{
			Movable = false;
		}

		public MetalChestEast( Serial serial ) : base( serial )
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

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	public class MetalChestSouth : RaresLockableContainer
	{
		[Constructable]
		public MetalChestSouth() : base( 0x9AB )
		{
			Movable = false;
		}

		public MetalChestSouth( Serial serial ) : base( serial )
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

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	public class MetalGoldenChestEast : RaresLockableContainer
	{
		[Constructable]
		public MetalGoldenChestEast() : base( 0xE40 )
		{
			Movable = false;
		}

		public MetalGoldenChestEast( Serial serial ) : base( serial )
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

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	public class MetalGoldenChestSouth : RaresLockableContainer
	{
		[Constructable]
		public MetalGoldenChestSouth () : base( 0xE41 )
		{
			Movable = false;
		}

		public MetalGoldenChestSouth ( Serial serial ) : base( serial )
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

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}
}