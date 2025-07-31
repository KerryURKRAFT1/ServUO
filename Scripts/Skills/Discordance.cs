using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Discordance
	{
		private static readonly Hashtable m_Table = new Hashtable();

		public static void Initialize()
		{
//			SkillInfo.Table[(int)SkillName.Discordance].Callback = OnUse;
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
			from.SendLocalizedMessage(1049541); // Choose the target for your song of discordance.
			from.Target = new DiscordanceTarget(from, instrument);
		}

		public static bool GetEffect(Mobile targ, ref int effect)
		{
			DiscordanceInfo info = m_Table[targ] as DiscordanceInfo;

			if (info == null)
			{
				return false;
			}

			effect = info.m_Effect;
			return true;
		}

		private static void ProcessDiscordance(DiscordanceInfo info)
		{
			Mobile from = info.m_From;
			Mobile targ = info.m_Creature;
			bool ends = false;

			// According to uoherald bard must remain alive, visible, and 
			// within range of the target or the effect ends in 15 seconds.
			if (!targ.Alive || targ.Deleted || !from.Alive || from.Hidden)
			{
				ends = true;
			}
			else
			{
				int range = (int)targ.GetDistanceToSqrt(from);
				int maxRange = BaseInstrument.GetBardRange(from, SkillName.Discordance);

				if (from.Map != targ.Map || range > maxRange)
				{
					ends = true;
				}
			}

			if (ends && info.m_Ending && info.m_EndTime < DateTime.UtcNow)
			{
				if (info.m_Timer != null)
				{
					info.m_Timer.Stop();
				}

				info.Clear();
				m_Table.Remove(targ);
			}
			else
			{
				if (ends && !info.m_Ending)
				{
					info.m_Ending = true;
					info.m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
				}
				else if (!ends)
				{
					info.m_Ending = false;
					info.m_EndTime = DateTime.UtcNow;
				}

				targ.FixedEffect(0x376A, 1, 32);
			}
		}

		public class DiscordanceTarget : Target
		{
			private readonly BaseInstrument m_Instrument;

			public DiscordanceTarget(Mobile from, BaseInstrument inst) : base(BaseInstrument.GetBardRange(from, SkillName.Discordance), false, TargetFlags.None)
			{
				m_Instrument = inst;
			}

			protected override void OnTarget(Mobile from, object target)
			{
				from.RevealingAction();

				if (!m_Instrument.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
				}
				else if (target is Mobile)
				{
					Mobile targ = (Mobile)target;

					if (targ == from ||
						(targ is BaseCreature && (((BaseCreature)targ).BardImmune || !from.CanBeHarmful(targ, false)) &&
						 ((BaseCreature)targ).ControlMaster != from) && from.AccessLevel == AccessLevel.Player)
					{
						from.SendLocalizedMessage(1049535); // A song of discord would have no effect on that.
					}
					else if (m_Table.Contains(targ)) //Already discorded
					{
						from.SendLocalizedMessage(1049537); // Your target is already in discord.
					}
					else if (!targ.Player || (from is BaseCreature) && ((BaseCreature)from).CanDiscord)
					{
						double diff = m_Instrument.GetDifficultyFor(targ) - 10.0;
						double music = from.Skills[SkillName.Musicianship].Value;
                        int masteryBonus = 0;

						diff += XmlMobFactions.GetScaledFaction(from, targ, -25, 25, -0.001);

						if (music > 100.0)
						{
							diff -= (music - 100.0) * 0.5;
						}

                        if (from is PlayerMobile)
                        {
                            masteryBonus = Spells.SkillMasteries.BardSpell.GetMasteryBonus((PlayerMobile)from, SkillName.Discordance);
                        }

                        if (masteryBonus > 0)
                        {
                            diff -= (diff * ((double)masteryBonus / 100));
                        }

						if (!BaseInstrument.CheckMusicianship(from))
						{
							from.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);
						}
						else if (from.CheckTargetSkill(SkillName.Discordance, target, diff - 25.0, diff + 25.0))
						{
							new SkillTimer(from, targ, m_Instrument, SkillRegistry.Delay).Start();
							
							return;
						}
						else
						{
							from.SendLocalizedMessage(1049540); // You fail to disrupt your target
							m_Instrument.PlayInstrumentBadly(from);
							m_Instrument.ConsumeUse(from);
						}
					}
					else
					{
						m_Instrument.PlayInstrumentBadly(from);
					}
				}
				else
				{
					from.SendLocalizedMessage(1049535); // A song of discord would have no effect on that.
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
				m_Owner.SendLocalizedMessage(1049539); // You play the song surpressing your targets strength
				m_Instrument.PlayInstrumentWell(m_Owner);
				m_Instrument.ConsumeUse(m_Owner);

				ArrayList mods = new ArrayList();
				int effect;
				double scalar;

				if (Core.AOS)
				{
					double discord = m_Owner.Skills[SkillName.Discordance].Value;

					if (discord > 100.0)
					{
						effect = -20 + (int)((discord - 100.0) / -2.5);
					}
					else
					{
						effect = (int)(discord / -5.0);
					}

					if (Core.SE && BaseInstrument.GetBaseDifficulty(m_Targ) >= 160.0)
					{
						effect /= 2;
					}

					scalar = effect * 0.01;

					mods.Add(new ResistanceMod(ResistanceType.Physical, effect));
					mods.Add(new ResistanceMod(ResistanceType.Fire, effect));
					mods.Add(new ResistanceMod(ResistanceType.Cold, effect));
					mods.Add(new ResistanceMod(ResistanceType.Poison, effect));
					mods.Add(new ResistanceMod(ResistanceType.Energy, effect));

					for (int i = 0; i < m_Targ.Skills.Length; ++i)
					{
						if (m_Targ.Skills[i].Value > 0)
						{
							mods.Add(new DefaultSkillMod((SkillName)i, true, m_Targ.Skills[i].Value * scalar));
						}
					}
				}
				else
				{
					effect = (int)(m_Owner.Skills[SkillName.Discordance].Value / -5.0);
					scalar = effect * 0.01;

					mods.Add(new StatMod(StatType.Str, "DiscordanceStr", (int)(m_Targ.RawStr * scalar), TimeSpan.Zero));
					mods.Add(new StatMod(StatType.Int, "DiscordanceInt", (int)(m_Targ.RawInt * scalar), TimeSpan.Zero));
					mods.Add(new StatMod(StatType.Dex, "DiscordanceDex", (int)(m_Targ.RawDex * scalar), TimeSpan.Zero));

					for (int i = 0; i < m_Targ.Skills.Length; ++i)
					{
						if (m_Targ.Skills[i].Value > 0)
						{
							mods.Add(new DefaultSkillMod((SkillName)i, true, m_Targ.Skills[i].Value * scalar));
						}
					}
				}

				DiscordanceInfo info = new DiscordanceInfo(m_Owner, m_Targ, Math.Abs(effect), mods);
				info.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.25), ProcessDiscordance, info);

                #region Bard Mastery Quest
                if (m_Owner is PlayerMobile)
                {
                    BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)m_Owner, typeof(WieldingTheSonicBladeQuest));

                    if (quest != null)
                    {
                        foreach (BaseObjective objective in quest.Objectives)
                            objective.Update(m_Targ);
                    }
                }
                #endregion

				m_Table[m_Targ] = info;
				
				SkillRegistry.Remove(m_Owner);
			}
		}

        private class DiscordanceInfo
		{
			public readonly Mobile m_From;
			public readonly Mobile m_Creature;
			public readonly int m_Effect;
			public readonly ArrayList m_Mods;
			public DateTime m_EndTime;
			public bool m_Ending;
			public Timer m_Timer;

			public DiscordanceInfo(Mobile from, Mobile creature, int effect, ArrayList mods)
			{
				m_From = from;
				m_Creature = creature;
				m_EndTime = DateTime.UtcNow;
				m_Ending = false;
				m_Effect = effect;
				m_Mods = mods;

				Apply();
			}

			public void Apply()
			{
				for (int i = 0; i < m_Mods.Count; ++i)
				{
					object mod = m_Mods[i];

					if (mod is ResistanceMod)
					{
						m_Creature.AddResistanceMod((ResistanceMod)mod);
					}
					else if (mod is StatMod)
					{
						m_Creature.AddStatMod((StatMod)mod);
					}
					else if (mod is SkillMod)
					{
						m_Creature.AddSkillMod((SkillMod)mod);
					}
				}
			}

			public void Clear()
			{
				for (int i = 0; i < m_Mods.Count; ++i)
				{
					object mod = m_Mods[i];

					if (mod is ResistanceMod)
					{
						m_Creature.RemoveResistanceMod((ResistanceMod)mod);
					}
					else if (mod is StatMod)
					{
						m_Creature.RemoveStatMod(((StatMod)mod).Name);
					}
					else if (mod is SkillMod)
					{
						m_Creature.RemoveSkillMod((SkillMod)mod);
					}
				}
			}
		}
	}
}