using System;

namespace Server.Items
{
	public class StealableRareIngot : RaresBaseItem
	{
		[Constructable]
		public StealableRareIngot() : this( 1 )
		{
		}

		[Constructable]
		public StealableRareIngot( int amount ) : base( 0x1BF2 )
		{
			Amount = amount;

			int chance = Utility.Random( 100 );

			if ( chance <= 5 )
				ItemID = 0x1BEF;

	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableRareIngot( Serial serial ) : base( serial )
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