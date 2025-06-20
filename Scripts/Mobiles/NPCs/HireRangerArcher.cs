using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireRangerArcher : BaseHire 
    {
        [Constructable] 
        public HireRangerArcher()
            : base(AIType.AI_Archer)
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else 
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the ranger";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.SetStr(91, 91);
            this.SetDex(76, 76);
            this.SetInt(61, 61);

            this.SetDamage(13, 24);

            this.SetSkill(SkillName.Wrestling, 100, 100);
            this.SetSkill(SkillName.Parry, 60, 80);
            this.SetSkill(SkillName.Archery, 100, 100);
            this.SetSkill(SkillName.Magery, 62, 62);
            this.SetSkill(SkillName.Swords, 100, 100);
            this.SetSkill(SkillName.Fencing, 100, 100);
            this.SetSkill(SkillName.Tactics, 100, 100);

            this.Fame = 100;
            this.Karma = 125;

            this.AddItem(new Shoes(Utility.RandomNeutralHue()));
            this.AddItem(new Shirt());

            // Pick a random sword
            switch ( Utility.Random(2)) 
            {
                case 0:
                    this.AddItem(new Bow());
                    break;
                case 1:
                    this.AddItem(new CompositeBow());
                    break;
            }

            this.AddItem(new RangerChest());
            this.AddItem(new RangerArms());
            this.AddItem(new RangerGloves());
            this.AddItem(new RangerGorget());
            this.AddItem(new RangerLegs());

            this.PackItem(new Arrow(20));
            this.PackGold(10, 75);
        }

        public HireRangerArcher(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer) 
        {
            base.Serialize(writer);

            writer.Write((int)0);// version 
        }

        public override void Deserialize(GenericReader reader) 
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}