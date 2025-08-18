using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	public abstract class RaresBaseFood : Food
	{
		public virtual bool SmartSpawning{ get{ return false; } }

		public RaresBaseFood( int itemID ) : this( 1, itemID )
		{
		}

		public RaresBaseFood( int amount, int itemID ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
			FillFactor = 1;
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

		public RaresBaseFood( Serial serial ) : base( serial )
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