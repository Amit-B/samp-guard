<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	
	$vars['count'] = 3;
	
	$vars['header1'] = "מידע על המערכת";
	$vars['topkey1'] = false;
	$vars['body1'] = "
	<p>מערכת זו עובדת באמצעות תוכנה שיושבת בצד המשתמש ומאפשרת לשרת לדעת בוודאות האם השחקנים משתמשים בצ'יטים.
	<br/>התוכנה קלה לשימוש ומאפשרת התקשרות פשוטה עם SA-MP, תוך כדי שהמידע המועבר בין המחשב של המשתמש למשחק,
	<br/>הוא האם המשתמש הפעיל צ'יטים, מודים אסורים או ג'ויפד.
	<br/>בין הייתר המערכת מאפשרת גם לבצע פעולות במשחק שלא אופשרו עד היום, כמו לשנות את גובה הגלים במים או לראות מכוניות בלתי נראות, ועוד.
	<br/><br/>המערכת שוחררה לראשונה בינואר 2013, ממשיכה ותמשיך להתעדכן ולקבל שיפורים חדשים עבור המשחק והאבטחה.
	<br/></p>
	<div class=\"cleaner h30\"></div>
	<h3>הערות חשובות</h3>
	<br/>- שרת שמעוניין לקבל הרשאה להשתמש בתוכנה צריך לפנות במייל לכתובת: <a href=\"mailto:amit@sa-mp.co.il\">Amit@SA-MP.co.il</a> או ליצור קשר ישירות עם <a href=\"http://sa-mp.co.il/member.php?u=2\">מפתח המערכת</a>
	<br/>- כל שרת שמריץ עליו את התוכנה מתחייב להשתמש בה בהגינות ובלי לפגוע בשחקן ובפרטיותו בשום צורה. השימוש במערכת תמיד ישאר מבוקר, ובמידה ונעשה שימוש לא נכון ע\"י אתר הוא יוסר מרשימת האישורים במיידי.
	<br/>- המערכת בלעדית לקהילות <a href=\"http://sa-mp.co.il\">SAMP-IL</a>
	<div class=\"cleaner h30\"></div>";
	
	$vars['header2'] = "שרתים בעלי אישור לשימוש במערכת";
	$vars['topkey2'] = false;
	$lines = file("servers.txt",FILE_IGNORE_NEW_LINES);
	$vars['body2'] = "<div dir=\"ltr\"><ul class=\"templatemo_list\">";
	foreach($lines as $line_num => $line)
	{
		$exp = explode(":",$line);
		if($exp[0] == "127.0.0.1") continue;
		$vars['body2'] .= "<li>(" . str_replace(" ","",$exp[3]) . ") " . $exp[2] . " - " . $exp[0] . ":" . $exp[1] . "<i>[" . $exp[4] . ", <u><a href=\"mailto:" . $exp[5] . "\">" . $exp[5] . "</a></u>]</i></li><br/>";
	}
	$vars['body2'] .= "</ul></div><div class=\"cleaner h30\"></div>";
	
	$vars['header3'] = "אודות";
	$vars['topkey3'] = false;
	$vars['body3'] = "
	<p><a href=\"http://sa-mp.co.il/showthread.php?t=46030\"><b>תכנות המערכת</b><br/>Amit_B</a><br/><br/><br/>
	<a href=\"http://forum.sa-mp.com/showthread.php?t=333934\"><b>תוסף Sockets</b><br/>BlueG</a><br/><br/><br/>
	<b>טסטים, רעיונות, תמיכה ועוד</b><br/>צוות SAMP-IL<br/><br/><br/>
	<em>הקרדיטים המורחבים יותר מופיעים בתוכנה עצמה בלחיצה על \"אודות\".</em>
	</p><div class=\"cleaner h30\"></div>";
?>