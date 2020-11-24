This is a tool for resolving self-referential logic statements. It is intended mainly for use with Hollow Knight Randomizer.
The input folder should have the following 4 files. Templates for proper formatting are included in the release.
	- locations.xml
	- macros.xml
	- settings.xml
	- waypoints.xml
The program will allow you to select an initial waypoint from the waypoints.xml, and any combination of settings from settings.xml.
The order of simplification is:
	- All macros are directly substituted into logic when logic is parsed into RPN. If there is any self-referential behavior in macro logic, an error will occur.
	- If "Set settings flags for output" is enabled, logic which requires settings that are not enabled is removed, and all references to settings are removed.
	- Logic for the initial waypoint is set to empty, and logic for all other waypoints is computed inductively by substituting and reducing to minimal statements.
	- References to waypoints in location logic are replaced by the results of the previous step.
The new locations and waypoints logic will be printed in the Output folder.

The current build uses slow string comparisons to compare statements. Simplification may take a long time.