# https://github.com/xieguigang/MassSpectrum-toolkits/blob/6f4284a0d537d86c112877243d9e3b8d9d35563f/DATA/ms2_math-core/Ms1/PrecursorType.vb

## @param mass 分子的质量
## @param adduct 离子的质量
## @param charge 加和物的电荷数量
##
## @return 返回加和物的m/z数据
Adduct.mass <- function(mass, adduct, charge) {
    return(mass / abs(charge) + adduct);
}

## 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
## @param precursorMZ MS/MS 之中的前体离子的m/z
## @param charge 离子的电荷数量
## @param adduct 加和物的离子模式的mass
Reverse.mass <- function(precursorMZ, M, charge, adduct) {
    return((precursorMZ - adduct) * abs(charge) / M);
}

## @param charge 电荷数，这里只需要绝对值就行了，不需要带有符号
## @param type 前体离子的类型名称
## @param calc 从MS/MS之中的加和物离子的m/z结果 precursorMZ 反推回mass结果的计算过程
AddKey <- function(type, charge, M, adducts) {

    out <- list();
	
    out$Name   <- type;
    out$calc   <- function(precursorMZ) {
        return(Reverse.mass(precursorMZ, M, charge, adducts));
    };
    out$charge <- charge;
    out$M      <- M;
	out$adduct <- adducts;
	out$cal.mz <- function(mass) {
		return(Adduct.mass(mass, adducts, charge));
	}

    return(out);
}

# http://fiehnlab.ucdavis.edu/staff/kind/Metabolomics/MS-Adduct-Calculator
#
# Example: 
# 1) Find Adduct: Taxol, C47H51NO14, M=853.33089 
#    Enter 853.33089 in green box read M+22.9, m/z=876.320108 
#
# 2) Reverse: take 12 Tesla-FT-MS result out of MS m/z=876.330
#    suspect M+Na adduct, read M=853.340782, enter this value into formula finder 
#    with 2 ppm mass accuracy (CHNSOP enabled) get some thousand results, 
#    compare isotopic pattern, get happy.

# Table 1. Monoisotopic exact masses of molecular ion adducts often observed in ESI mass spectra
#  	 	 	 	 	                                           Your M here:	Your M+X or M-X
#  	 	 	 	 	                                            853.33089	876.32
# Ion name	        Ion mass	    Charge	Mult	Mass	    Result:	    Reverse:
# 1. Positive ion mode	 	 	 	 	 	 
# M+3H	            M/3 + 1.007276	    3+	0.33	1.007276	285.450906	291.099391
# M+2H+Na	        M/3 + 8.334590	    3+	0.33	8.334590	292.778220	283.772077
# M+H+2Na	        M/3 + 15.7661904	3+	0.33	15.766190	300.209820	276.340476
# M+3Na	            M/3 + 22.989218	    3+	0.33	22.989218	307.432848	269.117449
# M+2H	            M/2 + 1.007276	    2+	0.50	1.007276	427.672721	437.152724
# M+H+NH4	        M/2 + 9.520550	    2+	0.50	9.520550	436.185995	428.639450
# M+H+Na	        M/2 + 11.998247	    2+	0.50	11.998247	438.663692	426.161753
# M+H+K	            M/2 + 19.985217	    2+	0.50	19.985217	446.650662	418.174783
# M+ACN+2H	        M/2 + 21.520550	    2+	0.50	21.520550	448.185995	416.639450
# M+2Na	            M/2 + 22.989218	    2+	0.50	22.989218	449.654663	415.170782
# M+2ACN+2H	        M/2 + 42.033823	    2+	0.50	42.033823	468.699268	396.126177
# M+3ACN+2H	        M/2 + 62.547097	    2+	0.50	62.547097	489.212542	375.612903
# M+H	            M + 1.007276	    1+	1.00	1.007276	854.338166	875.312724
# M+NH4	            M + 18.033823	    1+	1.00	18.033823	871.364713	858.286177
# M+Na	            M + 22.989218	    1+	1.00	22.989218	876.320108	853.330782
# M+CH3OH+H	        M + 33.033489	    1+	1.00	33.033489	886.364379	843.286511
# M+K	            M + 38.963158	    1+	1.00	38.963158	892.294048	837.356842
# M+ACN+H	        M + 42.033823	    1+	1.00	42.033823	895.364713	834.286177
# M+2Na-H	        M + 44.971160	    1+	1.00	44.971160	898.302050	831.348840
# M+IsoProp+H	    M + 61.06534	    1+	1.00	61.065340	914.396230	815.254660
# M+ACN+Na	        M + 64.015765	    1+	1.00	64.015765	917.346655	812.304235
# M+2K-H	        M + 76.919040	    1+	1.00	76.919040	930.249930	799.400960
# M+DMSO+H	        M + 79.02122	    1+	1.00	79.021220	932.352110	797.298780
# M+2ACN+H	        M + 83.060370	    1+	1.00	83.060370	936.391260	793.259630
# M+IsoProp+Na+H	M + 84.05511	    1+	1.00	84.055110	937.386000	792.264890
# 2M+H	            2M + 1.007276	    1+	2.00	1.007276	1707.669056	1751.632724
# 2M+NH4	        2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
# 2M+Na	            2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
# 2M+K	            2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
# 2M+ACN+H	        2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
# 2M+ACN+Na	        2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
  	 	 	 
# 2. Negative ion mode	 	 	 	 	 
# M-3H	            M/3 - 1.007276	    3-	0.33	-1.007276	283.436354	293.113943
# M-2H	            M/2 - 1.007276	    2-	0.50	-1.007276	425.658169	439.167276
# M-H2O-H	        M- 19.01839	        1-	1.00	-19.01839	834.312500	895.338390
# M-H	            M - 1.007276	    1-	1.00	-1.007276	852.323614	877.327276
# M+Na-2H	        M + 20.974666	    1-	1.00	20.974666	874.305556	855.345334
# M+Cl	            M + 34.969402	    1-	1.00	34.969402	888.300292	841.350598
# M+K-2H	        M + 36.948606	    1-	1.00	36.948606	890.279496	839.371394
# M+FA-H	        M + 44.998201	    1-	1.00	44.998201	898.329091	831.321799
# M+Hac-H	        M + 59.013851	    1-	1.00	59.013851	912.344741	817.306149
# M+Br	            M + 78.918885	    1-	1.00	78.918885	932.249775	797.401115
# M+TFA-H	        M + 112.985586	    1-	1.00	112.985586	966.316476	763.334414
# 2M-H	            2M - 1.007276	    1-	2.00	-1.007276	1705.654504	1753.647276
# 2M+FA-H	        2M + 44.998201	    1-	2.00	44.998201	1751.659981	1707.641799
# 2M+Hac-H	        2M + 59.013851	    1-	2.00	59.013851	1765.675631	1693.626149
# 3M-H	            3M - 1.007276	    1-	3.00	1.007276	2560.999946	2627.952724

positive <- function() {
    pos <- list();

    # AddKey <- function(type, charge, M, adducts)

    pos$"M+3H"	         <- AddKey("[M+3H]3+",          charge = 3, M = 1, adducts = 1.007276);  # M/3 + 1.007276	    3+	0.33	1.007276	285.450906	291.099391 
    pos$"M+2H+Na"	     <- AddKey("[M+2H+Na]3+",       charge = 3, M = 1, adducts = 8.334590);  # M/3 + 8.334590	    3+	0.33	8.334590	292.778220	283.772077
    pos$"M+H+2Na"	     <- AddKey("[M+H+2Na]3+",       charge = 3, M = 1, adducts = 15.766190); # M/3 + 15.7661904	    3+	0.33	15.766190	300.209820	276.340476
    pos$"M+3Na"	         <- AddKey("[M+3Na]3+",         charge = 3, M = 1, adducts = 22.989218); # M/3 + 22.989218	    3+	0.33	22.989218	307.432848	269.117449
    pos$"M+2H"	         <- AddKey("[M+2H]2+",          charge = 2, M = 1, adducts = 1.007276);  # M/2 + 1.007276	    2+	0.50	1.007276	427.672721	437.152724
    pos$"M+H+NH4"	     <- AddKey("[M+H+NH4]2+",       charge = 2, M = 1, adducts = 9.520550);  # M/2 + 9.520550	    2+	0.50	9.520550	436.185995	428.639450
    pos$"M+H+Na"	     <- AddKey("[M+H+Na]2+",        charge = 2, M = 1, adducts = 11.998247); # M/2 + 11.998247	    2+	0.50	11.998247	438.663692	426.161753
    pos$"M+H+K"	         <- AddKey("[M+H+K]2+",         charge = 2, M = 1, adducts = 19.985217); # M/2 + 19.985217	    2+	0.50	19.985217	446.650662	418.174783
    pos$"M+ACN+2H"	     <- AddKey("[M+ACN+2H]2+",      charge = 2, M = 1, adducts = 21.520550); # M/2 + 21.520550	    2+	0.50	21.520550	448.185995	416.639450
    pos$"M+2Na"	         <- AddKey("[M+2Na]2+",         charge = 2, M = 1, adducts = 22.989218); # M/2 + 22.989218	    2+	0.50	22.989218	449.654663	415.170782
    pos$"M+2ACN+2H"	     <- AddKey("[M+2ACN+2H]2+",     charge = 2, M = 1, adducts = 42.033823); # M/2 + 42.033823	    2+	0.50	42.033823	468.699268	396.126177
    pos$"M+3ACN+2H"	     <- AddKey("[M+3ACN+2H]2+",     charge = 2, M = 1, adducts = 62.547097); # M/2 + 62.547097	    2+	0.50	62.547097	489.212542	375.612903
    pos$"M+H"	         <- AddKey("[M+H]+",            charge = 1, M = 1, adducts = 1.007276);  #  M + 1.007276	    1+	1.00	1.007276	854.338166	875.312724
    pos$"M+NH4"	         <- AddKey("[M+NH4]+",          charge = 1, M = 1, adducts = 18.033823); #  M + 18.033823	    1+	1.00	18.033823	871.364713	858.286177
    pos$"M+Na"	         <- AddKey("[M+Na]+",           charge = 1, M = 1, adducts = 22.989218); #  M + 22.989218	    1+	1.00	22.989218	876.320108	853.330782
    pos$"M+CH3OH+H"	     <- AddKey("[M+CH3OH+H]+",      charge = 1, M = 1, adducts = 33.033489); #  M + 33.033489	    1+	1.00	33.033489	886.364379	843.286511
    pos$"M+K"	         <- AddKey("[M+K]+",            charge = 1, M = 1, adducts = 38.963158); #  M + 38.963158	    1+	1.00	38.963158	892.294048	837.356842
    pos$"M+ACN+H"	     <- AddKey("[M+ACN+H]+",        charge = 1, M = 1, adducts = 42.033823); #  M + 42.033823	    1+	1.00	42.033823	895.364713	834.286177
    pos$"M+2Na-H"	     <- AddKey("[M+2Na-H]+",        charge = 1, M = 1, adducts = 44.971160); #  M + 44.971160	    1+	1.00	44.971160	898.302050	831.348840
    pos$"M+IsoProp+H"	 <- AddKey("[M+IsoProp+H]+",    charge = 1, M = 1, adducts = 61.06534);  #  M + 61.06534	    1+	1.00	61.065340	914.396230	815.254660
    pos$"M+ACN+Na"	     <- AddKey("[M+ACN+Na]+",       charge = 1, M = 1, adducts = 64.015765); #  M + 64.015765	    1+	1.00	64.015765	917.346655	812.304235
    pos$"M+2K-H"	     <- AddKey("[M+2K-H]+",         charge = 1, M = 1, adducts = 76.919040); #  M + 76.919040	    1+	1.00	76.919040	930.249930	799.400960
    pos$"M+DMSO+H"	     <- AddKey("[M+DMSO+H]+",       charge = 1, M = 1, adducts = 79.02122);  #  M + 79.02122	    1+	1.00	79.021220	932.352110	797.298780
    pos$"M+2ACN+H"	     <- AddKey("[M+2ACN+H]+",       charge = 1, M = 1, adducts = 83.060370); #  M + 83.060370	    1+	1.00	83.060370	936.391260	793.259630
    pos$"M+IsoProp+Na+H" <- AddKey("[M+IsoProp+Na+H]+", charge = 1, M = 1, adducts = 84.05511);  # 	M + 84.05511	    1+	1.00	84.055110	937.386000	792.264890
    pos$"2M+H"	         <- AddKey("[2M+H]+",           charge = 1, M = 2, adducts = 1.007276);  # 2M + 1.007276	    1+	2.00	1.007276	1707.669056	1751.632724
    pos$"2M+NH4"	     <- AddKey("[2M+NH4]+",         charge = 1, M = 2, adducts = 18.033823); # 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
    pos$"2M+Na"	         <- AddKey("[2M+Na]+",          charge = 1, M = 2, adducts = 22.989218); # 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
    pos$"2M+K"	         <- AddKey("[2M+K]+",           charge = 1, M = 2, adducts = 38.963158); # 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
    pos$"2M+ACN+H"	     <- AddKey("[2M+ACN+H]+",       charge = 1, M = 2, adducts = 42.033823); # 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
    pos$"2M+ACN+Na"	     <- AddKey("[2M+ACN+Na]+",      charge = 1, M = 2, adducts = 64.015765); # 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235

    return(pos);
}

negative <- function() {
    neg <- list();

    neg$"M-3H"	         <- AddKey("[M-3H]3-",          charge = -3, M = 1, adducts = -1.007276);   # M/3 - 1.007276	    3-	0.33	-1.007276	283.436354	293.113943
    neg$"M-2H"	         <- AddKey("[M-2H]2-",          charge = -2, M = 1, adducts = -1.007276);   # M/2 - 1.007276	    2-	0.50	-1.007276	425.658169	439.167276
    neg$"M-H2O-H"	     <- AddKey("[M-H2O-H]-",        charge = -1, M = 1, adducts = -19.01839);   # M - 19.01839	    1-	1.00	-19.01839	834.312500	895.338390
    neg$"M-H"	         <- AddKey("[M-H]-",            charge = -1, M = 1, adducts = -1.007276);   # M - 1.007276	    1-	1.00	-1.007276	852.323614	877.327276
    neg$"M+Na-2H"	     <- AddKey("[M+Na-2H]-",        charge = -1, M = 1, adducts = 20.974666);  # M + 20.974666	    1-	1.00	20.974666	874.305556	855.345334
    neg$"M+Cl"	         <- AddKey("[M+Cl]-",           charge = -1, M = 1, adducts = 34.969402);  # M + 34.969402	    1-	1.00	34.969402	888.300292	841.350598
    neg$"M+K-2H"	     <- AddKey("[M+K-2H]-",         charge = -1, M = 1, adducts = 36.948606);  # M + 36.948606	    1-	1.00	36.948606	890.279496	839.371394
    neg$"M+FA-H"	     <- AddKey("[M+FA-H]-",         charge = -1, M = 1, adducts = 44.998201);  # M + 44.998201	    1-	1.00	44.998201	898.329091	831.321799
    neg$"M+Hac-H"	     <- AddKey("[M+Hac-H]-",        charge = -1, M = 1, adducts = 59.013851);  # M + 59.013851	    1-	1.00	59.013851	912.344741	817.306149
    neg$"M+Br"	         <- AddKey("[M+Br]-",           charge = -1, M = 1, adducts = 78.918885);  # M + 78.918885	    1-	1.00	78.918885	932.249775	797.401115
    neg$"M+TFA-H"	     <- AddKey("[M+TFA-H]-",        charge = -1, M = 1, adducts = 112.985586); # M + 112.985586	    1-	1.00	112.985586	966.316476	763.334414
    neg$"2M-H"	         <- AddKey("[2M-H]-",           charge = -1, M = 2, adducts = -1.007276);   # 2M - 1.007276	    1-	2.00	-1.007276	1705.654504	1753.647276
    neg$"2M+FA-H"	     <- AddKey("[2M+FA-H]-",        charge = -1, M = 2, adducts = 44.998201);  # 2M + 44.998201	    1-	2.00	44.998201	1751.659981	1707.641799
    neg$"2M+Hac-H"	     <- AddKey("[2M+Hac-H]-",       charge = -1, M = 2, adducts = 59.013851);  # 2M + 59.013851	    1-	2.00	59.013851	1765.675631	1693.626149
    neg$"3M-H"	         <- AddKey("[3M-H]-",           charge = -1, M = 3, adducts = -1.007276);   # 3M - 1.007276	    1-	3.00	1.007276	2560.999946	2627.952724

    return(neg);
}

init_calc <- function() {

    Calculator <- list();

    Calculator$"+" <- positive();
    Calculator$"-" <- negative(); 

    return(Calculator);
}

### 获取前体离子的电荷极性
### 函数返回+或者-
getPolarity <- function(type) {
    return(substrRight(type, n=1));
}

substrRight <- function(x, n) {
    substr(x, nchar(x)-n+1, nchar(x))
}

Calculator                   <- init_calc();
find.PrecursorType.no_result <- "Unknown";
debug.echo                   <- TRUE;

## @param chargeMode +/-
get.mass <- function(chargeMode, charge, PrecursorType) {

	if (PrecursorType %in% c("[M]+", "[M]-")) {
		return(function(x) x);
	}

	mode <- Calculator[[chargeMode]];
	found <- NULL;
	
	for (name in names(mode)) {
		calc <- mode[[name]];
		if (calc$Name == PrecursorType) {
			found <- calc;
			break;
		}
	}
	
	return(found$calc);
}

# 函数返回-1值表示没有找到目标类型的前体离子
get.PrecursorMZ <- function(M, precursorType) {
	mode        <- getPolarity(precursorType);
	mode        <- Calculator[[mode]];
	precursorMZ <- -1;
	
	# calc <- mode[[precursorType]];
	for (name in names(mode)) {
	
		calc <- mode[[name]];
		
		if (precursorType == calc$Name) {
			precursorMZ   <- calc$cal.mz(M);
			break;
		}
	}
		
	precursorMZ;
}

## 通过计算出最小的质量差来获取前体离子的类型信息
## @param charge 离子的电荷数量
## @param mass 分子质量
## @param precursorMZ MS/MS之中的MS1匹配上的前体离子质核比
## 
## 计算的公式为：
## 
## (mass/charge + precursor_ion_mass) = precursorMZ
##
## 算法的原理是使用数据库之中的质量分别遍历当前计算器内的前体离子的质量的和除以电荷数量
## 得到的结果与MS1的质核比结果进行比较
## 得到的最小的差值所对应的前体离子即为我们所需要查找的离子化模式
##
## test
## 
## mass = 853.33089
##
## pos "[M+3Na]3+" charge = 3,  307.432848	find.PrecursorType(853.33089, 307.432848,  charge = 3)
## pos "[2M+K]+"   charge = 1,  1745.624938	find.PrecursorType(853.33089, 1745.624938, charge = 1)
## pos "[M+H]+"    charge = 1,  854.338166	find.PrecursorType(853.33089, 854.338166,  charge = 1)
##
## neg "[M-3H]3-"  charge = -3, 283.436354	find.PrecursorType(853.33089, 283.436354,  charge = -3, chargeMode = "-")
## neg "[3M-H]-"   charge = -1, 2560.999946	find.PrecursorType(853.33089, 2560.999946, charge = -1, chargeMode = "-")
## neg "[M-H]-"    charge = -1, 852.323614  find.PrecursorType(853.33089, 852.323614,  charge = -1, chargeMode = "-")
##
find.PrecursorType <- function(mass, precursorMZ, charge, chargeMode = "+", minError.ppm = 100, debug.echo = TRUE) {

	if (charge == 0) {
		print("I can't calculate the ionization mode for no charge(charge = 0)!")
	}

	if (is.null(mass) || is.na(mass) || is.null(precursorMZ) || is.na(precursorMZ)) {
		if(is.null(mass)) {
			mass = NA;
		}
		if(is.null(precursorMZ)) {
			precursorMZ = NA;
		}
		print(sprintf("  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!", mass, precursorMZ));
        return(find.PrecursorType.no_result);
    }

	if (calc.PPM(precursorMZ, mass / abs(charge)) <= 500) {
		# 本身的分子质量和前体的mz一样，说明为[M]类型
		if(abs(charge) == 1) {
			return(sprintf("[M]%s", chargeMode));
		} else {
			return(sprintf("[M]%s%s", charge, chargeMode));
		}
	}

    ### 每一个模式都计算一遍，然后返回最小的ppm差值结果
    min     <- 999999;
    minType <- NULL;

    ## 得到某一个离子模式下的计算程序
    mode    <- Calculator[[chargeMode]];
	
	if (chargeMode == "-") {
		## 对于负离子模式而言，虽然电荷量是负数的，但是使用xcms解析出来的却是一个电荷数的绝对值
		## 所以需要判断一次，乘以-1 
		if (charge > 0) {
			charge = -1 * charge;
		}
	}
	
    ## 然后遍历这个模式下的所有离子前体计算
    for (i in 1:length(mode)) {

        calc  <- mode[[i]];
        ptype <- calc$Name;

        ## 跳过电荷数不匹配的离子模式计算表达式
        if (charge != calc$charge) {
            next;
        } else {
            calc <- calc$calc;
        }	
		
        ## 这里实际上是根据数据库之中的分子质量，通过前体离子的质量计算出mz结果
        ## 然后计算mz计算结果和precursorMZ的ppm信息
        mass.reverse <- calc(precursorMZ);
        delta.ppm    <- calc.PPM(mass.reverse, actualValue = mass);

        if(debug.echo) {
            print(sprintf("%s - %s = %s(ppm), type=%s", mass, mass.reverse, delta.ppm, ptype));
        }

        ## 根据质量计算出前体质量，然后计算出差值
        if (delta.ppm < min) {
            min     <- delta.ppm;
            minType <- ptype;
        }
    }

    ## 假若这个最小的ppm差值符合误差范围，则认为找到了一个前体模式
    if (debug.echo) {
        print(sprintf("  ==> %s", minType));
    }
    if (min <= minError.ppm) {
        return(minType);
    } else {
		
		if (debug.echo) {
			print(sprintf("But the '%s' ionization mode its ppm error (%s ppm) is ", minType, min));
			print(sprintf("not satisfy the minError requirement(%s), returns Unknown!", minError.ppm));
		}	
        return(find.PrecursorType.no_result);
    }
}
