// export R# package module type define for javascript/typescript language
//
//    imports "cellsPack" from "mzkit";
//
// ref=mzkit.SingleCellsPack@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace cellsPack {
   /**
     * @param source_tag default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function pack_cells(rawdata: any, source_tag?: string, env?: object): any;
}
