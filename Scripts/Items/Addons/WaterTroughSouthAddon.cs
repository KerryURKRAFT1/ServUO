using System;

namespace Server.Items
{
    public class WaterTroughSouthAddon : BaseAddon, IWaterSource
    {

        

        [Constructable]
        public WaterTroughSouthAddon()
        {
            this.AddComponent(new AddonComponent(0xB43), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xB44), 1, 0, 0);
        }

        public WaterTroughSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new WaterTroughSouthDeed();
            }
        }




        public int Quantity
        {
            get
            {
                return 500;
            }
            set
            {
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

    public class WaterTroughSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public WaterTroughSouthDeed()
        {
        }

        public WaterTroughSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new WaterTroughSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044350;
            }
        }// water trough (south)
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