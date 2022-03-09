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
			if (which == "StartWebSearch") return StartWebSearchOff();
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
			if (which == "StartWebSearch") StartWebSearchDisable();
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
			if (which == "StartWebSearch") StartWebSearchEnable();

		}

		// Safe delete of trees to prevent annoying and unnecessary error messages
		public static void DelTree(RegistryHive registryHive, string fullPathKeyToDelete)
		{

			using (var baseKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64))
			{
				// Adding false to this command says not to throw an exception if the key doesn't exist - just ignore
				//baseKey.DeleteSubKeyTree(fullPathKeyToDelete, false);
				baseKey.DeleteSubKeyTree(fullPathKeyToDelete);
			}
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
			DelTree(RegistryHive.Users,loggedInSIDStr + @"\SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0");
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
			DelTree(RegistryHive.ClassesRoot,@"Directory\shell\OpenCmdHereAsAdmin");
			DelTree(RegistryHive.ClassesRoot,@"Directory\Background\shell\OpenCmdHereAsAdmin");
			DelTree(RegistryHive.ClassesRoot,@"Drive\shell\OpenCmdHereAsAdmin");
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
		
		// When changing the default view, make sure to delete any saved "bags" which override the defaults
		public void killBags(string bagKey)
        {
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				string bagsFolder = loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\";
				using (RegistryKey explore = openCreate(hku, bagsFolder))
				{
					foreach (string key in explore.GetSubKeyNames())
					{
						var comTest = hku.OpenSubKey(bagsFolder + "\\" + key + "\\ComDlg\\" + bagKey);
						var shellTest = hku.OpenSubKey(bagsFolder + "\\" + key + "\\Shell\\" + bagKey);
						if ((comTest != null || shellTest != null) && key != "AllFolders")
                        {
							if (comTest != null) comTest.Close();
							if (shellTest != null) shellTest.Close();
							DelTree(RegistryHive.Users,bagsFolder+"\\"+key);
						}
					}
				}
			}
		}

		// Group bags. Are there some?
		public bool GroupBags(string bagKey)
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				string bagsFolder = loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\";
				using (RegistryKey explore = openCreate(hku, bagsFolder))
				{
					foreach (string key in explore.GetSubKeyNames())
					{
						RegistryKey comTest = hku.OpenSubKey(bagsFolder + "\\" + key + "\\ComDlg\\" + bagKey);
						RegistryKey shellTest = hku.OpenSubKey(bagsFolder + "\\" + key + "\\Shell\\" + bagKey);
						if ((comTest != null || shellTest != null) && key != "AllFolders")
						{
							RegistryKey toCheck = comTest != null ? comTest :shellTest;
							if (toCheck != null)
                            {
								var temp = toCheck.GetValue("GroupView");
								if (temp != null && (int)temp != 0) return true;
                            }
						}
					}
				}
			}
			return false;
		}


		//"{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}"
		internal bool DownloadGroupByOff()
		{
			// Using "using" to handle auto-close when it leaves this code block (so we don't have to manually close before returning)
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				// Found a local setting with groupby on for this key
				if (GroupBags("{885A186E-A440-4ADA-812B-DB871B942259}")) return false;
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885A186E-A440-4ADA-812B-DB871B942259}", true))
				{
					// Mode one for thumbnail view!
					if (explore == null || (int)explore.GetValue("Mode") != 1)
						return false;
					return true;
				}
			}

		}
		public void DownloadGroupByEnable()
		{
			killBags("{885A186E-A440-4ADA-812B-DB871B942259}");
			DelTree(RegistryHive.Users,loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885A186E-A440-4ADA-812B-DB871B942259}");
		}

		public void DownloadGroupByDisable()
		{
			killBags("{885A186E-A440-4ADA-812B-DB871B942259}");
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				// This sets the default option so that any time explorer tries to guess at the view for downloads, it uses ours (NO group by, thumbnail mode)
				using (RegistryKey explore = openCreate(hku, loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{885A186E-A440-4ADA-812B-DB871B942259}"))
				{
					explore.SetValue("GroupView", 0, RegistryValueKind.DWord);
					explore.SetValue("Mode", 1, RegistryValueKind.DWord);
				}
			}

		}


		internal bool SearchGroupByOff()
		{
			// Using "using" to handle auto-close when it leaves this code block (so we don't have to manually close before returning)
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				// Found a local setting with groupby on for this key
				if (GroupBags("{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}")) return false;
				using (RegistryKey explore = hku.OpenSubKey(loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}", true))
				{
					// Mode one for thumbnail view!
					if (explore == null || (int)explore.GetValue("Mode") != 1)
						return false;
					return true;
				}
			}

		}
		public void SearchGroupByEnable()
		{
			killBags("{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}");
			DelTree(RegistryHive.Users, loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}");
		}

		public void SearchGroupByDisable()
		{
			killBags("{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}");
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				// This sets the default option so that any time explorer tries to guess at the view for downloads, it uses ours (NO group by, thumbnail mode)
				using (RegistryKey explore = openCreate(hku, loggedInSIDStr + @"\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell\{7FDE1A1E-8B31-49A5-93B8-6BE14CFA4943}"))
				{
					explore.SetValue("GroupView", 0, RegistryValueKind.DWord);
					explore.SetValue("Mode", 1, RegistryValueKind.DWord);
				}
			}

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
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = openCreate(hku,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked\"))
				{
					explore.DeleteValue("{e2bf9676-5f8f-435c-97eb-11607a5bedf7}");
				}
			}
		}

		public void ExplorerRibbonEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey explore = openCreate(hku,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked\"))
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
			DelTree(RegistryHive.Users,loggedInSIDStr + @"\SOFTWARE\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
		}



		internal bool StartWebSearchOff()
		{
			using (var hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				using (var subKey = hku.OpenSubKey(loggedInSIDStr + @"\SOFTWARE\Policies\Microsoft\Windows\Explorer"))
				{
					// If an Eplorer policy doesn't exist, that means it's not set
					if (subKey == null) return false;
					return (GetValueInt(subKey,"DisableSearchBoxSuggestions") == 1);
				}
			}
		}

		public void StartWebSearchDisable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				RegistryKey subKey = openCreate(hku, loggedInSIDStr + @"\SOFTWARE\Policies\Microsoft\Windows\Explorer");
				subKey.SetValue("DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
			}
		}

		public void StartWebSearchEnable()
		{
			using (RegistryKey hku = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
			{
				RegistryKey subKey = openCreate(hku, loggedInSIDStr + @"\SOFTWARE\Policies\Microsoft\Windows\Explorer");
				subKey.DeleteValue("DisableSearchBoxSuggestions");
			}
		}







	}
}
