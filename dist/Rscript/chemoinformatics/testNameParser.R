imports "massbank" from "mzkit";

print(glycosyl.tokens("triacetoyl-succinyl-rhamnoside"));

cat("\n\n");
print(glycosyl.tokens("3-sophoroside-5-glucoside"));

cat("\n\n");

print(glycosyl.tokens("3-(6''-ferulyl-2'''-sinapylsambubioside)-5-(6-malonylglucoside)"));

cat("\n\n");

print(glycosyl.tokens(" 3-O-[2''-O-(xylosyl) 6''-O-(p-O-(glucosyl) p-coumaroyl) glucoside] 5-O-[6'''-O-(malonyl) glucoside]"));

cat("\n\n");

print(glycosyl.tokens("[6-O-(malonyl)-beta-D-glucopyranoside]-7-O-[6-O-(trans-caffeyl)-beta-D-glucopyranoside]-3'-O-[6-O-(trans-4-O-(6-O-(trans-4-O-(beta-D-glucopyranosyl)-caffeyl)-beta-D-glucopyranosyl)-caffeyl)-beta-D-glucopyranoside]"));

cat("\n\n");

print(glycosyl.tokens("3-O-[6-O-[4-O-[4-O-(6-O-caffeoyl-beta-D-glucopyranosyl)caffeoyl]-alpha-L-rhamnosyl]-beta-D-glucopyranoside]-5-O-beta-D-glucopyranoside"));

pause();