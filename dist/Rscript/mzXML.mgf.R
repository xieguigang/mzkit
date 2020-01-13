imports "mzkit.assembly" from "mzkit.dll"; 

let mzxml as string = ?"--mzxml";
let mgf as string   = ?"--out" || `${dirname(mzxml)}/${basename(mzxml)}.mgf`;

mzxml.mgf(mzxml) :> write.mgf(file = mgf);