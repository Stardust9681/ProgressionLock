using System;
namespace ProgressionLock
{
	public static class LockPermissions
	{
		#region Utils
		private static string Concat(params string[] key)
		{
			return string.Join(".", key);
		}
		private const string Identifier = "lock";
		private const string CommandPermission = "usecommands";
		private const string EditConfigPermission = "editconfig";
		#endregion
		#region Perms
		public static readonly string UseLockCommands = Concat(Identifier, CommandPermission); //lock.usecommands
		public static readonly string EditLockConfig = Concat(Identifier, EditConfigPermission); //lock.editconfig
		#endregion
	}
}
