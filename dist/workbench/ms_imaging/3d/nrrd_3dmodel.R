require(mzkit);
require(DICOM);

options(memory.load = "max");

imports "MSI" from "mzkit";
imports "NRRD" from "DICOM";

let source = list.files("G:\demo\3D\single_ions\413.0958", pattern = "*.dat");
let z = as.numeric(basename(source));

source = source[order(z)];

let nrrd = NRRD::write.nrrd_session(file = "G:\demo\3D\single_ions\413.0958.nrrd",
   dims = [528, 320], z = length(z)
);

print(z);

for(path in source) {
    path 
    |> readBin(what = "msi_layer")
    |> MSI::raster()    
    |> nrrd()
    ;

    print(path);
}

close(nrrd);