using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x0E48, 0x0E49 )] 
	public class StealableFullJarsX2 : RaresBaseItem 
	{ 
		[Constructable] 
		public StealableFullJarsX2() : base( 0x0E48 ) 
		{ 
			Weight = 2.0;
		    Movable = false; 
    	    ItemFlags.SetStealable(this,true);
		} 

		public StealableFullJarsX2( Serial serial ) : base( serial ) 
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