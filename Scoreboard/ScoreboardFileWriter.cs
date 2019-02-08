using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
		private string evenPenaltyInfo;
		private string homeName;
		private int homeScore;
		private string homePenaltyInfo;
		private string awayName;
		private int awayScore;
		private string awayPenaltyInfo;

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

			List<string> paths = new List<string> {
				gameClockPath,
				periodPath,
				evenStrengthPath,
				homeTeamNamePath,
				homeTeamScorePath,
				homeTeamPlayerAdvantagePath,
				awayTeamNamePath,
				awayTeamScorePath,
				awayTeamPlayerAdvantagePath,
			};
			foreach(string path in paths) {
				if (!File.Exists(path)) {
					FileStream f = File.Create(path);
					f.Close();
				}
			}

			gameClock = File.ReadAllText(gameClockPath);
			prd = File.ReadAllText(periodPath);
			evenPenaltyInfo = "CLEAR";
			homeName = File.ReadAllText(homeTeamNamePath);
			homePenaltyInfo = "CLEAR";
			awayName = File.ReadAllText(awayTeamNamePath);
			awayPenaltyInfo = "CLEAR";

			if (int.TryParse(File.ReadAllText(homeTeamScorePath), out int i)) {
				homeScore = i;
			}
			else {
				homeScore = 0;
			}

			if (int.TryParse(File.ReadAllText(awayTeamScorePath), out i)) {
				awayScore = i;
			}
			else {
				awayScore = 0;
			}

		}

		public void publishTimers() {
			foreach(string key in instructions.Keys) {
				if(instructions.TryRemove(key, out string val)) {
					//Console.WriteLine(key + ":" + val);
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
			if (evenPenaltyInfo != penaltyInfo) {
				instructions.TryAdd(evenStrengthPath, penaltyInfo);
				evenPenaltyInfo = penaltyInfo;
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
			if (homePenaltyInfo != penaltyInfo) {
				instructions.TryAdd(homeTeamPlayerAdvantagePath, penaltyInfo);
				homePenaltyInfo = penaltyInfo;
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
			if (awayPenaltyInfo != penaltyInfo) {
				instructions.TryAdd(awayTeamPlayerAdvantagePath, penaltyInfo);
				awayPenaltyInfo = penaltyInfo;
			}
		}
	}
}
