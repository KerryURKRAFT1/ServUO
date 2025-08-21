using System;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class DonaldTrumpShoes : BaseShoes
	{
		[Constructable]
		public DonaldTrumpShoes() : this( 0 )
		{
		}

		[Constructable]
		public DonaldTrumpShoes( int hue ) : base( 0x170B, hue )
		{
			Weight = 3.0;
			Name = "DonaldTrump's Shoes";
			Hue = 1;
		}

		public DonaldTrumpShoes( Serial serial ) : base( serial )
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
