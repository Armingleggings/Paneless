using Paneless.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Media.Animation;

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
		// Holds onto tags for filtering - hashset is like an array, but the values are unique apparently so adding the same one does nothing (behavior that helps for what we're doing)
		private HashSet<string> tags = new HashSet<string>();

		// For numlock
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		public static extern short GetKeyState(int keyCode);

		// Create a dictionary to hold our various controls.
		private Dictionary<string, FixerBox> fixerBoxes = new Dictionary<string, FixerBox> { };

		private string ClearWS(string str)
		{
			string temp = str.Replace("\t", "");
			return temp;
		}

		// Because it's complex trying to make status work properly with just strings, we need a translation function to take a string and make any needed changes
		private WrapPanel StatusMessage(string status)
		{
			Button btn = null;
			WrapPanel pnl = new WrapPanel();

			// There's probably a much cleaner way of doing this. Oh well...
			if (status.Contains("#RestartWinExplorer"))
			{
				status.Replace("#RestartWinExplorer", "");
				btn = new Button();
				btn.Content = "(Click here to retstart Windows Explorer now)";
				btn.Style = FindResource("LinkButton") as Style;
				btn.Foreground = new SolidColorBrush(Color.FromRgb(198, 00, 40));
				btn.Margin = new Thickness(4, 4, 4, 4);
				btn.Click += RestartWinExplorer;
			}

			Label lbl = new Label();
			lbl.Content = status;
			lbl.Foreground = new SolidColorBrush(Color.FromRgb(38, 98, 150));
			lbl.FontWeight = FontWeights.Bold;
			pnl.Children.Add(lbl);

			if (btn != null)
			{
				pnl.Children.Add(btn);
			}
			return pnl;
		}

		private void PulseStatus()
		{	

			ColorAnimation animation;
			animation = new ColorAnimation();
			animation.To = Color.FromRgb(40, 111, 60);
			animation.Duration = new Duration(TimeSpan.FromSeconds(.2));
			animation.AutoReverse = true;
 
			StatusArea.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		}
		// At times we'll need to show multiple status messages at once. Make sure they're readable.
		private void MultiStatus(List<string> messages)
		{
			ClearStatus();
			PulseStatus();

			foreach (string message in messages)
			{
				StatusArea.Children.Add(StatusMessage("* "+message));
			}
		}

		// Show a single status message
		private void ShowStatus(string status)
		{
			ClearStatus();
			PulseStatus();

			StatusArea.Children.Add(StatusMessage(status));
		}

		private void ClearStatus()
		{
			StatusArea.Children.Clear();
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
				prefs.SavePrefs();

				fixers.BreakIt(whichFix.Name);
				fixerBoxes[whichFix.Name].ClearDelta();
			}
			else
			{
				whichFix.btnOn();
				prefs.SetPref(theFix["PrefName"], "yes");
				prefs.SavePrefs();

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
			List<string> messages = new List<string>();
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
						messages.Add(message);
				}
			}
			// Tags just changed so update the view
			TagFilter();
			messages.Add("Done!");
			MultiStatus(messages);
		}
		
		// Loads prefs and applies (or removes) any delta tags
		private void DeltaCheckAll()
		{
			prefs.LoadPrefs();
			var fixNames = fixers.FixerNames();
			foreach (string key in fixNames)
			{
				var temp = fixers.GetFix(key);
				fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "yes");
			}
		}

		// Loads a user-custom prefs file with various settings. Good for "Win11 suite" or "Explorer only" customizations.
		private void LoadPrefs(object sender, RoutedEventArgs e)
		{
			ClearStatus();

			// Create OpenFileDialog
			Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog(); 
	   
			// Launch OpenFileDialog by calling ShowDialog method
			Nullable<bool> result = openFileDlg.ShowDialog();
			if (result == true)
			{
				if (openFileDlg.FileName == prefs.settingsFullPath)
				{
					ShowStatus("Load prefs is for opening custom and alternate prefs files, not the default PREFS.TXT file. ");
					return;
				}

				// Copy commands can't overwrite so have ot start out by deleting.
				if (File.Exists(prefs.settingsPath + "\\prefs.bak.txt"))
					File.Delete(prefs.settingsPath + "\\prefs.bak.txt");

				// Assuming a prefs file is there...
				if (File.Exists(prefs.settingsFullPath))
				{
					File.Copy(prefs.settingsFullPath, prefs.settingsPath + "\\prefs.bak.txt");
					File.Delete(prefs.settingsFullPath);
				}
				File.Copy(openFileDlg.FileName, prefs.settingsFullPath);
				LoadPrefButton.Visibility = Visibility.Collapsed;
				CancelPrefButton.Visibility = Visibility.Visible;

				DeltaCheckAll();

				ShowStatus("Loaded. Click CANCEL to return to your previous prefs or MATCH PREFS FILE to apply the changes");
			}
			else
				ShowStatus("File could not be loaded!");
		}

		private void CancelPrefs(object sender, RoutedEventArgs e)
		{
			ClearStatus();
			LoadPrefButton.Visibility = Visibility.Visible;
			CancelPrefButton.Visibility = Visibility.Collapsed;
			// Remove the temp prefs file
			if (File.Exists(prefs.settingsFullPath))
				File.Delete(prefs.settingsFullPath);

			// If we had one before, put it back
			if (File.Exists(prefs.settingsPath + "\\prefs.bak.txt"))
				File.Copy(prefs.settingsPath+"\\prefs.bak.txt",prefs.settingsFullPath);

			DeltaCheckAll();
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
		
		// Looks through the fixers and matches our prefs to whatever we see (used for saving and for first-load backup)
		private void ScrapePrefs()
		{
			var fixNames = fixers.FixerNames();
			string yesNo = "";
			foreach (string key in fixNames)
			{
				var temp = fixers.GetFix(key);
				yesNo = fixerBoxes[key].IsFixed ? "yes" : "no";
				prefs.SetPref(temp["PrefName"],yesNo);
			}
		}

		// Snark levels - Want to have some fun, but not offend anyone so you can choose what level of snark to use.
		private void FullSnark(object sender, RoutedEventArgs e)
		{
			ShowStatus("Let's face it: Microsoft decisions for some controls are just bone-headed. No need to pull punches, right?");
			foreach (var aBox in fixerBoxes)
				aBox.Value.FixerDesc.Text = fixers.GetFix(aBox.Key)["Snark"];
		}

		private void NoSnark(object sender, RoutedEventArgs e)
		{
			ShowStatus("Minimal snark descriptions activated.");
			foreach (var aBox in fixerBoxes)
				aBox.Value.FixerDesc.Text = fixers.GetFix(aBox.Key)["Description"];
		}

		// There were too many instance where I wanted to save all visible buttons in the positions they were in so I added a save button
		private void SaveAll(object sender, RoutedEventArgs e)
		{
			ScrapePrefs();
			prefs.SavePrefs();
			ShowStatus("All current fix settings saved to prefs file.");
		}

		// There were too many instance where I wanted to save all visible buttons in the positions they were in so I added a save button
		private void BackupPrefs(object sender, RoutedEventArgs e)
		{
			prefs.BackupPrefs();
			ShowStatus("Backed up the Prefs.txt file. The new file can be found in the same folder as Prefs.txt");
		}

		private void FixVisible(object sender, RoutedEventArgs e)
		{
			ClearStatus();

			// Collect activation messages
			List<string> messages = new List<string>();
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
							messages.Add(message);
						}
					}

				}
			}
			messages.Add("Done!");
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
			//DeltaCheckAll();
		}

		// Loads fixers into the window. Determines their status and whether they are matched to the prefs
		public void AddFixers()
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

			AddFixers();

			// No prefs file? Is this the first load situation? Either way, make one and a backup of the current state
			// Have to do this after AddFixers() or stuff breaks
			if (!prefs.PrefsFileExists())
			{
				// Save all current stuff to prefs dictionary
				ScrapePrefs();
				// Now create a backup
				prefs.BackupPrefs();
			}

			// Start the watcher
			Timer deltaTimer = new Timer();
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
