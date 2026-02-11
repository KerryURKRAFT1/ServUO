using System;
using System.Collections.Generic;
using System.IO;
using Server.Mobiles;

namespace Server.Commands
{
    public class OwnerToggle
    {

        private static readonly Dictionary<Mobile, AccessLevel> m_StoredAccessLevels = new Dictionary<Mobile, AccessLevel>();
        

        private static readonly string m_DataFile = Path.Combine(Core.BaseDirectory, "Saves", "OwnerToggle.bin");

        public static void Initialize()
        {

            CommandSystem.Register("gm", AccessLevel.Player, new CommandEventHandler(GM_OnCommand));
            

            LoadData();
            

            EventSink.WorldSave += OnWorldSave;
        }

        [Usage("gm")]
        [Description("Toggles between Player and Owner access levels. Only Owners can use this command.")]
        public static void GM_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;



            bool isStoredOwner = m_StoredAccessLevels.ContainsKey(from);
            bool isCurrentOwner = from.AccessLevel >= AccessLevel.Owner;
            

            if (!isStoredOwner && !isCurrentOwner)
            {
                from.SendMessage("You do not have permission to use this command.");
                return;
            }


            if (m_StoredAccessLevels.ContainsKey(from))
            {

                AccessLevel originalLevel = m_StoredAccessLevels[from];
                m_StoredAccessLevels.Remove(from);
                
                from.AccessLevel = originalLevel;
                from.Blessed = true; 
                
                from.SendMessage("You are now in Owner mode.");
            }
            else
            {

                m_StoredAccessLevels[from] = from.AccessLevel;
                
                from.AccessLevel = AccessLevel.Player;
                from.Blessed = false; 
                
                from.SendMessage("You are now in Player mode.");
            }
            

            SaveData();
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            SaveData();
        }

        private static void SaveData()
        {
            try
            {

                string savesDir = Path.Combine(Core.BaseDirectory, "Saves");
                if (!Directory.Exists(savesDir))
                    Directory.CreateDirectory(savesDir);

                using (FileStream fs = new FileStream(m_DataFile, FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
  
                    writer.Write(1);
                    

                    writer.Write(m_StoredAccessLevels.Count);
                    

                    foreach (KeyValuePair<Mobile, AccessLevel> kvp in m_StoredAccessLevels)
                    {
                        if (kvp.Key != null && !kvp.Key.Deleted)
                        {
                            writer.Write(kvp.Key.Serial.Value);
                            writer.Write((int)kvp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving OwnerToggle data: {0}", ex.Message);
            }
        }

        private static void LoadData()
        {
            if (!File.Exists(m_DataFile))
                return;

            try
            {
                using (FileStream fs = new FileStream(m_DataFile, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int version = reader.ReadInt32();
                    int count = reader.ReadInt32();
                    
                    for (int i = 0; i < count; i++)
                    {
                        int serial = reader.ReadInt32();
                        int accessLevel = reader.ReadInt32();
                        

                        Mobile m = World.FindMobile(serial);
                        if (m != null && !m.Deleted)
                        {
                            m_StoredAccessLevels[m] = (AccessLevel)accessLevel;
                            


                            if (m.AccessLevel == AccessLevel.Player && (AccessLevel)accessLevel >= AccessLevel.Owner)
                            {


                            }
                        }
                    }
                }
                
                Console.WriteLine("OwnerToggle: Loaded {0} stored owner(s).", m_StoredAccessLevels.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading OwnerToggle data: {0}", ex.Message);
            }
        }




        public static bool IsInPlayerMode(Mobile m)
        {
            return m_StoredAccessLevels.ContainsKey(m);
        }




        public static AccessLevel GetTrueAccessLevel(Mobile m)
        {
            if (m_StoredAccessLevels.ContainsKey(m))
                return m_StoredAccessLevels[m];
            return m.AccessLevel;
        }
    }
}
