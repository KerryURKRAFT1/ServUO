using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x1008, 0x1008 )] 
   public class StealableWaterBasin : RaresBaseItem
   { 
      [Constructable] 
      public StealableWaterBasin() : base( 0x1008 ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableWaterBasin( Serial serial ) : base( serial ) 
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
