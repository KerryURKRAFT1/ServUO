// Some parts of this script were originally Peacemaker v1.6.0
// Author: Felladrin

#region References
using System;
using Server.Items;
using Server.Engines.XmlSpawner2;
using System.Collections.Generic;
using Server.Engines.VvV;
using Server.Regions;
#endregion

namespace Server.Mobiles
{
	public class BaseKillableGuard : BaseCreature
	{
		public override bool IsEnemy(Mobile m)
		{
			if (!InLOS(m))
			{
				return false;
			}

			PlayerMobile pm = m as PlayerMobile;
			
			BaseCreature bc = m as BaseCreature;		

			if (pm != null || (bc != null && (bc.Controlled || bc.Summoned || bc.InitialInnocent)))
			{
				if (m.IsStaff() || m.Deleted)
				{
					return false;
				}

				if (m.AlwaysInnocent || !m.Alive || m.Blessed || m.Hidden)
				{
					return false;
				}

				if (bc != null)
				{	
					if (bc.InitialInnocent && !bc.IsInitialInnocent)
					{
//						Console.WriteLine($"II: {m.Name}");
						return true;
					}
					
				    if (bc.IsGuardExempt)
					{
//						Console.WriteLine($"IGE: {m.Name}");
						return false;
					}
	
				    if ((bc.Controlled || bc.Summoned) && bc.ControlMaster.Kills >= 5)
					{
//						Console.WriteLine($"CM: {m.Name}");
						return true;
					}
				}
			}										
							
			return (m.Kills >= 5 || m.Karma < 0 || m.Criminal);
		}

		public bool Bandaging = false;
		public bool BlockTeleport = false;
		public bool BlockCall = false;
		public bool BlockGreet = false;
		
		private Timer m_AttackTimer, m_IdleTimer;
		
		private Mobile m_Focus;
		
		public BaseKillableGuard(AIType ai, FightMode fm, int PR, int FR, double AS, double PS) : base( ai, fm, PR, FR, AS, PS )
		{
			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			Utility.AssignRandomHair(this);

			NextCombatTime = Core.TickCount + 500;
			
			SetDamageType(ResistanceType.Physical, 120);

			InitMount();
			
			InitBody();
			
			InitWeapon();
			
			InitSkills();

			InitPots();
		}

		public virtual void InitMount()
		{
			if (Map == Map.Ilshenar || Map == Map.Malas)
			{
				new KillableGuardMount().Rider = this;
			}
		}
		
		public virtual void InitBody()
		{
			List<Item> toColorWhite = new List<Item>();
			List<Item> toColorRed = new List<Item>();
			
			if (Female = Utility.RandomBool())
			{
				Body = 0x191;
				
				Name = NameList.RandomName("female");
				
				switch (Utility.Random(3))
				{
					case 0:
					{
						switch (Utility.Random(2))
						{
							case 0: toColorWhite.Add(new FemaleLeatherChest()); AddItem(new LeatherArms()); break;
							case 1: toColorWhite.Add(new LeatherBustierArms());break;
						}

						switch (Utility.Random(3))
						{
							case 0: toColorWhite.Add(new LeatherSkirt()); break;
							case 1: toColorWhite.Add(new LeatherShorts()); break;
							case 2: AddItem(new LeatherLegs()); break;
						}

						AddItem(new LeatherGorget());
						AddItem(new LeatherGloves());

						break;
					}
					case 1:
					{
						switch (Utility.Random(2))
						{
							case 0: toColorWhite.Add(new FemaleLeatherChest()); AddItem(new StuddedArms()); break;
							case 1: toColorWhite.Add(new StuddedBustierArms());break;
						}

						AddItem(new StuddedGorget());
						AddItem(new StuddedGloves());
						AddItem(new StuddedLegs());

						break;
					}
					case 2:
					{
						toColorWhite.Add(new FemalePlateChest());

						switch (Utility.Random(2))
						{
							case 0: AddItem(new CloseHelm()); break;
							case 1: AddItem(new NorseHelm()); break;
						}

						AddItem(new PlateGorget());
						AddItem(new PlateGloves());
						AddItem(new PlateArms());
						AddItem(new PlateLegs());
		
						break;
					}
				}
				
				AddItem(new ThighBoots());
			}
			else
			{
				Body = 0x190;
				
				Name = NameList.RandomName("male");

				if (Utility.RandomBool())
				{
					Utility.AssignRandomFacialHair(this, HairHue);
				}
								
				switch (Utility.Random(2))
				{
					case 0: AddItem(new CloseHelm()); break;
					case 1: AddItem(new NorseHelm()); break;
				}

				toColorWhite.Add(new PlateChest());

				AddItem(new PlateGorget());

				AddItem(new PlateGloves());

				AddItem(new PlateArms());
				
				AddItem(new PlateLegs());
				
				toColorRed.Add(new Boots());
			}

			switch (Utility.Random(3))
			{
				case 0: toColorWhite.Add(new Doublet()); break;
				case 1: toColorWhite.Add(new Tunic()); break;
				case 2: toColorRed.Add(new BodySash()); break;
			}
			
			toColorRed.Add(new Cloak());

			foreach (Item item in toColorWhite)
			{
				item.Hue = WhiteHue();
				item.Movable = false;
				AddItem(item);
			}

			foreach (Item item in toColorRed)
			{
				item.Hue = RedHue();
				item.Movable = false;
				AddItem(item);
			}
		}

		private int WhiteHue()
		{
			return Utility.RandomList(1153, 2738, 1150, 2035, 2498, 2301);
		}
		
		private int RedHue()
		{
			return Utility.RandomList(33, 1194, 1646, 1141, 1645, 335, 1964, 1358, 1652, 1644, 1194);
		}

		public virtual void InitSkills()
		{
			SetSkill(SkillName.Anatomy, 120, 200);
			SetSkill(SkillName.Tactics, 120, 200);
			SetSkill(SkillName.Wrestling, 65, 120);
			SetSkill(SkillName.Healing, 65, 75);
			SetSkill(SkillName.MagicResist, 80, 120);
			SetSkill(SkillName.DetectHidden, 120, 200);

			if (Utility.Random(1, 2) == 2)
			{
				OmniAI.SetRandomSkillSet(this, 70.0, 110.0);
			}
		}
		
		public virtual void InitPots()
		{
			for (int i = 0; i < 10; i++)
			{
				PackItem( new GreaterCurePotion() );
				PackItem( new GreaterHealPotion() );
				PackItem( new TotalRefreshPotion() );
			}
	
			PackItem(new Bandage(Utility.RandomMinMax(30, 50)));
		}
		
		protected override BaseAI ForcedAI
		{
			get
			{
				return new OmniAI(this);
			}
		}

		public override bool CanHeal { get { return true; } }
	
		public override void OnThink()
		{
			base.OnThink();
	
			if ( this.Poisoned )
			{
				GreaterCurePotion m_CurePot = (GreaterCurePotion)this.Backpack.FindItemByType( typeof ( GreaterCurePotion ) );
				
				if ( m_CurePot != null )
				{
					m_CurePot.Drink( this );
				}
			}

			if ( this.Hits <= (this.HitsMax * 0.7) ) // Will try to use heal pots if he's at or below 70% health
			{
				GreaterHealPotion m_HealPot = (GreaterHealPotion)this.Backpack.FindItemByType( typeof ( GreaterHealPotion ) );
				
				if ( m_HealPot != null )
				{
					m_HealPot.Drink( this );
				}
			}
			
			if ( this.Stam <= (this.StamMax * 0.25) ) // Will use a refresh pot if he's at or below 25% stam
			{
				TotalRefreshPotion m_RefreshPot = (TotalRefreshPotion)this.Backpack.FindItemByType( typeof ( TotalRefreshPotion) );
				
				if ( m_RefreshPot != null )
				{
					m_RefreshPot.Drink( this );
				}
			}
			
			if ( (this.IsHurt() || this.Poisoned) && Bandaging == false )// Use bandages
			{
				Bandage m_Bandage = (Bandage)this.Backpack.FindItemByType( typeof ( Bandage ) );

				if ( m_Bandage != null )
				{
					Bandaging = true;

					if ( BandageContext.BeginHeal( this, this ) != null )
					m_Bandage.Consume();
					BandageTimer bt = new BandageTimer( this );
					bt.Start();
				}
			}

			DismountPlayer(this);
		}
		
		public virtual void InitWeapon() {}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual Mobile Focus
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

					if (value != null && !BlockCall)
					{
						Say(500131); // Thou wilt regret thine actions, swine!
						
						BlockCall = true;

						Timer.DelayCall (TimeSpan.FromSeconds(20.0), () => { BlockCall = false; });
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
						if (!Mounted && m_Focus.Mounted)
						{
							new KillableGuardMount().Rider = this;
						}
						
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
				
		public override void OnAfterDelete()
		{
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

			base.OnAfterDelete();
		}

        public override bool HandlesOnSpeech(Mobile from) { return true; }

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (e.Handled || !e.Mobile.InRange(Location, 18))
			{
				return;
			}

			if (e.Speech.ToLower().Contains("guard") || e.Speech.ToLower().Contains("help"))
			{
				Direction = GetDirectionTo(e.Mobile);

				if (e.Mobile.Combatant != null && IsEnemy(e.Mobile.Combatant as Mobile))
				{
					Say(speech[Utility.Random(speech.Length)]);
										
					m_Focus = e.Mobile.Combatant as Mobile;
				}
			}
		}

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Utility.RandomBool() || !m.Player) return;

            if (!Hidden && Utility.RandomDouble() < 0.35 && m.Alive && !m.Hidden)
            {           	
        	    if( InRange(m, 3))
        		{
					if (!BlockGreet)
					{
	                	Say(greet[Utility.Random(greet.Length)]);
						
						BlockGreet = true;
	
						Timer.DelayCall (TimeSpan.FromSeconds(5.0), () => { BlockGreet = false; });
					}
        	    }
            }
        }

		public override bool OnBeforeDeath()
		{
			PlayerMobile pm = Combatant as PlayerMobile;
			
			BaseCreature m = Combatant as BaseCreature;
			
			if (pm != null)
			{
				pm.Kills += 1;
			}
		   	else if (m != null && m.ControlMaster is PlayerMobile cm)
		   	{
		   		cm.Kills += 1;
			}
			
			if (Mounted)
			{
			   	KillableGuardMount mount = Mount as KillableGuardMount;
				
			   	if (mount != null)
			   	{
				   	mount.Rider = null;
					
					mount.Name = "a murdered guard's horse";
	
					mount.IdleTime = DateTime.UtcNow + TimeSpan.FromMinutes(60);
			   	}
			}
																		
			return true;
		}

		public static void Spawn(Mobile caller, Mobile target, int amount)
		{
			int tomake = amount;
			
			if (target != null && !target.Deleted)
			{
				while (tomake-- > 0)
				{
					caller.Region.MakeGuard(target, amount);
				}
			}
		}

		public BaseKillableGuard( Serial serial ) : base( serial )
		{
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
		
		private class BandageTimer : Timer
		{
			private BaseKillableGuard m_Owner;

			public BandageTimer( BaseKillableGuard owner ) : base( TimeSpan.FromSeconds( Utility.RandomMinMax(4,7) ) )
			{
				m_Owner = owner;
				
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Owner.Bandaging = false;
			}
		}
		
		public class AttackTimer : Timer
		{
			private readonly BaseKillableGuard m_Owner;

			public AttackTimer(BaseKillableGuard owner) : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
			{
				m_Owner = owner;
				
				Priority = TimerPriority.OneSecond;
			}

			public void DoOnTick()
			{
				OnTick();
			}

			protected override void OnTick()
			{
				if (m_Owner == null || m_Owner.Deleted)
				{
					Stop();
					
					return;
				}

				m_Owner.Criminal = false;
				
				m_Owner.Kills = 0;
				
				m_Owner.Stam = m_Owner.StamMax;

			   	GuardedRegion region = m_Owner.Region.GetRegion(typeof(GuardedRegion)) as GuardedRegion;
			   	
			   	if (region != null && region.Disabled)
			   	{
					Effects.SendLocationParticles(EffectItem.Create(m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
									
					m_Owner.Delete();

					Stop();
			   	}
				
				Mobile target = m_Owner.Focus;

				if (target == null)
				{
					Stop();

					return;
				}

				if (target.Deleted || !target.Alive)
				{
					m_Owner.Focus = null;
					
					Stop();
					
					return;
				}
				
				if (!m_Owner.IsEnemy(target) || region == null)
				{
					m_Owner.Focus = null;
					
					m_Owner.Combatant = null;
					
					Stop();
					
					return;
				}
				
				if (m_Owner.Weapon is Fists)
				{
					Effects.SendLocationParticles(EffectItem.Create(m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
									
					m_Owner.Delete();

					Stop();
					
					return;
				}

				if (m_Owner.Combatant != target)
				{
					m_Owner.Combatant = target;
				}

				if (!m_Owner.BlockTeleport)
				{
					TeleportTo(target);
				}
				
				if (Utility.Random(100) < 1 || (m_Owner.Hits < m_Owner.HitsMax / 4))
				{
					Spawn(m_Owner, target, Utility.RandomList(1, 2, 2, 2, 3));
				}

				if (target is BaseCreature)
				{
					((BaseCreature)target).NoKillAwards = true;
				}
	
				m_Owner.Focus = null;
				
				Stop();
			}

			private void TeleportTo(Mobile target)
			{
				if (0.8 > Utility.RandomDouble())
				{
					m_Owner.BlockTeleport = true;

					return;
				}
				
				if (m_Owner != null && !m_Owner.Deleted && target != null)
				{
					Point3D from = m_Owner.Location;
					
					Point3D to = target.Location;
	
					m_Owner.Location = to;
	
					Effects.SendLocationParticles(EffectItem.Create(from, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
					
					Effects.SendLocationParticles(EffectItem.Create(to, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
	
					m_Owner.PlaySound(0x1FE);
					
					m_Owner.BlockTeleport = true;
				
					Timer.DelayCall (TimeSpan.FromSeconds(30.0), () => EndLock());
				}
			}

			private void EndLock()
			{
				m_Owner.BlockTeleport = false;
			}
		}

		public class IdleTimer : Timer
		{
			private readonly BaseKillableGuard m_Owner;
			
			private readonly DateTime m_End;
			
			private int m_Stage;
			
			public IdleTimer(BaseKillableGuard owner) : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				m_Owner = owner;
				
				m_End = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5));

				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if (m_Owner.Deleted)
				{
					return;
				}
			   	
				GuardedRegion region = m_Owner.Region.GetRegion(typeof(GuardedRegion)) as GuardedRegion;
			   	
			   	if (region == null || (region != null && region.Disabled))
			   	{
					Effects.SendLocationParticles(EffectItem.Create(m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
														
					m_Owner.Delete();

					Stop();
			   	}

				if ((m_Stage++ % 20) == 0 || !m_Owner.Move(m_Owner.Direction))
				{
					m_Owner.Direction = (Direction)Utility.Random(8);
				}

				if (m_Owner.Mounted)
				{
					KillableGuardMount mount = m_Owner.Mount as KillableGuardMount;
			
					mount.Rider = null;
				
					mount.Delete();
				}

				if (m_End < DateTime.UtcNow)
				{
					Effects.SendLocationParticles(EffectItem.Create(m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
									
					m_Owner.Delete();
					
					Stop();
				}
				
				m_Owner.Focus = null;
				
				m_Owner.Combatant = null;

			   	IPooledEnumerable eable = m_Owner.GetMobilesInRange(10);
	
				foreach (Mobile m in eable)
				{
					if (m is BaseKillableGuard bkg && bkg != m_Owner && bkg.Focus != null && m_Owner.InLOS(bkg) && m_Owner.InLOS(m))
					{
						Stop();

						m_Owner.Focus = bkg.Focus;
						
						m_Owner.Combatant = bkg.Focus;
					}
					else if (m_Owner.IsEnemy(m))
					{
						Stop();

						m_Owner.Say(speech[Utility.Random(speech.Length)]);
						
						m_Owner.Focus = m;
						
						m_Owner.Combatant = m;
					}
				}
	
				eable.Free();			
			}
		}
		
		static string[] greet =
		{
			"To the fight!",
			"To arms!",
			"Where away!",
			"The battle awaits!",
			"Mind your weapons!",
			"I keep my eye on my enemy!",
			"Now is the time to fight!",
			"Nothing walks away!",
			"We must defend our land!",
			"Fight for our people!",
			"Out of my Way!",
			"Move aside!",
			"*Mumbles*",
			"Beware!",
			"Watch out!",
			"Coming through!"
		};

		static string[] speech =
		{
			"To the fight!",
			"To arms!",
			"Attack!",
			"The battle is on!",
			"To your weapons!",
			"I have my eye on my enemy!",
			"Time to die!",
			"Nothing walks away!",
			"I have sight of my enemy!",
			"You will not prevail!",
			"To my side!",
			"We must defend our land!",
			"Fight for our people!",
			"I see them!",
			"Destroy them all!"
		};
	}
}