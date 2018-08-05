// SA-MP Guard v2.0 by Amit_B
// SA-MP.co.il
#include "a_samp.inc"
#include "a_http.inc"
#include "socket.inc"
// Defines:
#define MAX_CONNECTIONS 250
//#define SOCKET_BIND "127.0.0.1"
#define SOCKET_TYPE TCP
#define MAX_SOCKETS_PER_RECIEVE 8
#define MAX_SOCKET_LENGTH 512
#define MAX_IP_CONNECTIONS 3
#define ALLOW_MSG
//#define DEBUG
#define VERSION "2.0"
// Colors:
#define white 0xffffffff
#define red 0xff0000ff
#define green 0x00ff00ff
#define blue 0x0000ffff
// Requests:
#define INVALID_REQUEST -001
#define REQUEST_PROC 000
#define REQUEST_CHEATS 001
#define REQUEST_CLEO 002
#define REQUEST_PROCLIST 003
#define REQUEST_HWID 004
// Directive Functions:
#define equal(%1,%2) (!strcmp(%1,%2,true) && strlen(%1) == strlen(%2))
#define f(%1) (format(string,sizeof(string),%1), string)
enum ClientData
{
	bool:logged,
	disconnectReason[64],
	gameID,
	gameName[MAX_PLAYER_NAME],
	IP[16],
	cIdleTime,
	cArrayPos
}
enum PlayerData
{
	clientID,
	bool:updating,
	bool:kicking,
	request[2],
	idleTime,
	arrayPos,
	lastVehicle,
	hwid[64],
	/*	radio = 0, fireproof = 1, cheat_invisiblecars = 2,
		cheat_driveonwater = 3, cheat_noreload = 4, joypad = 5, playerstate = 6,
		jumpstate = 7, runstate = 8, lock = 9, menu = 10, town = 11,
		infiniterun = 12, radio = 13, sfx = 14, afk = 15, vsirens = 16,
		vhorn = 17, moonsize = 18, cheat_stopgameclock = 19, nightvision = 20,
		thermalvision = 21, hud = 22, blur = 23, stamina = 24, pclip = 25,
		sclip = 26, uclip = 27, aclip = 28, widescreen = 29
	*/
	#define max_infoarrayint 30
	infoArray_Int[max_infoarrayint],
	/*	gravity = 0, maxhealth = 1, wavelevel = 2, vmass = 3, vnitro = 4,
		vexplode = 5, trainspeed = 6, rotationspeed = 7
	*/
	#define max_infoarrayfloat 8
	Float:infoArray_Float[max_infoarrayfloat],
	/*	timestamp = 0, disableheadmove = 1, fpslimit = 2, pagesize = 3,
		multicore = 4, audioproxyoff = 5, audiomsgoff = 6
	*/
	#define max_infoarraysamp 7
	infoArray_SAMP[max_infoarraysamp]
}
new Socket:S, string[256], MAX_CONNECTIONS_ = MAX_CONNECTIONS,
	clientInfo[MAX_CONNECTIONS][ClientData], bool:clientsOnly = false,
	playerInfo[MAX_PLAYERS][PlayerData], players = 0, bind[16],
	player[MAX_PLAYERS] = {INVALID_PLAYER_ID,...}, bool:serverAllowed = false,
	splitedData[MAX_SOCKETS_PER_RECIEVE][MAX_SOCKET_LENGTH],
	vDr[MAX_VEHICLES] = {INVALID_PLAYER_ID,...}/*, clients = 0,
	client[MAX_CONNECTIONS] = {INVALID_CLIENT_ID,...}*/, bool:initialize = false;
public OnFilterScriptInit()
{
	GetServerVarAsString("bind",bind,sizeof(bind));
	if(!strlen(bind))
	{
		print("SA-MP Guard can't be loaded: you should add a \"bind\" line with your server IP to server.cfg.");
		return SendRconCommand("exit");
	}
	S = socket_create(SOCKET_TYPE);
	socket_set_max_connections(S,MAX_CONNECTIONS_);
	#if defined SOCKET_BIND
		socket_bind(S,SOCKET_BIND);
	#endif
	if(equal(bind,"127.0.0.1"))
	{
		format(string,sizeof(string),"%d8",GetServerVarAsInt("port"));
		strdel(string,3,4);
		socket_listen(S,strval(string));
	}
	else HTTP(0,HTTP_GET,f("guard.sa-mp.co.il/assign/request.php?bind=%d",GetServerVarAsInt("port")),"","Guard_ListenToPort");
	for(new i = 0; i < MAX_CONNECTIONS; i++) ResetInfo(i,true);
	#if defined ALLOW_MSG
		SendClientMessageToAll(white,"SA-MP Guard loaded successfully");
	#endif
	SetTimer("Guard_UpdateIdleTime",1000,1);
	SetTimer("Guard_TenSecondsPass",10000,0);
	HTTP(0,HTTP_GET,"guard.sa-mp.co.il/servers.txt","","Guard_DoesServerHavePermission");
	if(existproperty(.name="sguard_co")) clientsOnly = bool:getproperty(.name="sguard_co");
	initialize = true;
	for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i)) OnPlayerConnect(i);
	initialize = false;
	print("SA-MP Guard loaded successfully");
	return 1;
}
public OnFilterScriptExit()
{
	if(is_socket_valid(S))
	{
		socket_stop_listen(S);
		socket_destroy(S);
	}
	#if defined ALLOW_MSG
		SendClientMessageToAll(white,"SA-MP Guard unloaded successfully");
	#endif
	return 1;
}
forward Guard_DoesServerHavePermission(index,response_code,data[]);
public Guard_DoesServerHavePermission(index,response_code,data[])
{
	#pragma unused index
	#pragma unused response_code
	if(strfind(data,f("%s:%d",bind,GetServerVarAsInt("port")),true) == -1) NoPermissionExit();
	else
	{
		printf("SA-MP Guard permission granted.");
		serverAllowed = true;
	}
}
forward Guard_TenSecondsPass(index,response_code,data[]);
public Guard_TenSecondsPass(index,response_code,data[])
{
	#pragma unused index
	#pragma unused response_code
	#pragma unused data
	if(!serverAllowed) NoPermissionExit();
}
forward Guard_ListenToPort(index,response_code,data[]);
public Guard_ListenToPort(index,response_code,data[])
{
	#pragma unused index
	#pragma unused response_code
	socket_listen(S,strval(data));
	printf("SA-MP Guard port: %s",data);
}
public OnPlayerConnect(playerid)
{
	ResetInfo(playerid,false);
	player[playerInfo[playerid][arrayPos] = (players++)] = playerid;
	if(!IsPlayerNPC(playerid) && !initialize)
	{
		for(new i = 0; i < MAX_CONNECTIONS_ && playerInfo[playerid][clientID] == INVALID_CLIENT_ID; i++) if(clientInfo[i][logged]) if(clientInfo[i][gameID] == INVALID_PLAYER_ID && equal(UniqueID(playerid,true),UniqueID(i,false)))
		{
			playerInfo[playerid][clientID] = i, clientInfo[i][gameID] = playerid;
			#if defined DEBUG
				printf("Client %03d added to PID: %03d",i,playerid);
			#endif
			SendToClient(playerInfo[playerid][clientID],f("id:%d",playerid));
			#if defined ALLOW_MSG
				SendClientMessage(playerid,white,f("You've been recognized as client ID %03d in SA-MP Guard",i));
				SendClientMessageToAll(green,f("%s has connected with SA-MP Guard",GetName(playerid)));
			#endif
		}
		print("sg-3");
		if(clientsOnly && playerInfo[playerid][clientID] == INVALID_CLIENT_ID)
		{
			#if defined ALLOW_MSG
				SendClientMessage(playerid,red,"The server recognized that you're not using SA-MP Guard.");
				SendClientMessage(playerid,red,"This is a software that helps us recognize hackers and cheaters, you must install it in order to play.");
				SendClientMessage(playerid,green,"Please download from the following website: Guard.SA-MP.co.il");
			#endif
			return Out(playerid), 0;
		}
	}
	return 1;
}
public OnPlayerDisconnect(playerid, reason)
{
	for(new i = playerInfo[playerid][arrayPos]; i < players - 1; i++) player[i] = player[i+1];
	player[players--] = INVALID_PLAYER_ID;
	playerInfo[playerid][kicking] = false;
	if(!IsPlayerNPC(playerid)) if(playerInfo[playerid][clientID] != INVALID_CLIENT_ID) if(clientInfo[playerInfo[playerid][clientID]][gameID] != INVALID_PLAYER_ID)
	{
		clientInfo[playerInfo[playerid][clientID]][gameID] = INVALID_PLAYER_ID;
		#if defined DEBUG
			printf("Client %03d removed from PID: %03d",playerInfo[playerid][clientID],playerid);
		#endif
		SendToClient(playerInfo[playerid][clientID],"id:-1");
	}
	ResetInfo(playerid,false);
	return 1;
}
public OnPlayerUpdate(playerid)
{
	playerInfo[playerid][idleTime] = 0;
	return _:playerInfo[playerid][updating];
}
public OnPlayerCommandText(playerid, cmdtext[])
{
	new cmd[64], idx;
	cmd = strtok(cmdtext,idx);
	if(equal(cmdtext,"/sgclients"))
	{
		SendClientMessage(playerid,blue,"Connected to sGuard:");
		for(new i = 0, c = 0; i < MAX_CONNECTIONS_; i++) if(clientInfo[i][logged])
		{
			c++;
			if(clientInfo[i][gameID] == INVALID_PLAYER_ID) SendClientMessage(playerid,white,f(" %d) %s [Offline]",c,clientInfo[i][gameName]));
			else SendClientMessage(playerid,green,f(" %d) %s [Player ID: %03d]",c,clientInfo[i][gameName],clientInfo[i][gameID]));
		}
		return 1;
	}
	if(equal(cmdtext,"/sgplayers"))
	{
		SendClientMessage(playerid,blue,"Connected to SA-MP Server:");
		for(new i = 0, c = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i))
		{
			c++;
			if(playerInfo[i][clientID] == INVALID_CLIENT_ID) SendClientMessage(playerid,white,f(" %d) %s [Offline]",c,GetName(i)));
			else SendClientMessage(playerid,green,f(" %d) %s [sGuard Client ID: %03d]",c,GetName(i),playerInfo[i][clientID]));
		}
		return 1;
	}
	if(equal(cmd,"/sgcheck"))
	{
		cmd = strtok(cmdtext,idx);
		if(!strlen(cmd)) return SendClientMessage(playerid,white,"USAGE: /sgcheck [id]");
		new id = strval(cmd);
		if(!IsPlayerConnected(id)) return SendClientMessage(playerid,red,"Invalid ID.");
		if(playerInfo[id][clientID] == INVALID_CLIENT_ID) return SendClientMessage(playerid,red,"This player is not using sGuard.");
		SendClientMessage(playerid,white,f("%s's is using sGuard with Client ID: %03d",GetName(id),playerInfo[id][clientID]));
		return 1;
	}
	return 0;
}
public onSocketRemoteConnect(Socket:id, remote_client[], remote_clientid)
{
	assert id == S;
	new c = 0;
	for(new i = 0, ip[16]; i < MAX_CONNECTIONS_ && c < MAX_IP_CONNECTIONS; i++) if(clientInfo[i][logged])
	{
		if(remote_clientid == i) format(ip,sizeof(ip),remote_client);
		else get_remote_client_ip(id,i,ip);
		if(equal(ip,remote_client)) c++;
	}
	if(c >= MAX_IP_CONNECTIONS)
	{
		#if defined DEBUG
			printf("Too many connections, removed: %s (%03d)",remote_client,remote_clientid);
		#endif
		socket_close_remote_client(id,remote_clientid);
	}
	else
	{
		format(clientInfo[remote_clientid][IP],16,remote_client);
		#if defined DEBUG
			printf("Incoming connection: %s (%03d)",remote_client,remote_clientid);
		#endif
	}
	return 1;
}
public onSocketReceiveData(Socket:id, remote_clientid, data[], data_len)
{
	//assert id == S;
	if(data_len > 200) return 1;
	if(id != S || remote_clientid == INVALID_CLIENT_ID) return 1;
	new cmd[128], idx, p;
	if(clientInfo[remote_clientid][logged]) clientInfo[remote_clientid][cIdleTime] = 0;
	if((p = strfind(data,";",true)) != -1)
	{
		if(p == data_len-1) data[(data_len--)-1] = EOS;
		else
		{
			split(data,splitedData,';');
			for(new i = 0; i < MAX_SOCKETS_PER_RECIEVE; i++) if(strlen(splitedData[i]) > 1) onSocketReceiveData(id,remote_clientid,splitedData[i],strlen(splitedData[i]));
			return 1;
		}
	}
	#if defined DEBUG
		printf("Recieved from %03d: %s",remote_clientid,data);
		//strmid(cmd,data,0,128);
		//SendClientMessageToAll(white,f("Recieved from %03d: %s",remote_clientid,cmd));
	#endif
	cmd = strtok(data,idx);
	if(equal(cmd,"connect"))
	{
		assert serverAllowed;
		cmd = strtok(data,idx);
		new vers[64], bool:already = false;
		for(new i = 0; i < MAX_CONNECTIONS_ && !already; i++) if(clientInfo[i][logged]) if(equal(cmd,UniqueID(i,false))) already = true;
		if(already) return socket_close_remote_client(id,remote_clientid);
		format(vers,sizeof(vers),strrest(data,idx));
		if(!equal(vers,VERSION)) SendToClient(remote_clientid,f("badversion:%s",VERSION));
		else
		{
			ResetInfo(remote_clientid,true);
			clientInfo[remote_clientid][logged] = true;
			clientInfo[remote_clientid][gameID] = INVALID_PLAYER_ID;
			//client[clientInfo[playerid][cArrayPos] = (clients++)] = remote_clientid;
			strmid(cmd,cmd,0,strfind(cmd,"/"));
			format(clientInfo[remote_clientid][gameName],MAX_PLAYER_NAME,cmd);
			for(new i = 0; i < players && clientInfo[remote_clientid][gameID] == INVALID_PLAYER_ID; i++) if(IsPlayerConnected(player[i])) if(playerInfo[player[i]][clientID] == INVALID_CLIENT_ID && equal(UniqueID(player[i],true),UniqueID(remote_clientid,false))) playerInfo[player[i]][clientID] = remote_clientid, clientInfo[remote_clientid][gameID] = player[i], clientInfo[remote_clientid][logged] = true;
			SendToClient(remote_clientid,f("connected:%d:%d",remote_clientid,clientInfo[remote_clientid][gameID] == INVALID_PLAYER_ID ? -1 : clientInfo[remote_clientid][gameID]));
			#if defined ALLOW_MSG
				if(clientInfo[remote_clientid][gameID] != INVALID_PLAYER_ID) SendClientMessage(clientInfo[remote_clientid][gameID],white,f("You're now connected to SA-MP Guard as client ID %03d.",remote_clientid));
			#endif
			#if defined DEBUG
				printf("Connected: %s (CID: %03d PID: %03d IP: %s)",clientInfo[remote_clientid][gameName],remote_clientid,clientInfo[remote_clientid][gameID],clientInfo[remote_clientid][IP]);
				//SendClientMessageToAll(white,f(" .SA-MP Guard התחבר לתוכנת %s",clientInfo[remote_clientid][gameName]));
			#endif
		}
	}
	else if(equal(cmd,"disconnect")) clientInfo[remote_clientid][disconnectReason] = strrest(data,idx);
	else if(equal(cmd,"response"))
	{
		cmd = strtok(data,idx);
		new retrieved[1500], i = strval(cmd);
		format(cmd,sizeof(cmd),"response %d ",i);
		strmid(retrieved,data,strfind(data,cmd)+strlen(cmd),data_len,sizeof(retrieved));
		CallRemoteFunction("OnProgramResponse","iis",remote_clientid,i,strspace(retrieved));
	}
	else if(equal(cmd,"infoint"))
	{
		new playerid = INVALID_PLAYER_ID;
		if((playerid = clientInfo[remote_clientid][gameID]) == INVALID_PLAYER_ID) return 1;
		if(!IsPlayerConnected(playerid)) return 1;
		for(new i = 0; i < max_infoarrayint; i++)
		{
			cmd = strtok(data,idx);
			playerInfo[playerid][infoArray_Int][i] = strval(cmd);
		}
	}
	else if(equal(cmd,"infofloat"))
	{
		new playerid = INVALID_PLAYER_ID;
		if((playerid = clientInfo[remote_clientid][gameID]) == INVALID_PLAYER_ID) return 1;
		if(!IsPlayerConnected(playerid)) return 1;
		for(new i = 0; i < max_infoarrayfloat; i++)
		{
			cmd = strtok(data,idx);
			playerInfo[playerid][infoArray_Float][i] = floatstr(cmd);
		}
	}
	else if(equal(cmd,"infosamp"))
	{
		new playerid = INVALID_PLAYER_ID;
		if((playerid = clientInfo[remote_clientid][gameID]) == INVALID_PLAYER_ID) return 1;
		if(!IsPlayerConnected(playerid)) return 1;
		for(new i = 0; i < max_infoarraysamp; i++)
		{
			cmd = strtok(data,idx);
			playerInfo[playerid][infoArray_SAMP][i] = strval(cmd);
		}
	}
	else if(equal(cmd,"prctrace"))
	{
		cmd = strtok(data,idx);
		new i = strval(cmd);
		cmd = strrest(data,idx);
		if(i == 0 && funcidx("OnProcessStart") != -1) CallRemoteFunction("OnProcessStart","is",remote_clientid,strspace(cmd));
		else if(i == 1 && funcidx("OnProcessStop") != -1) CallRemoteFunction("OnProcessStop","is",remote_clientid,strspace(cmd));
	}
	else if(equal(cmd,"hwid"))
	{
		new playerid = INVALID_PLAYER_ID;
		if((playerid = clientInfo[remote_clientid][gameID]) == INVALID_PLAYER_ID) return 1;
		if(!IsPlayerConnected(playerid)) return 1;
		cmd = strrest(data,idx);
		format(playerInfo[playerid][hwid],64,cmd);
	}
	else if(equal(cmd,"collision"))
	{
		new playerid = INVALID_PLAYER_ID;
		if((playerid = clientInfo[remote_clientid][gameID]) == INVALID_PLAYER_ID) return 1;
		if(!IsPlayerConnected(playerid)) return 1;
		cmd = strtok(data,idx);
		CallRemoteFunction("OnVehicleCollision","iif",GetPlayerVehicleID(playerid),playerid,floatstr(cmd));
	}
	//strmid(cmd,data,0,strlen(data) > 50 ? 50 : strlen(data));
	//format(cmd,sizeof(cmd),"cmdsent:%s",data);
	//SendToClient(remote_clientid,cmd);
	return 1;
}
public onSocketRemoteDisconnect(Socket:id, remote_clientid)
{
	assert id == S;
	#if defined DEBUG
		printf("Incoming disconnect from %03d",remote_clientid);
	#endif
	assert clientInfo[remote_clientid][logged];
	//for(new i = clientInfo[playerid][cArrayPos]; i < clients - 1; i++) client[i] = client[i+1];
	//client[clients--] = INVALID_CLIENT_ID;
	#if defined DEBUG
		printf("Disconnected %03d",remote_clientid);
		if(clientInfo[remote_clientid][logged])
		{
			#if defined ALLOW_MSG
				/*if(!strlen(clientInfo[remote_clientid][disconnectReason])) SendClientMessageToAll(white,f("%s is no longer connected to SA-MP Guard. Unknown reason.",GetName(clientInfo[remote_clientid][gameID])));
				else
				{
					SendClientMessageToAll(white,f("%s is no longer connected to SA-MP guard. Reason:",GetName(clientInfo[remote_clientid][gameID])));
					SendClientMessageToAll(white,clientInfo[remote_clientid][disconnectReason]);
				}*/
			#endif
		}
	#endif
	clientInfo[remote_clientid][logged] = false;
	if(clientInfo[remote_clientid][gameID] != INVALID_PLAYER_ID) Out(clientInfo[remote_clientid][gameID]);
	ResetInfo(remote_clientid,true);
	return 1;
}
stock ResetInfo(id,bool)
{
	if(bool)
	{
		clientInfo[id][logged] = false;
		clientInfo[id][disconnectReason][0] = EOS;
		clientInfo[id][gameID] = INVALID_PLAYER_ID;
		clientInfo[id][gameName][0] = EOS;
		clientInfo[id][cIdleTime] = 0;
		clientInfo[id][cArrayPos] = -1;
	}
	else
	{
		playerInfo[id][clientID] = INVALID_CLIENT_ID;
		playerInfo[id][updating] = true;
		playerInfo[id][kicking] = false;
		playerInfo[id][idleTime] = 0;
		playerInfo[id][arrayPos] = -1;
		playerInfo[id][lastVehicle] = INVALID_VEHICLE_ID;
		playerInfo[id][hwid][0] = EOS;
		for(new i = 0; i < max_infoarrayint; i++) playerInfo[id][infoArray_Int][i] = -1;
		for(new i = 0; i < max_infoarrayfloat; i++) playerInfo[id][infoArray_Float][i] = -1.0;
		for(new i = 0; i < max_infoarraysamp; i++) playerInfo[id][infoArray_SAMP][i] = -1;
	}
	return 1;
}
stock SendToAll(data[]) for(new i = 0; i < MAX_CONNECTIONS_; i++) if(clientInfo[i][logged]) SendToClient(i,data);
stock SendToClient(clientid,data[])
{
	#if defined DEBUG
		printf("Sent to %03d: %s",clientid,data);
		//SendClientMessageToAll(white,f("Sent to %03d: %s",clientid,data));
	#endif
	if(strfind(data,";") == -1) socket_sendto_remote_client(S,clientid,f("%s;",data));
}
stock IsNumeric(const string_[])
{
	for(new i = string_[0] == '-' ? 1 : 0, j = strlen(string_); i < j; i++) if(string_[i] < '0' || string_[i] > '9') return 0;
	return 1;
}
stock strtok(const string_[], &index, somechar = ' ')
{   // by CompuPhase, improved by me
	new length = strlen(string_), result[64];
	while((index < length) && (string_[index] <= somechar)) index++;
	new offset = index;
	while((index < length) && (string_[index] > somechar) && ((index - offset) < (sizeof(result) - 1))) result[index - offset] = string_[index], index++;
	result[index - offset] = EOS;
	return result;
}
stock strrest(const string_[], index, somechar = ' ')
{   // by CompuPhase, improved by me
	new length = strlen(string_), offset = index, result[64];
	while((index < length) && ((index - offset) < (sizeof(result) - 1)) && (string_[index] > '\r')) result[index - offset] = string_[index], index++;
	result[index - offset] = EOS;
	if(result[0] == somechar && string_[0] != somechar) strdel(result,0,1);
	return result;
}
stock GetName(playerid)
{
	new n[MAX_PLAYER_NAME];
	GetPlayerName(playerid,n,sizeof(n));
	return n;
}
stock GetIP(playerid)
{
	new i[16];
	GetPlayerIp(playerid,i,sizeof(i));
	return i;
}
stock UniqueID(id,bool:pla)
{
	new uid[64];
	return ((pla ? format(uid,sizeof(uid),"%s/%s",GetName(id),GetIP(id)) : format(uid,sizeof(uid),"%s/%s",clientInfo[id][gameName],clientInfo[id][IP])), uid);
}
stock Out(playerid)
{
	playerInfo[playerid][updating] = false, playerInfo[playerid][kicking] = true;
	SetTimerEx("Guard_KickPlayer",500,0,"i",playerid);
	return 1;
}
stock split(const strsrc[],strdest[][],delimiter,maxlen=sizeof strdest[])
{
	new i, li, aNum, len, len2 = strlen(strsrc);
	while(i <= len2)
	{
		if(strsrc[i] == delimiter || i == len2)
		{
			len = strmid(strdest[aNum],strsrc,li,i,maxlen);
			strdest[aNum][len] = 0;
			li = i + 1;
			aNum++;
		}
		i++;
	}
	return 1;
}
stock set(dest[],source[],maxlength)
{
	new count = strlen(source);
	for(new i = 0; i < count && i < maxlength; i++) dest[i] = source[i];
	dest[count] = 0;
	return 1;
}
stock NoPermissionExit()
{
	print("SA-MP Guard: You do not have the permission to use this system. For more information, see Guard.SA-MP.co.il");
	print("- Server is shutting down");
	SendRconCommand("exit");
	for(;;) exit;
}
stock GetKeyNumber(key[])
{
	if(equal(key,"timestamp")) return 0;
	else if(equal(key,"disableheadmove")) return 1;
	else if(equal(key,"fpslimit")) return 2;
	else if(equal(key,"pagesize")) return 3;
	else if(equal(key,"multicore")) return 4;
	else if(equal(key,"audioproxyoff")) return 5;
	else if(equal(key,"audiomsgoff")) return 6;
	return -1;
}
stock GetCheatArrayPos(cheatid)
{
	switch(cheatid)
	{
		case 1..3: return cheatid+1;
		case 4: return 19;
	}
	return -1;
}
public OnPlayerStateChange(playerid,newstate,oldstate)
{
	new v = GetPlayerVehicleID(playerid);
	if(newstate == PLAYER_STATE_DRIVER && v > 0)
	{
		if(playerInfo[playerid][lastVehicle] != INVALID_VEHICLE_ID) vDr[playerInfo[playerid][lastVehicle]] = INVALID_PLAYER_ID;
		vDr[v] = playerid, playerInfo[playerid][lastVehicle] = v;
	}
	else if(!v && playerInfo[playerid][lastVehicle] != INVALID_VEHICLE_ID) vDr[playerInfo[playerid][lastVehicle]] = INVALID_PLAYER_ID, playerInfo[playerid][lastVehicle] = INVALID_VEHICLE_ID;
	return 1;
}
forward Guard_KickPlayer(playerid);
public Guard_KickPlayer(playerid)
{
	assert playerInfo[playerid][kicking];
	return Kick(playerid), 1;
}
forward Guard_UpdateIdleTime();
public Guard_UpdateIdleTime()
{
	for(new i = 0, m = max(players,MAX_CONNECTIONS_); i < m; i++)
	{
		if(i < players) if(IsPlayerConnected(player[i]))playerInfo[player[i]][idleTime]++;
		if(i < MAX_CONNECTIONS_) if(clientInfo[i][logged])
			if(clientInfo[i][cIdleTime] > 120)
			{
				SendToClient(i,"auto");
				socket_close_remote_client(S,i);
				if(IsPlayerConnected(clientInfo[i][gameID])) Kick(clientInfo[i][gameID]);
				ResetInfo(i,true);
				#if defined DEBUG
					printf("Removed automatically: %03d",i);
				#endif
			}
			else clientInfo[i][cIdleTime]++;
	}
}
stock prvCloseSGuardConnection(playerid)
{
	new c = playerInfo[playerid][clientID];
	SendToClient(c,"close");
	Kick(playerid);
	socket_close_remote_client(S,c);
}
forward Float:fclamp(Float:n,Float:mn,Float:mx);
stock Float:fclamp(Float:n,Float:mn,Float:mx)
{
	if(n < mn) n = mn;
	if(n > mx) n = mx;
	return n;
}
stock strspace(str[])
{
	new str_ret[256];
	format(str_ret,sizeof(str_ret),str);
	if(!strlen(str_ret)) format(str_ret,2," ");
	return str_ret;
}
//============================= [ Function List ] ==============================
#define isClientAPlayer(%1) (%1 >= 0 && %1 < MAX_CONNECTIONS_ && clientInfo[%1][gameID] != INVALID_PLAYER_ID && playerInfo[clientInfo[%1][gameID]][idleTime] < 6)
#define isPlayerAClient(%1) (%1 >= 0 && %1 < MAX_PLAYERS && playerInfo[%1][clientID] != INVALID_CLIENT_ID && playerInfo[%1][idleTime] < 6 && !IsPlayerNPC(%1))
#define isPlayerAClientNAFK(%1) (%1 >= 0 && %1 < MAX_PLAYERS && playerInfo[%1][clientID] != INVALID_CLIENT_ID)
#define isValidVehicle(%1) (%1 > 0 && %1 < MAX_VEHICLES && vDr[%1] != INVALID_PLAYER_ID && isPlayerAClient(vDr[%1]))
#define function<%1,%2> forward %1:sG_%2; public %1:sG_%2
// REMOVED: function<SetMaxClients(max)> return socket_set_max_connections(S,MAX_CONNECTIONS_ = max), 1;
function<_,AllowClientsOnly(bool:a)> return clientsOnly = a, 1;
function<_,IsPlayerUsingSGuard(playerid)> return _:isPlayerAClientNAFK(playerid);
function<_,IsPlayerRunningProcess(playerid,const procesname[])> if(isPlayerAClientNAFK(playerid)) SendToClient(playerInfo[playerid][clientID],f("proc:%s",procesname));
function<_,GetProcesses(playerid,const seperator[])> if(isPlayerAClientNAFK(playerid)) SendToClient(playerInfo[playerid][clientID],f("proclist:%s",seperator));
function<_,FindCheats(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"cheats");
function<_,FindCleoMods(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"cleo");
function<_,GetPlayerClientID(playerid)> return playerid >= 0 && playerid < MAX_PLAYERS ? playerInfo[playerid][clientID] : INVALID_CLIENT_ID;
function<_,GetClientPlayerID(clientid)> return clientid >= 0 && clientid < MAX_CONNECTIONS_ ? clientInfo[clientid][gameID] : INVALID_PLAYER_ID;
function<_,OpenWebpage(playerid,url[])> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("webpage:%s",url));
function<_,SavePlayerName(playerid,name[])> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("rename:%s",name));
function<_,TXD_Download(playerid,txdurl[])> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("dltxd:%s",txdurl));
function<_,QuitFromGame(playerid)> if(isPlayerAClientNAFK(playerid)) SendToClient(playerInfo[playerid][clientID],"quit");
function<_,IsPlayerUsingJoypad(playerid)> return isPlayerAClientNAFK(playerid) ? (_:(!playerInfo[playerid][infoArray_Int][5])) : -1;
function<_,ForceRemoveJoypad(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"removejoypad");
function<Float,GetPlayerGravity(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Float][0] : -1.0;
function<_,SetPlayerGravity(playerid,Float:gravity)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("gravity:%f",gravity));
function<_,GetPlayerRadioStation(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][0] : -1;
function<_,GetPlayerControlState(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][6] : -1;
function<_,GetPlayerJumpState(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][7] : -1;
function<_,GetPlayerRunState(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][8] : -1;
function<_,GetPlayerFireproof(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][1] : -1;
function<_,SetPlayerFireproof(playerid,bool:enabled)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("fireproof:%d",_:enabled));
function<_,GetPlayerLock(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][9] : -1;
function<_,SetPlayerLock(playerid,bool:locked)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("lock:%d",_:locked));
function<_,GetPlayerGameMenu(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][10] : -1;
function<_,SetPlayerGameMenu(playerid,menuid)> if(isPlayerAClient(playerid) && menuid >= 0 && menuid <= 43) SendToClient(playerInfo[playerid][clientID],f("menu:%d",menuid));
function<_,EnableGameCheat(playerid,cheatid,bool:enable)> if(isPlayerAClient(playerid) && cheatid >= 1 && cheatid <= 4) SendToClient(playerInfo[playerid][clientID],f("cheat:%d:%d",cheatid,_:enable));
function<_,IsGameCheatEnabled(playerid,cheatid)> return isPlayerAClientNAFK(playerid) && cheatid >= 1 && cheatid <= 4 ? playerInfo[playerid][infoArray_Int][GetCheatArrayPos(cheatid)] : -1;
function<_,GetPlayerTownID(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][11] : -1;
function<Float,GetPlayerMaxHealth(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Float][1] : -1.0;
function<_,SetPlayerMaxHealth(playerid,Float:maxhealth)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("maxhealth:%f",maxhealth));
function<_,GetPlayerWaveLevel(playerid)> return isPlayerAClientNAFK(playerid) ? floatround(playerInfo[playerid][infoArray_Float][2]) : -1;
function<_,SetPlayerWaveLevel(playerid,wavelevel)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("waves:%f",float(wavelevel)));
function<_,SetPlayerCameraDistance(playerid,Float:distance)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("camera:%f",distance));
function<_,ResetPlayerCameraDistance(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"camerareset");
function<_,SetPlayerRain(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"rain");
function<_,SAMP_GuardVersion(version[],len)> return strpack(VERSION,version,len);
function<_,GetPlayerInfiniteRun(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][12] : -1;
function<_,SetPlayerInfiniteRun(playerid,enabled)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("infiniterun:%d",_:bool:enabled));
function<_,CloseSGuardConnection(playerid)> if(isPlayerAClient(playerid)) prvCloseSGuardConnection(playerid);
function<_,GetPlayerVolume(playerid,volumetype)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][!volumetype ? 13 : 14] : -1;
function<_,GetSAMPInfo(playerid,keyname[])> return isPlayerAClientNAFK(playerid) && GetKeyNumber(keyname) != -1 ? playerInfo[playerid][infoArray_SAMP][GetKeyNumber(keyname)] : -1;
// REMOVED: function<_,SetSAMPInfo(playerid,keyname[],value)> if(isPlayerAClient(playerid) && GetKeyNumber(keyname) != -1) SendToClient(playerInfo[playerid][clientID],f("updatecfg:%s:%d",keyname,value));
function<_,IsPlayerAFK(playerid)> return isPlayerAClientNAFK(playerid) ? (_:(playerInfo[playerid][infoArray_Int][15] > 0)) : 0;
function<_,GetVehicleSirens(vehicleid)> return isValidVehicle(vehicleid) ? (_:(playerInfo[vDr[vehicleid]][infoArray_Int][16] == 208)) : 0;
function<_,SetVehicleSirens(vehicleid,activated)> if(isValidVehicle(vehicleid)) SendToClient(playerInfo[vDr[vehicleid]][clientID],f("vsirens:%d",bool:activated ? 208 : 80));
function<_,GetVehicleHorn(vehicleid)> return isValidVehicle(vehicleid) ? playerInfo[vDr[vehicleid]][infoArray_Int][17] : 0;
function<_,GetVehicleMass(vehicleid)> return isValidVehicle(vehicleid) ? floatround(playerInfo[vDr[vehicleid]][infoArray_Float][3]) : 0;
function<_,GetVehicleNitro(vehicleid)> return isValidVehicle(vehicleid) ? floatround(playerInfo[vDr[vehicleid]][infoArray_Float][4]) : 0;
function<_,SetVehicleNitro(vehicleid,nitro)> if(isValidVehicle(vehicleid)) SendToClient(playerInfo[vDr[vehicleid]][clientID],f("vnitro:%d.0",nitro));
function<_,GetVehicleExpTimer(vehicleid)> return isValidVehicle(vehicleid) ? floatround(playerInfo[vDr[vehicleid]][infoArray_Float][5]) : 0;
function<_,SetVehicleExpTimer(vehicleid,exptime)> if(isValidVehicle(vehicleid)) SendToClient(playerInfo[vDr[vehicleid]][clientID],f("explodetimer:%d.0",exptime));
function<Float,GetTrainSpeed(vehicleid)> return isValidVehicle(vehicleid) ? playerInfo[vDr[vehicleid]][infoArray_Float][6] : 0.0;
function<_,SetTrainSpeed(vehicleid,Float:trainspeed)> if(isValidVehicle(vehicleid)) SendToClient(playerInfo[vDr[vehicleid]][clientID],f("trainspeed:%f",fclamp(trainspeed,-1.0,1.0)));
function<_,GetPlayerMoonSize(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][18] : -1;
function<_,SetPlayerMoonSize(playerid,moonsize)> if(isPlayerAClient(playerid) && moonsize >= 0) SendToClient(playerInfo[playerid][clientID],f("moonsize:%d",moonsize));
function<_,ResetPlayerMoonSize(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"moonsize:3");
function<_,LockPlayerMoonSize(playerid,bool:lock)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("moonsizelock:%d",_:lock));
function<_,GetPlayerVision(playerid,vistype)> return isPlayerAClientNAFK(playerid) && (vistype == 0 || vistype == 1) ? playerInfo[playerid][infoArray_Int][20+vistype] : -1;
function<_,SetPlayerVision(playerid,vistype,activated)> if(isPlayerAClient(playerid) && (vistype == 0 || vistype == 1)) SendToClient(playerInfo[playerid][clientID],f("vision:%d:%d",vistype,_:bool:activated));
function<_,GetPlayerHUD(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][22] : -1;
function<_,SetPlayerHUD(playerid,bool:showing)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("hud:%d",_:showing));
function<_,GetPlayerHWID(playerid)> if(isPlayerAClient(playerid)) CallRemoteFunction("OnProgramResponse","iis",playerInfo[playerid][clientID],REQUEST_HWID,strspace(playerInfo[playerid][hwid]));
function<_,TXD_Exist(playerid,txdname[])> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("txdexist:%s",txdname));
function<_,OpenTeamSpeak(playerid,ip[],port)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("teamspeak:%s:%d",ip,port));
function<_,SetClipboardText(playerid,text[])> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("clipboard:%s",text));
function<_,GetPlayerBlur(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][23] : -1;
function<_,SetPlayerBlur(playerid,level)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("blur:%d",level));
function<_,GetPlayerWidescreen(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][29] : -1;
function<_,SetPlayerWidescreen(playerid,bool:toggle)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("widescreen:%d",_:toggle));
function<_,GetPlayerText(playerid)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],"gettext");
function<_,GetPlayerStamina(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][24] : -1;
function<_,SetPlayerStamina(playerid,stamina)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("stamina:%d",stamina));
function<Float,GetPlayerRotationSpeed(playerid)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Float][7] : -1.0;
function<_,SetPlayerRotationSpeed(playerid,Float:speed)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("rotspeed:%f",speed));
function<_,GetPlayerClipAmmo(playerid,clipslot)> return isPlayerAClientNAFK(playerid) ? playerInfo[playerid][infoArray_Int][25 + clamp(clipslot,1,4) - 1] : -1;
function<_,SetPlayerClipAmmo(playerid,clipslot,ammo)> if(isPlayerAClient(playerid)) SendToClient(playerInfo[playerid][clientID],f("clipammo:%d:%d",clamp(clipslot,1,4),ammo));
// EOF
