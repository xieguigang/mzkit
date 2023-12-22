// export R# package module type define for javascript/typescript language
//
//    imports "SingleCells" from "mzkit";
//
// ref=mzkit.SingleCells@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Single cells metabolomics data processor
 *  
 *  Single-cell analysis is a technique that measures only the target cell itself and can 
 *  extract information that would be buried in bulk-cell analysis with high-resolution.
 * 
 * > Single-cell metabolomics is a powerful tool that can reveal cellular heterogeneity and 
 * >  can elucidate the mechanisms of biological phenomena in detail. It is a promising 
 * >  approach in studying plants, especially when cellular heterogeneity has an impact on different 
 * >  biological processes. In addition, metabolomics, which can be regarded as a detailed 
 * >  phenotypic analysis, is expected to answer previously unrequited questions which will 
 * >  lead to expansion of crop production, increased understanding of resistance to diseases,
 * >  and in other applications as well.
*/
declare namespace SingleCells {
   module apply {
      /**
       * scale matrix for each spot/cell sample
       * 
       * 
        * @param x -
        * @param scaler A R# @``T:SMRUCC.Rsharp.Runtime.Components.Interface.RFunction`` for apply the scale transform.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function scale(x: any, scaler: object, env?: object): object;
   }
   module as {
      /**
       * Cast the ion feature matrix as the GCModeller expression matrix object
       * 
       * 
        * @param x -
        * @param single_cell 
        * + default value Is ``false``.
        * @return the gcmodeller expression matrix object, each @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow`` element inside the 
        *  generated matrix object is the expression vector of all metabolite ion features. which means
        *  the matrix format from this function outputs should be:
        *  
        *  1. cell labels, or spatial location in rows
        *  2. and ion features in columns.
      */
      function expression(x: object, single_cell?: boolean): object;
   }
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
   function cell_matrix(raw: object, mzdiff?: number, freq?: number, env?: object): object;
   module df {
      /**
       * cast matrix object to the R liked dataframe object
       * 
       * 
        * @param x the matrix object that going to do the type casting
      */
      function mz_matrix(x: object): object;
   }
   /**
    * cast the matrix object as the dataframe
    * 
    * > implements the ``as.data.frame`` function
    * 
     * @param x -
     * @param args -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mz_matrix(x: any, args: object, env?: object): object;
   module open {
      /**
       * open a single cell data matrix reader
       * 
       * > this function open a lazy reader of the matrix, for load all 
       * >  data into memory at once, use the ``read.mz_matrix`` 
       * >  function.
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A tuple list that contains the data elements:
        *  
        *  1. tolerance: the mass tolerance description for seperates the ion features
        *  2. featureSize: the number of the ion features in the raw data file
        *  3. ionSet: a numeric vector of the ion features m/z value.
        *  4. spots: the number of the spots that read from the rawdata matrix file
        *  5. reader: the rawdata @``T:BioNovoGene.Analytical.MassSpectrometry.SingleCells.MatrixReader``
      */
      function matrix(file: any, env?: object): object;
   }
   module read {
      /**
       * load the data matrix into memory at once
       * 
       * > for create a lazy data reader of the matrix, use the ``open.matrix`` function.
       * 
        * @param file a file connection to the matrix file or the matrix lazy 
        *  reader object which is created via the function 
        *  ``open.matrix``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mz_matrix(file: any, env?: object): object;
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
