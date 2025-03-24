using System;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Custom
{
    public class HueDisplayGrid
    {
        public static void Initialize()
        {
            CommandSystem.Register("PlaceHueGrid", AccessLevel.Administrator, new CommandEventHandler(PlaceHueGrid_OnCommand));
        }

        [Usage("PlaceHueGrid")]
        [Description("Places a 256x256 grid of tiki statues displaying all hues.")]
        public static void PlaceHueGrid_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int startX = from.X;
            int startY = from.Y;
            int startZ = from.Z;

            int hueIndex = 0;
            int spacing = 2; // 1 tile gap between statues
            int gridSize = 128; // 128x128 statues (each taking 2x2 space)

            from.SendMessage("Placing Hue Grid... This may take a moment.");

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (hueIndex >= 16384)
                        break;

                    int tileX = startX + (x * spacing);
                    int tileY = startY + (y * spacing);

                    // Place the floor tile
                    Item floor = new Static(1313);
                    floor.MoveToWorld(new Point3D(tileX, tileY, startZ), from.Map);

                    // Place the tiki statue
                    Item statue = new Static(5360);
                    statue.Hue = hueIndex;
                    statue.Name = "Hue " + hueIndex; // Sostituire la stringa interpolata
                    statue.MoveToWorld(new Point3D(tileX, tileY, startZ + 5), from.Map); // Slightly above floor

                    hueIndex++;
                }
            }

            from.SendMessage("Hue Grid Placement Complete!");
        }
    }
}