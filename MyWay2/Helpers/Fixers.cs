using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Word;

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
			["OfficePaste"] = new Dictionary<string, string>
			{
				["PrefName"] = "PasteTextNotJunk",
				["Img"] = @"/graphics/advertising_id.png",
				["Title"] = @"Paste Plain Text as Default",
				["Snark"] = @"
					I suppose they were trying to be helpful in pulling in any and every possible extra information when you copy paste, but the thing is that should be an OPTION not the default.
					Instead of helpful, it becomes a constant battle of having to paste things into Notepad to clean the extra content off before moving it to where you actually wanted it (you can also change each paste to ""paste as plain text"", but both are tedious).
					To switch this inane default, click this fix
					",				
				["Description"] = @"
					If you prefer to paste plain text without all the extra sytle or meta-data that comes with it by defualt, click this fix. You can still paste the extra stuff by using the menu, but 
					""Paste Normal Text"" will be the default.
					",
				["Tags"] = "#Outlook, #CopyPaste, #PlainText"
			},					
			["AdvertisingID"] = new Dictionary<string, string>
			{
				["PrefName"] = "ImNotCattleDontTagMe",
				["Img"] = @"/graphics/advertising_id.png",
				["Title"] = @"Disable Advertising ID",
				["Snark"] = @"
					There's a scale of people and companies I trust to know enough about me to make suggestions of products and services ranging from my close friends and family
					all the way to data brokers who are trying to constantly scrape your information without consent. Microsoft has not earned that right. The advertising id is a way for
					Windows to tattle on you to everyone so they can tag and track you like cattle. Click this fix to stop that.
					",				
				["Description"] = @"
					An Advertising ID is a unique identify that Windows can provide to websites and apps (probably) so they can uniquely identify you and track your activity for advertising purposes.
					If you trust this information will be used responsibly, leave this alone. If you don't, click this fix to disable the ""feature"".
					",
				["Tags"] = "#Ads, #Moo, #Privacy"
			},			
			["StartSuggestions"] = new Dictionary<string, string>
			{
				["PrefName"] = "NoOneAskedYouStartMenu",
				["Img"] = @"/graphics/start_suggestions.png",
				["Title"] = @"Offer app suggestions in Start Menu",
				["Snark"] = @"
					Are suggestions useful? That depends on who's making them and why. Do you trust Microsoft to make suggestions that are in your best interests? Ones that are are 
					legitimately designed to improve your workflow and are a good value? Lol. Click this fix to tell Microsoft to mind their own business.
					",				
				["Description"] = @"
					Sometimes you'll see suggested apps and services in the Start Menu. To disable that, click this fix.
					",
				["Tags"] = "#Start Menu, #Ads, #Suggestions, #Adblock"
			},
			["WindowsTips"] = new Dictionary<string, string>
			{
				["PrefName"] = "NoIDontWantEdge",
				["Img"] = @"/graphics/windows_tips.png",
				["Title"] = @"Offer tips and suggestions for Windows",
				["Snark"] = @"
					Sometimes you're minding your business and up pops and notification that ""helpfully"" tells you, ""you could do this with Edge instead!"" or something similar. 
					Technically, this feature provides other tips besides USE MOAR EDGE!?!?!?!, but maybe it's worth throwing the bathwater out with the turd. To prevent MS from nagging you constantly 
					to use their products when you dare to open Chrome or some other superior browser, click this fix.
					",				
				["Description"] = @"
					Windows will sometimes notice what you're doing and provide ""tips"" for how to do it better. Sometimes those tips are to use Microsoft options instead which 
					is not as helpful. To disable this feature (all tips, not just the MS advertising ones), click this fix.
					",
				["Tags"] = "#Windows, #Ads, #Suggestions, #Adblock"
			},	
			["F1"] = new Dictionary<string,string> {
				["PrefName"] = "KillF1UnhelpfulHelp",
				["Img"] = @"/graphics/F1key.png",
				["Title"] = @"Diable F1 ""Help"" function",
				["Snark"] = @"
					Have you ever hit the F1 key by accident and had a distracting and unhelpful window or webpage open as a result? 
					Windows set the F1 key to a generic help function that basically never helps and always gets in the way. 
					Enable this control to disable that obnoxious design choice. Note that some programs still respond to F1 on their own accord, 
					but this will stop the default Windows behavior in things like Windows Explorer at least.
					",
				["Description"] = @"
					Pressing the F1 key in certain Microsoft programs opens a web page with ""Help"" for that tool (Windows Explorer for example). 
					If you dislike or don't use the function, it's quite distracting and is best disabled. Click this fix to do so.
					",
				["Tags"] = "#Keyboard,#Rage"
			},
			["CMD"] = new Dictionary<string, string>
			{
				["PrefName"] = "RestoreAdminCMDContext",
				["Img"] = @"/graphics/CMD.png",
				["Title"] = @"Restore ""Open Admin CMD Window Here"" to Windows Explorer",
				["Snark"] = @"
					Maybe you never need to point to any given folder and drop into a command window with ULTIMATE POWA! but I do. Not only have recent versions of Windows 
					removed the option to ""open CMD here"" through right-click context menus, you only get normal level anyway making it REALLY painful to open an elevated CMD and then 
					have to manually navigate like it was still the 90's. No thanks.
					",
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
				["Snark"] = @"
					Can you even imagine the though process that led to ""you know what we really need for our NUMBER PAD? Something that makes it NOT NUMBERS"". 
					I'm convinced this is all some kind of nasty prank by a bored programmer somewhere. If you want to extend the long finger to that guy, click this fix and 
					the program will copter-parent the key and make sure it stays active every time it tries to slack off.
					",
				["Description"] = @"
					I've heard legend of people who prefer their number pad not to be numbers, but I'm convinced it's a myth. If you want to force Num Lock on for as long 
					as this program is open, click this fix (Paneless will keep watching and smack Num-Lock back into shape if it tries to turn off - or if you hit it accidentally)
					",
				["Tags"] = "#Keyboard,#Rage,#NumLock,#WhyDoWeEvenHAVEThatLever"
			},			
			["NumLBoot"] = new Dictionary<string, string>
			{
				["PrefName"] = "NumLockOnBoot",
				["Img"] = @"/graphics/num_lock_off.png",
				["Title"] = @"Num Lock ON when Booting",
				["Snark"] = @"
					Windows allows you to force numlock OFF during boot (madness), but also has the (correct) option to force it ON. This fix forces NumLock to start off correctly - enabled.
					",
				["Description"] = @"
					Windows allows you to force numlock OFF during boot, but also has the option to force it ON. This forces it on for boot so numlock will be automatically turn on while loading Windows/booting.
					",
				["Activation_message"] = @"Num Lock on Boot - Since this affects boot options, you won't see any changes until you next reboot",
				["Tags"] = "#Keyboard,#Boot,#NumLock"
			},
			["Expand"] = new Dictionary<string, string>
			{
				["PrefName"] = "ExpandFolders",
				["Img"] = @"/graphics/expand_files.png",
				["Title"] = @"Expand folders in Windows Eplorer",
				["Snark"] = @"
					For some reason, in newer Windows versions, they hide the folder structure. Maybe they think we're too stupid to follow along? 
					Either way, it's hella-confusing when you can't see the folders surrounding where you are in the 
					directory tree. How the hell are you supposed to navigate when most stuff is hidden?
					",
				["Description"] = @"
					Modern versions of Windows only show the folders you've touched this session in the left navigation pane which makes it easier for some people to keep sense of their work session. 
					For people who have trouble keeping track of how files and folders are arranged when some are left hidden, this fix makes them all visible all the time. You'll have to scroll a bit more, 
					but at least you can see everything.
					",
				["Tags"] = "#Windows Explorer,#Navigation"
			},
			["FileExt"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowFileExtensionsNotJustStupidIcons",
				["Img"] = @"/graphics/expand_files.png",
				["Title"] = @"Stop hiding file exentions",
				["Snark"] = @"
					Isn't it insane that the default behavior of Windows is to hide the extension/filetype from the users? So instead, all we see is the icon 
					which varies by which program currently has ""ownership of it"". So what happens when we install an alternate PDF reader (or one gets installed 
					without us noticing) and the icon changes? Now there's no way to know what the hell it is. Extensions on ALWAYS. Let us see what the file actually IS!
					",				
				["Description"] = @"
					On the assumption that file exensions is overly confusing for the average user, Windows hides them by default. This may have very little 
					effect for people who don't care and never need to change a file extension, but it's quite a bother for people who do. Click this fix to show all file extensions.
				",
				["Activation_message"] = @"Show File Extensions - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Files"
			},
			["ShowFiles"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowNonSystemHiddenFiles",
				["Img"] = @"/graphics/hidden_files.png",
				["Title"] = @"Show Hidden Files",
				["Snark"] = @"
					Some ""System"" files should be hidden, but that's not what this is. Very common and important files that you will need access to like AppData, 
					programming, settings files, and other stuff is hidden making it really hard to work with. This undoes that. You can still see they're hidden, 
					but you can at least SEE them now. 
					",
				["Description"] = @"
					Files that manage folder settings, system controls, and so on are not always relevant and you might prefer to let Windows handle them. 
					However, there are times it's useful to edit or modify those files which is hard if you can't see them. Click this fix to make them visible. 
					",
				["Activation_message"] = @"Show Hidden Files - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Files"
			},
			["UserNav"] = new Dictionary<string, string>
			{
				["PrefName"] = "RemoveUserNavTurd",
				["Img"] = @"/graphics/user_folder.png",
				["Title"] = @"Axe the User Folder in the navigation pane",
				["Snark"] = @"
					Something that has annoyed me for YEARS is how the User folder and all it's various subfolders is in the navigation pane between Quick Access 
					shortcuts that I like and My Computer (where I do any actual navigation). To Microsoft's credit, I didn't know that ""Show All Folders"" meant 
					turning that on (along with Control Panel and Recycle Bin in the Nav area). Regardless, I never needed those two there anyway and getting rid 
					of the turd is worth it.
					",				
				["Description"] = @"
					When using the left-pane navigation in Windows Explorer, there's a ""User Folder"" with the various common locations for that user listed.
					This is not helpful if you pin your key folders to Quick Access and commonly want to see the disk drives instead of user folders.
					Click this fix to hide the folder so it doesn't get in the way.
					",
				["Tags"] = "#Windows Explorer,#Navigation"
			},
			["SearchGroupBy"] = new Dictionary<string, string>
			{
				["PrefName"] = "StopGroupingMyDamnSearch",
				["Img"] = @"/graphics/groupby.png",
				["Title"] = @"Kill the ""Group By"" view for search",
				["Snark"] = @"
					I guess someone at Microsoft assumed that people wanted search results binned into groups by date for some reason. Not sure what they were smoking, 
					but all it does it complicate the view and make it impossible to find things. Woo! Let's hide most of the results by showing the most recent 3 or 4 
					and hiding the rest! Sensible, right? SMH... This removes ""Group By"" default settings for search it doesn't override your preferred view.
					",
				["Description"] = @"
					Recent versions of windows groups search items by features such as their filetype, size, or date of creation. If you find this helpful, great.
					If you'd just prefer to see an alphabetical list, click this fix to disable ""Group By"".
					",
				["Activation_message"] = @"Disable Group-By (Search) - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Rage"
			},			
			["DownloadGroupBy"] = new Dictionary<string, string>
			{
				["PrefName"] = "StopGroupingMyDamnDownloads",
				["Img"] = @"/graphics/groupby.png",
				["Title"] = @"Kill the ""Group By"" view for Downloads folders",
				["Snark"] = @"
					No matter what you do, no matter what dark god you sacrifice to, Windows will NOT allow you to view Downloads in a sensible way. 
					You can change settings and try every hack you know and STILL Downloads will force you to hunt for files in the ridiculous ""Group By"" format. 
					Is it here? Is it in another group? Who knows? Spend hours of frustrating time playing hide-and-seek with your files... or click on this fix to stop this nonsense for good!
					",
				["Description"] = @"
					Recent versions of windows groups downloaded items by features such as their filetype, size, or date of creation. If you find this helpful, great.
					If you'd just prefer to see an alphabetical list, click this fix to disable ""Group By"".
					",				
				["Tags"] = "#Windows Explorer,#Rage"
			},
			["ExplorerRibbon"] = new Dictionary<string, string>
			{
				["PrefName"] = "RestoreWinExplorerRibbonDammit",
				["Img"] = @"/graphics/explore_ribbon.png",
				["Title"] = @"Restore the Win Explorer Ribbon (Win 11)",
				["Snark"] = @"
					The Ribbon was a masterfull mix of function and design - so of course Microsoft removes it in Windows 11 without any clear/easy way to bring it back. 
					Well, some of us want to actually SEE more than 5 controls in our file explorer so this will bring back what they shouldn't have removed in the first place!
					",
				["Description"] = @"
					The Ribbon was a masterfull mix of function and design, but Microsoft determined that a minimal control bar would be more useful.
					If you disagree, click this fix to return the ribbon in Windows Explorer.
					",
				["Activation_message"] = @"Windows Explorer Ribbon - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Windows Explorer,#Hidden Controls,#Downgrade,#Windows 11"
			},
			["MenuAll"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowALLRightClickOptions",
				["Img"] = @"/graphics/right_click_menu.png",
				["Title"] = @"Show ALL OPTIONS on right-click",
				["Snark"] = @"
					Windows 11 is trying to dumb down the controls which isn't actually that terrible of a thing. The issue is if you know what you're doing, what you're looking for, and the thing you're looking for 
					didn't make the cut meaning two clicks before become three now. This disables the easy mode feature and restores pre-windows 11 right-click
					",
				["Description"] = @"
					Windows 11 uses a simplified right-click menu with a few new features, but makes it harder if you prefer the full menu or have custom context options.
					Click ""FIX"" to restore the pre-Windows 11 right-click menu in Windows Eplorer.
					",
				["Activation_message"] = @"Show Right Click Menu - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Hidden Controls, #Windows Explorer, #Windows 11"
			},
			["Hibernate"] = new Dictionary<string, string>
			{
				["PrefName"] = "ShowHibernateOptionAlways",
				["Img"] = @"/graphics/hibernate.png",
				["Title"] = @"Show Hibernate option on Shutdown",
				["Snark"] = @"
					Hibernate stores your computer state and shuts down completely - perfect for computers you don't use every day, or to save power, or when traveling, etc.... so why in the bacon-baked hell isn't it ALWAYS visible as an option!? 
					This control restore Hibernate to the list of shutdown options LIKE IT SHOULD BE.
					",
				["Description"] = @"
					For low power modes, sleep has many drawbacks. The coputer is easily awoken, still uses power (which keeps accessories like cooling fans running and drains the battery), 
					and is vulnerable to data loss if power is lost. Hibernate stores your computer state and shuts down completely. It's a great feature, but is hidden by default. 
					Click this fix to have it listed along with your other power options.
					",
				["Tags"] = "#Hidden Controls, #Hibernate, #Power, #Start Menu"
			},
			["StartWebSearch"] = new Dictionary<string, string>
			{
				["PrefName"] = "StartWebSearch",
				["Img"] = @"/graphics/start_web_search.png",
				["Title"] = @"Disable web results in Start Menu",
				["Snark"] = @"
					It's curious isn't it? Who would have ever thought that while we're looking for commands, programs, files on our computer, we want to see result FROM THE WEB!? 
					Click to disable this asinine behavior and end the timewasting distraction of web results where they don't belong.
					",				
				["Description"] = @"
					If you find it distracting having web results when searching from your start menu, click this fix to keep the search function focused on your files and programs only.
					",
				["Activation_message"] = @"Turn off Web in the Start Menu - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
				["Tags"] = "#Start Menu, #Internet, #Feature Bleed"
			},			
			["LidNoSleep"] = new Dictionary<string, string>
			{
				["PrefName"] = "LidGoNightNight",
				["Img"] = @"/graphics/lid_sleep.png",
				["Title"] = @"Don't sleep when closing the lid",
				["Snark"] = @"
					ONLY APPLIES TO LAPTOPS. There are lots of reasons to close a laptop lid - getting up to get coffee without leaving an open invitation to cat-typed nonsense. Carrying it to another room, 
					shutting off the screen to save power while you take a quick call. Bottom line, if the narcoleptic little bugger fell asleep when you were only going to open the screen and get right back to work, 
					it's a huge pain in the butt. Click this fix to let the computer turn off the screen, but not actually fall asleep just because you closed the lid.
					",				
				["Description"] = @"
					ONLY APPLIES TO LAPTOPS. Many laptops put the computer to sleep simply because you closed the lid. This prevents that.
					",
				["Tags"] = "#Laptops, #Power, #Sleep"
			},		
		};

		// remove cortana

		public Fixers()
		{
			// The readability hack above with multi-line strings introduces a bunch of extra whitespace. Let's clear that out
			foreach (var fixKey in fixers.Keys)
			{
				fixers[fixKey]["Description"] = ClearWS(fixers[fixKey]["Description"]);
				fixers[fixKey]["Snark"] = ClearWS(fixers[fixKey]["Snark"]);
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
			if ("OfficePaste" == which)
			{
				//				WdPasteOptions PasteFormatFromExternalSource;
				//				PasteFormatFromExternalSource(1);
				return false;
			}

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
