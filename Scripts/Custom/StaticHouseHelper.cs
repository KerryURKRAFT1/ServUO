using System;
using System.Collections;
using Server;

namespace Server.StaticHouse
{
    public static class StaticHouseHelper
    {
        // Trova tutte le case statiche
        private static IList GetAllStaticHouseSigns()
        {
            ArrayList list = new ArrayList();
            foreach (Item item in World.Items.Values)
            {
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null)
                    list.Add(sign);
            }
            return list;
        }

        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
                return null;

            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                if (sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location))
                    return sign;
            }
            return null;
        }

        public static StaticHouseSign FindStaticHouseAt(Point3D location, Map map)
        {
            if (map == null)
                return null;

            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                if (sign.Map == map && sign.HouseArea.Contains(location))
                    return sign;
            }
            return null;
        }

        public static bool IsItemInPlayerOwnedStaticHouse(Item item)
        {
            if (item == null || item.Map == null)
                return false;

            foreach (StaticHouseSign house in GetAllStaticHouseSigns())
            {
                if (house.Owner != null && house.Map == item.Map && house.HouseArea.Contains(item.Location))
                    return true;
            }
            return false;
        }
    }
}