require(mzkit);

let neg_adducts = [
    "[M-H]-" "[M-K]-" "[M-Na]-" "[M]-"
];

print(neg_adducts);
print(rank_adducts("C25H26N9NaO8S2", neg_adducts, -1));