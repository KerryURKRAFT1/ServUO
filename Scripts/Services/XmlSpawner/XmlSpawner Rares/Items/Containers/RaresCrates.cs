using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class SmallCrateEast : RaresLockableContainer
	{
		[Constructable]
		public SmallCrateEast() : base( 0x9A9 )
		{
		}

		public SmallCrateEast( Serial serial ) : base( serial )
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

	public class SmallCrateSouth : RaresLockableContainer
	{
		[Constructable]
		public SmallCrateSouth() : base( 0xE7E )
		{
		}

		public SmallCrateSouth( Serial serial ) : base( serial )
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

	public class MediumCrateEast : RaresLockableContainer
	{
		[Constructable]
		public MediumCrateEast() : base( 0xE3F )
		{
		}

		public MediumCrateEast( Serial serial ) : base( serial )
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

	public class MediumCrateSouth : RaresLockableContainer
	{
		[Constructable]
		public MediumCrateSouth() : base( 0xE3E )
		{
		}

		public MediumCrateSouth( Serial serial ) : base( serial )
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

	public class LargeCrateEast : RaresLockableContainer
	{
		[Constructable]
		public LargeCrateEast() : base( 0xE3C )
		{
		}

		public LargeCrateEast( Serial serial ) : base( serial )
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

	public class LargeCrateSouth : RaresLockableContainer
	{
		[Constructable]
		public LargeCrateSouth() : base( 0xE3D )
		{
		}

		public LargeCrateSouth( Serial serial ) : base( serial )
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