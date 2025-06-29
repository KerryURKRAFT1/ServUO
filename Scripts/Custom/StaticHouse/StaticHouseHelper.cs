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
        // Trova tutte le case statiche nel mondo
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

        // Ritorna true se il Mobile (o uno dei suoi personaggi) possiede una casa statica
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

        // Ritorna true se il Mobile è owner di almeno una casa statica
        public static bool PlayerOwnsStaticHouse(Mobile m)
        {
            foreach (StaticHouseSign sign in GetAllStaticHouseSigns())
            {
                if (sign.Owner == m)
                    return true;
            }
            return false;
        }

        // Controllo combinato: true se il Mobile (o uno dei suoi personaggi) ha casa normale (BaseHouse) OPPURE casa statica
        public static bool HasAnyHouse(Mobile m)
        {
            // Casa dinamica/normale
            if (BaseHouse.HasAccountHouse(m))
                return true;

            // Casa statica
            if (HasStaticHouse(m))
                return true;

            return false;
        }

        // Trova la casa statica in cui si trova il Mobile
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

        // Trova la casa statica a una location specifica
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

        // Verifica se un item è dentro una casa statica di un player
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

    public class StaticLockdownTarget : Target
    {
        private StaticHouseSign m_House;

        public StaticLockdownTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
        {
            m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            Item item = targeted as Item;

            if (item == null || item.Deleted)
            {
                from.SendMessage("Oggetto non valido.");
                return;
            }
            if (item.Parent != null)
            {
                from.SendMessage("Puoi bloccare solo oggetti posati a terra.");
                return;
            }
            if (item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
            {
                from.SendMessage("L'oggetto deve essere all'interno della tua casa statica.");
                return;
            }
            if (item is StaticHouseSign || item is Key)
            {
                from.SendMessage("Non puoi bloccare questo tipo di oggetto.");
                return;
            }

            // TOGGLE: blocca o sblocca
            if (!item.Movable)
            {
                item.Movable = true;
                // item.LockedDown = false; // opzionale, se hai la property
                from.SendMessage("Oggetto sbloccato: ora puoi raccoglierlo!");
            }
            else
            {
                item.Movable = false;
                // item.LockedDown = true; // opzionale, se hai la property
                from.SendMessage("Oggetto bloccato a terra nella casa!");
            }
        }
    }
}