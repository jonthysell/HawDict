# HawDict #

HawDict creates offline versions of popular Hawaiian dictionaries in a variety of digital dictionary formats.

By default all files produced by HawDict will use the proper Unicode characters for the [ʻokina](http://www.olelo.hawaii.edu/olelo/puana/okina.php) and [kahakōs](http://www.olelo.hawaii.edu/olelo/puana/kahako.php): `ʻ ā ē ī ō ū Ā Ē Ī Ō Ū`. However, for (typically older) dictionary formats where these characters cannot be used, HawDict will substitute ASCII instead: `' a e i o u A E I O U`.

## Building HawDict ##

HawDict can be built in Visual Studio 2017.

The source for HawDict is provided as a Visual Studio 2017 solution in the `src` folder. Simply open HawDict.sln and build.

Note: Be sure to enable NuGet Packages Restore to get all of HawDict's dependencies.

## Running HawDict ##

HawDict requires .NET 4.5 or later to run.

Run HawDict.exe to produce a set of folders with Hawaiian dictionaries in different formats. The first time you run this app it will take some time as each source dictionary has to be downloaded.

You may optionally provide HawDict.exe with an alternate root path to output the folders.

## The Dictionaries ##

Each folder contains the results from a particular source dictionary. The files will be named according to the direction of the translation (Hawaiian to English or English to Hawaiian) and can be of the following types:

 * `*.clean.txt`: a simple tab-delimited text file of terms and definitions
 * `*.dict.xdxf`: an XDXF structured XML file
  * `*.StarDict.*`: the StarDict formatted binary files
 * `*.html.tmp`: a temporary file with the raw source data (helps reduce bandwidth usage)

### PukuiElbert ###

*Hawaiian Dictionary, Revised and Enlarged Edition* by Mary Kawena Pūkuʻi and Samuel H. Elbert. ISBN 978-0824807030

The reference standard Hawaiian-English and English-Hawaiian dictionary.

Copyright (c) 1986 University of Hawaii Press

### MamakaKaiao ###

*Māmaka Kaiao: A Modern Hawaiian Vocabulary* by Kōmike Huaʻōlelo, Hale Kuamoʻo, ʻAha Pūnana Leo. ISBN 978-0824828035

A compilation of Hawaiian words that have been created, collected, and approved by the Hawaiian Lexicon Committee from 1987 through 2000.

Copyright (c) 2003 ʻAha Pūnana Leo and Hale Kuamoʻo, College of Hawaiian Language, University of Hawaiʻi at Hilo

## Errata ##

HawDict is open-source under the MIT license.

All source data (terms, definitions, etc) are copyright their respective copyright owners.

HawDict Copyright (c) 2018 Jon Thysell
