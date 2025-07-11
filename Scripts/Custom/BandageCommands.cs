//TO DEFINE RUNUO V2.1 comment the line below (like this -> //#define C2K)
//TO DEFINE CALLANDOR2K ML Version uncomment line below (like this -> #define C2K)
#define C2K

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Targeting;

namespace Server.Commands
{
    public class BandageOnCommand
    {
		public static void Initialize()
		{
    		CommandSystem.Register("bandageself", AccessLevel.Player, new CommandEventHandler(BandageSelf_OnCommand));
    		CommandSystem.Register("bs", AccessLevel.Player, new CommandEventHandler(BandageSelf_OnCommand));

    		CommandSystem.Register("bandagetarget", AccessLevel.Player, new CommandEventHandler(BandageTarget_OnCommand));
	    	CommandSystem.Register("bt", AccessLevel.Player, new CommandEventHandler(BandageTarget_OnCommand));
		}

		public static void BandageSelf_OnCommand(CommandEventArgs e)
		{
			Container pack = e.Mobile.Backpack;
			Bandage m_Bandage = pack.FindItemByType( typeof( Bandage ) ) as Bandage;

			if ( m_Bandage != null )
			{
				e.Mobile.RevealingAction();
#if(C2K)
				if ( BandageContext.BeginHeal( e.Mobile, e.Mobile, m_Bandage is EnhancedBandage ) != null )
#else
				if ( BandageContext.BeginHeal( e.Mobile, e.Mobile )!= null )
#endif
					m_Bandage.Consume();
			}
		}

		public static void BandageTarget_OnCommand(CommandEventArgs e)
		{
			Container pack = e.Mobile.Backpack;
			Bandage m_Bandage = pack.FindItemByType( typeof( Bandage ) ) as Bandage;

			if ( m_Bandage != null )
			{
				if ( e.Mobile.InRange( m_Bandage.GetWorldLocation(), Bandage.Range ) ) 
				{
					e.Mobile.RevealingAction();

					e.Mobile.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?

					e.Mobile.Target = new InternalTarget( m_Bandage );
				}
				else
				{
					e.Mobile.SendLocalizedMessage( 500295 ); // You are too far away to do that.
				}		
			}
		}

		private class InternalTarget : Target
		{
			private Bandage m_Bandage;

			public InternalTarget( Bandage bandage ) : base( Bandage.Range, false, TargetFlags.Beneficial )
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Bandage.Deleted )
					return;

				if ( targeted is Mobile )
				{
					if ( from.InRange( m_Bandage.GetWorldLocation(), Bandage.Range ) )
					{
#if(C2K)	
						if ( BandageContext.BeginHeal( from, (Mobile)targeted, m_Bandage is EnhancedBandage ) != null )
#else
						if ( BandageContext.BeginHeal( from, (Mobile)targeted ) != null )
#endif
							m_Bandage.Consume();
					}
					else
					{
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
					}
				}
				else if ( targeted is PlagueBeastInnard )
				{
					if ( ((PlagueBeastInnard) targeted).OnBandage( from ) )
						m_Bandage.Consume();
				}
				else
				{
					from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
				}
			}
		}
    }
}