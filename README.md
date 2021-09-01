![](https://user-images.githubusercontent.com/31874317/131729066-24fd6ca3-779f-4d7a-896c-3303e078b98b.png)


The goal of Modheim is to allow quick and easy demployments of modpacks for Valheim. 

Ever had that sitaution of wanting a friend to play with you but they need to download a bunch of mods? Like 26 mods and spend over an hour configuring them all? No more! Just build your modpack in Modheim and send them the modpack. Modheim will take the pack and apply all of the same mods and configurations you did, all with the push of a button. 

Users can also change between mod profiles at the touch of a button, allowing them to change the game's modded setup to suit their needs, be it for playing on different servers or playing solo. 

By design, Modheim functions similarly to Minecraft Forge and MultiMC. Modpacks do not contain a list of mods, but rather the mods and their configurations themselves in a compressable format for Modheim to extract into the game.

- Maintainer: [Michael Cox](https://github.com/Arylos07)

![Screenshot_134](https://user-images.githubusercontent.com/31874317/131731009-cee748b1-66f4-42b1-af25-b820d04fb2e2.png)
![Screenshot_133](https://user-images.githubusercontent.com/31874317/131731020-5d121b5b-38d4-4a82-89fa-9be9101e9dff.png)
![Screenshot_136](https://user-images.githubusercontent.com/31874317/131731026-bc72fb35-6654-4175-9f3b-d4bbf39b74ac.png)
![Screenshot_137](https://user-images.githubusercontent.com/31874317/131731086-64918a69-06f0-4e20-8699-d1b9a2d0d45e.png)


Platforms
============
Modheim only works on Windows 64-bit right now. While it can be built for multiple platforms, the only other machine I have is an old repo of Linux Mint, making it not a good test rig. 
Contributors that now the Unity build pipeline are more than welcome to build for Mac/Linux and run tests and let me know if they work or need changes. Since Modheim works at the file system level, I feel as though it won't be an easy task. 

**32-bit is not supported at all.** I'm sorry but maintaining 32-bit has always been a pain in my career and since we're dealing with file systems and packaging/extracting files that is shared on the internet, there could be vulnerabilities.

Installation
============

Download & installation instructions are available in [the wiki](https://github.com/Arylos07/Modheim/wiki/how_to_use#how-do-i-install-modheim).

Status & Branches
=================

Modheim uses a [gitflow](https://nvie.com/posts/a-successful-git-branching-model/) style setup. The default branch holds the most recent stable release. Develop is the staging ground for ongoing work and the default target of most PRs. 

Contributing
============

Contributions are always welcome!
To contribute code or documentation to Modheim, please see [CONTRIBUTING](https://github.com/Arylos07/Modheim/blob/master/CONTRIBUTING.md).

Attribution
============

* Thanks to [Fungus](https://github.com/snozbot/fungus) for being a good template of a public repository. Most of my repo's are private so I rarely have to fully format a repository for public use.
* Thanks to [Yasirkula](https://github.com/yasirkula/UnitySimpleFileBrowser) for making a good Unity file browser that still works with .NET 2.0. It's instrumental in making sure this project works and got me out of a really bad rut.
* Thanks to [Richard Elms](https://github.com/richardelms/FileBasedPlayerPrefs) for making a very nice and extendable replacement to Unity's built-in PlayerPrefs system that has always been more than lacking. It's helped me on several projects.