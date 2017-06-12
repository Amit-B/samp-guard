<?php
	/* SAMP-IL Guard Site
	Convert to Pawn function by XpOz */
	function convert_to_pawn($text)
    {
        $text = str_replace(
         array(
         'public',
         'return',
         'sizeof',
		 'forward',
		 'assert',
         'switch',
         'true',
         'false',
         'if',
         'new',
         'static',
         'const',
         'else',
         'for',
         'while',
         'case',
         'default',
         'goto',
         'do',
         'try',
         'continue',
         'break',
         'enum',
         'exit',
         'operator',
         /*'(',
         ')',
         '[',
         ']',
         '{',
         '}',*/
		 '0',
		 '1',
		 '2',
		 '3',
		 '4',
		 '5',
		 '6',
		 '7',
		 '8',
		 '9'
         ),
         array(
         '<span style="color: blue;">public</span>',	 
         '<span style="color: blue;">return</span>',
         '<span style="color: blue;">sizeof</span>',
         '<span style="color: blue;">forward</span>',
         '<span style="color: blue;">assert</span>',
         '<span style="color: blue;">switch</span>',
         '<span style="color: blue;">true</span>',
         '<span style="color: blue;">false</span>',
         '<span style="color: blue;">if</span>',
         '<span style="color: blue;">new</span>',
         '<span style="color: blue;">static</span>',
         '<span style="color: blue;">const</span>',
         '<span style="color: blue;">else</span>',
         '<span style="color: blue;">for</span>',
         '<span style="color: blue;">while</span>',
         '<span style="color: blue;">case</span>',
         '<span style="color: blue;">default</span>',
         '<span style="color: blue;">goto</span>',
         '<span style="color: blue;">do</span>',
         '<span style="color: blue;">try</span>',
         '<span style="color: blue;">continue</span>',
         '<span style="color: blue;">break</span>',
         '<span style="color: blue;">enum</span>',
         '<span style="color: blue;">exit</span>',
         '<span style="color: blue;">operator</span>',
         /*'<span style="color: red;">(</span>',
         '<span style="color: red;">)</span>',
         '<span style="color: red;">[</span>',
         '<span style="color: red;">]</span>',
         '<span style="color: red;">{</span>',
         '<span style="color: red;">}</span>',*/
         '<span style="color: darkblue;">0</span>',
         '<span style="color: darkblue;">1</span>',
         '<span style="color: darkblue;">2</span>',
         '<span style="color: darkblue;">3</span>',
         '<span style="color: darkblue;">4</span>',
         '<span style="color: darkblue;">5</span>',
         '<span style="color: darkblue;">6</span>',
         '<span style="color: darkblue;">7</span>',
         '<span style="color: darkblue;">8</span>',
         '<span style="color: darkblue;">9</span>'
         ), $text);
         
         $text = preg_replace(
         array(
         '~#(.+?)\<br \/>~is',
         '~\/\/(.+?)\<br \/\>~i',
         '~\/\*(.+?)\*\/~i'
         ),
         array(
          '<span style="color: blue;">#$1<br /></span>', 
         '<span style="color: green;">//$1<br /></span>',
         '<span style="color: green;">/*$1*/</span>'
         ), $text); 
        return $text;
    }
?>