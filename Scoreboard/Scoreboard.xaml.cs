using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using System.Reflection;
using log4net.Config;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Scoreboard {
	using calculator = PenaltyAndTimeCalculator;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {

		public GameInfoBanner banner;

		public HockeyTeam homeTeam { get; set; }
		public HockeyTeam awayTeam { get; set; }
		public GameInfo gameInfo { get; set; }
		public enum CLOCK_STATES{ STOPPED, RUNNING }
		public CLOCK_STATES clockState;

		private GameTimer timer { get; set; }
		private int refreshes = 0;
		private const int REFRESH_INTERVAL = 10;
		private const string GAME_INFO = @".\GameInfo.sb";
		private const string HOME_TEAM = @".\HomeTeam.sb";
		private const string AWAY_TEAM = @".\AwayTeam.sb";
		private int homeColumn;
		private int awayColumn;

		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		

		public MainWindow()
        {
			XmlConfigurator.Configure();
			initData();
            InitializeComponent();
			initFileData();
		}

		public void initData() {
			banner = new GameInfoBanner();
			gameInfo = new GameInfo();
			homeTeam = new HockeyTeam("HOME") {
				imagePath = @"./Images/Home_Cell_Specular.png"
			};
			awayTeam = new HockeyTeam("AWAY") {
				imagePath = @"./Images/Away_Cell_Specular.png"
			};
			timer = new GameTimer(REFRESH_INTERVAL);

			homeColumn = 0;
			awayColumn = 2;
			
			_log.Info("Initializing Data");
		}

		public void readGameDataFromFiles() {
			if (File.Exists(GAME_INFO)) {
				string saved = File.ReadAllText(GAME_INFO);
				gameInfo.fromJson(saved);
			}
			if (File.Exists(HOME_TEAM)) {
				string saved = File.ReadAllText(HOME_TEAM);
				homeTeam.fromJson(saved);
			}
			if (File.Exists(AWAY_TEAM)) {
				string saved = File.ReadAllText(AWAY_TEAM);
				awayTeam.fromJson(saved);
			}
		}

		public void writeGameDataToFiles() {
			List<string> paths = new List<string> {
				GAME_INFO,
				HOME_TEAM,
				AWAY_TEAM
			};
			foreach (string path in paths) {
				if (!File.Exists(path)) {
					FileStream f = File.Create(path);
					f.Close();
				}
			}
			File.WriteAllText(GAME_INFO, gameInfo.logInfo());
			File.WriteAllText(HOME_TEAM, homeTeam.logInfo());
			File.WriteAllText(AWAY_TEAM, awayTeam.logInfo());
		}

		public void initFileData() {
			readGameDataFromFiles();

			string p = gameInfo.period;

			initInterfaceAndRelatedData();

			gameInfo.setPeriod(p);
			banner.setPeriod(p);
			GamePeriod.Text = p;
		}

		public void initInterfaceAndRelatedData() {
			timer.TimeStopped += HandleTimeStoppage;
			timer.Refresh += HandleRefresh;

			timer.setTimerFields(gameInfo, homeTeam, awayTeam);
			setGameClockInformation();
			setPenaltyInformation();

			GamePeriodPicker.SelectedIndex = 0;

			HomeNameTextBox.Text = homeTeam.name;
			HomeScore.Text = homeTeam.score.ToString();

			AwayNameTextBox.Text = awayTeam.name;
			AwayScore.Text = awayTeam.score.ToString();

			ClockToggleButton.Content = "Start";
			clockState = CLOCK_STATES.STOPPED;

			bool hGoaliePulled = homeTeam.goaliePulled;
			bool aGoaliePulled = awayTeam.goaliePulled;
			HomeGoaliePulled.IsChecked = hGoaliePulled;
			AwayGoaliePulled.IsChecked = aGoaliePulled;
			homeTeam.goaliePulled = hGoaliePulled;
			awayTeam.goaliePulled = aGoaliePulled;
			calculateAndSetTimers();

			DEBUG_LABEL.Text = "DEBUG";

			BitmapImage bmp = filepathToImage(homeTeam.imagePath);
			if (bmp != null) {
				banner.HomeBackground.Source = bmp;
			}
			bmp = filepathToImage(awayTeam.imagePath);
			if (bmp != null) {
				banner.AwayBackground.Source = bmp;
			}
			HomeImage.Source = banner.HomeBackground.Source;
			AwayImage.Source = banner.AwayBackground.Source;

			if (gameInfo.reversedBanner) {
				swapTeamGrids();
				banner.swapBannerPositions();
			}

			banner.Show();
			banner.Activate();
			

			_log.Info("Initializing UI and Relevant Data");
		}
			
		public void logInfo() {
			string info = "";
			info += gameInfo.logInfo();
			info += homeTeam.logInfo();
			info += awayTeam.logInfo();
			info += "{\"clockState\":" + clockState.ToString() + "}";
			_log.Debug(info);
		}

		public TimeSpan formatTimeSpan(string minutesString, string secondsString) {
			TimeSpan penaltyTime = new TimeSpan();
			if (int.TryParse(minutesString, out int mins)) {
				if (int.TryParse(secondsString, out int sec)) {
					penaltyTime = new TimeSpan(0, mins, sec);
				}
			}
			_log.Debug("FormatPenalty: IN> " + minutesString + ":" + secondsString + " OUT> " + penaltyTime);
			return penaltyTime;
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

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			ClockToggleButton.Focus();
			DEBUG_LABEL.Text = "Press ENTER to toggle clock";
			_log.Info("Clock Toggle Focus");
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			timer.Dispose();
			banner.Close();
			writeGameDataToFiles();
			_log.Info("Exiting");
		}

		//=================================================
		// Game Info Methods
		//=================================================

		private void ClockToggleButton_Click(object sender, RoutedEventArgs e){
			toggleClock();
        }

        private void ClockSetButton_Click(object sender, RoutedEventArgs e){
			string minutesString = GameClockMinutes.Text;
			string secondsString = GameClockSeconds.Text;
			gameInfo.setGameTime(formatTimeSpan(minutesString , secondsString));
			calculateAndSetTimers();
			GameClockMinutes.Text = gameInfo.gameTime.Minutes.ToString();
			string s = gameInfo.gameTime.Seconds.ToString();
			GameClockSeconds.Text = (s.Length > 1 ? s : '0' + s);
			DEBUG_LABEL.Text = "Setting Clock: " + GameClockMinutes.Text + ':' + GameClockSeconds.Text;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void GamePeriodPicker_SelectionChanged(object sender, SelectionChangedEventArgs e){
			string period = ((ComboBoxItem)GamePeriodPicker.SelectedItem).Content.ToString();
			gameInfo.setPeriod(period);
			banner.setPeriod(period);
			GamePeriod.Text = period;
			DEBUG_LABEL.Text = "Period Set: " + GamePeriod.Text;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void GamePeriodSetter_Click(object sender, RoutedEventArgs e) {
			gameInfo.setPeriod(GamePeriod.Text);
			banner.setPeriod(gameInfo.period);
			DEBUG_LABEL.Text = "Period Set: " + GamePeriod.Text;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void ResetAllButton_Click(object sender, RoutedEventArgs e) {
			DEBUG_LABEL.Text = "Reset All Fields";
			_log.Info(DEBUG_LABEL.Text);
			timer.Dispose();
			banner.Close();
			initData();
			initInterfaceAndRelatedData();
		}

		private void ReverseButton_Click(object sender, RoutedEventArgs e) {
			swapTeamGrids();
			banner.swapBannerPositions();
			gameInfo.reversedBanner = !gameInfo.reversedBanner;
		}

		private void HandleTimeStoppage(object sender, TimerEventArgs e) {
			clockState = CLOCK_STATES.STOPPED;
			extractTimerFields(e);
			this.Dispatcher.InvokeAsync(() => {
				toggleClockButtonText();
				toggleClockTextBoxElements();
			});
			_log.Info("Time Stoppage");
		}

		private void HandleRefresh(Object sender, TimerEventArgs e) {
			this.Dispatcher.InvokeAsync(() => {
				extractTimerFields(e);
				setGameClockInformation();
				setPenaltyInformation();
				homeTeam.managePenalties();
				awayTeam.managePenalties();
				checkForDequeuedPenalties(e);
				calculateAndSetTimers();
			});
			if (++refreshes/1000 != 0) {
				logInfo();
				refreshes = 0;
			}
		}

		private void toggleClockTextBoxElements() {
			GameClockMinutes.IsEnabled = !GameClockMinutes.IsEnabled;
			GameClockSeconds.IsEnabled = !GameClockSeconds.IsEnabled;

			ClockSetButton.IsEnabled = !ClockSetButton.IsEnabled;

			HomePenMinutes1.IsEnabled = !HomePenMinutes1.IsEnabled;
			HomePenSeconds1.IsEnabled = !HomePenSeconds1.IsEnabled;
			HomePen1SetButton.IsEnabled = !HomePen1SetButton.IsEnabled;
			HomePenMinutes2.IsEnabled = !HomePenMinutes2.IsEnabled;
			HomePenSeconds2.IsEnabled = !HomePenSeconds2.IsEnabled;
			HomePen2SetButton.IsEnabled = !HomePen2SetButton.IsEnabled;

			AwayPenMinutes1.IsEnabled = !AwayPenMinutes1.IsEnabled;
			AwayPenSeconds1.IsEnabled = !AwayPenSeconds1.IsEnabled;
			AwayPen1SetButton.IsEnabled = !AwayPen1SetButton.IsEnabled;
			AwayPenMinutes2.IsEnabled = !AwayPenMinutes2.IsEnabled;
			AwayPenSeconds2.IsEnabled = !AwayPenSeconds2.IsEnabled;
			AwayPen2SetButton.IsEnabled = !AwayPen2SetButton.IsEnabled;

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
			if (!penaltiesEqualTolerant(homeTeam.penalty1,e.homePen1)) {
				timer.enqueuePenalty("HOME", homeTeam.penalty1);
				_log.Info("H P1 Queued");
			}
			if (!penaltiesEqualTolerant(homeTeam.penalty2, e.homePen2)) {
				timer.enqueuePenalty("HOME", homeTeam.penalty2);
				_log.Info("H P2 Queued");
			}
			if (!penaltiesEqualTolerant(awayTeam.penalty1, e.awayPen1)) {
				timer.enqueuePenalty("AWAY", awayTeam.penalty1);
				_log.Info("A P1 Queued");
			}
			if (!penaltiesEqualTolerant(awayTeam.penalty2, e.awayPen2)) {
				timer.enqueuePenalty("AWAY", awayTeam.penalty2);
				_log.Info("A P2 Queued");
			}
		}

		private void calculateAndSetTimers() {
			banner.setTime(gameInfo.getFormattedGameTime());
			string teamWithAdvantage = calculator.calculateTeamWithAdvantage(homeTeam, awayTeam);
			string time = calculator.calculateTimeToDisplay(homeTeam, awayTeam);
			string homeStatus = calculator.calculatePlayerAdvantage(homeTeam, awayTeam);
			string awayStatus = calculator.calculatePlayerAdvantage(awayTeam, homeTeam);
			switch (teamWithAdvantage) {
				case "NONE":
					string status = calculator.calculateEvenStrength(homeTeam, awayTeam);
					if(status != "") {
						banner.setEvenStrength(status + " " + time);
					}
					else {
						banner.setEvenStrength(status);
					}
					banner.setHomeAdvantage(homeStatus);
					banner.setAwayAdvantage(awayStatus);
					break;
				case "HOME":
					banner.setEvenStrength("");
					banner.setHomeAdvantage(homeStatus + " " + time);
					banner.setAwayAdvantage(awayStatus);
					break;
				case "AWAY":
					banner.setEvenStrength("");
					banner.setHomeAdvantage(homeStatus);
					banner.setAwayAdvantage(awayStatus + " " + time);
					break;
			}
		}

		private void setGameClockInformation() {
			GameClockMinutes.Text = gameInfo.gameTime.Minutes.ToString();
			string s = gameInfo.gameTime.Seconds.ToString();
			GameClockSeconds.Text = (s.Length > 1 ? s : '0' + s);
		}

		private void toggleClock() {
			if (clockState == CLOCK_STATES.STOPPED) {
				timer.setTimerFields(gameInfo, homeTeam, awayTeam);
				timer.startClock();
				clockState = CLOCK_STATES.RUNNING;
				toggleClockButtonText();
				toggleClockTextBoxElements();
				_log.Info("StartClock");
			}
			else {
				timer.stopClock();
				_log.Info("StopClock");
			}

			DEBUG_LABEL.Text = "Toggle Clock Mode: " + clockState;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void swapTeamGrids() {
			Grid.SetColumn(HomeTeamGrid, awayColumn);
			Grid.SetColumn(AwayTeamGrid, homeColumn);
			int tmp = homeColumn;
			homeColumn = awayColumn;
			awayColumn = tmp;
		}

		// small decimal differences inspire creation of phantom penalties
		private bool penaltiesEqualTolerant(TimeSpan p1, TimeSpan p2) {
			return (int)p1.TotalMilliseconds + 50 >= (int)p2.TotalMilliseconds 
				&&
				(int)p2.TotalMilliseconds >= (int)p1.TotalMilliseconds - 50;
		}

		//=================================================
		// Home Team Methods
		//=================================================

		private void HomeNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			homeTeam.name = HomeNameTextBox.Text;
			banner.setHomeName(homeTeam.name);
			DEBUG_LABEL.Text = "Home Team Name Changed: " + homeTeam.name;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void HomeSubGoalButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.subtractGoal();
			banner.setHomeScore(homeTeam.score);
			HomeScore.Text = homeTeam.score.ToString();
			DEBUG_LABEL.Text = "Subtract Home Goal";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomeAddGoalButton_Click(object sender, RoutedEventArgs e) {
			homeTeam.addGoal();
			banner.setHomeScore(homeTeam.score);
			HomeScore.Text = homeTeam.score.ToString();
			DEBUG_LABEL.Text = "Add Home Goal";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomeScore_TextChanged(object sender, TextChangedEventArgs e) {
			homeTeam.setScore(HomeScore.Text);
			banner.setHomeScore(homeTeam.score);
			HomeScore.Text = homeTeam.score.ToString();
			DEBUG_LABEL.Text = "Set Home Score: " + HomeScore.Text;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void HomeGoaliePulled_Checked(object sender, RoutedEventArgs e) {
			homeTeam.toggleGoaliePulled();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Home Goalie Pulled";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomeGoaliePulled_Unchecked(object sender, RoutedEventArgs e) {
			homeTeam.toggleGoaliePulled();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Home Goalie In Net";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomePenQueueButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(HomePenMinutesQ.Text, HomePenSecondsQ.Text);
			if (timer.Running) {
				timer.enqueuePenalty("HOME", queuedPenalty);
			}
			else {
				homeTeam.queuePenalty(queuedPenalty);
			}
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Queueing Home Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomePen1SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(HomePenMinutes1.Text, HomePenSeconds1.Text);
			homeTeam.setPen1(queuedPenalty);
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Setting First Home Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomePen2SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(HomePenMinutes2.Text, HomePenSeconds2.Text);
			homeTeam.setPen2(queuedPenalty);
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Setting Second Home Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomePen1ClearButton_Click(object sender, RoutedEventArgs e) {
			if (timer.Running) {
				timer.clearPenalty("HOME1");
			}
			homeTeam.clearPen1();
			setPenaltyInformation();
			if (!timer.Running) {
				calculateAndSetTimers();
			}
			DEBUG_LABEL.Text = "Clearing First Home Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomePen2ClearButton_Click(object sender, RoutedEventArgs e) {
			if (timer.Running) {
				timer.clearPenalty("HOME2");
			}
			homeTeam.clearPen2();
			setPenaltyInformation();
			if (!timer.Running) {
				calculateAndSetTimers();
			}
			DEBUG_LABEL.Text = "Clearing Second Home Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void HomeImageButton_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog {
				Filter = "Image Files(*PNG;*.BMP;*.JPG;*.GIF)|*PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
			};

			Nullable<bool> result = dlg.ShowDialog();

			if (result == true) {
				string filename = dlg.FileName;
				BitmapImage bmp = filepathToImage(filename);
				if (bmp != null) {
					HomeImage.Source = bmp;
					banner.HomeBackground.Source = bmp;
					homeTeam.imagePath = filename;
					DEBUG_LABEL.Text = "New Home Image: " + Path.GetFileName(filename);
				}
				else {
					DEBUG_LABEL.Text = "Selected File Does not have compatible dimensions";
				}
				
			}
		}

		//=================================================
		// Away Team Methods
		//=================================================

		private void AwayNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			awayTeam.name = AwayNameTextBox.Text;
			banner.setAwayName(awayTeam.name);
			DEBUG_LABEL.Text = "Away Team Name Changed: " + awayTeam.name;
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void AwaySubGoalButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.subtractGoal();
			banner.setAwayScore(awayTeam.score);
			AwayScore.Text = awayTeam.score.ToString();
			DEBUG_LABEL.Text = "Subtract Away Goal";
			_log.Debug(DEBUG_LABEL.Text);
		}

		private void AwayAddGoalButton_Click(object sender, RoutedEventArgs e) {
			awayTeam.addGoal();
			banner.setAwayScore(awayTeam.score);
			AwayScore.Text = awayTeam.score.ToString();
			DEBUG_LABEL.Text = "Add Away Goal";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayScore_TextChanged(object sender, TextChangedEventArgs e) {
			awayTeam.setScore(AwayScore.Text);
			banner.setAwayScore(awayTeam.score);
			AwayScore.Text = awayTeam.score.ToString();
			DEBUG_LABEL.Text = "Set Away Score: " + HomeScore.Text;
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayGoaliePulled_Checked(object sender, RoutedEventArgs e) {
			awayTeam.toggleGoaliePulled();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Away Goalie Pulled";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayGoaliePulled_Unchecked(object sender, RoutedEventArgs e) {
			awayTeam.toggleGoaliePulled();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Away Goalie In Net";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayPenQueueButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(HomePenMinutesQ.Text, HomePenSecondsQ.Text);
			if (timer.Running) {
				timer.enqueuePenalty("AWAY", queuedPenalty);
			}
			else {
				awayTeam.queuePenalty(queuedPenalty);
			}
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Queueing Away Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayPen1SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(AwayPenMinutes1.Text, AwayPenSeconds1.Text);
			awayTeam.setPen1(queuedPenalty);
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Setting First Away Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayPen2SetButton_Click(object sender, RoutedEventArgs e) {
			TimeSpan queuedPenalty = formatTimeSpan(AwayPenMinutes2.Text, AwayPenSeconds2.Text);
			awayTeam.setPen2(queuedPenalty);
			setPenaltyInformation();
			calculateAndSetTimers();
			DEBUG_LABEL.Text = "Setting Second Away Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayPen1ClearButton_Click(object sender, RoutedEventArgs e) {
			if (timer.Running) {
				timer.clearPenalty("AWAY1");
			}
			awayTeam.clearPen1();
			setPenaltyInformation();
			if (!timer.Running) {
				calculateAndSetTimers();
			}
			DEBUG_LABEL.Text = "Clearing First Away Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayPen2ClearButton_Click(object sender, RoutedEventArgs e) {
			if (timer.Running) {
				timer.clearPenalty("AWAY2");
			}
			awayTeam.clearPen2();
			setPenaltyInformation();
			if (!timer.Running) {
				calculateAndSetTimers();
			}
			DEBUG_LABEL.Text = "Clearing Second Away Penalty";
			_log.Info(DEBUG_LABEL.Text);
		}

		private void AwayImageButton_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog {
				Filter = "Image Files(*PNG;*.BMP;*.JPG;*.GIF)|*PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
			};

			Nullable<bool> result = dlg.ShowDialog();

			if (result == true) {
				string filename = dlg.FileName;
				BitmapImage bmp = filepathToImage(filename);
				if (bmp != null) {
					AwayImage.Source = bmp;
					banner.AwayBackground.Source = bmp;
					awayTeam.imagePath = filename;
					DEBUG_LABEL.Text = "New Away Image: " + Path.GetFileName(filename);
				}
				else {
					DEBUG_LABEL.Text = "Selected File Does not have compatible dimensions";
				}

			}
		}

		
	}
}
