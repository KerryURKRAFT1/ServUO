using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x1009, 0x1009 )] 
   public class StealableEmptyBasin : RaresBaseItem 
   { 
      [Constructable] 
      public StealableEmptyBasin() : base( 0x1009 ) 
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableEmptyBasin( Serial serial ) : base( serial ) 
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
