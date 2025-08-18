using System;

namespace Server.Items
{
	public class BookcaseEast : RaresBaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x4D; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 80, 5, 140, 70 ); }
		}

		[Constructable]
		public BookcaseEast() : base( 0xA99 )
		{
			Movable = false;
		}

		public BookcaseEast( Serial serial ) : base( serial )
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
	public class BookcaseSouth : RaresBaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x4D; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 80, 5, 140, 70 ); }
		}

		[Constructable]
		public BookcaseSouth() : base( 0xA97 )
		{
			Movable = false;
		}

		public BookcaseSouth( Serial serial ) : base( serial )
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