using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x12AB, 0x12AC )] 
   public class StealableTarotDeck : RaresBaseItem
   { 
      [Constructable] 
      public StealableTarotDeck() : base( 0x12AB ) 
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableTarotDeck( Serial serial ) : base( serial ) 
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
