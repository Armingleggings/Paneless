# Paneless
Fixes annoying windows problems by crippling some it's more annoying features and fixing others.

# Basic operation

* Opening the file, you'll see a series of boxes that describe various fixes the tool can make to your system. You can toggle them on and off by clicking the yellow/green button. Each click will update the preferences file to store that preference for the future. 

* If the setting were to change for some reason (another tool or update of some kind changed the setting, Paneless will detect it and notify you by tagging the fix with a #preffilemismatch tag). The test cycle runs on a 10 second timer so there will be some delay before the change is found.

* UI options - You can use the Filter box to filter fixes by words or partial strings. You can click tags to refine listed fixes by matching tags (you can click tags in sequence to further refine the view). Clicking the tags in the Tag Filter area will remove a tag from the filter. You can also click the "Clear" button to clear both the filter and all tags (showing all available fixes again). 

## Buttons

### Apply Visible Fixes
When filters are active, the "Apply visible fixes" button will fix only the settings that are currently visible (not hidden). If no filters or tags are active, the "Apply visible fixes" will apply all fixes (because they are all visible).

### Save Prefs File
Saves all current fixes (and their state of on or off) to the prefs file. This is largely unnecessary since every time you click a fix, it saves all current preferences to the file, but has been put in for UX reasons and because of possible future development centered on leaving some fixes in a nuetral non-tracked state.

### Match prefs file
This button is used to resolve any fixes that are currently tagged with #preffilemismatch. This state can occur when someone manually edits a prefs file, use the "Load Prefs File" button, or system changes have altered the registry or settings that Paneless tracks. To quickly see what changes would be made by pressing this button, click a #prefsfilemismatch tag on any fix action to filter to that tag. To fix all mismatches, press the "Match Prefs File" button. To discard the changes, press the Save Prefs File button (which will save all current settings positions to the prefs file which resolves the deltas)

### Load Prefs File
This is centered on the idea of having multiple types of prefs files for different purposes. For example, individual customized prefs, pref files limited only to some features (like a "Windows Explorer Prefs" fix file or a "Windows 11 undo" grouping). Loading the file does NOT make changes. It simply highlights deltas with #preffilemismatch tags. It also replaces the Load Prefs File button with a Cancel button (removes the tags and returns the load prefs button). To actually apply the changes, you would click "match prefs file".
