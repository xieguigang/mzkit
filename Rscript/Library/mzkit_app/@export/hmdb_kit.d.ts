// export R# package module type define for javascript/typescript language
//
//    imports "hmdb_kit" from "mzkit";
//
// ref=mzkit.HMDBTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for handling of the hmdb database
 *  
 *  The Human Metabolome Database (HMDB) is a comprehensive, high-quality, freely accessible, 
 *  online database of small molecule metabolites found in the human body. It bas been created 
 *  by the Human Metabolome Project funded by Genome Canada and is one of the first dedicated
 *  metabolomics databases. The HMDB facilitates human metabolomics research, including the 
 *  identification and characterization of human metabolites using NMR spectroscopy, GC-MS
 *  spectrometry and LC/MS spectrometry. To aid in this discovery process, the HMDB contains 
 *  three kinds of data: 1) chemical data, 2) clinical data, and 3) molecular biology/biochemistry 
 *  data. The chemical data includes 41,514 metabolite structures with detailed descriptions 
 *  along with nearly 10,000 NMR, GC-MS and LC/MS spectra.
 * 
 *  The clinical data includes information On >10,000 metabolite-biofluid concentrations And 
 *  metabolite concentration information On more than 600 different human diseases. The biochemical 
 *  data includes 5,688 protein (And DNA) sequences And more than 5,000 biochemical reactions that 
 *  are linked To these metabolite entries. Each metabolite entry In the HMDB contains more than 110 
 *  data fields With 2/3 Of the information being devoted To chemical/clinical data And the other 
 *  1/3 devoted To enzymatic Or biochemical data. Many data fields are hyperlinked To other 
 *  databases (KEGG, MetaCyc, PubChem, Protein Data Bank, ChEBI, Swiss-Prot, And GenBank) And a 
 *  variety Of Structure And pathway viewing applets. The HMDB database supports extensive text, 
 *  sequence, spectral, chemical Structure And relational query searches. It has been widely used
 *  In metabolomics, clinical chemistry, biomarker discovery And general biochemistry education.
 * 
 *  Four additional databases, DrugBank, T3DB, SMPDB And FooDB are also part Of the HMDB suite Of 
 *  databases. DrugBank contains equivalent information On ~1,600 drug And drug metabolites, T3DB 
 *  contains information On 3,100 common toxins And environmental pollutants, SMPDB contains pathway 
 *  diagrams For 700 human metabolic And disease pathways, While FooDB contains equivalent information 
 *  On ~28,000 food components And food additives.
 * 
*/
declare namespace hmdb_kit {
   /**
    * split the hmdb database by biospecimen locations
    * 
    * 
     * @param hmdb -
     * @param locations -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function biospecimen_slicer(hmdb: object, locations: object, env?: object): object;
   /**
    * Extract the chemical taxonomy data
    * 
    * 
     * @param metabolite the HMDB metabolite data
     * @return A character vector that contains the @``T:BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.taxonomy`` information from the @``T:BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.metabolite``
   */
   function chemical_taxonomy(metabolite: object): string;
   module export {
      /**
       * save the hmdb database as a csv table file
       * 
       * 
        * @param hmdb A collection of the HMDB @``T:BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.metabolite``.
        * @param file this function will returns a huge metabolite table
        *  if this parameter value default null
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function hmdb_table(hmdb: object, file?: any, env?: object): any;
   }
   /**
    * get metabolite via a given hmdb id from the hmdb.ca online web services
    * 
    * 
     * @param id the given hmdb id
     * @param cache_dir 
     * + default value Is ``'./hmdb/'``.
     * @param tabular 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function get_hmdb(id: string, cache_dir?: string, tabular?: boolean, env?: object): object|object;
   module read {
      /**
       * open a reader for read hmdb database
       * 
       * 
        * @param xml the file path of the hmdb metabolite database xml file
        * @return this function populate a collection of the hmdb metabolites data
      */
      function hmdb(xml: string): object;
      /**
       * read hmdb spectral data collection
       * 
       * 
        * @param repo A directory path to the hmdb spectral data files
        * @param hmdbRaw -
        * 
        * + default value Is ``false``.
        * @param lazy -
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function hmdb_spectrals(repo: string, hmdbRaw?: boolean, lazy?: boolean, env?: object): object|object;
   }
}
