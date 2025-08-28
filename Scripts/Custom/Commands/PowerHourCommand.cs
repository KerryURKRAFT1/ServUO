using Server.Mobiles;

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
			}
		}

		public static void OnPowerHourCommand(CommandEventArgs e)
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;
			
			if (pm != null)
			{
				if (e.ArgString.ToLower() == "start")
					pm.PowerHourCommandSequence();
				else
					pm.PowerHourStatusMessage();
			}
		}
	}
}