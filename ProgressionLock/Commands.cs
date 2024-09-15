using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;

using System.Text.RegularExpressions;

namespace ProgressionLock
{
	public static class Commands
	{
		private static bool TryRegexMatch(string literal, out Entities match)
		{
			string[] names = Enum.GetNames(typeof(Entities));
			match = Entities.UnusedOrError;

			//literal = literal.Trim(' ');
			literal = new string(literal.Where(x => !char.IsWhiteSpace(x)).ToArray());
			bool isAcronym = literal.All(x => char.IsUpper(x));
			if (isAcronym)
			{
				literal = string.Join(".*", literal.ToCharArray()) + ".*";

				foreach (string name in names)
				{
					if (Regex.IsMatch(name, literal))
					{
						match = (Entities)Enum.Parse(typeof(Entities), name, false);
						return true;
					}
				}
			}
			else
			{
				literal = literal.ToLower();
                foreach (string name in names)
				{
                    if (Regex.IsMatch(name.ToLower(), literal) || literal.Equals(name.ToLower()))
					{
						match = (Entities)Enum.Parse(typeof(Entities), name, false);
						return true;
					}
				}
			}
			return false;
		}
		private static string SeparateCapitals(string toSeparate)
		{
			if (string.IsNullOrEmpty(toSeparate))
				return "";
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for (int i = 0; i < toSeparate.Length; i++)
			{
				if (char.IsUpper(toSeparate[i]))
				{
					builder.Append(' ');
				}
				builder.Append(toSeparate[i]);
			}
			return builder.ToString();
		}

		//I use const string values here in reference to the command names
		//Sue me.
		//Not really, please don't.
		public static void LockListBosses(TSPlayer caller, List<string> args)
		{
			string[] names = Enum.GetNames(typeof(Entities));
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("List of all boss and event entities: ");
			foreach (string name in names)
			{
				if (name.Equals("UnusedOrError"))
					continue;
				builder.Append($"{SeparateCapitals(name)}, ");
			}
			caller.SendMessage(builder.ToString(), ProgressionPlugin.InfoColor);
		}
		public static void LockCurrent(TSPlayer caller, List<string> args)
		{
			if (args.Count == 0)
			{
				caller.SendMessage($"Use of command 'current' expected 1 argument. Try /current \"King Slime\"", ProgressionPlugin.ErrorColour);
				return;
			}
			else if (args.Count > 1)
			{
				caller.SendMessage($"Use of command 'current' expected 1 argument. Try surrounding your query in quotation marks, \"like this\"", ProgressionPlugin.ErrorColour);
				return;
			}

			string arg = args.First().Trim(' ');
			//string[] names = Enum.GetNames(typeof(Entities));

			if (!TryRegexMatch(arg, out Entities matchValue))
			{
				caller.SendMessage(
					$"Could not identify '{args.First()}' as boss or event type.",
					ProgressionPlugin.ErrorColour);
				return;
			}

			if (!ProgressionPlugin.Config.TryGetRecent(matchValue, out LockDate date) || !date.AllowedToSpawn)
			{
				caller.SendMessage(
					$"Information not found, '{matchValue}' is unavailable.",
					ProgressionPlugin.ErrorColour);
				return;
			}

			caller.SendMessage(
				$"'{matchValue}' is currently {(date.AllowedToSpawn ? "" : "un")}available.",
				ProgressionPlugin.InfoColor);
		}
		public static void LockNext(TSPlayer caller, List<string> args)
		{
			if (args.Count == 0)
			{
				caller.SendMessage($"Use of command 'next' expected 1 argument. Try /next \"King Slime\"", ProgressionPlugin.ErrorColour);
				return;
			}
			else if (args.Count > 1)
			{
				caller.SendMessage($"Use of command 'next' expected 1 argument. Try surrounding your query in quotation marks, \"like this\"", ProgressionPlugin.ErrorColour);
				return;
			}

			string arg = args.First().Trim(' ');
			//string[] names = Enum.GetNames(typeof(Entities));

			if (!TryRegexMatch(arg, out Entities matchValue))
			{
				caller.SendMessage($"Could not identify '{args.First()}' as boss or event type.", ProgressionPlugin.ErrorColour);
				return;
			}

			if (!ProgressionPlugin.Config.TryGetNext(matchValue, out LockDate date))
			{
				caller.SendMessage($"No next entries for '{matchValue},' it will remain {(date.AllowedToSpawn?"":"un")}available.", ProgressionPlugin.ErrorColour);
				return;
			}

			int hrsSinceStart = (int)DateTime.Now.Subtract(ProgressionPlugin.Config.serverStartTime.Convert()).TotalHours;
			caller.SendMessage(
				$"'{matchValue}' will be made {(date.AllowedToSpawn ? "" : "un")}available in {date.HoursFromStart - hrsSinceStart} hours.",
				ProgressionPlugin.InfoColor);
		}
		public static void LockStartTime(TSPlayer caller, List<string> args)
		{
			if (args.Count == 0)
			{
				SimpleDateFormat dateTime = ProgressionPlugin.Config.serverStartTime;
				caller.SendMessage($"{dateTime.Month} {dateTime.Day}, {dateTime.Year} at {dateTime.Hours%12}:00 {(dateTime.Hours<13?"am":"pm")}", ProgressionPlugin.InfoColor);
			}
			else if (args.Count == 1)
			{
				if (!int.TryParse(args[0], out int timeAdjustment))
				{
					caller.SendMessage($"Could not parse '{args[0]}' as a number of hours adjustment.", ProgressionPlugin.ErrorColour);
					return;
				}
				DateTime adjustedTime = ProgressionPlugin.Config.serverStartTime.Convert().Add(new TimeSpan(hours: timeAdjustment, 0, 0));
				ProgressionPlugin.Config.serverStartTime = new SimpleDateFormat(adjustedTime);
				ProgressionPlugin.WriteConfig();
				TShock.Log.Warn($"Player {caller.Name} modified server start time to: {ProgressionPlugin.Config.serverStartTime}");
			}
			else
			{
				caller.SendMessage($"Expected at most 1 parameter. Try /serverstart 12", ProgressionPlugin.ErrorColour);
			}
		}
		public static void LockServerTime(TSPlayer caller, List<string> args)
		{
			if (args.Count != 0)
			{
				caller.SendMessage($"Command use 'servertime' expected no arguments.", ProgressionPlugin.ErrorColour);
				return;
			}
			caller.SendMessage($"This server has been operating for {(int)DateTime.Now.Subtract(ProgressionPlugin.Config.serverStartTime.Convert()).TotalHours} hours", ProgressionPlugin.InfoColor);
		}
		public static void LockDeleteNext(TSPlayer caller, List<string> args)
		{
			if (args.Count == 0)
			{
				caller.SendMessage($"Use of command 'deletenext' expected 1 argument. Try /deletextnext \"King Slime\"", ProgressionPlugin.ErrorColour);
				return;
			}
			else if (args.Count > 1)
			{
				caller.SendMessage($"Use of command 'deletenext' expected 1 argument. Try surrounding your query in quotation marks, \"like this\"", ProgressionPlugin.ErrorColour);
				return;
			}

			string arg = args.First().Trim(' ');
			string[] names = Enum.GetNames(typeof(Entities));

			if (!TryRegexMatch(arg, out Entities matchValue))
			{
				caller.SendMessage($"Could not identify '{args.First()}' as boss or event type.", ProgressionPlugin.ErrorColour);
				return;
			}

			List<LockDate> dates = ProgressionPlugin.Config.locks[matchValue].ToList();
			int hrsSinceStart = (int)DateTime.Now.Subtract(ProgressionPlugin.Config.serverStartTime.Convert()).TotalHours;
			int minHrs = -1;
			int index = -1;
			for (int i = 0; i < dates.Count; i++)
			{
				int hrs = dates[i].HoursFromStart;
				if (hrs < hrsSinceStart)
					continue;

				if (hrs < minHrs || minHrs == -1)
				{
					index = i;
					minHrs = hrs;
				}
			}

			if (index == -1)
			{
				caller.SendMessage($"Next event of '{matchValue}' not found. Disabled.", ProgressionPlugin.ErrorColour);
				return;
			}

			caller.SendMessage($"Deleted next entry for '{matchValue}'", ProgressionPlugin.InfoColor);
			dates.RemoveAt(index);
			ProgressionPlugin.Config.ReplaceWith(matchValue, dates.ToArray());
			ProgressionPlugin.WriteConfig();
		}
		public static void LockDeleteAll(TSPlayer caller, List<string> args)
		{
			if (args.Count == 0)
			{
				caller.SendMessage($"Use of command 'deleteall' expected 1 argument. Try /deletextnext \"King Slime\"", ProgressionPlugin.ErrorColour);
				return;
			}
			else if (args.Count > 1)
			{
				caller.SendMessage($"Use of command 'deleteall' expected 1 argument. Try surrounding your query in quotation marks, \"like this\"", ProgressionPlugin.ErrorColour);
				return;
			}

			string arg = args.First().Trim(' ');

			if (!TryRegexMatch(arg, out Entities matchValue))
			{
				caller.SendMessage($"Could not identify '{args.First()}' as boss or event type.", ProgressionPlugin.ErrorColour);
				return;
			}

			caller.SendMessage($"Deleted all entries for '{matchValue}'", ProgressionPlugin.InfoColor);
			ProgressionPlugin.Config.ReplaceWith(matchValue, Array.Empty<LockDate>());
			ProgressionPlugin.WriteConfig();
			TShock.Log.Warn($"Player '{caller.Name}' removed all entries for '{matchValue}'");
		}
		public static void LockAddRule(TSPlayer caller, List<string> args)
		{
			if (args.Count < 3)
			{
				caller.SendMessage($"Use of command 'addrule' expected 3 arguments. Try /addrule \"King Slime\" 168 True", ProgressionPlugin.ErrorColour);
				return;
			}
			else if (args.Count > 3)
			{
				caller.SendMessage($"Use of command 'addrule' expected 3 arguments. Try surrounding your query in quotation marks, \"like this\"", ProgressionPlugin.ErrorColour);
				return;
			}

			bool absoluteTime = args[1].StartsWith("@");
			if (absoluteTime)
			{
				args[1] = args[1][1..];
			}
			if (!int.TryParse(args[1], out int hrsFromNow))
			{
				caller.SendMessage($"Could not interpret '{args[1]}' as a number of hours. Try /addrule \"King Slime\" 168 True", ProgressionPlugin.ErrorColour);
				return;
			}

			if (!bool.TryParse(args[2].ToLower(), out bool enabled))
			{
				caller.SendMessage($"Could not interpret '{args[2]}' as True|False. Try /addrule \"King Slime\" 168 True", ProgressionPlugin.ErrorColour);
				return;
			}

			string arg = args.First().Trim(' ');
			string[] names = Enum.GetNames(typeof(Entities));

			if (!TryRegexMatch(arg, out Entities matchValue))
			{
				caller.SendMessage($"Could not identify '{args.First()}' as boss or event type.", ProgressionPlugin.ErrorColour);
				return;
			}

			List<LockDate> dates = ProgressionPlugin.Config.locks[matchValue].ToList();

			int newHrs = -1;
			if (absoluteTime)
			{
				newHrs = hrsFromNow;
			}
			else
			{
				int hrsSinceStart = (int)DateTime.Now.Subtract(ProgressionPlugin.Config.serverStartTime.Convert()).TotalHours;
				newHrs = hrsSinceStart + hrsFromNow;
			}

			caller.SendMessage($"Added entry for '{matchValue}' at {newHrs} from start, to be {(enabled ? "" : "un")}available.", ProgressionPlugin.InfoColor);
			dates.Add(new LockDate() { AllowedToSpawn = enabled, HoursFromStart = newHrs });
			ProgressionPlugin.Config.ReplaceWith(matchValue, dates.ToArray());
			ProgressionPlugin.WriteConfig();
			TShock.Log.Warn($"Player '{caller.Name}' added entry for '{matchValue}' at {newHrs} (Enabled: {enabled})");
		}
		public static void LockRefreshConfig(TSPlayer caller, List<string> args)
		{
			bool isConfirmed = args.Last().Trim(' ').ToLower().Equals("confirm");
			if (!isConfirmed)
			{
				caller.SendMessage($"To confirm running this, please re-enter command, suffixed with \"confirm\"", ProgressionPlugin.ErrorColour);
				return;
			}
			bool shouldSkipResetCheck = isConfirmed && args.Count == 1;
			byte resetCheck = 2;
			if (!shouldSkipResetCheck)
			{
				//[Time|Lock|Both]
				switch (args[0].Trim(' ').ToLower())
				{
					case "time":
						resetCheck = 0;
						break;
					case "lock":
						resetCheck = 1;
						break;
					case "both":
						resetCheck = 2;
						break;
					default:
						caller.SendMessage($"Could not interpret '{args[0]}' as \"time,\" \"lock,\" or \"both.\" Please try again.", ProgressionPlugin.ErrorColour);
						return;
				}
			}
			ProgressionPlugin.ResetConfig(resetCheck);
			caller.SendMessage($"Reset server config", ProgressionPlugin.InfoColor);
			TShock.Log.Warn($"Player '{caller.Name}' fully reset config.");
		}

		public static void LockCommandList(TSPlayer caller, List<string> args)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder("List of commands for \"/lock:\" ");
			foreach (SubCommand cmd in CommandList.Where(x =>
			{
				foreach (string permission in x.Permissions)
					if (!caller.HasPermission(permission))
						return false;
				return true;
			}))
			{
				builder.Append($"{cmd.CommandNames.First()}, ");
			}
			caller.SendMessage(builder.ToString(), ProgressionPlugin.InfoColor);
		}
		public static void LockHelp(TSPlayer caller, List<string> args)
		{
			if (args is null || args.Count != 1)
				return;
			string commandName = args.FirstOrDefault()!.ToLower().Trim('/', ' ');
			int cmdIndex = CommandList.FindIndex(x => x.CommandNames.Contains(commandName));
			if (cmdIndex == -1)
			{
				caller.SendMessage($"Invalid command or syntax, '{commandName} {string.Join(" ", args.ToArray())}'", ProgressionPlugin.ErrorColour);
				return;
			}
			foreach (string permission in CommandList[cmdIndex].Permissions)
				if (!caller.HasPermission(permission))
				{
					caller.SendMessage($"Insufficient permissions to access command '{commandName}'", ProgressionPlugin.ErrorColour);
					return;
				}

			caller.SendMessage($"/lock help '{commandName}':" +
				$"\nSyntax: {CommandList[cmdIndex].Syntax}" +
				$"\nDescription: {CommandList[cmdIndex].HelpDesc}", ProgressionPlugin.InfoColor);
		}
		public static void LockSyntax(TSPlayer caller, List<string> args)
		{
			caller.SendMessage($"Command Syntax Help:" +
				$"\nAnything in <these> is a necessary command argument. You must include it in the command." +
				$"\nAnything in [these] is an optional command argument. You may opt not to include it." +
				$"\n\"Quotation marks\" are used to ensure arguments passed in are read as intended. Any parameters with spaces should be surrounded by quotation marks." +
				$"\nFor this plugin (any commands branching off of /lock), you may opt to define bosses and events by abbreviation (ex: King Slime = KS)" +
				$"\n    You may also shorten boss names (ex: Destroyer = Dest), though take caution for bosses with shared names (ex: Queen Bee and Queen Slime)", ProgressionPlugin.InfoColor);
		}

		//I don't feel like writing new string[] { a, .. } I'm lazy
		private static string[] Compose(params string[] args) => args;
		static Commands()
		{
			CommandList.Add(new SubCommand(LockHelp, "help", "sos")
			{
				Syntax = "/lock help <Command>",
				HelpDesc = "Shows this information for a named command.",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockSyntax, "syntax", "helpsyntax")
			{
				Syntax = "/lock syntax",
				HelpDesc = "Shows information about how to read command syntax for this plugin.",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockCommandList, "commandlist", "cmdlist", "list")
			{
				Syntax = "/lock commandlist",
				HelpDesc = "Shows a list of all commands (through this plugin) that are available to you.",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockListBosses, "bosslist", "eventlist", "bosses", "events")
			{
				Syntax = "/lock bosslist",
				HelpDesc = "Shows a list of all bosses and events that can be limited",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockCurrent, "current", "c")
			{
				Syntax = "/lock current \"<Name of Boss or Event>\"", //See how it should be used
				HelpDesc = "Shows the current availability of named boss or event.", //The description for what it does
				Permissions = Compose(LockPermissions.UseLockCommands), //See permission list here
			});
			CommandList.Add(new SubCommand(LockNext, "next", "n")
			{
				Syntax = "/lock next \"<Name of Boss or Event>\"",
				HelpDesc = "Shows when the next rule or entry for a named boss or event.",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockServerTime, "servertime", "time")
			{
				Syntax = "/lock servertime",
				HelpDesc = "Shows time since server start.",
				Permissions = Compose(LockPermissions.UseLockCommands),
			});
			CommandList.Add(new SubCommand(LockStartTime, "starttime", "start")
			{
				Syntax = "/lock starttime [Time Adjustment (in Hours)]",
				HelpDesc = "Shows time and date when server was started. When a duration is supplied, shifts start time by that many hours.",
				Permissions = Compose(LockPermissions.UseLockCommands, LockPermissions.EditLockConfig),
			});
			CommandList.Add(new SubCommand(LockAddRule, "addrule", "add", "ar")
			{
				Syntax = "/lock addrule \"<Name of Boss or Event>\" [@]<Hours> <True|False>",
				HelpDesc = "Adds new entry for named boss or event. Duration by default assumes 'hours from now,' prefix with '@' for time 'relative to server start.'",
				Permissions = Compose(LockPermissions.UseLockCommands, LockPermissions.EditLockConfig),
			});
			CommandList.Add(new SubCommand(LockDeleteNext, "deletenext", "dn")
			{
				Syntax = "/lock deletenext \"<Name of Boss or Event>\"",
				HelpDesc = "Removes the next-up entry for a named boss or event.",
				Permissions = Compose(LockPermissions.UseLockCommands, LockPermissions.EditLockConfig),
			});
			CommandList.Add(new SubCommand(LockDeleteAll, "deleteall", "da")
			{
				Syntax = "/lock deleteall \"<Name of Boss or Event>\"",
				HelpDesc = "Removes all entries for a named boss or event.",
				Permissions = Compose(LockPermissions.UseLockCommands, LockPermissions.EditLockConfig),
			});
			CommandList.Add(new SubCommand(LockRefreshConfig, "resetconfig")
			{
				Syntax = "/lock resetconfig [Time|Lock|Both]",
				HelpDesc = "Resets lock config to its default state. You may choose to reset server start time to current time, to reset boss/progression entries to their defaults, or to reset both.",
				Permissions = Compose(LockPermissions.UseLockCommands, LockPermissions.EditLockConfig),
			});
		}
		private delegate void CommandFunc(TSPlayer caller, List<string> args); //I might make this bool or enum return type, to show successful run or whatever
		private class SubCommand
		{
			public SubCommand(CommandFunc function, params string[] names)
			{
				Func = function;
				for (int i = 0; i < names.Length; i++)
					names[i] = names[i].ToLower(); //No commands in tShock care about capitalisation anyway, front loading optimisations this way
				CommandNames = names;
			}
			public bool Call(TSPlayer caller, List<string> args)
			{
				bool playerHasPermission = true;
				foreach (string permission in Permissions)
					if (!caller.HasPermission(permission))
					{
						playerHasPermission = false;
					}
				//Bypass for admins to be allowed to use this plugin's commands
				if (caller.HasPermission(TShockAPI.Permissions.user))
					playerHasPermission = true;
				if (!playerHasPermission)
				{
					caller.SendMessage($"Insufficient permissions to invoke command /{CommandNames.First()}", ProgressionPlugin.ErrorColour);
					return false;
				}
				Func.Invoke(caller, args);
				return true;
			}
			private readonly CommandFunc Func;
			public IReadOnlyCollection<string> CommandNames //Don't need write access to command names after they've been established
			{
				get;
				private init; //Use constructor
			}
			public string HelpDesc
			{
				get;
				init;
			}
			public string Syntax
			{
				get;
				init;
			}
			public string[] Permissions
			{
				get;
				init;
			}
		}
		private static readonly List<SubCommand> CommandList = new List<SubCommand>();
		public static void LockCommands(CommandArgs args)
		{
			List<string> parameters = args.Parameters;
			TSPlayer caller = args.Player;
			if (parameters.Count == 0)
			{
				LockCommandList(caller, parameters);
				LockHelp(caller, new List<string>() { "help" });
				LockSyntax(caller, parameters);
				return;
			}
			string commandName = parameters.First().ToLower();
			parameters.RemoveAt(0);
			int cmdIndex = CommandList.FindIndex(x => x.CommandNames.Contains(commandName));
			if(cmdIndex == -1)
			{
				caller.SendMessage($"Invalid command or syntax, '{commandName} {string.Join(" ", parameters.ToArray())}'", ProgressionPlugin.ErrorColour);
				return;
			}
			//if (parameters.Any(x => x.Length > 40))
			//{
			//	caller.SendMessage($"Excessively long input, command rejected.", ProgressionPlugin.ErrorColour);
			//	return;
			//}
			CommandList[cmdIndex].Call(caller, parameters);
		}
	}
}