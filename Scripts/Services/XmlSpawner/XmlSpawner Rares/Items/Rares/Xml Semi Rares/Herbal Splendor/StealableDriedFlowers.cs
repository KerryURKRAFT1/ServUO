using System;

namespace Server.Items
{
	[Flipable( 0xC3B, 0xC3C)]
	public class StealableWhiteDriedFlowers : RaresBaseItem
	{
		[Constructable]
		public StealableWhiteDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public StealableWhiteDriedFlowers( int amount ) : base( 0xC3B )
		{
			Amount = amount;
			ItemID = Utility.RandomList( 3131, 3132 );
			Stackable = true;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableWhiteDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Flipable( 0xC3D, 0xC3E)]
	public class StealableGreenDriedFlowers : RaresBaseItem
	{
		[Constructable]
		public StealableGreenDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public StealableGreenDriedFlowers( int amount ) : base( 0xC3D )
		{
			Amount = amount;
			ItemID = Utility.RandomList( 3133, 3134 );
			Stackable = true;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableGreenDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}