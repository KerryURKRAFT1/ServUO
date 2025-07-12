using System;

namespace Server.Items
{
    public class GreaterManaPotion : BaseManaPotion
    {
        [Constructable]
        public GreaterManaPotion() : base(PotionEffect.ManaGreater)
        {
        	Name = "Greater Mana Potion";
        	Hue = 1916;
        }

        public override int MinMana
        {
            get
            {
                return (Core.AOS ? 18 : 11);
            }
        }
        
        public override int MaxMana
        {
            get
            {
                return (Core.AOS ? 21 : 25);
            }
        }
        
        public override double Delay
        {
            get
            {
                return (Core.AOS ? 8.0 : 10.0);
            }
        }

        public GreaterManaPotion(Serial serial) : base(serial)
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