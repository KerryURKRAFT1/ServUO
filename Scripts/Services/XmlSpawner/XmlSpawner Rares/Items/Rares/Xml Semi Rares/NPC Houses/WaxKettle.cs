using System;
using Server.Network; 

namespace Server.Items 
{ 

   [FlipableAttribute( 0x142A, 0x142A )] 
   public class StealableWaxKettle : RaresBaseItem
   { 
      [Constructable] 
      public StealableWaxKettle() : base( 0x142A ) 
      { 
         Movable = false; 
         ItemFlags.SetStealable(this,true);
      } 

      public StealableWaxKettle( Serial serial ) : base( serial ) 
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
