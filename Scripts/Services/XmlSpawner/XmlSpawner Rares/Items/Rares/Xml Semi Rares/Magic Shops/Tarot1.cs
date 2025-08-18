using System;
using Server.Network; 

namespace Server.Items 
{ 
   [FlipableAttribute( 0x12A5, 0x12A6 )] 
   public class StealableTarot1 : RaresBaseItem
   { 
      [Constructable] 
      public StealableTarot1() : base( 0x12A5 ) 
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableTarot1( Serial serial ) : base( serial ) 
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
