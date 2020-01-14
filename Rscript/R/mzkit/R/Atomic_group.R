#Region "Microsoft.ROpen::8b4c4b2e5cd2f1956e11d18e6e7105f0, Atomic_group.R"

    # Summaries:

    # Atomic.group <- function() {...
    # Atomic.Weights <- function() {...
    # Atomic.group.Weight <- function(Atomic.group, name) {...

#End Region

# Hello, world!
#
# This is an example function named 'hello' 
# which prints 'Hello, world!'.
#
# You can learn more about package authoring with RStudio at:
#
#   http://r-pkgs.had.co.nz/
#
# Some useful keyboard shortcuts for package authoring:
#
#   Build and Reload Package:  'Ctrl + Shift + B'
#   Check Package:             'Ctrl + Shift + E'
#   Test Package:              'Ctrl + Shift + T'

# 在这个函数之中尽可能多的枚举出目前已知的所有的原子基团
Atomic.group <- function() {
    group <- list(
         CH3  = list(atoms = list(C  = 1, H = 3), charge = +1, symbol = "CH3+"   , name = ""),
         NH4  = list(atoms = list(N  = 1, H = 4), charge = +1, symbol = "NH4+"   , name = ""),
         OH   = list(atoms = list(O  = 1, H = 1), charge = -1, symbol = "OH-"    , name = ""),
         NO3  = list(atoms = list(N  = 1, O = 3), charge = -1, symbol = "NO3-"   , name = ""),
         PO31 = list(atoms = list(P  = 1, O = 3), charge = -1, symbol = "PO3-"   , name = ""), # 偏磷酸根  P = +5
         PO33 = list(atoms = list(P  = 1, O = 3), charge = -3, symbol = "PO3 3-" , name = ""), # 亚磷酸根  P = +3
        MnO42 = list(atoms = list(Mn = 1, O = 4), charge = -2, symbol = "MnO4 2-", name = ""), # 锰酸根   Mn = +6 
        MnO41 = list(atoms = list(Mn = 1, O = 4), charge = -1, symbol = "MnO4-"  , name = "")  # 高锰酸根 Mn = +7
    );

    group;
}

# 元素周期表
Atomic.Weights <- function() {
    list(
        C = 12, H = 1
    );
}

# 计算出指定的原子基团的分子质量大小
Atomic.group.Weight <- function(Atomic.group, name) {
    Atomic.group <- Atomic.group[[name]];
    atoms        <- Atomic.group[["atoms"]];
    weight       <- Atomic.Weights();
    weight       <- weight[names(atoms)];
    weight       <- weight * atoms;

    weight;
}
