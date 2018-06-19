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
## @param KEGG 为一个list列表，其数据结构为：
##        DB[KEGGID] = [KEGGID, mass, formula, name]
## @
find.KEGG <- function(precursor, KEGG, tolerance = tolerance.ppm(), precursor_type = "[M+H]+") {
    # binarysearch
    get.MS1 <- function(compound) {
        mass <- compound[["mass"]];
        mz   <- get.PrecursorMZ(mass, precursor_type);
        mz;
    }

    # 因为binary search只会返回一个index，所以需要对KEGG数据进行事先分组处理
    result <- binarySearch.list(
        list = KEGG, 
        find = precursor, 
        key  = get.MS1,
        compares = function(a, b) {
            if (tolerance(a, b)) {
                0;
            } else if (a < b) {
                -1; 
            } else {
                1;
            }
        });

    # 从一级质谱信息只能够从质量上找出一系列符合误差要求的化合物
    KEGG[[index]];
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
# @param identified 通过SSM算法得到的已经被视为鉴定结果的已知物质，这个数据集之中必须要包含有KEGG编号列
# @param ms2.similar 比较两个二级质谱矩阵是否相似，函数返回逻辑值
# 
#   假设二级质谱的相似度的比对函数的接口形式为：
#   ms2.alignment <- function(q, s) { score = SSM(q, s)； score >= threshold; }
#
# q,s为二级矩阵：
#
# mz, into
# mz, into
# mz, into
# mz, into
#
KEGG.rxnNetwork <- function(identified, sample, KEGG, RXN, ms2.similar, tolerance = tolerance.ppm(), precursor_type = "[M+H]+") {
    # 获取得到本已鉴定代谢物物质的KEGG编号
    KEGGID <- identified[["KEGGID"]];

    # 这个函数之中的算法仅应用于KEGG代谢物
    if (IsNothing(KEGGID)) {
        return(NA);
    }
    
    RXN <- lapply(names(RXN), function(RXNID) {
        # 根据这个已鉴定代谢物的KEGG编号找出所有相关的代谢过程
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
    RXN <- NOT(RXN, IS_NOTHING);

    # 在partner里面找出一级匹配的，并且二级和identified相似的即很有可能是目标代谢物
    # 
    # 首先将所有二级相似的都找出来
    query.ms2 <- identified[["ms2"]];
    sample <- lapply(names(sample), function(index) {
        unknown <- sample[[index]];
        ms2 <- unknown[["ms2"]];

        if (ms2.similar(ms2, query.ms2)) {
            # 找到了一个目标物质
            unknown;
        } else {
            NULL;
        }
    });

    sample <- NOT(sample, IS_NOTHING);

    # 然后对sample的subset进行KEGG的一级质谱结果搜索
    sample <- lapply(names(sample), function(index) {
        unknown <- sample[[index]];
        ms1 <- unknown[["ms1"]];
        KEGG.list <- find.KEGG(ms1, KEGG, tolerance, precursor_type);

        if (length(KEGG.list) == 0) {
            NULL;
        } else {

            ID.list <- names(KEGG.list);
            connection = list();

            # 而且能够通过ms1找到相应的KEGG代谢物
            # 则判断KEGG代谢物是否在代谢网络的connector里面
            for(r in RXN) {
                intersection <- intersect(r$connector, ID.list); 

                if (length(intersection) > 0) {
                    # 找到了一个
                    # 尝试将未知代谢物鉴定为目标KEGG代谢物
                    connection[r$RXNID] = list(KEGG = intersection, metabolite = unknown);
                }
            }

            if (length(connection) > 0) {
                # 候选鉴定列表
                connection;
            } else {
                NULL;
            }
        }
    });

    sample <- NOT(sample, IS_NOTHING);

    # 返回候选列表
    # 这个候选列表都是在代谢过程上面和identified有关联的，并且ms1能够在KEGG之中找到结果，ms2与identieid相似
    sample;
} 

NOT <- function(list, assert) {
    index <- lapply(names(list), function(name) {
        a <- list[[name]];
        assert(a);
    });
    index <- which(as.vector(index));

    sub.names <- names(list);
    sub.names <- sub.names[index]; 
    sub.list  <- list[index];
    names(sub.list) <- sub.names;

    sub.list;
}