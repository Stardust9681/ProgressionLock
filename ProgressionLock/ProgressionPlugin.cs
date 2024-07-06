using Terraria;
using TerrariaApi;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

using System.IO.Streams;

using Microsoft.Xna.Framework;

namespace ProgressionLock
{
	[ApiVersion(2, 1)]
	public class ProgressionPlugin : TerrariaPlugin
	{
		public override string Name => "Progressionlock";
		public override string Author => "Stardust";
		public override string Description => "Provides a means by which to prevent bosses and events from spawning";
		public override Version Version => new Version(1, 1);

		private static ProgressionLockerConfig config;
		public static ProgressionLockerConfig Config
		{
			get => config!;
			private set => config = value;
		}
		private static LockerConfigFile configFile;

		private static void RefreshConfig()
		{
			configFile = new LockerConfigFile();
			if (!configFile.TryRead(LockerConfigFile.FilePath, out ProgressionLockerConfig newConfig))
			{
				Config = newConfig;
				Directory.CreateDirectory(LockerConfigFile.DirectoryPath);
				Config.locks = GetDefaults();
			}
			configFile.Settings = Config;
			configFile.Write(LockerConfigFile.FilePath);
			configFile.TryRead(LockerConfigFile.FilePath, out config);
		}

		public static void WriteConfig()
		{
			configFile.Settings = Config;
			configFile.Write(LockerConfigFile.FilePath);
		}
		public static bool TryReadConfig()
		{
			configFile = new LockerConfigFile();
			if (!configFile.TryRead(LockerConfigFile.FilePath, out config))
			{
                Console.WriteLine("Failed to read Config file: reloading.");
                Directory.CreateDirectory(LockerConfigFile.DirectoryPath);
				Config = new ProgressionLockerConfig()
				{
					serverStartTime = new SimpleDateFormat(DateTime.Now),
					locks = GetDefaults(),
				};
				return false;
			}
			return true;
		}

		public static Color ErrorColour => Color.IndianRed;
		public static Color InfoColor => Color.LightSeaGreen;

		private Dictionary<Entities, bool> ignoreCheck = new Dictionary<Entities, bool>()
		{
			[Entities.SlimeRain] = false,
			[Entities.BloodMoon] = false,
			[Entities.SolarEclipse] = false,
			[Entities.PumpkinMoon] = false,
			[Entities.FrostMoon] = false,
			[Entities.GoblinArmy] = false,
			[Entities.PirateInvastion] = false,
			[Entities.FrostLegion] = false,
			[Entities.MartianMadness] = false,
			[Entities.MoonLordCountdown] = false,
			[Entities.OldOnesArmy] = false,
		};

		public ProgressionPlugin(Main game) : base(game)
		{
		}

		private static Dictionary<Entities, LockDate[]> GetDefaults()
		{
			return new Dictionary<Entities, LockDate[]>()
			{
				[Entities.KingSlime] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 0, AllowedToSpawn = true } },
				[Entities.SlimeRain] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 0, AllowedToSpawn = true } },
				[Entities.EyeOfCthulhu] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 0, AllowedToSpawn = true } },

				[Entities.BrainOfCthulhu] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 1, AllowedToSpawn = true } },
				[Entities.EaterOfWorlds] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 1, AllowedToSpawn = true } },
				[Entities.GoblinArmy] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 1, AllowedToSpawn = true } },

				[Entities.QueenBee] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 2, AllowedToSpawn = true } },
				[Entities.Deerclops] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 2, AllowedToSpawn = true } },
				[Entities.Skeletron] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 2, AllowedToSpawn = true } },

				[Entities.WallOfFlesh] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 3, AllowedToSpawn = true } },
				[Entities.PirateInvastion] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 3, AllowedToSpawn = true } },

				[Entities.Twins] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 4, AllowedToSpawn = true } },
				[Entities.Destroyer] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 4, AllowedToSpawn = true } },
				[Entities.SkeletronPrime] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 4, AllowedToSpawn = true } },

				[Entities.QueenSlime] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 5, AllowedToSpawn = true } },
				[Entities.DukeFishron] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 5, AllowedToSpawn = true } },

				[Entities.Plantera] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 6, AllowedToSpawn = true } },
				[Entities.FrostMoon] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 6, AllowedToSpawn = true } },
				[Entities.PumpkinMoon] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 6, AllowedToSpawn = true } },

				[Entities.EmpressOfLight] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 7, AllowedToSpawn = true } },
				[Entities.Golem] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 7, AllowedToSpawn = true } },

				[Entities.MartianMadness] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 8, AllowedToSpawn = true } },

				[Entities.LunaticCultist] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 9, AllowedToSpawn = true } },
				[Entities.MoonLord] = new LockDate[] { new LockDate() { HoursFromStart = 168 * 9, AllowedToSpawn = true } },

				[Entities.BloodMoon] = new LockDate[] { new LockDate() { HoursFromStart = 0, AllowedToSpawn = true } },
				[Entities.SolarEclipse] = new LockDate[] { new LockDate() { HoursFromStart = 0, AllowedToSpawn = true } },
				[Entities.OldOnesArmy] = new LockDate[] { new LockDate() { HoursFromStart = 0, AllowedToSpawn = true } },
				[Entities.MoonLordCountdown] = new LockDate[] { new LockDate() { HoursFromStart = 0, AllowedToSpawn = true } },
			};
		}
		public static void ResetConfig(byte reset)
		{
			switch (reset)
			{
				case 0: //Time
					config.serverStartTime = new SimpleDateFormat(DateTime.Now);
					break;
				case 1: //Lock
					config.locks = GetDefaults();
					break;
				case 2: //Both
					config.serverStartTime = new SimpleDateFormat(DateTime.Now);
					config.locks = GetDefaults();
					break;
				default:
					return;
			}
			configFile.Settings = config;
			configFile.Write(LockerConfigFile.FilePath);
			configFile.TryRead(LockerConfigFile.FilePath, out config);
		}

		public override void Initialize()
		{
			configFile = new LockerConfigFile();
			if (!configFile.TryRead(LockerConfigFile.FilePath, out config))
			{
				if (config is default(ProgressionLockerConfig))
				{
					Directory.CreateDirectory(LockerConfigFile.DirectoryPath);
					configFile.Settings = new ProgressionLockerConfig()
					{
						serverStartTime = new SimpleDateFormat(DateTime.Now),
						locks = GetDefaults(),
					};
					configFile.Write(LockerConfigFile.FilePath);
					configFile.TryRead(LockerConfigFile.FilePath, out config);
				}
			}
			
			//I don't want all the commands for this plugin cluttering the THREE PAGES of commands that already exist
			TShockAPI.Commands.ChatCommands.Add(new Command(LockPermissions.UseLockCommands, Commands.LockCommands, "lock", "proglock")
			{
				HelpText = "ProgressionLock plugin command system. Try \"/lock commandlist\" to see all available commands!",
			});

			//ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			GeneralHooks.ReloadEvent += OnReload;
			ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
			ServerApi.Hooks.NpcSpawn.Register(this, OnSpawnNPC);
		}

		private void OnSpawnNPC(NpcSpawnEventArgs args)
		{
			NPC npc = Main.npc[args.NpcId];

			Entities type = EntityTypeExtensions.FromID(npc.type);
			if (type is Entities.UnusedOrError)
				return;

			if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
			{
				string npcName = npc.FullName;
				npc.active = false;
				npc.type = 0;
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npc.whoAmI);
				TSPlayer.All.SendMessage($"{npc.FullName} has been disabled.", Color.MediumVioletRed);
			}
		}

		private void OnUpdate(EventArgs args)
		{
			Entities type;

			if (Main.slimeRain)
			{
				type = Entities.SlimeRain;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						TSPlayer.Server.SetBloodMoon(false);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.SlimeRain] = false;

			if (Main.bloodMoon)
			{
				type = Entities.BloodMoon;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						TSPlayer.Server.SetBloodMoon(false);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.BloodMoon] = false;

			if (Main.snowMoon)
			{
				type = Entities.FrostMoon;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						TSPlayer.Server.SetFrostMoon(false);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.FrostMoon] = false;

			if (Main.pumpkinMoon)
			{
				type = Entities.PumpkinMoon;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						TSPlayer.Server.SetPumpkinMoon(false);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.PumpkinMoon] = false;

			if (Main.eclipse)
			{
				type = Entities.SolarEclipse;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						TSPlayer.Server.SetEclipse(false);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.SolarEclipse] = false;

			if (NPC.MoonLordCountdown > 0)
			{
				type = Entities.MoonLordCountdown;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						NPC.MoonLordCountdown = 0;
						NPC.LunarApocalypseIsUp = false;
						NPC.TowerActiveNebula = false;
						NPC.TowerActiveSolar = false;
						NPC.TowerActiveStardust = false;
						NPC.TowerActiveVortex = false;
						TSPlayer.All.SendData(PacketTypes.MoonLordCountdown);
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.MoonLordCountdown] = false;

			if (Terraria.GameContent.Events.DD2Event.Ongoing)
			{
				type = Entities.OldOnesArmy;
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						Terraria.GameContent.Events.DD2Event.StopInvasion();
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
			}
			else
				ignoreCheck[Entities.OldOnesArmy] = false;

			if (Main.invasionType > 0)
			{
				switch (Main.invasionType)
				{
					case 1:
						type = Entities.GoblinArmy;
						break;
					case 2:
						type = Entities.FrostLegion;
						break;
					case 3:
						type = Entities.PirateInvastion;
						break;
					case 4:
						type = Entities.PumpkinMoon;
						break;
					case 5:
						type = Entities.FrostMoon;
						break;
					case 6:
						type = Entities.SolarEclipse;
						break;
					case 7:
						type = Entities.MartianMadness;
						break;
					default:
						return;
				}
				if (!ignoreCheck[type])
				{
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						Main.invasionType = 0;
						Main.invasionSize = 0;
						TSPlayer.All.SendMessage($"Event \'{type}\' has been disabled.", Color.MediumVioletRed);
					}
					else
						ignoreCheck[type] = true;
				}
				else
					ignoreCheck[type] = false;

				TSPlayer.All.SendData(PacketTypes.WorldInfo);
			}
		}

		private void OnReload(ReloadEventArgs e)
		{
			try
			{
				if (configFile is null)
					configFile = new LockerConfigFile();
				if (!configFile.TryRead(LockerConfigFile.FilePath, out config))
				{
					Directory.CreateDirectory(LockerConfigFile.DirectoryPath);
					configFile.Write(LockerConfigFile.FilePath);
					configFile.TryRead(LockerConfigFile.FilePath, out config);
				}
			}
			catch (Exception x)
			{
				TShock.Log.Error($"Failed to reload ProgressionLock config: [ERROR] {x.Message}");
			}
		}
		private void OnGetData(GetDataEventArgs args)
		{
			if (!(args.MsgID.Equals(PacketTypes.SpawnBossorInvasion)
							|| args.MsgID.Equals(PacketTypes.WorldInfo)
							|| args.MsgID.Equals(PacketTypes.PlayerUpdate)
							|| args.MsgID.Equals(PacketTypes.NpcUpdate)
							|| args.MsgID.Equals(PacketTypes.SetMiscEventValues)))
				return;
			//Terraria.ID.MessageID.SetMiscEventValues
			TSPlayer player = TShock.Players[args.Msg.whoAmI];
			if (player is null || !player.ConnectionAlive)
			{
				return;
			}
			if (player.RequiresPassword)
			{
				return;
			}
			if (player.IsBouncerThrottled())
			{
				return;
			}

			if (Main.slimeRain && !ignoreCheck[Entities.SlimeRain])
			{
				if (!config.TryGetRecent(Entities.SlimeRain, out LockDate date) || !date.AllowedToSpawn)
				{
					Main.StopSlimeRain(false);
					TSPlayer.All.SendData(PacketTypes.WorldInfo);
					TSPlayer.All.SendMessage("Slime Rain has been disabled.", Color.MediumVioletRed);
					args.Handled = true;
					return;
				}
				ignoreCheck[Entities.SlimeRain] = true;
			}
			else if (ignoreCheck[Entities.SlimeRain])
				ignoreCheck[Entities.SlimeRain] = false;

			if (args.MsgID is PacketTypes.PlayerUpdate)
			{
				return;
			}
			if (!player.HasPermission(Permissions.summonboss))
			{
				return;
			}

			using (MemoryStream stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1))
			{
				GetDataHandlerArgs handlerArgs = new GetDataHandlerArgs(player, stream);

				short plr = handlerArgs.Data.ReadInt16();
				short thingType = handlerArgs.Data.ReadInt16();

				bool isBoss = (thingType > 0 || thingType == -16) && thingType < Terraria.ID.NPCID.Count && Terraria.ID.NPCID.Sets.MPAllowedEnemies[thingType];
				bool isEvent = thingType is < 0 and > -12;
				//TShock.Log.ConsoleInfo($"ThingType:{thingType}, IsBoss:{isBoss}, IsEvent:{isEvent}");
				if (isBoss)
				{
					Entities type = EntityTypeExtensions.FromID(thingType);
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						handlerArgs.Player.SendMessage("That boss summon has been disabled!", Color.MediumVioletRed);
						args.Handled = true;
					}
					return;
				}
				if (isEvent)
				{
					Entities type = EntityTypeExtensions.FromID(thingType);
					if (!config.TryGetRecent(type, out LockDate date) || !date.AllowedToSpawn)
					{
						bool sendData = false;
						void DisableInvasion()
						{
							Main.invasionType = 0;
							Main.invasionSize = 0;
							sendData = true;
						}
						switch (type)
						{
							case Entities.GoblinArmy:
							case Entities.FrostLegion:
							case Entities.PirateInvastion:
							case Entities.MartianMadness:
								sendData = true;
								Main.invasionType = 0;
								Main.invasionSize = 0;
								break;
							case Entities.SolarEclipse:
								TSPlayer.Server.SetEclipse(false);
								DisableInvasion();
								break;
							case Entities.BloodMoon:
								TSPlayer.Server.SetBloodMoon(false);
								DisableInvasion();
								break;
							case Entities.PumpkinMoon:
								TSPlayer.Server.SetPumpkinMoon(false);
								DisableInvasion();
								break;
							case Entities.FrostMoon:
								TSPlayer.Server.SetFrostMoon(false);
								DisableInvasion();
								break;
							case Entities.MoonLordCountdown:
								NPC.MoonLordCountdown = 0;
								NPC.TowerActiveNebula = false;
								NPC.TowerActiveSolar = false;
								NPC.TowerActiveStardust = false;
								NPC.TowerActiveVortex = false;
								break;
						}
						if (sendData)
						{
							TSPlayer.All.SendData(PacketTypes.WorldInfo);
						}
						handlerArgs.Player.SendMessage("That event summon has been disabled!", Color.MediumVioletRed);
						args.Handled = true;
					}
					return;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				GeneralHooks.ReloadEvent -= OnReload;
				ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
			}
			base.Dispose(disposing);
		}
	}
}