using System;
using Server.Network; 

namespace Server.Items 
{ 
   	[FlipableAttribute( 0x1365, 0x1365 )] 
   	public class StealableRock3 : RaresBaseItem 
  	{ 
		[Constructable]
	 	public StealableRock3() : this( 1 )
		{
		}

	 	[Constructable]
		public StealableRock3( int amount ) : base( 0x1365 )
		{
			Stackable = false;
			Weight = 4.0;
			Amount = amount;
			Movable = false;
         	Name = "a rock";
			ItemFlags.SetStealable(this,true);
		}

     	public StealableRock3( Serial serial ) : base( serial ) 
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
