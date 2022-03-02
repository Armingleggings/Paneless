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
		Thickness btnDotOn = new Thickness(-60, 0, 0, 0);
		Thickness btnDotOff = new Thickness(0, 0, -60, 0);
		SolidColorBrush dotOn = new SolidColorBrush(Color.FromRgb(84, 130, 53));
		SolidColorBrush dotOff = new SolidColorBrush(Color.FromRgb(207, 178, 76));
		SolidColorBrush bkOn = new SolidColorBrush(Color.FromRgb(226, 240, 217));
		SolidColorBrush bkOff = new SolidColorBrush(Color.FromRgb(255, 242, 204));

		public event RoutedEventHandler toggleClick;
		public event RoutedEventHandler tagClick;
		// Cheating probably... define a class-wide var for the "pref file is mismatched" tag that comes and goes. Holds an instance of a button so we can remove it later.
		private Button PrefAlertBtn;
		public bool DeltaFlag { get; set; } = false;

		public bool IsFixed { get; set; } = false;
		
		public FixerBox(Dictionary<string,string> deets)
		{
			InitializeComponent();
			btnOff();
			this.Name = deets["Name"];
			this.FixerTitle.Text = deets["Title"];
			this.FixerDesc.Text = deets["Description"];
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

		public void btnOn() 
		{
			IsFixed = true;
			FixBtnBk.Fill = bkOn;
			FixBtnBk.Stroke = dotOn;
			FixBtnDot.Fill = dotOn;
			FixBtnDot.Margin = btnDotOn;
			FixBtnTxt.Visibility = Visibility.Visible;
		}
		public void btnOff() 
		{
			IsFixed = false;
			FixBtnBk.Fill = bkOff;
			FixBtnBk.Stroke = dotOff;
			FixBtnDot.Fill = dotOff;
			FixBtnDot.Margin = btnDotOff;
			FixBtnTxt.Visibility = Visibility.Hidden;
		}

		// Given a saved pref and a current test state, does the test state match that saved value
		// If delta, adds a tag otherwise removes
		public void DeltaCheck(string savedPref, string testState)
		{
			// If our current state doesn't match the pref file
			if (savedPref != "" && savedPref != testState && !DeltaFlag)
			{
				PrefAlertBtn = new Button();
				PrefAlertBtn.Content = "#PrefFileMismatch";
				PrefAlertBtn.Name = "mismatchTag";
				PrefAlertBtn.Style = FindResource("LinkButton") as Style;
				PrefAlertBtn.Foreground = new SolidColorBrush(Color.FromRgb(153, 0, 0));
				PrefAlertBtn.Click += this.TagClick;
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

		private async void FixBtnClick(object sender, RoutedEventArgs e)
		{
			try
			{
			// Don't send what htey clicked (sender), send the entire fixerbox (which is this)
				toggleClick?.Invoke(this, e);
			}
			catch(Exception ex)
			{
			}
		}

		private Type GetType(object sender)
		{
			throw new NotImplementedException();
		}

		private void TagClick(object sender, RoutedEventArgs e)
		{
			tagClick?.Invoke(sender, e);
		}
	}
}
