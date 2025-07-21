#region Header
// **********
// ServUO - Notoriety.cs
// **********
#endregion
using System;

namespace Server
{
	public delegate int NotorietyHandler(Mobile source, IDamageable target);

	public static class Notoriety
	{
		public const int Innocent = 1;
		public const int Ally = 2;
		public const int CanBeAttacked = 3;
		public const int Criminal = 4;
		public const int Enemy = 5;
		public const int Murderer = 6;
		public const int Invulnerable = 7;

		private static NotorietyHandler m_Handler;

		public static NotorietyHandler Handler { get { return m_Handler; } set { m_Handler = value; } }

		private static int[] m_Hues = {0x000, 0x059, 0x03F, 0x3B2, 0x3B2, 0x022, 0x022, 0x059};

		public static int[] Hues { get { return m_Hues; } set { m_Hues = value; } }

		public static int GetHue(int noto)
		{
			switch (noto)
			{
				case 1: return 0x059; //blue
				case 2: return 0x03F; //green
				case 3: 
				case 4: return 0x3B2; //grey
				case 5:
				case 6: return 0x022; //red
				case 7: return 0x059; //blue
				default: break;
			}

			return 0;
		}

        public static int Compute(Mobile source, IDamageable target)
		{
			return m_Handler == null ? CanBeAttacked : m_Handler(source, target);
		}
	}
}