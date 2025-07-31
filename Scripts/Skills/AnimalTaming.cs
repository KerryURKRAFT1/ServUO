#region Header
// **********
// ServUO - AnimalTaming.cs
// **********
#endregion

#region References
using System;
using System.Collections;

using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Spellweaving;
using Server.Targeting;
#endregion

namespace Server.SkillHandlers
{
	public class AnimalTaming
	{
		private static readonly Hashtable m_BeingTamed = new Hashtable();
		private static bool m_DisableMessage;
		public static bool DisableMessage { get { return m_DisableMessage; } set { m_DisableMessage = value; } }

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.AnimalTaming].Callback = OnUse;
		}

    	public static TimeSpan OnUse(Mobile m)
        {
    		if (!SkillRegistry.Contains(m))
    		{
	    		SkillRegistry.Add(m);
	        	
	        	if (!TriggerSkill(m))
	        	{
		    		SkillRegistry.Remove(m);
	        	}
    		}
    		else if (SkillRegistry.WaitMsg)
    		{
                m.SendMessage("You must wait to perform another action.");
    		}
        	
        	return TimeSpan.Zero;
        }

    	public static bool TriggerSkill(Mobile m)
		{
			m.RevealingAction();

			m.Target = new InternalTarget();
			
			m.RevealingAction();

			if (!m_DisableMessage)
			{
				m.SendLocalizedMessage(502789); // Tame which animal?
			}

			return true;
		}

		public static bool CheckMastery(Mobile tamer, BaseCreature creature)
		{
			BaseCreature familiar = (BaseCreature)SummonFamiliarSpell.Table[tamer];

			if (familiar != null && !familiar.Deleted && familiar is DarkWolfFamiliar)
			{
				if (creature is DireWolf || creature is GreyWolf || creature is TimberWolf || creature is WhiteWolf ||
					creature is BakeKitsune)
				{
					return true;
				}
			}

			return false;
		}

		public static bool MustBeSubdued(BaseCreature bc)
		{
			if (bc.Owners.Count > 0)
			{
				return false;
			} //Checks to see if the animal has been tamed before
			return bc.SubdueBeforeTame && (bc.Hits > ((double)bc.HitsMax / 10));
		}

		public static void ScaleStats(BaseCreature bc, double scalar)
		{
			if (bc.RawStr > 0)
			{
				bc.RawStr = (int)Math.Max(1, bc.RawStr * scalar);
			}

			if (bc.RawDex > 0)
			{
				bc.RawDex = (int)Math.Max(1, bc.RawDex * scalar);
			}

			if (bc.RawInt > 0)
			{
				bc.RawInt = (int)Math.Max(1, bc.RawInt * scalar);
			}

			if (bc.HitsMaxSeed > 0)
			{
				bc.HitsMaxSeed = (int)Math.Max(1, bc.HitsMaxSeed * scalar);
				bc.Hits = bc.Hits;
			}

			if (bc.StamMaxSeed > 0)
			{
				bc.StamMaxSeed = (int)Math.Max(1, bc.StamMaxSeed * scalar);
				bc.Stam = bc.Stam;
			}
		}

		public static void ScaleSkills(BaseCreature bc, double scalar)
		{
			ScaleSkills(bc, scalar, scalar);
		}

		public static void ScaleSkills(BaseCreature bc, double scalar, double capScalar)
		{
			for (int i = 0; i < bc.Skills.Length; ++i)
			{
				bc.Skills[i].Base *= scalar;

				bc.Skills[i].Cap = Math.Max(100.0, bc.Skills[i].Cap * capScalar);

				if (bc.Skills[i].Base > bc.Skills[i].Cap)
				{
					bc.Skills[i].Cap = bc.Skills[i].Base;
				}
			}
		}

		private class InternalTarget : Target
		{
					// CODICE MODIFICATO PER UOR PER TAMARE DISTANZA 4 CASELLE
			//public InternalTarget()
			//	: base(Core.AOS ? 3 : 2, false, TargetFlags.None)
			public InternalTarget() : base(Core.AOS ? 3 : (Core.UOR ? 4 : 2), false, TargetFlags.None)
			{ }

			protected override void OnTarget(Mobile from, object targeted)
			{
				from.RevealingAction();

				if (targeted is Mobile)
				{
					if (targeted is BaseCreature)
					{
						BaseCreature creature = (BaseCreature)targeted;

						new SkillTimer(from, creature, SkillRegistry.Delay).Start();
						
						return;
					}
					else
					{
						((Mobile)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502469, from.NetState);
							// That being cannot be tamed.
					}
				}
				else
				{
					from.SendLocalizedMessage(502801); // You can't tame that!
				}

                SkillRegistry.Remove(from);
			}

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                SkillRegistry.Remove(from);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1076203); // Target out of range.	
				SkillRegistry.Remove(from);
            }

	        protected override void OnTargetOutOfLOS(Mobile from, object o)
	        {
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500237);// Target can not be seen.
                SkillRegistry.Remove(from);
	        }

	        private class SkillTimer : Timer
			{
				private readonly Mobile m_Owner;
				private readonly BaseCreature m_Creature;
	
	            public SkillTimer(Mobile owner, BaseCreature creature, TimeSpan delay) : base(delay)
				{
					m_Owner = owner;
					m_Creature = creature;
	
					Priority = TimerPriority.TwoFiftyMS;
				}
	
				public virtual void ResetPacify(object obj)
				{
					if (obj is BaseCreature)
					{
						((BaseCreature)obj).BardPacified = true;
					}
				}
	
				protected override void OnTick()
				{
					if (!m_Creature.Tamable)
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049655, m_Owner.NetState);
							// That creature cannot be tamed.
					}
					else if (m_Creature.Controlled)
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502804, m_Owner.NetState);
							// That animal looks tame already.
					}
					else if (m_Owner.Female && !m_Creature.AllowFemaleTamer)
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049653, m_Owner.NetState);
							// That creature can only be tamed by males.
					}
					else if (!m_Owner.Female && !m_Creature.AllowMaleTamer)
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049652, m_Owner.NetState);
							// That creature can only be tamed by females.
					}
					else if (m_Owner.Followers + m_Creature.ControlSlots > m_Owner.FollowersMax)
					{
						m_Owner.SendLocalizedMessage(1049611); // You have too many followers to tame that creature.
					}
					else if (m_Creature.Owners.Count >= BaseCreature.MaxOwners && !m_Creature.Owners.Contains(m_Owner))
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1005615, m_Owner.NetState);
							// This animal has had too many owners and is too upset for you to tame.
					}
					else if (MustBeSubdued(m_Creature))
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1054025, m_Owner.NetState);
							// You must subdue this creature before you can tame it!
					}
					else if (CheckMastery(m_Owner, m_Creature) || m_Owner.Skills[SkillName.AnimalTaming].Value >= m_Creature.MinTameSkill)
					{
						FactionWarHorse warHorse = m_Creature as FactionWarHorse;

						if (warHorse != null)
						{
							Faction faction = Faction.Find(m_Owner);

							if (faction == null || faction != warHorse.Faction)
							{
								m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042590, m_Owner.NetState);
									// You cannot tame this creature.
								return;
							}
						}

						if (m_BeingTamed.Contains(m_Creature))
						{
							m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502802, m_Owner.NetState);
								// Someone else is already taming this.
						}
						else if (m_Creature.CanAngerOnTame && 0.95 >= Utility.RandomDouble())
						{
							m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502805, m_Owner.NetState);
								// You seem to anger the beast!
							m_Creature.PlaySound(m_Creature.GetAngerSound());
							m_Creature.Direction = m_Creature.GetDirectionTo(m_Owner);

							if (m_Creature.BardPacified && Utility.RandomDouble() > .24)
							{
								Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerStateCallback(ResetPacify), m_Creature);
							}
							else
							{
								m_Creature.BardEndTime = DateTime.UtcNow;
							}

							m_Creature.BardPacified = false;

							if (m_Creature.AIObject != null)
							{
								m_Creature.AIObject.DoMove(m_Creature.Direction);
							}

							if (m_Owner is PlayerMobile &&
								!(((PlayerMobile)m_Owner).HonorActive ||
								  TransformationSpellHelper.UnderTransformation(m_Owner, typeof(EtherealVoyageSpell))))
							{
								m_Creature.Combatant = m_Owner;
							}
						}
						else
						{
							m_BeingTamed[m_Creature] = m_Owner;

							m_Owner.LocalOverheadMessage(MessageType.Emote, 0x59, 1010597); // You start to tame the creature.
							m_Owner.NonlocalOverheadMessage(MessageType.Emote, 0x59, 1010598); // *begins taming a creature.*

							new InternalTimer(m_Owner, m_Creature, Utility.Random(3, 2)).Start();
														
							return;
						}
					}
					else
					{
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502806, m_Owner.NetState);
							// You have no chance of taming this creature.
					}						

	                SkillRegistry.Remove(m_Owner);
				}
			}

	        private class InternalTimer : Timer
			{
				private readonly Mobile m_Tamer;
				private readonly BaseCreature m_Creature;
				private readonly int m_MaxCount;
				private readonly DateTime m_StartTime;
				private int m_Count;
				private bool m_Paralyzed;

				public InternalTimer(Mobile tamer, BaseCreature creature, int count)
					: base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), count)
				{
					m_Tamer = tamer;
					m_Creature = creature;
					m_MaxCount = count;
					m_Paralyzed = creature.Paralyzed;
					m_StartTime = DateTime.UtcNow;
					Priority = TimerPriority.TwoFiftyMS;
				}

				protected override void OnTick()
				{
					bool end = true;
					m_Count++;

					DamageEntry de = m_Creature.FindMostRecentDamageEntry(false);
					bool alreadyOwned = m_Creature.Owners.Contains(m_Tamer);

					// CODICE MODIFICATO PER UOR MENTRE SI TAMA DISTANZA 7 CASELLE

					//if (!m_Tamer.InRange(m_Creature, Core.AOS ? 7 : 6))
					if (!m_Tamer.InRange(m_Creature, Core.AOS ? 7 : (Core.UOR ? 7 : 6)))

					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502795, m_Tamer.NetState);
							// You are too far away to continue taming.
					}
					else if (!m_Tamer.CheckAlive())
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502796, m_Tamer.NetState);
							// You are dead, and cannot continue taming.
					}
					else if (!m_Tamer.CanSee(m_Creature)|| !m_Tamer.InLOS(m_Creature) /* || !CanPath()*/)
					{
						m_BeingTamed.Remove(m_Creature);
						m_Tamer.SendLocalizedMessage(1049654);
							// You do not have a clear path to the animal you are taming, and must cease your attempt.
					}
					else if (!m_Creature.Tamable)
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049655, m_Tamer.NetState);
							// That creature cannot be tamed.
					}
					else if (m_Creature.Controlled)
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502804, m_Tamer.NetState);
							// That animal looks tame already.
					}
					else if (m_Creature.Owners.Count >= BaseCreature.MaxOwners && !m_Creature.Owners.Contains(m_Tamer))
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1005615, m_Tamer.NetState);
							// This animal has had too many owners and is too upset for you to tame.
					}
					else if (MustBeSubdued(m_Creature))
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1054025, m_Tamer.NetState);
							// You must subdue this creature before you can tame it!
					}
					else if (de != null && de.LastDamage > m_StartTime)
					{
						m_BeingTamed.Remove(m_Creature);
						m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502794, m_Tamer.NetState);
							// The animal is too angry to continue taming.
					}
					else if (m_Count < m_MaxCount)
					{
						m_Tamer.RevealingAction();

						switch (Utility.Random(3))
						{
							case 0:
								m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(502790, 4));
								break;
							case 1:
								m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(1005608, 6));
								break;
							case 2:
								m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(1010593, 4));
								break;
						}

						if (!alreadyOwned) // Passively check animal lore for gain
						{
							m_Tamer.CheckTargetSkill(SkillName.AnimalLore, m_Creature, 0.0, 120.0);
						}

						if (m_Creature.Paralyzed)
						{
							m_Paralyzed = true;
						}
						
						end = false;
					}
					else
					{
						m_Tamer.RevealingAction();

						m_BeingTamed.Remove(m_Creature);

						if (m_Creature.Paralyzed)
						{
							m_Paralyzed = true;
						}

						if (!alreadyOwned) // Passively check animal lore for gain
						{
							m_Tamer.CheckTargetSkill(SkillName.AnimalLore, m_Creature, 0.0, 120.0);
						}

						double minSkill = m_Creature.MinTameSkill + (m_Creature.Owners.Count * 6.0);

						if (minSkill > -24.9 && CheckMastery(m_Tamer, m_Creature))
						{
							minSkill = -24.9; // 50% at 0.0?
						}

						minSkill += 24.9;

						minSkill += XmlMobFactions.GetScaledFaction(m_Tamer, m_Creature, -25, 25, -0.001);

						if (CheckMastery(m_Tamer, m_Creature) || alreadyOwned ||
							m_Tamer.CheckTargetSkill(SkillName.AnimalTaming, m_Creature, minSkill - 25.0, minSkill + 25.0))
						{
							if (m_Creature.Owners.Count == 0) // First tame
							{
								if (m_Creature is GreaterDragon)
								{
									ScaleSkills(m_Creature, 0.72, 0.90); // 72% of original skills trainable to 90%
									m_Creature.Skills[SkillName.Magery].Base = m_Creature.Skills[SkillName.Magery].Cap;
										// Greater dragons have a 90% cap reduction and 90% skill reduction on magery
								}
								else if (m_Paralyzed)
								{
									ScaleSkills(m_Creature, 0.86); // 86% of original skills if they were paralyzed during the taming
								}
								else
								{
									ScaleSkills(m_Creature, 0.90); // 90% of original skills
								}

								if (m_Creature.StatLossAfterTame)
								{
									ScaleStats(m_Creature, 0.50);
								}

                                foreach (Skill sk in m_Creature.Skills)
                                {
                                    if (sk.Base > 100)
                                        sk.Cap = sk.Base;
                                    else
                                        sk.Cap = 100;
                                }
							}

							if (alreadyOwned)
							{
								m_Tamer.SendLocalizedMessage(502797); // That wasn't even challenging.
							}
							else
							{
								m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502799, m_Tamer.NetState);
									// It seems to accept you as master.
								m_Creature.Owners.Add(m_Tamer);
							}

							m_Creature.SetControlMaster(m_Tamer);
							m_Creature.IsBonded = false;

                            m_Creature.OnAfterTame(m_Tamer);
						}
						else
						{
							m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502798, m_Tamer.NetState);
								// You fail to tame the creature.
						}
					}
					
					if (end)
					{
						SkillRegistry.Remove(m_Tamer);
						Stop();
					}					
				}

				private bool CanPath()
				{
					IPoint3D p = m_Tamer;

					if (p == null)
					{
						return false;
					}

					if (m_Creature.InRange(new Point3D(p), 1))
					{
						return true;
					}

					MovementPath path = new MovementPath(m_Creature, new Point3D(p));
					return path.Success;
				}
			}
		}
	}
}