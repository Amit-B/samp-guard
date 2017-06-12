// AntiCheat based on SAMP-IL Guard by Amit_B
// 01/05/2013
#include "a_samp.inc"
#include "guard.inc"
new string[128];
public OnFilterScriptInit()
{
	AllowClientsOnly(true); // השרת יאשר רק התחברות של משתמשי התוכנה
	SetTimer("sGuardAntiCheat",15000,1); // כל 15 שניות נבדוק האם השחקנים בשרת הפעילו צ'יטים תוך כדי שהם במשחק
	print("AntiCheat based on SAMP-IL guard by Amit_B loaded");
	return 1;
}
public OnPlayerConnect(playerid)
{
	FindCleoMods(playerid); // נשלחת בקשה לחיפוש קלאו-מודס אצל השחקן
	FindCheats(playerid); // וגם בקשה לחיפוש צ'יטים
	return 1;
}
forward sGuardAntiCheat();
public sGuardAntiCheat() // במקרה שהשחקן נכנס לשרת ורק לאחר מכן התקין צ'יטים
{
	for(new i = 0; i < MAX_PLAYERS; i++) if(IsPlayerConnected(i)) FindCheats(i);
	/*	לולאות של 500 שחקנים הן טעות. לא רק שכמות כמו 500 היא מטורפת עבור
		לולאה, גם בטוח שבשרת לא יהיו 500 שחקנים. הלולאה שאפשר לראות פה היא
		דוגמה בלבד לאיך ליצור אנטיצ'יט מבוסס על התוכנה. רצוי להעביר את הקוד
		לתוך מוד מוכן, שעובד עם מגבלת שחקנים נמוכה יותר ולולאות יעילות יותר */
}
public OnProgramResponse(clientid, response, data[]) // קיבלנו תשובות לבקשות החיפוש קלאו-מוד \ צ'יטים
{
	new playerid = GetClientPlayerID(clientid);
	if(!IsPlayerConnected(playerid)) return 1; // מוודאים שהשחקן מחובר
	if(response == REQUEST_CLEO && equal(data,"True")) // אם התשובה שהתקבלה היא לגבי קלאו-מודס והתוצאה היא אמת, נוציא את השחקן מהשרת
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[sGuard] {ffffff}ניסה להכנס עם קלאו-מוד והוצא מהשרת %s",string);
		SendClientMessageToAll(0x00ff00ff,string);
		ShowPlayerDialog(playerid,666,DIALOG_STYLE_MSGBOX,"{ff0000}Cleo Mod","{ffffff}.שרת זה אינו מאשר שימוש בקלאו-מוד. הסר את המוד ואז תוכל לחזור","OK","");
		Kick(playerid);
	}
	if(response == REQUEST_CHEATS && equal(data,"True")) // אם התשובה שהתקבלה היא לגבי צ'יטים והתוצאה היא אמת, נחסום את השחקן מהשרת
	{
		GetPlayerName(playerid,string,MAX_PLAYER_NAME);
		format(string,sizeof(string),"[sGuard] {ffffff}ניסה להכנס לשרת עם צ'יטים ונחסם %s",string);
		SendClientMessageToAll(0x00ff00ff,string);
		ShowPlayerDialog(playerid,666,DIALOG_STYLE_MSGBOX,"{ff0000}Cheats","{ffffff}.השרת זיהה אצלך קשר לצ'יטים, האקים או תוכנות אסורות ולכן נחסמת מהשרת","OK","");
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
