using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server;
using Server.Multis;
using Server.Accounting;

namespace Server.StaticHouse
{
    public static class StaticHouseHelper
    {
        // Finds all static houses in the world
        private static IList GetAllStaticHouseSigns()
        {
            ArrayList list = new ArrayList();
            foreach (Item item in World.Items.Values)
            {
                if (item is StaticHouseSign sign)
                    list.Add(sign);
            }
            return list;
        }

        // Returns true if the Mobile (or any of his characters) owns a static house
        public static bool HasStaticHouse(Mobile m)
        {
            Account account = m.Account as Account;
            if (account == null)
                return false;

            for (int i = 0; i < account.Length; ++i)
            {
                Mobile mob = account[i];
                if (mob != null && PlayerOwnsStaticHouse(mob))
                    return true;
            }
            return false;
        }

        // Returns true if the Mobile is the owner of at least one static house
        public static bool PlayerOwnsStaticHouse(Mobile m)
        {
            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                if (sign.Owner == m)
                    return true;
            }
            return false;
        }

        // Combined check: true if the Mobile (or any of his characters) has a standard (BaseHouse) OR static house
        public static bool HasAnyHouse(Mobile m)
        {
            // Dynamic/standard house
            if (BaseHouse.HasAccountHouse(m))
                return true;

            // Static house
            if (HasStaticHouse(m))
                return true;

            return false;
        }

        // Find the static house where the Mobile is located
        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
                return null;

            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                // VECCHIO SISTEMA (un solo rettangolo)
                // if (sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location))
                //     return sign;

                // NUOVO SISTEMA: più rettangoli
                if (sign.Map == mob.Map && sign.IsInsideHouse(mob.Location))
                    return sign;
            }
            return null;
        }

        // Find the static house at a specific location
        public static StaticHouseSign FindStaticHouseAt(Point3D location, Map map)
        {
            if (map == null)
                return null;

            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                // VECCHIO SISTEMA
                // if (sign.Map == map && sign.HouseArea.Contains(location))
                //     return sign;

                // NUOVO SISTEMA
                if (sign.Map == map && sign.IsInsideHouse(location))
                    return sign;
            }
            return null;
        }

        // Checks if an item is inside a player-owned static house
        public static bool IsItemInPlayerOwnedStaticHouse(Item item)
        {
            if (item == null || item.Map == null)
                return false;

            foreach (StaticHouseSign house in GetAllStaticHouseSigns())
            {
                // VECCHIO SISTEMA
                // if (house.Owner != null && house.Map == item.Map && house.HouseArea.Contains(item.Location))
                //     return true;

                // NUOVO SISTEMA
                if (house.Owner != null && house.Map == item.Map && house.IsInsideHouse(item.Location))
                    return true;
            }
            return false;
        }
    }

    public class StaticLockdownTarget : Target
    {
        private StaticHouseSign m_House;
        private Key m_Key;

        //public StaticLockdownTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
        public StaticLockdownTarget(StaticHouseSign house, Key key) : base(14, false, TargetFlags.None)
        {
            m_House = house;
            m_Key = key;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            Item item = targeted as Item;

            if (!(m_House.Owner == from || m_House.IsCoOwner(from)))
            {
                from.SendMessage("Only the owner or co-owners can lock or unlock items in this house.");
                return;
            }
            if (item == null || item.Deleted)
            {
                from.SendMessage("Invalid item.");
                return;
            }
            if (item is BaseDoor door)
            {
                // Solo se la porta ha lo stesso valore della chiave usata!
                if (door.KeyValue == m_Key.KeyValue)
                {
                    door.Locked = !door.Locked;
                    if (door.Locked)
                        item.SendLocalizedMessageTo(from, 1048000); // You lock it.
                    else
                        item.SendLocalizedMessageTo(from, 1048001); // You unlock it.
                }
                else
                {
                    from.SendMessage("This key does not fit this door.");
                }
                return;
            }
            if (item.Parent != null)
            {
                from.SendMessage("You can only lock down items placed on the ground.");
                return;
            }
            // VECCHIO SISTEMA
            // if (item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
            // {
            //     from.SendMessage("The item must be inside your static house.");
            //     return;
            // }
            // NUOVO SISTEMA
            if (item.Map != m_House.Map || !m_House.IsInsideHouse(item.Location))
            {
                from.SendMessage("The item must be inside your static house.");
                return;
            }
            if (item is StaticHouseSign || item is Key)
            {
                from.SendMessage("You cannot lock down this type of item.");
                return;
            }

            // TOGGLE: lock or unlock
            if (!item.Movable)
            {
                item.Movable = true;
                item.SendLocalizedMessageTo(from, 1048001); // You unlock it.
            }
            else
            {
                item.Movable = false;
                item.SendLocalizedMessageTo(from, 1048000); // You lock it.
            }
        }
    }
}