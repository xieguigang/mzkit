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
     * @param engine -
     * @param uniqueId -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function getMetadata(engine: any, uniqueId: object, env?: object): any;
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
   /**
     * @param env default value Is ``null``.
   */
   function load_asQueryHits(x: object, env?: object): object;
   module mass_search {
      /**
       * 
       * 
        * @param massSet -
        * @param type -
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
   function ms1_handler(compounds: any, precursors: any, tolerance?: any, env?: object): any;
   /**
    * get duplictaed raw annotation results.
    * 
    * 
     * @param engine -
     * @param mz a m/z numeric vector or a object list that 
     *  contains the data mapping of unique id to 
     *  m/z value.
     * @param unique 
     * + default value Is ``false``.
     * @param uniqueByScore only works when **`unique`** parameter
     *  value is set to value TRUE.
     * 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function ms1_search(engine: any, mz: any, unique?: boolean, uniqueByScore?: boolean, env?: object): object;
   /**
     * @param keepsRaw default value Is ``false``.
     * @param env default value Is ``null``.
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
