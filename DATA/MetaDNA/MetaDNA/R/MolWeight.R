MolWeight <- function() {

	ionSymbols <- c("+", "-");
	weights    <- list(
		"H"       = 1.007276,
        "Na"      = 22.98976928,
        "NH4"     = 18.035534,
        "K"       = 39.0983,
        "H2O"     = 18.01471,
        "ACN"     = 41.04746,  # Acetonitrile (CH3CN)
        "CH3OH"   = 32.03773,
        "DMSO"    = 78.12089,  # dimethyl sulfoxide (CH3)2SO 
        "IsoProp" = 60.058064, # Unknown
        "Cl"      = 35.446,
        "FA"      = 46.00548,  # Unknown
        "Hac"     = 60.04636,  # AceticAcid (CH3COOH)
        "Br"      = 79.901,
        "TFA"     = 113.9929   # Unknown
	);
	
	.Weight <- function(symbol) {
		w <- weights[[symbol]];
		
		if (!IsNothing(w)) {
			w;
		} else {
			msg <- sprintf("Symbol '%s' is not exists in table!", symbol);
			warning(msg);
			-1;
		}
	}

	.Eval <- function(formula) {		
		if ((Strings.Split(formula, ""))[1] %in% ionSymbols) {
			formula <- sprintf("0H%s", formula);
		}
		
		mt      <- strsplit(formula, "[+-]")[[1]];
		op      <- grep("[+-]", formula, perl = TRUE, value = FALSE);
		x       <- 0;
		next_op <- "+";
		
		for(i in 1:length(mt)) {
			token <- Mul(mt[i]);
			M     <- token[["M"]];
			token <- token[["name"]];
			w     <- .Weight(token);
			
			if (w <= 0) {
				msg <- "Unknown symbol in: '%s', where symbol=%s";
				msg <- sprintf(msg, formula, token);
                stop(msg);
			}
			
			if (next_op == "+") {
				x <- x + (M * w);
			} else {
				x <- x - (M * w); 
			}
			
			if (!IsNothing(op) && (op != FALSE) && (i <= length(op))) {
				next_op <- op[i];
			}
		}
		
		x;
	}

	Mul <- function(token) {
		n   <- c();
		len <- Strings.Len(token);
		x0  <- utf8ToInt("09");
		x9  <- x0[2];
		x0  <- x0[1];
		xt  <- utf8ToInt(token);
		
		for (i in 1:len) {
			x <- xt[i];
			
			if (x >= x0 && x <= x9) {
				n <- append(n, xt[i]);
			} else {
				break;
			}
		}
		
		if (length(n) == 0) {
			name <- token;
			M    <- 1;
		} else {
			# substr(x, start, stop)
			token <- substr(token, length(n), length(token));
			name  <- token;
			M     <- Strings.Join(n, "") %=>% as.numeric;
		}
		
		list(name = name, M = M);
	}

	list(Weight = .Weight, 
		 Eval   = .Eval
	);
}