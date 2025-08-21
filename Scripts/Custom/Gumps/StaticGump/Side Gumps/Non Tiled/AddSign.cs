using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Gumps
{
	public class AddSignGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register("AddSign", AccessLevel.GameMaster, new CommandEventHandler(AddSign_OnCommand));
		}

		[Usage("AddStair")]
		
		public static void AddSign_OnCommand(CommandEventArgs e)
		{
			e.Mobile.CloseGump(typeof(AddSignGump));
			e.Mobile.SendGump(new AddSignGump());
		}

		public static ItemInfo[] m_Types = new ItemInfo[]
		{
            #region Signs
            new ItemInfo(2965), new ItemInfo(2966), 
            new ItemInfo(2979), new ItemInfo(2980),
            new ItemInfo(2981), new ItemInfo(2982), new ItemInfo(2983), new ItemInfo(2984), new ItemInfo(2985), new ItemInfo(2986), new ItemInfo(2987), new ItemInfo(2988),
            new ItemInfo(2989), new ItemInfo(2990), new ItemInfo(2991), new ItemInfo(2992), new ItemInfo(2993), new ItemInfo(2994), new ItemInfo(2995), new ItemInfo(2996),
            new ItemInfo(2997), new ItemInfo(2998), new ItemInfo(2999), new ItemInfo(3000), new ItemInfo(3001), new ItemInfo(3002), new ItemInfo(3003), new ItemInfo(3004),
            new ItemInfo(3005), new ItemInfo(3006), new ItemInfo(3007), new ItemInfo(3008), new ItemInfo(3009), new ItemInfo(3010), new ItemInfo(3011), new ItemInfo(3012),
            new ItemInfo(3013), new ItemInfo(3014), new ItemInfo(3015), new ItemInfo(3016), new ItemInfo(3017), new ItemInfo(3018), new ItemInfo(3019), new ItemInfo(3020),
            new ItemInfo(3021), new ItemInfo(3022), new ItemInfo(3023), new ItemInfo(3024), new ItemInfo(3025), new ItemInfo(3026), new ItemInfo(3027), new ItemInfo(3028),
            new ItemInfo(3029), new ItemInfo(3030), new ItemInfo(3031), new ItemInfo(3032), new ItemInfo(3033), new ItemInfo(3034), new ItemInfo(3035), new ItemInfo(3036),
            new ItemInfo(3037), new ItemInfo(3038), new ItemInfo(3039), new ItemInfo(3040), new ItemInfo(3041), new ItemInfo(3042), new ItemInfo(3043), new ItemInfo(3044),
            new ItemInfo(3045), new ItemInfo(3046), new ItemInfo(3047), new ItemInfo(3048), new ItemInfo(3049), new ItemInfo(3050), new ItemInfo(3051), new ItemInfo(3052),
            new ItemInfo(3053), new ItemInfo(3054), new ItemInfo(3055), new ItemInfo(3056), new ItemInfo(3057), new ItemInfo(3058), new ItemInfo(3059), new ItemInfo(3060),
            new ItemInfo(3061), new ItemInfo(3062), new ItemInfo(3063), new ItemInfo(3064), new ItemInfo(3065), new ItemInfo(3066), new ItemInfo(3067), new ItemInfo(3068),
            new ItemInfo(3069), new ItemInfo(3070), new ItemInfo(3071), new ItemInfo(3072), new ItemInfo(3073), new ItemInfo(3074), new ItemInfo(3075), new ItemInfo(3076),
            new ItemInfo(3077), new ItemInfo(3078), new ItemInfo(3079), new ItemInfo(3080), new ItemInfo(3081), new ItemInfo(3082), new ItemInfo(3083), new ItemInfo(3084),
            new ItemInfo(3085), new ItemInfo(3086),
            new ItemInfo(3139), new ItemInfo(3140),
            new ItemInfo(4759), new ItemInfo(4760), new ItemInfo(4761), new ItemInfo(4762), new ItemInfo(4763), new ItemInfo(4764), new ItemInfo(4765), new ItemInfo(4766),
            new ItemInfo(5693), new ItemInfo(5694), new ItemInfo(5695), new ItemInfo(5696), new ItemInfo(5697), new ItemInfo(5698), new ItemInfo(5699)
            #endregion
        };

		private int m_Page;

		////private readonly int m_Type;

		public AddSignGump()
			: this(0)
		{ }

		public AddSignGump(int page)
			: base(0, 0)
		{
			int pageCount = 1 + (m_Types.Length / 12);

			if (page >= pageCount)
				page = pageCount - 1;
			else if (page < 0)
				page = 0;

			m_Page = page;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddBackground(0, 0, 720, 180, 3500);

			AddHtmlLocalized(15, 15, 60, 20, 1042971, page.ToString(), 0x1, false, false); // #

			AddHtmlLocalized(20, 38, 60, 20, 1043353, 0x1, false, false); // Next

			if (page + 1 < pageCount)
				AddButton(15, 55, 0xFA5, 0xFA7, 10000 + (page + 1), GumpButtonType.Reply, 0);
			else
				AddButton(15, 55, 0xFA5, 0xFA7, 10000, GumpButtonType.Reply, 0);

			AddHtmlLocalized(20, 93, 60, 20, 1011393, 0x1, false, false); // Back

			if (page > 0)
				AddButton(15, 110, 0xFAE, 0xFB0, 10000 + (page - 1), GumpButtonType.Reply, 0);
			else
				AddButton(15, 110, 0xFAE, 0xFB0, 10000, GumpButtonType.Reply, 0);

			for (int i = 0; i < 12; ++i)
			{
				int index = (page * 12) + i;
				if(index >= m_Types.Length) break;

				int button = 1000000 + index;
				int offset = (i + 1) * 50;

				if (m_Types[index].m_BaseID > 0)
				{
					AddButton(45 + offset, 20, 0x2624, 0x2625, button, GumpButtonType.Reply, m_Types[index].m_BaseID);
					AddItem(50 + offset, 40, m_Types[index].m_BaseID);
				}
				else
				{
					AddImage(25 + offset, 20, 0x2625, 900);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			int button = info.ButtonID;

			if (button <= 0)
				return;

			int page = m_Page;

			if (button >= 1000000)
			{
				button -= 1000000;

				CommandSystem.Handle(from, string.Format("{0}M Add Static {1}", CommandSystem.Prefix, m_Types[button].m_BaseID));
			}
			else if (button >= 10000)
			{
				button -= 10000;

				page = button;
			}

			from.SendGump(new AddSignGump(page));
		}

		public class ItemInfo
		{
			public int m_BaseID;

			public ItemInfo(int baseID)
			{
				m_BaseID = baseID;
			}
		}
	}
}
