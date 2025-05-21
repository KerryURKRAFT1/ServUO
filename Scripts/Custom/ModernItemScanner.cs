using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Custom
{
    public class ModernItemScanner
    {
        public static readonly Type[] ModernTypes = new Type[]
        {
            typeof(Sand),
            typeof(DaemonBone),
            typeof(RuneBeetleCarapace),
            typeof(Stormgrip),
            typeof(SwordOfTheStampede),
            typeof(UnforgivenVeil),
            typeof(HailstormHuman),
            typeof(HailstormGargoyle),
            typeof(DespicableQuiver),
            typeof(Cauldron),
            typeof(NaxMarker),
            typeof(Bleach),
            typeof(BasePigmentsOfTokuno),

            // Puoi aggiungere altre classi trovate nella repo!
            // typeof(MinorArtifact), typeof(MajorArtifact), ecc.
        };

        public static void Initialize()
        {
            CommandSystem.Register("ScanModernItems", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        public static void OnCommand(CommandEventArgs e)
        {
            int found = 0;
            List<string> results = new List<string>();

            foreach (Item item in World.Items.Values)
            {
                foreach (Type t in ModernTypes)
                {
                    if (t.IsInstanceOfType(item))
                    {
                        string info = "Trovato: " + t.Name + " - Serial: " + item.Serial;
                        if (item.RootParent is Mobile)
                            info += " - Owner: " + ((Mobile)item.RootParent).Name + " (Player/NPC)";
                        else if (item.Parent != null)
                            info += " - Contenitore: " + item.Parent.GetType().Name;
                        else
                            info += " - Posizione: " + item.Location + " (" + item.Map + ")";

                        results.Add(info);
                        found++;
                        break;
                    }
                }
            }

            if (found == 0)
            {
                e.Mobile.SendMessage(61, "Nessun oggetto moderno trovato nel mondo.");
            }
            else
            {
                e.Mobile.SendMessage(33, "Trovati " + found + " oggetti moderni nel mondo:");
                foreach (string s in results)
                    e.Mobile.SendMessage(33, s);
            }
        }
    }
}

// Ricordati di chiamare l'init in Scripts/Custom/Global.cs o simile:
// Server.Custom.ModernItemScanner.Initialize();