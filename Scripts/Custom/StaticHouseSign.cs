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

        [Constructable]
        public StaticHouseSign()
            : base(0xBD2)
        {
            Name = "Insegna Casa Statica";
            Movable = true;
            m_ForSale = false;
            m_ForRent = false;
            m_SalePrice = 0;
            m_RentPrice = 0;
            m_HouseName = null;
            m_Owner = null;
            m_HouseArea = new Rectangle2D(this.X - 5, this.Y - 5, 11, 11);
            m_LastRefresh = DateTime.UtcNow;
            m_DecayPeriod = TimeSpan.FromDays(21);
            m_AssociatedDoors = new List<BaseDoor>();
            m_HouseKeyValue = 0;
        }

        public StaticHouseSign(Serial serial)
            : base(serial)
        {
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
            writer.Write((int)4);

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
                m_DecayPeriod = TimeSpan.FromDays(21);
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

            for (int i = 0; i < toRemove.Count; i++)
            {
                toRemove[i].Delete();
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
    }
}