﻿using Paneless.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
		private Dictionary<string,FixerBox> fixerBoxes = new Dictionary<string, FixerBox> { };

		private string ClearWS(string str)
		{
			string temp = str.Replace("\t", "");
			return temp;
		}

		private void ShowStatus(string status)
        {
			StatusBox.Content = status;
        }

		private void FixClick(object sender, RoutedEventArgs e)
		{
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
			// Everything that does tags ends up here so use this function to operate the tag messages
			if (ActiveTags.Children.Count == 0)
			{
				TagGuideNone.Visibility = Visibility.Visible;
				TagGuideSome.Visibility = Visibility.Collapsed;
			}
			else
			{
				TagGuideNone.Visibility = Visibility.Collapsed;
				TagGuideSome.Visibility = Visibility.Visible;
			}

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
				if (Filter.Text.Length > 0 && (string)Filter.Tag != "placeholder")
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
				btn.HorizontalAlignment = HorizontalAlignment.Center;
				btn.Click += RemoveTag;
				ActiveTags.Children.Add(btn);
			}

			TagFilter();
		}
	
		private void ClearFilter(object sender, RoutedEventArgs e)
        {
			Filter.Clear();
			TagFilter();
        }

		// Loads fixers into the window. Determines their status and whether they are matched to the prefs
		public void addFixers()
		{
			// Grab the list of defined fixers from the fixers class.
			var fixNames = fixers.FixerNames();

			// Take care of watchers first so the tile has the correct infomration about whether it's active or not.
			if (prefs.GetPref("ForceNumLockAlwaysOn") == "yes")
			{
				// If the fix for Num lock is active, start watching to keep it that way
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
				// Check to see if it's already active or not
				if (fixers.IsFixed(key))
				{
					fixerBoxes[key].btnOn();
					fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "yes");
				}
				else
				{
					fixerBoxes[key].btnOff();
					fixerBoxes[key].DeltaCheck(prefs.GetPref(temp["PrefName"]), "no");
				}
			}
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


		// Implement a call with the right signature for events going off
		private void watcher(object source, ElapsedEventArgs e) 
		{
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
			System.Timers.Timer myTimer = new System.Timers.Timer();
			// Tell the timer what to do when it elapses
			myTimer.Elapsed += new ElapsedEventHandler(watcher);
			// Set it to go off every second
			myTimer.Interval = 1000;
			// And start it        
			myTimer.Enabled = true;

			// Force the filter box placeholder info (since WPF doesn't think placeholders are necessary or useful... apparently)
			SetPlaceholder(null, null);
		}

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
