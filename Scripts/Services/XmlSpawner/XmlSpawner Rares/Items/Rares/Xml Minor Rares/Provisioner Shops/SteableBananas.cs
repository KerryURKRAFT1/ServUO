using System;

namespace Server.Items
{
	public class StealableBananas : RaresBaseFood
	{
		[Constructable]
		public StealableBananas() : this( 1 )
		{
		}

		[Constructable]
		public StealableBananas( int amount ) : base( amount, 0x171f )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableBananas( Serial serial ) : base( serial )
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