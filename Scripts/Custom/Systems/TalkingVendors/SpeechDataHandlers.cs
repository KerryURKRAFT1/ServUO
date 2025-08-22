using System;
using Server;
using System.Data;
using System.Threading;
using System.Collections;
using Server.Mobiles;
using Server.Commands;

public delegate SpeechResponse GetResponseDelegate(string Said, Mobile Speaker, BaseVendor tp);

namespace Server.Mobiles.Data
{
	public class SpeechData
	{
		public static System.Data.DataSet dsSpeechRules = new DataSet();
		
		public static bool blockRequests;

		public static void Initialize()
		{
			Timer.DelayCall (TimeSpan.FromSeconds(0.0), () => Init());
		}

		public static void Init()
		{
			Console.Write("SpeechRules Loading...");

			CommandSystem.Register("dbReload", AccessLevel.GameMaster, new CommandEventHandler(dbReload_OnCommand));

			SpeechData.LoadData();

			Console.WriteLine("done ({0})", SpeechData.dsSpeechRules.DataSetName);
		}
		
		public static void LoadData()
		{
			SpeechData.blockRequests = true;

			SpeechData.dsSpeechRules.Reset();
			
			SpeechData.dsSpeechRules.ReadXml("Data\\SpeechRules.xml", XmlReadMode.ReadSchema);

			SpeechData.blockRequests = false;
		}

		[Usage("dbReload")]
		[Description("Reloads the BaseVendor Speech Data from XML.")]
		public static void dbReload_OnCommand(CommandEventArgs e)
		{
			SpeechData.blockRequests = true;

			ReloadTimer t = new ReloadTimer(e.Mobile);
			
			t.Start();
		}

		private class ReloadTimer : Timer
		{
			Mobile m_from;

			public ReloadTimer(Mobile From) : base(TimeSpan.FromSeconds(2))
			{
				m_from = From;
				
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				SpeechData.LoadData();
				
				m_from.SendMessage("BaseVendor Database Reloaded");

				if (BaseVendor.Logging == LogLevel.Basic || BaseVendor.Logging == LogLevel.Debug)
				{
					BaseVendorLogging.WriteLine(m_from, "Reloaded the BaseVendor Speech Data from XML");
				}
			}
		}
	}

	public class Response
	{
		private static TimeSpan duration = TimeSpan.FromMilliseconds(1100);

		private static string[] blockedReplies = { "*cough*", "hmm", "*yawn*", "huh?" };

		private DateTime started;

		private SpeechResponse sr;

		public SpeechResponse GetResponse(string Said, Mobile Speaker, BaseVendor tp)
		{
			started = DateTime.Now;
			DataView dvReplies;
			DataRowView[] drvReplies;
			object key;
			ArrayList alReplies = new ArrayList();
			DataRow drFound = null;
			String test = null;
			bool isWord = false;

			if (BaseVendor.Logging == LogLevel.Debug)
			{
				BaseVendorLogging.WriteLine(Speaker, "Asynchronous call begun.");
			}

			if (SpeechData.blockRequests == true)
			{
				sr = new SpeechResponse(blockedReplies[Utility.Random(blockedReplies.Length)], Speaker, 0, 0, null, null);

				if (BaseVendor.Logging == LogLevel.Debug)
				{
					BaseVendorLogging.WriteLine(Speaker, "Access to Database blocked.");
				}
			}
			else
			{
				String temp = Said.ToLower().Trim();
				
				if (isEmpty(temp))
				{
					return new SpeechResponse(blockedReplies[Utility.Random(blockedReplies.Length)], Speaker, 0, 0, null, null);
				}

				temp = ' ' + temp + ' ';
				
				foreach (DataRow dr in SpeechData.dsSpeechRules.Tables["dtTriggers"].Rows)
				{
					test = dr["trigger"].ToString();

					isWord = (bool)dr["word"];
					
					if (isWord)
					{
						test = ' ' + test + ' ';
					}

					if (temp.IndexOf(test) >= 0)
					{
						drFound = dr;
						
						if (BaseVendor.Logging == LogLevel.Debug)
						{
							BaseVendorLogging.WriteLine(Speaker, "Trigger matched: \"{0}\" : \"{1}\"", test, temp);
						}

						break;
					}
				}

				dvReplies = new DataView(SpeechData.dsSpeechRules.Tables["dtResponses"]);
				
				dvReplies.RowStateFilter = DataViewRowState.CurrentRows;
				
				dvReplies.Sort = "index";

				if (drFound == null)
				{
					key = (object)0;

					if (BaseVendor.Logging == LogLevel.Basic || BaseVendor.Logging == LogLevel.Debug)
					{
						BaseVendorLogging.WriteLine(Speaker, "Default Rule: \"{0}\"", temp.Trim().ToUpper());
					}
				}
				else
				{
					key = (object)drFound[0].ToString();
				}

				drvReplies = dvReplies.FindRows(key);

				foreach (DataRowView drv in drvReplies)
				{
					if ((int)drv["npcAttitude"] != 0 && (int)drv["npcAttitude"] != (int)tp.attitude)
						continue;
					if ((int)drv["playerGender"] != 0 && (int)drv["playerGender"] != (Speaker.Female ? 2 : 1))
						continue;
					if ((int)drv["npcGender"] != 0 && (int)drv["npcGender"] != (tp.Female ? 2 : 1))
						continue;
					if ((int)drv["timeOfDay"] != 0 && !(BaseVendor.CheckTOD(tp, (int)drv["timeOfDay"])))
						continue;
					if (!isEmpty(drv["npcRegion"].ToString()) && drv["npcRegion"].ToString() != tp.Region.ToString())
						continue;
					if (!isEmpty(drv["npcTag"].ToString()) && drv["npcTag"].ToString() != tp.Tag)
						continue;
					if (!isEmpty(drv["npcName"].ToString()) && drv["npcName"].ToString() != tp.Name)
						continue;
					if (!isEmpty(drv["npcTitle"].ToString()) && drv["npcTitle"].ToString() != tp.Title)
						continue;
					
					if ((int)drv["objStatus"] != 0)
					{
						Item item = BaseVendor.CheckInventory(Speaker, drv["questObject"].ToString());
						
						if (item == null && (int)drv["objStatus"] == 1)
						{
							continue;
						}
						
						if (item != null && (int)drv["objStatus"] == 2)
						{
							continue;
						}
					}

					alReplies.Add(drv);
				}

				int cnt = alReplies.Count;
				
				DataRowView reply = (DataRowView)alReplies[Utility.Random(cnt)];

				if (BaseVendor.Logging == LogLevel.Debug)
				{
					BaseVendorLogging.WriteLine(Speaker, "Matched {0} Responses.", cnt);
				}

				string toSay = reply["response"].ToString();
				
				if (toSay == "{blank}")
				{
					toSay = "";
				}
				
				int anim = (int)reply["npcAnimation"];
				int react = (int)reply["npcReaction"];
				string reward = reply["packObject"].ToString(); // is it better to pass empty string or null?
				string remove = null;
				
				if (!isEmpty(reply["questObject"].ToString()) && (bool)reply["questObjDelete"])
				{
					remove = reply["questObject"].ToString();
				}

				sr = new SpeechResponse(toSay, Speaker, anim, react, reward, remove);
			}

			TimeSpan timeused = DateTime.Now - started;
			TimeSpan timeleft = duration - timeused;

			if (BaseVendor.Logging == LogLevel.Debug)
			{
				BaseVendorLogging.WriteLine(Speaker, "Asynchronous call took {0} ms.", timeused.Milliseconds.ToString());
			}

			if (timeleft > TimeSpan.Zero && !BaseVendor.Synchronous)
			{
				Thread.Sleep(timeleft);
			}

			return sr;
		}

		private static bool isEmpty(string str)
		{
			return (str == null || str == "");
		}
	}
}
