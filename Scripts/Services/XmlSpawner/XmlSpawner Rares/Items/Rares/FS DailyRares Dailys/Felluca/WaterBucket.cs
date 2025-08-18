using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0xFFA, 0xFFA )] 
   public class StealableWaterBucket : RaresBaseItem
   { 
      [Constructable] 
      public StealableWaterBucket() : base( 0xFFA ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableWaterBucket( Serial serial ) : base( serial ) 
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
