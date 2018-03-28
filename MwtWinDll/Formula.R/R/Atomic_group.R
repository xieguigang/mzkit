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
        CH3 = list(atoms = list(C = 1, H = 3), charge = -1)
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