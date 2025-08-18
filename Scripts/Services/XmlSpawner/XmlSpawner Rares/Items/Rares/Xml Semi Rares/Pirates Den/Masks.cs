using System;

namespace Server.Items
{
	public class StealableBearMask : BaseHat
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 8; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 30; } }

		[Constructable]
		public StealableBearMask() : this( 0 )
		{
		}

		[Constructable]
		public StealableBearMask( int hue ) : base( 0x1545, hue )
		{
			Weight = 5.0;
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


		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public StealableBearMask( Serial serial ) : base( serial )
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

	public class StealableDeerMask : BaseHat
	{
		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 8; } }
		public override int BasePoisonResistance{ get{ return 1; } }
		public override int BaseEnergyResistance{ get{ return 7; } }

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 30; } }

		[Constructable]
		public StealableDeerMask() : this( 0 )
		{
		}

		[Constructable]
		public StealableDeerMask( int hue ) : base( 0x1547, hue )
		{
			Weight = 4.0;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public override void GetProperties( ObjectPropertyList list )
      	{ 
         	base.GetProperties( list ); 

         	if (ItemFlags.GetStealable(this) )
         	    list.Add("Rare"); // notify

         	if (ItemFlags.GetTaken(this) )
         	    list.Add("Stolen Property"); // give credit
      	}

		public StealableDeerMask( Serial serial ) : base( serial )
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