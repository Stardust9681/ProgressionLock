using System;
namespace ProgressionLock
{
	public static class LockPermissions
	{
		#region Utils
		private static string Concat(string end, params string[] sets)
		{
			return string.Join(".", sets, end);
		}
		private const string Identifier = "lock";
		private const string CommandPermission = "usecommands";
		private const string EditConfigPermission = "editconfig";
		#endregion
		#region Perms
		public static readonly string UseLockCommands = Concat(CommandPermission, Identifier); //lock.usecommands
		public static readonly string EditLockConfig = Concat(EditConfigPermission, Identifier); //lock.editconfig
		#endregion
	}
}
