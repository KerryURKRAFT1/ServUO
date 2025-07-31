#region Header
// **********
// ServUO - Spell.cs
// **********
#endregion

#region PlayerMobile Mods
//
//		public override void OnDamage(int amount, Mobile from, bool willKill)
//		{
//			int disruptThreshold;
//
//			if (!Core.AOS)
//			{
//				//threshold for preAOS
//				disruptThreshold = 0;
//			}
//			else if (from != null && from.Player)
//			{
//				disruptThreshold = 19;
//			}
//			else
//			{
//				disruptThreshold = 26;
//			}
//
//			if (amount > disruptThreshold)
//			{
//				BandageContext c = BandageContext.GetContext(this);
//
//				if (c != null)
//				{
//					c.Slip();
//				}
//
//				//Disturb mod
//				if (this.Spell != null && this.Spell.IsCasting)
//            	{
//					((Spell)this.Spell).Disturb(DisturbType.Hurt);
//				}
//				//end
//			}
//
//			. . .
//
//			base.OnDamage(amount, from, willKill);
//		}
//
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Engines.ConPVP;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Second;
using Server.Spells.Spellweaving;
using Server.Targeting;
using Server.Spells.SkillMasteries;
using Server.Spells.Mystic;
#endregion

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private readonly Mobile m_Caster;
		private Item m_Scroll;
		private readonly SpellInfo m_Info;
		private SpellState m_State;
		private long m_StartCastTime;
		private long m_CastTime;
   		private bool m_Disturbed;
   		private object m_ObjectTargeted;
   		private int m_Mana;

		public SpellState State { get { return m_State; } set { m_State = value; } }
		public Mobile Caster { get { return m_Caster; } }
		public SpellInfo Info { get { return m_Info; } }
		public string Name { get { return m_Info.Name; } }
		public string Mantra { get { return m_Info.Mantra; } }
		public Type[] Reagents { get { return m_Info.Reagents; } }
		public Item Scroll { get { return m_Scroll; } set { m_Scroll = value; } }
		public long StartCastTime { get { return m_StartCastTime; } }
		public long CastTime { get { return m_CastTime; }}
		public bool Disturbed { get { return m_Disturbed; }}
		public object ObjectTargeted { get { return m_ObjectTargeted; }}
		public int Mana { get { return m_Mana; } set { m_Mana = value; } }

		private static readonly TimeSpan NextSpellDelay = TimeSpan.Zero;
		private static TimeSpan AnimateDelay = TimeSpan.FromSeconds(1.5);

		public virtual SkillName CastSkill { get { return SkillName.Magery; } }
		public virtual SkillName DamageSkill { get { return SkillName.EvalInt; } }

		public virtual bool RevealOnCast { get { return true; } }
		public virtual bool ClearHandsOnCast { get { return true; } }
		public virtual bool ShowHandMovement { get { return true; } }
		public virtual bool TravelSpell { get { return false; } }

		public virtual bool DelayedDamage { get { return false; } }

		public virtual bool DelayedDamageStacking { get { return true; } }
		//In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only
		//Expo & Magic Arrow have enough delay and a short enough cast time to bring up
		//the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
		//of course, two MA's won't.

		private static readonly Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable =
			new Dictionary<Type, DelayedDamageContextWrapper>();

		private class DelayedDamageContextWrapper
		{
            private readonly Dictionary<IDamageable, Timer> m_Contexts = new Dictionary<IDamageable, Timer>();

			public void Add(IDamageable d, Timer t)
			{
				Timer oldTimer;

				if (m_Contexts.TryGetValue(d, out oldTimer))
				{
					oldTimer.Stop();
					m_Contexts.Remove(d);
				}

				m_Contexts.Add(d, t);
			}

			public void Remove(IDamageable d)
			{
				m_Contexts.Remove(d);
			}
		}

        public void StartDelayedDamageContext(IDamageable d, Timer t)
		{
			if (DelayedDamageStacking)
			{
				return; //Sanity
			}

			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				contexts = new DelayedDamageContextWrapper();
				m_ContextTable.Add(GetType(), contexts);
			}

			contexts.Add(d, t);
		}

		public void RemoveDelayedDamageContext(IDamageable d)
		{
			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				return;
			}

			contexts.Remove(d);
		}

        public void HarmfulSpell(IDamageable d)
		{
			if (d is BaseCreature)
			{
				((BaseCreature)d).OnHarmfulSpell(m_Caster);
			}
            else if (d is IDamageableItem)
            {
                ((IDamageableItem)d).OnHarmfulSpell(m_Caster);
            }
		}

		public Spell(Mobile caster, Item scroll, SpellInfo info)
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;
		}

		public virtual int GetNewAosDamage(int bonus, int dice, int sides, IDamageable singleTarget)
		{
			if (singleTarget != null)
			{
				return GetNewAosDamage(bonus, dice, sides, (Caster.Player && singleTarget is PlayerMobile), GetDamageScalar(singleTarget as Mobile), singleTarget);
			}
			else
			{
				return GetNewAosDamage(bonus, dice, sides, false, null);
			}
		}

        public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, IDamageable damageable)
		{
			return GetNewAosDamage(bonus, dice, sides, playerVsPlayer, 1.0, damageable);
		}

		public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, double scalar, IDamageable damageable)
		{
            Mobile target = damageable as Mobile;

			int damage = Utility.Dice(dice, sides, bonus) * 100;
			int damageBonus = 0;

			int inscribeSkill = GetInscribeFixed(m_Caster);
			int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;
			damageBonus += inscribeBonus;

			int intBonus = Caster.Int / 10;
			damageBonus += intBonus;

			int sdiBonus = AosAttributes.GetValue(m_Caster, AosAttribute.SpellDamage);

            if (target != null && RunedSashOfWarding.IsUnderEffects(target, WardingEffect.SpellDamage))
                sdiBonus -= 10;

            sdiBonus -= Block.GetSpellReduction(target);

			// PvP spell damage increase cap of 15% from an item�s magic property, 30% if spell school focused.
			if (playerVsPlayer)
			{
			    if (SpellHelper.HasSpellFocus(m_Caster, CastSkill) && sdiBonus > 30)
                {
                    sdiBonus = 30;
                }

                if (!SpellHelper.HasSpellFocus(m_Caster, CastSkill) && sdiBonus > 15)
                {
                    sdiBonus = 15;
                }
			}

			damageBonus += sdiBonus;

			TransformContext context = TransformationSpellHelper.GetContext(Caster);

			if (context != null && context.Spell is ReaperFormSpell)
			{
				damageBonus += ((ReaperFormSpell)context.Spell).SpellDamageBonus;
			}

			damage = AOS.Scale(damage, 100 + damageBonus);

            if (target != null && Feint.Registry.ContainsKey(target) && Feint.Registry[target].Enemy == Caster)
                damage -= (int)((double)damage * ((double)Feint.Registry[target].DamageReduction / 100));

			int evalSkill = GetDamageFixed(m_Caster);
			int evalScale = 30 + ((9 * evalSkill) / 100);

			damage = AOS.Scale(damage, evalScale);

			damage = AOS.Scale(damage, (int)(scalar * 100));

			return damage / 100;
		}

		public virtual bool IsCasting { get { return m_State == SpellState.Precasting; } }

        public virtual void OnCasterHurt()
        {
            CheckCasterDisruption(false, 0, 0, 0, 0, 0);
        }

        public virtual void CheckCasterDisruption(bool checkElem = false, int phys = 0, int fire = 0, int cold = 0, int pois = 0, int nrgy = 0)
        {
              if (!Caster.Player)
            {
                return;
            }

            if (IsCasting)
            {
                object o = ProtectionSpell.Registry[m_Caster];
                bool disturb = true;

                if (o != null && o is double)
                {
                    if (((double)o) > Utility.RandomDouble() * 100.0)
                    {
                        disturb = false;
                    }
                }

                if (disturb)
                    Disturb(DisturbType.Hurt, false, true);
            }
        }

		public virtual bool OnCasterKilled()
		{
			return IsCasting;
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving(Direction d)
		{
			return true;
		}

		public virtual bool OnCasterEquiping(Item item)
		{
			return IsCasting;
		}

		public virtual bool OnCasterUsingObject(object o)
		{	
			return IsCasting && o is BasePotion;
		}

		public virtual bool OnCastInTown(Region r)
		{
			return m_Info.AllowTown;
		}
		
		public virtual bool OnWarModeChange()
		{
			return IsCasting;
		}

		public virtual bool OnDamage()
		{
			return IsCasting;
		}

		public virtual bool OnParalyze()
		{
			return IsCasting;
		}

		public virtual void Disturb(DisturbType type)
		{
			Disturb(type, true, false);
		}

		public virtual bool ConsumeReagents()
		{			
            GMRobe robe = m_Caster.FindItemOnLayer(Layer.OuterTorso) as GMRobe; 
			
            if (robe != null)
            {
            	return true;
            }

            if ((m_Scroll != null && !(m_Scroll is SpellStone)) || !m_Caster.Player)
			{
				return true;
			}

			if (AosAttributes.GetValue(m_Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
			{
				return true;
			}

			if (DuelContext.IsFreeConsume(m_Caster))
			{
				return true;
			}

			Container pack = m_Caster.Backpack;

			if (pack == null)
			{
				return false;
			}

			if (pack.ConsumeTotal(m_Info.Reagents, m_Info.Amounts) == -1)
			{
				return true;
			}

			return false;
		}

		public virtual double GetInscribeSkill(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );
			return m.Skills[SkillName.Inscribe].Value;
		}

		public virtual int GetInscribeFixed(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );
			return m.Skills[SkillName.Inscribe].Fixed;
		}

		public virtual int GetDamageFixed(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );
			return m.Skills[DamageSkill].Fixed;
		}

		public virtual double GetDamageSkill(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );
			return m.Skills[DamageSkill].Value;
		}

		public virtual double GetResistSkill(Mobile m)
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual double GetDamageScalar(Mobile target)
		{
			double scalar = 1.0;

            if (target == null)
                return scalar;

			if (!Core.AOS) //EvalInt stuff for AoS is handled elsewhere
			{
				double casterEI = m_Caster.Skills[DamageSkill].Value;
				double targetRS = target.Skills[SkillName.MagicResist].Value;

				/*
				if( Core.AOS )
				targetRS = 0;
				*/

				//m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

				if (casterEI > targetRS)
				{
					scalar = (1.0 + ((casterEI - targetRS) / 500.0));
				}
				else
				{
					scalar = (1.0 + ((casterEI - targetRS) / 200.0));
				}

				// magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 120 skill
				scalar += (m_Caster.Skills[CastSkill].Value - 100.0) / 400.0;

				if (!target.Player && !target.Body.IsHuman /*&& !Core.AOS*/)
				{
					scalar *= 2.0; // Double magery damage to monsters/animals if not AOS
				}
			}

			if (target is BaseCreature)
			{
				((BaseCreature)target).AlterDamageScalarFrom(m_Caster, ref scalar);
			}

			if (m_Caster is BaseCreature)
			{
				((BaseCreature)m_Caster).AlterDamageScalarTo(target, ref scalar);
			}

			if (Core.SE)
			{
				scalar *= GetSlayerDamageScalar(target);
			}

			target.Region.SpellDamageScalar(m_Caster, target, ref scalar);

			if (Evasion.CheckSpellEvasion(target)) //Only single target spells an be evaded
			{
				scalar = 0;
			}

			return scalar;
		}

		public virtual double GetSlayerDamageScalar(Mobile defender)
		{
			Spellbook atkBook = Spellbook.FindEquippedSpellbook(m_Caster);

			double scalar = 1.0;
			if (atkBook != null)
			{
				SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkBook.Slayer);
				SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkBook.Slayer2);

				if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
				{
					defender.FixedEffect(0x37B9, 10, 5); //TODO: Confirm this displays on OSIs
					scalar = 2.0;
				}

				TransformContext context = TransformationSpellHelper.GetContext(defender);

				if ((atkBook.Slayer == SlayerName.Silver || atkBook.Slayer2 == SlayerName.Silver) && context != null &&
					context.Type != typeof(HorrificBeastSpell))
				{
					scalar += .25; // Every necromancer transformation other than horrific beast take an additional 25% damage
				}

				if (scalar != 1.0)
				{
					return scalar;
				}
			}

			ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

			if (defISlayer == null)
			{
				defISlayer = defender.Weapon as ISlayer;
			}

			if (defISlayer != null)
			{
				SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
				SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);

				if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(m_Caster) ||
					defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(m_Caster))
				{
					scalar = 2.0;
				}
			}

			return scalar;
		}

		public virtual void DoFizzle()
		{
			if (m_Caster.Player)
			{
				if (!m_Disturbed)
				{
					m_Caster.SendLocalizedMessage(502632); // The spell fizzles.
				}
				
				DoHurtFizzle();

			}

			FinishSequence();
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect(0x3735, 6, 30);
			m_Caster.PlaySound(0x5C);
		}

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public virtual bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
		{
			if (resistable && m_Scroll is BaseWand)
			{
				return false;
			}

			return true;
		}

		public void Disturb(DisturbType type, bool firstCircle, bool resistable) //does not take mana
		{
			if (!CheckDisturb(type, firstCircle, resistable))
			{
				return;
			}
			
			if (IsCasting || (m_State == SpellState.Sequencing && !firstCircle && this is MagerySpell && ((MagerySpell)this).Circle != SpellCircle.First))
			{
				m_Disturbed = true;
								
				OnDisturb(type, true);

				if (m_State == SpellState.Precasting)
				{
					m_Caster.NextSpellTime = Core.TickCount + (int)GetDisturbRecovery().TotalMilliseconds;
				}

				DoFizzle();
				
				FinishSequence();
			}
		}

		public virtual void OnDisturb(DisturbType type, bool message)
		{
			if (message)
			{
				m_Caster.SendLocalizedMessage(500641, "", 0x22); // Your concentration is disturbed, thus ruining thy spell.
			}
		}

		public virtual bool CheckCast()
		{
			return true;
		}

		public virtual void SayMantra()
		{
            if (m_Scroll is SpellStone)
            {
                return;
            }

            if (m_Scroll is BaseWand)
			{
				return;
			}

			if (m_Info.Mantra != null && m_Info.Mantra.Length > 0 && (m_Caster.Player || (m_Caster is BaseCreature && ((BaseCreature)m_Caster).ShowSpellMantra)))
			{
				m_Caster.PublicOverheadMessage(MessageType.Regular, 2042, true, m_Info.Mantra, false);
			}
		}

		public virtual bool BlockedByHorrificBeast
        {
            get
            {
                if (TransformationSpellHelper.UnderTransformation(Caster, typeof(HorrificBeastSpell)) &&
                    SpellHelper.HasSpellFocus(Caster, CastSkill))
                    return false;

                return true;
            }
        }

		public virtual bool BlockedByAnimalForm { get { return false; } }
		public virtual bool BlocksMovement { get { return false; } }

		public virtual bool CheckNextSpellTime { get { return !(m_Scroll is BaseWand); } }

		public virtual bool Cast()
		{
			if (m_Caster.Mana >= (m_Mana = ScaleMana(GetMana())))
 			{
 				return StartSequence();
 			}
			
        	this.Caster.SendLocalizedMessage(502625, "", 0x22); // Insufficient mana
        	
			return false;
        }
		
		public virtual bool StartSequence(object target = null)
		{
			m_ObjectTargeted = target;
			
			m_StartCastTime = Core.TickCount;

			if (!m_Caster.CheckAlive())
			{
				return false;
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).Peaced)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
			}
			else if (m_Caster.Spell != null && m_Caster.Spell.IsCasting)
			{
				m_Caster.SendLocalizedMessage(502642); // You are already casting a spell.
			}
			else if (!(m_Scroll is BaseWand) && (m_Caster.Paralyzed || m_Caster.Frozen))
			{
				m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
			}
			else if (CheckNextSpellTime && Core.TickCount - m_Caster.NextSpellTime < 0)
			{
//				m_Caster.SendLocalizedMessage(502644); // You have not yet recovered from casting a spell.
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
			}

			#region Dueling
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).DuelContext != null &&
					 !((PlayerMobile)m_Caster).DuelContext.AllowSpellCast(m_Caster, this))
			{ }
				#endregion

			else
			{
				if (m_Caster.Spell == null && m_Caster.CheckSpellCast(this) && CheckCast() &&
				    m_Caster.Region.OnBeginSpellCast(m_Caster, this))
				{
					m_State = SpellState.Precasting;
					m_Caster.Spell = this;

					if (!(m_Scroll is BaseWand) && RevealOnCast)
					{
						m_Caster.RevealingAction();
					}

					SayMantra();

					TimeSpan castDelay = GetCastDelay() + TimeSpan.FromSeconds(0.5);
					
                    m_CastTime = Core.TickCount + (long)castDelay.TotalMilliseconds;

					if (ShowHandMovement && !(m_Scroll is SpellStone) && (m_Caster.Body.IsHuman || (m_Caster.Player && m_Caster.Body.IsMonster)))
					{
						int count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);

						if (count != 0)
						{
							m_AnimTimer = new AnimTimer(this, count);
							m_AnimTimer.Start();
						}

						if (m_Info.LeftHandEffect > 0)
						{
							Caster.FixedParticles(0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand);
						}

						if (m_Info.RightHandEffect > 0)
						{
							Caster.FixedParticles(0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand);
						}
					}

					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}
									
					OnBeginCast();

					m_CastTimer = new CastTimer(this, castDelay <= TimeSpan.FromSeconds(1.5) ? 25:100);

					if (castDelay > TimeSpan.Zero)
					{
		                m_CastTimer.Start();
		            }
		            else
		            {
		            	m_CastTimer.Tick();
		            }
					
					return true;
				}
			}

			return false;
		}

		public abstract void OnCast();
			
		public virtual void OnBeginCast()
		{ 
            if (m_Caster != m_ObjectTargeted)
            {
            	SpellHelper.Turn(m_Caster, m_ObjectTargeted);
            }
		}

		public virtual void GetCastSkills(out double min, out double max)
		{
			min = max = 0; //Intended but not required for overriding.
		}

		public virtual bool CheckFizzle()
		{
			if (m_Scroll is BaseWand)
			{
				return true;
			}

			double minSkill, maxSkill;

			GetCastSkills(out minSkill, out maxSkill);

			if (DamageSkill != CastSkill)
			{
				Caster.CheckSkill(DamageSkill, 0.0, Caster.Skills[DamageSkill].Cap);
			}

			return Caster.CheckSkill(CastSkill, minSkill, maxSkill);
		}

		public abstract int GetMana();

		public virtual int ScaleMana(int mana)
		{
			double scalar = 1.0;

            if (ManaPhasingOrb.IsInManaPhase(Caster))
            {
                ManaPhasingOrb.RemoveFromTable(Caster);
                return 0;
            }

			if (!MindRotSpell.GetMindRotScalar(Caster, ref scalar))
			{
				scalar = 1.0;
			}

			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue(m_Caster, AosAttribute.LowerManaCost);

			if (lmc > 40)
			{
				lmc = 40;
			}

            lmc += BaseArmor.GetInherentLowerManaCost(m_Caster);
 
			scalar -= (double)lmc / 100;

			return (int)(mana * scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
				return TimeSpan.Zero;
		}

		public virtual int CastRecoveryBase { get { return 6; } }
		public virtual int CastRecoveryFastScalar { get { return 1; } }
		public virtual int CastRecoveryPerSecond { get { return 4; } }
		public virtual int CastRecoveryMinimum { get { return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
			if (!Core.AOS)
			{
				return NextSpellDelay;
			}

			int fcr = AosAttributes.GetValue(m_Caster, AosAttribute.CastRecovery);

			int fcrDelay = -(CastRecoveryFastScalar * fcr);

			int delay = CastRecoveryBase + fcrDelay;

			if (delay < CastRecoveryMinimum)
			{
				delay = CastRecoveryMinimum;
			}

			return TimeSpan.FromSeconds((double)delay / CastRecoveryPerSecond);
		}

		public abstract TimeSpan CastDelayBase { get; }

		public virtual double CastDelayFastScalar { get { return 1; } }
		public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
		public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds(0.25); } }

		//public virtual int CastDelayBase{ get{ return 3; } }
		//public virtual int CastDelayFastScalar{ get{ return 1; } }
		//public virtual int CastDelayPerSecond{ get{ return 4; } }
		//public virtual int CastDelayMinimum{ get{ return 1; } }

		public virtual TimeSpan GetCastDelay()
		{
            if (m_Scroll is SpellStone)
            {
                return TimeSpan.Zero;
            }

            if (m_Scroll is BaseWand)
			{
				return Core.ML ? CastDelayBase : TimeSpan.Zero; // TODO: Should FC apply to wands?
			}

			// Faster casting cap of 2 (if not using the protection spell)
			// Faster casting cap of 0 (if using the protection spell)
			// Paladin spells are subject to a faster casting cap of 4
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2
			int fcMax = 4;

			if (CastSkill == SkillName.Magery || CastSkill == SkillName.Necromancy ||
                (CastSkill == SkillName.Chivalry && (m_Caster.Skills[SkillName.Magery].Value >= 70.0 || m_Caster.Skills[SkillName.Mysticism].Value >= 70.0)))
			{
				fcMax = 2;
			}

			int fc = AosAttributes.GetValue(m_Caster, AosAttribute.CastSpeed);

			if (fc > fcMax)
			{
				fc = fcMax;
			}

            if (ProtectionSpell.Registry.ContainsKey(m_Caster) /*|| EodonianPotion.IsUnderEffects(m, PotionEffect.Urali)*/)
            {
                fc = Math.Min(fcMax - 2, fc - 2);
            }

			if (EssenceOfWindSpell.IsDebuffed(m_Caster))
			{
				fc -= EssenceOfWindSpell.GetFCMalus(m_Caster);
			}

			TimeSpan baseDelay = CastDelayBase;

			TimeSpan fcDelay = TimeSpan.FromSeconds(-(CastDelayFastScalar * fc * CastDelaySecondsPerTick));

			TimeSpan delay = baseDelay + fcDelay;

			if (delay < CastDelayMinimum)
			{
				delay = CastDelayMinimum;
			}

			return delay;
		}

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;
			
			if (m_Caster.Spell == this)
			{
				m_Caster.Spell = null;
			}

			if (m_CastTimer != null)
			{
				m_CastTimer.Stop();
				m_CastTimer = null;
			}

			if (m_AnimTimer != null)
			{
				m_AnimTimer.Stop();
				m_AnimTimer = null;
			}
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			if (m_Caster.Deleted || !m_Caster.Alive || (!TravelSpell && (m_Caster.Spell != this || m_State != SpellState.Sequencing)))
			{
				DoFizzle();
			}
			else if (m_Scroll != null && !(m_Scroll is Runebook) &&
					 (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster ||
					  (m_Scroll is BaseWand && (((BaseWand)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))))
			{
				DoFizzle();
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
				DoFizzle();
			}
			else if (CheckFizzle())
			{
                if (m_Scroll is SpellStone)
                {
                    ((SpellStone)m_Scroll).Use(m_Caster);
                }

                if (m_Scroll is SpellScroll)
				{
					m_Scroll.Consume();
				}
				else if (m_Scroll is BaseWand)
				{
					((BaseWand)m_Scroll).ConsumeCharge(m_Caster);
					m_Caster.RevealingAction();
				}

				if (m_Scroll is BaseWand)
				{
					bool m = m_Scroll.Movable;

					m_Scroll.Movable = false;

					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}

					m_Scroll.Movable = m;
				}
				else
				{
					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}
				}

				int karma = ComputeKarmaAward();

				if (karma != 0)
				{
					Titles.AwardKarma(Caster, karma, true);
				}

				if (TransformationSpellHelper.UnderTransformation(m_Caster, typeof(VampiricEmbraceSpell)))
				{
					bool garlic = false;

					for (int i = 0; !garlic && i < m_Info.Reagents.Length; ++i)
					{
						garlic = (m_Info.Reagents[i] == Reagent.Garlic);
					}

					if (garlic)
					{
						m_Caster.SendLocalizedMessage(1061651); // The garlic burns you!
						AOS.Damage(m_Caster, Utility.RandomMinMax(17, 23), 100, 0, 0, 0, 0);
					}
				}

				m_Caster.Mana -= m_Mana;

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence(Mobile target, bool allowDead = false)
		{
			if (!target.Alive && !allowDead)
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}
			else if (Caster.CanBeBeneficial(target, true, allowDead) && CheckSequence())
			{
				Caster.DoBeneficial(target);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence(IDamageable target)
		{
			if (!target.Alive || (target is IDamageableItem && !((IDamageableItem)target).CanDamage))
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}
			else if (Caster.CanBeHarmful(target) && CheckSequence())
			{
				Caster.DoHarmful(target);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public void CastSequence()
		{
			m_Caster.NextSpellTime = Core.TickCount + (int)GetCastRecovery().TotalMilliseconds;
			Target originalTarget = m_Caster.Target;	
			m_State = SpellState.Sequencing;
			
			if (!Disturbed)
			{				
				if (m_Caster.Region != null)
				{
					m_Caster.Region.OnSpellCast(m_Caster, this);
				}
				
				m_Caster.OnSpellCast(this);
				
				if (CheckLOS(ObjectTargeted))
				{
					if (ConsumeReagents())
					{
						OnCast();
					}
					else
					{
						m_Caster.SendLocalizedMessage(502630, "", 0x22); // More reagents are needed for this spell.
					}
				}
				else
				{
					DoFizzle();
				}
			}
			else
			{
				m_Caster.NextSpellTime = Core.TickCount + (int)GetDisturbRecovery().TotalMilliseconds;
			}
			
			if (m_Caster.Player && m_Caster.Target != originalTarget && m_Caster.Target != null)
			{
				m_Caster.Target.BeginTimeout(m_Caster, TimeSpan.FromSeconds(30.0));
			}
			
			FinishSequence();
		}
		
		public bool CheckLOS(object obj, int range = 15)//overridden by mageryspell
		{
       		IPoint3D loc = obj as IPoint3D;
       		
       		if (loc != null)
       		{
				if (!this.Caster.InLOS(new Point3D(loc)) || !this.Caster.CanSee(obj))
				{
					this.Caster.SendLocalizedMessage(500237);// Target can not be seen.
					return false;
				}
        		else if( !this.Caster.InRange( new Point3D( loc ), range ))
				{
					this.Caster.SendLocalizedMessage(500446);// That is too far away.
					return false;
                }
			}
       		
        	return true;
		}
		
		public void Explode (BaseExplosionPotion pot)
		{
        	if (pot != null && CheckSequence())
            {
                pot.Explode (this.Caster, true, pot.GetWorldLocation(), pot.Map);
                this.Caster.MovingParticles (pot, 0x36E4, 5, 0, false, true, 3006, 4006, 0);
                this.Caster.PlaySound (0x1E5);
			}
		}

		private class AnimTimer : Timer
		{
			private readonly Spell m_Spell;

			public AnimTimer(Spell spell, int count)
				: base(TimeSpan.Zero, AnimateDelay, count)
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if (m_Spell.State != SpellState.Precasting || m_Spell.m_Caster.Spell != m_Spell)
				{
					Stop();
					return;
				}

				if (!m_Spell.Caster.Mounted && m_Spell.m_Info.Action >= 0)
				{
					if (m_Spell.Caster.Body.IsHuman)
					{
						m_Spell.Caster.Animate(m_Spell.m_Info.Action, 7, 1, true, false, 0);
					}
					else if (m_Spell.Caster.Player && m_Spell.Caster.Body.IsMonster)
					{
						m_Spell.Caster.Animate(12, 7, 1, true, false, 0);
					}
				}
                else if( m_Spell.Caster.Body.IsAnimal || m_Spell.Caster.Body.IsMonster )
                {
                	m_Spell.Caster.Animate( 11, 7, 1, true, false, 2 );
                }
                else if( m_Spell.Caster.Mounted && m_Spell.Caster.Body.IsHuman && m_Spell.m_Info.Action >= 0 )
                {
                	m_Spell.Caster.Animate( m_Spell.m_Info.Action == 263 ? 27 : 26, 5, 1, true, false, 2 );
                }

				if (!Running)
				{
					m_Spell.m_AnimTimer = null;
				}
			}
		}
			
		private class CastTimer : Timer
		{
			private readonly Spell m_Spell;

			public CastTimer(Spell spell, int castDelay) : base(TimeSpan.FromMilliseconds(castDelay), TimeSpan.FromMilliseconds(castDelay) )
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if (m_Spell == null || m_Spell.m_Caster == null || m_Spell.m_Caster.Spell != m_Spell
				    || m_Spell.State != SpellState.Precasting || !m_Spell.Caster.Alive || m_Spell.Caster.Deleted
				    || m_Spell.Caster.IsDeadBondedPet)
				{
                    m_Spell.FinishSequence();
				}
				else if (m_Spell.m_CastTime <= Core.TickCount)
                {
                    m_Spell.CastSequence();
                }
			}

			public void Tick()
			{
				OnTick();
			}
		}
	}
}
