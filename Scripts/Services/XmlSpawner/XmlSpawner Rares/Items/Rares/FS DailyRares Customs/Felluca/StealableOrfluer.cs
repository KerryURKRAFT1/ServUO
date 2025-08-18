using System;

namespace Server.Items
{
	public class StealableOrfluer : RaresBaseItem
	{
		[Constructable]
		public StealableOrfluer() : base( 0xCC1 )
		{
			Movable = false;
			ItemFlags.SetStealable(this,true);
		}

		public StealableOrfluer( Serial serial ) : base( serial )
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