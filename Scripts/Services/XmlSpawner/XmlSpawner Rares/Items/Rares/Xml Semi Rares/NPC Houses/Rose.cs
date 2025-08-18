using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x18E9, 0x18EA )] 
   public class StealableRose : RaresBaseItem
   { 
      [Constructable] 
      public StealableRose() : base( 0x18E9 ) 
      { 
		 Movable = false;
		 ItemFlags.SetStealable(this,true);
         Name = "a rose";
      } 

      public StealableRose( Serial serial ) : base( serial ) 
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
