<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	$vars['count'] = 1;
	
	$vars['topkey1'] = false;
	$pg = $_GET['f'];
	if(!file_exists("doc/$pg.txt"))
	{
		$vars['header1'] = "דף לא נמצא";
		$vars['body1'] = "<font color=\"red\">אז אתה אוהב לשחק עם הכתובת של האתר?<br/>יופי, גם אני!</font><br/>";
	}
	else
	{
		$lines = file("doc/$pg.txt");
		$vars['header1'] = $lines[0];
		$vars['body1'] .= "<fieldset>";
		for($i = 1; $i < count($lines); $i++) $vars['body1'] .= ($i == 1 ? "" : "<br/>") . $lines[$i];
		$vars['body1'] .= "</fieldset>";
	}
	$vars['body1'] .= "<br/><br/><a href=\"index.php\">» לחזרה לעמוד הראשי...</a><div class=\"cleaner h30\"></div>";
?>