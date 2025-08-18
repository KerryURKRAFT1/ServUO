using System;

namespace Server.Items
{
	public class StealableRareBandage : Bandage
	{
		[Constructable]
		public StealableRareBandage()
		{
			int chance = Utility.Random( 100 );

			if ( chance <= 5 )
				ItemID = 0xEE9;

	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public override void GetProperties( ObjectPropertyList list )
      	{ 
         	base.GetProperties( list ); 

         	if (ItemFlags.GetStealable(this) )
         	    list.Add("Rare"); // notify

         	if (ItemFlags.GetTaken(this) )
         	    list.Add("Stolen Property"); // give credit
      	}

		public StealableRareBandage( Serial serial ) : base( serial )
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