// export R# package module type define for javascript/typescript language
//
// ref=mzkit.TissueMorphology@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * spatial tissue region handler
 *  
 *  tissue morphology data handler for the internal 
 *  bionovogene MS-imaging analysis pipeline.
 * 
*/
declare namespace TissueMorphology {
   /**
    * create a collection of the umap sample data
    * 
    * 
     * @param points -
     * @param x -
     * @param y -
     * @param z -
     * @param cluster -
     * @param is_singlecells 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function UMAPsample(points: any, x: number, y: number, z: number, cluster: object, is_singlecells?: boolean, env?: object): object;
   /**
    * create a collection of the tissue region dataset
    * 
    * 
     * @param x -
     * @param y -
     * @param labels -
     * @param colorSet the color set schema name or a list of color data 
     *  which can be mapping to the given **`labels`** 
     *  list.
     * 
     * + default value Is ``'Paper'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function TissueData(x: object, y: object, labels: string, colorSet?: any, env?: object): object;
   /**
    * export the tissue data as cdf file
    * 
    * 
     * @param tissueMorphology -
     * @param file -
     * @param umap -
     * 
     * + default value Is ``null``.
     * @param dimension -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function writeCDF(tissueMorphology: object, file: any, umap?: object, dimension?: object, env?: object): any;
   /**
    * load tissue region polygon data
    * 
    * 
     * @param file -
     * @param id the region id, which could be used for load specific 
     *  region polygon data. default nothing means load all
     *  tissue region polygon data
     * 
     * + default value Is ``'*'``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a collection of tissue polygon region objects.
   */
   function loadTissue(file: any, id?: string, env?: object): object;
   /**
    * load UMAP data
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function loadUMAP(file: any, env?: object): object;
   module read {
      /**
       * read spatial mapping data of STdata mapping to SMdata
       * 
       * 
        * @param file the file path of the spatial mapping xml dataset file
        * @param remove_suffix removes of the numeric suffix of the STdata barcode?
        * 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function spatialMapping(file: string, remove_suffix?: boolean, env?: object): object;
   }
   /**
   */
   function splitMapping(mapping: object): object;
   /**
    * create a spatial grid for the spatial spot data
    * 
    * 
     * @param mapping -
     * @param gridSize -
     * 
     * + default value Is ``6``.
     * @param label the parameter value will overrides the internal
     *  label of the mapping if this parameter string 
     *  value is not an empty string.
     * 
     * + default value Is ``null``.
   */
   function gridding(mapping: object, gridSize?: object, label?: string): any;
}
