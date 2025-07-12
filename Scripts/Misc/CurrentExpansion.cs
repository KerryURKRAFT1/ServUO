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
		public static readonly Expansion Expansion = Config.GetEnum<Expansion>("Expansion.CurrentExpansion", Expansion.UOR);		

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.Expansion = Expansion;
	
			AccountGold.Enabled = false;
			
			AccountGold.ConvertOnBank = false;
			
			AccountGold.ConvertOnTrade = false;
			
			VirtualCheck.UseEditGump = true; // Facoltativo, se vuoi evitare UI moderne

			ObjectPropertyList.Enabled = false; // Disabilita il menu a comparsa sui vendor

			Mobile.InsuranceEnabled = false; // Disabilita l'assicurazione degli oggetti

			//Mobile.VisibleDamageType = VisibleDamageType.None; // Nessun danno visibile sopra la testa
			Mobile.VisibleDamageType = VisibleDamageType.Related;

			Mobile.GuildClickMessage = true; // Già corretto per Renaissance
			
			Mobile.AsciiClickMessage = true; // Già corretto per Renaissance

			SupportedFeatures.Value = FeatureFlags.ExpansionSA;		
			
			Mobile.ActionDelay = 50;

			if (Core.AOS)
			{
				AOS.DisableStatInfluences();
	
				if (ObjectPropertyList.Enabled)
				{
					PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
				}
	
				Mobile.ActionDelay = 1000;
				
				Mobile.AOSStatusHandler = AOS.GetStatus;			
			}
		}
	}
}