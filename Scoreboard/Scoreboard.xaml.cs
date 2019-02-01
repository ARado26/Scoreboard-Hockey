using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

		private enum CLOCK_STATES{ STOPPED, RUNNING }
		private CLOCK_STATES clockState;
		private const int REFRESH_INTERVAL = 10;


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
			timer.TimeStopped += HandleTimeStoppage;
			timer.Refresh += HandleRefresh;
			

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

		public TimeSpan formatPenalty(string minutesString, string secondsString) {
			TimeSpan penaltyTime = new TimeSpan();
			if (int.TryParse(minutesString, out int mins)) {
				if (int.TryParse(secondsString, out int sec)) {
					penaltyTime = new TimeSpan(0, mins, sec);
				}
			}
			return penaltyTime;
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
				toggleClockButtonText();
				toggleClockTextBoxElements();
			}
			else {
				timer.stopClock();
			}

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

		private void HandleTimeStoppage(object sender, TimerEventArgs e) {
			clockState = CLOCK_STATES.STOPPED;
			extractTimerFields(e);
			this.Dispatcher.Invoke(() => {
				toggleClockButtonText();
				toggleClockTextBoxElements();
			});
		}

		private void HandleRefresh(Object sender, TimerEventArgs e) {
			extractTimerFields(e);
			this.Dispatcher.Invoke(() => {
				setGameClockInformation();
				setPenaltyInformation();
				homeTeam.managePenalties();
				awayTeam.managePenalties();
			});
			checkForDequeuedPenalties(e);
		}

		private void toggleClockTextBoxElements() {
			GameClockMinutes.IsEnabled = !GameClockMinutes.IsEnabled;
			GameClockSeconds.IsEnabled = !GameClockSeconds.IsEnabled;

			ClockSetButton.IsEnabled = !ClockSetButton.IsEnabled;

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

			if (!timer.Running) {
				homeTeam.managePenalties();
				awayTeam.managePenalties();
			}

			if (homeTeam.penalty1.TotalMilliseconds != 0) {
				HomePenMinutes1.Text = homeTeam.penalty1.Minutes.ToString();
				string s = homeTeam.penalty1.Seconds.ToString();
				HomePenSeconds1.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				HomePenMinutes1.Text = "0";
				HomePenSeconds1.Text = "00";
			}

			if (homeTeam.penalty2.TotalMilliseconds != 0) {
				HomePenMinutes2.Text = homeTeam.penalty2.Minutes.ToString();
				string s = homeTeam.penalty2.Seconds.ToString();
				HomePenSeconds2.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				HomePenMinutes2.Text = "0";
				HomePenSeconds2.Text = "00";
			}

			if (awayTeam.penalty1.TotalMilliseconds != 0) {
				AwayPenMinutes1.Text = awayTeam.penalty1.Minutes.ToString();
				string s = awayTeam.penalty1.Seconds.ToString();
				AwayPenSeconds1.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				AwayPenMinutes1.Text = "0";
				AwayPenSeconds1.Text = "00";
			}

			if (awayTeam.penalty2.TotalMilliseconds != 0) {
				AwayPenMinutes2.Text = awayTeam.penalty2.Minutes.ToString();
				string s = awayTeam.penalty2.Seconds.ToString();
				AwayPenSeconds2.Text = (s.Length > 1 ? s : '0' + s);
			}
			else {
				AwayPenMinutes2.Text = "0";
				AwayPenSeconds2.Text = "00";
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

		private void extractTimerFields(TimerEventArgs e) {
			gameInfo.gameTime = e.gameClock;
			homeTeam.penalty1 = e.homePen1;
			homeTeam.penalty2 = e.homePen2;
			awayTeam.penalty1 = e.awayPen1;
			awayTeam.penalty2 = e.awayPen2;
		}

		private void checkForDequeuedPenalties(TimerEventArgs e) {
			if (!homeTeam.penalty1.Equals(e.homePen1)) {
				timer.enqueuePenalty("HOME", homeTeam.penalty1);
			}
			if (!homeTeam.penalty2.Equals(e.homePen2)) {
				timer.enqueuePenalty("HOME", homeTeam.penalty2);
			}
			if (!awayTeam.penalty1.Equals(e.awayPen1)) {
				timer.enqueuePenalty("AWAY", awayTeam.penalty1);
			}
			if (!awayTeam.penalty2.Equals(e.awayPen2)) {
				timer.enqueuePenalty("AWAY", awayTeam.penalty2);
			}
		}

		private void setGameClockInformation() {
			GameClockMinutes.Text = gameInfo.gameTime.Minutes.ToString();
			string s = gameInfo.gameTime.Seconds.ToString();
			GameClockSeconds.Text = (s.Length > 1 ? s : '0' + s);
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
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutesQ.Text, HomePenSecondsQ.Text);
			if (timer.Running) {
				timer.enqueuePenalty("HOME", queuedPenalty);
			}
			else {
				homeTeam.queuePenalty(queuedPenalty);
			}
			setPenaltyInformation();
		}

		private void HomePen1SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutes1.Text, HomePenSeconds1.Text);
			homeTeam.setPen1(queuedPenalty);
			setPenaltyInformation();
		}

		private void HomePen2SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutes2.Text, HomePenSeconds2.Text);
			homeTeam.setPen2(queuedPenalty);
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
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutesQ.Text, HomePenSecondsQ.Text);
			if (timer.Running) {
				timer.enqueuePenalty("AWAY", queuedPenalty);
			}
			else {
				awayTeam.queuePenalty(queuedPenalty);
			}
			setPenaltyInformation();
		}

		private void AwayPen1SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutes1.Text, HomePenSeconds1.Text);
			awayTeam.setPen2(queuedPenalty);
			setPenaltyInformation();
		}

		private void AwayPen2SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatPenalty(HomePenMinutes2.Text, HomePenSeconds2.Text);
			awayTeam.setPen1(queuedPenalty);
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
