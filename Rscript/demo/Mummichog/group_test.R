imports "package_utils" from "devkit";

package_utils::attach("D:\mzkit\Rscript\Library\mzkit_app");

imports "Mummichog" from "mzkit";

setwd(@dir);

peaks = read.csv("./peaks.csv", row.names = NULL);

groups = group_peaks(peaks);