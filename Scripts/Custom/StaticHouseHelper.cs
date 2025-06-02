using Server;
using System.Collections;

namespace Server.StaticHouse
{
    public static class StaticHouseHelper
    {
        // Trova la casa statica in cui si trova un Mobile
        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
                return null;

            IDictionary items = World.Items;
            foreach (DictionaryEntry entry in items)
            {
                Item item = entry.Value as Item;
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null && sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location))
                    return sign;
            }
            return null;
        }

        // Trova la casa statica in cui si trova una posizione (location,map)
        public static StaticHouseSign FindStaticHouseAt(Point3D location, Map map)
        {
            if (map == null)
                return null;

            IDictionary items = World.Items;
            foreach (DictionaryEntry entry in items)
            {
                Item item = entry.Value as Item;
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null && sign.Map == map && sign.HouseArea.Contains(location))
                    return sign;
            }
            return null;
        }

        // Verifica se un item si trova in una casa statica di un player
        public static bool IsItemInPlayerOwnedStaticHouse(Item item)
        {
            if (item == null || item.Map == null)
                return false;

            IDictionary items = World.Items;
            foreach (DictionaryEntry entry in items)
            {
                StaticHouseSign house = entry.Value as StaticHouseSign;
                if (house != null && house.Owner != null && house.Map == item.Map && house.HouseArea.Contains(item.Location))
                    return true;
            }
            return false;
        }
    }
}