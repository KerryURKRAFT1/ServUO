// Some parts of this script were originally Peacemaker v1.6.0
// Author: Felladrin

#region References
using System;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class KillableWarriorGuard : BaseKillableGuard
	{
		[Constructable]
		public KillableWarriorGuard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 )
		{
			InitStats(225, 200, 40);

			Title = "the guard";
			
			Focus = null;
		}

		[Constructable]
		public KillableWarriorGuard( Mobile target ) : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 )
		{
			InitStats(225, 200, 40);

			Title = "the guard";
			
			Focus = target;

            Fame = 300;
            Karma = 300;
		}

		public override void InitWeapon()
		{
            BaseWeapon weapon;
			
			if (Utility.RandomBool())
			{
				weapon = new Halberd();
			}
			else
			{
				weapon = new Longsword();
				
				AddItem(new HeaterShield());
			}

			weapon.Quality = WeaponQuality.Exceptional;

			AddItem(weapon);

			Container pack = new Backpack();
			
			pack.Movable = false;					
			pack.DropItem(new Gold(10, 25));
			
			AddItem(pack);
		}

		public override void InitSkills()
		{
            SetSkill(SkillName.Swords, 120, 200);
            SetSkill(SkillName.Fencing, 120, 200);
            SetSkill(SkillName.Macing, 120, 200);
            SetSkill(SkillName.Parry, 70, 120);

            base.InitSkills();
		}
				
		public KillableWarriorGuard(Serial serial) : base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}