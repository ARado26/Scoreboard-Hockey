using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scoreboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard.Tests {
	[TestClass()]
	public class PenaltyAndTimeCalculatorTests {
		[TestMethod()]
		public void calculateTimeToDisplayTest() {
			HockeyTeam home = new HockeyTeam("h");
			HockeyTeam away = new HockeyTeam("a");
			home.setPen1(new TimeSpan(0, 2, 0));
			home.managePenalties();

			string t = PenaltyAndTimeCalculator.calculateTimeToDisplay(home, away);
			Assert.AreEqual("2:00",t);

			away.setPen1(new TimeSpan(0, 1, 40));
			away.managePenalties();
			t = PenaltyAndTimeCalculator.calculateTimeToDisplay(home, away);
			Assert.AreEqual("1:40", t);
		}

		[TestMethod()]
		public void calculateTeamWithAdvantageTest() {
			HockeyTeam home = new HockeyTeam("h");
			HockeyTeam away = new HockeyTeam("a");

			string t = PenaltyAndTimeCalculator.calculateTeamWithAdvantage(home, away);
			Assert.AreEqual("NONE", t);

			home.setPen1(new TimeSpan(0, 2, 0));
			home.managePenalties();
			t = PenaltyAndTimeCalculator.calculateTeamWithAdvantage(home, away);
			Assert.AreEqual("AWAY", t);

			away.setPen1(new TimeSpan(0, 2, 0));
			away.setPen2(new TimeSpan(0, 2, 0));
			away.managePenalties();
			t = PenaltyAndTimeCalculator.calculateTeamWithAdvantage(home, away);
			Assert.AreEqual("HOME", t);

		}

		[TestMethod()]
		public void calculatePlayerAdvantageTest() {
			HockeyTeam home = new HockeyTeam("h");
			HockeyTeam away = new HockeyTeam("a");

			home.toggleGoaliePulled();
			string t = PenaltyAndTimeCalculator.calculatePlayerAdvantage(home, away);
			Assert.AreEqual("Empty Net", t);
			home.toggleGoaliePulled();

			away.setPen1(new TimeSpan(0, 2, 0));
			away.managePenalties();
			t = PenaltyAndTimeCalculator.calculatePlayerAdvantage(home, away);
			Assert.AreEqual("PP", t);

			away.setPen2(new TimeSpan(0, 2, 0));
			away.managePenalties();
			t = PenaltyAndTimeCalculator.calculatePlayerAdvantage(home, away);
			Assert.AreEqual("5 on 3", t);

			home.toggleGoaliePulled();
			t = PenaltyAndTimeCalculator.calculatePlayerAdvantage(home, away);
			Assert.AreEqual("6 on 3", t);
		}

		[TestMethod()]
		public void timeSpanToPenaltyStringTest() {
			string t = PenaltyAndTimeCalculator.timeSpanToPenaltyString(new TimeSpan(0, 2, 0));
			Assert.AreEqual("2:00", t);
		}
	}
}