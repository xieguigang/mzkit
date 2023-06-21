require(mzkit);

imports "formula" from "mzkit";

let f = ["","C6H33O9SNa","CCCCCCH56SPO88Cl1"];

# -1 will be return if the given formula is empty or error/invalid
print(formula::eval(f));