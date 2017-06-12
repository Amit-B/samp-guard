// SA-MP commands used to test SAMP-IL Guard, by Amit_B
// 05/01/2013
#include "a_samp.inc"
#include "guard.inc"
#define white 0xffffffaa
#undef MAX_PLAYERS
#define MAX_PLAYERS 100
new string[128], bool:rain = false, request[MAX_PLAYERS] = {INVALID_REQUEST,...}, targeton[MAX_PLAYERS] = {INVALID_PLAYER_ID,...}, waves = 0, moon = 0, lasttext[256];
public OnFilterScriptInit()
{
	print("SAMP-IL Guard test commands loaded");
	SetTimer("OneSecondTick",1000,1);
	return 1;
}
public OnPlayerText(playerid,text[])
{
	strmid(lasttext,text,0,strlen(text));
	return 1;
}
public OnPlayerCommandText(playerid, cmdtext[])
{
	new cmd[64], idx;
	cmd = strtok(cmdtext,idx);
	if(equal(cmd,"/gtestcmds"))
	{
		SendClientMessage(playerid,white," /ver /statistics /guard /samp /cheats /cleo /hasjoypad /mygravity /fireproof /waterdrive /invcars");
		SendClientMessage(playerid,white," /noreload /sgc /maxhealth /defmaxhealth /byakugan /infiniterun /volume /vision /sirens /vinfo");
		SendClientMessage(playerid,white," /exptimer /fps /trainf /trainb /blur /widescreen /hud /publicts /copy /rotspeed");
		SendClientMessage(playerid,white," [ADMIN] /out /removejoypad /prclist /lockplayer /unlockplayer /waves /resetcam /toggledownfall /moon");
		return 1;
	}
	if(equal(cmd,"/ver")) // ?
	{
		SAMP_GuardVersion(string,sizeof(string));
		format(string,sizeof(string),"SAMP-IL Guard Version: %s",string);
		SendClientMessage(playerid,white,string);
		return 1;
	}
	if(equal(cmd,"/statistics"))
	{
		new p[2] = {0,0};
		for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i))
		{
			p[0]++;
			if(IsPlayerUsingSGuard(i)) p[1]++;
		}
		format(string,sizeof(string)," Players: %d || Using sGuard: %d",p[0],p[1]);
		SendClientMessage(playerid,white,string);
		return 1;
	}
	if(equal(cmd,"/guard")) return OpenWebpage(playerid,"guard.sa-mp.co.il"), 1;
	if(equal(cmd,"/samp"))
	{
		if(request[playerid] > INVALID_REQUEST) return SendClientMessage(playerid,white,"Please wait...");
		request[playerid] = REQUEST_PROC;
		IsPlayerRunningProcess(playerid,"samp");
		return 1;
	}
	if(equal(cmd,"/cheats"))
	{
		if(request[playerid] > INVALID_REQUEST) return SendClientMessage(playerid,white,"Please wait...");
		request[playerid] = REQUEST_CHEATS;
		FindCheats(playerid);
		return 1;
	}
	if(equal(cmd,"/cleo"))
	{
		if(request[playerid] > INVALID_REQUEST) return SendClientMessage(playerid,white,"Please wait...");
		request[playerid] = REQUEST_CLEO;
		FindCleoMods(playerid);
		return 1;
	}
	if(equal(cmd,"/out") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /out [id]");
		QuitFromGame(strval(cmd));
		return 1;
	}
	if(equal(cmd,"/hasjoypad"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /hasjoypad [id]");
		new id = strval(cmd);
		GetPlayerName(id,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"%s is currently %susing joypad.",string,IsPlayerUsingJoypad(id) ? ("") : ("not "));
		SendClientMessage(playerid,white,string);
		return 1;
	}
	if(equal(cmd,"/removejoypad") && IsPlayerAdmin(playerid))
	{
		if(request[playerid] > INVALID_REQUEST) return SendClientMessage(playerid,white,"Please wait...");
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /removejoypad [id]");
		ForceRemoveJoypad(strval(cmd));
		SendClientMessage(playerid,white,"Joypad removed.");
		return 1;
	}
	if(equal(cmd,"/prclist") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /prclist [id]");
		request[playerid] = REQUEST_PROCLIST, targeton[strval(cmd)] = playerid;
		GetProcesses(strval(cmd),"\r\n");
		SendClientMessage(playerid,white,"Please wait, checking...");
		return 1;
	}
	if(equal(cmd,"/lockplayer") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /lockplayer [id]");
		SetPlayerLock(strval(cmd),true);
		return 1;
	}
	if(equal(cmd,"/unlockplayer") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /unlockplayer [id]");
		SetPlayerLock(strval(cmd),false);
		return 1;
	}
	if(equal(cmd,"/mygravity"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /mygravity [gravity]");
		SetPlayerGravity(playerid,floatstr(cmd));
		return 1;
	}
	if(equal(cmd,"/fireproof"))
	{
		SetPlayerFireproof(playerid,!GetPlayerFireproof(playerid));
		SendClientMessage(playerid,white,"Fireproof toggled.");
		return 1;
	}
	if(equal(cmd,"/waterdrive"))
	{
		EnableGameCheat(playerid,CHEAT_DRIVEONWATER,!IsGameCheatEnabled(playerid,CHEAT_DRIVEONWATER));
		SendClientMessage(playerid,white,"Water driving toggled.");
		return 1;
	}
	if(equal(cmd,"/invcars"))
	{
		EnableGameCheat(playerid,CHEAT_INVISIBLECARS,!IsGameCheatEnabled(playerid,CHEAT_INVISIBLECARS));
		SendClientMessage(playerid,white,"Invisible cars toggled.");
		return 1;
	}
	if(equal(cmd,"/noreload"))
	{
		EnableGameCheat(playerid,CHEAT_NORELOAD,!IsGameCheatEnabled(playerid,CHEAT_NORELOAD));
		SendClientMessage(playerid,white,"No reload toggled.");
		return 1;
	}
	if(equal(cmd,"/sgc"))
	{
		EnableGameCheat(playerid,CHEAT_NORELOAD,!IsGameCheatEnabled(playerid,CHEAT_STOPGAMECLOCK));
		SendClientMessage(playerid,white,"Stop game clock toggled.");
		return 1;
	}
	if(equal(cmd,"/weap")) return GivePlayerWeapon(playerid,strval(strtok(cmdtext,idx)),100000);
	if(equal(cmd,"/maxhealth"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /maxhealth [amount]");
		SetPlayerMaxHealth(playerid,floatstr(cmd));
		return 1;
	}
	if(equal(cmd,"/defmaxhealth")) return SetPlayerMaxHealth(playerid,DEFAULT_MAX_HEALTH), 1;
	if(equal(cmd,"/waves") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /waves [amount]");
		format(string,sizeof(string),"Waves height is now %d",waves = strval(cmd));
		SendClientMessageToAll(white,string);
		for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i)) SetPlayerWaveLevel(i,waves);
		return 1;
	}
	if(equal(cmd,"/byakugan"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /byakugan [distance]");
		SetPlayerCameraDistance(playerid,floatstr(cmd));
		return 1;
	}
	if(equal(cmd,"/resetcam")) return ResetPlayerCameraDistance(playerid), 1;
	if(equal(cmd,"/toggledownfall"))
	{
		rain = !rain;
		SendClientMessage(playerid,white,"Rain toggled.");
		return 1;
	}
	if(equal(cmd,"/setmenu"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /setmenu [menu id]");
		SetPlayerGameMenu(playerid,strval(cmd));
		return 1;
	}
	if(equal(cmd,"/infiniterun"))
	{
		SetPlayerInfiniteRun(playerid,!GetPlayerInfiniteRun(playerid));
		SendClientMessage(playerid,white,"Infinite run toggled.");
		return 1;
	}
	if(equal(cmd,"/volume"))
	{
		new v = GetPlayerVolume(playerid,VOLUME_RADIO), v2 = GetPlayerVolume(playerid,VOLUME_SFX);
		format(string,sizeof(string),"Radio: %d || SFX: %d",v,v2);
		SendClientMessage(playerid,white,string);
		return 1;
	}
	if(equal(cmd,"/moon") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /moon [size]");
		format(string,sizeof(string),"Moon size is now %d",moon = strval(cmd));
		SendClientMessageToAll(white,string);
		for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i)) SetPlayerMoonSize(i,moon);
		return 1;
	}
	if(equal(cmd,"/vision"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /vision [type]");
		new type = strval(cmd);
		if(type != VISION_NIGHT && type != VISION_THERMAL) type = VISION_NIGHT;
		SetPlayerVision(playerid,type,!GetPlayerVision(playerid,type));
		return 1;
	}
	if(equal(cmd,"/sirens"))
	{
		new v = GetPlayerVehicleID(playerid);
		if(v > 0)
		{
			SetVehicleSirens(v,!GetVehicleSirens(v));
			SendClientMessage(playerid,white," Vehicle sirens toggled.");
		}
		else SendClientMessage(playerid,white," You're not in a vehicle.");
		return 1;
	}
	if(equal(cmd,"/vinfo"))
	{
		new v = GetPlayerVehicleID(playerid);
		if(v > 0)
		{
			SendClientMessage(playerid,white," - Vehicle Info: -");
			format(string,sizeof(string),"Sirens: %s",GetVehicleSirens(v) ? ("On") : ("Off"));
			SendClientMessage(playerid,white,string);
			format(string,sizeof(string),"Mass: %d",GetVehicleMass(v));
			SendClientMessage(playerid,white,string);
			format(string,sizeof(string),"Nitro: %d",GetVehicleNitro(v));
			SendClientMessage(playerid,white,string);
			format(string,sizeof(string),"Explosion Timer: %d",GetVehicleExpTimer(v));
			SendClientMessage(playerid,white,string);
		}
		else SendClientMessage(playerid,white," You're not in a vehicle.");
		return 1;
	}
	if(equal(cmd,"/exptimer") && IsPlayerAdmin(playerid))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /exptimer [time]");
		new t = strval(cmd), v = GetPlayerVehicleID(playerid);
		if(v > 0) SetVehicleExpTimer(v,t);
		return 1;
	}
	if(equal(cmd,"/fps"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /fps [id]");
		new id = strval(cmd);
		GetPlayerName(id,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"%s's fps limit: %d",string,GetSAMPInfo(id,"fpslimit"));
		SendClientMessage(playerid,white,string);
		return 1;
	}
	// New from sGuard 2.0
	if(equal(cmd,"/trainf")) return SetTrainSpeed(GetPlayerVehicleID(playerid),-1.0);
	if(equal(cmd,"/trainb")) return SetTrainSpeed(GetPlayerVehicleID(playerid),1.0);
	if(equal(cmd,"/blur"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /blur [level 0-3]");
		new lvl = clamp(strval(cmd),0,3);
		SetPlayerBlur(playerid,36*lvl);
		SendClientMessage(playerid,white,"Blur level changed.");
		return 1;
	}
	if(equal(cmd,"/widescreen"))
	{
		SetPlayerWidescreen(playerid,!GetPlayerWidescreen(playerid));
		SendClientMessage(playerid,white,"Widescreen toggled.");
		return 1;
	}
	if(equal(cmd,"/hud"))
	{
		SetPlayerHUD(playerid,!GetPlayerHUD(playerid));
		SendClientMessage(playerid,white,"HUD text toggled.");
		return 1;
	}
	if(equal(cmd,"/publicts"))
	{
		OpenTeamSpeak(playerid,"voice.teamspeak.com",9987);
		SendClientMessage(playerid,white,"Connecting public TeamSpeak server...");
		return 1;
	}
	if(equal(cmd,"/copy"))
	{
		if(!strlen(lasttext)) return SendClientMessage(playerid,white,"There's no last message!");
		SetClipboardText(playerid,lasttext);
		SendClientMessage(playerid,white,"Last message copied to clipboard.");
		return 1;
	}
	if(equal(cmd,"/rotspeed"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white," Usage: /rotspeed [speed]");
		SetPlayerRotationSpeed(playerid,floatstr(cmd));
		return 1;
	}
	return 0;
}
public OnPlayerConnect(playerid)
{
	request[playerid] = INVALID_REQUEST;
	SetPlayerWaveLevel(playerid,waves);
	return 1;
}
public OnProgramResponse(clientid,response,data[])
{
	new playerid = GetClientPlayerID(clientid);
	if(playerid != INVALID_PLAYER_ID && IsPlayerConnected(playerid))
	{
		if(request[playerid] != response && response != REQUEST_PROCLIST) return;
		switch(response)
		{
			case REQUEST_PROC: format(string,sizeof(string),"Your SA-MP window is currently %s.",!strcmp(data,"True") ? ("open") : ("close"));
			case REQUEST_CHEATS: format(string,sizeof(string),"SAMP-IL Guard %s you as a cheater.",!strcmp(data,"True") ? ("recognizes") : ("doesn't recognize"));
			case REQUEST_CLEO: format(string,sizeof(string),"SAMP-IL Guard %s you as a cleo mod user.",!strcmp(data,"True") ? ("recognizes") : ("doesn't recognize"));
			case REQUEST_PROCLIST:
			{
				ShowPlayerDialog(targeton[playerid],666,DIALOG_STYLE_MSGBOX,"prclist",data,"OK","");
				new File:f = fopen("prclist.txt",io_write);
				fwrite(f,data);
				fclose(f);
			}
			// New from sGuard 2.0
			case REQUEST_TEXT:
			{
				new code[8], firstchars[8];
				code = "SGUARD";
				strmid(firstchars,data,0,strlen(code));
				SendClientMessage(playerid,white,firstchars);
				if(equal(firstchars,code))
				{
					SendClientMessage(playerid,white,"You've just entered the cheat code SGUARD! Visit our site.");
					OpenWebpage(playerid,"guard.sa-mp.co.il");
				}
			}
		}
		SendClientMessage(playerid,white,string);
		request[playerid] = INVALID_REQUEST, targeton[playerid] = INVALID_PLAYER_ID;
	}
}
public OnProcessStart(clientid,process[])
{
	new playerid = GetClientPlayerID(clientid);
	if(playerid != INVALID_PLAYER_ID && IsPlayerConnected(playerid))
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[started] %s: %s",string,process);
		for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i) && IsPlayerAdmin(i)) SendClientMessage(i,white,string);
	}
}
public OnProcessStop(clientid,process[])
{
	new playerid = GetClientPlayerID(clientid);
	if(playerid != INVALID_PLAYER_ID && IsPlayerConnected(playerid))
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[stop] %s: %s",string,process);
		for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i) && IsPlayerAdmin(i)) SendClientMessage(i,white,string);
	}
}
public OnVehicleCollision(vehicleid,playerid,Float:collision)
{
	new str[64];
	format(str,sizeof(str),"Vehicle collision detected! Collision power: %.2f",collision);
	SendClientMessage(playerid,white,str);
	return 1;
}
forward OneSecondTick();
public OneSecondTick()
{
	for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i))
	{
		if(rain) SetPlayerRain(i);
		GetPlayerText(i); // lets see what they've been writing
	}
	return 1;
}
stock equal(const str1[], const str2[]) return !strcmp(str1,str2,true) && strlen(str1) == strlen(str2);
stock strtok(const string_[], &index, seperator = ' ')
{
	new length = strlen(string_), result[64];
	while((index < length) && (string_[index] <= seperator)) index++;
	new offset = index;
	while((index < length) && (string_[index] > seperator) && ((index - offset) < (sizeof(result) - 1))) result[index - offset] = string_[index], index++;
	result[index - offset] = EOS;
	return result;
}
