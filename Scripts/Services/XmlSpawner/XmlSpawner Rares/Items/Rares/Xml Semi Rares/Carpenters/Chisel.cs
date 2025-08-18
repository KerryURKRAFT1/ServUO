using System;
using Server.Network; 

namespace Server.Items 
{ 
   [FlipableAttribute( 0x1026, 0x1027 )]
   public class StealableChisel : RaresBaseItem
   { 
      [Constructable] 
      public StealableChisel() : base( 0x1026 )
      { 
         Movable = false;
   	     ItemFlags.SetStealable(this,true);
      }

      public StealableChisel( Serial serial ) : base( serial )
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
