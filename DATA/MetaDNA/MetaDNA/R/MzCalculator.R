## 从mass分子质量计算出相应的前体离子的结果数据
##
## @param mode 电离模式，默认是阳离子，1: positive, -1: negative
mz.calculator <- function(mass, mode = 1) {
    calc <- NA;

    if (mode == 1) {
        calc <- positive();
    } else {
        calc <- negative();
    }

    # 枚举计算器之中的所有的前体离子的类型，然后计算完成之后返回数据框
    out <- c();
    
    for(name in names(calc)) {
        type <- calc[[name]];
        out  <- rbind(out, list(
            precursor_type = type$Name, 
            charge         = type$charge, 
            M              = type$M, 
            adduct         = type$adduct, 
            "m/z"          = type$calc.mz(mass)
        )); 
    }

    rownames(out) <- names(calc);
    out;
}