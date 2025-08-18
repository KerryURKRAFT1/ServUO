using System;

namespace Server.Items
{
		public class StealableSealedBarrel : RaresLockableContainer
	{
		[Constructable]
		public StealableSealedBarrel() : base(0xFAE)
		{
			Weight = 1.0;
	        Movable = false;
	        ItemFlags.SetStealable(this,true);
		}

		public StealableSealedBarrel(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}