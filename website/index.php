<?php
	/* SAMP-IL Guard Site
	(c) Copyright 2012  Amit `Amit_B` Barami */

	error_reporting(E_ALL);
	$p = isset($_GET['p']) ? $_GET['p'] : "main";
	function startsWith($haystack, $needle) { return !strncmp($haystack, $needle, strlen($needle)); }
	$VER = file_get_contents("version.txt");
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="he" dir="rtl">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>SAMP-IL Guard</title>
<link href="css/templatemo_style.css" rel="stylesheet" type="text/css" />
<link rel="icon" href="favicon.ico" type="image/x-icon"/>
<link rel="shortcut icon" href="favicon.ico" type="image/x-icon"/>

<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/jquery.lightbox-0.5.js"></script>
<link rel="stylesheet" type="text/css" href="css/jquery.lightbox-0.5.css" media="screen" />

<script type="text/javascript">
$(function() {
	$('#map a').lightBox();
});
</script>

</head>
<body>

<span id="top"></span>
<div id="templatemo_wrapper">
	<div id="templatemo_header">
		<div id="site_title">
			<h1><a href="http://guard.sa-mp.co.il/">SAMP-IL Guard</a></h1>
		</div>
		<div id="templatemo_menu">
			<ul>
				<?php
					$buttons = array(
						array("ראשי","main"),
						array("מידע","info"),
						array("חדשות","news"),
						array("הורדות","download"),
						array("למתכנתים","dev"),
						array("","function"),
						array("","event"),
						array("","page"),
						array("","faq"),
						array("","view")
					);
					for($i = count($buttons)-1; $i >= 0; $i--)
						if(strlen($buttons[$i][0]) > 0)
							echo "<li><a href=\"" . (startsWith($buttons[$i][1],"http") ? $buttons[$i][1] : ("index.php?p=" . $buttons[$i][1])) . "\">" . $buttons[$i][0] . "</a></li>";
				?>
			</ul>
		</div>
	</div>
	
	 <div id="templatemo_main">
	 
	<?php
		$avilable = false;
		for($i = 0, $m = count($buttons); $i < $m && !$avilable; $i++)
			if(!startsWith($buttons[$i][1],"http") && $buttons[$i][1] == $p)
				$avilable = true;
		if(!$avilable)
			$p = "main";
		include($p . "_vars.php");
		for($i = 1; $i <= $vars['count']; $i++)
			echo "
			<div class=\"content_box_top\"></div>
			<div class=\"content_box\">
				<h2>" . $vars['header' . $i] . " </h2>"
				. $vars['body' . $i]
				. ($vars['topkey' . $i] ? "<a class=\"gototop\" href=\"#top\">למעלה</a>" : "") . "
				<div class=\"cleaner\"></div>
			</div>
			<div class=\"content_box_bottom\"></div>";
	?>
		
	</div>
	
	<div id="templatemo_footer"><font color="darkblue">
		Copyright © 2012-2014 <a href="http://sa-mp.co.il">SAMP-IL</a> • Scripted by <a href="http://sa-mp.co.il/member.php?u=2">Amit_B</a> • Design from <a href="http://www.templatemo.com" target="_parent">Free CSS Templates</a>
	</font></div>

</div>

</body>
</html>