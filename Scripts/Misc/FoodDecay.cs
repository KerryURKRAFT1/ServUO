using System;
using Server.Network;

// MODIFICA: Il decremento della FAME è stato rimosso da questo timer.
// La logica di decremento della fame (e il relativo sistema di malus alle skill)
// è ora gestita interamente da HungerMalusSystem (Scripts/Custom/Systems/HungerSystem).
// Questo timer gestisce SOLO il decremento della SETE.
//
// TODO [TasteID]: Se in futuro si implementa il bonus TasteID, si potrà valutare
// se aggiungere un effetto anche sulla velocità di decadimento della sete.

namespace Server.Misc
{
    public class FoodDecayTimer : Timer
    {
        public FoodDecayTimer()
            : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
            this.Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new FoodDecayTimer().Start();
        }

        // Decrementa la sete del mobile di 1 ogni 5 minuti.
        public static void ThirstDecay(Mobile m)
        {
            if (m != null && m.Thirst >= 1)
                m.Thirst -= 1;
        }

        protected override void OnTick()
        {
            // Solo decremento della sete: la fame è gestita da HungerMalusSystem.
            foreach (NetState state in NetState.Instances)
            {
                ThirstDecay(state.Mobile);
            }
        }
    }
}