setwd(@dir);

let rawdata = read.csv("./data_TIC.csv", row.names = 1);
let rt = rawdata$rt;

print(rt);
print(diff(rt), max.print = 1000);

