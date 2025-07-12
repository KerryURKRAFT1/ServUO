using System;

namespace Server.Items
{
    public class ManaPotion : BaseManaPotion
    {
        [Constructable]
        public ManaPotion() : base(PotionEffect.Mana)
        {
        	Name = "Mana Potion";
        	Hue = 1916;
        }

        public override int MinMana
        {
            get
            {
                return (Core.AOS ? 13 : 6);
            }
        }
        
        public override int MaxMana
        {
            get
            {
                return (Core.AOS ? 16 : 20);
            }
        }
        
        public override double Delay
        {
            get
            {
                return (Core.AOS ? 8.0 : 10.0);
            }
        }

        public ManaPotion(Serial serial) : base(serial)
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