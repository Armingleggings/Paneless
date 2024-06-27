from flask import Flask, render_template, request, jsonify
import winreg
import subprocess
import sys
import ctypes
import os

app = Flask(__name__)

def is_admin():
	try:
		return ctypes.windll.shell32.IsUserAnAdmin()
	except:
		return False

if not is_admin():
	ctypes.windll.shell32.ShellExecuteW(
		None, "runas", sys.executable, " ".join(sys.argv), None, 1
	)
	sys.exit(0)

fixes = {
	"Welcome": {
		"pref_name": "WelcomeExperienceAfterEveryDamnedUpdate",
		"img": "graphics/windows_welcome.png",
		"title": "Disable 'Welcome to Windows' after updates",
		"snark": """
			<p>Another volley in Microsoft's cloying desperation to trick you into picking Edge as your browser; the Welcome to Windows screen that shows up after updates is clearly intended into 
			confusing you into selecting their browser (and other options that benefit Microsoft) instead of leaving your crap alone... the way you already set it.</p>
			""",				
		"description": """
			<p>On major updates to Windows, it shows you the Welcome Experience again in an attempt to give you a second chance to do things their way. If you don't need the nag, 
			click this fix to disable this obnoxious behavior.</p>
			""",
		"tags": ["#Windows", "#Update", "#Nags", "#PweaseUseEdgeSenpaiUwU", "#TrickGrandma"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
				"fixed": [
					{
						"name": "SubscribedContent-310093Enabled",
						"data": 0
					}
				],
				"default": [
					{
						"name": "SubscribedContent-310093Enabled",
						"data": 1
					}				
				]
			}
		]
	},	
	"StartWebSearch": {
		"pref_name": "StartMenuSearchesOnlineToo",
		"img": "graphics/start_web_search.png",
		"title": "Disable web results in Start Menu",
		"snark": """
			<p>It's curious isn't it? Who would have ever thought that while we're looking for commands, programs, files on our computer, we want to see result FROM THE WEB!? 
			Click to disable this asinine behavior and end the timewasting distraction of web results where they don't belong.</p>
			""",				
		"description": """
			<p>If you find it distracting having web results when searching from your start menu, click this fix to keep the search function focused on your files and programs only.</p>
			""",
		"activation_message": "Turn off Web in the Start Menu - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"tags": ["#StartMenu", "#Internet", "#FeatureBleed"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Policies\Microsoft\Windows\Explorer",
				"fixed": [
					{
						"name": "DisableSearchBoxSuggestions",
						"data": 1
					}
				],
				"default": [
					{
						"name": "DisableSearchBoxSuggestions",
						"data": 0
					}				
				]
			}
		]
	},			
	"LidNoSleep": {
		"pref_name": "CloseLidAndItSleepsWTF",
		"img": "graphics/lid_sleep.png",
		"title": "Don't sleep when closing the lid",
		"snark": """
			<p>ONLY APPLIES TO LAPTOPS. There are lots of reasons to close a laptop lid - getting up to get coffee without leaving an open invitation to cat-typed nonsense. Carrying it to another room, 
			shutting off the screen to save power while you take a quick call.</p><p>Bottom line, click this fix to keep the narcoleptic little bugger awake if you're just changing locations in the house 
			and don't need it to sleep and wake constantly just because you closed the lid.</p>
			""",				
		"description": "<p>ONLY APPLIES TO LAPTOPS. Many laptops put the computer to sleep simply because you closed the lid. This prevents that.</p>",
		"tags": ["#Laptops", "#Power", "#Sleep", "#ArtaxNo"],
		"reg_fix": [
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\4f971e89-eebd-4455-a8de-9e59040e7347\5ca83367-6e45-459f-a27b-476b1d01c936",
				"fixed": [
					{
						"name": "Attributes",
						"data": 0
					}
				],
				"default": [
					{
						"name": "Attributes",
						"data": 1
					}				
				]
			},
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\4f971e89-eebd-4455-a8de-9e59040e7347\5ca83367-6e45-459f-a27b-476b1d01c936\DefaultPowerSchemeValues\381b4222-f694-41f0-9685-ff5bb260df2e",
				"fixed": [
					{
						"name": "ACSettingIndex",
						"data": 0
					},
					{
						"name": "DCSettingIndex",
						"data": 0
					}
				],
				"default": [
					{
						"name": "ACSettingIndex",
						"data": 1
					},
					{
						"name": "DCSettingIndex",
						"data": 1
					}				
				]
			},
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\4f971e89-eebd-4455-a8de-9e59040e7347\5ca83367-6e45-459f-a27b-476b1d01c936\DefaultPowerSchemeValues\8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c",
				"fixed": [
					{
						"name": "ACSettingIndex",
						"data": 0
					},
					{
						"name": "DCSettingIndex",
						"data": 0
					}
				],
				"default": [
					{
						"name": "ACSettingIndex",
						"data": 1
					},
					{
						"name": "DCSettingIndex",
						"data": 1
					}				
				]
			},
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\4f971e89-eebd-4455-a8de-9e59040e7347\5ca83367-6e45-459f-a27b-476b1d01c936\DefaultPowerSchemeValues\a1841308-3541-4fab-bc81-f71556f20b4a",
				"fixed": [
					{
						"name": "ACSettingIndex",
						"data": 0
					},
					{
						"name": "DCSettingIndex",
						"data": 0
					}
				],
				"default": [
					{
						"name": "ACSettingIndex",
						"data": 1
					},
					{
						"name": "DCSettingIndex",
						"data": 1
					}				
				]
			}
		]
	},	
	"NoSwipe": {
		"pref_name": "LoginWithoutSwiping",
		"img": "graphics/lockscreen.png",
		"title": "Disable extra click/swipe before login",
		"snark": """
			<p>Here's a great idea: have an extra barrier to logging in that requires a click... no wait, it was a swipe... wait a click? Dang! Now I only half-swiped. Crap! 
			This is hard to do with a mouse! Or... let's just turn that nonsense off. Enable this fix to make the login box show immediately on load without the silly barrier first.</p>
			""",				
		"description": """
			<p>For some reason, Windows 10+ likes to load with a pretty picture and no login controls. To reach login, you have to click or swipe (or click then swipe) which is an extra step 
			that really doesn't seem necessary. If you agree, click this fix.</p>
			""",
		"tags": ["#Windows", "#Login", "#DesignedForMobile"],
		"reg_fix": [
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"Software\Policies\Microsoft\Windows\Personalization",
				"fixed": [
					{
						"name": "NoLockScreen",
						"data": 1
					}
				],
				"default": [
					{
						"name": "NoLockScreen",
						"data": 0
					}				
				]
			}
		]
	},			
	"TaskManView":
	{
		"pref_name": "ShowUsefulTaskManager",
		"img": "graphics/task_manager_view.png",
		"title": "Show the full Task Manager",
		"snark": """
			<p>Can you even imagine? In a meeting room somewhere, someone pitched, "Hey! Let's reduce the Task Manager to literally zero useful information"
			and apparrently everyone clapped. Absurd.</p>
			<p>This, like many attempts to put training wheels on the computer was poorly-thought-out, though this one
			especially. If you actually want to use the Task Manager, click this fix.</p>
			""",				
		"description": """
			<p>The Task Manager is an important tool for being able to see what is using your computer resources and what kind. In the "minified" state, you can only see
			the names of programs and an "End Task" button which is next to useless. If that's enough for you, so be it. Otherwise, click this fix to have
			the traditional Task Manager view.</p>
			""",
		"tags": ["#TaskManager", "#Tools", "#TrainingWheels"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\TaskManager",
				"fixed": [
					{
						"name": "(Default)",
						"data": None
					}
				],
				"default": [
					{
						"name": "(Default)",
						"data": None
					}				
				]
			}
		]
	},	
	"ContextMenu":
	{
		"pref_name": "ShowUsefulRightClickMenu",
		"img": "graphics/context_menu.png",
		"title": "Restore the classic context menu for in Explorer",
		"snark": """
			<p>In "other menus we didn't ask to be simplified, we have the right-click context menu. For anyone who uses more than copy/paste, and basic commands, 
			hiding all the stuff we need behind extra clicks is obnoxious. Enable this fix to restore the right-click context menu to the kind pre-Windows 11.</p>
			""",				
		"description": """
			<p>There are any number of important and useful things you can do with a file by right-clicking and bringing up options, but not if you <b>can't see them</b>.
			Windows 11 hides them under a new compact menu which I suppose is helpful if you never use any of the more precise options, but is annoying otherwise. Enable this 
			fix to get the classic menu back.</p>
			""",
		"tags": ["#WindowsExplorer", "#Menus", "#HiddenControls", "Windows11", "#TrainingWheels"],
		"activation_message": "Context Menu Fix - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32",
				"fixed": [
					{
						#It can't even be "(Default) "" " or "(Default) value not set". It has to be "(Default) with literally nothing in the Data area
						"name": "(Default)",
						"data": None
					}
				],
				"default": [
					{
						"deletekey": 1,
						"key": r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}",
					}				
				]
			}
		]
	},	
	"ShakeMin":
	{
		"pref_name": "SuddenMinimizeWhenDragging",
		"img": "graphics/shake_minimize.png",
		"title": "Prevent \"Shake to Minimize\"",
		"snark": """
			<p>I wanted the window on the left, but changed my mind and put it on the right. That counts as a "shake" apparently and all my other windows minimized!
			Maybe someone wants this feature, but I've literally never had it happen because I wanted it to. So out it goes! Enable this fix to turn the "feature" off.</p>
			""",				
		"description": """
			<p>If you ever found all your windows minimizing while dragging another window around, it was probably because Windows thought you were using "shake to minmimize".
			If you didn't know what was happening or know but don't want that feature, enable this fix.</p>
			""",
		"tags": ["#Windows", "#UnwantedFeatures"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "DisallowShaking",
						"data": 1
					}
				],
				"default": [
					{
						"deletevalue": 1,
						"name": "DisallowShaking",
					}				
				]			
			}
		]
	},	
	"NoCortana":
	{
		"pref_name": "DisableAllThingsCortana",
		"img": "graphics/disable_cortana.png",
		"title": "Disable Cortana entirely",
		"snark": """
			<p>If Windows search actually worked worth a damn, maybe I'd keep Cortana, but it doesn't so why waste the resources? Does Cortana dream of electric sheep? Who cares? To the bin with you!</p>
			""",				
		"description": """
			<p>The "Cortana" search feature has various processes and functions that take up resources that are better used for other things if you don't use it. 
			Bonus: disabling it actually speeds up your computer in some ways. Enable the fix to disable Cortana.</p>
			""",
		"tags": ["#Windows", "#UnwantedFeatures"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Policies\Microsoft\Windows\Windows Search",
				"fixed": [
					{
						"name": "AllowCortana",
						"data": 0
					}
				],
				"default": [
					{
						"name": "AllowCortana",
						"data": 1
					}				
				]
			}
		]
	},	
	"VerboseLogon":
	{
		"pref_name": "VerboseLogInOut",
		"img": "graphics/verbose_messaging.png",
		"title": "Disable Cortana entirely",
		"snark": """
			<p>I just like to know that Windows is doing something and maybe what it is. It's useful for debugging or just generally knowing that it's not doing nothing at all.
			This fix enables more detailed status messages when logging in or out. (photo from thewindowsclub.com)</p>
			""",				
		"description": """
			<p>I just like to know that Windows is doing something and maybe what it is. It's useful for debugging or just generally knowing that it's not doing nothing at all.
			This fix enables more detailed status messages when logging in or out. (photo from thewindowsclub.com)</p>
			""",
		"tags": ["#Windows", "#Improvements"],
		"activation_message": "Verbose messaging - You won't see any changes until you next reboot",
		"reg_fix": [
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
				"fixed": [
					{
						"name": "verbosestatus",
						"data": 1
					}
				],
				"default": [
					{
						"name": "verbosestatus",
						"data": 0
					}				
				]
			}
		]
	},	
	"AdvertisingID":
	{
		"pref_name": "ImNotCattleDontTagMe",
		"img": "graphics/advertising_id.png",
		"title": "Disable Advertising ID",
		"snark": """
			<p>There's a scale of people and companies I trust to know enough about me to make suggestions of products and services ranging from my close friends and family
			all the way to data brokers who are trying to constantly scrape your information without consent. Microsoft has not earned that right. The advertising id is a way for Windows to tattle on you to everyone so they can tag and track you like cattle. Click this fix to stop that.</p>
			""",				
		"description": """
			<p>An Advertising ID is a unique identify that Windows can provide to websites and apps (probably) so they can uniquely identify you and track your activity for advertising purposes.
			If you trust this information will be used responsibly, leave this alone. If you don't, click this fix to disable the ""feature"".</p>
			""",
		"tags": [ "#Ads","#Moo","#Privacy"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
				"fixed": [
					{
						"name": "enabled",
						"data": 0
					}
				],
				"default": [
					{
						"name": "enabled",
						"data": 1
					}				
				]
			}
		]
	},	
	"StartSuggestions":
	{
		"pref_name": "NoOneAskedYouStartMenu",
		"img": "graphics/start_suggestions.png",
		"title": "Offer app suggestions in Start Menu",
		"snark": """
			<p>Are suggestions useful? That depends on who's making them and why. Do you trust Microsoft to make suggestions that are in your best interests? Ones that are are 
			legitimately designed to improve your workflow and are a good value? Lol. Click this fix to tell Microsoft to mind their own business.</p>
			""",				
		"description": """
			<p>Sometimes you'll see suggested apps and services in the Start Menu. To disable that, click this fix.</p>
			""",
		"tags": [ "#StartMenu","#Ads","#Suggestions","#Adblock"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
				"fixed": [
					{
						"name": "SubscribedContent-338388Enabled",
						"data": 0
					}
				],
				"default": [
					{
						"name": "SubscribedContent-338388Enabled",
						"data": 1
					}				
				]
			}
		]
	},
	"WindowsTips":
	{
		"pref_name": "NoIDontWantEdge",
		"img": "graphics/windows_tips.png",
		"title": "Offer tips and suggestions for Windows",
		"snark": """
			<p>Sometimes you're minding your business and up pops and notification that ""helpfully"" tells you, ""you could do this with Edge instead!"" or something similar. 
			Technically, this feature provides other tips besides USE MOAR EDGE!?!?!?!, but maybe it's worth throwing the bathwater out with the turd. To prevent MS from nagging you constantly 
			to use their products when you dare to open Chrome or some other superior browser, click this fix.</p>
			""",				
		"description": """
			<p>Windows will sometimes notice what you're doing and provide ""tips"" for how to do it better. Sometimes those tips are to use Microsoft options instead which 
			is not as helpful. To disable this feature (all tips, not just the MS advertising ones), click this fix.</p>
			""",
		"tags": [ "#Windows","#Ads","#Suggestions","#PweaseUseEdgeSenpaiUwU", "#TrickGrandma","#Adblock"],
		"reg_fix": [
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SOFTWARE\Policies\Microsoft\Windows\CloudContent",
				"fixed": [
					{
						"name": "DisableSoftLanding",
						"data": 1
					}
				],
				"default": [
					{
						"name": "DisableSoftLanding",
						"data": 0
					}				
				]
			}
		]
	},	
	"F1": 
	{
		"pref_name": "KillF1UnhelpfulHelp",
		"img": "graphics/F1key.png",
		"title": "Diable F1 ""Help"" function",
		"snark": """
			<p>Have you ever hit the F1 key by accident and had a distracting and unhelpful window or webpage open as a result? 
			Windows set the F1 key to a generic help function that basically never helps and always gets in the way. </p>
			<p>Enable this control to disable that obnoxious design choice. Note that some programs still respond to F1 on their own accord, 
			but this will stop the default Windows behavior in things like Windows Explorer at least.</p>
			""",				
		"description": """
			<p>Pressing the F1 key in certain Microsoft programs opens a web page with ""Help"" for that tool (Windows Explorer for example). 
			If you dislike or don't use the function, it's quite distracting and is best disabled. Click this fix to do so.</p>
			""",
		"tags": [ "#Keyboard","#Rage"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\1.0\0\win64",
				"fixed": [
					{
						"name": "(Default)",
						"data": None
					}
				],
				"default": [
					{
						"deletevalue": 1,
						"key": r"SOFTWARE\Classes\Typelib\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}"
					}				
				]
			}
		]
	},
	"CMD":
	{
		"pref_name": "RestoreAdminCMDContext",
		"img": "graphics/CMD.png",
		"title": "Restore ""Open Admin CMD Window Here"" to Windows Explorer",
		"snark": """
			<p>Maybe you never need to point to any given folder and drop into a command window with ULTIMATE POWA! but I do. Not only have recent versions of Windows 
			removed the option to ""open CMD here"" through right-click context menus, you only get normal level anyway making it REALLY painful to open an elevated CMD and then 
			have to manually navigate like it was still the 90's. No thanks.</p>
			""",				
		"description": """
			<p>When you need to run commands in CMD, it's usually in a specific folder. Windows used to have an option when you CTRL+Right Click 
			to show ""Open CMD HERE"" on a folder. This restores that function AND it's at administrative level (and you don't need to CTRL+CLICK to see it)</p>
			""",
		"tags": [ "#WindowsExplorer","#TimeSaver","#AhHaHaThePower"],
		"reg_fix": [
			{
				"hive": "HKEY_CLASSES_ROOT",
				"path": r"Directory\Background\shell\OpenCmdHereAsAdmin",
				"fixed": [
					{
						"name": "(Default)",
						"type": "string_val",
						"data": "Command Prompt (Admin)"
					},
					{
						"name": "icon",
						"type": "string_val",
						"data": r"C:\Windows\System32\cmd.exe"
					},
					{
						"name": "HasLUAShield",
						"type": "string_val",
						"data": None
					},
					{
						"name": "NoWorkingDirectory",
						"type": "string_val",
						"data": None
					},
					{
						"name": "Position",
						"type": "string_val",
						"data": "Top"
					}
				],
				"default": [
					{
						"deletekey": 1,
						"key": r"Directory\Background\shell\OpenCmdHereAsAdmin"
					}				
				]
			},
			{
				"hive": "HKEY_CLASSES_ROOT",
				"path": r"Directory\Background\shell\OpenCmdHereAsAdmin\command",
				"fixed": [
					{
						"name": "(Default)",
						"type": "string_val",
						"data": "cmd.exe /s /k pushd \"%V\""
					}
				],
				# Delete is handled by the previous section since this key is under the one above
				"default": [
					{
						"nop": 1,
					}				
				]
			}
		]
	},
	"NoNumL":
	{
		"pref_name": "DisableNumLockKey",
		"img": "graphics/num_lock_off.png",
		"title": "Disable the NumLock Key",
		"snark": """
			<p>Can you even imagine the though process that led to ""you know what we really need for our NUMBER PAD? Something that makes it NOT NUMBERS"". 
			I'm convinced this is all some kind of nasty prank by a bored programmer somewhere. If you want to extend the long finger to that guy, click this fix and 
			the key will be disabled forevermore (important! pair with the num lock on boot fix to keep numlock ALWAYS ON)!</p>
			""",				
		"description": """
			<p>I've heard legend of people who prefer their number pad not to be numbers, but I'm convinced it's a myth. If you want to force Num Lock on forever, click this fix
			and then make sure "Num Lock ON when Booting" is also on and it will turn on when you boot and can't be turned off because they key will be disabled.</p>
			""",
		"tags": [ "#Keyboard","#Rage","#NumLock","#WhyDoWeEvenHAVEThatLever"]
	},			
	"NumLBoot":
	{
		"pref_name": "NumLockOnBoot",
		"img": "graphics/num_lock_off.png",
		"title": "Num Lock ON when Booting",
		"snark": """
			<p>Windows allows you to force numlock OFF during boot (madness), but also has the (correct) option to force it ON. This fix forces NumLock to start off correctly - enabled.</p>
			""",				
		"description": """
			<p>Windows allows you to force numlock OFF during boot, but also has the option to force it ON. This forces it on for boot so numlock will be automatically turn on while loading Windows/booting.</p>
			""",
		"activation_message": "Num Lock on Boot - Since this affects boot options, you won't see any changes until you next reboot",
		"tags": [ "#Keyboard","#Boot","#NumLock"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Control Panel\Keyboard",
				"fixed": [
					{
						"name": "InitialKeyboardIndicators",
						#not an error - 2 is the correct value here
						"data": 2,
					}
				],
				"default": [
					{
						"name": "InitialKeyboardIndicators",
						"data": 0
					}				
				]
			}
		]
	},
	"StartAccountNag":
	{
		"pref_name": "NoStartAccountNag",
		"img": "graphics/online_account_start_nag.png",
		"title": "Disable start menu account begging",
		"snark": """
			<p>No, Microsoft. You aren't trustworthy enough to tie my computer activity to your online accounts. You're not as bad as Google, but you're not great. So no, I won't make an online account.
			Stop nagging. Click to disable the start menu "You haven't signed in yet!" nag.</p>
			""",				
		"description": """
			<p>There are some cases where signing into a Microsoft account on your computer can be advantageous, but if you prefer to keep full control over your files and data, it's best not to.
			And if you made that decision, the nag is nothing but a nag. Click this fix to turn off the Start Menu "account nag".</p>
			""",
		"tags": [ "#StartMenu","#MicrosoftAccount", "#PweaseICanHazUrData", "Windows11"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "Start_AccountNotifications",
						"data": 0
					}
				],
				"default": [
					{
						"name": "Start_AccountNotifications",
						"data": 1
					}				
				]
			}
		]
	},
	"Expand":
	{
		"pref_name": "ExpandFolders",
		"img": "graphics/expand_files.png",
		"title": "Expand folders in Windows Eplorer",
		"snark": """
			<p>For some reason, in newer Windows versions, they hide the folder structure. Maybe they think we're too stupid to follow along? 
			Either way, it's hella-confusing when you can't see the folders surrounding where you are in the 
			directory tree. How the hell are you supposed to navigate when most stuff is hidden?</p>
			""",				
		"description": """
			<p>Modern versions of Windows only show the folders you've touched this session in the left navigation pane which makes it easier for some people to keep sense of their work session. 
			For people who have trouble keeping track of how files and folders are arranged when some are left hidden, this fix makes them all visible all the time. You'll have to scroll a bit more, 
			but at least you can see everything.</p>
			""",
		"tags": [ "#WindowsExplorer","#Navigation"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "NavPaneExpandToCurrentFolder",
						"data": 1
					}
				],
				"default": [
					{
						"name": "NavPaneExpandToCurrentFolder",
						"data": 0
					}				
				]			
			}
		]
	},
	"FileExt":
	{
		"pref_name": "ShowFileExtensionsNotJustStupidIcons",
		"img": "graphics/expand_files.png",
		"title": "Stop hiding file exentions",
		"snark": """
			<p>Isn't it insane that the default behavior of Windows is to hide the extension/filetype from the users? So instead, all we see is the icon 
			which varies by which program currently has ""ownership of it"". So what happens when we install an alternate PDF reader (or one gets installed 
			without us noticing) and the icon changes? Now there's no way to know what the hell it is. Extensions on ALWAYS. Let us see what the file actually IS!</p>
			""",				
		"description": """
			On the assumption that file exensions is overly confusing for the average user, Windows hides them by default. This may have very little 
			<p>effect for people who don't care and never need to change a file extension, but it's quite a bother for people who do. Click this fix to show all file extensions.</p>
			""",
		"activation_message": "Show File Extensions - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"tags": [ "#WindowsExplorer","#Files"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "HideFileExt",
						"data": 0
					}
				],
				"default": [
					{
						"name": "HideFileExt",
						"data": 1
					}				
				]			
			}
		]
	},
	"ShowFiles":
	{
		"pref_name": "ShowNonSystemHiddenFiles",
		"img": "graphics/hidden_files.png",
		"title": "Show Hidden Files",
		"snark": """
			<p>Some ""System"" files should be hidden, but that's not what this is. Very common and important files that you will need access to like AppData, 
			programming, settings files, and other stuff is hidden making it really hard to work with. This undoes that. You can still see they're hidden, 
			but you can at least SEE them now. </p>
			""",				
		"description": """
			<p>Files that manage folder settings, system controls, and so on are not always relevant and you might prefer to let Windows handle them. 
			However, there are times it's useful to edit or modify those files which is hard if you can't see them. Click this fix to make them visible. </p>
			""",
		"activation_message": "Show Hidden Files - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"tags": [ "#WindowsExplorer","#Files"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "Hidden",
						"data": 1
					}
				],
				"default": [
					{
						"name": "Hidden",
						"data": 2
					}				
				]			
			}
		]
	},
	"UserNav":
	{
		"pref_name": "RemoveUserNavTurd",
		"img": "graphics/user_folder.png",
		"title": "Axe the User Folder in the navigation pane",
		"snark": """
			<p>Something that has annoyed me for YEARS is how the User folder and all it's various subfolders is in the navigation pane between Quick Access 
			shortcuts that I like and My Computer (where I do any actual navigation). To Microsoft's credit, I didn't know that ""Show All Folders"" meant 
			turning that on (along with Control Panel and Recycle Bin in the Nav area). Regardless, I never needed those two there anyway and getting rid 
			of the turd is worth it.</p>
			""",				
		"description": """
			<p>When using the left-pane navigation in Windows Explorer, there's a ""User Folder"" with the various common locations for that user listed.
			This is not helpful if you pin your key folders to Quick Access and commonly want to see the disk drives instead of user folders.
			Click this fix to hide the folder so it doesn't get in the way.</p>
			""",
		"tags": [ "#WindowsExplorer","#Navigation"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
				"fixed": [
					{
						"name": "NavPaneShowAllFolders",
						"data": 0
					}
				],
				"default": [
					{
						"name": "NavPaneShowAllFolders",
						"data": 1
					}				
				]			
			}
		]
	},
	"GroupBy":
	{
		"pref_name": "KillTheGroupByViewForever",
		"img": "graphics/groupby.png",
		"title": "Kill the ""Group By"" view",
		"snark": """
			<p>What were they smoking? Seriously. I NEVER want to see things "grouped" by some random value - ever. Why would I? Are they stupid? They very least Microsoft
			could have done was give us an easy option for disabiling it, but it's a damned nightmare. Nice work Microsoft.</p.
			<p>Luckily some enterprising person created an open source tool to set ALL folder views to whatever you choose (which removes GroupBy in the process) - check out 
			<a href="https://lesferch.github.io/WinSetView/">https://lesferch.github.io/WinSetView/</a></p>
			<p>OR you can click here to open the included version (might be outdated compared to what's at the link).</p>
			""",				
		"description": """
			<p>Recent versions of windows groups search items by features such as their filetype, size, or date of creation. If you find this helpful, great.
			If you'd just prefer to see an alphabetical list, click this fix to disable ""Group By"".</p>
			<p>Luckily some enterprising person created an open source tool to set ALL folder views to whatever you choose (which removes GroupBy in the process) - check out 
			<a href="https://lesferch.github.io/WinSetView/">https://lesferch.github.io/WinSetView/</a></p>
			<p>OR you can click here to open the included version (might be outdated compared to what's at the link).</p>
			""",
		"activation_message": "Disable Group-By (Search) - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"tags": [ "#WindowsExplorer", "#Rage", "#SeriouslyRage", "#SoMuchRage"]
	},			
	"ExplorerRibbon":
	{
		"pref_name": "RestoreWinExplorerRibbonDammit",
		"img": "graphics/explore_ribbon.png",
		"title": "Restore the Win Explorer Ribbon (Win 11)",
		"snark": """
			<p>The Ribbon was a masterfull mix of function and design - so of course Microsoft removes it in Windows 11 without any clear/easy way to bring it back. 
			Well, some of us want to actually SEE more than 5 controls in our file explorer so this will bring back what they shouldn't have removed in the first place!</p>
			""",				
		"description": """
			<p>The Ribbon was a masterfull mix of function and design, but Microsoft determined that a minimal control bar would be more useful.
			If you disagree, click this fix to return the ribbon in Windows Explorer.</p>
			""",
		"activation_message": "Windows Explorer Ribbon - Windows must be restarted OR restart Explorer from the Task Manager to see the changes. #RestartWinExplorer",
		"tags": [ "#WindowsExplorer","#HiddenControls","#Downgrade","#Windows11"],
		"reg_fix": [
			{
				"hive": "HKEY_CURRENT_USER",
				"path": r"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked",
				"fixed": [
					{
						"name": "{e2bf9676-5f8f-435c-97eb-11607a5bedf7}",
						"type": "string_val",
						"data": None
					}
				],
				"default": [
					{
						"delete": 1,
						"name": "{e2bf9676-5f8f-435c-97eb-11607a5bedf7}",
					}				
				]
			}
		]
	},
	"Hibernate":
	{
		"pref_name": "ShowHibernateOptionAlways",
		"img": "graphics/hibernate.png",
		"title": "Show Hibernate option on Shutdown",
		"snark": """
			<p>Hibernate stores your computer state and shuts down completely - perfect for computers you don't use every day, or to save power, or when traveling, etc.... so why in the bacon-baked hell isn't it ALWAYS visible as an option!? 
			This control restore Hibernate to the list of shutdown options LIKE IT SHOULD BE.</p>
			""",				
		"description": """
			<p>For low power modes, sleep has many drawbacks. The computer is easily awoken, still uses power (which keeps accessories like cooling fans running and drains the battery), and is vulnerable to data loss if power is lost. Hibernate stores your computer state and shuts down completely. It's a great feature, but is hidden by default. Click this fix to have it listed along with your other power options.</p>
			""",
		"tags": [ "#Hidden Controls","#Hibernate","#Power","#StartMenu"],
		"reg_fix": [
			{
				"hive": "HKEY_LOCAL_MACHINE",
				"path": r"SYSTEM\CurrentControlSet\Control\Power",
				"fixed": [
					{
						"name": "HibernateEnabled",
						"data": 1
					}
				],
				"default": [
					{
						"name": "HibernateEnabled",
						"data": 0
					}				
				]			
			}
		]
	},
}

def user_message(message):
	print(message)

def binary_to_stringlist(binary_value):
    hex_string = binary_value.hex()  # Convert binary to hex string
    return [hex_string[i:i+8] for i in range(0, len(hex_string), 8)]

def stringlist_to_binary(hex_list):
    hex_string = ''.join(hex_list)  # Join the list of hex strings
    return bytes.fromhex(hex_string)  # Convert hex string back to binary

def strip_stringlist(string_list, target_string):
    string_list = [s for s in string_list if s != target_string]
    return string_list

def hive_name(hive):
	if hive == winreg.HKEY_CURRENT_USER:
		return "HKEY_CURRENT_USER"
	elif hive == winreg.HKEY_LOCAL_MACHINE:
		return "HKEY_LOCAL_MACHINE"
	elif hive == winreg.HKEY_CLASSES_ROOT:
		return "HKEY_CLASSES_ROOT"
	elif hive == winreg.HKEY_USERS:
		return "HKEY_USERS"
	elif hive == winreg.HKEY_PERFORMANCE_DATA:
		return "HKEY_PERFORMANCE_DATA"
	elif hive == winreg.HKEY_CURRENT_CONFIG:
		return "HKEY_CURRENT_CONFIG"
	else:
		return "UNKNOWN_HIVE"
	
def open_or_create_key(hive, path):
	try:
		# Open the registry key for reading and writing
		key = winreg.OpenKeyEx(hive, path, 0, winreg.KEY_ALL_ACCESS | winreg.KEY_WOW64_64KEY)
		print(f"Key opened: {hive_name(hive)}\\{path}")
	except FileNotFoundError:
		# Handle if the key doesn't exist
		print(f"Creating key: {hive_name(hive)}\\{path}")
		key = winreg.CreateKeyEx(hive, path, 0, winreg.KEY_ALL_ACCESS | winreg.KEY_WOW64_64KEY)
	except PermissionError:
		# Handle if there are permission issues
		print(f"Permission denied while accessing the key: {hive_name(hive)}\\{path}")
		key = None
	except Exception as e:
		# Handle any other exceptions
		print(f"An error occurred: {e}")
		key = None
	return key

def is_fixed(which):
	global fixes

	# Damned hex binary crap
	if which == "NoNumL":
		key = open_or_create_key(winreg.HKEY_LOCAL_MACHINE, r"SYSTEM\CurrentControlSet\Control\Keyboard Layout")
		try:
			current_value, _ = winreg.QueryValueEx(key, "Scancode Map")
			current_value = binary_to_stringlist(current_value)
			if "000045e0" in current_value:
				return 1
			else:
				winreg.CloseKey(key)
				return 0
		except FileNotFoundError:
			# if it's not found, it's not fixed.
			winreg.CloseKey(key)
			return 0

	if not "reg_fix" in fixes[which]:
		return 0

	reg_fix = fixes[which]["reg_fix"]

	for rg in reg_fix:
		if rg["hive"] == "HKEY_LOCAL_MACHINE":
			hive = winreg.HKEY_LOCAL_MACHINE
		else:
			hive = winreg.HKEY_CURRENT_USER

		key = open_or_create_key(hive, rg["path"])
		if not key: 
			return 0
		
		for a_fix in rg["fixed"]:
			try:
				current_value, _ = winreg.QueryValueEx(key, a_fix["name"])
				#default is integer, but if not, leave it alone
				if not "type" in a_fix:
					current_value = int(current_value)
			except FileNotFoundError:
				winreg.CloseKey(key)
				return 0
		
	winreg.CloseKey(key)
	return 1

# 0 means break it
# 1 means fix it
# returns dict with various information about the fix
def other_fixer(which, fix_it=None):
	global fixes

	# Get and store current value
	previous = is_fixed(which)
	if previous == fix_it: 
		return {"previous": fix_it, "desired": fix_it,"actual": fix_it, "note": "no change was made"}
	
	# Damned hex binary crap
	if which == "NoNumL":
		key = open_or_create_key(winreg.HKEY_LOCAL_MACHINE, r"SYSTEM\CurrentControlSet\Control\Keyboard Layout")
		try:
			current_value, _ = winreg.QueryValueEx(key, "Scancode Map")
			current_value = binary_to_stringlist(current_value)
			# is there a numlock value in there already? 
			# Search for a string that ends with "45e0"
			found_45e0 = next(value.endswith("45e0") for value in current_value)
			# Follow along, whether we're fixing or not, this value needs to go. If we're going to default, no numlock map. If we're disabling it, best to make sure there's no other mapping. Just kill it
			current_value = strip_stringlist(current_value, found_45e0)
			# We just deleted the mapping so fix_it == 0 is covered so just check fix_it == 1
			if fix_it == 1:
				current_value.insert(-1, "000045e0")
			# This mapping has two rows of zero to start and one at the end. Just count everything else.
			current_value[2] = str(len(current_value) - 3).zfill(2) + "000000"			
			winreg.SetValueEx(key, "Scancode Map", 0, winreg.REG_BINARY, stringlist_to_binary(current_value))
		except FileNotFoundError:
			# Scancode Map isn't there? Then this is easy.
			if fix_it == 1:
				winreg.SetValueEx(key, "Scancode Map", 0, winreg.REG_BINARY, stringlist_to_binary(['00000000', '00000000', '02000000', '000045e0', '00000000']))
		winreg.CloseKey(key)
		return {"previous": previous, "desired": fix_it, "actual": is_fixed(which)}

	if which == "GroupBy":
		user_message("Select the view you want and click Apply To All")
		# Get the path to the virtual environment directory
		venv_path = os.path.join(os.getcwd(), "venv")
		# Specify the path to the executable within the virtual environment
		executable_path = os.path.join(venv_path, "extras", "WinSetView.exe")

		# Check if the executable file exists
		if os.path.isfile(executable_path):
			# Call the executable using subprocess
			subprocess.run([executable_path])
		return {"previous": previous, "desired": fix_it, "actual": fix_it}


# 0 means break it
# 1 means fix it
# returns dict with various information about the fix
def reg_fixer(which, fix_it=None):
	global fixes
	reg_fix = fixes[which]["reg_fix"]

	# Get and store current value
	previous = is_fixed(which)
	if previous == fix_it: 
		return {"previous": fix_it, "desired": fix_it,"actual": fix_it, "note": "no change was made"}

	for rg in reg_fix:
		if rg["hive"] == "HKEY_LOCAL_MACHINE":
			hive = winreg.HKEY_LOCAL_MACHINE
		elif rg["hive"] == "HKEY_CURRENT_USER":
			hive = winreg.HKEY_CURRENT_USER

		key = open_or_create_key(hive, rg["path"])
		if not key:
			return 0
		
		if fix_it:
			fix_or_break = rg["fixed"]
		else:
			fix_or_break = rg["default"]

		for a_fix in fix_or_break:
			try:
				if "type" in a_fix and a_fix["type"] == "string_val":
					which_type = winreg.REG_SZ
				else:
					which_type = winreg.REG_DWORD

				if "nop" in a_fix:
					# In some cases, we specifically don't want to do anything.
					continue
				elif "deletekey" in a_fix:
					winreg.DeleteKey(hive, a_fix["key"])
				elif "deletevalue" in a_fix:
					winreg.DeleteValue(key, a_fix["name"])
				else:
					winreg.SetValueEx(key, a_fix["name"], 0, which_type, a_fix["data"])

			except FileNotFoundError:
				print ("registry value couldn't be set", a_fix)
				winreg.CloseKey(key)
				return 0

	winreg.CloseKey(key)
	return {"previous": previous, "desired": fix_it, "actual": is_fixed(which)}

# Executes a command and returns the last value
def ps_command(the_command):
	output = subprocess.run(["powershell", "-Command", the_command], capture_output=True, text=True)
	return output.stdout.strip()

settings = {}

def get_settings():
	global fixes
	for key, a_fix in fixes.items():
		settings[a_fix["pref_name"]] = is_fixed(key)
	return settings

settings = get_settings()

print(settings)

@app.route('/')
def home_page():
	settings = get_settings()	
	return render_template("index.html", fixes=fixes, settings=settings)

@app.route('/toggle_fix', methods=['POST'])
def toggle_fix():
	global fixes, settings
	
	data = request.get_json()
	fix_key = data.get('fix_key')
	fixed = data.get('fixed')

	if "reg_fix" in fixes[fix_key]:
		result = reg_fixer(fix_key, 0 if fixed else 1)
	# not clean registry stuff so have to get more creative
	else:
		result = other_fixer(fix_key, 0 if fixed else 1)
		
	response = {'status': 'success', 'message': {"text": f'Toggled fix {fix_key}', 'fixed': result["actual"], 'full_return': result}}
	return jsonify(response)

if __name__ == '__main__':
	app.run(debug=True)