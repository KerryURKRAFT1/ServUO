using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Custom;
using Server.Multis;

namespace Custom
{
    public static class StaticHouseSpeech
    {
        public static void Initialize()
        {
            EventSink.Speech += OnSpeech;
        }

        // Ricerca una StaticHouseSign che copre la posizione del mobile
        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
                return null;

            foreach (Item item in World.Items.Values)
            {
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null && sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location) && sign.Owner == mob)
                    return sign;
            }
            return null;
        }

        private static void OnSpeech(SpeechEventArgs e)
        {
            if (e.Handled || e.Mobile == null || !e.Mobile.Player)
                return;

            // Se sei già in una vera casa, lascia il comportamento standard
            if (BaseHouse.FindHouseAt(e.Mobile) != null)
                return;

            // Sei in una static house di cui sei owner?
            StaticHouseSign house = FindStaticHouseAt(e.Mobile);
            if (house == null)
                return;

            string speech = e.Speech.Trim().ToLowerInvariant();

            // LOCKDOWN
            if (speech == "i wish to lock this down")
            {
                e.Mobile.Target = new StaticLockdownTarget(house);
                e.Mobile.SendMessage("Target the item you wish to lock down.");
                e.Handled = true;
            }
            // RELEASE
            else if (speech == "i wish to release this")
            {
                e.Mobile.Target = new StaticReleaseTarget(house);
                e.Mobile.SendMessage("Target the item you wish to release.");
                e.Handled = true;
            }
            // SECURE
            else if (speech == "i wish to secure this")
            {
                e.Mobile.Target = new StaticSecureTarget(house);
                e.Mobile.SendMessage("Target the container you wish to secure.");
                e.Handled = true;
            }
            // UNSECURE
            else if (speech == "i wish to unsecure this")
            {
                e.Mobile.Target = new StaticReleaseTarget(house); // Usa lo stesso target del release
                e.Mobile.SendMessage("Target the container you wish to unsecure.");
                e.Handled = true;
            }
            // Puoi aggiungere qui altre frasi come ban/unban/kick owner ecc, seguendo lo stesso schema!
        }

        // --- TARGET PER LOCKDOWN ---
        private class StaticLockdownTarget : Target
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
                    from.SendMessage("That's not a valid item.");
                    return;
                }

                if (item.IsLockedDown || item.IsSecure)
                {
                    from.SendMessage("That item is already locked down or secure.");
                    return;
                }

                if (item.Parent != null || item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
                {
                    from.SendMessage("That item must be on the floor inside your house.");
                    return;
                }

                item.IsLockedDown = true;
                from.SendMessage("Item locked down.");
            }
        }

        // --- TARGET PER RELEASE/UNSECURE ---
        private class StaticReleaseTarget : Target
        {
            private StaticHouseSign m_House;

            public StaticReleaseTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
            {
                m_House = house;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Item item = targeted as Item;
                if (item == null || item.Deleted)
                {
                    from.SendMessage("That's not a valid item.");
                    return;
                }

                if (!item.IsLockedDown && !item.IsSecure)
                {
                    from.SendMessage("That item is not locked down or secured.");
                    return;
                }

                if (item.Parent != null || item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
                {
                    from.SendMessage("That item must be on the floor inside your house.");
                    return;
                }

                item.IsLockedDown = false;
                item.IsSecure = false;
                from.SendMessage("Item released.");
            }
        }

        // --- TARGET PER SECURE ---
        private class StaticSecureTarget : Target
        {
            private StaticHouseSign m_House;

            public StaticSecureTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
            {
                m_House = house;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Container cont = targeted as Container;
                if (cont == null || cont.Deleted)
                {
                    from.SendMessage("You can only secure containers.");
                    return;
                }

                if (cont.IsSecure)
                {
                    from.SendMessage("That container is already secure.");
                    return;
                }

                if (cont.IsLockedDown)
                {
                    from.SendMessage("That container is already locked down.");
                    return;
                }

                if (cont.Parent != null || cont.Map != m_House.Map || !m_House.HouseArea.Contains(cont.Location))
                {
                    from.SendMessage("That container must be on the floor inside your house.");
                    return;
                }

                cont.IsSecure = true;
                from.SendMessage("Container secured.");
            }
        }
    }
}