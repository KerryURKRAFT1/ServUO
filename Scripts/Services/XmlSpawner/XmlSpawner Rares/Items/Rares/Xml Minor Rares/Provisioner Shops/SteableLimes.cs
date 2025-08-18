using System;

namespace Server.Items
{
	public class StealableLimes : RaresBaseFood
	{
		[Constructable]
		public StealableLimes() : this( 1 )
		{
		}

		[Constructable]
		public StealableLimes( int amount ) : base( amount, 0x172B )
		{
			this.Weight = 1.0;
			this.FillFactor = 1;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableLimes( Serial serial ) : base( serial )
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