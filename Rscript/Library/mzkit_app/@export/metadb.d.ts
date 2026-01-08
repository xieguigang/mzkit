// export R# package module type define for javascript/typescript language
//
//    imports "metadb" from "mzkit";
//
// ref=mzkit.MetaDbXref@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Metabolite annotation database search engine
 * 
 * > this library module mainly address of the ion m/z database search problem
*/
declare namespace metadb {
   module annotationStream {
      /**
       * Construct a basic metabolite annotation data collection
       * 
       * > the exact mass will be evaluated based on the input **`formula`** data.
       * 
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function compounds(compounds: any, env?: object): object;
   }
   module cbind {
      /**
        * @param env default value Is ``null``.
      */
      function metainfo(anno: object, engine: any, env?: object): any;
   }
   /**
    * removes all of the annotation result which is not 
    *  hits in the given ``id`` set.
    * 
    * 
     * @param query -
     * @param id the required compound id set that should be hit!
     * @param field -
     * @param metadb -
     * @param includes_metal_ions removes metabolite annotation result which has metal
     *  ions inside formula string by default.
     * 
     * + default value Is ``false``.
     * @param excludes reverse the logical of select the annotation result 
     *  based on the given **`id`** set.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function excludeFeatures(query: object, id: string, field: string, metadb: object, includes_metal_ions?: boolean, excludes?: boolean, env?: object): object;
   /**
    * get metabolite annotation metadata by a set of given unique reference id
    * 
    * 
     * @param engine A local annotation repository object that should implements of the @``T:BioNovoGene.BioDeep.MSEngine.IMetaDb`` interface.
     * @param uniqueId a set of the unique reference id
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function getMetadata(engine: any, uniqueId: any, env?: object): any;
   module has {
      /**
       * Check the formula string has metal ion inside?
       * 
       * 
        * @param formula -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function metal_ion(formula: any, env?: object): boolean;
   }
   module is {
      /**
       * check of the given formula is metal ion or not?
       * 
       * 
        * @param formula -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function metal_ion(formula: any, env?: object): boolean;
      /**
       * check of the given formula is organic or not?
       *  this function will return TRUE if the formula is organic,
       *  otherwise it returns FALSE.
       * 
       * 
        * @param formula -
        * @param strict 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function organic(formula: any, strict?: boolean, env?: object): boolean;
   }
   /**
    * cast the given dataframe as the ion feature annotation result
    * 
    * 
     * @param x a dataframe of the ion annotation data that required of the data fields:
     *  
     *  1. unique_id: metabolite reference id
     *  2. name: metabolite name
     *  3. mz: target ion feature m/z value
     *  4. ppm: the ppm error between the sample m/z and evaluated mz valuefrom the exact mass
     *  5. adducts: ion feature adducts type for the annotation
     *  6. score: the ion annotation score for the result.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function load_asQueryHits(x: object, env?: object): object;
   module mass_search {
      /**
       * A general method for build exact mass search index
       * 
       * 
        * @param massSet -
        * @param type the clr type description string of the elements in the given **`massSet`** collection
        * @param tolerance -
        * 
        * + default value Is ``'da:0.01'``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A simple mass index search engine object instance
      */
      function index(massSet: any, type: any, tolerance?: any, env?: object): object;
   }
   /**
    * a generic function for handle ms1 search
    * 
    * 
     * @param compounds kegg compounds
     * @param precursors -
     * @param tolerance -
     * 
     * + default value Is ``'ppm:20'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function ms1_handler(compounds: any, precursors: any, tolerance?: any, env?: object): object;
   /**
    * get duplictaed raw annotation results.
    * 
    * 
     * @param engine the ms1 search engine which implements the clr interface @``T:BioNovoGene.BioDeep.MSEngine.IMzQuery``
     * @param mz a m/z numeric vector or a object list that 
     *  contains the data mapping of unique id to 
     *  m/z value.
     * @param unique 
     * + default value Is ``false``.
     * @param uniqueByScore only works when **`unique`** parameter
     *  value is set to value TRUE.
     * 
     * + default value Is ``false``.
     * @param field_mz the field name of the ion m/z, options for list and dataframe input.
     * 
     * + default value Is ``'mz'``.
     * @param field_score the field name of the ion score, options for list and dataframe input.
     * 
     * + default value Is ``'score'``.
     * @param env 
     * + default value Is ``null``.
   */
   function ms1_search(engine: any, mz: any, unique?: boolean, uniqueByScore?: boolean, field_mz?: string, field_score?: string, env?: object): object;
   /**
    * Parse the lipid names
    * 
    * 
     * @param name a character vector of the lipid names
     * @param keepsRaw keeps the mzkit clr object instead of convert the clr object as R# runtime tuple list value.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parseLipidName(name: any, keepsRaw?: boolean, env?: object): object;
   /**
    * parse the precursor type calculator
    * 
    * 
     * @param ion A precursor type string, example as ``[M+H]``.
   */
   function precursorIon(ion: string): object;
   /**
    * A general interface method for query the exact mass search index
    * 
    * > this function will return a list of the matched results, which it could be empty if no matched results.
    * 
     * @param search the mass search index engine
     * @param mass the target exact mass value
   */
   function queryByMass(search: object, mass: number): any;
   /**
    * Found the best matched mz value with the target **`exactMass`**
    * 
    * 
     * @param mz -
     * @param exactMass -
     * @param adducts -
     * @param mzdiff -
     * 
     * + default value Is ``'da:0.005'``.
     * @param env 
     * + default value Is ``null``.
     * @return function returns a evaluated mz under the specific **`adducts`** value
     *  and it also the min mass tolerance, if no result has mass tolerance less then the 
     *  given threshold value, then this function returns nothing
   */
   function searchMz(mz: any, exactMass: number, adducts: object, mzdiff?: any, env?: object): object;
   /**
    * take the valid cas number from a collection of the given id set
    * 
    * 
     * @param x the target id collection set for taks the valid cas number.
   */
   function select_cas_number(x: any): string;
   /**
    * unique of the peak annotation features
    * 
    * 
     * @param query all query result that comes from the ms1_search function.
     * @param uniqueByScore 
     * + default value Is ``false``.
     * @param scoreFactors the reference name this score data must be 
     *  generated via the @``M:BioNovoGene.BioDeep.MSEngine.MzQuery.ReferenceKey(BioNovoGene.BioDeep.MSEngine.MzQuery,System.String)`` 
     *  function.
     * 
     * + default value Is ``null``.
     * @param format the numeric format of the mz value for generate the reference key
     * 
     * + default value Is ``'F4'``.
     * @param removesZERO removes all metabolites with ZERO score?
     * 
     * + default value Is ``false``.
     * @param verbose 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function uniqueFeatures(query: object, uniqueByScore?: boolean, scoreFactors?: object, format?: string, removesZERO?: boolean, verbose?: boolean, env?: object): object;
   /**
    * verify that the given cas registry number is correct or not
    * 
    * > based on the @``M:BioNovoGene.BioDeep.Chemoinformatics.CASNumber.Verify(System.String)`` clr function.
    * 
     * @param num -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function verify_cas_number(num: any, env?: object): boolean;
}
