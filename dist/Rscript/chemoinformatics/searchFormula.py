import mzkit

from mzkit import formula

D_glucose = formula::scan("C6H12O6")
print(D_glucose)
print(formula::eval(D_glucose))

print("run search test")
formulas = formula::candidates(formula::eval(D_glucose), ppm = 5, C= [3,12], H = [3,12], O = [3,12], N = [0, 4])
print(as.data.frame(formulas))

# PC 32:1 C40H78NO8P

exact_mass = formula::eval(C40H78NO8P)
formulas = formula::candidates(exact_mass, ppm = 5, C= [6,72], H = [3,120], O = [1,20], N = [0, 18], P  = [0, 18], S = [0, 20])
list = as.data.frame(formulas)

list = list[ order(list[, "ppm"]) , ]

print("search for exact mass value: ${exact_mass}")
print(list)