using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public class PowerHourTimer : Timer
	{
		private static PowerHourTimer _Instance;

		public static PowerHourTimer Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new PowerHourTimer();

				return _Instance;
			}
		}

		public List<PlayerMobile> Registry { get; set; } = new List<PlayerMobile>();

		public PowerHourTimer() : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
		}

		public static void AddTimer(PlayerMobile player)
		{
			Instance.Registry.Insert(0, player);

			if (!Instance.Running)
				Instance.Start();
		}

		protected override void OnTick()
		{
			var registry = Instance.Registry;

			if (registry.Count > 0)
			{
				for (int i = registry.Count - 1; i >= 0; i--)
				{
					var player = registry[i];

					if (player.PowerHourChangeSequence())
						registry.RemoveAt(i);
				}
			}
		}
	}
}