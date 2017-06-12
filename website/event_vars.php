<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	include("convert_to_pawn.php");
	$vars['count'] = 1;
	
	$vars['topkey1'] = false;
	$funcname = $_GET['f'];
	$flag = false;
	if($handle = opendir("events"))
	{
		while(false !== ($entry = readdir($handle)) && !$flag)
		{
			if(strlen($entry) < 3) continue;
			$entry = str_replace(".txt","",$entry);
			if($entry == $funcname) $flag = true;
		}
		closedir($handle);
	}
	if(!$flag)
	{
		$vars['header1'] = "מקרה לא נמצא";
		$vars['body1'] .= "<font color=\"red\">אז אתה אוהב לשחק עם הכתובת של האתר?<br/>יופי, גם אני!</font><br/>";
	}
	else
	{
		$vars['header1'] = $funcname;
		$lines = file("events/" . $funcname . ".txt",FILE_IGNORE_NEW_LINES);
		$example = "";
		foreach($lines as $line_num => $line)
		{
			if(strpos($line,"|") === false) $example .= strlen($example) == 0 ? $line : ("<br/>" . $line);
			else
			{
				$exp = explode("|",$line);
				$func[$exp[0]] = $exp[1];
			}
		}
		switch($func['status'])
		{
			case "0": $status = "<font color=\"green\">המקרה אושר כעובד.</font>"; break;
			case "1": $status = "<font color=\"orange\">המקרה לא נבדק ולא אושר מאף אחד אם הוא עובד או לא. הוא אמור לעבוד. כדי לאשר את הסטאטוס, שלח <a href=\"http://sa-mp.co.il/private.php?do=newpm&u=2\">הודעה פרטית</a> או <a href=\"mailto:amit@sa-mp.co.il\">מייל</a> ליוצר המערכת.</font>"; break;
			case "2": $status = "<font color=\"red\">המקרה נבדק ונכון לעכשיו הוא לא עובד.</font>"; break;
			case "3": $status = "<font color=\"darkred\">המקרה נמחק ולא ניתן לשימוש בגרסת המערכת הנוכחית.</font>"; break;
			default: $status = "<font color=\"red\">לא ידוע.</font>"; break;
		}
		$vars['body1'] .= "
		<div dir=\"ltr\" style=\"font-family:Courier New; font-size:14px; color:black;\"><fieldset><legend>Pawn Code</legend>" . convert_to_pawn($func['func']) . "</fieldset></div><br/><br/>" .
		$func['desc'] . "<br/><br/><b>סטאטוס: " . $status . "</b>";
		$vars['body1'] .= "<br/><br/><table style=\"width:100%;\" border=\"1\"><tr><th>גרסה</th><th>היסטוריה</th></tr>";
		$c = 1;
		while(isset($func['changelog' . $c]))
		{
			$changelog = explode(":",$func['changelog' . $c]);
			$vars['body1'] .= "<tr><td><div dir=\"ltr\" style=\"font-family:Courier New; font-size:14px;\" align=\"center\">" . $changelog[0] . "</div></td><td>" . $changelog[1] . "</td></tr>";
			$c++;
		}
		$vars['body1'] .= "</table>";
		$c = 1;
		while(isset($func['note' . $c]))
		{
			$vars['body1'] .= "</br>" . $func['note' . $c];
			$c++;
		}
		// xpoz bitch $params = explode(",",explode(")",explode("(",$func['func'])[1])[0]);
		$params = explode("(",$func['func']);
		$params = $params[1];
		$params = explode(")",$params);
		$params = $params[0];
		$params = explode(",",$params);
		$vars['body1'] .= "<br/><br/><table style=\"width:100%;\" border=\"1\"><tr><th>פרמטר</th><th>מידע</th></tr>";
		for($i = 0, $m = count($params); $i < $m; $i++) $vars['body1'] .= "<tr><td><div dir=\"ltr\" style=\"font-family:Courier New; font-size:14px;\">" . $params[$i] . "</div></td><td>" . $func['param' . ($i+1)] . "</td></tr>";
		$vars['body1'] .= "</table>";
		$vars['body1'] .= strlen($example) > 0 ? ("<br/><u>קוד לדוגמה:</u><br/><br/><div dir=\"ltr\" style=\"font-family:Courier New; font-size:14px; color:black;\"><fieldset><legend>Pawn Code</legend>" . convert_to_pawn($example) . "</fieldset></div>") : "<br/>קוד לדוגמה אינו זמין כעת.";
	}
	$vars['body1'] .= "<br/><a href=\"index.php?p=dev\">» לחזרה לרשימת המקרים...</a><div class=\"cleaner h30\"></div>";
?>