using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class WoodenChestEast : RaresLockableContainer
	{
		[Constructable]
		public WoodenChestEast() : base( 0xe42 )
		{
		}

		public WoodenChestEast( Serial serial ) : base( serial )
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

	public class WoodenChestSouth : RaresLockableContainer
	{
		[Constructable]
		public WoodenChestSouth() : base( 0xe43 )
		{
		}

		public WoodenChestSouth( Serial serial ) : base( serial )
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

	public class PlainWoodenChestEast : RaresLockableContainer
	{
		[Constructable]
		public PlainWoodenChestEast() : base( 0x280B )
		{
		}

		public PlainWoodenChestEast( Serial serial ) : base( serial )
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

	public class PlainWoodenChestSouth : RaresLockableContainer
	{
		[Constructable]
		public PlainWoodenChestSouth() : base( 0x280C )
		{
		}

		public PlainWoodenChestSouth( Serial serial ) : base( serial )
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

	public class OrnateWoodenChestEast : RaresLockableContainer
	{
		[Constructable]
		public OrnateWoodenChestEast() : base( 0x280D )
		{
		}

		public OrnateWoodenChestEast( Serial serial ) : base( serial )
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

	public class OrnateWoodenChestSouth : RaresLockableContainer
	{
		[Constructable]
		public OrnateWoodenChestSouth() : base( 0x280E )
		{
		}

		public OrnateWoodenChestSouth( Serial serial ) : base( serial )
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

	public class GildedWoodenChestEast : RaresLockableContainer
	{
		[Constructable]
		public GildedWoodenChestEast() : base( 0x280F )
		{
		}

		public GildedWoodenChestEast( Serial serial ) : base( serial )
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

	public class GildedWoodenChestSouth : RaresLockableContainer
	{
		[Constructable]
		public GildedWoodenChestSouth() : base( 0x28010 )
		{
		}

		public GildedWoodenChestSouth( Serial serial ) : base( serial )
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