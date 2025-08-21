#region Header
// **********
// ServUO - WarriorGuard.cs
// **********
#endregion

#region References
using System;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class WarriorGuard : BaseKillableGuard
	{
		private Timer m_AttackTimer, m_IdleTimer;
		private Mobile m_Focus;

		[Constructable]
		public WarriorGuard() : this(null)
		{
		}

		[Constructable]
		public WarriorGuard( Mobile target ) : base( AIType.AI_Archer, FightMode.Weakest, 15, 5, 0.1, 0.2 )
		{
			InitStats(225, 200, 40);

			Title = "the guard";

			Focus = target;
		}

		public override void InitBody()
		{
			if (Female = Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");

				switch (Utility.Random(2))
				{
					case 0:
						AddItem(new LeatherSkirt());
						break;
					case 1:
						AddItem(new LeatherShorts());
						break;
				}

				switch (Utility.Random(5))
				{
					case 0:
						AddItem(new FemaleLeatherChest());
						break;
					case 1:
						AddItem(new FemaleStuddedChest());
						break;
					case 2:
						AddItem(new LeatherBustierArms());
						break;
					case 3:
						AddItem(new StuddedBustierArms());
						break;
					case 4:
						AddItem(new FemalePlateChest());
						break;
				}
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");

				AddItem(new PlateChest());
				AddItem(new PlateArms());
				AddItem(new PlateLegs());

				switch (Utility.Random(3))
				{
					case 0:
						AddItem(new Doublet(Utility.RandomNondyedHue()));
						break;
					case 1:
						AddItem(new Tunic(Utility.RandomNondyedHue()));
						break;
					case 2:
						AddItem(new BodySash(Utility.RandomNondyedHue()));
						break;
				}

				if (Utility.RandomBool())
				{
					Utility.AssignRandomFacialHair(this, HairHue);
				}
			}

			Hue = Utility.RandomSkinHue();

			Utility.AssignRandomHair(this);
		}

		public override void InitWeapon()
		{
			Halberd weapon = new Halberd();

			weapon.Movable = false;
			weapon.Crafter = this;
			weapon.Quality = WeaponQuality.Exceptional;

			AddItem(weapon);

			Container pack = new Backpack();

			pack.Movable = false;

			pack.DropItem(new Gold(10, 25));

			AddItem(pack);
		}

		public override void InitSkills()
		{
			Skills[SkillName.Anatomy].Base = 120.0;
			Skills[SkillName.Tactics].Base = 120.0;
			Skills[SkillName.Swords].Base = 120.0;
			Skills[SkillName.MagicResist].Base = 120.0;
			Skills[SkillName.DetectHidden].Base = 100.0;
		}	
		
		public WarriorGuard(Serial serial)
			: base(serial)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public override Mobile Focus
		{
			get { return m_Focus; }
			set
			{
				if (Deleted)
				{
					return;
				}

				Mobile oldFocus = m_Focus;

				if (oldFocus != value)
				{
					m_Focus = value;

					if (value != null)
					{
						AggressiveAction(value);
					}

					Combatant = value;

					if (oldFocus != null && !oldFocus.Alive)
					{
						Say("Thou hast suffered thy punishment, scoundrel.");
					}

					if (value != null)
					{
						Say(500131); // Thou wilt regret thine actions, swine!
					}

					if (m_AttackTimer != null)
					{
						m_AttackTimer.Stop();
						m_AttackTimer = null;
					}

					if (m_IdleTimer != null)
					{
						m_IdleTimer.Stop();
						m_IdleTimer = null;
					}

					if (m_Focus != null)
					{
						m_AttackTimer = new AttackTimer(this);
						m_AttackTimer.Start();
						((AttackTimer)m_AttackTimer).DoOnTick();
					}
					else
					{
						m_IdleTimer = new IdleTimer(this);
						m_IdleTimer.Start();
					}
				}
				else if (m_Focus == null && m_IdleTimer == null)
				{
					m_IdleTimer = new IdleTimer(this);
					m_IdleTimer.Start();
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_Focus);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_Focus = reader.ReadMobile();

						if (m_Focus != null)
						{
							m_AttackTimer = new AttackTimer(this);
							m_AttackTimer.Start();
						}
						else
						{
							m_IdleTimer = new IdleTimer(this);
							m_IdleTimer.Start();
						}

						break;
					}
			}
		}
	}
}