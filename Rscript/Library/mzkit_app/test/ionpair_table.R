require(mzkit);

imports "assembly" from "mzkit";
imports "MRMLinear" from "mz_quantify";

let ions = read.msl("E:\biodeep\lab_automation\backend\data\targets-bileacid_cal.MSL");
let ionpairs = as.ion_pairs(ions);

ionpairs = as.data.frame(ionpairs);

print(ionpairs);

write.csv(ionpairs, file = "E:\biodeep\lab_automation\backend\data\targets-bileacid_cal.csv");