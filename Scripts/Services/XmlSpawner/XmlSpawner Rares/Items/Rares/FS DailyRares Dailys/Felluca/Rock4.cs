using System;
using Server.Network; 

namespace Server.Items 
{ 
   	[FlipableAttribute( 0x1366, 0x1366 )] 
   	public class StealableRock4 : RaresBaseItem
   	{ 
   		[Constructable]
	 	public StealableRock4() : this( 1 )
		{
		}

	  	[Constructable]
		public StealableRock4( int amount ) : base( 0x1366 )
		{
			Stackable = false;
			Weight = 4.0;
			Amount = amount;
			Movable = false;
         	Name = "a rock";
         	ItemFlags.SetStealable(this,true);
		}

      	public StealableRock4( Serial serial ) : base( serial ) 
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
