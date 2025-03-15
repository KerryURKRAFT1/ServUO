using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Engines.Craft
{
    public class ClassicCraftGump : Gump
    {
        private CraftSystem _craftSystem;

        public ClassicCraftGump(Mobile from, CraftSystem craftSystem) : base(50, 50)
        {
            _craftSystem = craftSystem;

            AddPage(0);

            // Sfondo rettangolare nero
            AddBackground(0, 0, 400, 300, 5054);

            // Titolo
            AddLabel(150, 20, 0, "Crafting Menu (Pre-AoS)");

            // Mostra le categorie basate sui materiali disponibili
            AddCategories(from);

            // Pulsante per chiudere il menu
            AddButton(40, 200, 4005, 4007, 0, GumpButtonType.Reply, 0); // Pulsante Cancel
            AddLabel(80, 200, 0, "Annulla");
        }

        private void AddCategories(Mobile from)
        {
            int y = 60; // Posizione verticale iniziale

            foreach (CraftGroup group in _craftSystem.CraftGroups)
            {
                bool hasEnoughMaterials = false;

                // Controlla gli oggetti nel gruppo
                foreach (CraftItem item in group.CraftItems)
                {
                    if (HasMaterials(from, item))
                    {
                        hasEnoughMaterials = true;
                        break;
                    }
                }

                // Mostra la categoria solo se ha materiali sufficienti
                if (hasEnoughMaterials)
                {
                    AddButton(40, y, 4005, 4007, group.GroupID, GumpButtonType.Reply, 0); // Pulsante categoria
                    AddLabel(80, y, 0, group.Name); // Nome categoria
                    y += 40; // Sposta in basso per il prossimo pulsante
                }
            }
        }

        private bool HasMaterials(Mobile from, CraftItem item)
        {
            foreach (CraftRes res in item.Resources)
            {
                int count = from.Backpack.GetAmount(res.ItemType); // Ottieni quantità disponibile nello zaino
                if (count < res.Amount)
                {
                    return false; // Non ci sono abbastanza materiali
                }
            }
            return true; // Materiali sufficienti
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0) // Annulla
            {
                from.SendMessage("Hai chiuso il menu di crafting.");
                return;
            }

            CraftGroup selectedGroup = _craftSystem.GetGroupByID(info.ButtonID);

            if (selectedGroup != null)
            {
                from.SendMessage("Hai selezionato la categoria: " + selectedGroup.Name);
                // Qui possiamo aprire un sotto-menu per il gruppo selezionato
                from.SendGump(new CraftGroupGump(from, _craftSystem, selectedGroup));
            }
            else
            {
                from.SendMessage("Categoria non valida.");
            }
        }
    }
}
