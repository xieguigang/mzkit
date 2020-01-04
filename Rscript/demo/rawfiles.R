imports "mzkit.mrm" from "mzkit.quantify.dll";

let mzML  = "D:\20191230_WYL_MRM\MeiZhou\raw\20191224cal";
let files = mzML 
:> wiff.rawfiles("[-]?LM\d+") 
:> as.object 
:> do.call("GetRawFileList");

print(files);

files = list(
	samples   = "D:\20191230_WYL_MRM\MeiZhou\raw\20191224sample", 
	reference = "D:\20191230_WYL_MRM\MeiZhou\raw\20191224cal"
);

files
:> wiff.rawfiles("[-]?LM\d+")
:> as.object 
:> do.call("GetRawFileList")
:> print
;