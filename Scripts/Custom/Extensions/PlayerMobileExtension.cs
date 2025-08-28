//This is the most simplistic Power Hour Script ive ever made
//Carlin4737 Aug25

using System;

namespace Server.Mobiles
{
	public enum PowerHourActions
	{
		Initial, 	//0
		Start,		//1
		Activate,	//2
		Delay,		//3
		DeActivate	//4
	}

	public partial class PlayerMobile : Mobile
	{
		#region configure			
		public static bool PowerHourEnabled = Config.Get("Custom_PowerHour.PowerHourEnabled", true);
		public static bool PowerHourStaffEnabled = Config.Get("Custom_PowerHour.PowerHourStaffEnabled", false);
		
		public static TimeSpan PowerHourDuration = TimeSpan.FromMinutes(Config.Get("Custom_PowerHour.PowerHourDuration", 60.0));
		public static TimeSpan PowerHourDelay = TimeSpan.FromMinutes(Config.Get("Custom_PowerHour.PowerHourDelay", 300.0));
		public static TimeSpan PowerHourDelayInit = TimeSpan.FromMinutes(Config.Get("Custom_PowerHour.PowerHourDelayInit", 60.0));
		
		public static double PowerHourGainFactor = Config.Get("Custom_PowerHour.PowerHourGainFactor", 200.0);
		public static bool PowerHourFastGain = Config.Get("Custom_PowerHour.PowerHourFastGain", false);
		public static double PowerHourFastGainMultiplier = Config.Get("Custom_PowerHour.PowerHourFastGainMultiplier", 500.0);
		
		private bool m_PowerHourActive;
		private DateTime m_PowerHourTime = DateTime.MinValue;
		
		public bool PowerHourActive { get { return m_PowerHourActive; } set { m_PowerHourActive = value; } }
		public DateTime PowerHourTime { get { return m_PowerHourTime; } set { m_PowerHourTime = value; } }
		
		public int PowerHourLoad { get; set; }

		private PowerHourActions m_PowerHourAction;

		public PowerHourActions PowerHourAction
		{
			get { return m_PowerHourAction; }
			set
			{
				m_PowerHourAction = value;

				m_PowerHourActive = false;
												
				switch (m_PowerHourAction)
				{
					case PowerHourActions.Initial:
						PowerHourTime = DateTime.Now + PowerHourDelayInit;
						PowerHourAction = PowerHourActions.DeActivate;
						return;
					case PowerHourActions.Start:
						break;
					case PowerHourActions.Activate:
						PowerHourTime = DateTime.Now + PowerHourDuration;		
						PowerHourTimer.AddTimer(this);
						PowerHourActive = true;
						break;
					case PowerHourActions.Delay:
						PowerHourTime = DateTime.Now + PowerHourDelay;
						PowerHourAction = PowerHourActions.DeActivate;
						return;
					case PowerHourActions.DeActivate:
						PowerHourTimer.AddTimer(this);
						break;
				}
				
				PowerHourStatusMessage();
			}
		}
		#endregion
				
		#region Sequence
		public void PowerHourConfigureSequence()
		{
			if (PowerHourEnabled || (IsStaff() && PowerHourStaffEnabled))
			{
				if (PowerHourLoad == 2) //if active restart it
					PowerHourAction = PowerHourActions.Start;
				else 
					PowerHourAction = (PowerHourActions)PowerHourLoad;
			}
		}
		
		public void PowerHourStatusMessage()
		{
			switch (PowerHourAction)
			{
				case PowerHourActions.Initial: 
					PowerHourLoadingMessage(); break;
				case PowerHourActions.Start: 
					PowerHourReadyMessage(); break;
				case PowerHourActions.Activate: 
					PowerHourActiveMessage(); break;
				default: 
					PowerHourDeActivateMessage(); break;
			}
		}

		public void PowerHourChangeSequence()
		{						
			if (!PowerHourActive)
				PowerHourAction = PowerHourActions.Start;
			else
				PowerHourAction = PowerHourActions.Delay;
		}

		public void PowerHourCommandSequence() //called by command
		{			
			if (PowerHourAction == PowerHourActions.Start)
				PowerHourAction = PowerHourActions.Activate;
			else
				PowerHourStatusMessage();	
		}
		#endregion
		
		#region Messages
		private void PowerHourLoadingMessage()
		{
			SendMessage (48, "Your power hour is loading, please wait a few seconds...");
		}

		private void PowerHourReadyMessage()
		{
			if (PowerHourAction == PowerHourActions.Start)
			{
				SendMessage (48, "Your power hour is ready (command [powerHour start)");				
				Timer.DelayCall (TimeSpan.FromMinutes(5.0), () => PowerHourReadyMessage());				
			}
		}

		private void PowerHourActiveMessage()
		{		
			SendMessage (60, $"Your power hour has {FormatTime(PowerHourTime - DateTime.Now)} remaining");
		}

		private void PowerHourDeActivateMessage()
		{
			SendMessage (60, $"Your next power hour is in {FormatTime(PowerHourTime - DateTime.Now)}");
		}
		#endregion
			
		#region format TimeSpan
		public static string FormatTime( TimeSpan t)
		{
			double minutes = (t.Minutes % 60) + 1;			
			double hours = (t.Hours % 60); 
			
			if (t.Days > 0)
				return String.Format("{0} day {1} hour{2}", t.Days, hours, (hours != 1 ? "s":""));
			else if (t.Hours > 0)
				return String.Format("{0} hour{1} {2} minute{3}", hours, (hours != 1 ? "s":""), minutes, (minutes != 1 ? "s":""));
			else if (t.Minutes > 0)
				return String.Format("{0} minute{1}", minutes, (minutes != 1 ? "s":""));			

			return "< 1 minute";
		}
		#endregion
		
		#region Gains
		public double PowerHourBonus(Skill skill, double gc)
		{
			if (CanGain())
				gc *= (PowerHourGainFactor / 100);

			if (PowerHourFastGain && skill.Value < 90.0)			
				gc *= (6.0 - (skill.Value / 18)) * (PowerHourFastGainMultiplier / 100);
			
			return gc;
		}

		public int PowerHourGain(Skill skill, int toGain)
		{
			if (CanGain())
                toGain += Utility.Random(2);
			
			if (PowerHourFastGain && skill.Value < 90.0)			
                toGain += Utility.Random(2) + 1;
				
			return toGain;
		}

		private bool CanGain() //add constraints here
		{
			if (!IsStaff() && (Hidden || Blessed)) //no power hour gains for those players hiding or blessed
				return false;
									
			return PowerHourActive;
		}
		#endregion
		
		public void SerializeExt(GenericWriter writer)
		{
			writer.Write(1); // version

			//version 1
			writer.Write((int)PowerHourAction);
			//version 0
			writer.Write(false);
			writer.Write(PowerHourTime);
		}

		public void DeserializeExt(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
				{
					PowerHourLoad = reader.ReadInt();
					
					goto case 0;
				}
				case 0:
				{
					PowerHourActive = reader.ReadBool();
					PowerHourTime = reader.ReadDateTime();

					break;
				}
			}
		}
	}
}