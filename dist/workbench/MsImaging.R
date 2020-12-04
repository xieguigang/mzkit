imports "MsImaging" from "mzkit.plot";

# demo R# script for ms-imaging
#
# R'mpp A, Guenther S, Schober Y, Schulz O, Takats Z, Kummer W and Spengler B (2010) Angewandte Chemie International Edition 49 (22):3834-3838. (Figure 1)
# 
# Red   616.1767 Heme     [M]+
# Green 812.5566 PE(38:1) [M+K]+
# Blue  798.541  PC(34:1) [M+K]+

const HR2MSI_mouse_urinary_bladder = viewer("E:\demo\HR2MSI mouse urinary bladder S096.imzML");
const output_img as string = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096.png`;

let R = HR2MSI_mouse_urinary_bladder :> layer(mz = 616.1767, ppm = 20, color = "OrRd:c8");
let G = HR2MSI_mouse_urinary_bladder :> layer(mz = 812.5566, ppm = 20, color = "YlGn:c8");
let B = HR2MSI_mouse_urinary_bladder :> layer(mz = 798.541,  ppm = 20, color = "PuBu:c8");

save.graphics(flatten(layers = [R, G, B]), file = output_img);