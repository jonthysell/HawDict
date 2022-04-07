# HawDict #

[![CI Build](https://github.com/jonthysell/HawDict/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/HawDict/actions/workflows/ci.yml)

HawDict creates offline versions of popular Hawaiian dictionaries in a variety of digital dictionary formats.

By default all files produced by HawDict will use the proper Unicode characters for the [ʻokina](http://www.olelo.hawaii.edu/olelo/puana/okina.php) and [kahakōs](http://www.olelo.hawaii.edu/olelo/puana/kahako.php): `ʻ ā ē ī ō ū Ā Ē Ī Ō Ū`. However, for (typically older) dictionary formats where these characters cannot be used, HawDict will substitute ASCII instead: `' a e i o u A E I O U`.

HawDict was written in C# and should run anywhere that supports [.NET 6.0](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md). It has been officially tested on:

* Windows 10

For help building/running HawDict and/or using the created dictionaries, please see the [HawDict Wiki](https://github.com/jonthysell/HawDict/wiki).

## Installation ##

### Windows ###

The Windows release is a self-contained x86/x64 binary which run on Windows 7 SP1+, 8.1, and 10.

1. Download the latest Windows zip file (HawDict.Win64.zip *or* HawDict.Win32.zip) from https://github.com/jonthysell/HawDict/releases/latest
2. Extract the zip file

**Note:** If you're unsure which version to download, try HawDict.Win64.zip first. Most modern PCs are 64-Bit.

### MacOS ###

The MacOS release is a self-contained x64 binary and runs on OSX >= 10.13.

1. Download the latest MacOS tar.gz file (HawDict.MacOS.tar.gz) from https://github.com/jonthysell/HawDict/releases/latest
2. Extract the tar.gz file

### Linux ###

The Linux release is a self-contained x64 binary and runs on many Linux distributions.

1. Download the latest Linux tar.gz file (HawDict.Linux.tar.gz) from https://github.com/jonthysell/HawDict/releases/latest
2. Extract the tar.gz file

## Source Dictionaries ##

HawDict creates offline versions of the following source dictionaries:

### PukuiElbert ###

*Hawaiian Dictionary, Revised and Enlarged Edition* by Mary Kawena Pūkuʻi and Samuel H. Elbert. ISBN 978-0824807030

The reference standard Hawaiian-English and English-Hawaiian dictionary.

Copyright (c) 1986 University of Hawaii Press

### MamakaKaiao ###

*Māmaka Kaiao: A Modern Hawaiian Vocabulary* by Kōmike Huaʻōlelo, Hale Kuamoʻo, ʻAha Pūnana Leo. ISBN 978-0824828035

A compilation of Hawaiian words that have been created, collected, and approved by the Hawaiian Lexicon Committee from 1987 through 2000.

Copyright (c) 2003 ʻAha Pūnana Leo and Hale Kuamoʻo, College of Hawaiian Language, University of Hawaiʻi at Hilo

### PlaceNames ###

*Place Names of Hawaii: Revised and Expanded Edition* by Mary Kawena Pūkuʻi, Samuel H. Elbert, Esther T. Moʻokini. ISBN 978-0824805241

The standard glossary of Hawaiian place names.

Copyright (c) 1974 The University Press of Hawaii

## Errata ##

HawDict is open-source under the MIT license.

HawDict does not include any copyrighted dictionary data - it is just a converter. HawDict uses the [Html Agility Pack](https://github.com/zzzprojects/html-agility-pack/) to download and parse the dictionary data from [Ulukau](https://ulukau.org/) at runtime. All dictionary data (terms, definitions, etc) is copyright their respective copyright owners.

HawDict Copyright (c) 2018-2022 Jon Thysell
