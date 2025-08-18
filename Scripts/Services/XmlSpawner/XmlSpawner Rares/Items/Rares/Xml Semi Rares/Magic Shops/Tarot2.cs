using System;
using Server.Network; 

namespace Server.Items 
{ 
   [FlipableAttribute( 0x12A7, 0x12A8 )] 
   public class StealableTarot2 : RaresBaseItem
   { 
      [Constructable] 
      public StealableTarot2() : base( 0x12A7 ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableTarot2( Serial serial ) : base( serial ) 
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
