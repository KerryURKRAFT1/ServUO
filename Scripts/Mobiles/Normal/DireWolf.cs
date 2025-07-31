using System;

namespace Server.Mobiles
{
    [CorpseName("a dire wolf corpse")]
    [TypeAlias("Server.Mobiles.Direwolf")]
    public class DireWolf : BaseCreature
    {
        [Constructable]
        public DireWolf()
            : base(AIType.AI_Melee,FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a dire wolf";
            this.Body = 23;
            this.BaseSoundID = 0xE5;

            this.SetStr(75, 150);
            this.SetDex(153, 172);
            this.SetInt(51, 90);

            this.SetHits(58, 72);
            this.SetMana(0);

            this.SetDamage(11, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.Parry, 63.1, 75.0);
            this.SetSkill(SkillName.MagicResist, 57.6, 75.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = Utility.RandomMinMax(-1200, -600);

            this.VirtualArmor = 22;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 83.1;
        }

        public DireWolf(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 7;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
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
    }
}