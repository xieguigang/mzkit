imports "MsImaging" from "mzkit.plot";

# demo R# script for ms-imaging
#
# R'mpp A, Guenther S, Schober Y, Schulz O, Takats Z, Kummer W and Spengler B (2010) Angewandte Chemie International Edition 49 (22):3834-3838. (Figure 1)
# 
# 616.1767 Heme     [M]+
# 812.5566 PE(38:1) [M+K]+
# 798.541  PC(34:1) [M+K]+

const HR2MSI_mouse_urinary_bladder = viewer("E:\demo\HR2MSI mouse urinary bladder S096.imzML");

const scan_ppm as double  = 20;
const threshold as double = 0.0001;
const output_img          = `${!script$dir}/ms_imaging/HR2MSI_mouse_urinary_bladder_S096_Figure_1.png`;

print(output_img);

HR2MSI_mouse_urinary_bladder 
:> layer(
	mz = [
		616.1767, # Heme     [M]+
		812.5566, # PE(38:1) [M+K]+
		798.541   # PC(34:1) [M+K]+
	], 
	ppm       = scan_ppm, 
	threshold = threshold, 
	color     = "darkblue,blue,skyblue,green,Lime,SpringGreen,Fuchsia,Magenta,purple,BlueViolet,red"
)
:> flatten(bg = "black")
:> save.graphics(file = output_img)
;