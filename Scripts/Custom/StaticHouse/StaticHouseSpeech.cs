using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Multis;

namespace Server.StaticHouse
{
    public static class StaticHouseSpeech
    {
        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(OnSpeech);
        }

        public static StaticHouseSign FindStaticHouseAt(Mobile mob)
        {
            if (mob == null || mob.Map == null)
            {
                Console.WriteLine("[DEBUG] FindStaticHouseAt: mob nullo o mappa nulla");
                return null;
            }

            foreach (Item item in World.Items.Values)
            {
                StaticHouseSign sign = item as StaticHouseSign;
                if (sign != null)
                {
                    Console.WriteLine("[DEBUG] Analizzo StaticHouseSign: sign.Map={0} mob.Map={1} areaContains={2} sign.Owner.Serial={3} mob.Serial={4}",
                        sign.Map,
                        mob.Map,
                        sign.HouseArea.Contains(mob.Location),
                        sign.Owner != null ? sign.Owner.Serial.ToString() : "null",
                        mob.Serial.ToString()
                    );
                }
                if (sign != null && sign.Map == mob.Map && sign.HouseArea.Contains(mob.Location) && sign.Owner != null && sign.Owner.Serial == mob.Serial)
                {
                    Console.WriteLine("[DEBUG] FindStaticHouseAt: trovata StaticHouseSign valida!");
                    return sign;
                }
            }
            Console.WriteLine("[DEBUG] FindStaticHouseAt: nessuna StaticHouseSign trovata");
            return null;
        }

        // *** ECCO IL METODO MANCANTE ***
        private static void OnSpeech(SpeechEventArgs e)
        {
            Console.WriteLine("[DEBUG] OnSpeech: evento ricevuto. Frase: " + (e == null ? "NULL" : e.Speech));

            if (e.Handled)
            {
                Console.WriteLine("[DEBUG] OnSpeech: e.Handled=true, esco");
                return;
            }
            if (e.Mobile == null)
            {
                Console.WriteLine("[DEBUG] OnSpeech: Mobile nullo, esco");
                return;
            }
            if (!e.Mobile.Player)
            {
                Console.WriteLine("[DEBUG] OnSpeech: Mobile non player, esco");
                return;
            }

            if (BaseHouse.FindHouseAt(e.Mobile) != null)
            {
                Console.WriteLine("[DEBUG] OnSpeech: Sei in una vera BaseHouse, esco");
                return;
            }

            StaticHouseSign house = FindStaticHouseAt(e.Mobile);
            if (house == null)
            {
                Console.WriteLine("[DEBUG] OnSpeech: Nessuna static house trovata o non sei owner");
                return;
            }
            else
            {
                Console.WriteLine("[DEBUG] OnSpeech: StaticHouseSign trovata!");
            }

            string speech = e.Speech.Trim().ToLower();
            Console.WriteLine("[DEBUG] OnSpeech: Frase normalizzata: " + speech);

            if (speech == "i wish to lock this down")
            {
                Console.WriteLine("[DEBUG] OnSpeech: Comando LOCK intercettato!");
                e.Mobile.Target = new StaticLockdownTarget(house);
                e.Mobile.SendMessage("Target the item you wish to lock down.");
                e.Handled = true;
            }
            else if (speech == "i wish to release this")
            {
                Console.WriteLine("[DEBUG] OnSpeech: Comando RELEASE intercettato!");
                e.Mobile.Target = new StaticReleaseTarget(house);
                e.Mobile.SendMessage("Target the item you wish to release.");
                e.Handled = true;
            }
            else if (speech == "i wish to secure this")
            {
                Console.WriteLine("[DEBUG] OnSpeech: Comando SECURE intercettato!");
                e.Mobile.Target = new StaticSecureTarget(house);
                e.Mobile.SendMessage("Target the container you wish to secure.");
                e.Handled = true;
            }
            else if (speech == "i wish to unsecure this")
            {
                Console.WriteLine("[DEBUG] OnSpeech: Comando UNSECURE intercettato!");
                e.Mobile.Target = new StaticReleaseTarget(house);
                e.Mobile.SendMessage("Target the container you wish to unsecure.");
                e.Handled = true;
            }
            else if (speech == "debug static house")
            {
                Console.WriteLine("[DEBUG] OnSpeech: Comando DEBUG intercettato!");
                e.Mobile.SendMessage("DEBUG: Il comando è stato intercettato dal sistema static house!");
                e.Handled = true;
            }
        }

        // *** ANCHE QUESTE CLASSI SONO NECESSARIE (target lockdown/release/secure) ***
        private class StaticLockdownTarget : Target
        {
            private StaticHouseSign m_House;

            public StaticLockdownTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
            {
                m_House = house;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Console.WriteLine("[DEBUG] StaticLockdownTarget: OnTarget chiamato");

                Item item = targeted as Item;
                if (item == null || item.Deleted)
                {
                    Console.WriteLine("[DEBUG] StaticLockdownTarget: Oggetto non valido");
                    from.SendMessage("That's not a valid item.");
                    return;
                }

                if (item.IsLockedDown || item.IsSecure)
                {
                    Console.WriteLine("[DEBUG] StaticLockdownTarget: Oggetto già locked down o secure");
                    from.SendMessage("That item is already locked down or secure.");
                    return;
                }

                if (item.Parent != null || item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
                {
                    Console.WriteLine("[DEBUG] StaticLockdownTarget: Oggetto non sul pavimento della casa");
                    from.SendMessage("That item must be on the floor inside your house.");
                    return;
                }

                item.IsLockedDown = true;
                Console.WriteLine("[DEBUG] StaticLockdownTarget: Oggetto lockato con successo");
                from.SendMessage("Item locked down.");
            }
        }

        private class StaticReleaseTarget : Target
        {
            private StaticHouseSign m_House;

            public StaticReleaseTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
            {
                m_House = house;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Console.WriteLine("[DEBUG] StaticReleaseTarget: OnTarget chiamato");

                Item item = targeted as Item;
                if (item == null || item.Deleted)
                {
                    Console.WriteLine("[DEBUG] StaticReleaseTarget: Oggetto non valido");
                    from.SendMessage("That's not a valid item.");
                    return;
                }

                if (!item.IsLockedDown && !item.IsSecure)
                {
                    Console.WriteLine("[DEBUG] StaticReleaseTarget: Oggetto non locked down o secure");
                    from.SendMessage("That item is not locked down or secured.");
                    return;
                }

                if (item.Parent != null || item.Map != m_House.Map || !m_House.HouseArea.Contains(item.Location))
                {
                    Console.WriteLine("[DEBUG] StaticReleaseTarget: Oggetto non sul pavimento della casa");
                    from.SendMessage("That item must be on the floor inside your house.");
                    return;
                }

                item.IsLockedDown = false;
                item.IsSecure = false;
                Console.WriteLine("[DEBUG] StaticReleaseTarget: Oggetto rilasciato con successo");
                from.SendMessage("Item released.");
            }
        }

        private class StaticSecureTarget : Target
        {
            private StaticHouseSign m_House;

            public StaticSecureTarget(StaticHouseSign house) : base(14, false, TargetFlags.None)
            {
                m_House = house;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Console.WriteLine("[DEBUG] StaticSecureTarget: OnTarget chiamato");

                Container cont = targeted as Container;
                if (cont == null || cont.Deleted)
                {
                    Console.WriteLine("[DEBUG] StaticSecureTarget: Target non è un container valido");
                    from.SendMessage("You can only secure containers.");
                    return;
                }

                if (cont.IsSecure)
                {
                    Console.WriteLine("[DEBUG] StaticSecureTarget: Container già secure");
                    from.SendMessage("That container is already secure.");
                    return;
                }

                if (cont.IsLockedDown)
                {
                    Console.WriteLine("[DEBUG] StaticSecureTarget: Container già locked down");
                    from.SendMessage("That container is already locked down.");
                    return;
                }

                if (cont.Parent != null || cont.Map != m_House.Map || !m_House.HouseArea.Contains(cont.Location))
                {
                    Console.WriteLine("[DEBUG] StaticSecureTarget: Container non sul pavimento della casa");
                    from.SendMessage("That container must be on the floor inside your house.");
                    return;
                }

                cont.IsSecure = true;
                Console.WriteLine("[DEBUG] StaticSecureTarget: Container reso secure con successo");
                from.SendMessage("Container secured.");
            }
        }
    }
}