// export R# package module type define for javascript/typescript language
//
//    imports "TissueMorphology" from "mzkit";
//
// ref=mzkit.TissueMorphology@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace TissueMorphology {
   /**
     * @param gridSize default value Is ``6``.
     * @param label default value Is ``null``.
   */
   function gridding(mapping: object, gridSize?: object, label?: string): any;
   /**
     * @param trim_suffix default value Is ``false``.
   */
   function intersect_layer(layer: object, tissues: object, trim_suffix?: boolean): any;
   /**
     * @param id default value Is ``'*'``.
     * @param env default value Is ``null``.
   */
   function loadTissue(file: any, id?: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function loadUMAP(file: any, env?: object): object;
   module read {
      /**
        * @param remove_suffix default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function spatialMapping(file: string, remove_suffix?: boolean, env?: object): object;
   }
   /**
   */
   function splitMapping(mapping: object): object;
   /**
     * @param trim_suffix default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function tag_samples(MSI: any, tissues: object, trim_suffix?: boolean, env?: object): any;
   /**
     * @param colorSet default value Is ``'Paper'``.
     * @param env default value Is ``null``.
   */
   function TissueData(x: object, y: object, labels: string, colorSet?: any, env?: object): object;
   /**
     * @param is_singlecells default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function UMAPsample(points: any, x: number, y: number, z: number, cluster: string, is_singlecells?: boolean, env?: object): object;
   /**
     * @param umap default value Is ``null``.
     * @param dimension default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function writeCDF(tissueMorphology: object, file: any, umap?: object, dimension?: any, env?: object): boolean;
}
