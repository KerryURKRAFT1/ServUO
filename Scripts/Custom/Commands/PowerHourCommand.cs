using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class PowerHourCommand
	{
		public static void Initialize()
		{
			if (PlayerMobile.PowerHourEnabled)
			{
				CommandSystem.Register("PH", AccessLevel.Player, new CommandEventHandler(OnPowerHourCommand));
				CommandSystem.Register("PowerHour", AccessLevel.Player, new CommandEventHandler(OnPowerHourCommand));

				CommandSystem.Register("PHReset", AccessLevel.GameMaster, new CommandEventHandler(OnPowerHourResetCommand));
				CommandSystem.Register("PowerHourReset", AccessLevel.GameMaster, new CommandEventHandler(OnPowerHourResetCommand));
			}
		}

		public static void OnPowerHourCommand(CommandEventArgs e)
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;
			
			if (pm != null)
			{
				if (e.ArgString.ToLower() == "status")
					pm.PowerHourStatusMessage();
				else if (e.ArgString.ToLower() == "start")
					pm.PowerHourCommandSequence();
				else if (e.ArgString.ToLower() == "stop")
					pm.PowerHourStop = true;
				else
					pm.SendMessage(60, "Command: [powerhour status/start/stop");
			}
		}

		public static void OnPowerHourResetCommand(CommandEventArgs e)
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;
			
			if (pm != null)
			{
        		pm.SendMessage(60, $"target player to reset their powerhour");
        			
	            pm.Target = new InternalTarget();
			}
		}

	    private class InternalTarget : Target
	    {
	        public InternalTarget() : base(-1, false, TargetFlags.None)
	        {
	        }
	
	        protected override void OnTarget(Mobile from, object targeted)
	        {
	        	if (targeted is PlayerMobile pm)
	        	{
	        		pm.PowerHourAction = PowerHourActions.Start;
	        		
	        		from.SendMessage(60, $"{pm.Name}s powerhour has been reset");
	        	}        	        	
	        }
	    }
	}
}