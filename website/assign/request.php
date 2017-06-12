<?php
	if(!isset($_GET['bind'])) exit;
	$generated = (1000 + rand(1,999));
	$file = fopen($_SERVER['REMOTE_ADDR'] .'_'. $_GET['bind'], 'w');
	fwrite($file, $generated);
	fclose($file);
	print($generated);
?>