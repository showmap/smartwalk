SmartWalk Development
=====================

General Conventions
-------------------
Every commit that is pushed to the repo must have a reference to a JIRA (https://smartwalk.atlassian.net) issue. The ID of a correspondent issue should be written in the beginning of a commit description. For example:

git commit -m "SW-555 The brain bug with missing JIRA issue ids in commits was fixed"

4-Spaces as a Tab indentation should be used in all source code files. 

Client Solution Setup
---------------------

The Xamarin Studio for Mac OS or Visual Studio with Xamarin Plugin should be used for opening and compiling SmartWalk.Client.sln solution.

The referenced in solution projects binaries are not included into the repo. It should be downloaded from public storage (https://docs.google.com/file/d/0B4_Y0szw-iF8VTY4dkJTaFdWREk) and extracted into Client/Binaries folder before compilation.
