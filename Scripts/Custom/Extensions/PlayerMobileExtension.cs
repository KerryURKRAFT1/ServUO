//This is the most simplistic Power Hour Script ive ever made
//Carlin4737 Aug25

using System;

namespace Server.Mobiles
{
	public enum PowerHourActions
	{
		Initial, Activate, Start, Delay, DeActivate
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
		private bool PowerHourConfigured = false;
		private DateTime m_PowerHourTime = DateTime.MinValue;
		
		public bool PowerHourActive { get { return m_PowerHourActive; } set { m_PowerHourActive = value; } }
		public DateTime PowerHourTime { get { return m_PowerHourTime; } set { m_PowerHourTime = value; } }
		
		private PowerHourActions m_PowerHourAction;

		public PowerHourActions PowerHourAction
		{
			get { return m_PowerHourAction; }
			set
			{
				m_PowerHourAction = value;
				
				PowerHourActive = false;

				switch (m_PowerHourAction)
				{
					case PowerHourActions.Initial:
						PowerHourTime = DateTime.Now + PowerHourDelayInit;
						goto case PowerHourActions.DeActivate;
					case PowerHourActions.Activate:
						PowerHourReadyMessage();
						return;
					case PowerHourActions.Start:
						PowerHourTime = DateTime.Now + PowerHourDuration;
						PowerHourActive = true;
						PowerHourActiveMessage();
						break;
					case PowerHourActions.Delay:
						PowerHourTime = DateTime.Now + PowerHourDelay;
						PowerHourDelayMessage();
						goto case PowerHourActions.DeActivate;
					case PowerHourActions.DeActivate:
						PowerHourDeActivateMessage();
						break;
				}
				
				PowerHourTimer.AddTimer(this);
			}
		}
		#endregion
				
		#region Sequence
		public void PowerHourConfigureSequence() //triggered by startup (Configure it)
		{
			if (!PowerHourEnabled || (IsStaff() && !PowerHourStaffEnabled))
				return;
						
			if (PowerHourTime <= DateTime.MinValue) //detect new start
				PowerHourAction = PowerHourActions.Initial; 
			else if (PowerHourTime > DateTime.Now + PowerHourDelay) //incase delay is changed to a shorter value
				PowerHourAction = PowerHourActions.Delay;
			else if (PowerHourActive || PowerHourTime < DateTime.Now) // if Active or deactive then continue
				PowerHourAction = PowerHourActions.Activate;
			else
				PowerHourAction = PowerHourActions.DeActivate;

			PowerHourConfigured = true; //loading message off
		}
		
		public bool PowerHourChangeSequence() //triggered by Timer (Swap Status)
		{
			if (PowerHourTime > DateTime.Now)
				return false;
							
			if (PowerHourActive)
				PowerHourAction = PowerHourActions.Delay; //if active move to delay
			else
				PowerHourAction = PowerHourActions.Activate; //otherwise activate

			return true;
		}

		public void PowerHourReadySequence() //triggered by command (Activate it)
		{
			if (!PowerHourActive)
			{
				if (PowerHourAction == PowerHourActions.Activate)
				{
					PowerHourStartMessage();
					
					PowerHourAction = PowerHourActions.Start;
				}
				else
					PowerHourDeActivateMessage();
			}
			else
				PowerHourActiveMessage();
		}

		public void PowerHourQuerySequence() //triggered by command (Answer it)
		{
			if (!PowerHourConfigured)
				PowerHourLoadingMessage();
			else if (PowerHourAction == PowerHourActions.Activate)
				PowerHourReadyMessage();
			else if (PowerHourAction == PowerHourActions.DeActivate)
				PowerHourDeActivateMessage();
			else if (PowerHourAction == PowerHourActions.Start)
				PowerHourActiveMessage();
			else
				PowerHourDeActivateMessage();
		}
		#endregion
		
		#region Messages
		public void PowerHourReadyMessage()
		{
			if (PowerHourAction == PowerHourActions.Activate)
			{
				SendMessage (48, "Your power hour is ready (command [PowerHour Start)");
				
				Timer.DelayCall (TimeSpan.FromMinutes(5.0), () => PowerHourReadyMessage());				
			}
		}

		public void PowerHourActiveMessage()
		{
			string minutes = (PowerHourTime - DateTime.Now).TotalMinutes.ToString("N0");
			
			if (minutes != "0")
				SendMessage (48, $"Your power hour has {minutes:F0} minutes remaining");
			else
				SendMessage (48, $"Your power hour is ending");				
		}

		public void PowerHourStartMessage()
		{
			SendMessage (48, "Your power hour has started");
		}

		public void PowerHourDelayMessage()
		{
			SendMessage (48, "Your power hour is over");
		}

		public void PowerHourDeActivateMessage()
		{
			string minutes = (PowerHourTime - DateTime.Now).TotalMinutes.ToString("N0");
			
			if (minutes != "0")
				SendMessage (48, $"Your next power hour is in {minutes:F0} minutes");
			else
				SendMessage (48, $"Your power hour is preparing");				
		}

		public void PowerHourLoadingMessage()
		{
			SendMessage (48, "Your power hour is loading, please wait a few seconds...");
		}
		#endregion
				
		#region Gains
		public double PowerHourRunning(Skill skill, double gc)
		{
			if (CanGain())
				gc *= (PowerHourGainFactor / 100);			

			if (PowerHourFastGain && skill.Value < 90.0)			
				gc *= (6.0 - (skill.Value / 18)) * (PowerHourFastGainMultiplier / 100);

			return gc;
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
			writer.Write(0); // version

			writer.Write(PowerHourActive);
			writer.Write(PowerHourTime);
		}

		public void DeserializeExt(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
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