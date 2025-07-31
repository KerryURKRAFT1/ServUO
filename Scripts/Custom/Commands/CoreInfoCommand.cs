using System;
using Server.Commands;
using Server.Items;

namespace Server.Scripts.Commands
{
    public class CoreInfoCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CoreInfo", AccessLevel.Player, new CommandEventHandler(CoreInfo_OnCommand));
        }

        [Usage("CoreInfo")]
        [Description("Displays the current Core version, Core flags, and which weapon parameters are currently in use.")]
        public static void CoreInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Show which core is active, with clear info
            if (Core.AOS)
                from.SendMessage("The server is running on AOS Core.");
            else if (Core.ML)
                from.SendMessage("The server is running on ML Core.");
            else if (Core.SE)
                from.SendMessage("The server is running on SE Core.");
            else if (Core.UOR)
                from.SendMessage("The server is running on UOR Core.");
            else
                from.SendMessage("The server is running on an unknown Core version.");

            // Show all flags (for debug)
            from.SendMessage($"Core.AOS: {Core.AOS}");
            from.SendMessage($"Core.ML: {Core.ML}");
            from.SendMessage($"Core.SE: {Core.SE}");
            from.SendMessage($"Core.UOR: {Core.UOR}");

            // Weapon parameter check
            BaseWeapon weapon = from.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon
                             ?? from.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weapon != null)
            {
                // Check which value is used for each parameter
                int minDamage, maxDamage, speed, strReq;
                string minDamageType, maxDamageType, speedType, strReqType;

                // MinDamage
                if (Core.ML)
                {
                    minDamage = weapon.AosMinDamage;
                    maxDamage = weapon.AosMaxDamage;
                    speed = (int)weapon.MlSpeed;
                    strReq = weapon.AosStrengthReq;
                    minDamageType = "AOS/ML";
                    maxDamageType = "AOS/ML";
                    speedType = "ML";
                    strReqType = "AOS";
                }
                else if (Core.AOS || Core.SE)
                {
                    minDamage = weapon.AosMinDamage;
                    maxDamage = weapon.AosMaxDamage;
                    speed = weapon.AosSpeed;
                    strReq = weapon.AosStrengthReq;
                    minDamageType = "AOS";
                    maxDamageType = "AOS";
                    speedType = "AOS";
                    strReqType = "AOS";
                }
                else // Presumibilmente Core.UOR o altro
                {
                    minDamage = weapon.OldMinDamage;
                    maxDamage = weapon.OldMaxDamage;
                    speed = weapon.OldSpeed;
                    strReq = weapon.OldStrengthReq;
                    minDamageType = "OLD";
                    maxDamageType = "OLD";
                    speedType = "OLD";
                    strReqType = "OLD";
                }

                from.SendMessage($"Weapon equipped: {weapon.GetType().Name}");
                from.SendMessage($"MinDamage: {minDamage} [{minDamageType}]");
                from.SendMessage($"MaxDamage: {maxDamage} [{maxDamageType}]");
                from.SendMessage($"Speed: {speed} [{speedType}]");
                from.SendMessage($"StrengthReq: {strReq} [{strReqType}]");
            }
            else
            {
                from.SendMessage("You do not have a weapon equipped for parameter debug.");
            }
        }
    }
}