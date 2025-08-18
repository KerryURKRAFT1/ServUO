using System;
using Server.Network; 

namespace Server.Items 
{ 
   [FlipableAttribute( 0x1EBA, 0x1EBB )]
   public class StealableSquareToolkit : RaresBaseItem
   { 
      [Constructable] 
      public StealableSquareToolkit() : base( 0x1EBA )
      { 
         Movable = false;
         ItemFlags.SetStealable(this,true);
      } 

      public StealableSquareToolkit( Serial serial ) : base( serial )
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
