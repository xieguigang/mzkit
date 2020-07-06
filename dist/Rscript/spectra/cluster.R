imports ["math", "assembly"] from "mzkit";
imports "mzkit.visual" from "mzkit.plot";

let clusters = 
"D:\biodeep\B062.mgf"
:> read.mgf
:> mgf.ion_peaks
:> centroid
:> which(ms2 -> as.object(ms2)$fragments > 0)
:> spectrum_tree.cluster()
:> cluster.nodes
:> projectAs(as.object)
;

print(clusters :> sapply(c -> c$MID));

for(cluster in clusters) {
	let dir = `D:/plot/${cluster$MID}/`;
	
	write.mgf(cluster$cluster, file = `${dir}/spectrum.mgf`);
	
	for(matrix in cluster$cluster) {
		let png = `${dir}/${file.index(matrix)}.png`;
		
		matrix 
		:> mass_spectrum.plot 
		:> save.graphics(file = png);
	}
}