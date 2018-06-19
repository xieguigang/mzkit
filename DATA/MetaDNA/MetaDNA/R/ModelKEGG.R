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

ms2.similar.fn <- function(threshold) {
    function(q, s) {
        q   <- q[, 2];
		s   <- s[, 2];
		SSM <- sum(q * s) / sqrt(sum(q ^ 2) * sum(s ^ 2));

        list(similar       = SSM >= threshold, 
			 score.forward = SSM, 
			 score.reverse = SSM
		);
    }
}

# 进行KEGG辅助注释的前提是必须要从实验数据之中已经注释出了一个非常确定的KEGG代谢物
# 假设这个代谢物的数据结构为
# [KEGGID, ms2, ms1]
# 
# 假设代谢过程的数据结构为
# [RXNID, reactants, products]
# 其中reactants和products都是化合物的KEGG编号

# 算法计算过程
#
# 1. 在通过SSM鉴定之后，大致可以依据二级结果的相似度将sample分为已鉴定代谢物和未鉴定代谢物
# 2. 对未鉴定代谢物进行遍历，通过未鉴定的代谢物的mz进行KEGG代谢物的一级查找，找出所有的可能结果
# 3. 将查找到的KEGG编号从已鉴定代谢物之中取补集，即取出已鉴定代谢物之中不存在的KEGG编号
# 4. 利用KEGG代谢反应过程找出和未鉴定代谢物的KEGG编号相匹配的同过程内的KEGG代谢物对应的已鉴定代谢物的二级质谱信息
# 5. 进行二级比较，如果二级相似度较高，则确认该未鉴定代谢物可能为某一个KEGG编号

# 使用KEGG的代谢物数据库和代谢反应过程数据库找出和目标已经鉴定出的代谢物的所有代谢过程相关的未鉴定代谢物的KEGG注释信息
# 
# @param sample 一般是未鉴定的代谢物结果，peak_ms2？?
# @param RXN KEGG数据库之中的代谢过程的集合
# @param KEGG KEGG的代谢物注释信息库，可以将metadb之中的含有KEGG编号的物质注释信息取子集
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
KEGG.rxnNetwork <- function(identified, sample, KEGG, KEGG.rxn, 
    ms2.similar    = ms2.similar.fn(0.65), 
    tolerance      = tolerance.ppm(), 
    precursor_type = "[M+H]+") {

	identified.uid <- as.vector(identified[, "uid"]);
	identified <- GroupBy(identified, "KEGG");
	identify.KEGG <- names(identified);
			
    KEGG.rxnTuple <- function(unknown.mz, unknwon.ms2) {
		# 2. 对未鉴定代谢物进行遍历，通过未鉴定的代谢物的mz进行KEGG代谢物的一级查找，找出所有的可能结果
        KEGG.list <- find.KEGG(unknown.mz, KEGG, tolerance, precursor_type);
		KEGG.list <- as.vector(KEGG.list[, "KEGG"]);
		# 3. 将查找到的KEGG编号从已鉴定代谢物之中取补集，即取出已鉴定代谢物之中不存在的KEGG编号
		KEGG.list <- KEGG.list(!(KEGG.list %in% identify.KEGG));
		
		metaDNA.identify <- c();
		
		# 通过KEGG代谢反应过程的模型找出相对应的KEGG编号
		for (rxn in KEGG.rxn) {
			for (kegg_id in KEGG.list) {
				identify_kegg <- NULL;
				
				if (kegg_id %in% rxn$reactants) {
					# 这个未鉴定代谢物的可能的KEGG编号出现在了这个反应过程的底物之中
					# 则需要通过反应的产物的二级结果来进行辅助鉴定
					identify_kegg <- rxn$products;
				} else if (kegg_id %in% rxn$products) {
					identify_kegg <- rxn$reactants;
				} else {
					identify_kegg <- NULL;
				}
				
				if (!IsNothing(identify_kegg)) {
				
					# 找到了匹配的结果
					# 开始进行二级比对
					
					for (id in identify_kegg) {
						# 为了得到已鉴定代谢物的二级信息，需要查找这个id编号是否存在与已鉴定化合物列表之中
						.identify <- identified[[id]];
						
						if (!IsNothing(.identify)) {
							# id编号存在
							# 取出二级信息
							# 和unknwon.ms2进行相似度比较
							ms2 <- .identify$ms2;
							
							# 5. 进行二级比较，如果二级相似度较高，则确认该未鉴定代谢物可能为某一个KEGG编号
							align <- ms2.similar(ms2, unknwon.ms2);
							
							if (align$similar) {
							
								# 2018-6-19 
								# 注意：
								# 从metaDB之中取出结果应该是通过这个kegg_id编号来完成，而不应该是.identify$libname
							
								append <- c(
									kegg_id,                         # 可能确定为目标代谢物的kegg编号
									sprintf("%s@%s", id, rxn$rxnID), # 利用metaDNA算法进行计算的调试traceback
									.identify$libname,               # 用于获取ms2信息作为进行比对作图所需要ref的二级质谱矩阵数据
									align$score.forward,             # 得分信息
									align$score.reverse              # 得分信息，这两个得分信息用来计算出当前的这个未鉴定代谢物的最佳的匹配结果
								);
								metaDNA.identify <- rbind(metaDNA.identify, append);
							}
						}
					}
				}
			}
		}
		
		# 取出平均得分最高的作为最终的可能的注释结果
		top       <- null;
		top.score <- -100;
		
		colnames(metaDNA.identify) <- c(
			"kegg_id", 
			"traceback", 
			"ref.libname", 
			"forward", 
			"reverse"
		);
		metaDNA.identify <- .as.list(metaDNA.identify);
		
		for (identify in metaDNA.identify) {
			s <- identify$forward + identify$reverse;
			
			if (s > top) {
				top.score <- s;
				top <- identify;
			}
		}
		
		top;
    }

	metaDNA.result <- list();
	
	for (unknown in sample) {
		if (unknown$uid %in% identified.uid) {
			# 这是一个已鉴定代谢物，跳过
			next;
		}
		
		unknown.mz  <- unknown$mz;
		unknwon.ms2 <- unknown$ms2;
		unknown.candidate <- KEGG.rxnTuple(unknown.mz, unknown.ms2);

		metaDNA.result[[unknown$uid]] <- list(
			metaInfo.candidate = unknown.candidate, 
			uid = unknown$uid, 
			query = unknown
		);
	}
	
	metaDNA.result;
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