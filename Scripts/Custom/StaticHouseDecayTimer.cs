using System;
using Server;

namespace Custom
{
    public class StaticHouseDecayTimer : Timer
    {
        public StaticHouseDecayTimer()
            : base(TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0)) // Ogni 5 minuti
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            foreach (Item item in World.Items.Values)
            {
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null)
                {
                    sign.CheckDecay();
                }
            }
        }

        public static void Initialize()
        {
            new StaticHouseDecayTimer().Start();
        }
    }
}