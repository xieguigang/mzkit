// export R# package module type define for javascript/typescript language
//
//    imports "cellsPack" from "mzkit";
//
// ref=mzkit.SingleCellsPack@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * package tools for the single cells metabolomics rawdata processing
 * 
 * > works for the single-cell flow cytometry rawdata
*/
declare namespace cellsPack {
   module pack_cells {
      /**
       * pack of the single cells metabolomics data in multiple sample groups
       * 
       * > this function required of each single rawdata file just contains only 
       * >  one single cell data.
       * 
        * @param groups could be a character vector of the folder path of the raw data files, 
        *  it is recommended that using a tuple list for set this sample group value, 
        *  the key name in the tuple list is the sample group name and the corresponding
        *  value is the folder path of the single cells rawdata files.
        * @param source_tag usually be the organism source name
        * 
        * + default value Is ``null``.
        * @param verbose 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function group(groups: any, source_tag?: string, verbose?: boolean, env?: object): any;
   }
}
