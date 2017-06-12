/*	SAMP-IL Guard v2.0
 *
 *	(c) Copyright 2012-2014, Amit_B, SA-MP.co.il
 *	Contact: amit@sa-mp.co.il
 *
 */
#if defined _guard_included
	#endinput
#endif
#define _guard_included
#include "a_samp.inc"
#include "core.inc"
#define function(%1,%2,<%3>,<%4>) stock %1(%3) return CallRemoteFunction("sG_" #%1,%2,%4)
#define tagged(%1,%2,%3,<%4>,<%5>) stock %1:%2(%4) return Float:CallRemoteFunction("sG_" #%2,%3,%4)
#define dvrdfunc(%1,%2,<%3>,<%4>,<%5>) stock %1(%3) return %5, CallRemoteFunction("sG_" #%1,%2,%4)
// Defines
#define INVALID_CLIENT_ID -1
#define DEFAULT_GRAVITY 0.008
#define DEFAULT_MAX_HEALTH 569.0
#define DEFAULT_WAVE_HEIGHT 0
#define DEFAULT_MOON_SIZE 3
#define MAX_CLIENTS 250
// Request Types
#define INVALID_REQUEST -001
#define REQUEST_PROC 000
#define REQUEST_CHEATS 001
#define REQUEST_CLEO 002
#define REQUEST_PROCLIST 003
#define REQUEST_HWID 004
#define REQUEST_TEXT 005
#define REQUEST_TXD 006
// Cheats
#define CHEAT_INVISIBLECARS 1
#define CHEAT_DRIVEONWATER 2
#define CHEAT_NORELOAD 3
#define CHEAT_STOPGAMECLOCK 4
// Volume Types
#define VOLUME_RADIO 0
#define VOLUME_SFX 1
// Vision Types
#define VISION_NIGHT 0
#define VISION_THERMAL 1
// Player
tagged(Float,GetPlayerGravity,"i",<playerid>,<playerid>);
function(SetPlayerGravity,"if",<playerid,Float:gravity>,<playerid,gravity>);
function(GetPlayerRadioStation,"i",<playerid>,<playerid>);
function(GetPlayerControlState,"i",<playerid>,<playerid>);
function(GetPlayerJumpState,"i",<playerid>,<playerid>);
function(GetPlayerRunState,"i",<playerid>,<playerid>);
function(GetPlayerFireproof,"i",<playerid>,<playerid>);
function(SetPlayerFireproof,"ii",<playerid,bool:enabled>,<playerid,enabled>);
function(GetPlayerLock,"i",<playerid>,<playerid>);
function(SetPlayerLock,"ii",<playerid,bool:locked>,<playerid,locked>);
function(GetPlayerGameMenu,"i",<playerid>,<playerid>);
function(SetPlayerGameMenu,"ii",<playerid,menuid>,<playerid,menuid>);
function(EnableGameCheat,"iii",<playerid,cheatid,bool:enable>,<playerid,cheatid,enable>);
function(IsGameCheatEnabled,"ii",<playerid,cheatid>,<playerid,cheatid>);
function(GetPlayerTownID,"i",<playerid>,<playerid>);
tagged(Float,GetPlayerMaxHealth,"i",<playerid>,<playerid>);
function(SetPlayerMaxHealth,"if",<playerid,Float:maxhealth>,<playerid,maxhealth>);
function(GetPlayerWaveLevel,"i",<playerid>,<playerid>);
function(SetPlayerWaveLevel,"ii",<playerid,wavelevel>,<playerid,wavelevel>);
function(SetPlayerCameraDistance,"if",<playerid,Float:distance>,<playerid,distance>);
function(ResetPlayerCameraDistance,"i",<playerid>,<playerid>);
function(SetPlayerRain,"i",<playerid>,<playerid>);
function(GetPlayerInfiniteRun,"i",<playerid>,<playerid>);
function(SetPlayerInfiniteRun,"ii",<playerid,enabled>,<playerid,enabled>);
function(IsPlayerAFK,"i",<playerid>,<playerid>);
function(GetPlayerMoonSize,"i",<playerid>,<playerid>);
function(SetPlayerMoonSize,"ii",<playerid,moonsize>,<playerid,moonsize>);
function(ResetPlayerMoonSize,"i",<playerid>,<playerid>);
function(LockPlayerMoonSize,"ii",<playerid,bool:lock>,<playerid,lock>);
function(GetPlayerVision,"ii",<playerid,vistype>,<playerid,vistype>);
function(SetPlayerVision,"iii",<playerid,vistype,activated>,<playerid,vistype,activated>);
function(GetPlayerHUD,"i",<playerid>,<playerid>);
function(SetPlayerHUD,"ii",<playerid,bool:showing>,<playerid,showing>);
function(GetPlayerBlur,"i",<playerid>,<playerid>);
function(SetPlayerBlur,"ii",<playerid,level>,<playerid,level>);
function(GetPlayerWidescreen,"i",<playerid>,<playerid>);
function(SetPlayerWidescreen,"ii",<playerid,bool:toggle>,<playerid,toggle>);
function(GetPlayerText,"i",<playerid>,<playerid>);
function(GetPlayerStamina,"i",<playerid>,<playerid>);
function(SetPlayerStamina,"ii",<playerid,stamina>,<playerid,stamina>);
tagged(Float,GetPlayerRotationSpeed,"i",<playerid>,<playerid>);
function(SetPlayerRotationSpeed,"if",<playerid,Float:speed>,<playerid,speed>);
function(GetPlayerClipAmmo,"ii",<playerid,clipslot>,<playerid,clipslot>);
function(SetPlayerClipAmmo,"iii",<playerid,clipslot,ammo>,<playerid,clipslot,ammo>);
// Connections
dvrdfunc(AllowClientsOnly,"i",<bool:a>,<a>,<setproperty(.name="sguard_co",.value=_:a)>);
function(GetPlayerClientID,"i",<playerid>,<playerid>);
function(GetClientPlayerID,"i",<clientid>,<clientid>);
function(GetPlayerHWID,"i",<playerid>,<playerid>);
function(CloseSGuardConnection,"i",<playerid>,<playerid>);
// Vehicle
function(GetVehicleSirens,"i",<vehicleid>,<vehicleid>);
function(SetVehicleSirens,"ii",<vehicleid,activated>,<vehicleid,activated>);
function(GetVehicleHorn,"i",<vehicleid>,<vehicleid>);
function(GetVehicleMass,"i",<vehicleid>,<vehicleid>);
function(GetVehicleNitro,"i",<vehicleid>,<vehicleid>);
function(SetVehicleNitro,"ii",<vehicleid,nitro>,<vehicleid,nitro>);
function(GetVehicleExpTimer,"i",<vehicleid>,<vehicleid>);
function(SetVehicleExpTimer,"ii",<vehicleid,exptime>,<vehicleid,exptime>);
tagged(Float,GetTrainSpeed,"i",<vehicleid>,<vehicleid>);
function(SetTrainSpeed,"if",<vehicleid,Float:trainspeed>,<vehicleid,trainspeed>);
// Settings
function(SavePlayerName,"is",<playerid,name[]>,<playerid,name>);
function(GetSAMPInfo,"is",<playerid,keyname[]>,<playerid,keyname>);
function(GetPlayerVolume,"ii",<playerid,volumetype>,<playerid,volumetype>);
// Security
function(FindCheats,"i",<playerid>,<playerid>);
function(FindCleoMods,"i",<playerid>,<playerid>);
function(IsPlayerUsingJoypad,"i",<playerid>,<playerid>);
function(ForceRemoveJoypad,"i",<playerid>,<playerid>);
function(IsPlayerRunningProcess,"is",<playerid,const procesname[]>,<playerid,procesname>);
function(GetProcesses,"is",<playerid,const seperator[]>,<playerid,seperator>);
// Management
function(OpenWebpage,"is",<playerid,url[]>,<playerid,url>);
function(QuitFromGame,"i",<playerid>,<playerid>);
function(OpenTeamSpeak,"isi",<playerid,ip[],port>,<playerid,ip,port>);
function(SetClipboardText,"is",<playerid,text[]>,<playerid,text>);
// sGuard
function(SAMP_GuardVersion,"si",<version[],len>,<version,len>);
function(IsPlayerUsingSGuard,"i",<playerid>,<playerid>);
// TXD
function(TXD_Download,"is",<playerid,txdurl[]>,<playerid,txdurl>);
function(TXD_Exist,"is",<playerid,txdname[]>,<playerid,txdname>);
/*	Full function list:
	native Float:GetPlayerGravity(playerid);
	native SetPlayerGravity(playerid,Float:gravity);
	native GetPlayerRadioStation(playerid);
	native GetPlayerControlState(playerid);
	native GetPlayerJumpState(playerid);
	native GetPlayerRunState(playerid);
	native GetPlayerFireproof(playerid);
	native SetPlayerFireproof(playerid,bool:enabled);
	native GetPlayerLock(playerid);
	native SetPlayerLock(playerid,bool:locked);
	native GetPlayerGameMenu(playerid);
	native SetPlayerGameMenu(playerid,menuid);
	native EnableGameCheat(playerid,cheatid,bool:enable);
	native IsGameCheatEnabled(playerid,cheatid);
	native GetPlayerTownID(playerid);
	native Float:GetPlayerMaxHealth(playerid);
	native SetPlayerMaxHealth(playerid,Float:maxhealth);
	native GetPlayerWaveLevel(playerid);
	native SetPlayerWaveLevel(playerid,wavelevel);
	native SetPlayerCameraDistance(playerid,Float:distance);
	native ResetPlayerCameraDistance(playerid);
	native SetPlayerRain(playerid);
	native GetPlayerInfiniteRun(playerid);
	native SetPlayerInfiniteRun(playerid,enabled);
	native IsPlayerAFK(playerid);
	native GetPlayerMoonSize(playerid);
	native SetPlayerMoonSize(playerid,moonsize);
	native ResetPlayerMoonSize(playerid);
	native LockPlayerMoonSize(playerid,bool:lock);
	native GetPlayerVision(playerid,vistype);
	native SetPlayerVision(playerid,vistype,activated);
	native GetPlayerHUD(playerid);
	native SetPlayerHUD(playerid,bool:showing);
	native GetPlayerBlur(playerid);
	native SetPlayerBlur(playerid,level);
	native GetPlayerWidescreen(playerid);
	native SetPlayerWidescreen(playerid,bool:toggle);
	native GetPlayerText(playerid);
	native GetPlayerStamina(playerid);
	native SetPlayerStamina(playerid,stamina);
	native Float:GetPlayerRotationSpeed(playerid);
	native SetPlayerRotationSpeed(playerid,Float:speed);
	native GetPlayerClipAmmo(playerid,clipslot);
	native SetPlayerClipAmmo(playerid,clipslot,ammo);
	// Connections
	native AllowClientsOnly(bool:a);
	native GetPlayerClientID(playerid);
	native GetClientPlayerID(clientid);
	native GetPlayerHWID(playerid);
	native CloseSGuardConnection(playerid);
	// Vehicle
	native GetVehicleSirens(vehicleid);
	native SetVehicleSirens(vehicleid,activated);
	native GetVehicleHorn(vehicleid);
	native GetVehicleMass(vehicleid);
	native GetVehicleNitro(vehicleid);
	native SetVehicleNitro(vehicleid,nitro);
	native GetVehicleExpTimer(vehicleid);
	native SetVehicleExpTimer(vehicleid,exptime);
	native Float:GetTrainSpeed(vehicleid);
	native SetTrainSpeed(vehicleid,Float:trainspeed);
	// Settings
	native SavePlayerName(playerid,name[]);
	native GetSAMPInfo(playerid,keyname[]);
	native GetPlayerVolume(playerid,volumetype);
	// Security
	native FindCheats(playerid);
	native FindCleoMods(playerid);
	native IsPlayerUsingJoypad(playerid);
	native ForceRemoveJoypad(playerid);
	native IsPlayerRunningProcess(playerid,const procesname[]);
	native GetProcesses(playerid,const seperator[]);
	// Management
	native OpenWebpage(playerid,url[]);
	native QuitFromGame(playerid);
	native OpenTeamSpeak(playerid,ip[],port);
	native SetClipboardText(playerid,text[]);
	// sGuard
	native SAMP_GuardVersion(version[],len);
	native IsPlayerUsingSGuard(playerid);
	// TXD
	native TXD_Download(playerid,txdurl[]);
	native TXD_Exist(playerid,txdname[]);
*/
// Callbacks
forward OnProgramResponse(clientid,response,data[]);
forward OnProcessStart(clientid,process[]);
forward OnProcessStop(clientid,process[]);
forward OnVehicleCollision(vehicleid,playerid,Float:collision);
// EOF
#undef function
#undef tagged
#undef dvrdfunc
