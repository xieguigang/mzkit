# ``m/z`` value and ``precursor type``

The ``m/z`` (_mass to charge ratio_) value is a kind of numeric value to represents the mass value of the molecular fragment in Mass Spectrometry data analysis, it can be calculated in such division expression:

```
m/z = mass / charge
```

For analysis the mass composition by using mass spectrometer, the molecule must have charge value for obtained acceleration in Magnetic Fields. In order to 


Here is a R# demo script for calculate m/z value for each known precursor type, just simply calling the ``m/z`` function:

```R
imports "mzkit.math" from "mzkit.dll";

print(mz(233, "+"));
print(mz(233, "-"));

# adduct              charge  M  mz                precursor_type
# 0                   1       1  233               [M]+
# 1.007276            3       1  78.6739426666667  [M+3H]3+
# 8.33459             3       1  86.0012566666667  [M+2H+Na]3+
# 15.76619            3       1  93.4328566666667  [M+H+2Na]3+
# 22.989218           3       1  100.655884666667  [M+3Na]3+
# 1.007276            2       1  117.507276        [M+2H]2+
# 9.52055             2       1  126.02055         [M+H+NH4]2+
# 11.998247           2       1  128.498247        [M+H+Na]2+
# 19.985217           2       1  136.485217        [M+H+K]2+
# 21.52055            2       1  138.02055         [M+ACN+2H]2+
# 22.989218           2       1  139.489218        [M+2Na]2+
# 42.033823           2       1  158.533823        [M+2ACN+2H]2+
# 62.547097           2       1  179.047097        [M+3ACN+2H]2+
# 1.007276            1       1  234.007276        [M+H]+
# 6.941               1       1  239.941           [M+Li]+
# 0.0208239999999975  1       1  233.020824        [M-H2O+NH4]+
# -35.022144          1       1  197.977856        [M+H-2H2O]+
# -17.007434          1       1  215.992566        [M+H-H2O]+
# 18.033823           1       1  251.033823        [M+NH4]+
# 22.989218           1       1  255.989218        [M+Na]+
# 33.033489           1       1  266.033489        [M+CH3OH+H]+
# 38.963158           1       1  271.963158        [M+K]+
# 42.033823           1       1  275.033823        [M+ACN+H]+
# 44.97116            1       1  277.97116         [M+2Na-H]+
# 61.06534            1       1  294.06534         [M+IsoProp+H]+
# 64.015765           1       1  297.015765        [M+ACN+Na]+
# 76.91904            1       1  309.91904         [M+2K-H]+
# 79.02122            1       1  312.02122         [M+DMSO+H]+
# 83.06037            1       1  316.06037         [M+2ACN+H]+
# 84.05511            1       1  317.05511         [M+IsoProp+Na+H]+
# 1.007276            1       2  467.007276        [2M+H]+
# 18.033823           1       2  484.033823        [2M+NH4]+
# 22.989218           1       2  488.989218        [2M+Na]+
# 38.963158           1       2  504.963158        [2M+K]+
# 42.033823           1       2  508.033823        [2M+ACN+H]+
# 64.015765           1       2  530.015765        [2M+ACN+Na]+

# adduct      charge  M  mz                precursor_type
# 0           -1      1  233               [M]-
# -1.007276   -3      1  76.6593906666667  [M-3H]3-
# -1.007276   -2      1  115.492724        [M-2H]2-
# -19.01839   -1      1  213.98161         [M-H2O-H]-
# -1.007276   -1      1  231.992724        [M-H]-
# 20.974666   -1      1  253.974666        [M+Na-2H]-
# 34.969402   -1      1  267.969402        [M+Cl]-
# 36.948606   -1      1  269.948606        [M+K-2H]-
# 44.998201   -1      1  277.998201        [M+FA-H]-
# 59.013851   -1      1  292.013851        [M+Hac-H]-
# 78.918885   -1      1  311.918885        [M+Br]-
# 112.985586  -1      1  345.985586        [M+TFA-H]-
# 18.998      -1      1  251.998           [M+F]-
# -1.007276   -1      2  464.992724        [2M-H]-
# 44.998201   -1      2  510.998201        [2M+FA-H]-
# 59.013851   -1      2  525.013851        [2M+Hac-H]-
# -1.007276   -1      3  697.992724        [3M-H]-
```

