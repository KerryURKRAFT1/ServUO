using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x100C, 0x100D )] 
   public class StealableHaySheaf : RaresBaseItem
   { 
      [Constructable] 
      public StealableHaySheaf() : base( 0x100C ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableHaySheaf( Serial serial ) : base( serial ) 
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
