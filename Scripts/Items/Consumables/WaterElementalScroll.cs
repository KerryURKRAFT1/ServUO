using System;

namespace Server.Items
{
    public class WaterElementalScroll : SpellScroll
    {
        [Constructable]
        public WaterElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public WaterElementalScroll(int amount)
            : base(8, 0x1F35, amount)
        {
        }

        public WaterElementalScroll(Serial serial)
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