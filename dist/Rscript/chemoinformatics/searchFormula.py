import mzkit

from mzkit import formula

D_glucose = formula::scan("C6H12O6")
print(D_glucose)
print(formula::eval(D_glucose))

print("run search test")
formulas = formula::candidates(formula::eval(D_glucose), ppm = 30, C= [3,12], H = [3,12], O = [3,12], N = [0, 4])

print(formulas)

test2 =  formula::candidates(formula::eval(C19H20O9), ppm = 5, C= [3,32], H = [3,32], O = [3,12], N = [0, 4]) 

print(test2)