﻿using Paneless.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Paneless
{

	public partial class MainWindow : Window
	{
		// Detects state of fix. Toggles fixes on and off (eitehr by registry, filesystem, or whatever is needed)
		private Fixers fixers = new Fixers();
		// Registry stuff. Don't need a lot, but need some
		private Regis regStuff = new Regis();
		// Loads preferences from the file
		private Prefs prefs = null;
		// A copy of prefs when loading a new file so we can undo
		private Prefs holdPrefs = null;
		// Holds onto tags for filtering - hashset is like an array, but the values are unique apparently so adding the same one does nothing (behavior that helps for what we're doing)
		private HashSet<string> tags = new HashSet<string>();

		// For numlock
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		public static extern short GetKeyState(int keyCode);

		// Create a dictionary to hold our various controls.
		private Dictionary<string,FixerBox> fixerBoxes = new Dictionary<string, FixerBox> { };

		private string ClearWS(string str)
		{
			string temp = str.Replace("\t", "");
			return temp;
		}

		private void MultiStatus(string[] messages)
		{
			ShowStatus(String.Join("<LineBreak/>", messages));
		}

		private void ShowStatus(string status)
		{
			ClearStatus();
			Button btn = null;

			// There's probably a much cleaner way of doing this. Oh well...
			if (status.Contains("#RestartWinExplorer"))
            {
				status.Replace("#RestartWinExplorer", "");
				btn = new Button();
				btn.Content = "(Click here to retstart Windows Explorer now)";
				btn.Style = FindResource("LinkButton") as Style;
				btn.Foreground = new SolidColorBrush(Color.FromRgb(222, 00, 40));
				btn.Margin = new Thickness(4,4,4,4);
				btn.Click += RestartWinExplorer;
            }

			StatusBox.Text = status;
			if (btn != null)
            {
				StatusArea.Children.Add(btn);
            }
		}

		private void ClearStatus()
		{
			StatusBox.Text = "";
			// clear any prior button controls
			StatusArea.Children.OfType<Button>().ToList().ForEach(b => StatusArea.Children.Remove(b));
		}

		private void RestartWinExplorer(object sender, RoutedEventArgs e)
        {
			ShowStatus("Restarting");

			Process p = new Process();
            foreach (Process exe in Process.GetProcesses())
            {
                if (exe.ProcessName == "explorer")
                    exe.Kill();
            }
            Process.Start("explorer.exe");
        }

		private void FixClick(object sender, RoutedEventArgs e)
		{
			ClearStatus();
			FixerBox whichFix = (FixerBox)sender;
			Dictionary<string, string> theFix = fixers.GetFix(whichFix.Name);
			if (whichFix.IsFixed)
			{
				whichFix.btnOff();
				prefs.SetPref(theFix["PrefName"],"no");
				fixers.BreakIt(whichFix.Name);
				fixerBoxes[whichFix.Name].ClearDelta();
			}
			else
			{
				whichFix.btnOn();
				prefs.SetPref(theFix["PrefName"], "yes");
				fixers.FixIt(whichFix.Name);
				fixerBoxes[whichFix.Name].ClearDelta();
			}

			// Cheap way of saying, it's not blank
			string message;
			if (fixers.GetFix(whichFix.Name).TryGetValue("Activation_message",out message) == true && (message.Length > 3))
			{
				ShowStatus(ClearWS(message));
			}
		}

		// Since tags and filter text work together, this function checks both
		private void TagFilter()
		{
			bool isFilter = false;
			bool isTags = false;

			// Everything that does tags ends up here so use this function to operate the tag messages
			if (ActiveTags.Children.Count == 0)
			{
				TagGuideNone.Visibility = Visibility.Visible;
				TagGuideSome.Visibility = Visibility.Collapsed;
			}
			else
			{
				isTags = true;
				TagGuideNone.Visibility = Visibility.Collapsed;
				TagGuideSome.Visibility = Visibility.Visible;
			}

			if (Filter.Text.Length > 0 && (string)Filter.Tag != "placeholder")
				isFilter = true;

			HashSet<string> boxTags = new HashSet<string>();
			bool found;
			foreach (var aBox in fixerBoxes)
			{
				boxTags.Clear();
				// Make a test string full of tags for this box
				foreach (Button looking in aBox.Value.FixerTags.Children)
				{
					boxTags.Add((string)looking.Content);
				}

				// If it's missing any tags, it's false
				found = true;
				foreach (var checkTag in tags)
				{
					if (!boxTags.Contains(checkTag))
					{
						found = false;
					}
				}
				// If the filter has legit text in it
				if (isFilter)
				{
					bool inTitle = fixerBoxes[aBox.Key].FixerTitle.Text.Contains(Filter.Text);
					bool inText = fixerBoxes[aBox.Key].FixerDesc.Text.Contains(Filter.Text);
					if (!inTitle && !inText) found = false;
				}

				if (!found)
				{
					fixerBoxes[aBox.Key].Visibility = Visibility.Collapsed;
				}
				else
				{
					fixerBoxes[aBox.Key].Visibility = Visibility.Visible;
				}
			}

			// Handle the "clear" button
			if (isFilter || isTags)
				ClearButtonTxt.Text = "Clear Filter & Tags";
			else if (isTags && !isFilter)
				ClearButtonTxt.Text  = "Clear Tags";
			else if (!isTags && isFilter)
				ClearButtonTxt.Text  = "Clear Filter";
			else
				ClearButtonTxt.Text  = "Clear";
		}

		private void RemoveTag(object sender, RoutedEventArgs e)
		{
			Button TagClicked = (Button)sender;
			string ToRemove = TagClicked.Content.ToString();
			if (tags.Contains(ToRemove))
			{
				tags.Remove(ToRemove);
			}
			ActiveTags.Children.Remove(sender as Button);
			TagFilter();
		}

		private void tagClick(object sender, RoutedEventArgs e)
		{
			// What was the tag?
			Button whichBtn = (Button)sender;
			string lookingFor = (string) whichBtn.Content;
			if (!tags.Contains(lookingFor))
			{
				tags.Add(lookingFor);
				Button btn = null;
				btn = new Button();
				btn.Content = lookingFor;
				btn.Style = FindResource("LinkButton") as Style;
				btn.Foreground = new SolidColorBrush(Color.FromRgb(80, 200, 255));
				btn.HorizontalContentAlignment = HorizontalAlignment.Right;
				btn.Click += RemoveTag;
				ActiveTags.Children.Add(btn);
			}

			TagFilter();
		}

		// Takes all current fixes and makes a new prefs file from it
		private void MatchPrefs(object sender, RoutedEventArgs e)
		{
			ClearStatus();

			// In case we were in the middle of a load of new prefs, the load is complete once we match so toggle the buttons
			LoadPrefButton.Visibility = Visibility.Visible;
			CancelPrefButton.Visibility = Visibility.Collapsed;

			// Collect activation messages
			string messages = "";
			// throwaway to build the messages
			string message = "";
			foreach (var aBox in fixerBoxes)
			{
				FixerBox aFix = aBox.Value;

				if (aFix.DeltaFlag)
				{
					// trigger a click;
					FixClick(aFix, null);
					aFix.ClearDelta();
					// Cheap way of saying, it's not blank
					if (fixers.GetFix(aBox.Key).TryGetValue("Activation_message",out message) == true && (message.Length > 3))
					{
						messages += message;
					}
                }
			}
			// Tags just changed so update the view
			TagFilter();
			ShowStatus("Done! "+messages);
		}
		
		// Takes all current fixes and makes a new prefs file from it
		private void LoadPrefs(object sender, RoutedEventArgs e)
		{
			ClearStatus();

			// Create OpenFileDialog
			Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog(); 
       
			// Launch OpenFileDialog by calling ShowDialog method
			Nullable<bool> result = openFileDlg.ShowDialog();
			if (result == true)
			{
				File.Copy(prefs.settingsFullPath,prefs.settingsPath+"\\prefs.bak");
				File.Copy(openFileDlg.FileName, prefs.settingsFullPath);
				prefs.LoadPrefs();
				LoadPrefButton.Visibility = Visibility.Collapsed;
				CancelPrefButton.Visibility = Visibility.Visible;

				// Check for deltas vs the file
				var fixNames = fixers.FixerNames();
				foreach (string key in fixNames)
				{
					var temp = fixers.GetFix(key);
					fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "yes");
				}


				ShowStatus("Loaded. Click CANCEL to return to your previous prefs or Match Prefs File to apply the changes");
			}
			else
				ShowStatus("File could not be loaded!");
		}

		private void CancelPrefs(object sender, RoutedEventArgs e)
		{
			ClearStatus();
			LoadPrefButton.Visibility = Visibility.Visible;
			CancelPrefButton.Visibility = Visibility.Collapsed;
			File.Copy(prefs.settingsPath+"\\prefs.bak",prefs.settingsFullPath);
		}

		private void ClearFilter(object sender, RoutedEventArgs e)
		{
			ClearStatus();
		
			// Clear the text filter
			Filter.Clear();
			// Clear the list of tags visible in the UI
			ActiveTags.Children.Clear();
			// Clear the tags hashset we use to track active tags
			tags.Clear();

			TagFilter();
		}
		
		private void FixVisible(object sender, RoutedEventArgs e)
		{
			ClearStatus();

			// Collect activation messages
			string[] messages = new string[0];
			// throwaway to build the messages
			string message = "";
			foreach (var aBox in fixerBoxes)
			{
				// It's visible...
				if (fixerBoxes[aBox.Key].Visibility == Visibility.Visible)
                {
					// but it's not fixed
					if (!fixerBoxes[aBox.Key].IsFixed)
                    {
						// trigger a click;
						FixClick(fixerBoxes[aBox.Key], null);
						// Cheap way of saying, it's not blank
						if (fixers.GetFix(aBox.Key).TryGetValue("Activation_message",out message) == true && (message.Length > 3))
						{
							messages.Append(message);
						}
					}

				}
			}
			// turn off for a sec to test multi-line messages
			//if (messages.Length > 0)
			//	messages = messages.Distinct().ToArray();
			messages.Append("Done!");
			MultiStatus(messages);
		}

		// Used to check all our fixes on a pulse. If something changes in either the prefs file or the registry, this will light it up
		public void WatchDeltas(object source, ElapsedEventArgs e)
        {
			// Grab the list of defined fixers from the fixers class.
			var fixNames = fixers.FixerNames();
			foreach (string key in fixNames)
			{
				var temp = fixers.GetFix(key);

				// Check to see if it's already active or not
				if (fixers.IsFixed(key))
				{
					// if the box isn't already showing fixed, change it
					this.Dispatcher.Invoke(() =>
					{
						if (!fixerBoxes[key].IsFixed)
							fixerBoxes[key].btnOn();
						fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "yes");
					});
				}
				else
				{
					// if the box is showing as fixed when, in reality, it isn't, change that
					this.Dispatcher.Invoke(() =>
					{
						if (fixerBoxes[key].IsFixed)
							fixerBoxes[key].btnOff();
						fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "no");
					});
				}
			}

			// *************
			// Numlock stuff
				bool NumLock = (GetKeyState(0x90) & 0x0001) != 0;

				if (fixers.watchNumL && !NumLock)
				{
					// Force NumLock back on
					// Simulate a key press
					Interop.keybd_event((byte)0x90,0x45,Interop.KEYEVENTF_EXTENDEDKEY | 0,IntPtr.Zero);

					// Simulate a key release
					Interop.keybd_event((byte)0x90,0x45,Interop.KEYEVENTF_EXTENDEDKEY | Interop.KEYEVENTF_KEYUP,	IntPtr.Zero);
				}

				if (NumLock)
				{
					this.Dispatcher.Invoke(() =>
					{
						fixerBoxes["NumL"].FixerImg.Source = new BitmapImage(new Uri(@"/graphics/num_lock_on.png", UriKind.Relative));
					});
				}
				else {
					this.Dispatcher.Invoke(() =>
					{
						fixerBoxes["NumL"].FixerImg.Source = new BitmapImage(new Uri(@"/graphics/num_lock_off.png", UriKind.Relative));
					});
				}
        }

		// Loads fixers into the window. Determines their status and whether they are matched to the prefs
		public void addFixers()
		{
			// Grab the list of defined fixers from the fixers class.
			var fixNames = fixers.FixerNames();

			// Take care of watchers first so the tile has the correct infomration about whether it's active or not.
			if (prefs.GetPref("ForceNumLockAlwaysOn") == "yes")
			{
				// Set the flag that says num lock should be on
				fixers.watchNumL = true;
			}

			// For each fixer, initialize a custom control and store it in a dictionary by fixer name for easy access
			foreach (string key in fixNames)
			{
				var temp = fixers.GetFix(key);
				// Be sure to pass this along as well
				temp["Name"] = key;
				fixerBoxes[key] = new FixerBox(temp);
				// Makes it findable by name
				FixersArea.Children.Add(fixerBoxes[key]);
				// Direct click events to our mainfile function
				fixerBoxes[key].toggleClick += FixClick;
				// Directs tag clicks
				fixerBoxes[key].tagClick += tagClick;
				// Image update
				fixerBoxes[key].FixerImg.Source = new BitmapImage(new Uri(temp["Img"], UriKind.Relative));
			}
			WatchDeltas(null, null);
		}

		internal partial class Interop
		{
			public static int VK_NUMLOCK = 0x90;
			public static int VK_SCROLL = 0x91;
			public static int VK_CAPITAL = 0x14;
			public static int KEYEVENTF_EXTENDEDKEY = 0x0001; // If specified, the scan code was preceded by a prefix byte having the value 0xE0 (224).
			public static int KEYEVENTF_KEYUP = 0x0002; // If specified, the key is being released. If not specified, the key is being depressed.

			[DllImport("User32.dll", SetLastError = true)]
			public static extern void keybd_event(
				byte bVk,
				byte bScan,
				int dwFlags,
				IntPtr dwExtraInfo);

			[DllImport("User32.dll", SetLastError = true)]
			public static extern short GetKeyState(int nVirtKey);

			[DllImport("User32.dll", SetLastError = true)]
			public static extern short GetAsyncKeyState(int vKey);
		}


		private void SetPlaceholder(object sender, RoutedEventArgs e)
		{
			if (Filter.Text.Length == 0)
			{
				Filter.Tag = "placeholder";
				Filter.Text = "Filter fixes";
				Filter.Foreground = new SolidColorBrush(Color.FromRgb(155, 155, 155));
			}
		}

		// If filter gains focus and doesn't have custom data in it, clear the placeholder
		private void ClearPlaceholder(object sender, RoutedEventArgs e)
		{
			// We have it flagged as a placeholder, so clear that and set the color to normal
			if ((string) Filter.Tag == "placeholder")
			{
				Filter.Foreground = new SolidColorBrush(Color.FromRgb(44, 44, 44));
				Filter.Text = "";
				Filter.Tag = "";
			}
		}

		private void FilterFixes(object sender, RoutedEventArgs e)
		{
			TagFilter();
		}

		public MainWindow()
		{
			InitializeComponent();
			// Had to do it here because it needs this path, but we couldn't use the regstuff var in the initializers area (shrug)
			prefs = new Prefs(regStuff.MyDocsPath());

			addFixers();

			// Start the watcher
			System.Timers.Timer deltaTimer = new System.Timers.Timer();
			// Tell the timer what to do when it elapses
			deltaTimer.Elapsed += new ElapsedEventHandler(WatchDeltas);
			// Set it to go off every second
			deltaTimer.Interval = 10000;
			// And start it        
			deltaTimer.Enabled = true;

			// Force the filter box placeholder info (since WPF doesn't think placeholders are necessary or useful... apparently)
			SetPlaceholder(null, null);

		}
	}
}
