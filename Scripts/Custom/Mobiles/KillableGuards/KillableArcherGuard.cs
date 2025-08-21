// Some parts of this script were originally Peacemaker v1.6.0
// Author: Felladrin

#region References
using System;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class KillableArcherGuard : BaseKillableGuard
	{
		[Constructable]
		public KillableArcherGuard() : base( AIType.AI_Archer, FightMode.Weakest, 15, 5, 0.1, 0.2 )
		{
			InitStats(200, 275, 40);
			
			Title = "the guard";

			Focus = null;

            Fame = 300;
            Karma = 300;
		}

		[Constructable]
		public KillableArcherGuard( Mobile target ) : base( AIType.AI_Archer, FightMode.Weakest, 15, 5, 0.1, 0.2 )
		{
			InitStats(200, 275, 40);
			
			Title = "the guard";

			Focus = target;
		}
		
		public override void InitWeapon()
		{
            Bow bow = new Bow();
            
			bow.Quality = WeaponQuality.Exceptional;
			
			AddItem(bow);

			Container pack = new Backpack();
			
			pack.Movable = false;
			
			Arrow arrows = new Arrow(250);
			arrows.LootType = LootType.Newbied;
			
			pack.DropItem(arrows);
			pack.DropItem(new Gold(10, 25));
			
			AddItem(pack);
		}

		public override void InitSkills()
		{
            SetSkill(SkillName.Archery, 120, 200);
            SetSkill(SkillName.Fencing, 120, 200);
            SetSkill(SkillName.Macing, 120, 200);

            base.InitSkills();
		}

		public KillableArcherGuard(Serial serial) : base(serial)
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