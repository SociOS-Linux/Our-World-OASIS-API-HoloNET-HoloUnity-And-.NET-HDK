﻿using System;
using System.Reflection;
using NextGenSoftware.OASIS.API.Core;
using NextGenSoftware.OASIS.API.OASISAPIManager;

namespace NextGenSoftware.OASIS.STAR.TestHarness
{
    class Program
    {
        static Planet ourWorld;

        static void Main(string[] args)
        {
            var versionString = Assembly.GetEntryAssembly()
                                       .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                       .InformationalVersion
                                       .ToString();

            Console.WriteLine($"********************************************************************");
            Console.WriteLine($"NextGen Software STAR ODK TEST HARNESS v{versionString}");
            Console.WriteLine($"********************************************************************");
            Console.WriteLine("\nUsage:");
            Console.WriteLine("  star beamin = Log in");
            Console.WriteLine("  star beamout = Log out");
            Console.WriteLine("  star light -dnaFolder -cSharpGeneisFolder -rustGenesisFolder = Creates a new Planet (OAPP) at the given folder genesis locations, from the given OAPP DNA.");
            Console.WriteLine("  star light -transmute -hAppDNA -cSharpGeneisFolder -rustGenesisFolder = Creates a new Planet (OAPP) at the given folder genesis locations, from the given hApp DNA.");
            Console.WriteLine("  star flare -planetName = Build a planet (OAPP).");
            Console.WriteLine("  star shine -planetName = Launch & activate a planet (OAPP) by shining the star's light upon it...");
            Console.WriteLine("  star dim -planetName = Deactivate a planet (OAPP).");
            Console.WriteLine("  star seed -planetName = Deploy a planet (OAPP).");
            Console.WriteLine("  star twinkle -planetName = Deactivate a planet (OAPP).");
            Console.WriteLine("  star dust -planetName = Delete a planet (OAPP).");
            Console.WriteLine("  star radiate -planetName = Highlight the Planet (OAPP) in the OAPP Store (StarNET). *Admin Only*");
            Console.WriteLine("  star emit -planetName = Show how much light the planet (OAPP) is emitting into the solar system (StarNET/HoloNET)");
            Console.WriteLine("  star reflect -planetName = Show stats of the Planet (OAPP).");
            Console.WriteLine("  star evolve -planetName = Upgrade/update a Planet (OAPP).");
            Console.WriteLine("  star mutate -planetName = Import/Export hApp, dApp & others.");
            Console.WriteLine("  star love -planetName = Send/Receive Love.");
            Console.WriteLine("  star burst = View network stats/management/settings.");
            Console.WriteLine("  star super - Reserved For Future Use...");
            Console.WriteLine($"********************************************************************");


            string dnaFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\CelestialBodyDNA";
            string cSharpGeneisFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\bin\Release\net5.0\Genesis\CSharp";
            string rustGenesisFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\bin\Release\net5.0\Genesis\Rust";

            // TODO: Not sure what events should expose on Star, StarCore and HoloNETClient?
            // I feel the events should at least be on the Star object, but then they need to be on the others to bubble them up (maybe could be hidden somehow?)
            SuperStar.OnZomeError += Star_OnZomeError;
            SuperStar.OnHolonLoaded += Star_OnHolonLoaded;
            SuperStar.OnHolonsLoaded += Star_OnHolonsLoaded;
            SuperStar.OnHolonSaved += Star_OnHolonSaved;
            SuperStar.OnInitialized += Star_OnInitialized;
            SuperStar.OnStarError += Star_OnStarError;

          //  Star.StarCore.OnZomeError += StarCore_OnZomeError;
          //  Star.StarCore.HoloNETClient.OnError += HoloNETClient_OnError;

            Console.WriteLine("Beaming In...");
            Console.WriteLine("");

            Avatar avatar = SuperStar.BeamIn("david@nextgensoftware.co.uk", "lettherebelight").Result;

            if (avatar != null)
            {
                Console.WriteLine(string.Concat("Successfully Beamed In! Welcome back ", avatar.Name, ". Have a nice day! :)"));
                Console.WriteLine(string.Concat("Karma: ", avatar.Karma));
                Console.WriteLine(string.Concat("Level: ", avatar.Level));
                Console.WriteLine("");

                // Create Planet (OAPP) by generating dynamic template/scaffolding code.
                Console.WriteLine("Generating Planet Our World...");
                CoronalEjection result = SuperStar.Light(GenesisType.Planet, "Our World", dnaFolder, cSharpGeneisFolder, rustGenesisFolder, "NextGenSoftware.Holochain.HoloNET.HDK.Core.TestHarness.Genesis").Result;

                if (result.ErrorOccured)
                    Console.WriteLine(string.Concat("ERROR OCCURED: Error Message: ", result.Message));
                else
                {
                    Console.WriteLine("Planet Our World Generated.");
                    ourWorld = result.CelestialBody as Planet;

                    ourWorld.OnHolonLoaded += OurWorld_OnHolonLoaded;
                    ourWorld.OnHolonSaved += OurWorld_OnHolonSaved;
                    ourWorld.OnZomeError += OurWorld_OnZomeError;

                    ourWorld.LoadAll();
                    //ourWorld.Zomes.Add()

                    Holon newHolon = new Holon();
                    newHolon.Name = "Test Data";
                    newHolon.Description = "Test Desc";
                    newHolon.HolonType = HolonType.Park;

                    Console.WriteLine("Saving Holon...");

                    // If you are using the generated code from Light above (highly recommended) you do not need to pass the HolonTypeName in, you only need to pass the holon in.
                    //ourWorld.CelestialBodyCore.SaveHolonAsync("Test", newHolon);
                    ourWorld.CelestialBodyCore.SaveHolonAsync(newHolon);


                    OASISAPIManager.AvatarManager.LoadAllAvatars();
                    OASISAPIManager.MapManager.CreateAndDrawRouteOnMapBetweenHolons(newHolon, newHolon);
                    OASISAPIManager.SEEDAPI.JoinSeeds();
                    OASISAPIManager.SEEDAPI.PayWithSeeds();



                    // Build
                    CoronalEjection ejection = ourWorld.Flare();
                    //OR
                    //CoronalEjection ejection = Star.Flare(ourWorld);

                    // Activate & Launch - Launch & activate the planet (OAPP) by shining the star's light upon it...
                    SuperStar.Shine(ourWorld);
                    ourWorld.Shine();

                    // Deactivate the planet (OAPP)
                    SuperStar.Dim(ourWorld);

                    // Deploy the planet (OAPP)
                    SuperStar.Seed(ourWorld);

                    // Run Tests
                    SuperStar.Twinkle(ourWorld);

                    // Highlight the Planet (OAPP) in the OAPP Store (StarNET). *Admin Only*
                    SuperStar.Radiate(ourWorld);

                    // Show how much light the planet (OAPP) is emitting into the solar system (StarNET/HoloNET)
                    SuperStar.Emit(ourWorld);

                    // Show stats of the Planet (OAPP).
                    SuperStar.Reflect(ourWorld);

                    // Upgrade/update a Planet (OAPP).
                    SuperStar.Evolve(ourWorld);

                    // Import/Export hApp, dApp & others.
                    SuperStar.Mutate(ourWorld);

                    // Send/Receive Love
                    SuperStar.Love(ourWorld);

                    // Show network stats/management/settings
                    SuperStar.Burst(ourWorld);

                    // Reserved For Future Use...
                    SuperStar.Super(ourWorld);

                    // Delete a planet (OAPP).
                    SuperStar.Dust(ourWorld);
                }
            }
            else
                Console.WriteLine("Error Beaming In.");
        }

        private static void Star_OnStarError(object sender, StarErrorEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "EndPoint: ", e.EndPoint));
        }

        //private static void HoloNETClient_OnError(object sender, Client.Core.HoloNETErrorEventArgs e)
        //{
        //    Console.WriteLine(string.Concat("HoloNET Client Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "EndPoint: ", e.EndPoint));
        //}

        private static void StarCore_OnZomeError(object sender, ZomeErrorEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Core Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
        }

        private static void Star_OnInitialized(object sender, EventArgs e)
        {
            Console.WriteLine("Star Initialized.");
        }

        private static void Star_OnHolonSaved(object sender, HolonSavedEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Holons Saved. Holon Saved: ", e.Holon.Name));
        }

        private static void Star_OnHolonsLoaded(object sender, HolonsLoadedEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Holons Loaded. Holons Loaded: ", e.Holons.Count));
        }

        private static void Star_OnHolonLoaded(object sender, HolonLoadedEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Holons Loaded. Holon Name: ", e.Holon.Name));
        }

        private static void Star_OnZomeError(object sender, ZomeErrorEventArgs e)
        {
            Console.WriteLine(string.Concat("Star Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
        }

        private static void OurWorld_OnZomeError(object sender, ZomeErrorEventArgs e)
        {
            Console.WriteLine(string.Concat("Our World Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
        }

        private static void OurWorld_OnHolonSaved(object sender, HolonSavedEventArgs e)
        {
            Console.WriteLine("Holon Saved");
            Console.WriteLine(string.Concat("Holon Id: ", e.Holon.Id));
            Console.WriteLine(string.Concat("Holon ProviderKey: ", e.Holon.ProviderKey));
            Console.WriteLine(string.Concat("Holon Name: ", e.Holon.Name));
            Console.WriteLine(string.Concat("Holon Type: ", e.Holon.HolonType));
            Console.WriteLine(string.Concat("Holon Description: ", e.Holon.Description));

            Console.WriteLine("Loading Holon...");
            //ourWorld.CelestialBodyCore.LoadHolonAsync(e.Holon.Name, e.Holon.ProviderKey);
            ourWorld.CelestialBodyCore.LoadHolonAsync(e.Holon.ProviderKey);
        }

        private static void OurWorld_OnHolonLoaded(object sender, HolonLoadedEventArgs e)
        {
            Console.WriteLine("Holon Loaded");
            Console.WriteLine(string.Concat("Holon Id: ", e.Holon.Id));
            Console.WriteLine(string.Concat("Holon ProviderKey: ", e.Holon.ProviderKey));
            Console.WriteLine(string.Concat("Holon Name: ", e.Holon.Name));
            Console.WriteLine(string.Concat("Holon Type: ", e.Holon.HolonType));
            Console.WriteLine(string.Concat("Holon Description: ", e.Holon.Description));

            //Console.WriteLine(string.Concat("ourWorld.Zomes[0].Holons[0].ProviderKey: ", ourWorld.Zomes[0].Holons[0].ProviderKey));
            Console.WriteLine(string.Concat("ourWorld.Zomes[0].Holons[0].ProviderKey: ", ourWorld.CelestialBodyCore.Zomes[0].Holons[0].ProviderKey));
        }
    }
}