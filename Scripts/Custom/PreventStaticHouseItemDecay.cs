using System;
using Server;

namespace Server.StaticHouse
{
    public static class PreventStaticHouseItemDecay
    {
        private static Timer m_Timer;

        public static void Initialize()
        {
            // Avvia il timer solo una volta all'avvio shard
            if (m_Timer == null)
            {
                m_Timer = new DecayPreventTimer();
                m_Timer.Start();
            }
        }

        private class DecayPreventTimer : Timer
        {
            public DecayPreventTimer() : base(TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0))
            {
                // Timer: scatta ogni 10 minuti
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                int count = 0;
                foreach (Item item in World.Items.Values)
                {
                    if (item == null || item.Deleted)
                        continue;

                    // Se l'item si trova in una static house con owner
                    if (StaticHouseHelper.IsItemInPlayerOwnedStaticHouse(item))
                    {
                        // Reset del timer di decay
                        item.LastMoved = DateTime.UtcNow;
                        count++;
                    }
                }
                // Log opzionale in console
                // Console.WriteLine($"[PreventStaticHouseItemDecay] Refreshed {count} items in static houses.");
            }
        }
    }
}