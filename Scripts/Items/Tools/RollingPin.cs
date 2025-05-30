using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class RollingPin : BaseTool
    {
        [Constructable]
        public RollingPin()
            : base(0x1043)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public RollingPin(int uses)
            : base(uses, 0x1043)
        {
            this.Weight = 1.0;
        }

        public RollingPin(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                if (Core.UOR)
                {
                    return DefClassicCooking.CraftSystem;
                }
                else
                {
                    return DefCooking.CraftSystem;
                }
            }
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