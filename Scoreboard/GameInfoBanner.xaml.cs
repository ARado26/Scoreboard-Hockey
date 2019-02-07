using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Scoreboard {
	/// <summary>
	/// Interaction logic for GameInfoBanner.xaml
	/// </summary>
	public partial class GameInfoBanner : Window {

		private int homeColumn;
		private int awayColumn;

		public GameInfoBanner() {
			InitializeComponent();
			ClockBackground.Source = filepathToImage(@".\Images\Clock_Cell.png");
			BitmapImage bmp = filepathToImage(@".\Images\Penalty_Cell.png");
			EvenStrengthBackground.Source = bmp;
			HomeAdvantageBackground.Source = bmp;
			AwayAdvantageBackground.Source = bmp;
			homeColumn = 0;
			awayColumn = 2;
		}

		public void setTime(string time) {
			Clock.Text = time;
		}

		public void setPeriod(string period) {
			Period.Text = period;
		}

		public void setEvenStrength(string penaltyInfo) {
			EvenStrength.Text = penaltyInfo;
			if (penaltyInfo == "") {
				EvenStrengthBackground.Visibility = System.Windows.Visibility.Hidden;
			}
			else {
				EvenStrengthBackground.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void setHomeName(string name) {
			HomeName.Text = name;
		}

		public void setHomeScore(int score) {
			HomeScore.Text = score.ToString();
		}

		public void setHomeAdvantage(string penaltyInfo) {
			HomeAdvantage.Text = penaltyInfo;
			if (penaltyInfo == "") {
				HomeAdvantageBackground.Visibility = System.Windows.Visibility.Hidden;
			}
			else {
				HomeAdvantageBackground.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void setAwayName(string name) {
			AwayName.Text = name;
		}

		public void setAwayScore(int score) {
			AwayScore.Text = score.ToString();
		}

		public void setAwayAdvantage(string penaltyInfo) {
			AwayAdvantage.Text = penaltyInfo;
			if (penaltyInfo == "") {
				AwayAdvantageBackground.Visibility = System.Windows.Visibility.Hidden;
			}
			else {
				AwayAdvantageBackground.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void swapBannerPositions() {
			Grid.SetColumn(HomeBackground, awayColumn);
			Grid.SetColumn(HomeGrid, awayColumn);
			Grid.SetColumn(HomeAdvGrid, awayColumn);
			Grid.SetColumn(AwayBackground, homeColumn);
			Grid.SetColumn(AwayGrid, homeColumn);
			Grid.SetColumn(AwayAdvGrid, homeColumn);
			int tmp = homeColumn;
			homeColumn = awayColumn;
			awayColumn = tmp;
		}

		private BitmapImage filepathToImage(string filepath) {
			string fullpath = Path.GetFullPath(filepath);
			Uri uriPath = new Uri(fullpath);
			BitmapImage bmp = new BitmapImage(uriPath);
			if (imageHasCorrectDimensions(bmp)) {
				return bmp;
			}
			else {
				return null;
			}
		}

		private bool imageHasCorrectDimensions(BitmapImage bmp) {
			double desiredAspect = 5;
			double imgAspect = (double)bmp.PixelWidth / (double)bmp.PixelHeight;
			return desiredAspect == imgAspect;
		}

	}
}
