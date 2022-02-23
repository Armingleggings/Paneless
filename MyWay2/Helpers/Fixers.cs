using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Paneless.Helpers
{

	class Fixers
	{

		private string ClearWS(string str)
		{
			var first = str.Replace(System.Environment.NewLine, "");
			return first.Replace("\t", "");
		}

		// Loads registry functions
		private Regis regStuff = new Regis();
		// Watcher action toggles
		public bool watchNumL = false;

		// Translation array from fix shortname to various data about them
		private Dictionary<string, Dictionary<string, string>> fixers = new Dictionary<string, Dictionary<string, string>>
		{
			["MenuAll"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowALLRightClickOptions",
				["Img"] = @"/graphics/right_click_menu.png",
				["Title"] = @"Show ALL OPTIONS on right-click",
				["Description"] = @"
					Windows 11 is trying to dumb down the controls which isn't actually that terrible of a thing. The issue is if you know what you're doing, what you're looking for, and the thing you're looking for
					didn't make the cut meaning two clicks before become three now. This disables the easy mode feature and restores pre-windows 11 right-click
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Hidden Controls, #Windows Explorer, #Windows 11"
			},
			["F1"] = new Dictionary<string,string> {
				["PrefName"] = "KillF1UnhelpfulHelp",
				["Img"] = @"/graphics/F1key.png",
				["Title"] = @"Diable F1 ""Help"" function",
				["Description"] = @"
					Have you ever hit the F1 key by accident and had a distracting and unhelpful window or webpage open as a result? 
					Windows set the F1 key to a generic help function that basically never helps and always gets in the way. 
					Enable this control to disable that obnoxious design choice. Note that some programs still respond to F1 on their own accord, 
					but this will stop the default Windows behavior in things like Windows Explorer at least.
					",
				["Tags"] = "#Keyboard,#Rage"
			},
			["CMD"] = new Dictionary<string, string>
			{
				["PrefName"] = "RestoreAdminCMDContext",
				["Img"] = @"/graphics/CMD.png",
				["Title"] = @"Restore ""Open Admin CMD Window Here"" to Windows Explorer",
				["Description"] = @"
					When you need to run commands in CMD, it's usually in a specific folder. Windows used to have an option when you CTRL+Right Click 
					to show ""Open CMD HERE"" on a folder. This restores that function AND it's at administrative level (and you don't need to CTRL+CLICK to see it)
					",
				["Tags"] = "#Windows Explorer,#TimeSaver"
			},
			["NumL"] = new Dictionary<string, string>
			{
				["PrefName"] = "ForceNumLockAlwaysOn",
				["Img"] = @"/graphics/num_lock_off.png",
				["Title"] = @"Force Num-Lock ON",
				["Description"] = @"
					I don't know about you, but I don't even know why we CAN disable Num-Lock. If you have a number pad, it should be NUMBERS. 
					Enabling this fix starts a watcher process that continually monitors Num-Lock and if it sees that it's off, it forces it back ON. LIKE IT SHOULD BE!
					",
				["Tags"] = "#Keyboard,#Rage,#NumLock,#WhyDoWeEvenHAVEThatLever"
			},			
			["NumLBoot"] = new Dictionary<string, string>
			{
				["PrefName"] = "NumLockOnBoot",
				["Img"] = @"/graphics/num_lock_off.png",
				["Title"] = @"Num Lock ON when Booting",
				["Description"] = @"
					Windows allows you to force numlock OFF during boot, but also has the (correct) option to force it ON. This forces it on for boot so numlock will be on during the boot proccess.
					",
				["Activation_message"] = @"Since this affects boot options, you won't see any changes until you next reboot",
				["Tags"] = "#Keyboard,#Boot,#NumLock"
			},
			["Expand"] = new Dictionary<string, string>
			{
				["PrefName"] = "ExpandFolders",
				["Img"] = @"/graphics/expand_files.png",
				["Title"] = @"Expand folders in Windows Eplorer",
				["Description"] = @"
					For some reason, in newer Windows versions, they hide the folder structure. Maybe they think we're too stupid to follow along? 
					Either way, it's hella-confusing when you can't see the folders surrounding where you are in the 
					directory tree. How the hell are you supposed to navigate when most stuff is hidden?
					",
				["Tags"] = "#Windows Explorer,#Navigation"
			},
			["FileExt"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowFileExtensionsNotJustStupidIcons",
				["Img"] = @"/graphics/expand_files.png",
				["Title"] = @"Stop hiding file exentions",
				["Description"] = @"
					Isn't it insane that the default behavior of Windows is to hide the extension/filetype from the users? So instead, all we see is the icon 
					which varies by which program currently has ""ownership of it"". So what happens when we install an alternate PDF reader (or one gets installed 
					without us noticing) and the icon changes? Now there's no way to know what the hell it is. Extensions on ALWAYS. Let us see what the file actually IS!
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Files"
			},
			["ShowFiles"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowNonSystemHiddenFiles",
				["Img"] = @"/graphics/hidden_files.png",
				["Title"] = @"Show Hidden Files",
				["Description"] = @"
					Some ""System"" files should be hidden, but that's not what this is. Very common and important files that you will need access to like AppData, 
					programming, settings files, and other stuff is hidden making it really hard to work with. This undoes that. You can still see they're hidden, 
					but you can at least SEE them now. 
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Files"
			},
			["UserNav"] = new Dictionary<string, string>
			{
				["PrefName"] = "RemoveUserNavTurd",
				["Img"] = @"/graphics/user_folder.png",
				["Title"] = @"Axe the User Folder in the navigation pane",
				["Description"] = @"
					Something that has annoyed me for YEARS is how the User folder and all it's various subfolders is in the navigation pane between Quick Access 
					shortcuts that I like and My Computer (where I do any actual navigation). To Microsoft's credit, I didn't know that ""Show All Folders"" meant 
					turning that on (along with Control Panel and Recycle Bin in the Nav area). Regardless, I never needed those two there anyway and getting rid 
					of the turd is worth it.
					",
				["Tags"] = "#Windows Explorer,#Navigation"
			},
			["SearchGroupBy"] = new Dictionary<string, string>
			{
				["PrefName"] = "StopGroupingMyDamnSearch",
				["Img"] = @"/graphics/groupby.png",
				["Title"] = @"Kill the ""Group By"" view for search",
				["Description"] = @"
					I guess someone at Microsoft assumed that people wanted search results binned into groups by date for some reason. Not sure what they were smoking, 
					but all it does it complicate the view and make it impossible to find things. Woo! Let's hide most of the results by showing the most recent 3 or 4
					and hiding the rest! Sensible, right? SMH... This removes ""Group By"" default settings for search it doesn't override your preferred view.
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Rage"
			},			
			["DownloadGroupBy"] = new Dictionary<string, string>
			{
				["PrefName"] = "StopGroupingMyDamnDownloads",
				["Img"] = @"/graphics/groupby.png",
				["Title"] = @"Kill the ""Group By"" view for Downloads folders",
				["Description"] = @"
					No matter what you do, no matter what dark god you sacrifice to, Windows will NOT allow you to view Downloads in a sensible way.
					You can change settings and try every hack you know and STILL Downloads will force you to hunt for files in the ridiculous ""Group By"" format. 
					Is it here? Is it in another group? Who knows? Spend hours of frustrating time playing hide-and-seek with your files... or click on this fix to stop this nonsense for good!
					",
				["Tags"] = "#Windows Explorer,#Rage"
			},
			["ExplorerRibbon"] = new Dictionary<string, string>
			{
				["PrefName"] = "RestoreWinExplorerRibbonDammit",
				["Img"] = @"/graphics/explore_ribbon.png",
				["Title"] = @"Restore the Win Explorer Ribbon (Win 11)",
				["Description"] = @"
					The Ribbon was a masterfull mix of function and design - so of course Microsoft removes it in Windows 11 without any clear/easy way to bring it back.
					Well, some of us want to actually SEE more than 5 controls in our file explorer so this will bring back what they shouldn't have removed in the first place!
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Hidden Controls,#Downgrade,#Windows 11"
			},
			["Hibernate"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowHibernateOptionAlways",
				["Img"] = @"/graphics/hibernate.png",
				["Title"] = @"Show Hibernate option on Shutdown",
				["Description"] = @"
					Hibernate stores your computer state and shuts down completely - perfect for computers you don't use every day, or to save power, or when traveling, etc.... so why in the bacon-baked hell isn't it ALWAYS visible as an option!?
					This control restore Hibernate to the list of shutdown options LIKE IT SHOULD BE.
					",
				["Tags"] = "#Hidden Controls, #Hibernate, #Power, #Start Menu"
			},
			["StartWebSearch"] = new Dictionary<string, string>
			{
				["PrefName"] = "StartWebSearch",
				["Img"] = @"/graphics/start_web_search.png",
				["Title"] = @"Disable web results in Start Menu",
				["Description"] = @"
					It's curious isn't it? Who would have ever thought that while we're looking for commands, programs, files on our computer, we want to see result FROM THE WEB!?
					Click to disable this asinine behavior and end the timewasting distraction of web results where they don't belong.
					",
				["Activation_message"] = @"Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Start Menu, #Internet, #Feature Bleed"
			},
		};

		// remove start menu search online
		// remove cortana

		public Fixers()
		{
			// The readability hack above with multi-line strings introduces a bunch of extra whitespace. Let's clear that out
			foreach (var fixKey in fixers.Keys)
			{
				fixers[fixKey]["Description"] = ClearWS(fixers[fixKey]["Description"]);
			}
		}

		public List<string> FixerNames()
		{
			return fixers.Keys.ToList();
		}

		public bool IsFixed(string which)
		{
			// For NumL, if it's watching, it's fixed.
			if ("NumL" == which) return watchNumL;
			// For anything registry related
			return regStuff.IsFixed(which);
		}

		public void FixIt(string which)
		{
			if ("NumL" == which) watchNumL = true;
			else regStuff.FixIt(which);
		}
		public void BreakIt(string which)
		{
			if ("NumL" == which) watchNumL = false;
			else regStuff.BreakIt(which);
		}

		public Dictionary<string,string> GetFix(string which)
		{
			return fixers[which];
		}
	}
}
