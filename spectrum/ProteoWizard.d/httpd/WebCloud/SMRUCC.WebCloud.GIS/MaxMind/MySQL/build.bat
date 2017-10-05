reflector --reflects /sql ./geolite.sql /namespace MaxMind.geolite /split --language visualbasic 
reflector --reflects /sql ./geolite2.sql /namespace MaxMind.geolite2 /split --language visualbasic 

Reflector /MySQL.Markdown /sql ./geolite.sql
Reflector /MySQL.Markdown /sql ./geolite2.sql