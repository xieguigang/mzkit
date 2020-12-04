imports "MsImaging" from "mzkit.plot";

viewer("E:\demo\HR2MSI mouse urinary bladder S096.imzML")
:> layer(mz = 812.5566)
:> save.graphics(file = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096.png`)
;