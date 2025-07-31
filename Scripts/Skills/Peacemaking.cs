using System;
using Server.Engines.ConPVP;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Network;
using Server.Spells;

namespace Server.SkillHandlers
{
	public class Peacemaking
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Peacemaking].Callback = OnUse;
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

			if (BaseInstrument.PickInstrument(m, OnPickedInstrument))
			{
				return false;
			}

			return true;
		}

		public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
		{
			from.RevealingAction();
			from.SendLocalizedMessage(1049525); // Whom do you wish to calm?
			from.Target = new InternalTarget(from, instrument);
		}

		public class InternalTarget : Target
		{
			private readonly BaseInstrument m_Instrument;

			public InternalTarget(Mobile from, BaseInstrument instrument) : base(BaseInstrument.GetBardRange(from, SkillName.Peacemaking), false, TargetFlags.None)
			{
				m_Instrument = instrument;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				from.RevealingAction();
				
				Mobile m = targeted as Mobile;

				if (m == null)
				{
					from.SendLocalizedMessage(1049528); // You cannot calm that!
				}
				else if (from.Region.IsPartOf(typeof(SafeZone)))
				{
					from.SendMessage("You may not peacemake in this area.");
				}
				else if (((Mobile)targeted).Region.IsPartOf(typeof(SafeZone)))
				{
					from.SendMessage("You may not peacemake there.");
				}
				else if (!m_Instrument.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
				}
				else
				{					
					new SkillTimer(from, m, m_Instrument, SkillRegistry.Delay).Start();
					
					return;				
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
		}

        private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;
			private readonly Mobile m_Targ;
			private readonly BaseInstrument m_Instrument;

            public SkillTimer(Mobile owner, Mobile targ, BaseInstrument instrument, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;
				m_Instrument = instrument;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
	            int masteryBonus = 0;
	
	            if (m_Owner is PlayerMobile)
	                masteryBonus = Spells.SkillMasteries.BardSpell.GetMasteryBonus((PlayerMobile)m_Owner, SkillName.Peacemaking);
	
				if (m_Targ == m_Owner)
				{
					// Standard mode : reset combatants for everyone in the area
					if (!BaseInstrument.CheckMusicianship(m_Owner))
					{
						m_Owner.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
						m_Instrument.PlayInstrumentBadly(m_Owner);
						m_Instrument.ConsumeUse(m_Owner);
					}
					else if (!m_Owner.CheckSkill(SkillName.Peacemaking, 0.0, 120.0))
					{
						m_Owner.SendLocalizedMessage(500613); // You attempt to calm everyone, but fail.
						m_Instrument.PlayInstrumentBadly(m_Owner);
						m_Instrument.ConsumeUse(m_Owner);	
					}
					else
					{
						m_Instrument.PlayInstrumentWell(m_Owner);
						m_Instrument.ConsumeUse(m_Owner);
	
						Map map = m_Owner.Map;
	
						if (map != null)
						{
							int range = BaseInstrument.GetBardRange(m_Owner, SkillName.Peacemaking);
	
							bool calmed = false;
	
							foreach (Mobile m in m_Owner.GetMobilesInRange(range))
							{
								if ((m is BaseCreature && ((BaseCreature)m).Uncalmable) ||
									(m is BaseCreature && ((BaseCreature)m).AreaPeaceImmune) || m == m_Owner || !m_Owner.CanBeHarmful(m, false))
								{
									continue;
								}
	
								calmed = true;
	
								m.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
								m.Combatant = null;
								m.Warmode = false;
	
								if (m is BaseCreature && !((BaseCreature)m).BardPacified)
								{
									((BaseCreature)m).Pacify(m_Owner, DateTime.UtcNow + TimeSpan.FromSeconds(1.0));

									//Disturb mod
									ISpell spell = ((BaseCreature)m).Spell;
									
									if (spell != null && spell.OnParalyze())
					            	{
									    ((Spell)spell).Disturb(DisturbType.Paralyzed);
									}
									//end
								}
							}
	
							if (!calmed)
							{
								m_Owner.SendLocalizedMessage(1049648); // You play hypnotic music, but there is nothing in range for you to calm.
							}
							else
							{
								m_Owner.SendLocalizedMessage(500615); // You play your hypnotic music, stopping the battle.
							}
						}
					}
				}
				else
				{	
					if (!m_Owner.CanBeHarmful(m_Targ, false) && m_Owner.AccessLevel == AccessLevel.Player)
					{
						m_Owner.SendLocalizedMessage(1049528); // You cannot calm that!
					}
					else if (m_Targ is BaseCreature && ((BaseCreature)m_Targ).Uncalmable)
					{
						m_Owner.SendLocalizedMessage(1049526); // You have no chance of calming that creature.
					}
					else if (m_Targ is BaseCreature && ((BaseCreature)m_Targ).BardPacified)
					{
						m_Owner.SendLocalizedMessage(1049527); // That creature is already being calmed.
					}
					else if (!BaseInstrument.CheckMusicianship(m_Owner))
					{
						m_Owner.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
						m_Instrument.PlayInstrumentBadly(m_Owner);
						m_Instrument.ConsumeUse(m_Owner);
					}
					else
					{
						double diff = m_Instrument.GetDifficultyFor(m_Targ) - 10.0;
						double music = m_Owner.Skills[SkillName.Musicianship].Value;
	
						diff += XmlMobFactions.GetScaledFaction(m_Owner, m_Targ, -25, 25, -0.001);
	
						if (music > 100.0)
						{
							diff -= (music - 100.0) * 0.5;
						}
	
	                    if (masteryBonus > 0)
	                        diff -= (diff * ((double)masteryBonus / 100));
	
						if (!m_Owner.CheckTargetSkill(SkillName.Peacemaking, m_Targ, diff - 25.0, diff + 25.0))
						{
							m_Owner.SendLocalizedMessage(1049531); // You attempt to calm your target, but fail.
							m_Instrument.PlayInstrumentBadly(m_Owner);
							m_Instrument.ConsumeUse(m_Owner);
						}
						else
						{
							m_Instrument.PlayInstrumentWell(m_Owner);
							m_Instrument.ConsumeUse(m_Owner);
	
							if (m_Targ is BaseCreature)
							{
								BaseCreature bc = (BaseCreature)m_Targ;
	
								m_Owner.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.
	
								m_Targ.Combatant = null;
								m_Targ.Warmode = false;
	
								double seconds = 100 - (diff / 1.5);
	
								if (seconds > 120)
								{
									seconds = 120;
								}
								else if (seconds < 10)
								{
									seconds = 10;
								}
	
								bc.Pacify(m_Owner, DateTime.UtcNow + TimeSpan.FromSeconds(seconds));
	
	                            #region Bard Mastery Quest
	                            if (m_Owner is PlayerMobile)
	                            {
	                                BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)m_Owner, typeof(TheBeaconOfHarmonyQuest));
	
	                                if (quest != null)
	                                {
	                                    foreach (BaseObjective objective in quest.Objectives)
	                                        objective.Update(bc);
	                                }
	                            }
	                            #endregion
							}
							else
							{
								m_Owner.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.
	
								m_Targ.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
								m_Targ.Combatant = null;
								m_Targ.Warmode = false;
							}
						}
					}
				}

				SkillRegistry.Remove(m_Owner);
			}
		}
	}
}