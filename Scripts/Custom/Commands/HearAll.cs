//Author Zippy
//Swapped Arrays for Lists - Carlin

using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Commands
{
	public class HearAll
	{
		private static bool m_ConsolePrint;
		
		private static List<Mobile> m_HearAll = new List<Mobile>();

		public static void Initialize ()
		{
			CommandSystem.Register ("HearAll", AccessLevel.GameMaster, new CommandEventHandler (HearAll_OnCommand));
			
			CommandSystem.Register ("ConsoleHearAll", AccessLevel.GameMaster, new CommandEventHandler (ConsoleHearAll_OnCommand));

			EventSink.Speech += new SpeechEventHandler (OnSpeech);
		}

		private static void OnSpeech (SpeechEventArgs args)
		{
			string msg;

			if (args.Mobile.Region.Name != "")
			{
				msg = String.Format ("{0} ({1}): {2}", args.Mobile.Name, args.Mobile.Region.Name, args.Speech);
			}
			else
			{
				msg = String.Format ("{0}: {1}", args.Mobile.Name, args.Speech);
			}

			if (m_ConsolePrint)
			{
				Console.WriteLine (msg);
			}

			for (int i=0; i < m_HearAll.Count; i++)
			{
				if ((m_HearAll[i]).NetState != null)
				{
				    if (args.Mobile != m_HearAll[i])
				    {
				    	(m_HearAll[i]).SendMessage(60, msg );
				    }
				}
				else
				{
					m_HearAll.Remove (m_HearAll[i]);
				}
			}
		}

		private static void HearAll_OnCommand (CommandEventArgs args)
		{
			if (m_HearAll.Contains (args.Mobile))
			{
				m_HearAll.Remove (args.Mobile);
				
				args.Mobile.SendMessage (48, "Hear all disabled.");
			}
			else
			{
				m_HearAll.Add( args.Mobile );
				
				args.Mobile.SendMessage (48, "Hear all enabled");
			}
		}

		private static void ConsoleHearAll_OnCommand (CommandEventArgs args)
		{
			m_ConsolePrint = !m_ConsolePrint;

			if (m_ConsolePrint)
			{
				args.Mobile.SendMessage (48, "Now sending all speech to the console.");
			}
			else
			{
				args.Mobile.SendMessage (48, "No longer sending speech to the console.");
			}
		}
	}
}

