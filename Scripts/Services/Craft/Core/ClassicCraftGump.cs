using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Engines.Craft
{
    public class ClassicCraftGump : Gump
    {
        private CraftSystem _craftSystem;

        public ClassicCraftGump(Mobile from, CraftSystem craftSystem) : base(50, 50)
        {
            _craftSystem = craftSystem;

            AddPage(0);

            // Utilizzo immagine 2320 come layout principale del gump
            AddImage(0, 0, 2320);

            // Recupero lingotti di ferro (Iron)
            int ironIngotCount = from.Backpack.GetAmount(typeof(IronIngot));

            // Controllo dei materiali e visualizzazione delle categorie nella banda nera
            int x = 70; // Posizione iniziale orizzontale nella banda nera
            int y = 320; // Coordinata Y fissa nella banda nera

            // Aggiunge categorie solo se ci sono lingotti di ferro
            if (ironIngotCount > 0)
            {
                // Categoria: Armi (icona spada)
                AddImage(x, y, 5110); // ID immagine: spada
                x += 80; // Sposta verso destra

                // Categoria: Armature (icona armatura)
                AddImage(x, y, 5109); // ID immagine: armatura
                x += 80;

                // Categoria: Strumenti (icona strumento generica)
                AddImage(x, y, 5120); // ID immagine: strumento
            }
            else
            {
                AddLabel(100, y, 1153, "Non ci sono lingotti di ferro.");
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            from.SendMessage("La gestione della selezione delle categorie verrà implementata successivamente.");
        }
    }
}
