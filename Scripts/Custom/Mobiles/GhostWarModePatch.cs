using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
    public static class GhostWarModePatch
    {
        public static void Initialize()
        {
            // Ogni secondo forza WAR mode ai morti
            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), ForceGhostsWarMode);
        }

        private static void ForceGhostsWarMode()
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                PlayerMobile pm = m as PlayerMobile;
                if (pm == null)
                    continue;

                if (!pm.Alive && !pm.Warmode)
                    pm.Warmode = true;
            }
        }
    }
}