imports "mzkit.mrm" from "mzkit.quantify.dll";

let cal as string = "D:\20191230_WYL_MRM\XBL\raw\20191218cal";
let files = cal :> wiff.rawfiles("[-]?LM\d+") :> as.object;

print("Raw file contains linear groups:");
print(files$numberOfStandardReference);

print(files$GetLinearGroups());