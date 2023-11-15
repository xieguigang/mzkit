// export R# package module type define for javascript/typescript language
//
//    imports "SingleCells" from "mzkit";
//
// ref=mzkit.SingleCells@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Single cells metabolomics data processor
 * 
*/
declare namespace SingleCells {
   /**
    * export single cell expression matrix from the raw data scans
    * 
    * 
     * @param raw -
     * @param mzdiff -
     * 
     * + default value Is ``0.005``.
     * @param freq 
     * + default value Is ``0.001``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function cell_matrix(raw: object, mzdiff?: number, freq?: number, env?: object): any;
   module open {
      /**
       * open a single cell data matrix reader
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function matrix(file: any, env?: object): object;
   }
   /**
    * do stats of the single cell metabolomics ions
    * 
    * 
     * @param raw -
     * @param da -
     * 
     * + default value Is ``0.01``.
     * @param parallel -
     * 
     * + default value Is ``true``.
   */
   function SCM_ionStat(raw: object, da?: number, parallel?: boolean): object;
   module write {
      /**
       * write the single cell ion feature data matrix
       * 
       * 
        * @param x -
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function matrix(x: object, file: any, env?: object): boolean;
   }
}
