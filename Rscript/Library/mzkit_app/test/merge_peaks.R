require(mzkit);

let NC = [
    "F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L62.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L63.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L64.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L65.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L66.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L67.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L68.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L69.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L70.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L71.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L72.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L73.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L74.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L75.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L76.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L77.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L78.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L79.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L80.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L61.cache"
];

let Prostate_cancer = [
    "F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L11.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L12.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L13.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L14.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L15.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L16.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L17.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L18.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L19.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L20.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L1.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L2.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L3.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L4.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L5.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L6.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L7.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L8.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L9.cache"
"F:\datapool\MTBLS6039\FILES\DERIVED_FILES\POS\L10.cache"
];

NC = lapply(tqdm(NC), file -> read.cache(file)) |> unlist();
Prostate_cancer = lapply(tqdm(Prostate_cancer), file -> read.cache(file)) |> unlist();

print(NC);
print(Prostate_cancer);

NC = unionPeaks(NC, norm = TRUE, aggreate.sum =TRUE ,matrix = TRUE);
Prostate_cancer = unionPeaks(Prostate_cancer, norm = TRUE, aggreate.sum =TRUE ,matrix = TRUE);

let logfc = data::logfc(Prostate_cancer, NC, lb1 = "Prostate_cancer", lb2 = "NC");

NC = NC |> as.data.frame();
NC = NC[order(NC$intensity ,decreasing=TRUE),];
Prostate_cancer = Prostate_cancer |> as.data.frame();
Prostate_cancer= Prostate_cancer[order(Prostate_cancer$intensity ,decreasing=TRUE),];

print(NC, max.print = 13);
print(Prostate_cancer, max.print = 13);
print(logfc, max.print = 13);

write.csv(NC, file = "./NC.csv");
write.csv(Prostate_cancer, file = "./Prostate_cancer.csv");
write.csv(logfc, file = "./logfc.csv");
