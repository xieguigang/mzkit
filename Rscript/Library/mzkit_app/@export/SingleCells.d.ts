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
      function scale(x: any, scaler: any, env?: object): object;
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
    * export the cell clustering result
    * 
    * 
     * @param pool -
     * @return a tuple list of the cell clustering result,
     *  each tuple is a cluster result.
   */
   function cell_clusters(pool: object): object;
   /**
    * create a session for create spot cell embedding
    * 
    * 
     * @param ndims the embedding vector size, greater than 30 and less than 100 dimension is recommended.
     * 
     * + default value Is ``30``.
     * @param method -
     * 
     * + default value Is ``null``.
     * @param freq -
     * 
     * + default value Is ``3``.
     * @param diff the score diff for build the tree branch
     * 
     * + default value Is ``0.1``.
   */
   function cell_embedding(ndims?: object, method?: object, freq?: object, diff?: number): object;
   /**
    * export single cell expression matrix from the raw data scans
    * 
    * 
     * @param raw the raw data for make epxression matrix, could be a mzkit @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.mzPack`` object, 
     *  or a tuple list of the msdata @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.LibraryMatrix``
     * @param mzdiff -
     * 
     * + default value Is ``0.005``.
     * @param freq 
     * + default value Is ``0.001``.
     * @param ions_mz 
     * + default value Is ``null``.
     * @param mz_matrix 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function cell_matrix(raw: any, mzdiff?: number, freq?: number, ions_mz?: any, mz_matrix?: boolean, env?: object): object|object;
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
    * push a sample data into the embedding session
    * 
    * > the spectrum data will be re-order via the spectrum total ions desc
    * 
     * @param pool -
     * @param sample -
     * @param tag -
     * 
     * + default value Is ``null``.
     * @param vocabulary 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function embedding_sample(pool: object, sample: any, tag?: string, vocabulary?: object, env?: object): object;
   /**
    * cast the matrix object as the dataframe
    * 
    * > implements the ``as.data.frame`` function
    * 
     * @param x should be a rawdata object in general type: @``T:BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.MzMatrix``.
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
    * do statistics of the single cell metabolomics ions features
    * 
    * 
     * @param raw the @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.mzPack`` rawdata object, or a single cells matrix object
     * @param da -
     * 
     * + default value Is ``0.01``.
     * @param parallel -
     * 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function SCM_ionStat(raw: any, da?: number, parallel?: boolean, env?: object): object;
   /**
   */
   function singleCell_labels(x: object): string;
   /**
    * get the labels based on the spatial information of each spot
    * 
    * 
     * @param x -
   */
   function spatial_labels(x: object): string;
   /**
    * get the cell spot embedding result
    * 
    * 
     * @param pool -
     * @return vector data could be converts the dataframe object via ``as.data.frame``
   */
   function spot_vector(pool: object): object;
   /**
    * do matrix data normalization via total peak sum
    * 
    * 
     * @param x -
     * @param scale -
     * 
     * + default value Is ``1000000``.
   */
   function total_peaksum(x: object, scale?: number): object;
   module write {
      /**
       * write the single cell ion feature data matrix
       * 
       * 
        * @param x the expression @``T:BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.MzMatrix`` object.
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function matrix(x: object, file: any, env?: object): boolean;
   }
}
