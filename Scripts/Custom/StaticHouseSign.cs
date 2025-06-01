using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

namespace Custom
{
    public class StaticHouseSign : Item
    {
        private string m_HouseName;
        private int m_SalePrice;
        private int m_RentPrice;
        private Rectangle2D m_HouseArea;
        private Mobile m_Owner;
        private DateTime m_LastRefresh;
        private TimeSpan m_DecayPeriod;
        private bool m_ForSale;
        private bool m_ForRent;

        // Nuove proprietà per porte/chiavi
        private List<BaseDoor> m_AssociatedDoors;
        private uint m_HouseKeyValue;

        [CommandProperty(AccessLevel.GameMaster)]
        public List<BaseDoor> AssociatedDoors { get { return m_AssociatedDoors; } set { m_AssociatedDoors = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint HouseKeyValue { get { return m_HouseKeyValue; } set { m_HouseKeyValue = value; } }

        [Constructable]
        public StaticHouseSign()
            : base(0xBD2)
        {
            this.Name = "static house sign";
            this.Movable = false;
            this.m_DecayPeriod = TimeSpan.FromDays(14.0); // default 2 settimane
            this.m_LastRefresh = DateTime.UtcNow;

            m_AssociatedDoors = new List<BaseDoor>();
        }

        public StaticHouseSign(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string HouseName
        {
            get { return m_HouseName; }
            set { m_HouseName = value; InvalidateProperties(); }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SalePrice
        {
            get { return m_SalePrice; }
            set { m_SalePrice = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RentPrice
        {
            get { return m_RentPrice; }
            set { m_RentPrice = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D HouseArea
        {
            get { return m_HouseArea; }
            set { m_HouseArea = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForSale
        {
            get { return m_ForSale; }
            set { m_ForSale = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForRent
        {
            get { return m_ForRent; }
            set { m_ForRent = value; }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }
        public DateTime LastRefresh { get { return m_LastRefresh; } }
        public TimeSpan DecayPeriod { get { return m_DecayPeriod; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("Static City House");
            list.Add("Name: {0}", m_HouseName != null ? m_HouseName : "N/A");
            if (m_ForSale)
                list.Add("For Sale: {0} gp", m_SalePrice);
            else if (m_ForRent)
                list.Add("For Rent: {0} gp/week", m_RentPrice);
            else if (m_Owner != null)
                list.Add("Owner: {0}", m_Owner.Name);
            else
                list.Add("Unowned");

            if (m_Owner != null)
            {
                TimeSpan left = (m_LastRefresh + m_DecayPeriod) - DateTime.UtcNow;
                if (left < TimeSpan.Zero) left = TimeSpan.Zero;
                list.Add("Decay in: {0} days", left.Days);
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
            if (m_Owner != null && DateTime.UtcNow > (m_LastRefresh + m_DecayPeriod))
            {
                OnDecayExpired();
            }
        }

        private void OnDecayExpired()
        {
            RemoveDoorsInArea();
            m_Owner = null;
            m_ForSale = true; // Torna in vendita
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

            foreach (Item door in toRemove)
            {
                door.Delete();
            }
        }

        // -------- SISTEMA PORTE/CHIAVI --------

        // Per GM: Associa una porta alla casa statica
        public void BeginAssociateDoor(Mobile from)
        {
            from.SendMessage("Seleziona la porta da abbinare a questa casa.");
            from.Target = new DoorTarget(this);
        }

        private class DoorTarget : Target
        {
            private StaticHouseSign m_Sign;
            public DoorTarget(StaticHouseSign sign) : base(10, false, TargetFlags.None)
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
            key1.Description = "Chiave di " + (this.HouseName != null ? this.HouseName : "");
            Key key2 = new Key(m_HouseKeyValue);
            key2.Description = "Chiave di " + (this.HouseName != null ? this.HouseName : "");

            if (newOwner.Backpack != null)
                newOwner.Backpack.DropItem(key1);
            if (newOwner.BankBox != null)
                newOwner.BankBox.DropItem(key2);
        }

        // -------- SERIALIZZAZIONE ESTESA --------
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(m_HouseName);
            writer.Write(m_SalePrice);
            writer.Write(m_RentPrice);
            writer.Write(m_HouseArea);
            writer.Write(m_ForSale);
            writer.Write(m_ForRent);
            writer.Write(m_Owner);
            writer.Write(m_LastRefresh);
            writer.Write(m_DecayPeriod);

            writer.Write(m_HouseKeyValue);

            if (m_AssociatedDoors == null)
                m_AssociatedDoors = new List<BaseDoor>();

            writer.Write(m_AssociatedDoors.Count);
            for (int i = 0; i < m_AssociatedDoors.Count; i++)
            {
                writer.Write(m_AssociatedDoors[i]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_HouseName = reader.ReadString();
            m_SalePrice = reader.ReadInt();
            m_RentPrice = reader.ReadInt();
            m_HouseArea = reader.ReadRect2D();
            m_ForSale = reader.ReadBool();
            m_ForRent = reader.ReadBool();
            m_Owner = reader.ReadMobile();
            m_LastRefresh = reader.ReadDateTime();
            m_DecayPeriod = reader.ReadTimeSpan();

            if (version >= 1)
            {
                m_HouseKeyValue = reader.ReadUInt();
                int doorCount = reader.ReadInt();
                m_AssociatedDoors = new List<BaseDoor>();
                for (int i = 0; i < doorCount; i++)
                {
                    BaseDoor door = reader.ReadItem() as BaseDoor;
                    if (door != null)
                        m_AssociatedDoors.Add(door);
                }
            }
            else
            {
                m_HouseKeyValue = 0;
                m_AssociatedDoors = new List<BaseDoor>();
            }
        }
    }
}