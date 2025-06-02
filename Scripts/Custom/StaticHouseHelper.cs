using Server;
using System.Collections;

namespace Server.StaticHouse
{
    public static class StaticHouseHelper
    {
        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
                return null;

            foreach (Item item in World.Items.Values)
            {
                var sign = item as StaticHouseSign;
                if (sign != null && sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location))
                    return sign;
            }
            return null;
        }



        // Funzione per evitare il decay degli item dentro case statiche con owner
        public static bool IsItemInPlayerOwnedStaticHouse(Item item)
        {
            if (item == null || item.Map == null)
                return false;

            foreach (Item worldItem in World.Items.Values)
            {
                StaticHouseSign house = worldItem as StaticHouseSign;
                if (house != null && house.Owner != null && house.Map == item.Map && house.HouseArea.Contains(item.Location))
                    return true;
            }
            return false;
        }
    }
}