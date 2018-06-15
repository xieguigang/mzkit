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
    
    print(names(calc));

    for(name in names(calc)) {
        type <- calc[[name]];
        cal  <- type$calc.mz;

        print(cal);

        r <- c(type$Name, 
               type$charge, 
               type$M, 
               type$adduct, 
               cal(mass)
        );
        out <- rbind(out, r); 
    }

    rownames(out) <- names(calc);
    colnames(out) <- c("precursor_type", "charge", "M", "adduct", "m/z");
    out;
}