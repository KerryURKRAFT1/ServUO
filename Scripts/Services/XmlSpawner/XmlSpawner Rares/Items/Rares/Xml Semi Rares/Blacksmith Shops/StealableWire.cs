using System;

namespace Server.Items 
{ 
	public class StealableIronWire : RaresBaseItem
	{
		[Constructable]
		public StealableIronWire() : this( 1 )
		{
		}

		[Constructable]
		public StealableIronWire( int amount ) : base( 0x1876 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}		

		public StealableIronWire( Serial serial ) : base( serial )
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

	public class StealableSilverWire : RaresBaseItem
	{
		[Constructable]
		public StealableSilverWire() : this( 1 )
		{
		}

		[Constructable]
		public StealableSilverWire( int amount ) : base( 0x1877 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableSilverWire( Serial serial ) : base( serial )
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

	public class StealableGoldWire : RaresBaseItem
	{
		[Constructable]
		public StealableGoldWire() : this( 1 )
		{
		}

		[Constructable]
		public StealableGoldWire( int amount ) : base( 0x1878 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}	

		public StealableGoldWire( Serial serial ) : base( serial )
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

	public class StealableCopperWire : RaresBaseItem
	{
		[Constructable]
		public StealableCopperWire() : this( 1 )
		{
		}

		[Constructable]
		public StealableCopperWire( int amount ) : base( 0x1879 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
	        Movable = false;
    	    ItemFlags.SetStealable(this,true);
		}

		public StealableCopperWire( Serial serial ) : base( serial )
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
