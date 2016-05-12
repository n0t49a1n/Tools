Copyright (c) 2011 Carlos Ramzuel - Tlaxcala, Mexico.

WoWParser Version 2.0 Build 98 (October 30 2011)

This is an easy and small utility to read ADB, DB2, DBC and WDB files and convert it to CSV file format and CSV to DBC.

I developed this program, so any bugs related, any suggestions, etc, send me a private message to correct the problem or add new functions.

The program is linked with the static library so you don't need additional dependencies, but if you can't run the program for some strange reason, download VC Redistributable Package 2010.

WoWParser Features:
Support to read ADB, DB2, DBC and WDB files.
Reading WDB files is only supported with proper format in configuration file.
The program can predict field types but not in case of byte fields of ADB, DB2 and DBC files.
You can add specific format in configuration file to read byte fields or read all fields with your specific format.
Parsing ADB, DB2, DBC files will read the files using recursive mode in program directory or specific path for recursive or single file/directory in configuration file.
Reading CSV files will write DBC files using recursive mode only in program directory.
Configuration file sample are inside the zip.
Warnings: Reading values from CSV file for integer, float, and byte fields still no error message if you put an alphabetical character, normally in conversion to numeric value is zero '0', so beware.

Lastest Build: WoWParser2 Build 98 (October 30 2011)

Notes:
This will be the Final 2.0 Version
Soon Version 3 (Main features):
Configuration file changed to XML format.
Support to extract data to SQL file format.
Support to select name and order of columns in SQL and CSV output format.



