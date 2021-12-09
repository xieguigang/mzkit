require(mzkit);

A = new("mzInto", mz = c(156,456,42,312,999), intensity = c(165,78,97,651,32));
B_similar = new("mzInto", mz = c(156,456,42,312, 1233), intensity = c(165,78,97,651,32));
B_diff = new("mzInto", mz = c(579,54,455,658, 1233), intensity = c(245,5755,452,45,545));

test_score = function(x, y) {
    rx = mzkit::globalAlign(toMsMatrix(x), toMsMatrix(y));
    c1 = mzkit::MScos(rx, toMsMatrix(y));

    ry = mzkit::globalAlign(toMsMatrix(y), toMsMatrix(x));
    c2 = mzkit::MScos(ry, toMsMatrix(x));

    cw1 = mzkit::weighted_MScos(rx, toMsMatrix(y));
    cw2 = mzkit::weighted_MScos(ry, toMsMatrix(x));

    j = mzkit::MSjaccard(toMsMatrix(y), toMsMatrix(x));

    e = mzkit::MSDiffEntropy(x, y);

    print(data.frame(
        type = c("forward", "reverse", "forward(weighted)", "reverse(weighted)", "jaccard", "entropy"),
        score = c(c1, c2, cw1, cw2, j, e)
    ));
}

test_score(A, B_similar);
test_score(A, B_diff);