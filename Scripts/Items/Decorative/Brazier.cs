using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class Brazier : BaseLight, ICraftable
    {
        [Constructable]
        public Brazier()
            : base(0xE31)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = true;
            this.Light = LightType.Circle225;
            this.Weight = 20.0;
        }

        public Brazier(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xE31;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
                public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Movable = true; // Imposta Movable su true solo quando l'oggetto viene craftato

            return 1;
        }
    }
}