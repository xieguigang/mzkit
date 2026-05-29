require(mzkit);

let xcms_peaks_dir = ?"--xcms" || stop("xcms single peaks folder path must be provided!");
let outputdir = ?"--outputdir" || file.path(dirname( xcms_peaks_dir), `${basename(xcms_peaks_dir)}_peaktable`); 
let peakfiles = list.files(xcms_peaks_dir, pattern = "*.csv");
let peaktable = make_peak_alignment(peakfiles, max_rtwin = 15,mzdiff = 0.01);
let rt_shifts = attr(peaktable,"rt.shift");

write.csv(peaktable, file = `${outputdir}/peaktable.csv`, 
        row.names = TRUE);
write.csv(rt_shifts, file = `${outputdir}/rt_shifts.csv`, 
    row.names = TRUE);

bitmap(file = file.path(outputdir, "rt_shifts.png"), size = [4000, 2700], padding = [50 650 200 200]) {
    plot(rt_shifts, res = 1000, grid.fill = "white");
}
bitmap(file = file.path(outputdir, "peakset.png")) {
    plot(as.peak_set(peaktable), scatter = TRUE, 
        dimension = "npeaks");
}