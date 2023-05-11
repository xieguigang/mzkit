// export R# package module type define for javascript/typescript language
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
}
