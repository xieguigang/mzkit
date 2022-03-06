require(mzkit);

setwd(@dir);

data = "E:\mzkit\DATA\test\HR2MSI mouse urinary bladder S096 - Figure1.cdf";
data = open.mzpack(data);

print(data);
