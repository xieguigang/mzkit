require(mzkit);

let rawdata = ?"--raw" || stop("no rawdata file was provided!");
let outfile = ?"--out" || `${dirname(rawdata)}/${basename(rawdata)}.txt`;

if (file.ext(rawdata) == "raw") {

} else {
    
}