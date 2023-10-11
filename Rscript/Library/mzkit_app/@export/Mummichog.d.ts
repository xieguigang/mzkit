// export R# package module type define for javascript/typescript language
//
//    imports "Mummichog" from "mzkit";
//
// ref=mzkit.Mummichog@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Mummichog searches for enrichment patterns on metabolic network, 
 *  bypassing metabolite identification, to generate high-quality
 *  hypotheses directly from a LC-MS data table.
 * 
*/
declare namespace Mummichog {
   /**
    * Extract all candidates unique id from the given query result
    * 
    * 
     * @param env 
     * + default value Is ``null``.
   */
   function candidates_Id(q: any, env?: object): string;
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
     * @param min_size 
     * + default value Is ``3``.
   */
   function fromGseaBackground(background: object, min_size?: object): object;
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
    * ### do ms1 peaks annotation
    *  
    *  Do ms1 peak list annotation based on the given biological context information
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
     * @param ga 
     * + default value Is ``false``.
     * @param pop_size 
     * + default value Is ``100``.
     * @param mutation_rate 
     * + default value Is ``0.3``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakList_annotation(background: object, candidates: object, minhit?: object, permutation?: object, modelSize?: object, pinned?: string, ignore_topology?: boolean, ga?: boolean, pop_size?: object, mutation_rate?: number, env?: object): object;
   /**
    * Matches all of the annotation hits candidates from a given of the mass peak list
    * 
    * 
     * @param mz A numeric vector, the given mass peak list for run candidate search.
     * @param msData the @``T:BioNovoGene.BioDeep.MSEngine.IMzQuery`` annotation engine, should has the 
     *  interface function for query annotation candidates by the
     *  given m/z mass value.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A set of the metabolite ion m/z query candidates result
   */
   function queryCandidateSet(mz: number, msData: any, env?: object): object;
}
