using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom
{
    public class ModernMobScanner
    {
        // Mantieni solo le classi effettivamente presenti nel tuo shard!
        public static readonly Type[] ModernTypes = new Type[]
        {
			typeof( AntLion ),			
			typeof( ArcticOgreLord ),	
			typeof( BogThing ),
			typeof( Bogle ),			
			//typeof( BoneKnight ),		
			//typeof( FrostOoze ),		
			//typeof( FrostTroll ),
			typeof( GazerLarva ),		
			typeof( Golem ),
			typeof( Jwilson ),			
			typeof( PlagueBeast ),
			typeof( Quagmire ),			
			typeof( RestlessSoul ),
			typeof( CrystalElemental ),	
			typeof( DarknightCreeper ),	
			typeof( MoundOfMaggots ),
			typeof( Juggernaut ),
			typeof( LordOaks ),			
			typeof( Silvani ),			
			//typeof( AncientWyrm ),		
			//typeof( Balron ),			
			typeof( DreadSpider ),		
			//typeof( Efreet ),			
			typeof( EtherealWarrior ),
			//typeof( Nightmare ),		
			//typeof( OphidianArchmage ),
			//typeof( OphidianMage ),		
			//typeof( OphidianWarrior ),	
			//typeof( OphidianMatriarch ),
			//typeof( OphidianKnight ),	
			//typeof( PoisonElemental ),	
			typeof( Revenant ),
			typeof( SandVortex ),		
			typeof( SavageRider ),		
			typeof( SavageShaman ),
			//typeof( SnowElemental ),	
			//typeof( WhiteWyrm ),		
			//typeof( Wisp ),
			typeof( GiantBlackWidow ), 
			//typeof( DemonKnight ),
			typeof( Barracoon ),		
			typeof( Mephitis ),			
			typeof( Neira ),
			typeof( Rikktor ),			
			typeof( Semidar ),			
			typeof( Beetle ),			
			typeof( Pixie ),			
			typeof( SilverSerpent ),
			typeof( VorpalBunny ),		
			typeof( FleshRenderer ),	
			typeof( KhaldunRevenant ),
			//typeof( ToxicElemental ),	
			//typeof( AgapiteElemental ),	
			typeof( Betrayer ),			
			typeof( BlackSolenInfiltratorQueen ), 
			typeof( BlackSolenInfiltratorWarrior ),
			typeof( BlackSolenQueen ),	
			typeof( BlackSolenWarrior ), 
			typeof( BlackSolenWorker ),
			//typeof( BloodElemental ),	
			typeof( Bogling ),
			//typeof( BronzeElemental ),
			typeof( Centaur ),			
			typeof( ChaosDaemon ),
			typeof( GolemController ),	
			//typeof( CopperElemental ),	
			//typeof( Cyclops ),			
			//typeof( DesertOstard ),		
			//typeof( DullCopperElemental ), 
			typeof( Executioner ),
			typeof( Savage ),			
			//typeof( FireElemental ),	
			//typeof( FireGargoyle ),
			typeof( FireSteed ),		
			//typeof( ForestOstard ),		
			//typeof( FrenziedOstard ),
			//typeof( FrostSpider ),		
			//typeof( Gargoyle ),			
			//typeof( IceSerpent ),		
			//typeof( GiantSerpent ),
			//typeof( GiantSpider ),		
			//typeof( GiantToad ),		
			//typeof( GoldenElemental ),	
			typeof( Guardian ),
			typeof( Harrower ),			
			//typeof( HellHound ),
			typeof( HordeMinion ),		
			//typeof( IceElemental ),		
			//typeof( IceFiend ),
			//typeof( IceSnake ),			
			//typeof( Imp ),				
			typeof( Kirin ),			
			typeof( PredatorHellCat ),
			//typeof( LavaLizard ),		
			//typeof( LavaSerpent ),		
			//typeof( LavaSnake ),
			typeof( PlagueSpawn ),		
			typeof( RedSolenInfiltratorQueen ), 
			typeof( RedSolenInfiltratorWarrior ), 
			typeof( RedSolenQueen ),
			typeof( RedSolenWarrior ),	
			typeof( RedSolenWorker ),	
			//typeof( RidableLlama ),
			typeof( Ridgeback ),		
			typeof( SerpentineDragon ),	
			//typeof( ShadowIronElemental ),
			typeof( ShadowWisp ),		
			//typeof( ShadowWyrm ),		
			typeof( SilverSteed ),		
			typeof( SkeletalDragon ),	
			typeof( SkeletalMage ),
			typeof( SkeletalMount ),	
			//typeof( HellCat ),			
			typeof( SpectralArmour ),	
			//typeof( StoneGargoyle ),	
			typeof( SwampDragon ),
			typeof( ScaledSwampDragon ), 
			//typeof( SwampTentacle ),	
			//typeof( TerathanAvenger ),
			//typeof( TerathanDrone ),	
			//typeof( TerathanMatriarch ), 
			//typeof( TerathanWarrior ),
			//typeof( TimberWolf ),		
			//typeof( Titan ),			
			typeof( Unicorn ),			
			//typeof( ValoriteElemental ), 
			//typeof( VeriteElemental ),
			typeof( CoMWarHorse ),		
			typeof( WhippingVine ),
			//typeof( WhiteWolf ),		
			typeof( KhaldunZealot ),	
			typeof( KhaldunSummoner ),	
			typeof( SavageRidgeback ),
			typeof( MeerWarrior ),		
			typeof( MeerEternal ),		
			typeof( MeerMage ),
			typeof( MeerCaptain ),		
			typeof( JukaLord ),			
			typeof( JukaMage ),
			typeof( JukaWarrior ),		
			typeof( AbysmalHorror ),	
			typeof( BoneDemon ),
			typeof( Devourer ),			
			typeof( FleshGolem ),		
			typeof( Gibberling ),
			typeof( GoreFiend ),		
			typeof( Impaler ),			
			typeof( PatchworkSkeleton ),
			typeof( Ravager ),			
			typeof( ShadowKnight ),		
			typeof( SkitteringHopper ),
			typeof( Treefellow ),		
			typeof( VampireBat ),		
			typeof( WailingBanshee ),
			typeof( WandererOfTheVoid ),	
			typeof( Cursed ),			
			typeof( GrimmochDrummel ),
			typeof( LysanderGathenwale),	
			typeof( MorgBergen ),	
			typeof( ShadowFiend ),
			typeof( SpectralArmour ),	
			typeof( TavaraSewel ),		
			typeof( ArcaneDaemon ),
			typeof( Doppleganger ),		
			typeof( EnslavedGargoyle ), 
			typeof( ExodusMinion ),
			typeof( ExodusOverseer ),	
			typeof( GargoyleDestroyer ),	
			typeof( GargoyleEnforcer ),
			typeof( Moloch )
        };

        public static void Initialize()
        {
            CommandSystem.Register("ScanModernMobs", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        public static void OnCommand(CommandEventArgs e)
        {
            int found = 0;
            List<string> results = new List<string>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                foreach (Type t in ModernTypes)
                {
                    if (t.IsInstanceOfType(m))
                    {
                        // Per proprietà extra controlla se è un BaseCreature
                        BaseCreature bc = m as BaseCreature;
                        string info = "Trovato: " + t.Name + " - Serial: " + m.Serial + " - Posizione: " + m.Location + " (" + m.Map + ")";
                        if (bc != null && bc.Controlled && bc.ControlMaster != null)
                            info += " [Controllato da: " + bc.ControlMaster.Name + "]";
                        else
                            info += " [Libero]";

                        results.Add(info);
                        found++;
                        break;
                    }
                }
            }

            if (found == 0)
            {
                e.Mobile.SendMessage(61, "Nessun pet/mount moderno trovato nel mondo.");
            }
            else
            {
                e.Mobile.SendMessage(33, "Trovati " + found + " pet/mount moderni nel mondo:");
                foreach (string s in results)
                    e.Mobile.SendMessage(33, s);
            }
        }
    }
}