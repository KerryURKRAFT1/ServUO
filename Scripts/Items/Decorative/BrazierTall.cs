using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class BrazierTall : BaseLight, ICraftable
    {
        [Constructable]
        public BrazierTall()
            : base(0x19AA)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = true;
            this.Light = LightType.Circle300;
            this.Weight = 25.0;
        }

        public BrazierTall(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x19AA;
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