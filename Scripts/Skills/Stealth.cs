using System;
using Server.Items;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Stealth
    {
		private static bool m_TriggeredFromHide = false;

		private static readonly int[,] m_ArmorTable = new int[,]
		{
			//	Gorget	Gloves	Helmet	Arms	Legs	Chest	Shield
			/* Cloth	*/ { 0, 0, 0, 0, 0, 0, 0 },
			/* Leather	*/ { 0, 0, 0, 0, 0, 0, 0 },
			/* Studded	*/ { 2, 2, 0, 4, 6, 10, 0 },
			/* Bone		*/ { 0, 5, 10, 10, 15, 25, 0 },
			/* Spined	*/ { 0, 0, 0, 0, 0, 0, 0 },
			/* Horned	*/ { 0, 0, 0, 0, 0, 0, 0 },
			/* Barbed	*/ { 0, 0, 0, 0, 0, 0, 0 },
			/* Ring		*/ { 0, 5, 0, 10, 15, 25, 0 },
			/* Chain	*/ { 0, 0, 10, 0, 15, 25, 0 },
			/* Plate	*/ { 5, 5, 10, 10, 15, 25, 0 },
			/* Dragon	*/ { 0, 5, 10, 10, 15, 25, 0 }
		};
		
		public static double HidingRequirement
		{
			get
			{
				return (Core.ML ? 30.0 : (Core.SE ? 50.0 : 80.0));
			}
		}
		
		public static double StealthRequirement
		{
			get
			{
				return (Core.ML ? 30.0 : (Core.SE ? 50.0 : 80.0));
			}
		}

		public static int[,] ArmorTable
		{
			get
			{
				return m_ArmorTable;
			}
		}
		
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Stealth].Callback = new SkillUseCallback(OnUse);
		}

		public static int GetArmorRating(Mobile m)
		{
			if (!Core.AOS)
				return (int)m.ArmorRating;

			int ar = 0;

			for (int i = 0; i < m.Items.Count; i++)
			{
				BaseArmor armor = m.Items[i] as BaseArmor;

				if (armor == null)
				{
					continue;
				}

				int materialType = (int)armor.MaterialType;
				int bodyPosition = (int)armor.BodyPosition;

				if (materialType >= m_ArmorTable.GetLength(0) || bodyPosition >= m_ArmorTable.GetLength(1))
				{
					continue;
				}

				if (armor.ArmorAttributes.MageArmor == 0)
				{
					ar += m_ArmorTable[materialType, bodyPosition];
				}
			}

			return ar;
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
		
		public static void Start(Mobile m)
		{
			if (SkillRegistry.Contains(m))
			{
				SkillRegistry.Add(m);
			}

			m_TriggeredFromHide = true;
	
			if (m.Skills[SkillName.Hiding].Base >= HidingRequirement && m.Skills[SkillName.Stealth].Base >= StealthRequirement)
			{			
				new SkillTimer(m, SkillRegistry.ShortDelay).Start();
			}
		}
		
		public static bool TriggerSkill(Mobile m)
		{
			if (!m.Hidden)
			{
				m.SendLocalizedMessage(502725); // You must hide first
			}
			else if (m.Skills[SkillName.Hiding].Base < HidingRequirement)
			{
				m.SendLocalizedMessage(502726); // You are not hidden well enough.  Become better at hiding.
				
				m.RevealingAction();
				
				BuffInfo.RemoveBuff(m, BuffIcon.HidingAndOrStealth);
			}
			else if (!m.CanBeginAction(typeof(Stealth)))
			{
				m.SendLocalizedMessage(1063086); // You cannot use this skill right now.
				
				m.RevealingAction();
				
				BuffInfo.RemoveBuff(m, BuffIcon.HidingAndOrStealth);
			}
			else
			{				
				new SkillTimer(m, SkillRegistry.Delay).Start();
				
				return true;
			}

			return false;
		}

		private class SkillTimer : Timer
		{
			private readonly Mobile m_Owner;

			public SkillTimer(Mobile owner, TimeSpan delay) : base(delay)
			{
				m_Owner = owner;

				Priority = TimerPriority.TwoFiftyMS;
			}
			
			protected override void OnTick()
			{
				int armorRating = GetArmorRating(m_Owner);

				if (armorRating >= (Core.AOS ? 42 : 26)) //I have a hunch '42' was chosen cause someone's a fan of DNA
				{
					m_Owner.SendLocalizedMessage(502727); // You could not hope to move quietly wearing this much armor.

					if (!m_TriggeredFromHide)
					{
						m_Owner.RevealingAction();
						
						BuffInfo.RemoveBuff(m_Owner, BuffIcon.HidingAndOrStealth);
					}
				}
				else if (m_Owner.CheckSkill(SkillName.Stealth, -20.0 + (armorRating * 2), (Core.AOS ? 60.0 : 80.0) + (armorRating * 2)))
				{
					int steps = (int)(m_Owner.Skills[SkillName.Stealth].Value / 5.0);

					if (steps < 1)
					{
						steps = 1;
					}

					m_Owner.AllowedStealthSteps = steps;

					PlayerMobile pm = m_Owner as PlayerMobile;

					if (pm != null)
					{
						pm.IsStealthing = true;
					}

					m_Owner.SendLocalizedMessage(502730); // You begin to move quietly.

					if (!m_TriggeredFromHide)
					{
						BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.HidingAndOrStealth, 1044107, 1075655));
					}
				}
				else
				{
					m_Owner.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
					
					if (!m_TriggeredFromHide)
					{
						m_Owner.RevealingAction();
					
						BuffInfo.RemoveBuff(m_Owner, BuffIcon.HidingAndOrStealth);
					}
				}

				SkillRegistry.Remove(m_Owner);
			}
		}
	}
}