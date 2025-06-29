using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Items; // Necessario per BaseDoor e Key

namespace Server.StaticHouse
{
    public class StaticHouseSign : Item
    {
        private string m_HouseName;
        private Mobile m_Owner;
        private bool m_ForSale;
        private int m_SalePrice;
        private bool m_ForRent;
        private int m_RentPrice;
        private Rectangle2D m_HouseArea;
        private DateTime m_LastRefresh;
        private TimeSpan m_DecayPeriod;
        private List<BaseDoor> m_AssociatedDoors;
        private uint m_HouseKeyValue;
        private int m_RequiredKarma;
        private int m_RequiredFame;

        private static readonly TimeSpan DefaultDecay = TimeSpan.FromDays(21);
        private static readonly TimeSpan DowngradedDecay = TimeSpan.FromDays(7);

        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredKarma
        {
            get { return m_RequiredKarma; }
            set { m_RequiredKarma = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredFame
        {
            get { return m_RequiredFame; }
            set { m_RequiredFame = value; InvalidateProperties(); }
        }


        [Constructable]
        public StaticHouseSign()
            : base(0xBD2)
        {
            Name = "Insegna Casa Statica";
            Movable = false;
            m_ForSale = false;
            m_ForRent = false;
            m_SalePrice = 0;
            m_RentPrice = 0;
            m_HouseName = null;
            m_Owner = null;
            m_HouseArea = new Rectangle2D(this.X, this.Y, 0, 0);
            m_LastRefresh = DateTime.UtcNow;
            m_DecayPeriod = DefaultDecay;
            m_AssociatedDoors = new List<BaseDoor>();
            m_HouseKeyValue = 0;
            m_RequiredKarma = 0;
            m_RequiredFame = 0;
        }

        public StaticHouseSign(Serial serial)
            : base(serial)
        {
        }

        public override bool Decays
        {
            get { return false; }
        }
        public string HouseName
        {
            get { return m_HouseName; }
            set { m_HouseName = value; InvalidateProperties(); }
        }
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateProperties(); }
        }
        public bool ForSale
        {
            get { return m_ForSale; }
            set { m_ForSale = value; InvalidateProperties(); }
        }
        public int SalePrice
        {
            get { return m_SalePrice; }
            set { m_SalePrice = value; InvalidateProperties(); }
        }
        public bool ForRent
        {
            get { return m_ForRent; }
            set { m_ForRent = value; InvalidateProperties(); }
        }
        public int RentPrice
        {
            get { return m_RentPrice; }
            set { m_RentPrice = value; InvalidateProperties(); }
        }
        public Rectangle2D HouseArea
        {
            get { return m_HouseArea; }
            set { m_HouseArea = value; }
        }
        public DateTime LastRefresh
        {
            get { return m_LastRefresh; }
            set { m_LastRefresh = value; }
        }
        public TimeSpan DecayPeriod
        {
            get { return m_DecayPeriod; }
            set { m_DecayPeriod = value; }
        }
        public List<BaseDoor> AssociatedDoors
        {
            get { return m_AssociatedDoors; }
        }
        public uint HouseKeyValue
        {
            get { return m_HouseKeyValue; }
            set { m_HouseKeyValue = value; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)5); // versione aggiornata!

            writer.Write(m_HouseName);
            writer.Write(m_Owner);
            writer.Write(m_ForSale);
            writer.Write(m_SalePrice);
            writer.Write(m_ForRent);
            writer.Write(m_RentPrice);

            // Rectangle2D manuale
            writer.Write(m_HouseArea.Start.X);
            writer.Write(m_HouseArea.Start.Y);
            writer.Write(m_HouseArea.Width);
            writer.Write(m_HouseArea.Height);

            writer.Write(m_LastRefresh);
            writer.Write(m_DecayPeriod);

            // Porte abbinate
            writer.Write(m_AssociatedDoors.Count);
            for (int i = 0; i < m_AssociatedDoors.Count; i++)
                writer.Write(m_AssociatedDoors[i]);

            writer.Write(m_HouseKeyValue);

            // Nuovi parametri
            writer.Write(m_RequiredKarma);
            writer.Write(m_RequiredFame);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_HouseName = reader.ReadString();
            m_Owner = reader.ReadMobile();
            m_ForSale = reader.ReadBool();
            m_SalePrice = reader.ReadInt();
            m_ForRent = reader.ReadBool();
            m_RentPrice = reader.ReadInt();

            int x = reader.ReadInt();
            int y = reader.ReadInt();
            int w = reader.ReadInt();
            int h = reader.ReadInt();
            m_HouseArea = new Rectangle2D(x, y, w, h);

            if (version >= 4)
            {
                m_LastRefresh = reader.ReadDateTime();
                m_DecayPeriod = reader.ReadTimeSpan();
            }
            else
            {
                m_LastRefresh = DateTime.UtcNow;
                m_DecayPeriod = DefaultDecay;
                if (version >= 3)
                {
                    reader.ReadTimeSpan();
                    reader.ReadTimeSpan();
                }
                else if (version >= 2)
                {
                    reader.ReadDateTime();
                    reader.ReadTimeSpan();
                }
            }

            m_AssociatedDoors = new List<BaseDoor>();
            if (version >= 2)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    BaseDoor door = reader.ReadItem() as BaseDoor;
                    if (door != null)
                        m_AssociatedDoors.Add(door);
                }
                m_HouseKeyValue = reader.ReadUInt();
            }
            else
            {
                m_HouseKeyValue = 0;
            }

            if (version >= 5)
            {
                m_RequiredKarma = reader.ReadInt();
                m_RequiredFame = reader.ReadInt();
            }
            else
            {
                m_RequiredKarma = 0;
                m_RequiredFame = 0;
            }
        }



        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendGump(new StaticHouseSignGumpGM(this, from));
            }
            else if ((m_ForSale || m_ForRent) && m_Owner == null)
            {
                from.SendGump(new StaticHouseSignGumpBuy(this, from));
            }
            else if (m_Owner == from)
            {
                from.SendGump(new StaticHouseSignGumpOwner(this, from));
            }
            else
            {
                from.SendMessage("Questa casa non è disponibile.");
            }
        }

        public void RefreshDecay()
        {
            m_LastRefresh = DateTime.UtcNow;
        }

        public void CheckDecay()
        {
            // Se non c'è owner, non fare nulla
            if (m_Owner == null)
                return;

            // --- NUOVA LOGICA: se owner NON ha più il titolo richiesto, decay a 7 giorni ---
            bool hasTitle = OwnerHasRequiredTitle();

            if (!hasTitle && m_DecayPeriod != DowngradedDecay)
            {
                m_DecayPeriod = DowngradedDecay;
                m_Owner.SendMessage(33, "Hai perso il titolo richiesto per questa casa: ora il decay è di 7 giorni!");
            }
            else if (hasTitle && m_DecayPeriod != DefaultDecay)
            {
                m_DecayPeriod = DefaultDecay;
                m_Owner.SendMessage(33, "Hai recuperato il titolo richiesto: il decay è tornato normale.");
            }

            if (DateTime.UtcNow > (m_LastRefresh + m_DecayPeriod))
            {
                OnDecayExpired();
            }
        }

        private bool OwnerHasRequiredTitle()
        {
            if (m_Owner == null)
                return false;

            // Confronta titolo attuale con quello richiesto
            string actual = GetTitleFromKarmaFame(m_Owner.Karma, m_Owner.Fame);
            string required = GetTitleFromKarmaFame(m_RequiredKarma, m_RequiredFame);

            // Serve >= titolo richiesto (puoi rendere più sofisticato il confronto)
            return actual == required || OwnerIsHigherTitle(m_Owner.Karma, m_Owner.Fame, m_RequiredKarma, m_RequiredFame);
        }

        private bool OwnerIsHigherTitle(int ownerKarma, int ownerFame, int reqKarma, int reqFame)
        {
            // Usa il punteggio combinato per semplicità (puoi fare una tabella di ranking)
            int ownerScore = Math.Max(ownerFame, Math.Abs(ownerKarma));
            int reqScore = Math.Max(reqFame, Math.Abs(reqKarma));
            return ownerScore > reqScore;
        }

        private void OnDecayExpired()
        {
            // RemoveDoorsInArea(); 
            UnassignKeysFromDoors();
            m_Owner = null;
            m_ForSale = true; // Back for Sale
            m_HouseName = null; // Reset home name !
            InvalidateProperties();
        }

        private void RemoveDoorsInArea()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return;

            IPooledEnumerable e = this.Map.GetItemsInBounds(m_HouseArea);
            List<Item> toRemove = new List<Item>();

            foreach (Item item in e)
            {
                if (item is BaseDoor && m_HouseArea.Contains(item.Location))
                {
                    toRemove.Add(item);
                }
            }
            e.Free();

            for (int i = 0; i < toRemove.Count; i++)
            {
                toRemove[i].Delete();
            }
        }

        // -------- SYSTEM DOORS / KEY  --------

        public void UnassignKeysFromDoors()
        {
            foreach (BaseDoor door in m_AssociatedDoors)
            {
                if (door != null)
                {
                    door.KeyValue = 0;   // Nessuna serratura
                    door.Locked = false; // (opzionale: la porta si apre senza chiave)
                }
            }
            m_HouseKeyValue = 0; // azzera la casa, nuove chiavi future saranno diverse
        }

        private void RemoveKeysFromContainer(Container cont)
        {
            List<Item> toDelete = new List<Item>();
            foreach (Item item in cont.FindItemsByType(typeof(Key), true))
            {
                Key key = item as Key;
                if (key != null && key.KeyValue == m_HouseKeyValue)
                    toDelete.Add(key);
            }
            foreach (Item key in toDelete)
                key.Delete();
        }

        // Per GM: Associa una porta alla casa statica
        public void BeginAssociateDoor(Mobile from)
        {
            from.SendMessage("Seleziona la porta da abbinare a questa casa.");
            from.Target = new DoorTarget(this);
        }

        private class DoorTarget : Target
        {
            private StaticHouseSign m_Sign;
            public DoorTarget(StaticHouseSign sign)
                : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseDoor)
                {
                    BaseDoor door = (BaseDoor)targeted;
                    if (!m_Sign.AssociatedDoors.Contains(door))
                    {
                        m_Sign.AssociatedDoors.Add(door);
                        from.SendMessage("Porta abbinata con successo.");
                    }
                    else
                    {
                        from.SendMessage("Questa porta è già abbinata!");
                    }
                }
                else
                {
                    from.SendMessage("Seleziona una porta valida.");
                }
            }
        }

        // Quando il player compra la casa: imposta KeyValue porte e genera chiavi
        public void AssignKeysToOwner(Mobile newOwner)
        {
            m_HouseKeyValue = Key.RandomValue();
            for (int i = 0; i < m_AssociatedDoors.Count; i++)
            {
                if (m_AssociatedDoors[i] != null)
                {
                    m_AssociatedDoors[i].KeyValue = m_HouseKeyValue;
                    m_AssociatedDoors[i].Locked = true;
                }
            }

            // Genera le chiavi in zaino e banca
            Key key1 = new Key(m_HouseKeyValue);
            key1.Description = string.Format("Chiave di {0}", (this.HouseName != null ? this.HouseName : ""));
            newOwner.AddToBackpack(key1);

            Key key2 = new Key(m_HouseKeyValue);
            key2.Description = string.Format("Chiave di {0}", (this.HouseName != null ? this.HouseName : ""));
            if (newOwner.BankBox != null)
                newOwner.BankBox.DropItem(key2);
        }

        // --- UTILITY: calcola titolo da Karma/Fama ---
        public static string GetTitleFromKarmaFame(int karma, int fame)
        {
            if (fame >= 10000 && karma >= 10000)
                return "Lord";
            if (fame >= 10000 && karma <= -10000)
                return "Dread Lord";
            if (fame >= 10000)
                return "Noble";
            if (fame >= 5000)
                return "Knight";
            if (fame >= 2500)
                return "Squire";
            if (fame >= 1250)
                return "Citizen";
            if (fame >= 625)
                return "Peasant";
            return "Commoner";
        }
    }
}