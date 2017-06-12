# SA-MP Guard
SA-MP Guard is a script for the multiplayer game GTA: San Andreas which extends the server scripters ability with a few new features, including cheat and hack checking.

**History:** The software was originally made for an Israeli gaming community of mine which contained a SA-MP gameserver. As times passed by, cheaters grew smarter and the community had problems blocking them. Then I had the idea of making a client-side app that will communicate with the server and report about files directly from the client's computer. Of course there may be a lot of workarounds to bypass the software, however, it was still preventing low-level cheaters at these times. Additionally, when I found out more about memory, I decided to try them out as I already had a networking system working between each player and the server, and then came the new features.

**Use:** It was originally made with block so only servers that are allowed to use the script could actually use it. However, as I haven't maintained this for a long time, I figured it'd be best to make it open-source, so developers can actually reactivate it if they learn how. Note that this can easily be hacked and requires developer maintaince in order to work.

**Files:**
- client: Contains the client-side desktop software, for Windows only (written in C#).
- server: Contains the server-side service, for SA-MP gameserver (written in Pawn). Requires Sockets plugin by BlueG. Also contains the .inc file in order to implement the features in any code.
- server/examples: Shows examples of working with the sGuard system. Note that any chat messages are written in Hebrew.
- website: Contains the files of the web services that are required for the system to work, as well as a full website that represents the system and contains a info/download pages and development information (functions usage, etc). Note that the website is written in Hebrew too.
