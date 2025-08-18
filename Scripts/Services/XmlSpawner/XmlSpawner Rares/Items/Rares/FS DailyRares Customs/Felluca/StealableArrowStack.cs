using System;

namespace Server.Items
{
	public class StealableArrowStack : RaresBaseItem
	{
		[Constructable]
		public StealableArrowStack() : base( 0xF41 )
		{
		    Movable = false; 
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableArrowStack( Serial serial ) : base( serial )
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