function $() { 
  var elements = new Array(); 
  for (var i = 0; i < arguments.length; i++) { 
  var element = arguments[i]; 
  if (typeof element == 'string') 
    element = document.getElementById(element); 
  if (arguments.length == 1)  
    return element;    
  elements.push(element); 
  }    
  return elements; 
} 
function CSSencode(code) 
{ 
 code = code.replace(/\r\n/ig,''); 
 code = code.replace(/(\s){2,}/ig,'$1'); 
 code = code.replace(/\t/ig,''); 
 code = code.replace(/\n\}/ig,'\}'); 
 code = code.replace(/\n\{\s*/ig,'\{'); 
 code = code.replace(/(\S)\s*\}/ig,'$1\}'); 
 code = code.replace(/(\S)\s*\{/ig,'$1\{'); 
 code = code.replace(/\{\s*(\S)/ig,'\{$1'); 
 return code; 
} 
function CSSdecode(code) 
{ 
 code = code.replace(/(\s){2,}/ig,'$1'); 
 code = code.replace(/(\S)\s*\{/ig,'$1 {'); 
 code = code.replace(/\*\/(.[^\}\{]*)}/ig,'\*\/\n$1}'); 
 code = code.replace(/\/\*/ig,'\n\/\*'); 
 code = code.replace(/;\s*(\S)/ig,';\n\t$1'); 
 code = code.replace(/\}\s*(\S)/ig,'\}\n$1'); 
 code = code.replace(/\n\s*\}/ig,'\n\}'); 
 code = code.replace(/\{\s*(\S)/ig,'\{\n\t$1'); 
 code = code.replace(/(\S)\s*\*\//ig,'$1\*\/'); 
 code = code.replace(/\*\/\s*([^\}\{]\S)/ig,'\*\/\n\t$1'); 
 code = code.replace(/(\S)\}/ig,'$1\n\}'); 
 code = code.replace(/(\n){2,}/ig,'\n'); 
 code = code.replace(/:/ig,':'); 
 code = code.replace(/  /ig,' '); 
 return code; 
} 