using System;
using System.Collections;
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
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                int count = 0;
                // Cicla solo le static house
                foreach (Item item in World.Items.Values)
                {
                    StaticHouseSign house = item as StaticHouseSign;
                    if (house == null || house.Owner == null)
                        continue;

                    IPooledEnumerable e = house.Map.GetItemsInBounds(house.HouseArea);
                    foreach (Item inside in e)
                    {
                        if (inside == null || inside.Deleted)
                            continue;
                        inside.LastMoved = DateTime.UtcNow;
                        count++;
                    }
                    e.Free();
                }
                // Console.WriteLine("[PreventStaticHouseItemDecay] Refreshed {0} items in static houses.", count);
            }
        }
    }
}