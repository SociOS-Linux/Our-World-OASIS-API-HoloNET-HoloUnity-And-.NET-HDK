﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using EOSNewYork.EOSCore.Response.API;
using Nethereum.Contracts;
using NextGenSoftware.OASIS.API.Core.Interfaces;
using NextGenSoftware.OASIS.API.Core.Enums;
using NextGenSoftware.OASIS.API.Core.Events;
using NextGenSoftware.OASIS.API.Core.Holons;
using NextGenSoftware.OASIS.API.Core.Managers;
using NextGenSoftware.OASIS.API.Core.Objects;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.STAR.ErrorEventArgs;
using NextGenSoftware.OASIS.STAR.CelestialBodies;
using NextGenSoftware.OASIS.API.Providers.SEEDSOASIS.Membranes;

namespace NextGenSoftware.OASIS.STAR.TestHarness
{
    class Program
    {
        static Planet ourWorld;

        static async Task Main(string[] args)
        {
            OASISResult<IAvatar> beamInResult = null;
            string privateKey = ""; //Set to privatekey when testing BUT remember to remove again before checking in code! Better to use avatar methods so private key is retreived from avatar and then no need to pass them in.
            var versionString = Assembly.GetEntryAssembly()
                                       .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                       .InformationalVersion
                                       .ToString();

            Console.WriteLine($"********************************************************************");
            Console.WriteLine($"NextGen Software STAR (Synergiser Transformer Aggregator Resolver) HDK/ODK TEST HARNESS v{versionString}");
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


            //string dnaFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\CelestialBodyDNA";
            //string cSharpGeneisFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\bin\Release\net5.0\Genesis\CSharp";
            //string rustGenesisFolder = @"C:\CODE\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\NextGenSoftware.OASIS.STAR.TestHarness\bin\Release\net5.0\Genesis\Rust";

            string dnaFolder = "C:\\CODE\\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\\NextGenSoftware.OASIS.STAR.TestHarness\\CelestialBodyDNA";
            string cSharpGeneisFolder = "C:\\CODE\\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\\NextGenSoftware.OASIS.STAR.TestHarness\\bin\\Release\\net5.0\\Genesis\\CSharp";
            string rustGenesisFolder = "C:\\CODE\\Our-World-OASIS-API-HoloNET-HoloUnity-And-.NET-HDK\\NextGenSoftware.OASIS.STAR.TestHarness\\bin\\Release\\net5.0\\Genesis\\Rust";


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

            Console.WriteLine("");
            Console.WriteLine("Welcome to STAR (The Heart Of The OASIS)");

            while (beamInResult == null || (beamInResult != null && beamInResult.IsError)) 
            {
                Console.WriteLine("");
                Console.WriteLine("Please login below:");
                Console.WriteLine("");
                Console.WriteLine("Username/Email?");
                string username = Console.ReadLine();
                Console.WriteLine("");
                Console.WriteLine("Password?");

                ConsoleKey key;
                string password = "";
                do
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        Console.Write("\b \b");
                        password = password[0..^1];
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write("*");
                        password += keyInfo.KeyChar;
                    }
                } while (key != ConsoleKey.Enter);

               // string password = Console.ReadLine();

                Console.WriteLine("");
                Console.WriteLine("Beaming In...");
                Console.WriteLine("");

                // If you wish to change the default init options for STAR then manually call the Initialize method below, otherwise STAR will init with default options.
                //SuperStar.Initialize(InitOptions.InitWithCurrentDefaultProvider);
                beamInResult = SuperStar.BeamIn("davidellams@hotmail.com", "my-super-secret-password");
               // beamInResult = SuperStar.BeamIn(username, password);

                if (beamInResult.IsError)
                    Console.WriteLine(string.Concat("Error logging in. Error Message: ", beamInResult.ErrorMessage));

                else if (SuperStar.LoggedInUser == null)
                    Console.WriteLine("Error Beaming In. Username/Password may be incorrect.");
            }

            Console.WriteLine(string.Concat("Successfully Beamed In! Welcome back ", SuperStar.LoggedInUser.FullName, ". Have a nice day! :)"));
            
            Console.WriteLine("");
            Console.WriteLine(string.Concat("Karma: ", SuperStar.LoggedInUser.Karma));
            Console.WriteLine(string.Concat("Level: ", SuperStar.LoggedInUser.Level));
            Console.WriteLine(string.Concat("XP: ", SuperStar.LoggedInUser.XP));

            Console.WriteLine("");
            Console.WriteLine("Chakras:");
            Console.WriteLine(string.Concat("Crown XP: ", SuperStar.LoggedInUser.Chakras.Crown.XP));
            Console.WriteLine(string.Concat("Crown Level: ", SuperStar.LoggedInUser.Chakras.Crown.Level));
            Console.WriteLine(string.Concat("ThirdEye XP: ", SuperStar.LoggedInUser.Chakras.ThirdEye.XP));
            Console.WriteLine(string.Concat("ThirdEye Level: ", SuperStar.LoggedInUser.Chakras.ThirdEye.Level));
            Console.WriteLine(string.Concat("Throat XP: ", SuperStar.LoggedInUser.Chakras.Throat.XP));
            Console.WriteLine(string.Concat("Throat Level: ", SuperStar.LoggedInUser.Chakras.Throat.Level));
            Console.WriteLine(string.Concat("Heart XP: ", SuperStar.LoggedInUser.Chakras.Heart.XP));
            Console.WriteLine(string.Concat("Heart Level: ", SuperStar.LoggedInUser.Chakras.Heart.Level));
            Console.WriteLine(string.Concat("SoloarPlexus XP: ", SuperStar.LoggedInUser.Chakras.SoloarPlexus.XP));
            Console.WriteLine(string.Concat("SoloarPlexus Level: ", SuperStar.LoggedInUser.Chakras.SoloarPlexus.Level));
            Console.WriteLine(string.Concat("Sacral XP: ", SuperStar.LoggedInUser.Chakras.Sacral.XP));
            Console.WriteLine(string.Concat("Sacral Level: ", SuperStar.LoggedInUser.Chakras.Sacral.Level));
            Console.WriteLine(string.Concat("Root XP: ", SuperStar.LoggedInUser.Chakras.Root.XP));
            Console.WriteLine(string.Concat("Root Level: ", SuperStar.LoggedInUser.Chakras.Root.Level));

            Console.WriteLine("");
            Console.WriteLine("Attributes:");
            Console.WriteLine(string.Concat("Strength: ", SuperStar.LoggedInUser.Attributes.Strength));
            Console.WriteLine(string.Concat("Speed: ", SuperStar.LoggedInUser.Attributes.Speed));
            Console.WriteLine(string.Concat("Dexterity: ", SuperStar.LoggedInUser.Attributes.Dexterity));
            Console.WriteLine(string.Concat("Intelligence: ", SuperStar.LoggedInUser.Attributes.Intelligence));
            Console.WriteLine(string.Concat("Magic: ", SuperStar.LoggedInUser.Attributes.Magic));
            Console.WriteLine(string.Concat("Wisdom: ", SuperStar.LoggedInUser.Attributes.Wisdom));
            Console.WriteLine(string.Concat("Toughness: ", SuperStar.LoggedInUser.Attributes.Toughness));

            Console.WriteLine("");
            Console.WriteLine("Super Powers:");
            Console.WriteLine(string.Concat("Flight: ", SuperStar.LoggedInUser.SuperPowers.Flight));
            Console.WriteLine(string.Concat("AstralProjection: ", SuperStar.LoggedInUser.SuperPowers.AstralProjection));
            Console.WriteLine(string.Concat("BioLocatation: ", SuperStar.LoggedInUser.SuperPowers.BioLocatation));
            Console.WriteLine(string.Concat("HeatVision: ", SuperStar.LoggedInUser.SuperPowers.HeatVision));
            Console.WriteLine(string.Concat("Invulerability: ", SuperStar.LoggedInUser.SuperPowers.Invulerability));
            Console.WriteLine(string.Concat("RemoteViewing: ", SuperStar.LoggedInUser.SuperPowers.RemoteViewing));
            Console.WriteLine(string.Concat("SuperSpeed: ", SuperStar.LoggedInUser.SuperPowers.SuperSpeed));
            Console.WriteLine(string.Concat("SuperStrength: ", SuperStar.LoggedInUser.SuperPowers.SuperStrength));
            Console.WriteLine(string.Concat("Telekineseis: ", SuperStar.LoggedInUser.SuperPowers.Telekineseis));
            Console.WriteLine(string.Concat("XRayVision: ", SuperStar.LoggedInUser.SuperPowers.XRayVision));

            Console.WriteLine("");
            Console.WriteLine("Skills:");
            Console.WriteLine(string.Concat("Computers: ", SuperStar.LoggedInUser.Skills.Computers));
            Console.WriteLine(string.Concat("Engineering: ", SuperStar.LoggedInUser.Skills.Engineering));
            Console.WriteLine(string.Concat("Farming: ", SuperStar.LoggedInUser.Skills.Farming));
            Console.WriteLine(string.Concat("FireStarting: ", SuperStar.LoggedInUser.Skills.FireStarting));
            Console.WriteLine(string.Concat("Fishing: ", SuperStar.LoggedInUser.Skills.Fishing));
            Console.WriteLine(string.Concat("Languages: ", SuperStar.LoggedInUser.Skills.Languages));
            Console.WriteLine(string.Concat("Meditation: ", SuperStar.LoggedInUser.Skills.Meditation));
            Console.WriteLine(string.Concat("MelleeCombat: ", SuperStar.LoggedInUser.Skills.MelleeCombat));
            Console.WriteLine(string.Concat("Mindfulness: ", SuperStar.LoggedInUser.Skills.Mindfulness));
            Console.WriteLine(string.Concat("Negotiating: ", SuperStar.LoggedInUser.Skills.Negotiating));
            Console.WriteLine(string.Concat("RangeCombat: ", SuperStar.LoggedInUser.Skills.RangeCombat));
            Console.WriteLine(string.Concat("Research: ", SuperStar.LoggedInUser.Skills.Research));
            Console.WriteLine(string.Concat("Science: ", SuperStar.LoggedInUser.Skills.Science));
            Console.WriteLine(string.Concat("SpellCasting: ", SuperStar.LoggedInUser.Skills.SpellCasting));
            Console.WriteLine(string.Concat("Translating: ", SuperStar.LoggedInUser.Skills.Translating));
            Console.WriteLine(string.Concat("Yoga: ", SuperStar.LoggedInUser.Skills.Yoga));

            Console.WriteLine("");
            Console.WriteLine("Gifts:");

            foreach (AvatarGift gift in SuperStar.LoggedInUser.Gifts)
                Console.WriteLine(string.Concat(Enum.GetName(gift.GiftType), " earnt on ", gift.GiftEarnt.ToString()));

            Console.WriteLine("");
            Console.WriteLine("Spells:");
                
            foreach (Spell spell in SuperStar.LoggedInUser.Spells)
                Console.WriteLine(string.Concat(spell.Name));

            Console.WriteLine("");
            Console.WriteLine("Inventory:");

            foreach (InventoryItem inventoryItem in SuperStar.LoggedInUser.Inventory)
                Console.WriteLine(string.Concat(inventoryItem.Name));

            Console.WriteLine("");
            Console.WriteLine("Achievements:");

            foreach (Achievement achievement in SuperStar.LoggedInUser.Achievements)
                Console.WriteLine(string.Concat(achievement.Name));

            Console.WriteLine("");
            Console.WriteLine("Gene Keys:");

            foreach (GeneKey geneKey in SuperStar.LoggedInUser.GeneKeys)
                Console.WriteLine(string.Concat(geneKey.Name));

            Console.WriteLine("");
            Console.WriteLine("Human Design:");
            Console.WriteLine(string.Concat("Type: ", SuperStar.LoggedInUser.HumanDesign.Type));

            Console.WriteLine("");
            Console.WriteLine("READY PLAYER ONE?");
            Console.WriteLine("");

            // Create Planet (OAPP) by generating dynamic template/scaffolding code.
            Console.WriteLine("Generating Planet Our World...");
            Console.WriteLine("");
            CoronalEjection result = SuperStar.Light(GenesisType.Planet, "Our World", dnaFolder, cSharpGeneisFolder, rustGenesisFolder, "NextGenSoftware.Holochain.HoloNET.HDK.Core.TestHarness.Genesis").Result;

            if (result.ErrorOccured)
                Console.WriteLine(string.Concat("ERROR OCCURED. Error Message: ", result.Message));

            else
            {
                Console.WriteLine("Planet Our World Generated.");
                ourWorld = result.CelestialBody as Planet;

                Console.WriteLine("");
                Console.WriteLine(string.Concat("Id: ", ourWorld.Id));
                Console.WriteLine(string.Concat("CreatedByAvatarId: ", ourWorld.CreatedByAvatarId));
                Console.WriteLine(string.Concat("CreatedDate: ", ourWorld.CreatedDate));
                Console.WriteLine("");

                ourWorld.OnHolonLoaded += OurWorld_OnHolonLoaded;
                ourWorld.OnHolonSaved += OurWorld_OnHolonSaved;
                ourWorld.OnZomeError += OurWorld_OnZomeError;

                Console.WriteLine("Loading Zomes & Holons...");
                ourWorld.LoadAll();
                //ourWorld.Zomes.Add()

                Holon newHolon = new Holon();
                newHolon.Name = "Test Data";
                newHolon.Description = "Test Desc";
                newHolon.HolonType = HolonType.Park;

                Console.WriteLine("Saving Holon...");

                // If you are using the generated code from Light above (highly recommended) you do not need to pass the HolonTypeName in, you only need to pass the holon in.
                //ourWorld.CelestialBodyCore.SaveHolonAsync("Test", newHolon);
                await ourWorld.CelestialBodyCore.SaveHolonAsync(newHolon);


                // BEGIN OASIS API DEMO ***********************************************************************************

                //Set auto-replicate for all providers except IPFS and Neo4j.
                ProviderManager.SetAutoReplicateForAllProviders(true);
                ProviderManager.SetAutoReplicationForProviders(false, new List<ProviderType>() { ProviderType.IPFSOASIS, ProviderType.Neo4jOASIS });

                //Set auto-failover for all providers except Holochain.
                ProviderManager.SetAutoFailOverForAllProviders(true);
                ProviderManager.SetAutoFailOverForProviders(false, new List<ProviderType>() { ProviderType.HoloOASIS });

                //Set auto-load balance for all providers except Ethereum.
                ProviderManager.SetAutoLoadBalanceForAllProviders(true);
                ProviderManager.SetAutoLoadBalanceForProviders(false, new List<ProviderType>() { ProviderType.EthereumOASIS });

                //  Set the default provider to MongoDB.
                ProviderManager.SetAndActivateCurrentStorageProvider(ProviderType.MongoDBOASIS, true); // Set last param to false if you wish only the next call to use this provider.


                     
                //  Give HoloOASIS Store permission for the Name field(the field will only be stored on Holochain).
                SuperStar.OASISAPI.Avatar.Config.FieldToProviderMappings.Name.Add(new ProviderManagerConfig.FieldToProviderMappingAccess { Access = ProviderManagerConfig.ProviderAccess.Store, Provider = ProviderType.HoloOASIS });

                // Give all providers read/write access to the Karma field (will allow them to read and write to the field but it will only be stored on Holochain).
                // You could choose to store it on more than one provider if you wanted the extra redundancy (but not normally needed since Holochain has a lot of redundancy built in).
                SuperStar.OASISAPI.Avatar.Config.FieldToProviderMappings.Karma.Add(new ProviderManagerConfig.FieldToProviderMappingAccess { Access = ProviderManagerConfig.ProviderAccess.ReadWrite, Provider = ProviderType.All });

                //Give Ethereum read-only access to the DOB field.
                SuperStar.OASISAPI.Avatar.Config.FieldToProviderMappings.DOB.Add(new ProviderManagerConfig.FieldToProviderMappingAccess { Access = ProviderManagerConfig.ProviderAccess.ReadOnly, Provider = ProviderType.EthereumOASIS });


                // All calls are load-balanced and have multiple redudancy/fail over for all supported OASIS Providers.
                SuperStar.OASISAPI.Avatar.LoadAllAvatars(); // Load-balanced across all providers.
                SuperStar.OASISAPI.Avatar.LoadAllAvatars(ProviderType.MongoDBOASIS); // Only loads from MongoDB.
                SuperStar.OASISAPI.Avatar.LoadAvatar(SuperStar.LoggedInUser.Id, ProviderType.HoloOASIS); // Only loads from Holochain.
                SuperStar.OASISAPI.Map.CreateAndDrawRouteOnMapBetweenHolons(newHolon, newHolon); // Load-balanced across all providers.

                SuperStar.OASISAPI.Data.LoadHolon(newHolon.Id); // Load-balanced across all providers.
                SuperStar.OASISAPI.Data.LoadHolon(newHolon.Id, HolonType.All, ProviderType.IPFSOASIS); // Only loads from IPFS.
                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.Moon, ProviderType.HoloOASIS); // Loads all moon (OAPPs) from Holochain.
                SuperStar.OASISAPI.Data.SaveHolon(newHolon); // Load-balanced across all providers.
                SuperStar.OASISAPI.Data.SaveHolon(newHolon, ProviderType.EthereumOASIS); //  Only saves to Etherum.

                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.All, ProviderType.Default); // Loads all parks from current default provider.
                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.Park, ProviderType.All); // Loads all parks from all providers (load-balanced/fail over).
                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.Park); // shorthand for above.
                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.Quest); //  Loads all quests from all providers.
                SuperStar.OASISAPI.Data.LoadAllHolons(HolonType.Restaurant); //  Loads all resaurants from all providers.

                    // Holochain Support
                    await SuperStar.OASISAPI.Providers.Holochain.HoloNETClient.CallZomeFunctionAsync(SuperStar.OASISAPI.Providers.Holochain.HoloNETClient.AgentPubKey, "our_world_core", "load_holons", null);

                    // IPFS Support
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.FileSystem.ReadFileAsync("");
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.FileSystem.AddFileAsync("");
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.Swarm.PeersAsync();
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.KeyChainAsync();
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.Dns.ResolveAsync("test");
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.Dag.GetAsync(new Ipfs.Cid() { Hash = "" });
                    await SuperStar.OASISAPI.Providers.IPFS.IPFSEngine.Dag.PutAsync(new Ipfs.Cid() { Hash = "" });

                // Ethereum Support
                await SuperStar.OASISAPI.Providers.Ethereum.Web3.Client.SendRequestAsync(new Nethereum.JsonRpc.Client.RpcRequest("id", "test"));
                await SuperStar.OASISAPI.Providers.Ethereum.Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync("");
                Contract contract = SuperStar.OASISAPI.Providers.Ethereum.Web3.Eth.GetContract("abi", "contractAddress");

                // EOSIO Support
                SuperStar.OASISAPI.Providers.EOSIO.ChainAPI.GetTableRows("accounts", "accounts", "users", "true", 0, 0, 1, 3);
                SuperStar.OASISAPI.Providers.EOSIO.ChainAPI.GetBlock("block");
                SuperStar.OASISAPI.Providers.EOSIO.ChainAPI.GetAccount("test.account");
                SuperStar.OASISAPI.Providers.EOSIO.ChainAPI.GetCurrencyBalance("test.account", "", "");

                    // Graph DB Support
                    await SuperStar.OASISAPI.Providers.Neo4j.GraphClient.Cypher.Merge("(a:Avatar { Id: avatar.Id })").OnCreate().Set("a = avatar").ExecuteWithoutResultsAsync(); //Insert/Update Avatar.
                    Avatar newAvatar = SuperStar.OASISAPI.Providers.Neo4j.GraphClient.Cypher.Match("(p:Avatar {Username: {nameParam}})").WithParam("nameParam", "davidellams@hotmail.com").Return(p => p.As<Avatar>()).ResultsAsync.Result.Single(); //Load Avatar.

                // Document/Object DB Support
                SuperStar.OASISAPI.Providers.MongoDB.Database.MongoDB.ListCollectionNames();
                SuperStar.OASISAPI.Providers.MongoDB.Database.MongoDB.GetCollection<Avatar>("testCollection");

                    // SEEDS Support
                    Console.WriteLine("Getting Balance for account davidsellams...");
                    string balance = SuperStar.OASISAPI.Providers.SEEDS.GetBalanceForTelosAccount("davidsellams");
                    Console.WriteLine(string.Concat("Balance: ", balance));

                    Console.WriteLine("Getting Balance for account nextgenworld...");
                    balance = SuperStar.OASISAPI.Providers.SEEDS.GetBalanceForTelosAccount("nextgenworld");
                    Console.WriteLine(string.Concat("Balance: ", balance));

                    Console.WriteLine("Getting Account for account davidsellams...");
                    Account account = SuperStar.OASISAPI.Providers.SEEDS.TelosOASIS.GetTelosAccount("davidsellams");
                    Console.WriteLine(string.Concat("Account.account_name: ", account.account_name));
                    Console.WriteLine(string.Concat("Account.created: ", account.created_datetime.ToString()));

                    Console.WriteLine("Getting Account for account nextgenworld...");
                    account = SuperStar.OASISAPI.Providers.SEEDS.TelosOASIS.GetTelosAccount("nextgenworld");
                    Console.WriteLine(string.Concat("Account.account_name: ", account.account_name));
                    Console.WriteLine(string.Concat("Account.created: ", account.created_datetime.ToString()));

                    // Check that the Telos account name is linked to the avatar and link it if it is not (PayWithSeeds will fail if it is not linked when it tries to add the karma points).
                    if (!SuperStar.LoggedInUser.ProviderKey.ContainsKey(ProviderType.TelosOASIS))
                    SuperStar.OASISAPI.Avatar.LinkProviderKeyToAvatar(SuperStar.LoggedInUser.Id, ProviderType.TelosOASIS, "davidsellams");

                    Console.WriteLine("Sending SEEDS from nextgenworld to davidsellams...");
                    OASISResult<string> payWithSeedsResult = SuperStar.OASISAPI.Providers.SEEDS.PayWithSeedsUsingTelosAccount("davidsellams", privateKey, "nextgenworld", 1, KarmaSourceType.API, "test", "test", "test", "test memo");
                    Console.WriteLine(string.Concat("Success: ", payWithSeedsResult.IsError ? "false" : "true"));

                    if (payWithSeedsResult.IsError)
                        Console.WriteLine(string.Concat("Error Message: ", payWithSeedsResult.ErrorMessage));

                    Console.WriteLine(string.Concat("Result: ", payWithSeedsResult.Result));

                    Console.WriteLine("Getting Balance for account davidsellams...");
                    balance = SuperStar.OASISAPI.Providers.SEEDS.GetBalanceForTelosAccount("davidsellams");
                    Console.WriteLine(string.Concat("Balance: ", balance));

                    Console.WriteLine("Getting Balance for account nextgenworld...");
                    balance = SuperStar.OASISAPI.Providers.SEEDS.GetBalanceForTelosAccount("nextgenworld");
                    Console.WriteLine(string.Concat("Balance: ", balance));

                    Console.WriteLine("Getting Organsiations...");
                    string orgs = SuperStar.OASISAPI.Providers.SEEDS.GetAllOrganisationsAsJSON();
                    Console.WriteLine(string.Concat("Organisations: ", orgs));

                    //Console.WriteLine("Getting nextgenworld organsiation...");
                    //string org = OASISAPI.Providers.SEEDS.GetOrganisation("nextgenworld");
                    //Console.WriteLine(string.Concat("nextgenworld org: ", org));

                    Console.WriteLine("Generating QR Code for davidsellams...");
                    string qrCode = SuperStar.OASISAPI.Providers.SEEDS.GenerateSignInQRCode("davidsellams");
                    Console.WriteLine(string.Concat("SEEDS Sign-In QRCode: ", qrCode));

                    Console.WriteLine("Sending invite to davidsellams...");
                    OASISResult<SendInviteResult> sendInviteResult = SuperStar.OASISAPI.Providers.SEEDS.SendInviteToJoinSeedsUsingTelosAccount("davidsellams", privateKey, "davidsellams", 1, 1, KarmaSourceType.API, "test", "test", "test");
                    Console.WriteLine(string.Concat("Success: ", sendInviteResult.IsError ? "false" : "true"));

                    if (sendInviteResult.IsError)
                        Console.WriteLine(string.Concat("Error Message: ", sendInviteResult.ErrorMessage));
                    else
                    {
                        Console.WriteLine(string.Concat("Invite Sent To Join SEEDS. Invite Secret: ", sendInviteResult.Result.InviteSecret, ". Transction ID: ", sendInviteResult.Result.TransactionId));

                        Console.WriteLine("Accepting invite to davidsellams...");
                        OASISResult<string> acceptInviteResult = SuperStar.OASISAPI.Providers.SEEDS.AcceptInviteToJoinSeedsUsingTelosAccount("davidsellams", sendInviteResult.Result.InviteSecret, KarmaSourceType.API, "test", "test", "test");
                        Console.WriteLine(string.Concat("Success: ", acceptInviteResult.IsError ? "false" : "true"));

                        if (acceptInviteResult.IsError)
                            Console.WriteLine(string.Concat("Error Message: ", acceptInviteResult.ErrorMessage));
                        else
                            Console.WriteLine(string.Concat("Invite Accepted To Join SEEDS. Transction ID: ", acceptInviteResult.Result));
                    }
                    // ThreeFold, AcivityPub, SOLID, Cross/Off Chain, Smart Contract Interoperability & lots more coming soon! :)

                    // END OASIS API DEMO ***********************************************************************************
                     


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
            //Console.WriteLine(string.Concat("Star Core Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
            Console.WriteLine(string.Concat("Star Core Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails));
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
            //Console.WriteLine(string.Concat("Star Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
            Console.WriteLine(string.Concat("Star Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails));
        }

        private static void OurWorld_OnZomeError(object sender, ZomeErrorEventArgs e)
        {
            //Console.WriteLine(string.Concat("Our World Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails, "HoloNETErrorDetails.Reason: ", e.HoloNETErrorDetails.Reason, "HoloNETErrorDetails.ErrorDetails: ", e.HoloNETErrorDetails.ErrorDetails));
            Console.WriteLine(string.Concat("Our World Error Occured. EndPoint: ", e.EndPoint, ". Reason: ", e.Reason, ". Error Details: ", e.ErrorDetails));
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
            ourWorld.CelestialBodyCore.LoadHolonAsync(e.Holon.Id);
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
