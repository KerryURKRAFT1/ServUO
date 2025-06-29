using System;
using System.Linq; // aggiungi questa using!
using Server;
using Server.StaticHouse;

namespace Server.StaticHouse
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
            // Copia la collezione in una lista per enumerare in modo sicuro
            foreach (Item item in World.Items.Values.ToList())
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