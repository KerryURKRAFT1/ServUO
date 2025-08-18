using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x1372, 0x1373 )] 
   public class StealableHorseBrush2 : RaresBaseItem
   { 
      [Constructable] 
      public StealableHorseBrush2() : base( 0x1372 ) 
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableHorseBrush2( Serial serial ) : base( serial ) 
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
