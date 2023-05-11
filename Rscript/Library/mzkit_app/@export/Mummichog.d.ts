// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Mummichog@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace Mummichog {
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param env default value Is ``null``.
   */
   function createMzset(query: object, tolerance?: any, env?: object): object;
   /**
    * cast background models for ``peakList_annotation`` analysis based on
    *  a given gsea background model object, this conversion will loose
    *  all of the network topology information
    * 
    * 
     * @param background -
   */
   function fromGseaBackground(background: object): object;
   /**
     * @param adducts default value Is ``["[M]+","[M+H]+","[M+H2O]+","[M+H-H2O]+"]``.
     * @param isotopic_max default value Is ``5``.
     * @param mzdiff default value Is ``0.01``.
     * @param delta_rt default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function group_peaks(peaktable: any, adducts?: any, isotopic_max?: object, mzdiff?: number, delta_rt?: number, env?: object): any;
   /**
    * create kegg pathway network graph background model
    * 
    * 
     * @param maps -
     * @param reactions -
     * @param alternative 
     * + default value Is ``false``.
   */
   function kegg_background(maps: object, reactions: object, alternative?: boolean): object;
   /**
    * export of the annotation score result table
    * 
    * 
     * @param result the annotation result which is generated from the 
     *  ``peakList_annotation`` function.
     * @param minHits 
     * + default value Is ``-1``.
     * @param ignore_topology 
     * + default value Is ``false``.
   */
   function mzScore(result: object, minHits?: object, ignore_topology?: boolean): object;
   /**
    * do ms1 peaks annotation
    * 
    * 
     * @param background the enrichment and network topology graph mode list
     * @param candidates a set of m/z search result list based on the given background model
     * @param minhit -
     * 
     * + default value Is ``3``.
     * @param permutation -
     * 
     * + default value Is ``100``.
     * @param modelSize -
     * 
     * + default value Is ``-1``.
     * @param pinned 
     * + default value Is ``null``.
     * @param ignore_topology 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakList_annotation(background: object, candidates: object, minhit?: object, permutation?: object, modelSize?: object, pinned?: string, ignore_topology?: boolean, env?: object): object;
   /**
    * 
    * 
     * @param mz -
     * @param msData the @``T:BioNovoGene.BioDeep.MSEngine.IMzQuery`` annotation engine
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function queryCandidateSet(mz: number, msData: any, env?: object): object;
}
