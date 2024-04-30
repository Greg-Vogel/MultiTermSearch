# MultiTermSearch
A quick and dirty search tool for searching a file system for files that contain one or more search terms.

![Image](Assets/Screenshot_1.png)


The list of files list allows you to export some of the data via a right-click context menu:

![Image](Assets/Screenshot_2.png)


## Notes on use
- If the directories you are searching for contain excessively large files you intend to search... change the Thread Count to something hire than 1 so you can continue scanning other files while the tool works on the larger files.
- To search for all file types just set the Include File Types field to:  `.*`.
- All search terms are turned into their own unique precompiled regex searches for efficiency... so if you choose to 'Match Whole Word' beware that it is already wrapping the terms in a `\b` tags.


## Future Updates
- An option to filter out binary file types from the wildcard search.
- Add a way to exclude file types when the `.*` option is entered in the Include File Types field.
- Add a Regex option to the Search Options section that allows the user to enter in their entirely own regex string.
