using System;
using Server.Items;
using Server.Network; 

namespace Server.Items 
{ 
	public class StealableHorseShoes : Item
	{
		[Constructable]
		public StealableHorseShoes() : base( 0xFB6 )
		{
			Weight = 3.0;
	        Movable = false; 
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableHorseShoes( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
} 
