using Server;
using System;
using System.Collections.Generic;

namespace Server.Items
{
	public class CommonStealables
	{
		public static List<string> NoItems = new List<string>();
		
		public static List<string> Dungeon = new List<string>
		{
			"ReactiveArmorScroll",	"ClumsyScroll",			"CreateFoodScroll",		"FeeblemindScroll",
			"HealScroll",			"MagicArrowScroll",		"NightSightScroll",		"WeakenScroll",			
			"AgilityScroll",		"CunningScroll",		"CureScroll",			"HarmScroll",
			"MagicTrapScroll",		"MagicUnTrapScroll",	"ProtectionScroll",		"StrengthScroll",			
			"BlessScroll",			"FireballScroll",		"MagicLockScroll",		"PoisonScroll",
			"TelekinisisScroll",	"TeleportScroll",		"UnlockScroll",			"WallOfStoneScroll",			
			"ArchCureScroll",		"ArchProtectionScroll",	"CurseScroll",			"FireFieldScroll",
			"GreaterHealScroll",	"LightningScroll",		"ManaDrainScroll",		"RecallScroll",			
					
			"HarmWand",				"HealWand",				"ClumsyWand",			"FeebleWand",
			"ManaDrainWand",		"WeaknessWand",			"IDWand",

			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",

			"BoneArms",				"BoneChest",			"BoneGloves",			"BoneLegs",
			"BoneHelm",				"ChainChest",			"ChainLegs",			"ChainCoif",
			"Bascinet",				"CloseHelm",			"Helmet",				"NorseHelm",
			"OrcHelm",				"FemaleLeatherChest",	"LeatherArms",			"LeatherBustierArms",
			"LeatherChest",			"LeatherGloves",		"LeatherGorget",		"LeatherLegs",
			"LeatherShorts",		"LeatherSkirt",			"LeatherCap",			"FemalePlateChest",
			"PlateArms",			"PlateChest",			"PlateGloves",			"PlateGorget",
			"PlateHelm",			"PlateLegs",			"RingmailArms",			"RingmailChest",
			"RingmailGloves",		"RingmailLegs",			"FemaleStuddedChest",	"StuddedArms",
			"StuddedBustierArms",	"StuddedChest",			"StuddedGloves",		"StuddedGorget",
			"StuddedLegs"		
		};
		
		public static List<string> Outdoors = new List<string>
		{
			"ReactiveArmorScroll",	"ClumsyScroll",			"CreateFoodScroll",		"FeeblemindScroll",
			"HealScroll",			"MagicArrowScroll",		"NightSightScroll",		"WeakenScroll",
			"AgilityScroll",		"CunningScroll",		"CureScroll",			"HarmScroll",
			"MagicTrapScroll",		"MagicUnTrapScroll",	"ProtectionScroll",		"StrengthScroll",
			"BlessScroll",			"FireballScroll",		"MagicLockScroll",		"PoisonScroll",
			"TelekinisisScroll",	"TeleportScroll",		"UnlockScroll",			"WallOfStoneScroll",

			"HarmWand",				"HealWand",				"ClumsyWand",			"FeebleWand",
			"ManaDrainWand",		"WeaknessWand",			"IDWand",
			
			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",

			"Cloak",				"Bonnet",              	"Cap",		           	"FeatheredHat",
			"FloppyHat",           	"JesterHat",			"Surcoat",				"SkullCap",
			"StrawHat",	           	"TallStrawHat",			"TricorneHat",			"WideBrimHat",
			"WizardsHat",			"BodySash",            	"Doublet",             	"Boots",
			"FullApron",           	"JesterSuit",          	"Sandals",				"Tunic",
			"Shoes",				"Shirt",				"Kilt",                	"Skirt",
			"FancyShirt",			"FancyDress",			"ThighBoots",			"LongPants",
			"PlainDress",          	"Robe",					"ShortPants",			"HalfApron",
		};

		public static List<string> Bank = new List<string>
		{
			"BulkOrderBook",		"Dices", 				"Backgammon",
			
			"Amber", 				"Amethyst", 			"Citrine", 				"Diamond",
			"Emerald", 				"Ruby", 				"Sapphire", 			"StarSapphire",
			"Tourmaline",
			
			"GoldNecklace", 		"GoldBeadNecklace", 	"SilverNecklace", 		"SilverBeadNecklace",
			"GoldRing", 			"SilverRing"			
		};

		public static List<string> Provisioner = new List<string> // food
		{
			"Cloak",				"Bonnet",              	"Cap",		           	"FeatheredHat",
			"FloppyHat",           	"JesterHat",			"Surcoat",				"SkullCap",
			"StrawHat",	           	"TallStrawHat",			"TricorneHat",			"WideBrimHat",
			"WizardsHat",			"BodySash",            	"Doublet",             	"Boots",
			"FullApron",           	"JesterSuit",          	"Sandals",				"Tunic",
			"Shoes",				"Shirt",				"Kilt",                	"Skirt",
			"FancyShirt",			"FancyDress",			"ThighBoots",			"LongPants",
			"PlainDress",          	"Robe",					"ShortPants",			"HalfApron",

			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",

			"FriedEggs", 			"Eggs",					"FishSteak", 			"RawFishSteak",
			"BreadLoaf", 			"FrenchBread", 			"SheafOfHay",			"Bacon", 
			"CookedBird", 			"Sausage",				"Ham", 					"Ribs", 
			"LambLeg",				"ChickenLeg",			"RawBird", 				"RawRibs", 
			"RawLambLeg",			"RawChickenLeg",		"HoneydewMelon", 		"YellowGourd", 
			"GreenGourd",			"Banana", 				"Bananas", 				"Lemon", 
			"Lime",					"Dates", 				"Grapes", 				"Peach", 
			"Pear",					"Apple", 				"Watermelon", 			"Squash",
			"Cantaloupe", 			"Carrot", 				"Cabbage",				"Onion", 
			"Lettuce", 				"Pumpkin"
		};

		public static List<string> Smith = new List<string> 
		{
			"IronIngot",			"DullCopperIngot",		"ShadowIronIngot",		"CopperIngot",
			"BronzeIngot",			"GoldIngot",			"AgapiteIngot",			"VeriteIngot",
			"ValoriteIngot",

			"BoneArms",				"BoneChest",			"BoneGloves",			"BoneLegs",
			"BoneHelm",				"ChainChest",			"ChainLegs",			"ChainCoif",			
			"Bascinet",				"CloseHelm",			"Helmet",				"NorseHelm",
			"OrcHelm",				"FemalePlateChest",		"PlateArms",			"PlateChest",			
			"PlateGloves",			"PlateGorget",			"PlateHelm",			"PlateLegs",
			"RingmailArms",			"RingmailChest",		"RingmailGloves",		"RingmailLegs",			

			"Axe",					"BattleAxe",			"DoubleAxe",			"ExecutionersAxe",		
			"Hatchet",				"LargeBattleAxe",		"TwoHandedAxe",			"WarAxe",				
			"Club",					"Mace",					"Maul",					"WarHammer",
			"WarMace",				"Bardiche",				"Halberd",				"Spear",				
			"ShortSpear",			"Pitchfork",			"WarFork",				"BlackStaff",			
			"GnarledStaff",			"QuarterStaff",			"Broadsword",			"Cutlass",
			"Katana",				"Kryss",				"Longsword",			"Scimitar",				
			"VikingSword",			"Pickaxe",				"HammerPick",			"ButcherKnife",			
			"Cleaver",				"Dagger",				"SkinningKnife",		"ShepherdsCrook",

			"Cloak",				"Bonnet",              	"Cap",		           	"FeatheredHat",
			"FloppyHat",           	"JesterHat",			"Surcoat",				"SkullCap",
			"StrawHat",	           	"TallStrawHat",			"TricorneHat",			"WideBrimHat",
			"WizardsHat",			"BodySash",            	"Doublet",             	"Boots",
			"FullApron",           	"JesterSuit",          	"Sandals",				"Tunic",
			"Shoes",				"Shirt",				"Kilt",                	"Skirt",
			"FancyShirt",			"FancyDress",			"ThighBoots",			"LongPants",
			"PlainDress",          	"Robe",					"ShortPants",			"HalfApron",

			"BronzeShield",			"Buckler",				"HeaterShield",			"MetalShield",
			"MetalKiteShield",		"WoodenKiteShield",		"WoodenShield"
		};

		public static List<string> Tailor = new List<string> //clothing armor
		{
			"Cloak",				"Bonnet",              	"Cap",		           	"FeatheredHat",
			"FloppyHat",           	"JesterHat",			"Surcoat",				"SkullCap",
			"StrawHat",	           	"TallStrawHat",			"TricorneHat",			"WideBrimHat",
			"WizardsHat",			"BodySash",            	"Doublet",             	"Boots",
			"FullApron",           	"JesterSuit",          	"Sandals",				"Tunic",
			"Shoes",				"Shirt",				"Kilt",                	"Skirt",
			"FancyShirt",			"FancyDress",			"ThighBoots",			"LongPants",
			"PlainDress",          	"Robe",					"ShortPants",			"HalfApron",

			"FemaleLeatherChest",	"LeatherArms",			"LeatherBustierArms",	"LeatherChest",
			"LeatherGloves",		"LeatherGorget",		"LeatherLegs",			"LeatherShorts",
			"LeatherSkirt",			"LeatherCap",
			"FemaleStuddedChest",	"StuddedArms",			"StuddedBustierArms",	"StuddedChest",			
			"StuddedGloves",		"StuddedGorget",		"StuddedLegs" 
		};

		public static List<string> Tinker = new List<string> //tinker carpenter bard
		{
			"IronIngot",			"DullCopperIngot",		"ShadowIronIngot",		"CopperIngot",
			"BronzeIngot",			"GoldIngot",			"AgapiteIngot",			"VeriteIngot",
			"ValoriteIngot",
			
			"Board",				"HeartwoodBoard",		"BloodwoodBoard",		"FrostwoodBoard",
			"OakBoard",				"AshBoard",				"YewBoard",
			
			"Scissors",				"MortarPestle",			"Scorp",				"TinkerTools",
			"Hatchet",				"DrawKnife",			"SewingKit",			"Saw",
			"DovetailSaw",			"Froe",					"Shovel",				"Hammer",
			"Tongs",				"SmithHammer",			"SledgeHammer",			"Inshave",
			"Pickaxe",				"Lockpick",				"Skillet",				"FlourSifter",
			"FletcherTools",		"MapmakersPen",			"ScribesPen",	

			"GoldNecklace", 		"GoldBeadNecklace", 	"SilverNecklace", 		"SilverBeadNecklace",
			"GoldRing", 			"SilverRing"
		};

		public static List<string> Mage = new List<string> // potions scrolls wands hats cloaks
		{
			"ReactiveArmorScroll",	"ClumsyScroll",			"CreateFoodScroll",		"FeeblemindScroll",
			"HealScroll",			"MagicArrowScroll",		"NightSightScroll",		"WeakenScroll",
			"AgilityScroll",		"CunningScroll",		"CureScroll",			"HarmScroll",
			"MagicTrapScroll",		"MagicUnTrapScroll",	"ProtectionScroll",		"StrengthScroll",
			"BlessScroll",			"FireballScroll",		"MagicLockScroll",		"PoisonScroll",
			"TelekinisisScroll",	"TeleportScroll",		"UnlockScroll",			"WallOfStoneScroll",

			"HarmWand",				"HealWand",				"ClumsyWand",			"FeebleWand",
			"ManaDrainWand",		"WeaknessWand",			"IDWand",
			
			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",
		};

		public static List<string> Cook = new List<string> //food
		{
			"FriedEggs", 			"Eggs",					"FishSteak", 			"RawFishSteak",
			"BreadLoaf", 			"FrenchBread", 			"SheafOfHay",			"Bacon", 
			"CookedBird", 			"Sausage",				"Ham", 					"Ribs", 
			"LambLeg",				"ChickenLeg",			"RawBird", 				"RawRibs", 
			"RawLambLeg",			"RawChickenLeg",		"HoneydewMelon", 		"YellowGourd", 
			"GreenGourd",			"Banana", 				"Bananas", 				"Lemon", 
			"Lime",					"Dates", 				"Grapes", 				"Peach", 
			"Pear",					"Apple", 				"Watermelon", 			"Squash",
			"Cantaloupe", 			"Carrot", 				"Cabbage",				"Onion", 
			"Lettuce", 				"Pumpkin",

			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",

			"HalfApron",
		};

		public static List<string> Gypsy = new List<string> //trinkets
		{
			"Amber", 				"Amethyst", 			"Citrine", 				"Diamond",
			"Emerald", 				"Ruby", 				"Sapphire", 			"StarSapphire",
			"Tourmaline",
			
			"GoldNecklace", 		"GoldBeadNecklace", 	"SilverNecklace", 		"SilverBeadNecklace",
			"GoldRing", 			"SilverRing",			

			"HarmWand",				"HealWand",				"ClumsyWand",			"FeebleWand",
			"ManaDrainWand",		"WeaknessWand",			"IDWand",
			
			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",
		};

		public static List<string> Random = new List<string>
		{
			"ReactiveArmorScroll",	"ClumsyScroll",			"CreateFoodScroll",		"FeeblemindScroll",
			"HealScroll",			"MagicArrowScroll",		"NightSightScroll",		"WeakenScroll",
			"AgilityScroll",		"CunningScroll",		"CureScroll",			"HarmScroll",
			"MagicTrapScroll",		"MagicUnTrapScroll",	"ProtectionScroll",		"StrengthScroll",
			"BlessScroll",			"FireballScroll",		"MagicLockScroll",		"PoisonScroll",
			"TelekinisisScroll",	"TeleportScroll",		"UnlockScroll",			"WallOfStoneScroll",

			"HarmWand",				"HealWand",				"ClumsyWand",			"FeebleWand",
			"ManaDrainWand",		"WeaknessWand",			"IDWand",
			
			"AgilityPotion",		"StrengthPotion",		"RefreshPotion",		"LesserCurePotion",
			"LesserHealPotion",		"LesserPoisonPotion",

			"Cloak",				"Bonnet",              	"Cap",		           	"FeatheredHat",
			"FloppyHat",           	"JesterHat",			"Surcoat",				"SkullCap",
			"StrawHat",	           	"TallStrawHat",			"TricorneHat",			"WideBrimHat",
			"WizardsHat",			"BodySash",            	"Doublet",             	"Boots",
			"FullApron",           	"JesterSuit",          	"Sandals",				"Tunic",
			"Shoes",				"Shirt",				"Kilt",                	"Skirt",
			"FancyShirt",			"FancyDress",			"ThighBoots",			"LongPants",
			"PlainDress",          	"Robe",					"ShortPants",			"HalfApron",

			"FemaleLeatherChest",	"LeatherArms",			"LeatherBustierArms",	"LeatherChest",
			"LeatherGloves",		"LeatherGorget",		"LeatherLegs",			"LeatherShorts",
			"LeatherSkirt",			"LeatherCap",
			"FemaleStuddedChest",	"StuddedArms",			"StuddedBustierArms",	"StuddedChest",			
			"StuddedGloves",		"StuddedGorget",		"StuddedLegs"
		};
	}
}