# Beer Routing - a different approach to teach routing algorithms.
Created by Jonas Kraus, Matthias Mak, Philipp Speidel, Fabian Widmann.
Advisors: Julian Frommel, Katja Rogers.

The game was developed in the WS15/16 term as one part of an individual project in the serious games (see more at the [media informatics department](https://www.uni-ulm.de/in/mi.html)) department.
___
# Goal
+ Create a working game
+ Teach the basics of several routing algorithms such as:
  + Dijkstra's algorithm
  + Greedy routing
  + Uniform Cost Search (to show the differences to Dijkstra's algorithm)
  + Policy based routing
+ Provide decent tutorials
___

# Comparison between both versions
![differences between both images](img/diff.png)
___
Half a year later, the goal is to create two sub versions of this game, one containing the same comic look and one containing a more simulation style. Additionally instead of having all the levels unlocked we only use dijkstra for our study. Generally this game will then be used in a course at the Ulm University in WS16/17 and evaluated.
# Todo
- [x] provide logging options
    - Example Log:
    ```
    Level= MaxScore=85, Type=Dijkstra	 current Time=2016-10-08T14:40:26
    2016-10-08T14:40:42: Valid Hop Discovery;	Player did perform a valid hop. destination=PathAB (PathScript);	Score% = 6
    2016-10-08T14:40:52: Valid Hop Discovery;	Player did perform a valid hop. destination=PathAC (PathScript);	Score% = 12
    2016-10-08T14:40:54: Valid Hop Discovery;	Player did perform a valid hop. destination=PathAD (PathScript);	Score% = 18
    2016-10-08T14:40:57: Valid Hop;	Player did perform a valid hop. destination=PathAC (PathScript);	Score% = 29
    2016-10-08T14:40:58: Valid Hop Discovery;	Player did perform a valid hop. destination=PathCB (PathScript);	Score% = 35
    2016-10-08T14:41:00: Valid Hop Discovery;	Player did perform a valid hop. destination=PathCF (PathScript);	Score% = 41
    2016-10-08T14:41:02: Valid Hop;	Player did perform a valid hop. destination=PathCB (PathScript);	Score% = 53
    2016-10-08T14:41:03: Valid Hop Discovery;	Player did perform a valid hop. destination=PathBE (PathScript);	Score% = 59
    2016-10-08T14:41:06: Valid Hop;	Player did perform a valid hop. destination=PathBE (PathScript);	Score% = 71
    2016-10-08T14:41:07: No Operation;	Player travels through completely handled destination=PathEB (PathScript);	Score% = 71
    2016-10-08T14:41:10: No Operation;	Player travels through completely handled destination=PathBC (PathScript);	Score% = 71
    2016-10-08T14:41:15: Valid Hop;	Player did perform a valid hop. destination=PathCF (PathScript);	Score% = 82
    2016-10-08T14:41:16: Valid Hop Discovery;	Player did perform a valid hop. destination=PathFD (PathScript);	Score% = 88
    2016-10-08T14:41:20: Valid Hop;	Player did perform a valid hop. destination=PathFD (PathScript);	Score% = 100
    2016-10-08T14:41:21: Level Finished; Player has finished the level;	Score% = 100
    2016-10-08T14:41:21: ButtonClicks; Professor Button clicks= 3, Routing Table Button clicks=2;	Score% = 100

    ```
- [ ] provide a means to send the log files
    - [ ] Uni email?
    - [ ] ftp?
- [ ] provide dual languages at least (German, English, ...)
  - [x] Added the Localization class that reads from the Json file in Resources\localization
- [ ] Add an option to enter a nickname to eventually link students to their points
- [ ] Alternate art version for the "simulation" style
  - [ ] Adapted texts
  - [ ] New tutorial
    - [ ] New texts, different level?
  - [ ] Adapted GUI
  - [ ] Adapt Sequences for the Tutorial
- [ ] New Levels (2 comic, 2 simulation, eventually even 2 more for paper exercises)
- ...

___
Runs fine on Unity 5.3.4f1 - in newer versions movement seems to be broken without a cause.
