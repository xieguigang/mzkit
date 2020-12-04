imports "MsImaging" from "mzkit.plot";

# demo R# script for ms-imaging
#
# R'mpp A, Guenther S, Schober Y, Schulz O, Takats Z, Kummer W and Spengler B (2010) Angewandte Chemie International Edition 49 (22):3834-3838. (Figure 1)
# 
# Red   616.1767 Heme     [M]+
# Green 812.5566 PE(38:1) [M+K]+
# Blue  798.541  PC(34:1) [M+K]+

const HR2MSI_mouse_urinary_bladder = viewer("E:\demo\HR2MSI mouse urinary bladder S096.imzML");

const output_img         = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096.png`;
const output_red_layer   = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096_Heme_red.png`;
const output_green_layer = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096_PE(38_1)_green.png`;
const output_blue_layer  = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096_PC(34_1)_blue.png`;

const scan_ppm as double  = 20;
const threshold as double = 0;

let extract_layer as function(mz, colors, file) {
	let img = HR2MSI_mouse_urinary_bladder 
	:> layer(
		mz        = mz, 
		ppm       = scan_ppm, 
		threshold = threshold, 
		color     = colors
	);
	
	save.graphics(img, file = file);
}

let R = extract_layer(mz = 616.1767, colors = "OrRd:c8", file = output_red_layer);
let G = extract_layer(mz = 812.5566, colors = "YlGn:c8", file = output_green_layer);
let B = extract_layer(mz = 798.541,  colors = "PuBu:c8", file = output_blue_layer);

save.graphics(flatten(layers = [R, G, B]), file = output_img);