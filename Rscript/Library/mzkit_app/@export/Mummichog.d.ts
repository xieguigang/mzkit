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
     * @return A tuple list object that contains elements inside each slot data:
     *  
     *  1. name: the pathway map id
     *  2. desc: the pathway map names
     *  3. model: the network graph object that will be used as the model for run enrichment
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
     * @param params 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function kegg_background(metabolites: any, pathways: any, params?: object, env?: object): object;
   /**
    * create kegg pathway network graph background model
    * 
    * 
     * @param maps A collection of the kegg @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map`` clr object
     * @param reactions A collection of the kegg @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction`` clr object
     * @param alternative 
     * + default value Is ``false``.
   */
   function kegg_graph(maps: object, reactions: object, alternative?: boolean): object;
   /**
    * export of the annotation score result table
    * 
    * 
     * @param result the annotation result which is generated from the 
     *  ``peakList_annotation`` function.
     * @param args 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function mzScore(result: object, args?: object, env?: object): object;
   /**
    * ### do ms1 peaks annotation
    *  
    *  Do ms1 peak list annotation based on the given biological context information
    * 
    * 
     * @param background the enrichment and network topology graph mode list
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakList_annotation(background: object, peaks: any, sampleinfo: any, env?: object): object;
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
