using System;

namespace Server.Items
{
	public abstract class RaresBaseItem : Item
	{
		public virtual bool SmartSpawning{ get{ return false; } }

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

      	public virtual int ArtifactRarity {	get{ return 0; } } 

		public RaresBaseItem( int itemID ) : base( itemID )
		{
			Weight = 1.0;
	        ItemFlags.SetStealable(this,true);
		}

		public override void GetProperties( ObjectPropertyList list )
      	{ 
         	base.GetProperties( list ); 

         	if ( ArtifactRarity > 0 )
            	list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~ - display AR 

         	if (ItemFlags.GetStealable(this) )
         	    list.Add("Rare"); // notify

         	if (ItemFlags.GetTaken(this) )
         	    list.Add("Stolen Property"); // give credit
      	}

		public RaresBaseItem( Serial serial ) : base( serial )
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