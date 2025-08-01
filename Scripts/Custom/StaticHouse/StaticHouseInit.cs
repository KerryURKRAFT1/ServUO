using System;
using Server;
using Server.StaticHouse;

namespace Server.StaticHouse
{
    public class StaticHouseInit
    {
        static StaticHouseInit()
        {
            StaticHouseDecayTimer.Initialize();
            PreventStaticHouseItemDecay.Initialize();
        }
    }
}