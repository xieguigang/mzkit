require(mzkit);

imports "math" from "mzkit";
imports "data" from "mzkit";
imports "visual" from "mzplot";

spectrum = data.frame(mz = [69.071,86.0969], into = [7.917962, 100.0]);
spectrum = libraryMatrix(spectrum, "spectrum data");

# 0.2605222463607788
print(math::spectral_entropy(spectrum));


spec_query =libraryMatrix( data.frame(mz = [69.071,86.066, 86.0969], into = [ 7.917962, 1.021589, 100.0]));
spec_reference = libraryMatrix(data.frame(mz = [41.04, 69.07, 86.1], into = [37.16, 66.83, 999.0]));

# Calculate entropy similarity.
similarity = math::spectral_entropy(spec_query, spec_reference);
print(similarity);
# The output should be: Entropy similarity:0.8984397722577456.
setwd(@dir);

bitmap(file = "./spectral_entropy_demo1.png") {
	plot(spec_query, alignment = spec_reference );
}

alignment = "71.0857_0.0272_0 81.0695_0.0226_0 83.0851_0.0123_0 85.1013_0.0168_0 95.0859_0.027_0 97.101_0.0124_0 98.9844_0.0234_0.0144 109.101_0.0158_0 155.0099_0.2244_0.1298 184.0741_0.024_0 297.1243_0.2444_0.2866 298.1254_0.0211_0.0838 299.1393_0.0194_0.0263 341.3027_0.1056_0.0544 342.3076_0_0.0204 583.2535_0.1689_0.2194 584.2549_0.0348_0.1456 585.2607_0_0.0193";
alignment = strsplit(alignment, " ");
alignment = alignment |> lapply(function(str) as.numeric(strsplit(str, "_")));

let mz = sapply(alignment, x -> x[1]);
let query = sapply(alignment, x -> x[2]);
let reference = sapply(alignment, x -> x[3]);

query = libraryMatrix(data.frame(mz, into = query), "query");
reference = libraryMatrix(data.frame(mz, into = reference), "reference_hit");

print(math::spectral_entropy(query, reference));
print([math::cosine(query, reference)]::cosine);

bitmap(file = "./spectral_entropy.png") {
	plot(query, alignment = reference);
}