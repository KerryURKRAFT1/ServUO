using System;

namespace Server.Items
{
    public class AirElementalScroll : SpellScroll
    {
        [Constructable]
        public AirElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public AirElementalScroll(int amount)
            : base(8, 0x1F35, amount)
        {
        }

        public AirElementalScroll(Serial serial)
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