using System;

namespace Server.Items 
{ 

   [FlipableAttribute( 0x142B, 0x142B )] 
   public class StealableWaxPot : RaresBaseItem
   { 
      [Constructable] 
      public StealableWaxPot() : base( 0x142B ) 
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableWaxPot( Serial serial ) : base( serial ) 
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
