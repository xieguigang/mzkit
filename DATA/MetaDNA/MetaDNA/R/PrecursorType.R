#Region "Microsoft.ROpen::0f5ca9243ed6f3828d004ae68b985b55, PrecursorType.R"

    # Summaries:

    # PrecursorType <- function() {...
    # adduct.mz <- function(mass, adduct, charge) {...
    # adduct.mz.general <- function(mass, adduct, charge) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # reverse.mass <- function(precursorMZ, M, charge, adduct) {...
    # reverse.mass.general <- function(precursorMZ, M, charge, adduct) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # .addKey <- function(type, charge, M, adducts) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # positive <- function() {...
    # negative <- function() {...

#End Region

# https://github.com/xieguigang/MassSpectrum-toolkits/blob/6f4284a0d537d86c112877243d9e3b8d9d35563f/DATA/ms2_math-core/Ms1/PrecursorType.vb

#' The precursor type data model
#'
#' @details This helper function returns a list, with members:
#'    \enumerate{
#'       \item \code{mz} Calculate mass \code{m/z} value with given adduct and charge values.
#'       \item \code{mass} Calculate mass value from given \code{m/z} with given adduct and charge, etc.
#'       \item \code{new} Create a new mass and \code{m/z} calculator from given adduct info
#'    }
#'
PrecursorType <- function() {

    #' Evaluate adducts text to molecular weight.
    .eval <- Eval(MolWeight)$Eval;

	#' Calculate m/z
	#'
	#' @param mass Molecule weight
	#' @param adduct adduct mass
	#' @param charge precursor charge value
	#'
	#' @return Returns the m/z value of the precursor ion
	adduct.mz <- function(mass, adduct, charge) {
		(mass + adduct) / abs(charge);
	}
	adduct.mz.general <- function(mass, adduct, charge) {
	    # Evaluate the formula expression to weights
	    if (!is.numeric(adducts)) {
	        adducts <- .eval(adducts);
	    }

	    adduct.mz(mass, adduct, charge);
	}

	#' Calculate mass from m/z
	#'
	#' @description Calculate the molecule mass from precursor adduct ion m/z
	#'
	#' @param precursorMZ MS/MS precursor adduct ion m/z
	#' @param charge Net charge of the ion
	#' @param adduct Adduct mass
	#' @param M The number of the molecule for formula a precursor adduct ion.
	#'
	#' @return The molecule mass.
	reverse.mass <- function(precursorMZ, M, charge, adduct) {
		(precursorMZ * abs(charge) - adduct) / M;
	}
	reverse.mass.general <- function(precursorMZ, M, charge, adduct) {
	    # Evaluate the formula expression to weights
	    if (!is.numeric(adducts)) {
	        adducts <- .eval(adducts);
	    }

	    reverse.mass(precursorMZ, M, charge, adduct);
	}

	#' Construct a \code{precursor_type} model
	#'
	#' @param charge The ion charge value, no sign required.
	#' @param type Full name of the precursor type
	#' @param M The number of the target molecule
	#' @param adducts The precursor adducts formula expression
	#'
	.addKey <- function(type, charge, M, adducts) {
		# Evaluate the formula expression to weights
		if (!is.numeric(adducts)) {
			adducts <- .eval(adducts);
		}

		list(Name   = type,
			 calc   = function(precursorMZ) reverse.mass(precursorMZ, M, charge, adducts),
			 charge = charge,
			 M      = M,
			 adduct = adducts,
			 cal.mz = function(mass) adduct.mz(mass * M, adducts, charge)
		);
	}

	list(mz   = adduct.mz.general,
		 mass = reverse.mass.general,
		 new  = .addKey
	);
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

#' Precursor types in positive mode
#'
positive <- function() {
    pos <- list();

    # AddKey <- function(type, charge, M, adducts)
	.addKey <- Eval(PrecursorType)$new;

    pos$"M+3H"	         <- .addKey("[M+3H]3+",          charge = 3, M = 1, adducts = "M+3H"          ); # M/3 + 1.007276	    3+	0.33	1.007276	285.450906	291.099391
    pos$"M+2H+Na"	     <- .addKey("[M+2H+Na]3+",       charge = 3, M = 1, adducts = "M+2H+Na"       ); # M/3 + 8.334590	    3+	0.33	8.334590	292.778220	283.772077
    pos$"M+H+2Na"	     <- .addKey("[M+H+2Na]3+",       charge = 3, M = 1, adducts = "M+H+2Na"       ); # M/3 + 15.7661904	    3+	0.33	15.766190	300.209820	276.340476
    pos$"M+3Na"	         <- .addKey("[M+3Na]3+",         charge = 3, M = 1, adducts = "M+3Na"	      ); # M/3 + 22.989218	    3+	0.33	22.989218	307.432848	269.117449
    pos$"M+2H"	         <- .addKey("[M+2H]2+",          charge = 2, M = 1, adducts = "M+2H"          ); # M/2 + 1.007276	    2+	0.50	1.007276	427.672721	437.152724
    pos$"M+H+NH4"	     <- .addKey("[M+H+NH4]2+",       charge = 2, M = 1, adducts = "M+H+NH4"       ); # M/2 + 9.520550	    2+	0.50	9.520550	436.185995	428.639450
    pos$"M+H+Na"	     <- .addKey("[M+H+Na]2+",        charge = 2, M = 1, adducts = "M+H+Na"        ); # M/2 + 11.998247	    2+	0.50	11.998247	438.663692	426.161753
    pos$"M+H+K"	         <- .addKey("[M+H+K]2+",         charge = 2, M = 1, adducts = "M+H+K"         ); # M/2 + 19.985217	    2+	0.50	19.985217	446.650662	418.174783
    pos$"M+ACN+2H"	     <- .addKey("[M+ACN+2H]2+",      charge = 2, M = 1, adducts = "M+ACN+2H"      ); # M/2 + 21.520550	    2+	0.50	21.520550	448.185995	416.639450
    pos$"M+2Na"	         <- .addKey("[M+2Na]2+",         charge = 2, M = 1, adducts = "M+2Na"         ); # M/2 + 22.989218	    2+	0.50	22.989218	449.654663	415.170782
    pos$"M+2ACN+2H"	     <- .addKey("[M+2ACN+2H]2+",     charge = 2, M = 1, adducts = "M+2ACN+2H"     ); # M/2 + 42.033823	    2+	0.50	42.033823	468.699268	396.126177
    pos$"M+3ACN+2H"	     <- .addKey("[M+3ACN+2H]2+",     charge = 2, M = 1, adducts = "M+3ACN+2H"     ); # M/2 + 62.547097	    2+	0.50	62.547097	489.212542	375.612903
    pos$"M+H"	         <- .addKey("[M+H]+",            charge = 1, M = 1, adducts = "M+H"           ); #  M + 1.007276	    1+	1.00	1.007276	854.338166	875.312724
    pos$"M+NH4"	         <- .addKey("[M+NH4]+",          charge = 1, M = 1, adducts = "M+NH4"	      ); #  M + 18.033823	    1+	1.00	18.033823	871.364713	858.286177
    pos$"M+Na"	         <- .addKey("[M+Na]+",           charge = 1, M = 1, adducts = "M+Na"          ); #  M + 22.989218	    1+	1.00	22.989218	876.320108	853.330782
    pos$"M+CH3OH+H"	     <- .addKey("[M+CH3OH+H]+",      charge = 1, M = 1, adducts = "M+CH3OH+H"     ); #  M + 33.033489	    1+	1.00	33.033489	886.364379	843.286511
    pos$"M+K"	         <- .addKey("[M+K]+",            charge = 1, M = 1, adducts = "M+K"           ); #  M + 38.963158	    1+	1.00	38.963158	892.294048	837.356842
    pos$"M+ACN+H"	     <- .addKey("[M+ACN+H]+",        charge = 1, M = 1, adducts = "M+ACN+H"       ); #  M + 42.033823	    1+	1.00	42.033823	895.364713	834.286177
    pos$"M+2Na-H"	     <- .addKey("[M+2Na-H]+",        charge = 1, M = 1, adducts = "M+2Na-H"       ); #  M + 44.971160	    1+	1.00	44.971160	898.302050	831.348840
    pos$"M+IsoProp+H"	 <- .addKey("[M+IsoProp+H]+",    charge = 1, M = 1, adducts = "M+IsoProp+H"   ); #  M + 61.06534	    1+	1.00	61.065340	914.396230	815.254660
    pos$"M+ACN+Na"	     <- .addKey("[M+ACN+Na]+",       charge = 1, M = 1, adducts = "M+ACN+Na"      ); #  M + 64.015765	    1+	1.00	64.015765	917.346655	812.304235
    pos$"M+2K-H"	     <- .addKey("[M+2K-H]+",         charge = 1, M = 1, adducts = "M+2K-H"        ); #  M + 76.919040	    1+	1.00	76.919040	930.249930	799.400960
    pos$"M+DMSO+H"	     <- .addKey("[M+DMSO+H]+",       charge = 1, M = 1, adducts = "M+DMSO+H"      ); #  M + 79.02122	    1+	1.00	79.021220	932.352110	797.298780
    pos$"M+2ACN+H"	     <- .addKey("[M+2ACN+H]+",       charge = 1, M = 1, adducts = "M+2ACN+H"      ); #  M + 83.060370	    1+	1.00	83.060370	936.391260	793.259630
    pos$"M+IsoProp+Na+H" <- .addKey("[M+IsoProp+Na+H]+", charge = 1, M = 1, adducts = "M+IsoProp+Na+H"); # 	M + 84.05511	    1+	1.00	84.055110	937.386000	792.264890
    pos$"2M+H"	         <- .addKey("[2M+H]+",           charge = 1, M = 2, adducts = "2M+H"          ); # 2M + 1.007276	    1+	2.00	1.007276	1707.669056	1751.632724
    pos$"2M+NH4"	     <- .addKey("[2M+NH4]+",         charge = 1, M = 2, adducts = "2M+NH4"        ); # 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
    pos$"2M+Na"	         <- .addKey("[2M+Na]+",          charge = 1, M = 2, adducts = "2M+Na"         ); # 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
    pos$"2M+K"	         <- .addKey("[2M+K]+",           charge = 1, M = 2, adducts = "2M+K"          ); # 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
    pos$"2M+ACN+H"	     <- .addKey("[2M+ACN+H]+",       charge = 1, M = 2, adducts = "2M+ACN+H"      ); # 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
    pos$"2M+ACN+Na"	     <- .addKey("[2M+ACN+Na]+",      charge = 1, M = 2, adducts = "2M+ACN+Na"     ); # 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
	pos$"M"              <- .addKey("[M]+",              charge = 1, M = 1, adducts = 0);
	
    pos;
}

#' Precursor types in negative mode
#'
negative <- function() {
    neg     <- list();

    # AddKey <- function(type, charge, M, adducts)
	.addKey <- Eval(PrecursorType)$new;

    neg$"M-3H"	         <- .addKey("[M-3H]3-",          charge = -3, M = 1, adducts = "M-3H"    ); # M/3 -   1.007276	 3-	0.33	-1.007276	283.436354	293.113943
    neg$"M-2H"	         <- .addKey("[M-2H]2-",          charge = -2, M = 1, adducts = "M-2H"    ); # M/2 -   1.007276	 2-	0.50	-1.007276	425.658169	439.167276
    neg$"M-H2O-H"	     <- .addKey("[M-H2O-H]-",        charge = -1, M = 1, adducts = "M-H2O-H" ); # M   -  19.01839	 1-	1.00	-19.01839	834.312500	895.338390
    neg$"M-H"	         <- .addKey("[M-H]-",            charge = -1, M = 1, adducts = "M-H"     ); # M   -   1.007276	 1-	1.00	-1.007276	852.323614	877.327276
    neg$"M+Na-2H"	     <- .addKey("[M+Na-2H]-",        charge = -1, M = 1, adducts = "M+Na-2H" ); # M   +  20.974666	 1-	1.00	20.974666	874.305556	855.345334
    neg$"M+Cl"	         <- .addKey("[M+Cl]-",           charge = -1, M = 1, adducts = "M+Cl"    ); # M   +  34.969402	 1-	1.00	34.969402	888.300292	841.350598
    neg$"M+K-2H"	     <- .addKey("[M+K-2H]-",         charge = -1, M = 1, adducts = "M+K-2H"  ); # M   +  36.948606	 1-	1.00	36.948606	890.279496	839.371394
    neg$"M+FA-H"	     <- .addKey("[M+FA-H]-",         charge = -1, M = 1, adducts = "M+FA-H"  ); # M   +  44.998201	 1-	1.00	44.998201	898.329091	831.321799
    neg$"M+Hac-H"	     <- .addKey("[M+Hac-H]-",        charge = -1, M = 1, adducts = "M+Hac-H" ); # M   +  59.013851	 1-	1.00	59.013851	912.344741	817.306149
    neg$"M+Br"	         <- .addKey("[M+Br]-",           charge = -1, M = 1, adducts = "M+Br"    ); # M   +  78.918885	 1-	1.00	78.918885	932.249775	797.401115
    neg$"M+TFA-H"	     <- .addKey("[M+TFA-H]-",        charge = -1, M = 1, adducts = "M+TFA-H" ); # M   + 112.985586	 1-	1.00	112.985586	966.316476	763.334414
    neg$"2M-H"	         <- .addKey("[2M-H]-",           charge = -1, M = 2, adducts = "2M-H"    ); # 2M  -   1.007276	 1-	2.00	-1.007276	1705.654504	1753.647276
    neg$"2M+FA-H"	     <- .addKey("[2M+FA-H]-",        charge = -1, M = 2, adducts = "2M+FA-H" ); # 2M  +  44.998201	 1-	2.00	44.998201	1751.659981	1707.641799
    neg$"2M+Hac-H"	     <- .addKey("[2M+Hac-H]-",       charge = -1, M = 2, adducts = "2M+Hac-H"); # 2M  +  59.013851	 1-	2.00	59.013851	1765.675631	1693.626149
    neg$"3M-H"	         <- .addKey("[3M-H]-",           charge = -1, M = 3, adducts = "3M-H"    ); # 3M  -   1.007276	 1-	3.00	1.007276	2560.999946	2627.952724
	neg$"M"              <- .addKey("[M]-",              charge = -1, M = 1, adducts = 0);
	
    neg;
}
