using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Scoreboard {
	/// <summary>
	/// Interaction logic for GameInfoBanner.xaml
	/// </summary>
	public partial class GameInfoBanner : Window {

		
		private int homeColumn;
		private int awayColumn;
		private bool movable;

		private double lastScale;
		private const int HEIGHT = 160;
		private const int WIDTH = 1200;
		private const int FONT_SIZE = 50;
		private const double MAX_SCALE = 3.4133; //4k Resolution max
		private const double MIN_SCALE = 0.5;

		public GameInfoBanner() {
			InitializeComponent();
			ClockBackground.Source = filepathToImage(@".\Images\Clock_Cell.png");
			BitmapImage bmp = filepathToImage(@".\Images\Penalty_Cell.png");
			EvenStrengthBackground.Source = bmp;
			HomeAdvantageBackground.Source = bmp;
			AwayAdvantageBackground.Source = bmp;
			homeColumn = 0;
			awayColumn = 2;
			lastScale = 1;
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

		public void setMovable(bool flag) {
			movable = flag;
			if (movable) {
				this.Cursor = Cursors.SizeAll;
			}
			else {
				this.Cursor = Cursors.No;
			}
		}

		public void setScale(double scale) {
			if(scale > MAX_SCALE) {
				scale = MAX_SCALE;
			}else if(scale < MIN_SCALE) {
				scale = MIN_SCALE;
			}
			this.Height = HEIGHT * scale;
			this.Width = WIDTH * scale;
			double newFontSize = FONT_SIZE * scale;
			Clock.FontSize = newFontSize;
			Period.FontSize = newFontSize;
			EvenStrength.FontSize = newFontSize;
			HomeName.FontSize = newFontSize;
			HomeScore.FontSize = newFontSize;
			HomeAdvantage.FontSize = newFontSize;
			AwayName.FontSize = newFontSize;
			AwayScore.FontSize = newFontSize;
			AwayAdvantage.FontSize = newFontSize;
			
			foreach (ColumnDefinition definition in GameInfoGrid.ColumnDefinitions) {
				if (definition.Width.IsAbsolute) {
					double original = definition.Width.Value / lastScale;
					double newWidth = original * scale;
					Console.WriteLine(original);
					Console.WriteLine(newWidth);
					definition.Width = new GridLength(newWidth);
				}
			}

			foreach (ColumnDefinition definition in HomeGrid.ColumnDefinitions) {
				if (definition.Width.IsAbsolute) {
					double original = definition.Width.Value / lastScale;
					double newWidth = original * scale;
					Console.WriteLine(original);
					Console.WriteLine(newWidth);
					definition.Width = new GridLength(newWidth);
				}
			}

			foreach (ColumnDefinition definition in AwayGrid.ColumnDefinitions) {
				if (definition.Width.IsAbsolute) {
					double original = definition.Width.Value / lastScale;
					double newWidth = original * scale;
					Console.WriteLine(original);
					Console.WriteLine(newWidth);
					definition.Width = new GridLength(newWidth);
				}
			}

			lastScale = scale;
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


		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (movable) {
				this.DragMove();
			}
		}
	}
}
