SmartWalk Development
=====================

General Conventions
-------------------
4-Spaces as a Tab indentation should be used in all source code files.

Server Solution Setup
---------------------
In order to turn on Orchard source code browsing and debugging the compiled in Debug mode Orchard binaries with PDB files should be placed into binaries folder instead of Release ones. ReSharper plugin in VS may be used for easier navigation within external source code.

**[IMPORTANT]** It's recommended to use directory junction links (see examples Server/Misc/orchard-make-hardlink.txt) (mklink /j) to SmartWalk.Server project folder from Orchard.Web/Modules folder to sync the last changes of SmartWalk.Server module with Orchard in real-time. Also the SmartWalk.Server.Theme project folder should have a junction link in Orchard.Web/Themes folder. The theme should be enabled in Orchard Admin Dashboard.

**[IMPORTANT]** If Orchard solution is used as a main environment for server development using the regular references (not junction links) to SmartWalk.Server, SmartWalk.Server.Theme and its dependent projects is recommended to avoid paths resolution conflicts.

**[IMPORTANT]** To avoid conflict with resolving NuGet references in SmartWalk.Server project while using within Orchard solution, a custom NuGet.config (see examples Server/Misc/NuGet.config) file should be placed into Orchard solution root folder. It will redirect NuGet to SmartWalk solution packages folder:

	<config>
		<add key="repositorypath" value="C:\Git\smartwalk\packages" />
	</config>

**[IMPORTANT]**  To keep ReSharper settings synchronized between Orchard and SmartWalk.Server solutions all the original R#er settings files should be removed in Orchard root folder (*.ReSharper and *.DotSettings), our custom file should be put as default instead Orchard.sln.DotSettings (see examples Server/Misc/Orchard.sln.DotSettings). It will redirect ReSharper to SmartWalk solution settings:

	<s:String x:Key="/Default/Environment/InjectedLayers/FileInjectedLayer/=8054DDCCDFCFDA4AB36ED948952DCD4B/AbsolutePath/@EntryValue">
		C:\Git\smartwalk\SmartWalk.Server.sln.DotSettings
	</s:String>

Please update paths to smartwalk GIT root folder accordingly to your local environment config.

Publishing Server Solution to Azure
-----------------------------------

The Web Deploy method is preferable for publishing Orchard Solution to web-server. App_Data folder should be excluded. "Remove additional files at destination" should be turned on. Before publishing make sure that local Orchard instance has the same modules installed as server's one, this is to avoid its removing during publishing.

**[IMPORTANT]** The Orchard.Web/Media/Default folder (that may have local media gallery images) should be excluded from publishing to server. Please see a web deploy (see example Server/Misc/Web-Deploy.pub-xml) configuration, section <ExcludeFoldersFromDeployment />. This exclude folders section should be added into correspondent pubxml config in Orchard.Web/Properties/PublishProfiles/(your profile).pubxml.

Client Solution Setup
---------------------
The Xamarin Studio for Mac OS or Visual Studio with Xamarin Plugin should be used for opening and compiling SmartWalk.Client.sln solution.

Server and Client 3rd party Binaries Setup
-------------------------------------------
The referenced in solution projects binaries are not included into the repo. They should be referenced from another repo [smartwalk-binaries](https://github.com/showmap/smartwalk-binaries). It's recommended to clone this repo next to smartwalk one, so the project links will be valid.

All binaries are complied using Release mode. There are no pdb files stored in the binaries repo. If a debugging session is required the binaries should be overwritten by Debug version ones with PDBs, please refrain from pushing Debug binaries into the repo.

License
-------
[![Creative Commons License](https://i.creativecommons.org/l/by-nc-nd/4.0/88x31.png "Creative Commons License")](http://creativecommons.org/licenses/by-nc-nd/4.0/)

SmartWalk is licensed under a [Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License](http://creativecommons.org/licenses/by-nc-nd/4.0/).
