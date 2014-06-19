SmartWalk Development
=====================

General Conventions
-------------------
Every commit that is pushed to the repo must have a reference to a [JIRA](https://smartwalk.atlassian.net) issue. The ID of a correspondent issue should be written in the beginning of a commit description. For example:

git commit -m "SW-555 The brain bug with missing JIRA issue ids in commits was fixed"

4-Spaces as a Tab indentation should be used in all source code files. 

Server Solution Setup
---------------------
In order to turn on Orchard source code browsing and debugging the compiled in Debug mode Orchard binaries with PDB files should be placed into binaries folder instead of Release ones. ReSharper plugin in VS may be used for easier navigation within external source code.

**[IMPORTANT]** It's recommended to use directory [junction link](https://github.com/ievgen/smartwalk/blob/master/Server/Misc/orchard-make-hardlink.txt) (mklink /j) to SmartWalk.Server project folder from Orchard.Web/Modules folder to sync the last changes of SmartWalk.Server module with Orchard in real-time. Also the SmartWalk.Server.Theme project folder should have a junction link in Orchard.Web/Themes folder. The theme should be enabled in Orchard Admin Dashboard.

**[IMPORTANT]** If Orchard solution is used as a main environment for server development using the regular references (not junction links) to SmartWalk.Server, SmartWalk.Server.Theme and its dependent projects is recommended to avoid paths resolution conflicts.

**[IMPORTANT]** To avoid conflict with resolving NuGet references in SmartWalk.Server project while using within Orchard solution, a custom [NuGet.config](https://github.com/ievgen/smartwalk/blob/master/Server/Misc/NuGet.config) file should be placed into Orchard solution root folder. It will redirect NuGet to SmartWalk solution packages folder:

  <config>
    <add key="repositorypath" value="C:\Git\smartwalk\packages" />
  </config> 

Client Solution Setup
---------------------
The Xamarin Studio for Mac OS or Visual Studio with Xamarin Plugin should be used for opening and compiling SmartWalk.Client.sln solution.

Server and Client 3rd party Binaries Setup
-------------------------------------------
The referenced in solution projects binaries are not included into the repo. They should be referenced from another repo [smartwalk-binaries](https://github.com/ievgen/smartwalk-binaries). It's recommended to clone this repo next to smartwalk one, so the project links will be valid.

All binaries are complied using Release mode. There are no pdb files stored in the binaries repo. If a debugging session is required the binaries should be overwritten by Debug version ones with PDBs, please refrain from pushing Debug binaries into the repo.
