<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */
	
	$vars['count'] = 3;
	$TD = 3;

	$vars['header1'] = "פונקציות - Functions";
	$vars['topkey1'] = false;
	$order = $_GET['o'];
	if(!isset($_GET['o']) or ($order < 1 and $order > 2)) $order = 1;
	if($handle = opendir("functions"))
	{
		$vars['body1'] .= "<center>" . ($order == 1 ? "<b>" : "<a href=\"index.php?p=dev&o=1\">") . "סדר לפי ABC</" . ($order == 1 ? "b" : "a") . "> • " . ($order == 2 ? "<b>" : "<a href=\"index.php?p=dev&o=2\">") . "סדר לפי קטגוריות</" . ($order == 2 ? "b" : "a") . "></center><br/>";
		$c = 0;
		$fid = 0;
		$m = 0;
		while(false !== ($entry = readdir($handle))) $functions[$fid++] = $entry;
		if($order == 1)
		{
			sort($functions);
			for($abc = 0; $abc < 26; $abc++)
			{
				$c = 0;
				$newfunclist[$abc][0] = 0;
				for($i = 0; $i < $fid; $i++)
				{
					if(strlen($functions[$i]) < 3) continue;
					$functions[$i] = str_replace(".txt","",$functions[$i]);
					if($functions[$i][0] == chr(65+$abc)) $newfunclist[$abc][$c++] = $functions[$i];
				}
			}
			$m = 26;
		}
		else if($order == 2)
		{
			$categories = array();
			$counts = array();
			for($i = 0, $m = count($functions); $i < $m; $i++)
			{
				if(strlen($functions[$i]) < 3) continue;
				$lines = file("functions/" . $functions[$i],FILE_IGNORE_NEW_LINES);
				foreach($lines as $line_num => $line)
				{
					if(strpos($line,"|") !== false)
					{
						$exp = explode("|",$line);
						if($exp[0] == "category") $cat = $exp[1];
					}
				}
				if(!in_array($cat,$categories))
				{
					array_push($categories,$cat);
					array_push($counts,0);
				}
				$idx = array_search($cat,$categories);
				$newfunclist[$idx][$counts[$idx]] = str_replace(".txt","",$functions[$i]);
				$counts[$idx]++;
			}
			$m = count($categories);
		}
		$vars['body1'] .= "<div dir=\"ltr\">";
		$flag = false;
		for($i = 0; $i < $m; $i++)
		{
			if(strlen($newfunclist[$i][0]) < 3) continue;
			$c = 0;
			if($flag) $vars['body1'] .= "<br/>";
			else $flag = true;
			$vars['body1'] .= "<h4>" . ($order == 1 ? chr(65+$i) : $categories[$i]) . "</h4><table style=\"width:100%;table-layout:fixed;\" border=\"0\">";
			for($j = 0; $j < count($newfunclist[$i]); $j++)
			{
				if($c == 0) $vars['body1'] .= "<tr>";
				$vars['body1'] .= "<td><a href=\"index.php?p=function&f=" . $newfunclist[$i][$j] . "\">" . $newfunclist[$i][$j] . "</a></td>";
				$c++;
				if($c == $TD)
				{
					$c = 0;
					$vars['body1'] .= "</tr>";
				}
			}
			if($c != 0)
			{
				for($j = $c; $j < $TD; $j++) $vars['body1'] .= "<td> </td>";
				$vars['body1'] .= "</tr>";
			}
			$vars['body1'] .= "</table>";
		}
		$vars['body1'] .= "</div>";
		closedir($handle);
	}
	$vars['body1'] .= "<div class=\"cleaner h30\"></div>";
	
	$vars['header2'] = "מקרים - Events";
	$vars['topkey2'] = false;
	if($handle = opendir("events"))
	{
		$c = 0;
		$eid = 0;
		while(false !== ($entry = readdir($handle))) $events[$eid++] = $entry;
		sort($events);
		for($abc = 0; $abc < 26; $abc++)
		{
			$c = 0;
			$neweventlist[$abc][0] = 0;
			for($i = 0; $i < $eid; $i++)
			{
				if(strlen($events[$i]) < 3) continue;
				$events[$i] = str_replace(".txt","",$events[$i]);
				if($events[$i][0] == chr(65+$abc)) $neweventlist[$abc][$c++] = $events[$i];
			}
		}
		$vars['body2'] .= "<div dir=\"ltr\">";
		$flag = false;
		for($i = 0; $i < 26; $i++)
		{
			if(strlen($neweventlist[$i][0]) < 2) continue;
			$c = 0;
			if($flag) $vars['body2'] .= "<br/>";
			else $flag = true;
			$vars['body2'] .= "<h4>" . chr(65+$i) . "</h4><table style=\"width:100%;table-layout:fixed;\" border=\"0\">";
			for($j = 0; $j < count($neweventlist[$i]); $j++)
			{
				if($c == 0) $vars['body2'] .= "<tr>";
				$vars['body2'] .= "<td><a href=\"index.php?p=event&f=" . $neweventlist[$i][$j] . "\">" . $neweventlist[$i][$j] . "</a></td>";
				$c++;
				if($c == $TD)
				{
					$c = 0;
					$vars['body2'] .= "</tr>";
				}
			}
			if($c != 0)
			{
				for($j = $c; $j < $TD; $j++) $vars['body2'] .= "<td> </td>";
				$vars['body2'] .= "</tr>";
			}
			$vars['body2'] .= "</table>";
		}
		$vars['body2'] .= "</div>";
		closedir($handle);
	}
	$vars['body2'] .= "<div class=\"cleaner h30\"></div>";
	
	$vars['header3'] = "שימושי";
	$vars['topkey3'] = false;
	if($handle = opendir("pages"))
	{
		$c = 0;
		while(false !== ($entry = readdir($handle)))
		{
			if(strlen($entry) < 3) continue;
			$lines = file("pages/" . $entry,FILE_IGNORE_NEW_LINES);
			$vars['body3'] .= ($c == 0 ? "" : "<br/>") . "» <a href=\"index.php?p=page&i=" . str_replace(".txt","",$entry) . "\">" . $lines[0] . "</a>";
			$c++;
		}
		closedir($handle);
	}
	$vars['body3'] .= "<div class=\"cleaner h30\"></div>";
?>