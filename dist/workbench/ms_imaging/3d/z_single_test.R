require(mzkit);

imports "MSI" from "mzkit";

let single = open.mzpack("E:\etc\016.mzPack", verbose = FALSE);


MSI::z_assembler(single, file = "E:\etc\test.mzImage", verbose = TRUE,
            z.pattern = $"\d+");