imports 'mzkit.formula' from "mzkit.dll";

print("(CH3)3CH" :> scan.formula);
print("H2O" :> scan.formula);
print("(CH3)4C" :> scan.formula);
print("C18H23N4O14P(C9H11N2O8P)5" :> scan.formula);
print("C18H23N4O14P(C9H11N2O8P)n" :> scan.formula(n=5));