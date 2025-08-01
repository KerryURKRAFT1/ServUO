using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Items; // Needed for BaseDoor and Key

namespace Server.StaticHouse
{
    [Flipable(0x0BD1, 0xBD2)]
    public class StaticHouseSign : Item
    {
        public string OriginalHouseName { get; set; }
        private string m_HouseName;
        private Mobile m_Owner;
        private bool m_ForSale;
        private int m_SalePrice;
        private bool m_ForRent;
        private int m_RentPrice;

        // --- VECCHIO SISTEMA: UN SOLO RETTANGOLO ---
        //private Rectangle2D m_HouseArea; // <<--- COMMENTATO MA NON RIMOSSO
        // public Rectangle2D HouseArea { get { return m_HouseArea; } set { m_HouseArea = value; } } // <<--- COMMENTATO MA NON RIMOSSO

        // --- NUOVO SISTEMA: LISTA DI RETTANGOLI ---
        private List<Rectangle2D> m_HouseAreas = new List<Rectangle2D>();

        public List<Rectangle2D> HouseAreas
        {
            get { return m_HouseAreas; }
            set { m_HouseAreas = value; }
        }

        private DateTime m_LastRefresh;
        private TimeSpan m_DecayPeriod;
        private List<BaseDoor> m_AssociatedDoors;
        private uint m_HouseKeyValue;
        private int m_RequiredKarma;
        private int m_RequiredFame;

        private static readonly TimeSpan DefaultDecay = TimeSpan.FromDays(21);
        private static readonly TimeSpan DowngradedDecay = TimeSpan.FromDays(7);

        /// <summary>
        /// /  FRIENDS TAB
        /// </summary>
        private List<Mobile> m_Friends = new List<Mobile>();
        private List<Mobile> m_CoOwners = new List<Mobile>();
        private List<Mobile> m_Bans = new List<Mobile>();

        public List<Mobile> Friends => m_Friends;
        public List<Mobile> CoOwners => m_CoOwners;
        public List<Mobile> Bans => m_Bans;

        public void AddFriend(Mobile m) { if (!m_Friends.Contains(m)) m_Friends.Add(m); }
        public void RemoveFriend(Mobile m) { m_Friends.Remove(m); }
        public void ClearFriends() { m_Friends.Clear(); }

        public void AddCoOwner(Mobile m) { if (!m_CoOwners.Contains(m)) m_CoOwners.Add(m); }
        public void RemoveCoOwner(Mobile m) { m_CoOwners.Remove(m); }
        public void ClearCoOwners() { m_CoOwners.Clear(); }

        public void AddBan(Mobile m) { if (!m_Bans.Contains(m)) m_Bans.Add(m); }
        public void RemoveBan(Mobile m) { m_Bans.Remove(m); }
        public void ClearBans() { m_Bans.Clear(); }

        public bool IsFriend(Mobile m) => m_Owner == m || m_Friends.Contains(m) || m_CoOwners.Contains(m);
        public bool IsCoOwner(Mobile m) => m_Owner == m || m_CoOwners.Contains(m);
        public bool IsBanned(Mobile m) => m_Bans.Contains(m);

        public StaticHouseDefaults DefaultSettings = new StaticHouseDefaults();

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
            Name = "Static House Sign";
            Movable = true; // MODIFICA: ora è movibile!
            m_ForSale = false;
            m_ForRent = false;
            m_SalePrice = 0;
            m_RentPrice = 0;
            m_HouseName = null;
            m_Owner = null;
            //m_HouseArea = new Rectangle2D(this.X, this.Y, 0, 0); // VECCHIO SISTEMA: LASCIARE PER COMPATIBILITÀ
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
            set
            {
                m_HouseName = value;
                Name = !string.IsNullOrEmpty(m_HouseName) ? m_HouseName : "Static House Sign"; // MODIFICA: aggiorna il nome visualizzato!
                InvalidateProperties();
            }
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

        // --- VECCHIO SISTEMA ---
        // public Rectangle2D HouseArea
        // {
        //     get { return m_HouseArea; }
        //     set { m_HouseArea = value; }
        // }

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

        // --- NUOVA FUNZIONE: VERIFICA SE UN PUNTO È DENTRO LA CASA (ANY RECTANGLE) ---
        public bool IsInsideHouse(Point3D loc)
        {
            foreach (var rect in m_HouseAreas)
                //if (rect.Contains(loc.X, loc.Y))
                if (rect.Contains(new Point2D(loc.X, loc.Y))) // NUOVO SISTEMA
                    return true;
            return false;
        }

        public bool IsInsideHouse(Point2D loc)
        {
            foreach (var rect in m_HouseAreas)
                if (rect.Contains(loc))
                    return true;
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)6); // updated version! Incrementa la versione

            writer.Write(m_HouseName);
            writer.Write(m_Owner);
            writer.Write(m_ForSale);
            writer.Write(m_SalePrice);
            writer.Write(m_ForRent);
            writer.Write(m_RentPrice);

            // Rectangle2D manual (VECCHIO SISTEMA)
            //writer.Write(m_HouseArea.Start.X);
            //writer.Write(m_HouseArea.Start.Y);
            //writer.Write(m_HouseArea.Width);
            //writer.Write(m_HouseArea.Height);

            // --- NUOVO SISTEMA: SERIALIZZAZIONE LISTA DI RETTANGOLI ---
            writer.Write(m_HouseAreas.Count);
            foreach (var rect in m_HouseAreas)
            {
                writer.Write(rect.Start.X);
                writer.Write(rect.Start.Y);
                writer.Write(rect.Width);
                writer.Write(rect.Height);
            }

            writer.Write(m_LastRefresh);
            writer.Write(m_DecayPeriod);

            // Associated doors
            writer.Write(m_AssociatedDoors.Count);
            for (int i = 0; i < m_AssociatedDoors.Count; i++)
                writer.Write(m_AssociatedDoors[i]);

            writer.Write(m_HouseKeyValue);

            // New parameters
            writer.Write(m_RequiredKarma);
            writer.Write(m_RequiredFame);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_HouseName = reader.ReadString();
            Name = !string.IsNullOrEmpty(m_HouseName) ? m_HouseName : "Static House Sign"; // MODIFICA: nome sempre valorizzato!
            m_Owner = reader.ReadMobile();
            m_ForSale = reader.ReadBool();
            m_SalePrice = reader.ReadInt();
            m_ForRent = reader.ReadBool();
            m_RentPrice = reader.ReadInt();

            // Vecchio rettangolo (compatibilità)
            //int x = reader.ReadInt();
            //int y = reader.ReadInt();
            //int w = reader.ReadInt();
            //int h = reader.ReadInt();
            //m_HouseArea = new Rectangle2D(x, y, w, h);

            // --- NUOVO: LEGGI LA LISTA DI RETTANGOLI ---
            int count = reader.ReadInt();
            m_HouseAreas = new List<Rectangle2D>(count);
            for (int i = 0; i < count; i++)
            {
                int rx = reader.ReadInt();
                int ry = reader.ReadInt();
                int rw = reader.ReadInt();
                int rh = reader.ReadInt();
                m_HouseAreas.Add(new Rectangle2D(rx, ry, rw, rh));
            }

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
                int doorCount = reader.ReadInt();
                for (int i = 0; i < doorCount; i++)
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

        /// <summary>
        /// /  FOR DEFAULT SETTINGS
        /// </summary>
        public class StaticHouseDefaults
        {
            public string HouseName;
            public int SalePrice;
            public int RentPrice;
            // public Rectangle2D HouseArea; // VECCHIO SISTEMA
            public List<Rectangle2D> HouseAreas = new List<Rectangle2D>(); // NUOVO SISTEMA
            public bool ForSale;
            public bool ForRent;
            public int RequiredKarma;
            public int RequiredFame;
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
            else if (IsCoOwner(from))
            {
                from.SendGump(new StaticHouseSignGumpCoOwner(this, from));
            }
            else
            {
                from.SendMessage("This house is not available.");
            }
        }

        public void RefreshDecay()
        {
            m_LastRefresh = DateTime.UtcNow;
        }

        public void CheckDecay()
        {
            if (m_Owner == null)
                return;

            bool hasTitle = OwnerHasRequiredTitle();

            if (!hasTitle && m_DecayPeriod != DowngradedDecay)
            {
                m_DecayPeriod = DowngradedDecay;
                m_Owner.SendMessage(33, "You have lost the required title for this house: decay is now set to 7 days!");
            }
            else if (hasTitle && m_DecayPeriod != DefaultDecay)
            {
                m_DecayPeriod = DefaultDecay;
                m_Owner.SendMessage(33, "You have regained the required title: decay is now back to normal.");
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

            string actual = GetTitleFromKarmaFame(m_Owner.Karma, m_Owner.Fame);
            string required = GetTitleFromKarmaFame(m_RequiredKarma, m_RequiredFame);

            return actual == required || OwnerIsHigherTitle(m_Owner.Karma, m_Owner.Fame, m_RequiredKarma, m_RequiredFame);
        }

        private bool OwnerIsHigherTitle(int ownerKarma, int ownerFame, int reqKarma, int reqFame)
        {
            int ownerScore = Math.Max(ownerFame, Math.Abs(ownerKarma));
            int reqScore = Math.Max(reqFame, Math.Abs(reqKarma));
            return ownerScore > reqScore;
        }

        private void OnDecayExpired()
        {
            // RemoveDoorsInArea();
            UnassignKeysFromDoors();
            m_Owner = null;
            m_ForSale = true;
            m_HouseName = null;
            InvalidateProperties();
        }

        // --- VECCHIO SISTEMA: SOLO 1 RETTANGOLO ---
        // private void RemoveDoorsInArea()
        // {
        //     if (this.Map == null || this.Map == Map.Internal)
        //         return;
        //
        //     IPooledEnumerable e = this.Map.GetItemsInBounds(m_HouseArea);
        //     List<Item> toRemove = new List<Item>();
        //
        //     foreach (Item item in e)
        //     {
        //         if (item is BaseDoor && m_HouseArea.Contains(item.Location))
        //         {
        //             toRemove.Add(item);
        //         }
        //     }
        //     e.Free();
        //
        //     for (int i = 0; i < toRemove.Count; i++)
        //     {
        //         toRemove[i].Delete();
        //     }
        // }

        // --- NUOVO SISTEMA: SU TUTTI I RETTANGOLI ---
        private void RemoveDoorsInArea()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return;

            List<Item> toRemove = new List<Item>();
            foreach (var rect in m_HouseAreas)
            {
                IPooledEnumerable e = this.Map.GetItemsInBounds(rect);
                foreach (Item item in e)
                {
                    if (item is BaseDoor && rect.Contains(item.Location))
                    {
                        toRemove.Add(item);
                    }
                }
                e.Free();
            }

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
                    door.KeyValue = 0;
                    door.Locked = false;
                }
            }
            m_HouseKeyValue = 0;
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

        public void BeginAssociateDoor(Mobile from, Gump gumpToReturn)
        {
            from.SendMessage("Select the door to associate with this house.");
            from.Target = new DoorTarget(this, gumpToReturn);
        }
        public void BeginAssociateDoor(Mobile from)
        {
            from.SendMessage("Select the door to associate with this house.");
            from.Target = new DoorTarget(this, null);
        }

        private class DoorTarget : Target
        {
            private StaticHouseSign m_Sign;
            private Gump m_ReturnGump;

            public DoorTarget(StaticHouseSign sign, Gump gumpToReturn)
                : base(10, false, TargetFlags.None)
            {
                m_Sign = sign;
                m_ReturnGump = gumpToReturn;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseDoor door)
                {
                    if (!m_Sign.AssociatedDoors.Contains(door))
                    {
                        m_Sign.AssociatedDoors.Add(door);
                        from.SendMessage("Door successfully associated.");
                    }
                    else
                    {
                        from.SendMessage("This door is already associated!");
                    }
                }
                else
                {
                    from.SendMessage("Select a valid door.");
                }

                if (m_ReturnGump != null)
                    from.SendGump(m_ReturnGump);
            }
        }

        public void AssignKeysToOwner(Mobile newOwner)
        {
            if (m_AssociatedDoors == null || m_AssociatedDoors.Count == 0)
                return;

            foreach (BaseDoor door in m_AssociatedDoors)
            {
                if (door != null)
                {
                    uint keyVal = Key.RandomValue();
                    door.KeyValue = keyVal;
                    door.Locked = true;

                    Key key1 = new Key(keyVal);
                    key1.LootType = LootType.Blessed;
                    key1.Description = $"Key of {(this.HouseName != null ? this.HouseName : "")} [{door.Serial}]";
                    newOwner.AddToBackpack(key1);

                    Key key2 = new Key(keyVal);
                    key2.LootType = LootType.Blessed;
                    key2.Description = $"Key of {(this.HouseName != null ? this.HouseName : "")} [{door.Serial}]";
                    if (newOwner.BankBox != null)
                        newOwner.BankBox.DropItem(key2);
                }
            }
        }

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

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (!string.IsNullOrEmpty(HouseName))
                list.Add(HouseName);
            else
                base.AddNameProperty(list);
        }

        public override bool OnDragLift(Mobile from)
        {
            // Solo GM e superiori possono spostare il cartello
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;
            from.SendMessage("Only a GM can move this sign.");
            return false;
        }

        // --- VECCHIO SISTEMA: SOLO 1 RETTANGOLO ---
        // public void UnlockAllItemsInHouse()
        // {
        //     if (this.Map == null || this.Map == Map.Internal)
        //         return;
        //
        //     IPooledEnumerable e = this.Map.GetItemsInBounds(this.HouseArea);
        //     foreach (Item item in e)
        //     {
        //         if (item != null && !item.Deleted && !item.Movable && !(item is StaticHouseSign))
        //         {
        //             item.Movable = true;
        //         }
        //     }
        //     e.Free();
        // }

        // --- NUOVO SISTEMA: SU TUTTI I RETTANGOLI ---
        public void UnlockAllItemsInHouse()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return;

            foreach (var rect in m_HouseAreas)
            {
                IPooledEnumerable e = this.Map.GetItemsInBounds(rect);
                foreach (Item item in e)
                {
                    if (item != null && !item.Deleted && !item.Movable && !(item is StaticHouseSign))
                    {
                        item.Movable = true;
                    }
                }
                e.Free();
            }
        }
    }
}