using System;
using Server.Network; 

namespace Server.Items 
{ 
   [FlipableAttribute( 0x12A9, 0x12AA )] 
   public class StealableTarot3 : RaresBaseItem 
   { 
      [Constructable] 
      public StealableTarot3() : base( 0x12A9 ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableTarot3( Serial serial ) : base( serial ) 
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
