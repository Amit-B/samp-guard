<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	
	$news =
		array(
			array("scnd","12/10/2014","גרסה 2.0"),
			array("sgrd","19/04/2014","עדכונים לקראת שיפור מרכזי"),
			array("udmg","01/02/2013","העלאת המערכת ל-United DM"),
			array("frst","05/01/2013","הפעלת המערכת לראשונה")
		);

	$vars['count'] = count($news);
	
	for($i = 0; $i < $vars['count']; $i++)
	{
		$vars['header' . ($i+1)] = $news[$i][2];
		$vars['topkey' . ($i+1)] = $i > 0;
		$vars['body' . ($i+1)] = "
		<p><em>עדכון #" . ($vars['count']-$i) . " • " . $news[$i][1] . "</em></p>
		<br/>" . str_replace("\n","<br/>",file_get_contents("news/" . $news[$i][0] . ".txt")) .
		"<br/>" . "<div class=\"cleaner h30\"></div>";
	}
?>