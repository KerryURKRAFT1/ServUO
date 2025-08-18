using System;

namespace Server.Items
{
	public class StealableCheeseWedge : RaresBaseFood
	{
		[Constructable]
		public StealableCheeseWedge() : this( 1 )
		{
		}

		[Constructable]
		public StealableCheeseWedge( int amount ) : base( amount, 0x97D )
		{
			this.FillFactor = 3;
			this.Weight = 1.0;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableCheeseWedge( Serial serial ) : base( serial )
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