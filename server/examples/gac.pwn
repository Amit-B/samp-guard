// AntiCheat based on SAMP-IL Guard by Amit_B
// 01/05/2013
#include "a_samp.inc"
#include "guard.inc"
new string[128];
public OnFilterScriptInit()
{
	AllowClientsOnly(true); // ���� ���� �� ������� �� ������ ������
	SetTimer("sGuardAntiCheat",15000,1); // �� 15 ����� ����� ��� ������� ���� ������ �'���� ��� ��� ��� �����
	print("AntiCheat based on SAMP-IL guard by Amit_B loaded");
	return 1;
}
public OnPlayerConnect(playerid)
{
	FindCleoMods(playerid); // ����� ���� ������ ����-���� ��� �����
	FindCheats(playerid); // ��� ���� ������ �'����
	return 1;
}
forward sGuardAntiCheat();
public sGuardAntiCheat() // ����� ������ ���� ���� ��� ���� ��� ����� �'����
{
	for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i)) FindCheats(i);
	/*	������ �� 500 ������ �� ����. �� �� ����� ��� 500 ��� ������ ����
		�����, �� ���� ����� �� ���� 500 ������. ������ ����� ����� �� ���
		����� ���� ���� ����� �����'�� ����� �� ������. ���� ������ �� ����
		���� ��� ����, ����� �� ����� ������ ����� ���� ������� ������ ���� */
}
public OnProgramResponse(clientid, response, data[]) // ������ ������ ������ ������ ����-��� \ �'����
{
	new playerid = GetClientPlayerID(clientid);
	if(!IsPlayerConnected(playerid)) return 1; // ������� ������ �����
	if(response == REQUEST_CLEO && equal(data,"True")) // �� ������ ������� ��� ���� ����-���� ������� ��� ���, ����� �� ����� �����
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[sGuard] {ffffff}���� ����� �� ����-��� ����� ����� %s",string);
		SendClientMessageToAll(0x00ff00ff,string);
		ShowPlayerDialog(playerid,666,DIALOG_STYLE_MSGBOX,"{ff0000}Cleo Mod","{ffffff}.��� �� ���� ���� ����� �����-���. ��� �� ���� ��� ���� �����","OK","");
		Kick(playerid);
	}
	if(response == REQUEST_CHEATS && equal(data,"True")) // �� ������ ������� ��� ���� �'���� ������� ��� ���, ����� �� ����� �����
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[sGuard] {ffffff}���� ����� ���� �� �'���� ����� %s",string);
		SendClientMessageToAll(0x00ff00ff,string);
		ShowPlayerDialog(playerid,666,DIALOG_STYLE_MSGBOX,"{ff0000}Cheats","{ffffff}.���� ���� ���� ��� ��'����, ����� �� ������ ������ ���� ����� �����","OK","");
		return BanEx(playerid,"SAMP-IL Guard: Cheating"), 0;
	}
	return 1;
}
stock equal(const str1[], const str2[]) return !strcmp(str1,str2,true) && strlen(str1) == strlen(str2);
stock strtok(const string_[], &index, somechar = ' ')
{
	new length = strlen(string_), result[64];
	while((index < length) && (string_[index] <= somechar)) index++;
	new offset = index;
	while((index < length) && (string_[index] > somechar) && ((index - offset) < (sizeof(result) - 1))) result[index - offset] = string_[index], index++;
	result[index - offset] = EOS;
	return result;
}
