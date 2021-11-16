using Paneless.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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


		// Create a dictionary to hold our various controls.
		private Dictionary<string,FixerBox> fixerBoxes = new Dictionary<string, FixerBox> { };

		private string ClearWS(string str)
		{
			string temp = str.Replace("\t", "");
			return temp;
		}

		private void fixClick(object sender, RoutedEventArgs e)
		{
			FixerBox whichFix = (FixerBox)sender;
			Dictionary<string, string> theFix = fixers.GetFix(whichFix.Name);
			if (whichFix.IsFixed)
			{
				whichFix.btnOff();
				prefs.SetPref(theFix["PrefName"],"no");
				fixers.BreakIt(whichFix.Name);
				StatusBox.Text = ClearWS($@"
					Fix for {whichFix.Name} applied.
					Preference for {theFix["PrefName"]} saved.
				");
				fixerBoxes[whichFix.Name].DeltaCheck(prefs.GetPref(theFix["PrefName"]), "no");
			}
			else
			{
				whichFix.btnOn();
				prefs.SetPref(theFix["PrefName"], "yes");
				fixers.FixIt(whichFix.Name);
				StatusBox.Text = ClearWS($@"
					Fix for  <Run FontWeight=""Bold"">more bold text</Run>{whichFix.Name} removed.
					Preference for {theFix["PrefName"]} saved.
				");
				fixerBoxes[whichFix.Name].DeltaCheck(prefs.GetPref(theFix["PrefName"]), "yes");
			}

		}

		private void tagClick(object sender, RoutedEventArgs e)
		{
			StatusBox.Text = "Clicked tag";
		}

		// Used to determine if the fixes are applied. Run once at start and every now and then afterward (to make sure things haven't changed).
		public void StatusCheck()
		{
			// Grab the list of defined fixers from the fixers class.
			var fixNames = fixers.FixerNames();
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
				fixerBoxes[key].toggleClick += fixClick;
				// Directs tag clicks
				fixerBoxes[key].tagClick += tagClick;
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

		public MainWindow()
		{
			InitializeComponent();
			// Had to do it here because it needs this path, but we couldn't use the regstuff var in the initializers area (shrug)
			prefs = new Prefs(regStuff.MyDocsPath());

			StatusCheck();
		}
	}
}
