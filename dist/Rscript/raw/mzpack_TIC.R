imports "mzweb" from "mzkit";

using mzpack as mzweb::open("F:\20211123_CDF\P210702366.mzpack") {
	write.csv(TIC(mzpack), file = "F:\20211123_CDF\P210702366_TIC.csv");
}