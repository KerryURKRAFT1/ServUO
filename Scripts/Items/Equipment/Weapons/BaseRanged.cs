#region Header
// **********
// ServUO - BaseRanged.cs
// **********
#endregion

#region References
using System;

using Server.Mobiles;
using Server.Network;
using Server.Spells;
#endregion

namespace Server.Items
{
	public abstract class BaseRanged : BaseMeleeWeapon
	{
		private static readonly bool m_ArchersCanMove = Config.Get("Custom_Settings.ArchersCanMove", false);

		public abstract int EffectID { get; }
		public abstract Type AmmoType { get; }
		public abstract Item Ammo { get; }

		public override int DefHitSound { get { return 0x234; } }
		public override int DefMissSound { get { return 0x238; } }

		public override SkillName DefSkill { get { return SkillName.Archery; } }
		public override WeaponType DefType { get { return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootXBow; } }

		public override SkillName AccuracySkill { get { return SkillName.Archery; } }

		private Timer m_RecoveryTimer; // so we don't start too many timers
		private int m_Velocity;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Balanced
		{
            get { return Attributes.BalancedWeapon > 0; }
			set
			{
                if (value)
                    Attributes.BalancedWeapon = 1;
                else
                    Attributes.BalancedWeapon = 0;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Velocity
		{
			get { return m_Velocity; }
			set
			{
				m_Velocity = value;
				InvalidateProperties();
			}
		}

		public BaseRanged(int itemID)
			: base(itemID)
		{ }

		public BaseRanged(Serial serial)
			: base(serial)
		{ }

		public override TimeSpan OnSwing(Mobile attacker, IDamageable damageable)
		{
			bool canMove;
	
			if (Core.UOR)
			{
				canMove = m_ArchersCanMove || Core.TickCount - attacker.LastMoveTime >= (1000);
			}
			else
			{
				canMove = Core.TickCount - attacker.LastMoveTime >= (Core.SE ? 250 : Core.AOS ? 500 : 1000) ||
							(Core.AOS && WeaponAbility.GetCurrentAbility(attacker) is MovingShot);
			}
							
			if (canMove)
			{				
				// ======= SWING FOR UOR SPHERE-STYLE =======
				if (CanSwing(attacker) && attacker.HarmfulCheck(damageable))
				{
					TimeSpan swingDelay = GetDelay(attacker);
					
					if (attacker.InRange(damageable.Location, MaxRange))
					{
						Timer.DelayCall(swingDelay, () =>
						{
							if (CanSwing(attacker) && attacker.InRange(damageable.Location, MaxRange))
							{
								if (damageable is Mobile mobileTarget)
								{
									if (mobileTarget.Deleted || !mobileTarget.Alive || mobileTarget.Body.IsGhost)
										return;
								}
								else if (damageable is Item itemTarget)
								{
									// Add item controls here if needed
								}
	
								attacker.Send(new Swing(0, attacker, damageable));
	
								if (CheckHit(attacker, damageable))
								{
									OnHit(attacker, damageable, 1.0);
								}
								else
								{
									OnMiss(attacker, damageable);
								}
							}
							else
							{
								// Target out of range: do nothing, no miss, no attack!
							}
						});
					}
					else
					{
						// Target is out of range: do not swing, do not start swing timer
					}
	
					attacker.RevealingAction();
		
					return swingDelay;
				}
    			// ======= END =======
			}
			
			attacker.RevealingAction();

			return TimeSpan.FromSeconds(0.25);
		}
        
        // DLICK FOR SPHERE-UOR
        public override void OnDoubleClick(Mobile from)
        {
        	ClickToEquip.OnDoubleClick(from, this);
        }

		public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
		{
			if (AmmoType != null && attacker.Player && damageable is Mobile && !((Mobile)damageable).Player && (((Mobile)damageable).Body.IsAnimal || ((Mobile)damageable).Body.IsMonster) &&
				0.4 >= Utility.RandomDouble())
			{
				((Mobile)damageable).AddToBackpack(Ammo);
			}

			if (Core.ML && m_Velocity > 0)
			{
				int bonus = (int)attacker.GetDistanceToSqrt(damageable);

				if (bonus > 0 && m_Velocity > Utility.Random(100))
				{
					AOS.Damage(damageable, attacker, bonus * 3, 100, 0, 0, 0, 0);

					if (attacker.Player)
					{
						attacker.SendLocalizedMessage(1072794); // Your arrow hits its mark with velocity!
					}

					if (damageable is Mobile && ((Mobile)damageable).Player)
					{
						((Mobile)damageable).SendLocalizedMessage(1072795); // You have been hit by an arrow with velocity!
					}
				}
			}

			base.OnHit(attacker, damageable, damageBonus);
		}

        public override void OnMiss(Mobile attacker, IDamageable damageable)
		{
			if (attacker.Player && 0.4 >= Utility.RandomDouble())
			{
				if (Core.SE)
				{
					PlayerMobile p = attacker as PlayerMobile;

					if (p != null && AmmoType != null)
					{
						Type ammo = AmmoType;

						if (p.RecoverableAmmo.ContainsKey(ammo))
						{
							p.RecoverableAmmo[ammo]++;
						}
						else
						{
							p.RecoverableAmmo.Add(ammo, 1);
						}

						if (!p.Warmode)
						{
							if (m_RecoveryTimer == null)
							{
								m_RecoveryTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), p.RecoverAmmo);
							}

							if (!m_RecoveryTimer.Running)
							{
								m_RecoveryTimer.Start();
							}
						}
					}
				}
				else
				{
                    Point3D loc = damageable.Location;

					Ammo.MoveToWorld(
                        new Point3D(loc.X + Utility.RandomMinMax(0, 1), loc.Y + Utility.RandomMinMax(0, 1), loc.Z),
						damageable.Map);
				}
			}

			base.OnMiss(attacker, damageable);
		}

        public virtual bool OnFired(Mobile attacker, IDamageable damageable)
		{
			WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);
			
			// Respect special moves that use no ammo
			if (ability != null && ability.ConsumeAmmo == false)
			{
				return true;
			}

			if (attacker is PlayerMobile)
			{
				BaseQuiver quiver = attacker.FindItemOnLayer(Layer.Cloak) as BaseQuiver;
				Container pack = attacker.Backpack;

                int lowerAmmo = AosAttributes.GetValue(attacker, AosAttribute.LowerAmmoCost);

                if (quiver == null || Utility.Random(100) >= lowerAmmo)
				{
					// consume ammo
					if (quiver != null && quiver.ConsumeTotal(AmmoType, 1))
					{
						quiver.InvalidateWeight();
					}
					else if (pack == null || !pack.ConsumeTotal(AmmoType, 1))
					{
						return false;
					}
				}
				else if (quiver.FindItemByType(AmmoType) == null && (pack == null || pack.FindItemByType(AmmoType) == null))
				{
					// lower ammo cost should not work when we have no ammo at all
					return false;
				}
			}

            attacker.MovingEffect(damageable, EffectID, 18, 1, false, false);

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(4); // version

			writer.Write(m_Velocity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
                case 4:
				case 3:
					{
                        if (version == 3 && reader.ReadBool())
                            Attributes.BalancedWeapon = 1;

						m_Velocity = reader.ReadInt();

						goto case 2;
					}
				case 2:
				case 1:
					{
						break;
					}
				case 0:
					{
						/*m_EffectID =*/
						reader.ReadInt();
						break;
					}
			}

			if (version < 2)
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}
