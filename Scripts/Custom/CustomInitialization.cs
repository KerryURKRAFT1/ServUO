using Server.Misc;

namespace Server.Custom
{
    public class CustomInitialization
    {
        public static void Initialize()
        {
            // Inizializza patch ghost WAR mode
            GhostWarModePatch.Initialize();

            // Qui puoi aggiungere altre inizializzazioni custom future
        }
    }
}