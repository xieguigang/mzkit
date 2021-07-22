imports ["formula", "math"] from "mzkit";

const isotope = "CHCl3"
|> isotope_distribution
|> toMS
|> centroid
;

print(ms1);

# 118 100
# 119 1.1
# 120 97.2
# 121 1.1
# 122 31.5
# 123 0.3
# 124 3.4

pause();