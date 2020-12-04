imports "MsImaging" from "mzkit.plot";

viewer("E:\demo\HR2MSI mouse urinary bladder S096.imzML")
:> layer(mz = 404.1111325)
:> save.graphics(file = `${!script$dir}/HR2MSI_mouse_urinary_bladder_S096.png`)
;