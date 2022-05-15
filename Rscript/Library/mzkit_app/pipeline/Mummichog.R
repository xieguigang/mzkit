require(mzkit);

[@info "A text file that contains a list of the MS1 
        m/z peaks to run data annotations, the text 
        file format should be multiple lines data, 
        and each line text is the m/z value of the 
        given ions list."]
[@type "*.txt"]
const mzlist as string = ?"--mzlist" || stop("A set of m/z value list must be provided!");
[@info "A result table for export of the m/z peak list 
        data annotation results."]
[@type "*.csv"]
const output as string = ?"--save" || `${dirname(mzlist)}/${basename(mzlist)}_annotations.csv`;
[@info "the min number hits of metabolites in a pathway
        for keeps as the annotation result data."]
const minHits as integer = ?"--minhits" || 3;
[@info "the number of times to run annotation permutation."]
const permutation as integer = ?"--permutation" || 100;