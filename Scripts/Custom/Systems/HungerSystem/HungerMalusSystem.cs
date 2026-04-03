using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

// =============================================================================
// HungerMalusSystem - Sistema avanzato di gestione della fame
// =============================================================================
// Questo sistema sostituisce il decremento della fame in FoodDecayTimer.
// Gestisce:
//   1. Il decremento periodico della fame (ogni 5 minuti, per tutti i giocatori)
//   2. L'applicazione di malus proporzionali alle skill quando il giocatore è affamato
//   3. La rimozione automatica dei malus quando il giocatore si sfama
//
// Il sistema usa EventSink.HungerChanged per reagire in tempo reale a ogni
// variazione di fame (sia per decadimento che per consumo di cibo).
//
// TODO [TasteID]: In futuro sarà possibile aggiungere un bonus basato sulla skill
// TasteID del giocatore. Il bonus ridurrà il malus alla fame in modo proporzionale.
// I metodi placeholder sono documentati nei commenti TODO qui sotto.
// =============================================================================

namespace Server.Custom.HungerSystem
{
    public static class HungerMalusSystem
    {
        // -----------------------------------------------------------------------
        // Configurazione
        // -----------------------------------------------------------------------

        /// <summary>
        /// Soglia di fame (su scala 0-20) al di sotto della quale si applica il malus.
        /// Es.: con soglia 10, il malus scatta quando Hunger è 0-9.
        /// </summary>
        public static int HungerThreshold { get; private set; } = 10;

        /// <summary>
        /// Percentuale massima di malus alle skill (0.0 = nessun malus, 1.0 = 100%).
        /// Con fame a 0 e MaxMalusPercent = 0.50, tutte le skill sono dimezzate.
        /// </summary>
        public static double MaxMalusPercent { get; private set; } = 0.50;

        // Intervallo di decremento della fame (uguale all'originale FoodDecayTimer)
        private static readonly TimeSpan DecayInterval = TimeSpan.FromMinutes(5);

        // -----------------------------------------------------------------------
        // Strutture dati interne
        // -----------------------------------------------------------------------

        // Mappa: Serial.Value → lista di SkillMod applicati per il malus fame
        private static readonly Dictionary<int, List<DefaultSkillMod>> _modsAttivi =
            new Dictionary<int, List<DefaultSkillMod>>();

        // Insieme dei Serial di giocatori attualmente in stato di malus per fame.
        // Usato per inviare la notifica solo al primo ingresso nello stato di fame
        // e alla ripresa, evitando messaggi ripetuti ogni tick.
        private static readonly HashSet<int> _giocatoriAffamati = new HashSet<int>();

        // TODO [TasteID]: Aggiungere qui la struttura per i bonus TasteID.
        // Esempio: dizionario Serial → valore bonus (percentuale riduzione del malus).
        // private static readonly Dictionary<int, double> _bonusTasteID =
        //     new Dictionary<int, double>();

        // -----------------------------------------------------------------------
        // Inizializzazione (chiamata automaticamente da ServUO all'avvio)
        // -----------------------------------------------------------------------

        public static void Initialize()
        {
            // Aggancio all'evento HungerChanged: aggiorna i malus ogni volta
            // che la fame di un mobile cambia (decadimento o consumo di cibo).
            EventSink.HungerChanged += OnHungerChanged;

            // Aggancio all'evento Login: ricalcola i malus al login del giocatore.
            // Necessario perché i SkillMod sono in-memory e vanno riapplicati dopo
            // ogni riavvio del server o primo login con fame già sotto soglia.
            EventSink.Login += OnLogin;

            // Avvia il timer interno per il decremento periodico della fame.
            new DecayTimer().Start();
        }

        // -----------------------------------------------------------------------
        // Hook: EventSink.HungerChanged
        // Richiamato automaticamente ogni volta che Mobile.Hunger cambia.
        // -----------------------------------------------------------------------

        private static void OnHungerChanged(HungerChangedEventArgs e)
        {
            // Considera solo i PlayerMobile vivi
            if (!(e.Mobile is PlayerMobile pm) || pm.Deleted || !pm.Alive)
                return;

            AggiornaMalus(pm);
        }

        // -----------------------------------------------------------------------
        // Hook: EventSink.Login
        // Richiamato quando un giocatore si collega al server.
        // Serve per riapplicare i malus ai giocatori che si ricollegano dopo
        // un riavvio del server con fame già sotto la soglia.
        // -----------------------------------------------------------------------

        private static void OnLogin(LoginEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm) || pm.Deleted)
                return;

            // Controlla e applica i malus in base alla fame corrente
            AggiornaMalus(pm);
        }

        // -----------------------------------------------------------------------
        // Logica principale: calcola e applica/rimuove i malus alle skill
        // -----------------------------------------------------------------------

        /// <summary>
        /// Aggiorna i malus alle skill del mobile in base al suo livello di fame corrente.
        /// Chiamato da OnHungerChanged (automaticamente) e dal timer di decay.
        /// Può essere chiamato anche manualmente (es. da HungerCommand per forzare l'aggiornamento).
        /// </summary>
        public static void AggiornaMalus(Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive)
            {
                // Giocatore non valido: rimuovi eventuali malus residui
                RimuoviMalus(m);
                return;
            }

            int chiave = m.Serial.Value;

            if (m.Hunger < HungerThreshold)
            {
                bool eraGiaAffamato = _giocatoriAffamati.Contains(chiave);

                // Intensità del malus: 0.0 (fame = soglia) → 1.0 (fame = 0)
                double intensita = 1.0 - ((double)m.Hunger / HungerThreshold);

                // Percentuale di malus effettiva
                double percentuale = intensita * MaxMalusPercent;

                // TODO [TasteID]: Ridurre "percentuale" in base alla skill TasteID.
                // Un giocatore con alta TasteID "sente meno" la fame grazie alla
                // sua esperienza con il cibo. Decommentare e implementare GetBonusTasteID.
                //
                // double bonusTasteID = GetBonusTasteID(m);
                // percentuale = Math.Max(0.0, percentuale - bonusTasteID);

                // Rimuovi i vecchi SkillMod e ricalcola con i valori base aggiornati
                RimuoviModDalDizionario(m, chiave);

                var nuoviMod = new List<DefaultSkillMod>();

                for (int i = 0; i < m.Skills.Length; i++)
                {
                    Skill skill = m.Skills[(SkillName)i];

                    // Salta le skill non allenate (valore base 0)
                    if (skill == null || skill.Base <= 0.0)
                        continue;

                    // Malus assoluto (relativo al valore base della skill)
                    double malusValore = -(skill.Base * percentuale);
                    var mod = new DefaultSkillMod((SkillName)i, true, malusValore);
                    m.AddSkillMod(mod);
                    nuoviMod.Add(mod);
                }

                _modsAttivi[chiave] = nuoviMod;
                _giocatoriAffamati.Add(chiave);

                // Notifica il giocatore solo al primo ingresso nello stato di fame
                if (!eraGiaAffamato)
                    m.SendMessage(38, "Sei affamato: le tue abilità sono penalizzate!");
            }
            else
            {
                // Fame >= soglia: giocatore sazio, rimuovi i malus
                bool eraAffamato = _giocatoriAffamati.Contains(chiave);
                RimuoviMalus(m);

                // Notifica il giocatore solo quando recupera dallo stato di fame
                if (eraAffamato)
                    m.SendMessage(63, "Hai mangiato abbastanza: le tue abilità sono tornate alla normalità.");
            }
        }

        // -----------------------------------------------------------------------
        // Rimozione dei malus attivi per un giocatore
        // -----------------------------------------------------------------------

        /// <summary>
        /// Rimuove tutti i malus alle skill attivi per il mobile specificato.
        /// Può essere chiamato manualmente (es. da HungerCommand o alla morte del giocatore).
        /// </summary>
        public static void RimuoviMalus(Mobile m)
        {
            if (m == null)
                return;

            int chiave = m.Serial.Value;
            RimuoviModDalDizionario(m, chiave);
            _modsAttivi.Remove(chiave);
            _giocatoriAffamati.Remove(chiave);
        }

        // Rimuove i SkillMod dal mobile e svuota la lista nel dizionario
        private static void RimuoviModDalDizionario(Mobile m, int chiave)
        {
            if (_modsAttivi.TryGetValue(chiave, out List<DefaultSkillMod> mods))
            {
                foreach (var mod in mods)
                    m.RemoveSkillMod(mod);

                mods.Clear();
            }
        }

        // -----------------------------------------------------------------------
        // Decremento della fame (richiamato dal timer ogni 5 minuti)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Decrementa la fame del mobile di 1.
        /// Il valore non può scendere sotto 0.
        /// I malus vengono aggiornati automaticamente tramite EventSink.HungerChanged.
        /// </summary>
        public static void DecrementaFame(Mobile m)
        {
            if (m != null && m.Hunger >= 1)
                m.Hunger -= 1;
        }

        // -----------------------------------------------------------------------
        // Metodi di utilità pubblica
        // -----------------------------------------------------------------------

        /// <summary>
        /// Restituisce true se il giocatore è attualmente in stato di malus per fame.
        /// </summary>
        public static bool IsAffamato(Mobile m)
        {
            return m != null && _giocatoriAffamati.Contains(m.Serial.Value);
        }

        // -----------------------------------------------------------------------
        // TODO [TasteID]: Metodo placeholder per il futuro bonus TasteID
        // -----------------------------------------------------------------------
        // La skill TasteID (SkillName.TasteID, indice 36) sarà utilizzata in futuro
        // per ridurre il malus alla fame: un giocatore esperto nel "gusto" manterrà
        // le sue abilità meglio anche a stomaco quasi vuoto.
        //
        // Per attivare il sistema:
        //   1. Decommentare il metodo GetBonusTasteID qui sotto.
        //   2. Decommentare le righe "bonusTasteID" nel metodo AggiornaMalus.
        //   3. Aggiustare la formula in base al bilanciamento desiderato.
        //
        // public static double GetBonusTasteID(Mobile m)
        // {
        //     if (m == null) return 0.0;
        //     double tasteID = m.Skills[SkillName.TasteID].Value;
        //     // Formula placeholder: ogni 10 punti TasteID riduce il malus del 5%.
        //     // Con TasteID = 100 si ottiene una riduzione del 25% del malus totale.
        //     // Con TasteID Legendary (120) si ottiene una riduzione del 30%.
        //     return Math.Min(tasteID / 400.0, 0.30);
        // }

        // -----------------------------------------------------------------------
        // Timer interno per il decremento periodico della fame
        // -----------------------------------------------------------------------

        private sealed class DecayTimer : Timer
        {
            public DecayTimer()
                : base(DecayInterval, DecayInterval)
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                // Decrementa la fame di ogni giocatore connesso e vivo.
                // I malus vengono aggiornati in automatico tramite EventSink.HungerChanged
                // che scatta ogni volta che Mobile.Hunger cambia valore.
                foreach (NetState state in NetState.Instances)
                {
                    if (state.Mobile is PlayerMobile pm && pm.Alive)
                        DecrementaFame(pm);
                }
            }
        }
    }
}
