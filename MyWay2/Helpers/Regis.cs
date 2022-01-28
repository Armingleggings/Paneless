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

		// Helper because apparently it won't do this on its own
		public RegistryKey openCreate(RegistryKey baseKey, string path)
		{
			RegistryKey test = baseKey.OpenSubKey(path);

			var reg = baseKey.OpenSubKey(path, true);
			if (reg == null)
			{
				reg = baseKey.CreateSubKey(path);
			}
			return reg;
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
			if (which == "DownloadGroupBy") return DownloadGroupByOff();	
			if (which == "ExplorerRibbon") return ExplorerRibbonOff();
			if (which == "Hibernate") return HibernateOptionOn();
			if (which == "NumLBoot") return NumLockOnBootOn();
			if (which == "MenuAll") return FullRightClickMenu();
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
			if (which == "DownloadGroupBy") DownloadGroupByDisable();
			if (which == "ExplorerRibbon") ExplorerRibbonEnable();
			if (which == "Hibernate") HibernateOptionEnable();
			if (which == "NumLBoot") NumLockOnBootEnable();
			if (which == "MenuAll") EnableFullRightClickMenu();
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
			if (which == "DownloadGroupBy") DownloadGroupByEnable();
			if (which == "ExplorerRibbon") ExplorerRibbonDisable();
			if (which == "Hibernate") HibernateOptionDisable();
			if (which == "NumLBoot") NumLockOnBootDisable();
			if (which == "MenuAll") DisableFullRightClickMenu();

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
					return GetValueInt(explore, "Hidden") == 1;
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
		
		internal bool DownloadGroupByOff()
		{
			// Using "using" to handle auto-close when it leaves this code block (so we don't have to manually close before returning)
			using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = localMachine.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885A186E-A440-4ADA-812B-DB871B942259}")){
					if (explore == null || GetValueInt(explore, "GroupView") != 0)
						return false;
				}				
				using (RegistryKey explore = localMachine.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\AllFolders\ComDlg\{885A186E-A440-4ADA-812B-DB871B942259}")){
					if (explore == null || GetValueInt(explore, "GroupView") != 0)
						return false;
				}				
				using (RegistryKey explore = localMachine.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\AllFolders\ComDlgLegacy\{885A186E-A440-4ADA-812B-DB871B942259}")){
					if (explore == null || GetValueInt(explore, "GroupView") != 0)
						return false;
				}
				return true;
			}
		}

		public void DownloadGroupByEnable()
		{
			RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			// Adding false to this command says not to throw an exception if the key doesn't exist - just ignore
			localMachine.DeleteSubKeyTree(@"Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885A186E-A440-4ADA-812B-DB871B942259}", false);
			localMachine.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\Shell\Bags\AllFolders\ComDlg\{885a186e-a440-4ada-812b-db871b942259}", false);
			localMachine.DeleteSubKeyTree(@"Software\Microsoft\Windows\Shell\Bags\AllFolders\ComDlgLegacy\{885A186E-A440-4ADA-812B-DB871B942259}", false);
			localMachine.Close();
		}

		public void DownloadGroupByDisable()
		{
			RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			
			RegistryKey explore = openCreate(localMachine,@"SOFTWARE\Microsoft\Windows\Shell\Bags\AllFolders\ComDlg\{885a186e-a440-4ada-812b-db871b942259}");
			explore.SetValue("", "Downloads");
			explore.SetValue("GroupView", "0");
			explore.SetValue("Mode", "4");
			explore.Close();

			explore = openCreate(localMachine, @"SOFTWARE\Microsoft\Windows\Shell\Bags\AllFolders\ComDlgLegacy\{885a186e-a440-4ada-812b-db871b942259}");
			explore.SetValue("", "Downloads");
			explore.SetValue("GroupView", "0");
			explore.SetValue("Mode", "4");
			explore.Close();

			explore = openCreate(localMachine, @"SOFTWARE\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885a186e-a440-4ada-812b-db871b942259}");
			explore.SetValue("", "Downloads");
			explore.SetValue("GroupView", "0");
			explore.SetValue("Mode", "4");
			explore.Close();

			localMachine.Close();
		}

		internal bool ExplorerRibbonOff()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked\"))
				{
					if (explore == null || (string) explore.GetValue("{e2bf9676-5f8f-435c-97eb-11607a5bedf7}") == "")	
						return true;
					return false;
				}
			}
		}

		public void ExplorerRibbonDisable()
		{
			RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			// Adding false to this command says not to throw an exception if the key doesn't exist - just ignore
			localMachine.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked\{e2bf9676-5f8f-435c-97eb-11607a5bedf7}", false);
			localMachine.Close();
		}

		public void ExplorerRibbonEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked\", true))
				{
					explore.SetValue("{e2bf9676-5f8f-435c-97eb-11607a5bedf7}", "");
				}
			}
		}

		internal bool HibernateOptionOn()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings"))
				{
					if (explore != null && GetValueInt(explore, "ShowHibernateOption") == 1)
						return true;
					return false;
				}
			}
		}

		public void HibernateOptionDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings", true))
				{
					explore.SetValue("ShowHibernateOption", 0);
				}
			}
		}

		public void HibernateOptionEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				// Make sure Hibernate is enabled or showing it on the shutdown menu won't matter much.
				using (RegistryKey explore = hku.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power", true))
				{
					explore.SetValue("HibernateEnabled", 1);
				}
				using (RegistryKey explore = hku.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings", true))
				{
					explore.SetValue("ShowHibernateOption", 1);
				}
			}
		}

		public bool NumLockOn()
		{
			return false;
		}

		internal bool NumLockOnBootOn()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\Control Panel\Keyboard"))
				{
					if (explore != null && GetValueInt(explore, "InitialKeyboardIndicators") == 2)
						return true;
					return false;
				}
			}
		}

		public void NumLockOnBootDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.CreateSubKey(loggedInSIDStr + @"\Control Panel\Keyboard"))
				{
					explore.SetValue("InitialKeyboardIndicators", "0");
				}
			}
		}

		public void NumLockOnBootEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (RegistryKey explore = hku.CreateSubKey(loggedInSIDStr + @"\Control Panel\Keyboard"))
				{
					explore.SetValue("InitialKeyboardIndicators", "2");
				}
			}
		}



		internal bool FullRightClickMenu()
		{
			using (var hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (var newMenuBlocker = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"))
				{
					// If it exists, the right click menu is restored!
					return newMenuBlocker != null;

				}
			}
		}

		public void EnableFullRightClickMenu()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				RegistryKey newMenuBlocker = openCreate(hku,loggedInSIDStr + @"\SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
				// It can't even be "(Default) "" " or "(Default) value not set". It has to be "(Default) with literally nothing in the Data area
				newMenuBlocker.SetValue("", string.Empty, RegistryValueKind.String);
			}
		}

		public void DisableFullRightClickMenu()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				hku.DeleteSubKeyTree(loggedInSIDStr + @"\SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
			}
		}











	}
}
