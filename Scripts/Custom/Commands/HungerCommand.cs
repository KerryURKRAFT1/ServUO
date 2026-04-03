using Server;
using Server.Commands;
using Server.Custom.HungerSystem;
using Server.Mobiles;
using Server.Targeting;

// =============================================================================
// HungerCommand - Comando per la gestione della fame
// =============================================================================
// Comandi disponibili:
//   [hunger           → mostra il livello di fame corrente del giocatore
//   [hungerset <0-20> → (solo GameMaster+) seleziona un bersaglio e ne imposta la fame
//
// TODO [TasteID]: In futuro aggiungere la visualizzazione del bonus TasteID attivo
// nel comando [hunger, in modo che il giocatore possa vedere quanto la skill
// TasteID influenza il suo malus alla fame.
// =============================================================================

namespace Server.Commands
{
    public class HungerCommand
    {
        public static void Initialize()
        {
            // Comando per tutti i giocatori: visualizza lo stato di fame
            CommandSystem.Register("Hunger", AccessLevel.Player, new CommandEventHandler(OnHungerCommand));

            // Comando solo per GameMaster+: imposta la fame di un target
            CommandSystem.Register("HungerSet", AccessLevel.GameMaster, new CommandEventHandler(OnHungerSetCommand));
        }

        // -----------------------------------------------------------------------
        // [hunger → mostra lo stato di fame del giocatore che lo usa
        // -----------------------------------------------------------------------

        private static void OnHungerCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m == null)
                return;

            int fame = m.Hunger;
            bool affamato = HungerMalusSystem.IsAffamato(m);

            // Mostra il livello di fame su scala 0-20
            m.SendMessage(63, $"Il tuo livello di fame è: {fame}/20");

            if (affamato)
                m.SendMessage(38, "Sei affamato! Le tue abilità sono penalizzate.");
            else
                m.SendMessage(63, "Non sei affamato. Le tue abilità sono al massimo.");

            // TODO [TasteID]: Aggiungere qui la visualizzazione del bonus TasteID attivo.
            // Esempio: m.SendMessage(53, $"Bonus TasteID attivo: {bonusTasteID:P0}");
        }

        // -----------------------------------------------------------------------
        // [hungerset <valore> → il GM seleziona un target e ne imposta la fame
        // Uso: [hungerset 0  (imposta la fame a 0, massimo affamato)
        //      [hungerset 20 (imposta la fame a 20, sazio)
        // -----------------------------------------------------------------------

        private static void OnHungerSetCommand(CommandEventArgs e)
        {
            Mobile gm = e.Mobile;

            if (gm == null)
                return;

            // Verifica che sia stato fornito il valore come argomento
            if (!int.TryParse(e.ArgString.Trim(), out int nuovaFame) || nuovaFame < 0 || nuovaFame > 20)
            {
                gm.SendMessage(38, "Uso corretto: [HungerSet <valore> (valore tra 0 e 20)");
                return;
            }

            gm.SendMessage(63, $"Seleziona il giocatore a cui impostare la fame a {nuovaFame}.");
            gm.Target = new InternalTarget(nuovaFame);
        }

        // -----------------------------------------------------------------------
        // Target interno per [hungerset: applica il nuovo valore di fame al bersaglio
        // -----------------------------------------------------------------------

        private class InternalTarget : Target
        {
            private readonly int _nuovaFame;

            public InternalTarget(int nuovaFame) : base(-1, false, TargetFlags.None)
            {
                _nuovaFame = nuovaFame;
            }

            protected override void OnTarget(Mobile gm, object targeted)
            {
                if (!(targeted is PlayerMobile pm))
                {
                    gm.SendMessage(38, "Devi selezionare un giocatore valido.");
                    return;
                }

                int vecchiaFame = pm.Hunger;
                pm.Hunger = _nuovaFame;

                // Forza l'aggiornamento immediato dei malus
                HungerMalusSystem.AggiornaMalus(pm);

                gm.SendMessage(63, $"Fame di {pm.Name} impostata da {vecchiaFame} a {_nuovaFame}/20.");
                pm.SendMessage(53, $"Un GM ha modificato il tuo livello di fame a {_nuovaFame}/20.");
            }
        }
    }
}
