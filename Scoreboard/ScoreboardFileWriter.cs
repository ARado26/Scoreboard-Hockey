using System;
using System.IO;
using System.Collections.Concurrent;

namespace Scoreboard {
	public class ScoreboardFileWriter {
		private ConcurrentDictionary<string,string> instructions;
		private string path;

		private string gameClockPath;
		private string periodPath;
		private string evenStrengthPath;

		private string homeTeamNamePath;
		private string homeTeamScorePath;
		private string homeTeamPlayerAdvantagePath;

		private string awayTeamNamePath;
		private string awayTeamScorePath;
		private string awayTeamPlayerAdvantagePath;

		private string gameClock;
		private string prd;
		private string penInfo;
		private string homeName;
		private int homeScore;
		private string awayName;
		private int awayScore;

		public ScoreboardFileWriter(string filepath) {
			path = filepath;
			Directory.CreateDirectory(path);

			instructions = new ConcurrentDictionary<string, string>();

			gameClockPath = Path.Combine(path, "GameClock.txt");
			periodPath = Path.Combine(path, "Period.txt");
			evenStrengthPath = Path.Combine(path, "EvenStrength.txt");

			homeTeamNamePath = Path.Combine(path, "HomeName.txt");
			homeTeamScorePath = Path.Combine(path, "HomeScore.txt");
			homeTeamPlayerAdvantagePath = Path.Combine(path, "HomeAdvantage.txt");

			awayTeamNamePath = Path.Combine(path, "AwayName.txt");
			awayTeamScorePath = Path.Combine(path, "AwayScore.txt");
			awayTeamPlayerAdvantagePath = Path.Combine(path, "AwayAdvantage.txt");
		}

		public void publishTimers() {
			foreach(string key in instructions.Keys) {
				if(instructions.TryRemove(key, out string val)) {
					File.WriteAllText(key, val);
				}
			}
		}

		public void writeGameClock(string time) {
			if (gameClock != time) {
				instructions.TryAdd(gameClockPath, time);
				gameClock = time;
			}
			
		}

		public void writePeriod(string period) {
			if (prd != period) {
				File.WriteAllText(periodPath, period);
				prd = period;
			}
		}

		public void writeEvenStrengthPenaltyInfo(string penaltyInfo) {
			if (penInfo != penaltyInfo) {
				instructions.TryAdd(evenStrengthPath, penaltyInfo);
				penInfo = penaltyInfo;
			}
		}


		public void writeHomeTeamName(string homeTeamName) {
			if(homeName != homeTeamName) {
				File.WriteAllText(homeTeamNamePath, homeTeamName);
				homeName = homeTeamName;
			}
		}

		public void writeHomeTeamScore(int score) {
			if (homeScore != score) {
				File.WriteAllText(homeTeamScorePath, score.ToString());
				homeScore = score;
			}
			
		}

		public void writeHomeTeamPlayerAdvantage(string penaltyInfo) {
			if (penInfo != penaltyInfo) {
				instructions.TryAdd(homeTeamPlayerAdvantagePath, penaltyInfo);
				penInfo = penaltyInfo;
			}
		}


		public void writeAwayTeamName(string awayTeamName) {
			if(awayName != awayTeamName) {
				File.WriteAllText(awayTeamNamePath, awayTeamName);
				awayName = awayTeamName;
			}
		}

		public void writeAwayTeamScore(int score) {
			if (awayScore != score) {
				File.WriteAllText(awayTeamScorePath, score.ToString());
				awayScore = score;
			}
		}

		public void writeAwayTeamPlayerAdvantage(string penaltyInfo) {
			if (penInfo != penaltyInfo) {
				instructions.TryAdd(awayTeamNamePath, penaltyInfo);
				penInfo = penaltyInfo;
			}
		}
	}
}
