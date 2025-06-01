using Server;
using Custom;

public class StaticHouseInit
{
    static StaticHouseInit()
    {
        StaticHouseDecayTimer.Initialize(); // Se serve ancora questa
        StaticHouseSpeech.Initialize(); // <<< AGGIUNGI QUESTA RIGA QUI
    }
}