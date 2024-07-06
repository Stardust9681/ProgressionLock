# Progression Lock
TShock plugin that allows you to impose time restraints on when bosses and events can spawn.

## Config
**The config file will be generated automatially when this is first run in the server.**
You may opt to keep defaults, or you may instead prefer to change this afterwards. In that case, you have two options:
1) You may edit the config file directly. I've made this as accessible as possible, but I'd still only recommend this if you know what you are doing. Furthermore, this requires a server restart for the changes to take effect.
2) You can edit the config using this plugin's commands! These will be much easier for most users, they come with help on syntax, formatting, and use, and allow for quick changes on the fly.

This plugin will default to **DISABLING** boss and event spawns, until they have been explicitly enabled through the config (and, then, only when the time threshold for the given entry has been reached).
You may add **MULTIPLE** entries for a given boss or event, in which case, the most recent will be used to determine whether something can be spawned.

## Permissions
| Permission Name   | Description   |
| :-  | :-
| uselockcommands | Allows a group or user to see and use this plugin's commands. |
| editlockconfig | Grants a group or user access to the commands necessary to change the configuration. |

## Commands
This plugin's commands are all contained within an overarching "/lock" command.
Following is a list of all commands, their required permissions, and their descriptions.

| Command Name | Permissions Required | Description |
| :- | :- | :-
| help | uselockcommands | Provides syntax and help text for a given command. |
| commandlist | uselockcommands | Offers a list of commands available to the caller. |
| syntax | uselockcommands | Gives an overaching description of the format used for commands. |
| bosslist | uselockcommands | Shows a list of all bosses and events by name. |
| current | uselockcommands | Yields the current availability of a named boss or event. |
| next | uselockcommands | Tells the next entry for a named boss or event. |
| servertime | uselockcommands | Returns the number of hours since server start. |
| starttime | uselockcommands, editlockconfig | Shows start time and date for plugin. When provided a time (in hours), adjusts it by that many hours. |
| addrule | uselockcommands, editlockconfig | Adds a new entry for a boss or event. You may have multiple entries, whereby the most recent takes presidence. |
| deletenext | uselockcommands, editlockconfig | Removes the next entry for a boss or event. If this leaves a boss/event with no entries, it cannot be spawned. |
| deleteall | uselockcommands, editlockconfig | Removes all entries for a boss or event. Any bosses or events with no entries will be unable to be spawned. |
| resetconfig | uselockcommands, editlockconfig | Resets lock config to its default state. |