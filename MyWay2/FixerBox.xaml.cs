using Paneless.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

		public bool IsFixed { get; set; } = false;
		
		public FixerBox(Dictionary<string,string> deets)
		{
			InitializeComponent();
			btnOff();
			this.Name = deets["Name"];
			this.FixerTitle.Text = deets["Title"];
			this.FixerDesc.Text = deets["Description"];
			//this.FixerTags.Text = deets["Tags"];
			this.FixerImg.Source = new BitmapImage(new Uri(deets["Img"], UriKind.Relative));
			this.FixerDeltaTag.Visibility = Visibility.Collapsed;
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

		public void DeltaCheck(string savedPref, string testState)
		{
			// IF a preference exists and matches the current system state...
			if (savedPref == testState)
			{
				FixBtnArea.Background = new SolidColorBrush(Color.FromRgb(195, 195, 195));
				this.FixerDeltaTag.Visibility = Visibility.Collapsed;
			}
			// ... but if the pref is mismatched or missing...
			else
			{
				FixBtnArea.Background = new SolidColorBrush(Color.FromRgb(158, 147, 128));
				this.FixerDeltaTag.Visibility = Visibility.Visible;
			}
		}

		private void FixBtnClick(object sender, MouseButtonEventArgs e)
		{
			toggleClick?.Invoke(this, new RoutedEventArgs());
		}

		private void TagClick(object sender, MouseButtonEventArgs e)
		{
			tagClick?.Invoke(this, new RoutedEventArgs());
		}
	}
}
