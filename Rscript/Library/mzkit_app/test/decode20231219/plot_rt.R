require(mzkit);

imports "mzDeco" from "mz_quantify";
imports "visual" from "mzplot";

let rt_data = load.csv("G:\\tmp\\QC_pos\\rt_shifts.csv", type = "rt_shift");

print(rt_data);