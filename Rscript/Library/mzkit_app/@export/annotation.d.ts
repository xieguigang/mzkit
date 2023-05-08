// export R# package module type define for javascript/typescript language
//
// ref=mzkit.library@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the metabolite annotation toolkit
 * 
*/
declare namespace annotation {
   module assert {
      /**
        * @param ion_mode default value Is ``'+'``.
        * @param env default value Is ``null``.
      */
      function adducts(formula:string, adducts:any, ion_mode?:any, env?:object): object;
   }
   /**
    * a shortcut method for populate the peak ms2 data from a mzpack raw data file
    * 
    * 
     * @param raw -
     * @param mzdiff -
     * 
     * + default value Is ``'da:0.3'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function populateIonData(raw:object, mzdiff?:any, env?:object): object;
   module make {
      /**
       * create a new metabolite annotation information
       * 
       * 
        * @param id -
        * @param formula -
        * @param name -
        * @param synonym -
        * 
        * + default value Is ``null``.
        * @param xref -
        * 
        * + default value Is ``null``.
      */
      function annotation(id:string, formula:string, name:string, synonym?:string, xref?:object): object;
   }
   /**
    * Check the ms1 parent ion is generated via the in-source fragment or not
    * 
    * 
     * @param ms1 the ms1 peaktable dataset, it could be a xcms peaktable object dataframe, a collection of ms1 scan with unique id tagged.
     * @param ms2 the ms2 products list
     * @param da 
     * + default value Is ``0.1``.
     * @param rt_win 
     * + default value Is ``5``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a tuple key-value pair list object that contains the flags for each ms1 ion
     *  corresponding slot value TRUE means the key ion is a possible in-source
     *  fragment ion data, otherwise slot value FALSE means not.
   */
   function checkInSourceFragments(ms1:any, ms2:any, da?:number, rt_win?:number, env?:object): boolean;
}
