#Region "Microsoft.ROpen::b425cc1a1ce53d6d779d4d2d24342cb4, Utils.R"

    # Summaries:

    # Delete.EmptyKEGG <- function(dataframe, col.name = "KEGG") {...
    # tolerance.deltaMass <- function(da = 0.3) {...
    # tolerance.ppm <- function(ppm = 20) {...
    # PPM <- function(measured, actualValue) {...

#End Region

#' Removes empty \code{KEGG} row
#'
#' @description Removes all of the rows in a given \code{dataframe}
#'      which its \code{KEGG} column value is string empty.
#'
#' @param d A \code{dataframe} object which contains the metabolite
#'      annotation result data. This dataframe must contains a column
#'      which is named \code{KEGG}.
#'
#' @return A subset of the input \code{dataframe} with all KEGG column
#'     value non-empty.
Delete.EmptyKEGG <- function(dataframe, col.name = "KEGG") {
    KEGG <- dataframe[, col.name] %=>% as.vector;
	test <- !Strings.Empty(KEGG, TRUE);

    dataframe[test, ];
}

#' Tolerance in Mass delta mode
#'
#' @param da The mass delta value. By default if two mass value which
#'           their delta value is less than \code{0.3da}, then
#'           the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
#'
tolerance.deltaMass <- function(da = 0.3) {
    describ <- sprintf("%s(da)", da);

    function(a, b) {
        err  <- abs(a - b);
        test <- err <= da;

        list(error     = err,
             valid     = test,
             describ   = describ,
             threshold = da
        );
    }
}

#' Tolerance in PPM mode
#'
#' @param ppm The mass ppm value. By default if two mass value which
#'            their ppm delta value is less than \code{20ppm}, then
#'            the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
#'
tolerance.ppm <- function(ppm = 20) {
    describ <- sprintf("%s(ppm)", ppm);

    function(a, b) {
        err  <- PPM(a, b);
        test <- err <= ppm;

        list(error     = err,
             valid     = test,
             describ   = describ,
             threshold = ppm
        );
    }
}

assert.deltaMass <- function(da = 0.3) {
    function(a, b) abs(a-b) <= da;
}

assert.ppm <- function(ppm = 20) {
    function(a, b) PPM(a, b) <= ppm;
}

#' PPM value between two mass value
#'
PPM <- function(measured, actualValue) {
	# 2018-7-8 without abs function for entir value, this may cause bugs in metaDNA
	# for unknown query when actualValue is negative

    # |(measure - reference)| / measure * 1000000
    abs(((measured - actualValue) / actualValue) * 1000000);
}


#' Get ionlization mode
#'
#' @description Get ionlization mode from a given precursor type name
#'
#' @param type The precursor type name, it should be in format like: \code{[M+H]+}.
#'
#' @return Function returns character \code{+} or \code{-}.
getPolarity <- function(type) {
    return(substr.Right(type, n=1));
}

#' Get mass calculator
#'
#' @param chargeMode Character value of \code{+/-}.
#' @param PrecursorType The precursor type full name or brief name.
#'
#' @return Returns a function for calculate mass from \code{m/z} value.
#'
get.mass <- function(chargeMode, PrecursorType) {
    if (PrecursorType %in% c("[M]+", "[M]-")) {
        return(function(x) x);
    }

    mode <- Calculator[[chargeMode]];
    found <- mode[[PrecursorType]];

    if (found %=>% IsNothing) {
        # Is the precursor type full name.
        for (name in names(mode)) {
            calc <- mode[[name]];
            if (calc$Name == PrecursorType) {
                found <- calc;
                break;
            }
        }
    }

    found$calc;
}

#' Calculate m/z
#'
#' @description Calculate \code{m/z} for mass by given precursor type
#'
#' @param M Molecule mass
#' @param precursorType Charge mode was parsed from this parameter string, 
#'      it is convenient, but inefficient when in batch mode.
#' 
#' @return -1 means target precursor type is not found.
#'
get.PrecursorMZ.Auto <- function(M, precursorType) {
    get.PrecursorMZ(M, precursorType, mode = getPolarity(precursorType));
}

#' Calculate m/z with given charge mode
#'
#' @param mode The ion charge mode, if the \code{precursorType} is multiple length,
#'       then this mode parameter should be the same length as the \code{precursorType}.
#'
#' @return If \code{precursorType} just have one element, then this function will 
#'      returns a numeric vector. 
#'      If this parameter \code{precursorType} contains multiple type names, then this 
#'      function will returns a list object with member name is the \code{precursorType}
#'      member value is the corresponding \code{m/z} vector
get.PrecursorMZ <- function(M, precursorType, mode) {
	mzVector <- function(type, mode) {
		types <- Calculator[[mode]];
		loop <- lapply(types, function(calc) {
			if (type == calc$Name) {
				calc$cal.mz(M);				
			} else {
				NULL;
			}
		});
		loop <- loop[!sapply(loop, is.null)];
		
		if (length(loop) == 0) {
			rep(1, length(M));
		} else {
			loop[[1]];
		}
	}
	notFound.warn <- function(mz, precursorType) {
		if ((mz == -1) %=>% all) {
			warnMsg <- "\"%s\" is not found... Precursor m/z is set to -1.";
			warnMsg <- sprintf(warnMsg, precursorType);
			warning(warnMsg);			
		}
		
		invisible(NULL);
	}
	
	if (length(precursorType) == 1) {
		precursorMZ <- mzVector(precursorType, mode);
		#' test and warn
		notFound.warn(precursorMZ, precursorType);	
	} else {
		precursorMZ <- lapply(1:length(precursorType), function(i) {
			mzVector(precursorType[i], mode[i]);
		});
		names(precursorMZ) <- precursorType;
		
		lapply(names(precursorMZ), function(type) {
			#' test and warn
			notFound.warn(precursorMZ[[type]], type);	
		});
	}

    precursorMZ;
}