<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	
	$vars['count'] = 1;
	
	$vars['header1'] = "SAMP-IL Guard";
	$vars['topkey1'] = false;
	$links = array
		(
			array("לרשימת העדכונים המלאה","index.php?p=news"),
			array("להורדה","index.php?p=download"),
			array("פורום רשמי","http://sa-mp.co.il/forumdisplay.php?f=346"),
			array("מדריך למערכת SAMP-IL Guard","http://sa-mp.co.il/showthread.php?t=51231"),
			array("פתרון בעיות טכניות","http://sa-mp.co.il/showthread.php?t=53461"),
			array("הסכם שימוש (למשתמש)","index.php?p=view&f=client-agreement"),
			array("הסכם שימוש (לשרת)","index.php?p=view&f=server-agreement"),
			array("Changelog: הסטוריית שינויים","http://sa-mp.co.il/showthread.php?t=46030")
		);
	/*"<br/>» <a href=\"index.php?p=news\">לרשימת העדכונים המלאה</a>" .
	"<br/>» <a href=\"index.php?p=download\">להורדה</a>" .
	"<br/>» <a href=\"http://sa-mp.co.il/forumdisplay.php?f=346\">פורום רשמי</a>" .
	"<br/>» <a href=\"http://sa-mp.co.il/showthread.php?t=51231\">מדריך למערכת SAMP-IL Guard</a>" .
	"<br/>» <a href=\"http://sa-mp.co.il/showthread.php?t=53461\">פתרון בעיות טכניות</a>" .
	"<br/>» <a href=\"index.php?p=view&f=client-agreement\">הסכם שימוש (למשתמש)</a>" .
	"<br/>» <a href=\"index.php?p=view&f=server-agreement\">הסכם שימוש (לשרת)</a>" .*/
	$vars['body1'] = "
	<div class=\"image_wrapper image_fl\"><img src=\"images/1.png\" alt=\"Image\" /></div>
	<p><em>השינוי הבא בסאמפ הישראלי מגיע...</em></p>
	<p>SAMP-IL Guard היא המערכת החדשה והראשונה בארץ שנועדה למשחק <a href=\"http://sa-mp.com\">GTA: San Andreas Multiplayer</a> ומיועדת לחסום צ'יטים, האקים ובאופן כללי - משחק לא פייר.</p>
	<div class=\"cleaner h30\"></div>
	<div class=\"col_w320 float_l\">
		<h3>חסימות</h3>
		<ul class=\"templatemo_list\">
			<li>Aimbot</li>
			<li>Cleo מוד</li>
			<li>Joypad</li>
			<li>צ'יטים והאקים מסוגים שונים ועוד...</li>
		</ul>
	</div>
	<div class=\"col_w320 float_r\">
	<h3>מידע שימושי</h3>
	<p>הגרסה הנוכחית היא <b>" . $VER . "</b>";
	for($i = 0; $i < count($links); $i++)
		$vars['body1'] .= "<br/>» <a href=\"".$links[$i][1]."\">".$links[$i][0]."</a>";
	$vars['body1'] .= "</p></div>
	<div class=\"cleaner h30\"></div>";
?>