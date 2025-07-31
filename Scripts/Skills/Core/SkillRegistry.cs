/*
	public enum TimerPriority
	{
		EveryTick,
		TenMS,
		TwentyFiveMS,
		FiftyMS,
		TwoFiftyMS,
		OneSecond,
		FiveSeconds,
		OneMinute
	}
*/

using System;
using Server.Commands;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;

namespace Server.SkillHandlers
{
	class SkillRegistry
	{
		public static void Initialize()
		{
    		CommandSystem.Register("ClearSkillRegistry", AccessLevel.Administrator, new CommandEventHandler(ClearSkillRegistry_OnCommand));
    		CommandSystem.Register("csr", AccessLevel.Administrator, new CommandEventHandler(ClearSkillRegistry_OnCommand));
		}

		public static void ClearSkillRegistry_OnCommand(CommandEventArgs e)
		{
			Entries.Clear();
		}

		public static List<Mobile> Entries = new List<Mobile>();

    	public static TimeSpan ShortDelay = TimeSpan.FromSeconds(1.0);
    	public static TimeSpan Delay = TimeSpan.FromSeconds(2.0);
    	public static TimeSpan MusicDelay = TimeSpan.FromSeconds(4.0);
    	public static TimeSpan LongDelay = TimeSpan.FromSeconds(6.0);
    	public static TimeSpan Timeout = TimeSpan.FromSeconds(6.0);

    	public static TimeSpan MeditateStamRate = TimeSpan.FromSeconds(2.0);

    	public static double MeditateSlowRate = 0.4;
    	public static double MeditateFastRate = 0.25;

    	public static TimeSpan Blood = TimeSpan.FromSeconds(30.0);

    	public static bool WaitMsg { get; set; } //You must wait to perform another action
    	public static bool Bandage { get; set; } //Prevents skill use while applying bandages
		
    	public static void Configure()
		{
	    	WaitMsg = false;
	    	
	    	Skills.WaitMsg = false;
	    	
	    	Bandage = false;
		}
			
    	public static bool Contains(Mobile m)
		{
			CheckTimeOut(m);
			
			if (Bandage && GetBandageContext(m))
			{
				return true;
			}
			
			return Entries.Contains(m);
		}

		public static void Add(Mobile m)
		{
			m.NextSkillTime = Core.TickCount + (int)Timeout.TotalMilliseconds;
			
			Entries.Add(m);
    	}
		
		public static void Remove(Mobile m)
		{
			Entries.Remove(m);
			
			m.Target = null;
		}

		public static void CheckTimeOut(Mobile m)
		{
			if (Entries.Contains(m) && !m.Meditating && m.NextSkillTime < Core.TickCount)
			{
				Entries.Remove(m);
			}
    	}

    	public static bool GetBandageContext(Mobile m)
		{
			BandageContext context = BandageContext.GetContext(m);

			return context != null;
    	}
	}
}