using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scoreboard {
	/// <summary>
	/// Interaction logic for GameInfoBanner.xaml
	/// </summary>
	public partial class GameInfoBanner : Window {
		public GameInfoBanner() {
			InitializeComponent();
		}

		public void setTime(string time) {
			Clock.Text = time;
		}

		public void setPeriod(string period) {
			Period.Text = period;
		}

		public void setEvenStrength(string penaltyInfo) {
			if (penaltyInfo != "") {
				EvenStrengthBackground.Visibility = System.Windows.Visibility.Visible;
				EvenStrength.Text = penaltyInfo;
			}
			else {
				EvenStrengthBackground.Visibility = System.Windows.Visibility.Hidden;
				EvenStrength.Text = penaltyInfo;
			}
		}

		public void setHomeName(string name) {
			HomeName.Text = name;
		}

		public void setHomeScore(int score) {
			HomeScore.Text = score.ToString();
		}

		public void setHomeAdvantage(string penaltyInfo) {
			if (penaltyInfo != "") {
				HomeAdvantageBackground.Visibility = System.Windows.Visibility.Visible;
				HomeAdvantage.Text = penaltyInfo;
			}
			else {
				HomeAdvantageBackground.Visibility = System.Windows.Visibility.Hidden;
				HomeAdvantage.Text = penaltyInfo;
			}
		}

		public void setAwayName(string name) {
			AwayName.Text = name;
		}

		public void setAwayScore(int score) {
			AwayScore.Text = score.ToString();
		}

		public void setAwayAdvantage(string penaltyInfo) {
			if(penaltyInfo != "") {
				AwayAdvantageBackground.Visibility = System.Windows.Visibility.Visible;
				AwayAdvantage.Text = penaltyInfo;
			}
			else {
				AwayAdvantageBackground.Visibility = System.Windows.Visibility.Hidden;
				AwayAdvantage.Text = penaltyInfo;
			}
		}


	}
}
