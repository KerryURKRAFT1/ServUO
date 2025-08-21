using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom
{
    public class ModernPetScanner
    {
        // Mantieni solo le classi effettivamente presenti nel tuo shard!
        public static readonly Type[] ModernTypes = new Type[]
        {
            typeof(CuSidhe),
            typeof(Hiryu),
            typeof(LesserHiryu),
            typeof(GreaterDragon),
            typeof(FireBeetle),
            typeof(RuneBeetle),
            typeof(Reptalon),
            typeof(BakeKitsune),
            typeof(Skree),
            typeof(Raptor),
            typeof(SilverSteed),
            typeof(SerpentineDragon),
            typeof(FairyDragon1),
            // Rimuovi o aggiungi qui SOLO classi che ESISTONO nei tuoi script!
        };

        public static void Initialize()
        {
            CommandSystem.Register("ScanModernPets", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        public static void OnCommand(CommandEventArgs e)
        {
            int found = 0;
            List<string> results = new List<string>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                foreach (Type t in ModernTypes)
                {
                    if (t.IsInstanceOfType(m))
                    {
                        // Per proprietà extra controlla se è un BaseCreature
                        BaseCreature bc = m as BaseCreature;
                        string info = "Trovato: " + t.Name + " - Serial: " + m.Serial + " - Posizione: " + m.Location + " (" + m.Map + ")";
                        if (bc != null && bc.Controlled && bc.ControlMaster != null)
                            info += " [Controllato da: " + bc.ControlMaster.Name + "]";
                        else
                            info += " [Libero]";

                        results.Add(info);
                        found++;
                        break;
                    }
                }
            }

            if (found == 0)
            {
                e.Mobile.SendMessage(61, "Nessun pet/mount moderno trovato nel mondo.");
            }
            else
            {
                e.Mobile.SendMessage(33, "Trovati " + found + " pet/mount moderni nel mondo:");
                foreach (string s in results)
                    e.Mobile.SendMessage(33, s);
            }
        }
    }
}

// Ricordati di inizializzare la classe all'avvio del server con:
// Server.Custom.ModernPetScanner.Initialize();