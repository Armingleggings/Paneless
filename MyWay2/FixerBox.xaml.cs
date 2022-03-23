using Paneless.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paneless
{
	/// <summary>
	/// Interaction logic for FixerBox.xaml
	/// </summary>
	public partial class FixerBox : UserControl
	{

		public event RoutedEventHandler toggleClick;
		public event RoutedEventHandler tagClick;
		// Cheating probably... define a class-wide var for the "pref file is mismatched" tag that comes and goes. Holds an instance of a button so we can remove it later.
		private Button PrefAlertBtn;
		public bool DeltaFlag { get; set; } = false;

		public bool IsFixed { get; set; } = false;
		// Not in the XAML, but we need it
		public string PrefName { get; set; } = "";
	
		public FixerBox(Dictionary<string,string> deets)
		{
			InitializeComponent();
			btnOff();
			this.Name = deets["Name"];
			this.PrefName = deets["PrefName"];
			this.FixerTitle.Text = deets["Title"];
			// This is set later depending on snarklevel
			this.FixerDesc.Text = "";
			var temp = deets["Tags"].Split(',');
			Button btn = null;
			// remove placeholder
			this.FixerTags.Children.Clear();
			foreach (var tag in temp)
			{
				btn = new Button();
				btn.Content = (tag).Trim();
				btn.Style = FindResource("LinkButton") as Style;
				btn.Foreground = new SolidColorBrush(Color.FromRgb(125,125,125));
				btn.Click += this.TagClick;
				this.FixerTags.Children.Add(btn);
			}
		}

		public void SetDesc(string desc)
		{
			this.FixerDesc.Text = desc;
		}

		public void btnOn() 
		{
			IsFixed = true;
			FixedButton.Visibility = Visibility.Visible;
			FixButton.Visibility = Visibility.Hidden;
		}
		public void btnOff() 
		{
			IsFixed = false;
			FixButton.Visibility = Visibility.Visible;
			FixedButton.Visibility = Visibility.Hidden;
		}

		// Given a prefs array where the key is this fix's name, compare the "isfixed" versus the pref and show mismatch if there is one
		// If delta, adds a tag otherwise removes (NO DON"T REMOVE - WE NEED IT TO STAY FOR THE LOAD PREFS THING)
		public void DeltaCheck(Dictionary<string,string> prefs)
		{
			// If we set it already, don't bother again (save cycles)
			if (DeltaFlag) return;
			// Sometimes we don't have a preference. If it's empty, we don't care and shouldn't mark it
			if (!prefs.ContainsKey(PrefName)) return;

			// If our current state doesn't match the pref file
			if (prefs[PrefName] != (IsFixed?"yes":"no"))
			{
				PrefAlertBtn = new Button();
				PrefAlertBtn.Content = "#PrefFileMismatch";
				PrefAlertBtn.Name = "mismatchTag";
				PrefAlertBtn.Style = FindResource("LinkButton") as Style;
				PrefAlertBtn.Foreground = new SolidColorBrush(Color.FromRgb(153, 0, 0));
				PrefAlertBtn.Click += TagClick;
				FixerTags.Children.Add(PrefAlertBtn);
				DeltaFlag = true;
			}
		}

		// When updating preferences, clear deltas.
		public void ClearDelta()
		{
			DeltaFlag = false;
			FixerTags.Children.Remove(PrefAlertBtn);
		}

		private void FixBtnClick(object sender, RoutedEventArgs e)
		{
			// Don't send what htey clicked (sender), send the entire fixerbox (which is this)
			toggleClick?.Invoke(this, e);
		}

		private void TagClick(object sender, RoutedEventArgs e)
		{
			tagClick?.Invoke(sender, e);
		}
	}
}
