using System;
using Server.Network; 

namespace Server.Items 
{ 
	public class StealablePenAndInk : RaresBaseItem
	{
		[Constructable]
		public StealablePenAndInk() : base( 0xFBF )
		{
	         Movable = false; 
    	     ItemFlags.SetStealable(this,true);
		}

		public StealablePenAndInk( Serial serial ) : base( serial )
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
