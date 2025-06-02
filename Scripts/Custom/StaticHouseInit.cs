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
            StaticHouseSpeech.Initialize();
            StaticHouseContainerAccess.Initialize();
            PreventStaticHouseItemDecay.Initialize();
        }
    }
}