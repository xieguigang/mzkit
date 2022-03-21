import mzkit

from mzkit import formula

exact_mass = 131.0946
formulas = formula::candidates(exact_mass, ppm = 30, C = [6,72], H = [3,120], O = [0,20], N = [0, 18], P  = [0, 18], S = [0, 20])
list = as.data.frame(formulas)
list = list[ order(list[, "ppm"]) , ]

print(`search for exact mass value: ${exact_mass}`)
print(list)