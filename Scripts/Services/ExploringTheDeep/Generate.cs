using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;

namespace Server.Engines.ExploringTheDeep
{
    public static class GenerateExploringTheDeep
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenExploringTheDeep", AccessLevel.Administrator, Generate);
            CommandSystem.Register("GenStorageLocker", AccessLevel.Developer, new CommandEventHandler(GenStorageLocker_Command));
        }

        [Usage("GenStorageLocker")]
        private static void GenStorageLocker_Command(CommandEventArgs e)
        {
            foreach (Item item in World.Items.Values)
            {
                if (item is StorageLocker && !item.Deleted)
                {
                    item.Delete();
                }
            }

            StorageLocker storagelocker = new StorageLocker(Parts.Flywheel);
            storagelocker.MoveToWorld(new Point3D(6421, 1753, 0), Map.Trammel);
            storagelocker.Active = true;

            storagelocker = new StorageLocker(Parts.BearingAssembly);
            storagelocker.MoveToWorld(new Point3D(6441, 1753, 0), Map.Trammel);
            storagelocker.Active = true;

            storagelocker = new StorageLocker(Parts.PowerCore);
            storagelocker.MoveToWorld(new Point3D(6441, 1733, 0), Map.Trammel);
            storagelocker.Active = true;

            storagelocker = new StorageLocker(Parts.WireSpool);
            storagelocker.MoveToWorld(new Point3D(6421, 1733, 0), Map.Trammel);
            storagelocker.Active = true;

            e.Mobile.SendMessage("Storage Locker Generation completed!");
        }

        public static void Generate(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            Generate(m);
        }

        public static void Generate(Mobile m)
        {
            #region Gravewater Lake Finish

            CommandSystem.Handle(m, Server.Commands.CommandSystem.Prefix + "XmlLoad Spawns/GravewaterLake.xml");

            // Winch 
            WinchAssembly winch = new WinchAssembly();
            Hatch hatch = new Hatch();
            WinchAssemblyLever lever = new WinchAssemblyLever(winch, hatch);

            lever.MoveToWorld(new Point3D(6310, 1705, 0), Map.Trammel);
            winch.MoveToWorld(new Point3D(6310, 1704, 0), Map.Trammel);
            hatch.MoveToWorld(new Point3D(6303, 1711, 10), Map.Trammel);

            // StorageLocker
            CommandSystem.Handle(m, Server.Commands.CommandSystem.Prefix + "GenStorageLocker");

            Item door = new LightWoodDoor(DoorFacing.SouthCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6427, 1735, 0), Map.Trammel);

            door = new LightWoodDoor(DoorFacing.SouthCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6427, 1752, 0), Map.Trammel);

            door = new LightWoodDoor(DoorFacing.SouthCCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6435, 1735, 0), Map.Trammel);

            door = new LightWoodDoor(DoorFacing.SouthCCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6435, 1752, 0), Map.Trammel);

            door = new LightWoodDoor(DoorFacing.WestCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6431, 1727, 0), Map.Trammel);

            door = new LightWoodDoor(DoorFacing.EastCCW);
            door.Hue = 2952;
            door.MoveToWorld(new Point3D(6432, 1727, 0), Map.Trammel);

            Static decor = new Static(0x1EAF);
            decor.MoveToWorld(new Point3D(6310, 1704, 11), Map.Trammel);

            decor = new Static(0x1ED5);
            decor.MoveToWorld(new Point3D(6310, 1705, -5), Map.Trammel);

            decor = new Static(0x10A4);
            decor.MoveToWorld(new Point3D(6310, 1703, 8), Map.Trammel);

            decor = new Static(0x2E3D);
            decor.MoveToWorld(new Point3D(6311, 1703, 19), Map.Trammel);

            decor = new Static(0x3A8);
            decor.MoveToWorld(new Point3D(6309, 1704, 20), Map.Trammel);

            decor = new Static(0x3A8);
            decor.MoveToWorld(new Point3D(6310, 1704, 20), Map.Trammel);

            decor = new Static(0x3A6);
            decor.MoveToWorld(new Point3D(6309, 1703, 24), Map.Trammel);

            decor = new Static(0x3A6);
            decor.MoveToWorld(new Point3D(6310, 1703, 24), Map.Trammel);

            Item ladder = new ShipLadder(new Point3D(6302, 1672, 0), Map.Trammel, 0x08A6);
            ladder.MoveToWorld(new Point3D(6431, 1699, 0), Map.Trammel);

            ladder = new ShipLadder(new Point3D(6432, 1699, 0), Map.Trammel, 0x08A6);
            ladder.MoveToWorld(new Point3D(6304, 1672, -5), Map.Trammel);

            ladder = new ShipLadder(new Point3D(6432, 1699, 0), Map.Trammel, 0x08A1);
            ladder.MoveToWorld(new Point3D(6292, 1721, 0), Map.Trammel);

            ladder = new ShipLadder(new Point3D(1699, 1646, -115), Map.Malas, 0x14FA);
            ladder.MoveToWorld(new Point3D(6278, 1773, 0), Map.Trammel);

            Item sign = new ShipSign(0xBD2, 1154461); // Use Ladder to Return to Foredeck
            sign.MoveToWorld(new Point3D(6400, 1658, 0), Map.Trammel);

            sign = new ShipSign(0xBCF, 1154492); // Use the rope to return to the surface
            sign.MoveToWorld(new Point3D(6278, 1773, 0), Map.Trammel);

            sign = new ShipSign(0xBD1, 1154463); // Warning! Only those with proper gear may enter the lake for salvage operations! Enter at your own risk! No Pets!
            sign.MoveToWorld(new Point3D(1698, 1566, -110), Map.Malas);

            // CaptainsLogScroll
            Item scroll = new CaptainsLogScroll();
            scroll.MoveToWorld(new Point3D(6430, 1743, 0), Map.Trammel);

            Item tele = new Teleporter(new Point3D(6445, 1743, 0), Map.Trammel);
            tele.MoveToWorld(new Point3D(6321, 1710, -35), Map.Trammel);

            tele = new Teleporter(new Point3D(6445, 1743, 0), Map.Trammel);
            tele.MoveToWorld(new Point3D(6321, 1711, -35), Map.Trammel);
            
            tele = new Teleporter(new Point3D(6322, 1710, -35), Map.Trammel);
            tele.MoveToWorld(new Point3D(6447, 1741, 1), Map.Trammel);

            tele = new Teleporter(new Point3D(6322, 1710, -35), Map.Trammel);
            tele.MoveToWorld(new Point3D(6447, 1742, 1), Map.Trammel);

            tele = new Teleporter(new Point3D(6322, 1710, -35), Map.Trammel);
            tele.MoveToWorld(new Point3D(6447, 1743, 1), Map.Trammel);

            tele = new Teleporter(new Point3D(6322, 1710, -35), Map.Trammel);
            tele.MoveToWorld(new Point3D(6447, 1744, 1), Map.Trammel);

            tele = new Teleporter(new Point3D(6322, 1710, -35), Map.Trammel);
            tele.MoveToWorld(new Point3D(6447, 1745, 1), Map.Trammel);

            tele = new Whirlpool(new Point3D(6274, 1787, 0), Map.Trammel);
            tele.MoveToWorld(new Point3D(1700, 1638, -115), Map.Malas);

            Item item = new AnkhWest();
            item.MoveToWorld(new Point3D(1694, 1562, -109), Map.Malas);

            item = new DungeonHitchingPost();
            item.MoveToWorld(new Point3D(1702, 1552, -109), Map.Malas);

            #endregion

            #region Quester Spawns

            XmlSpawner sp;

            sp = new XmlSpawner("GipsyGemologist");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1509, 618, -16), Map.Ilshenar);
            sp.Respawn();

            sp = new XmlSpawner("ChampHuthwait");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2995, 635, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("JosefSkimmons");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2630, 2092, 10), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("MadelineHarte");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1364, 3780, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("CousteauPerron");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5212, 2314, 28), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("HeplerPaulson");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2039, 2842, 0), Map.Trammel);
            sp.Respawn();

            #endregion
             
            #region Custeau Perron House
            door = new CusteauPerronHouseDoor();
            door.MoveToWorld(new Point3D(1651, 1551, 25), Map.Trammel);

            scroll = new MasterThinkerSchematics();
            scroll.MoveToWorld(new Point3D(1649, 1547, 54), Map.Trammel);

            decor = new Static(0xB7F);
            decor.MoveToWorld(new Point3D(1651, 1549, 45), Map.Trammel);

            decor = new Static(0xB80);
            decor.MoveToWorld(new Point3D(1652, 1549, 45), Map.Trammel);

            decor = new Static(0xB7E);
            decor.MoveToWorld(new Point3D(1653, 1549, 45), Map.Trammel);

            item = new MasterThinkerContoller();
            item.MoveToWorld(new Point3D(1652, 1547, 45), Map.Trammel);
            #endregion

            #region Ice Dungeon
            Item addon = new CousteauPerronAddon();
            addon.MoveToWorld(new Point3D(5211, 2312, 28), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5799, 234, -5), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5799, 235, -4), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5800, 236, -8), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5802, 234, -4), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5801, 239, -7), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5801, 240, -4), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5803, 243, -2), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5806, 244, -6), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5807, 240, -2), Map.Trammel);

            item = new IceCrystals();
            item.MoveToWorld(new Point3D(5808, 237, -3), Map.Trammel);

            #endregion

            #region Sorcerers Dungeon

            sp = new XmlSpawner(2, 5, 5, 0, 5, 1, "RockMite");
            sp.MoveToWorld(new Point3D(122, 10, -28), Map.Ilshenar);
            sp.Respawn();

            CommandSystem.Handle(m, Server.Commands.CommandSystem.Prefix + "GenSorcerersPlate");

            #endregion

            #region Scroll
            scroll = new WillemHarteScroll();
            scroll.MoveToWorld(new Point3D(1359, 3779, 7), Map.Trammel);

            scroll = new MadelineHarteScroll();
            scroll.MoveToWorld(new Point3D(1359, 3780, 7), Map.Trammel);

            scroll = new LiamDeFoeScroll();
            scroll.MoveToWorld(new Point3D(1364, 3778, 1), Map.Trammel);

            scroll = new CalculationsScroll();
            scroll.MoveToWorld(new Point3D(2997, 632, 9), Map.Trammel);

            scroll = new SuspicionsScroll();
            scroll.MoveToWorld(new Point3D(2993, 632, 14), Map.Trammel);

            scroll = new SealedLettersScroll();
            scroll.MoveToWorld(new Point3D(2992, 636, 5), Map.Trammel);

            scroll = new LedgerScroll();
            scroll.MoveToWorld(new Point3D(2580, 1118, 5), Map.Trammel);

            scroll = new JournalScroll();
            scroll.MoveToWorld(new Point3D(2578, 1120, 0), Map.Trammel);

            scroll = new SorcerersScroll();
            scroll.MoveToWorld(new Point3D(101, 42, -22), Map.Ilshenar);
            #endregion
        }
    }
}