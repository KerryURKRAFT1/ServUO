using System;

namespace Server.Items
{
    public class RemoveTrapScroll : SpellScroll
    {
        [Constructable]
        public RemoveTrapScroll()
            : this(1)
        {
        }

        [Constructable]
        public RemoveTrapScroll(int amount)
            : base(47, 0x1F5C, amount)
        {
        }

        public RemoveTrapScroll(Serial serial)
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