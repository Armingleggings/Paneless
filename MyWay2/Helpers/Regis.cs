using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Paneless.Helpers
{
	class Regis
	{
		private string loggedInUser;
		private SecurityIdentifier loggedInSID;
		private string loggedInSIDStr;

		[DllImport("Wtsapi32.dll")]
		private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
		[DllImport("Wtsapi32.dll")]
		private static extern void WTSFreeMemory(IntPtr pointer);

		private enum WtsInfoClass
		{
			WTSUserName = 5,
			WTSDomainName = 7,
		}

		public Regis()
		{
			loggedInUser = GetUsername(Process.GetCurrentProcess().SessionId);
			NTAccount f = new NTAccount(loggedInUser);
			loggedInSID = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
			loggedInSIDStr = loggedInSID.ToString();
		}

		private static string GetUsername(int sessionId, bool prependDomain = true)
		{
			IntPtr buffer;
			int strLen;
			string username = "SYSTEM";
			if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1)
			{
				username = Marshal.PtrToStringAnsi(buffer);
				WTSFreeMemory(buffer);
				if (prependDomain)
				{
					if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1)
					{
						username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
						WTSFreeMemory(buffer);
					}
				}
			}
			return username;
		}

		private int GetValueInt(RegistryKey key, string val)
		{
			return Convert.ToInt32(key.GetValue(val));
		}

		// Since we run as an admin, have to write a function to get the users mydocs path
		public string MyDocsPath()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"))
				{
					return (string)explore.GetValue("Personal");
				}
			}
		}

		public bool IsFixed(string which)
		{
			if (which == "F1") return F1HelpFixed();
			if (which == "CMD") return CMDContextOn();
			if (which == "Expand") return ExpandOn();
			if (which == "FileExt") return ExtensionOn();
			if (which == "ShowFiles") return HiddenFilesVisible();	
			if (which == "UserNav") return UserNavHidden();	
			if (which == "SearchGroupBy") return SearchGroupByOff();	
			// Just in case, return false
			return false;
		}

		public void FixIt(string which)
		{
			if (which == "F1") KillF1();
			if (which == "CMD") CMDenable();
			if (which == "Expand") ExpandEnable();
			if (which == "FileExt") ExtensionEnable();
			if (which == "ShowFiles") ShowFilesEnable();
			if (which == "UserNav") UserNavDisable();
			if (which == "SearchGroupBy") SearchGroupByDisable();
		}

		public void BreakIt(string which)
		{
			if (which == "F1") RestoreF1();
			if (which == "CMD") CMDdisable();
			if (which == "Expand") ExpandDisable();
			if (which == "FileExt") ExtensionDisable();
			if (which == "ShowFiles") ShowFilesDisable();
			if (which == "UserNav") UserNavEnable();
			if (which == "SearchGroupBy") SearchGroupByEnable();
		}

		public bool F1HelpFixed()
		{
			using (var hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (var F1key = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Classes\TypeLib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0\win32"))
				{
					return F1key != null;
				}
			}
		}

		public void KillF1()
		{
			using (var hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				RegistryKey setter = hku.CreateSubKey(loggedInSIDStr + @"\SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0\win32", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "");
				setter.Close();
				setter = hku.CreateSubKey(loggedInSIDStr + @"\SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0\win64", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "");
				setter.Close();
			}
		}

		public void RestoreF1()
		{
			Registry.Users.DeleteSubKeyTree(loggedInSIDStr + @"\SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0");
		}

		public bool CMDContextOn()
		{
			using (var hku = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64))
			{
				using (var cmdKey = hku.OpenSubKey(@"\Directory\shell\OpenCmdHereAsAdmin"))
				{
					return cmdKey != null;
				}
			}
		}

		public void CMDenable()
		{
			// BASED ON THE REGISTRY HACKS By Shawn Brink
			// https://www.tenforums.com/tutorials/59686-open-command-window-here-administrator-add-windows-10-a.html

			using (var reg = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64))
			{
				RegistryKey setter;
				setter = reg.CreateSubKey(@"Directory\shell\OpenCmdHereAsAdmin", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "Open command window here as administrator");
				//setter.DeleteValue("Extended");
				setter.SetValue("Icon", "imageres.dll,-5324");
				setter.Close();

				setter = reg.CreateSubKey(@"Directory\shell\OpenCmdHereAsAdmin\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "cmd /c echo|set/p=\"%L\"|powershell -NoP -W 1 -NonI -NoL \"SaPs 'cmd' -Args '/c \"\"\"cd /d',$([char]34+$Input+[char]34),'^&^& start /b cmd.exe\"\"\"' -Verb RunAs\"");
				setter.Close();

				setter = reg.CreateSubKey(@"Directory\Background\shell\OpenCmdHereAsAdmin", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "Open command window here as administrator");
				//setter.DeleteValue("Extended");
				setter.SetValue("Icon", "imageres.dll,-5324");
				setter.Close();

				setter = reg.CreateSubKey(@"Directory\Background\shell\OpenCmdHereAsAdmin\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "cmd /c echo|set/p=\"%L\"|powershell -NoP -W 1 -NonI -NoL \"SaPs 'cmd' -Args '/c \"\"\"cd /d',$([char]34+$Input+[char]34),'^&^& start /b cmd.exe\"\"\"' -Verb RunAs\"");
				setter.Close();

				setter = reg.CreateSubKey(@"Drive\shell\OpenCmdHereAsAdmin", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "Open command window here as administrator");
				//setter.DeleteValue("Extended");
				setter.SetValue("Icon", "imageres.dll,-5324");
				setter.Close();

				setter = reg.CreateSubKey(@"Drive\shell\OpenCmdHereAsAdmin\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
				setter.SetValue("", "cmd /c echo|set/p=\"%L\"|powershell -NoP -W 1 -NonI -NoL \"SaPs 'cmd' -Args '/c \"\"\"cd /d',$([char]34+$Input+[char]34),'^&^& start /b cmd.exe\"\"\"' -Verb RunAs\"");
				setter.Close();
			}
		}

		public void CMDdisable()
		{
			Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\OpenCmdHereAsAdmin");
			Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\Background\shell\OpenCmdHereAsAdmin");
			Registry.ClassesRoot.DeleteSubKeyTree(@"Drive\shell\OpenCmdHereAsAdmin");
		}

		internal bool ExpandOn()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
				{
					if (GetValueInt(explore,"NavPaneExpandToCurrentFolder") == 1)
						return true;
					return false;
				}
			}
		}

		public void ExpandDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("NavPaneExpandToCurrentFolder", 0);
				}
			}
		}

		public void ExpandEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced",true))
				{
					explore.SetValue("NavPaneExpandToCurrentFolder", 1);
				}
			}
		}		

		internal bool ExtensionOn()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
				{
					return (GetValueInt(explore, "HideFileExt") == 0);
				}
			}
		}

		public void ExtensionDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("HideFileExt",1);
				}
			}
		}

		public void ExtensionEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("HideFileExt", 0);
				}
			}
		}


		internal bool HiddenFilesVisible()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
				{
					return (GetValueInt(explore, "Hidden") == 1);
				}
			}
		}

		public void ShowFilesEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("Hidden", 1);
				}
			}
		}

		public void ShowFilesDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("Hidden", 2);
				}
			}
		}


		internal bool UserNavHidden()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
				{
					if (GetValueInt(explore, "NavPaneShowAllFolders") == 0)
						return true;
					return false;
				}
			}
		}

		public void UserNavDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("NavPaneShowAllFolders", 0);
				}
			}
		}

		public void UserNavEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
				{
					explore.SetValue("NavPaneShowAllFolders", 1);
				}
			}
		}
		
		internal bool SearchGroupByOff()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes\{7fde1a1e-8b31-49a5-93b8-6be14cfa4943}\TopViews\{4804caf0-de08-42ec-b811-52350e94c01e}"))
				{
					if ((string)explore.GetValue("GroupBy") == "System.DateModified")
						return false;
					return true;
				}
			}
		}

		public void SearchGroupByDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes\{7fde1a1e-8b31-49a5-93b8-6be14cfa4943}\TopViews\{4804caf0-de08-42ec-b811-52350e94c01e}", true))
				{
					explore.DeleteValue("GroupBy");
				}
			}
		}

		public void SearchGroupByEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes\{7fde1a1e-8b31-49a5-93b8-6be14cfa4943}\TopViews\{4804caf0-de08-42ec-b811-52350e94c01e}", true))
				{
					explore.SetValue("GroupBy", "System.DateModified");
				}
			}
		}

	}
}
