using System;
using Server.Items;
using Server.Network; 

namespace Server.Items 
{ 
	public class StealablePaintsAndBrush : RaresBaseItem
	{
		[Constructable]
		public StealablePaintsAndBrush() : base( 0xFC1 )
		{
    	    ItemFlags.SetStealable(this,true);
		}

		public StealablePaintsAndBrush( Serial serial ) : base( serial )
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
