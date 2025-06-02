using Server;
using Server.Items;
using Server.StaticHouse;

namespace Server.StaticHouse
{
    public static class StaticHouseContainerAccess
    {
        public static void Initialize()
        {
            // Hook globale: viene chiamato ogni volta che un player cerca di accedere a un container
            EventSink.OpenContainer += EventSink_OpenContainer;
        }

        private static void EventSink_OpenContainer(OpenContainerEventArgs e)
        {
            Mobile from = e.Mobile;
            Container cont = e.Container;

            // Solo per container statici (mobili fissi in mappa, NON in zaini, NON su altri mobile)
            if (cont != null && from != null && cont.Parent == null && cont.Map != Map.Internal)
            {
                // Cerchiamo una casa statica all’owner dove si trova il container
                var house = StaticHouseHelper.FindStaticHouseAt(cont.Location, cont.Map);

                if (house != null && house.Owner != null)
                {
                    // Permetti solo all’owner (o eventualmente amici, se vuoi aggiungerlo) di aprire il container
                    if (house.Owner == from /* || house.IsFriend(from) */ )
                    {
                        // Forza apertura
                        e.Allow = true;
                    }
                    else
                    {
                        // Blocca apertura
                        e.Allow = false;
                        from.SendMessage("Non puoi accedere a questo contenitore: non sei il proprietario della casa.");
                    }
                }
            }
        }
    }
}