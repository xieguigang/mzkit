# 假设二级质谱的相似度的比对函数的接口形式为：
# ms2.alignment <- function(q, s) { score }

# q,s为二级矩阵：
#
# mz, into
#
#

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
