using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Commands
{
    public class FindQuestGivers
    {
        // Inserisci qui i nomi delle classi dei quest giver (SOLO il nome, NON il namespace!)
        private static readonly string[] QuestGiverTypeNames = new string[]
        {
            "Victoria",
            "Uzeraan",
            "Emino",
            "Haochi",
            "Hawkwind",
            "JukariHighChief",
            "KurakHighChief",
            "BarakoHighChief",
            "Vernix"
            // aggiungi altri nomi se necessario!
        };

        public static void Initialize()
        {
            CommandSystem.Register("FindQuestGivers", AccessLevel.GameMaster, new CommandEventHandler(FindQuestGivers_OnCommand));
        }

        [Usage("FindQuestGivers [Go]")]
        [Description("Cerca e mostra la posizione degli NPC quest giver nel mondo. Se usi l'opzione 'Go', ti teletrasporta sopra il primo trovato.")]
        public static void FindQuestGivers_OnCommand(CommandEventArgs e)
        {
            bool shouldGo = false;
            if (e.Arguments.Length > 0 && e.Arguments[0].ToLower() == "go")
                shouldGo = true;

            Mobile from = e.Mobile;
            int found = 0;

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob == null || mob.Deleted)
                    continue;

                string mobTypeName = mob.GetType().Name;

                foreach (string questGiverName in QuestGiverTypeNames)
                {
                    if (mobTypeName == questGiverName)
                    {
                        found++;
                        string msg = string.Format("Trovato: {0} ({1}) a {2} [{3}]", mob.Name, mob.GetType().Name, mob.Location, mob.Map);
                        from.SendMessage(0x44, msg);

                        if (shouldGo)
                        {
                            from.Map = mob.Map;
                            from.Location = mob.Location;
                            from.SendMessage(0x44, string.Format("Teletrasportato su {0} ({1})", mob.Name, mob.GetType().Name));
                            return; // Teletrasporta solo sul primo trovato
                        }
                    }
                }
            }

            if (found == 0)
                from.SendMessage(0x22, "Nessun quest giver trovato nel mondo!");
            else if (!shouldGo)
                from.SendMessage(0x44, string.Format("Totale quest giver trovati: {0}", found));
        }
    }
}