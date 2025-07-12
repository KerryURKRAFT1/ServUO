using System;

namespace Server.Items
{
    public class LesserManaPotion : BaseManaPotion
    {
        [Constructable]
        public LesserManaPotion() : base(PotionEffect.ManaLesser)
        {
        	Name = "Lesser Mana Potion";
        	Hue = 1916;
        }

        public override int MinMana
        {
            get
            {
                return (Core.AOS ? 8 : 5);
            }
        }
        
        public override int MaxMana
        {
            get
            {
                return (Core.AOS ? 11 : 15);
            }
        }
        
        public override double Delay
        {
            get
            {
                return (Core.AOS ? 8.0 : 10.0);
            }
        }

        public LesserManaPotion(Serial serial) : base(serial)
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