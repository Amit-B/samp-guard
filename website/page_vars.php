<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	$vars['count'] = 1;
	
	$vars['topkey1'] = false;
	$pg = $_GET['i'];
	$flag = false;
	if($handle = opendir("pages"))
	{
		while(false !== ($entry = readdir($handle)) && !$flag)
		{
			if(strlen($entry) < 3) continue;
			$entry = str_replace(".txt","",$entry);
			if($entry == $pg) $flag = true;
		}
		closedir($handle);
	}
	if(!$flag)
	{
		$vars['header1'] = "דף לא נמצא";
		$vars['body1'] = "<font color=\"red\">אז אתה אוהב לשחק עם הכתובת של האתר?<br/>יופי, גם אני!</font><br/>";
	}
	else
	{
		$lines = file("pages/$pg.txt");
		$vars['header1'] = $lines[0];
		for($i = 1; $i < count($lines); $i++) $vars['body1'] .= $lines[$i];
	}
	$vars['body1'] .= "<br/><br/><a href=\"index.php?p=dev\">» לחזרה לעמוד למתכנתים...</a><div class=\"cleaner h30\"></div>";
?>