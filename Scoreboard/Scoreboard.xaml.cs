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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scoreboard
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		public HockeyTeam homeTeam { get; set; }
		public HockeyTeam awayTeam { get; set; }
		public GameInfo gameInfo { get; set; }
		public GameTimer timer { get; set; }
		public System.Timers.Timer passiveTimer { get; set; }

		private enum CLOCK_STATES{ STOPPED, RUNNING }
		private CLOCK_STATES clockState;
		private const int REFRESH_INTERVAL = 100;


        public MainWindow()
        {
            InitializeComponent();

			init();
		}

		public void init() {
			gameInfo = new GameInfo();
			homeTeam = new HockeyTeam("HOME");
			awayTeam = new HockeyTeam("AWAY");
			timer = new GameTimer(REFRESH_INTERVAL);
			timer.TimeExpired += HandleTimeExpiration;
			passiveTimer = new System.Timers.Timer(REFRESH_INTERVAL);
			

			timer.setTimerFields(gameInfo, homeTeam, awayTeam);

			HomeNameTextBox.Text = homeTeam.name;
			HomeScore.Text = homeTeam.score.ToString();

			AwayNameTextBox.Text = awayTeam.name;
			AwayScore.Text = awayTeam.score.ToString();

			ClockToggleButton.Content = "Start";
			clockState = CLOCK_STATES.STOPPED;

			HomeGoaliePulled.IsChecked = false;
			AwayGoaliePulled.IsChecked = false;

			homeTeam.goaliePulled = false;
			awayTeam.goaliePulled = false;

			setPenaltyInformation();

		}

		public void printInfo() {
			gameInfo.printInfo();
			homeTeam.printInfo();
			awayTeam.printInfo();
			Console.WriteLine("Clock Mode: " + clockState);
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			Keyboard.ClearFocus();
		}

		//=================================================
		// Game Info Methods
		//=================================================

		private void ClockToggleButton_Click(object sender, RoutedEventArgs e)
        {
			
			
			if (clockState == CLOCK_STATES.STOPPED) {
				timer.setTimerFields(gameInfo, homeTeam, awayTeam);
				timer.startClock();
				clockState = CLOCK_STATES.RUNNING;
			}
			else {
				timer.stopClock();
				extractTimerFields();
				clockState = CLOCK_STATES.STOPPED;
			}

			toggleClockButtonText();
			toggleClockTextBoxElements();

			debug.Text = "Toggle Clock Mode: " + clockState;
        }

        private void ClockSetButton_Click(object sender, RoutedEventArgs e)
        {
			string minutesString = GameClockMinutes.Text;
			string secondsString = GameClockSeconds.Text;
			gameInfo.setGameTime(minutesString, secondsString);
			GameClockMinutes.Text = gameInfo.minutes.ToString();
			string s = gameInfo.seconds.ToString();
			GameClockSeconds.Text = (s.Length > 1 ? s : '0' + s);
            debug.Text = "Setting Clock: " + gameInfo.minutes + ':' + (s.Length > 1 ? s : '0' + s);

        }

        private void GamePeriodPicker_SelectionChanged(object sender, SelectionChangedEventArgs e){
            GamePeriod.Text = ((ComboBoxItem) GamePeriodPicker.SelectedItem).Content.ToString();
        }

		private void GamePeriodSetter_Click(object sender, RoutedEventArgs e) {
			debug.Text = "Period Set: " + GamePeriod.Text;
		}

		private void ResetAllButton_Click(object sender, RoutedEventArgs e) {
			debug.Text = "Reset All Fields";
			init();
		}

		private void HandleTimeExpiration(object sender, EventArgs e) {
			clockState = CLOCK_STATES.STOPPED;
			extractTimerFields();
			//toggleClockButtonText();
			//toggleClockTextBoxElements();
			Console.WriteLine("Time Expired: Handled");
		}

		private void toggleClockTextBoxElements() {
			GameClockMinutes.IsEnabled = !GameClockMinutes.IsEnabled;
			GameClockSeconds.IsEnabled = !GameClockSeconds.IsEnabled;

			HomePenMinutes1.IsEnabled = !HomePenMinutes1.IsEnabled;
			HomePenSeconds1.IsEnabled = !HomePenSeconds1.IsEnabled;
			HomePenMinutes2.IsEnabled = !HomePenMinutes2.IsEnabled;
			HomePenSeconds2.IsEnabled = !HomePenSeconds2.IsEnabled;

			AwayPenMinutes1.IsEnabled = !AwayPenMinutes1.IsEnabled;
			AwayPenSeconds1.IsEnabled = !AwayPenSeconds1.IsEnabled;
			AwayPenMinutes2.IsEnabled = !AwayPenMinutes2.IsEnabled;
			AwayPenSeconds2.IsEnabled = !AwayPenSeconds2.IsEnabled;
		}

		private void setPenaltyInformation() {

			if (homeTeam.penalty1.TotalMilliseconds != 0) {
				HomePenMinutes1.Text = homeTeam.penalty1.Minutes.ToString();
				string s = homeTeam.penalty1.Seconds.ToString();
				HomePenSeconds1.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				HomePenMinutes1.Text = "";
				HomePenSeconds1.Text = "";
			}

			if (homeTeam.penalty2.TotalMilliseconds != 0) {
				HomePenMinutes2.Text = homeTeam.penalty2.Minutes.ToString();
				string s = homeTeam.penalty2.Seconds.ToString();
				HomePenSeconds2.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				HomePenMinutes2.Text = "";
				HomePenSeconds2.Text = "";
			}

			if (awayTeam.penalty1.TotalMilliseconds != 0) {
				AwayPenMinutes1.Text = awayTeam.penalty1.Minutes.ToString();
				string s = awayTeam.penalty1.Seconds.ToString();
				AwayPenSeconds1.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				AwayPenMinutes1.Text = "";
				AwayPenSeconds1.Text = "";
			}

			if (awayTeam.penalty2.TotalMilliseconds != 0) {
				AwayPenMinutes2.Text = awayTeam.penalty2.Minutes.ToString();
				string s = awayTeam.penalty2.Seconds.ToString();
				AwayPenSeconds2.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				AwayPenMinutes2.Text = "";
				AwayPenSeconds2.Text = "";
			}

			HomePenaltiesQueued.Content = homeTeam.getAmtOfQueuedPenalties().ToString();
			AwayPenaltiesQueued.Content = awayTeam.getAmtOfQueuedPenalties().ToString();
		}

		private void toggleClockButtonText() {
			if (clockState == CLOCK_STATES.STOPPED) {
				ClockToggleButton.Content = "Start";
			}
			else {
				ClockToggleButton.Content = "Stop";
			}
		}

		private void extractTimerFields() {
			gameInfo.gameTime = timer.gameClock;
			homeTeam.penalty1 = timer.homePen1;
			homeTeam.penalty2 = timer.homePen2;
			awayTeam.penalty1 = timer.awayPen1;
			awayTeam.penalty2 = timer.awayPen2;
		}



		//=================================================
		// Home Team Methods
		//=================================================

		private void HomeNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			homeTeam.name = HomeNameTextBox.Text;
		}

		private void HomeSubGoalButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.subtractGoal();
			HomeScore.Text = homeTeam.score.ToString();
		}

		private void HomeAddGoalButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.addGoal();
			HomeScore.Text = homeTeam.score.ToString();
		}

		private void HomeScore_TextChanged(object sender, TextChangedEventArgs e) {
			homeTeam.setScore(HomeScore.Text);
			HomeScore.Text = homeTeam.score.ToString();
		}

		private void HomeGoaliePulled_Checked(object sender, RoutedEventArgs e) {
			homeTeam.toggleGoaliePulled();
		}

		private void HomeGoaliePulled_Unchecked(object sender, RoutedEventArgs e) {
			homeTeam.toggleGoaliePulled();
		}

		private void HomePenQueueButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.queuePenalty(HomePenMinutesQ.Text, HomePenSecondsQ.Text);
			setPenaltyInformation();
		}

		private void HomePen1SetButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.setPen1(HomePenMinutes1.Text, HomePenSeconds1.Text);
			setPenaltyInformation();
		}

		private void HomePen2SetButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.setPen2(HomePenMinutes2.Text, HomePenSeconds2.Text);
			setPenaltyInformation();
		}

		private void HomePen1ClearButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.clearPen1();
			setPenaltyInformation();
		}

		private void HomePen2ClearButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.clearPen2();
			setPenaltyInformation();
		}

		//=================================================
		// Away Team Methods
		//=================================================

		private void AwayNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			awayTeam.name = AwayNameTextBox.Text;
		}

		private void AwaySubGoalButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.subtractGoal();
			AwayScore.Text = awayTeam.score.ToString();
		}

		private void AwayAddGoalButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.addGoal();
			AwayScore.Text = awayTeam.score.ToString();
		}

		private void AwayScore_TextChanged(object sender, TextChangedEventArgs e) {
			awayTeam.setScore(AwayScore.Text);
			AwayScore.Text = awayTeam.score.ToString();
		}

		private void AwayGoaliePulled_Checked(object sender, RoutedEventArgs e) {
			awayTeam.toggleGoaliePulled();
		}

		private void AwayGoaliePulled_Unchecked(object sender, RoutedEventArgs e) {
			awayTeam.toggleGoaliePulled();
		}

		private void AwayPenQueueButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.queuePenalty(AwayPenMinutesQ.Text, AwayPenSecondsQ.Text);
			setPenaltyInformation();
		}

		private void AwayPen2SetButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.setPen1(AwayPenMinutes1.Text, AwayPenSeconds1.Text);
			setPenaltyInformation();
		}

		private void AwayPen1SetButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.setPen2(AwayPenMinutes2.Text, AwayPenSeconds2.Text);
			setPenaltyInformation();
		}

		private void AwayPen1ClearButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.clearPen1();
			setPenaltyInformation();
			awayTeam.printInfo();
		}

		private void AwayPen2ClearButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.clearPen2();
			setPenaltyInformation();
			awayTeam.printInfo();
		}
	}
}
