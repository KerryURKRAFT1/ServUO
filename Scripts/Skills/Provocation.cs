using System;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Provocation
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Provocation].Callback = OnUse;
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
            from.SendLocalizedMessage(501587); // Whom do you wish to incite?
            from.Target = new InternalFirstTarget(from, instrument);
        }

        public class InternalFirstTarget : Target
        {
            private readonly BaseInstrument m_Instrument;

            public InternalFirstTarget(Mobile from, BaseInstrument instrument)
                : base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
            {
                m_Instrument = instrument;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                if (targeted is BaseCreature && (from.CanBeHarmful((Mobile)targeted, true) || from.AccessLevel > AccessLevel.Player))
                {
                    BaseCreature creature = (BaseCreature)targeted;

                    if (!m_Instrument.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                    }
                    else if (from is PlayerMobile && creature.Controlled)
                    {
                        from.SendLocalizedMessage(501590); // They are too loyal to their master to be provoked.
                    }
                    else if (creature.IsParagon && BaseInstrument.GetBaseDifficulty(creature) >= 160.0)
                    {
                        from.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                    }
                    else
                    {
                        from.RevealingAction();
                        m_Instrument.PlayInstrumentWell(from);
                        from.SendLocalizedMessage(1008085);
                        // You play your music and your target becomes angered.  Whom do you wish them to attack?
                        from.Target = new InternalSecondTarget(from, m_Instrument, creature);
                        return;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(501589); // You can't incite that!
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

        public class InternalSecondTarget : Target
        {
            private readonly BaseCreature m_Creature;
            private readonly BaseInstrument m_Instrument;

            public InternalSecondTarget(Mobile from, BaseInstrument instrument, BaseCreature creature)
                : base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
            {
                m_Instrument = instrument;
                m_Creature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                if (targeted is BaseCreature || (from is BaseCreature && ((BaseCreature)from).CanProvoke))
                {			
                	new SkillTimer(from, targeted, m_Creature, m_Instrument, SkillRegistry.Delay).Start();

                	return;
                }
                else
                {
                    from.SendLocalizedMessage(501589); // You can't incite that!
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
			private readonly object m_Targ;
			private readonly BaseCreature m_Creature;
			private readonly BaseInstrument m_Instrument;

            public SkillTimer(Mobile owner, object targ, BaseCreature creature, BaseInstrument instrument, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;
				m_Targ = targ;
				m_Instrument = instrument;
				m_Creature = creature;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
                BaseCreature creature = m_Targ as BaseCreature;
                Mobile target = m_Targ as Mobile;

                bool questTargets = QuestTargets(creature, m_Owner);

                if (!m_Instrument.IsChildOf(m_Owner.Backpack))
                {
                    m_Owner.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                }
                else if (m_Creature.Unprovokable)
                {
                    m_Owner.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                }
                else if (creature != null && creature.Unprovokable && !(creature is DemonKnight) && !questTargets)
                {
                    m_Owner.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                }
                else if (m_Creature.Map != target.Map ||
                         !m_Creature.InRange(target, BaseInstrument.GetBardRange(m_Owner, SkillName.Provocation)))
                {
                    m_Owner.SendLocalizedMessage(1049450);
                    // The creatures you are trying to provoke are too far away from each other for your music to have an effect.
                }
                else if (m_Creature != target)
                {
                    double diff = ((m_Instrument.GetDifficultyFor(m_Creature) + m_Instrument.GetDifficultyFor(target)) * 0.5) - 5.0;
                    double music = m_Owner.Skills[SkillName.Musicianship].Value;
                    int masteryBonus = 0;

                    if (m_Owner is PlayerMobile)
                        masteryBonus = Spells.SkillMasteries.BardSpell.GetMasteryBonus((PlayerMobile)m_Owner, SkillName.Provocation);

                    if (masteryBonus > 0)
                        diff -= (diff * ((double)masteryBonus / 100));

                    diff += (XmlMobFactions.GetScaledFaction(m_Owner, m_Creature, -25, 25, -0.001) +
                        XmlMobFactions.GetScaledFaction(m_Owner, target, -25, 25, -0.001)) * 0.5;

                    if (music > 100.0)
                    {
                        diff -= (music - 100.0) * 0.5;
                    }

                    if (questTargets || m_Owner.AccessLevel > AccessLevel.Player 
                        || (m_Owner.CanBeHarmful(m_Creature, true) && m_Owner.CanBeHarmful(target, true)))
                    {
                        if (!BaseInstrument.CheckMusicianship(m_Owner))
                        {
                            m_Owner.SendLocalizedMessage(500612); // You play poorly, and there is no effect.
                            m_Instrument.PlayInstrumentBadly(m_Owner);
                            m_Instrument.ConsumeUse(m_Owner);
                        }
                        else
                        {
                            if (!m_Owner.CheckTargetSkill(SkillName.Provocation, target, diff - 25.0, diff + 25.0))
                            {
                                m_Owner.SendLocalizedMessage(501599); // Your music fails to incite enough anger.
                                m_Instrument.PlayInstrumentBadly(m_Owner);
                                m_Instrument.ConsumeUse(m_Owner);
                            }
                            else
                            {
                                m_Owner.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.
                                m_Instrument.PlayInstrumentWell(m_Owner);
                                m_Instrument.ConsumeUse(m_Owner);
                                m_Creature.Provoke(m_Owner, target, true);

                                #region Bard Mastery Quest
                                if (questTargets)
                                {
                                    BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)m_Owner, typeof(IndoctrinationOfABattleRouserQuest));

                                    if (quest != null)
                                    {
                                        foreach (BaseObjective objective in quest.Objectives)
                                            objective.Update(creature);
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                else
                {
					m_Owner.Target = new InternalSecondTarget(m_Owner, m_Instrument, m_Creature);
					
                	m_Owner.SendLocalizedMessage(501593); // You can't tell someone to attack themselves!
                	
                	return;
                }				

				SkillRegistry.Remove(m_Owner);
			}

	        public bool QuestTargets(BaseCreature creature, Mobile from)
            {
                if (creature != null)
                {
                    Mobile getmaster = creature.GetMaster();

                    if (getmaster != null)
                    {
                        if (getmaster is PlayerMobile)
                            return false;
                    }

                    if (from is PlayerMobile && (m_Creature.GetType() == typeof(Rabbit) || m_Creature.GetType() == typeof(JackRabbit)) && ((creature is WanderingHealer) || (creature is EvilWanderingHealer)))
                        return true;

                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
