# Scoreboard-Hockey
 
## Scoreboard v1.1.1

Scoreboard is designed to be used in tandem with OBS or other streaming software to display game information for an ice hockey game. This version of scoreboard creates a borderless window with a magenta background for OBS to capture directly. The magenta background can be filtered away in the scene using a chroma key in OBS.

Added Features:
- Swap Home and Away information positions in both the controlling window and the banner
- Better labelling of penalties and empty net situations
- Banner images can be changed in the interface
- game and team information written and read from file on close and start respectively
- more accurate display of time (OBS reading text files adds about a 1s delay)

## Scoreboard v1.0.0

Scoreboard is designed to be used in tandem with OBS or other streaming software to display game information for an ice hockey game. This version of scoreboard writes text files for the streaming software to read then display.

Text files are written for the following information:
- Game Clock
- Period
- Even Strength and Time
- Home Team Name
- Home Team Score
- Home Team Player Advantage and Time
- Away Team Name
- Away Team Score
- Away Team Player Advantage and Time

In a future release, the program will generate a separate window for OBS or other streaming software to capture directly.
