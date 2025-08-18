using System;
using Server.Items;
using Server.Network; 

namespace Server.Items 
{ 
	public class StealableWhip : RaresBaseItem
	{
		[Constructable]
		public StealableWhip() : base( 0x166E )
		{
			Weight = 1.0;
	         Movable = false; 
    	     ItemFlags.SetStealable(this,true);
		}

		public StealableWhip( Serial serial ) : base( serial )
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
