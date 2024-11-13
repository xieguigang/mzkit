require(mzkit);

let neg_adducts = [
    "[M]-" "[M-H]-" "[M-K]-" "[M-Na]-" 
];

print(neg_adducts);
# print(rank_adducts("C25H26N9NaO8S2", neg_adducts, -1));
print(rank_adducts("CH4", neg_adducts, -1));