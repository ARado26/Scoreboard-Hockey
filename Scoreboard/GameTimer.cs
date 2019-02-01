﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;

namespace Scoreboard {
	public class GameTimer {
		

		public TimeSpan gameClock { get; private set; }
		public TimeSpan homePen1 { get; private set; }
		public TimeSpan homePen2 { get; private set; }
		public TimeSpan awayPen1 { get; private set; }
		public TimeSpan awayPen2 { get; private set; }

		public ConcurrentQueue<Tuple<String,TimeSpan>> penaltyQueue { get; set; }

		public delegate void RefreshHandler(object sender, TimerEventArgs e);
		public event RefreshHandler TimeStopped;
		public event RefreshHandler Refresh;

		public bool Running { get; private set; }

		
		private Timer timer;
		private int interval;

		private DateTime refreshStart;
		private DateTime timerStarted;
		private DateTime now;
		private TimeSpan elapsedTime;

		public GameTimer(int millisecondsInterval) {
			interval = millisecondsInterval;
			penaltyQueue = new ConcurrentQueue<Tuple<string, TimeSpan>>();
			timer = new Timer(interval);
			timer.Elapsed += adjustTimersHandler;
			Running = false;
		}
		
		public void startClock() {
			Console.WriteLine("Starting Clock: " + gameClock);
			now = System.DateTime.Now;
			timerStarted = now;
			refreshStart = now;
			Running = true;
			timer.Start();
		}

		public void stopClock() {
			timer.Stop();
			Running = false;
			adjustTimers();
			Console.WriteLine("Stopping Clock: " + gameClock);
			Console.WriteLine("Elapsed time: " + (System.DateTime.Now - timerStarted));
			Console.WriteLine("Started at: " + (gameClock + (System.DateTime.Now - timerStarted)));
			OnTimeStop();
		}

		public void setTimerFields(GameInfo game, HockeyTeam home, HockeyTeam away) {
			gameClock = game.gameTime;
			homePen1 = home.penalty1;
			homePen2 = home.penalty2;
			awayPen1 = away.penalty1;
			awayPen2 = away.penalty2;
		}

		public void enqueuePenalty(string team, TimeSpan pen) {
			penaltyQueue.Enqueue(new Tuple<string, TimeSpan>(team, pen));
		}


		private void adjustTimers() {
			now = System.DateTime.Now;
			elapsedTime = (now - refreshStart);
			refreshStart = now;
			gameClock -= elapsedTime;
			homePen1 -= elapsedTime;
			homePen2 -= elapsedTime;
			awayPen1 -= elapsedTime;
			awayPen2 -= elapsedTime;
			handleNegativeTimers();
			checkQueueForNewPenalty();
		}

		private void handleNegativeTimers() {
			if (gameClock.TotalMilliseconds < 0) {
				gameClock = new TimeSpan();
			}
			if (homePen1.TotalMilliseconds < 0) {
				homePen1 = new TimeSpan();
			}
			if (homePen2.TotalMilliseconds < 0) {
				homePen2 = new TimeSpan();
			}
			if (awayPen1.TotalMilliseconds < 0) {
				awayPen1 = new TimeSpan();
			}
			if (awayPen2.TotalMilliseconds < 0) {
				awayPen2 = new TimeSpan();
			}
		}

		private void checkQueueForNewPenalty() {
			if (penaltyQueue.TryDequeue(out Tuple<string,TimeSpan> pen)) {
				string team = pen.Item1;
				TimeSpan penalty = pen.Item2;
				if(team == "HOME") {
					if (homePen1.TotalMilliseconds == 0) {
						homePen1 = penalty;
					} else if (homePen2.TotalMilliseconds == 0) {
						homePen2 = penalty;
					}
				} else { // AWAY
					if (awayPen1.TotalMilliseconds == 0) {
						awayPen1 = penalty;
					}
					else if (awayPen2.TotalMilliseconds == 0) {
						awayPen2 = penalty;
					}
				}
			}
		}

		private void adjustTimersHandler(Object source, ElapsedEventArgs e) {
			if (gameClock.TotalMilliseconds <= 0) {
				stopClock();
			}else {
				adjustTimers();
				OnRefresh();
			}
		}

		protected virtual void OnTimeStop() {
			TimeStopped?.Invoke(this, new TimerEventArgs(this));
		}

		protected virtual void OnRefresh() {
			Refresh?.Invoke(this, new TimerEventArgs(this));
		}

	}

	public class TimerEventArgs : EventArgs {
		public TimeSpan gameClock;
		public TimeSpan homePen1;
		public TimeSpan homePen2;
		public TimeSpan awayPen1;
		public TimeSpan awayPen2;

		public TimerEventArgs(GameTimer t) {
			gameClock = t.gameClock;
			homePen1 = t.homePen1;
			homePen2 = t.homePen2;
			awayPen1 = t.awayPen1;
			awayPen2 = t.awayPen2;
		}

	}
}
