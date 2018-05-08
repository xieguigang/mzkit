require(VisualBasic);

tolerance.deltaMass <- function(da = 0.3) {
    function(a, b) abs(a - b) <= da;
}

tolerance.ppm <- function(ppm = 20) {
    function(a, b) PPM(a, b) <= ppm;
}

PPM <- function(measured, actualValue) {
    # |(实验数据 - 数据库结果)| / 实验数据 * 1000000
    (abs(measured - actualValue) / actualValue) * 1000000;
}

# KEGG注释原理：假设在某一些特定类型的代谢过程而言，其反应底物和反应产物的区别仅仅在于一些基团的加减，则二级结构应该是比较相似的
# 故而，对于一个反应过程 A <=> B ，假若数据库之中没有找B的二级数据，但是却找到了A的二级数据，那么就可以通过这个原理来进行B的注释

## 通过一级质谱信息搜索KEGG数据库
##
## @param KEGG 数据结构为：
##        DB[KEGGID] = [KEGGID, mass, formula, name]
find.KEGG <- function(precursor, KEGG, tolerance = tolerance.ppm(), precursor_type = "[M+H]+") {
    # binarysearch
    get.MS1 <- function(compound) {
        mass <- compound[["mass"]];
        mz   <- get.PrecursorMZ(mass, precursor_type);
        mz;
    }
    get.index <- function(compound) {
        compound[["KEGGID"]];
    }
    index <- binarysearch(KEGG, get.MS1, precursor, tolerance, get.index);

    # 从一级质谱信息只能够从质量上找出一系列符合误差要求的化合物
    KEGG[index];
}

# 进行KEGG辅助注释的前提是必须要从实验数据之中已经注释出了一个非常确定的KEGG代谢物
# 假设这个代谢物的数据结构为
# [KEGGID, ms2, ms1]
# 
# 假设代谢过程的数据结构为
# [RXNID, reactants, products]
# 其中reactants和products都是化合物的KEGG编号

# 使用KEGG的代谢物数据库和代谢反应过程数据库找出和目标已经鉴定出的代谢物的所有代谢过程相关的未鉴定代谢物的KEGG注释信息
#
# @param ms2.similar 比较两个二级质谱矩阵是否相似，函数返回逻辑值
# 
# 假设二级质谱的相似度的比对函数的接口形式为：
# ms2.alignment <- function(q, s) { score = SSM(q, s)； score >= threshold; }

# q,s为二级矩阵：
#
# mz, into
#
#
#
KEGG.rxnNetwork <- function(identified, sample, KEGG, RXN, ms2.similar, tolerance = tolerance.ppm(), precursor_type = "[M+H]+") {
    KEGGID <- identified[["KEGGID"]];
    RXN <- lapply(names(RXN), function(RXNID) {
        # 找出所有相关的代谢过程
        r <- RXN[[RXNID]];

        if (sum(r[["reactants"]] == KEGGID) > 0) {
            # 已经鉴定出来的代谢物在底物侧，则返回产物侧
            list(RXNID = RXNID, connector = r[["products"]]);
        } else if (sum(r[["products"]] == KEGGID) > 0) {
            # 已鉴定代谢物在产物侧，则返回底物侧
            list(RXNID = RXNID, connector = r[["reactants"]]);
        } else {
            # 不是这个代谢过程的成员，则返回空值
            NULL;
        }
    });

    ## 删除集合之中的空值 
    RXN <- %NOT% (RXN %IS_NOTHING%);


}