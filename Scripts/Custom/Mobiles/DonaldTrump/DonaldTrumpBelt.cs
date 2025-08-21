using System;
using Server.Items;

namespace Server.Items
{
	[Flipable( 0x27A0, 0x27EB )]
	public class DonaldTrumpBelt : BaseWaist
	{
		[Constructable]
		public DonaldTrumpBelt() : this( 0 )
		{
			Name = "Donald Trump's Belt";
			Hue = 643;
		}

		[Constructable]
		public DonaldTrumpBelt( int hue ) : base( 0x27A0, hue )
		{
			Weight = 1.0;
		}

		public DonaldTrumpBelt( Serial serial ) : base( serial )
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
