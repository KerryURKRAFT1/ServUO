#region Header
// **********
// ServUO - CurrentExpansion.cs
// **********
#endregion

#region References
using System;

using Server.Accounting;
using Server.Network;
#endregion

namespace Server
{
	public class CurrentExpansion
	{
		//public static readonly Expansion Expansion = Config.GetEnum<Expansion>("Expansion.CurrentExpansion", Expansion.TOL);
		public static readonly Expansion Expansion = Config.GetEnum<Expansion>("Expansion.CurrentExpansion", Expansion.UOR);
		

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.Expansion = Expansion;

			/* MODIFICHE PER RENAISSANCE STYLE
			AccountGold.Enabled = Core.TOL;
			AccountGold.ConvertOnBank = true;
			AccountGold.ConvertOnTrade = false;
			VirtualCheck.UseEditGump = true;
			ObjectPropertyList.Enabled = Core.AOS;
			/bjectPropertyList.Enabled = false;
			Mobile.InsuranceEnabled = Core.AOS;
			Mobile.VisibleDamageType = Core.AOS ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Core.AOS;
			Mobile.AsciiClickMessage = !Core.AOS;
			*/
			
			AccountGold.Enabled = false;
			AccountGold.ConvertOnBank = false;
			AccountGold.ConvertOnTrade = false;
			VirtualCheck.UseEditGump = true; // Facoltativo, se vuoi evitare UI moderne

			ObjectPropertyList.Enabled = false; // Disabilita il menu a comparsa sui vendor
 			PacketHandlers.SingleClickProps = false; // Disabilita il menu automatico sui vendor

			Mobile.InsuranceEnabled = false; // Disabilita l'assicurazione degli oggetti
			Mobile.VisibleDamageType = VisibleDamageType.None; // Nessun danno visibile sopra la testa

			Mobile.GuildClickMessage = !Core.AOS; // Già corretto per Renaissance
			Mobile.AsciiClickMessage = !Core.AOS; // Già corretto per Renaissance





			if (!Core.AOS)
			{
				return;
			}

			AOS.DisableStatInfluences();

			/* rimozione menu vendor
			if (ObjectPropertyList.Enabled)
			{
				PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
			}
			*/
			Mobile.ActionDelay = 1000;
			Mobile.AOSStatusHandler = AOS.GetStatus;
			
		}
	}
}