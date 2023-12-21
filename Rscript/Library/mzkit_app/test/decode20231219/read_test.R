require(mzkit);

imports "mzDeco" from "mz_quantify";

let xic_data = readBin(`${@dir}/demo.dat`, what = "mz_group");

print(xic_data);