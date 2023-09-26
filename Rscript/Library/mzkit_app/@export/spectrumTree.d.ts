// export R# package module type define for javascript/typescript language
//
//    imports "spectrumTree" from "mzkit";
//
// ref=mzkit.ReferenceTreePkg@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the spectrum tree reference library tools
 * 
 * > the spectrum data is clustering and save in family 
 * >  tree data structure.
*/
declare namespace spectrumTree {
   /**
    * push the reference spectrum data into the spectrum reference tree library
    * 
    * 
     * @param tree The reference spectrum database, which the spectrum data 
     *  is store in family tree style
     * @param x A new spectrum data to push into the reference database
     * @param ignore_error 
     * + default value Is ``false``.
     * @param args additional parameters for create the spectrum library in spectrum pack format:
     *  
     *  1. uuid, BioDeepID, biodeep_id is used for the metabolite unique reference id
     *  2. chemical_formula, formula is used for the metabolite exact mass value
     *  
     *  and the spectrum input of x should be the same metabolite if save data as 
     *  the spectrum pack data.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function addBucket(tree: any, x: any, ignore_error?: boolean, args?: object, env?: object): any;
   /**
    * Compress and make cleanup of the spectrum library
    * 
    * 
     * @param spectrumLib -
     * @param file A file object for write the spectrum library.
     * @param metadb metabolite annotation database library for get annotation information
     * @param nspec 
     * + default value Is ``5``.
     * @param xrefDb 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function compress(spectrumLib: object, file: any, metadb: object, nspec?: object, xrefDb?: string, env?: object): any;
   /**
    * set dot cutoff parameter for the cos score similarity algorithm
    * 
    * 
     * @param search The spectrum library stream engine
     * @param cutoff cutoff threshold value of the cos score
   */
   function dotcutoff(search: object, cutoff: number): object;
   /**
    * Extract the test sample data for run evaluation of the annotation workflow
    * 
    * 
     * @param packlib -
     * @param n -
     * 
     * + default value Is ``30``.
     * @param rtmax 
     * + default value Is ``840``.
     * @param source_name -
     * 
     * + default value Is ``'get_testSample'``.
   */
   function get_testSample(packlib: object, n?: object, rtmax?: number, source_name?: string): object;
   /**
    * construct a fragment set library for run spectrum search in jaccard index matches method
    * 
    * 
     * @param libname -
     * @param mz -
     * @param mzset -
     * @param rt -
     * 
     * + default value Is ``null``.
     * @param cutoff -
     * 
     * + default value Is ``0.1``.
     * @param filter_complex_adducts 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function jaccardSet(libname: string, mz: number, mzset: string, rt?: number, cutoff?: number, filter_complex_adducts?: boolean, env?: object): object;
   /**
    * create new reference spectrum database
    * 
    * 
     * @param file A file path to save the spectrum reference database
     * @param type 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function new(file: any, type?: object, env?: object): object|object|object;
   /**
    * ### open the spectrum reference database
    *  
    *  open the reference spectrum database file and 
    *  then create a host to run spectrum cluster 
    *  search
    * 
    * > the data format is test via the magic header
    * 
     * @param file -
     * @param dotcutoff 
     * + default value Is ``0.6``.
     * @param adducts 
     * + default value Is ``["[M]+","[M+H]+"]``.
     * @param target_uuid a character vector of the target metabolite biodeep_id, default value
     *  is NULL means load all reference spectrum from the required reference 
     *  database file. this function will just load a subset of the reference 
     *  spectrum data from the database file is this parameter value is not 
     *  NULL.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function open(file: any, dotcutoff?: number, adducts?: any, target_uuid?: any, env?: object): object|object;
   /**
    * do spectrum family alignment via cos similarity
    * 
    * 
     * @param tree The reference spectrum tree object to search
     * @param x The query spectrum data from the sample raw data files
     * @param maxdepth The max depth of the tree search
     * 
     * + default value Is ``1024``.
     * @param treeSearch Do alignment in family tree search mode?
     * 
     * + default value Is ``false``.
     * @param top_hits the top n hits of the candidate result populated for the each query input,
     *  set this parameter value to zero or negative value means no limits.
     * 
     * + default value Is ``3``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return function returns nothing means no query hits or the 
     *  given input query sample data **`x`**
   */
   function query(tree: object, x: any, maxdepth?: object, treeSearch?: boolean, top_hits?: object, env?: object): object;
   /**
    * open the spectrum pack reference database file
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function readpack(file: any, env?: object): object;
}
