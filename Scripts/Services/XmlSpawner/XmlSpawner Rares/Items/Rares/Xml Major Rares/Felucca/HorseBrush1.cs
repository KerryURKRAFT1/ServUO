using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x1370, 0x1371 )] 
   public class StealableHorseBrush1 : RaresBaseItem 
   { 
      [Constructable] 
      public StealableHorseBrush1() : base( 0x1370 ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableHorseBrush1( Serial serial ) : base( serial ) 
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
