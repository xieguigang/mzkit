// export R# package module type define for javascript/typescript language
//
//    imports "MSI" from "mzkit";
//
// ref=mzkit.MSI@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * MS-Imaging data handler
 * 
*/
declare namespace MSI {
   module as {
      /**
       * cast the pixel collection to a ion imaging layer data
       * 
       * 
        * @param x Should be a collection of the ms-imaging pixel data 
        *  object, or a mz matrix object
        * @param context the ms-imaging layer title, must be a valid mz numeric value if the input x 
        *  is a mz matrix object
        * 
        * + default value Is ``'MSIlayer'``.
        * @param dims the dimension size of the ms-imaging layer data,
        *  this dimension size will be evaluated based on the input pixel collection
        *  data if this parameter leaves blank(or NULL) by default.
        * 
        * + default value Is ``null``.
        * @param strict -
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function layer(x: any, context?: any, dims?: any, strict?: boolean, env?: object): object;
   }
   /**
    * Get the mass spectrum data of the MSI base peak data
    * 
    * 
     * @param summary -
   */
   function basePeakMz(summary: object): object;
   module cast {
      /**
       * cast the rawdata matrix as the ms-imaging ion layer
       * 
       * 
        * @param x the matrix object
        * @param mzdiff -
        * 
        * + default value Is ``0.01``.
        * @param dims the dimension size of the ms-imaging spatial data
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function spatial_layers(x: object, mzdiff?: number, dims?: any, env?: object): any;
   }
   /**
    * calculate the X scale
    * 
    * 
     * @param totalTime -
     * @param pixels -
     * @param hasMs2 -
     * 
     * + default value Is ``false``.
   */
   function correction(totalTime: number, pixels: object, hasMs2?: boolean): object;
   /**
    * get matrix ions feature m/z vector
    * 
    * 
     * @param raw -
     * @param mzdiff -
     * 
     * + default value Is ``0.001``.
     * @param q -
     * 
     * + default value Is ``0.001``.
     * @param fast_bins -
     * 
     * + default value Is ``true``.
   */
   function getMatrixIons(raw: object, mzdiff?: number, q?: number, fast_bins?: boolean): number;
   /**
     * @param env default value Is ``null``.
   */
   function ions_jointmatrix(raw: object, env?: object): object;
   /**
    * Extract the ion features inside a MSI raw data slide sample file
    * 
    * > count pixels/density/etc for each ions m/z data
    * 
     * @param raw the raw data object could be a mzpack data object or 
     *  MS-imaging ion feature layers object
     * @param grid_size the grid cell size for evaluate the pixel density
     * 
     * + default value Is ``5``.
     * @param da the mass tolerance value, only works when
     *  the input raw data object is mzpack object
     * 
     * + default value Is ``0.01``.
     * @param parallel 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function ionStat(raw: any, grid_size?: object, da?: number, parallel?: boolean, env?: object): object;
   /**
    * evaluate the moran index for each ion layer
    * 
    * 
     * @param x A spatial expression data matrix, should be in format of:
     *  
     *  1. the spatial spot xy in row names, and
     *  2. the ions feature m/z label in col names
   */
   function moran_I(x: object): any;
   /**
    * get ms-imaging metadata
    * 
    * 
     * @param raw -
     * @param env 
     * + default value Is ``null``.
   */
   function msi_metadata(raw: any, env?: object): object;
   /**
    * Fetch MSI summary data
    * 
    * 
     * @param raw -
     * @param x 
     * + default value Is ``null``.
     * @param y 
     * + default value Is ``null``.
     * @param as_vector returns the raw vector of @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.iPixelIntensity`` if set this
     *  parameter value to value TRUE, or its wrapper object @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.MSISummary`` 
     *  if set this parameter value to FALSE by default.
     * 
     * + default value Is ``false``.
     * @param dims overrides the MSI data its scan dimension value? This parameter value is
     *  a numeric vector with two integer element that represents the dimension
     *  of the MSI data(width and height)
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function MSI_summary(raw: object, x?: object, y?: object, as_vector?: boolean, dims?: any, env?: object): object|object;
   module open {
      /**
       * open the reader for the imzML ms-imaging file
       * 
       * 
        * @param file the file path to the specific imzML metadata file for load for run ms-imaging analysis.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return this function returns a tuple list object that contains 2 slot elements inside:
        *  
        *  1. scans: is the [x,y] spatial scans data
        *  2. ibd: is the binary data reader wrapper object for the corresponding 
        *        ``ibd`` file of the given input imzML file.
      */
      function imzML(file: string, env?: object): object;
   }
   /**
    * pack the matrix file as the MSI mzpack
    * 
    * 
     * @param file the file resource reference to the csv table file, and the
     *  csv file should be in format of ion peaks features in column
     *  and spatial spot id in rows
     * @param dims 
     * + default value Is ``null``.
     * @param res 
     * + default value Is ``17``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pack_matrix(file: any, dims?: any, res?: number, env?: object): any;
   /**
    * Extract the ion data matrix
    * 
    * 
     * @param raw -
     * @param topN select top N ion feature in each spot and then union the ion features as 
     *  the features set, this parameter only works when the **`ionSet`** 
     *  parameter is empty or null.
     * 
     * + default value Is ``3``.
     * @param mzError The mass tolerance of the ion m/z
     * 
     * + default value Is ``'da:0.05'``.
     * @param ionSet A tuple list of the ion dataset range, the tuple list object should 
     *  be in data format of [unique_id => mz]. Or this parameter value could also
     *  be a numeric vector of the target m/z feature values
     * 
     * + default value Is ``null``.
     * @param raw_matrix 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakMatrix(raw: object, topN?: object, mzError?: any, ionSet?: any, raw_matrix?: boolean, env?: object): object|object;
   /**
    * split the raw MSI 2D data into multiple parts with given resolution parts
    * 
    * 
     * @param raw -
     * @param resolution -
     * 
     * + default value Is ``100``.
     * @param mzError -
     * 
     * + default value Is ``'da:0.05'``.
     * @param cutoff -
     * 
     * + default value Is ``0.05``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return returns the raw matrix data that contains the peak samples.
   */
   function peakSamples(raw: object, resolution?: object, mzError?: any, cutoff?: number, env?: object): any;
   /**
    * get pixels [x,y] tags collection for a specific ion
    * 
    * 
     * @param raw -
     * @param mz -
     * @param tolerance -
     * 
     * + default value Is ``'da:0.1'``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a character vector of the pixel [x,y] tags.
   */
   function pixelId(raw: object, mz: number, tolerance?: any, env?: object): string;
   /**
    * get number of ions in each pixel scans
    * 
    * 
     * @param raw -
   */
   function pixelIons(raw: object): object;
   /**
    * dumping raw data matrix as text table file.
    * 
    * 
     * @param raw -
     * @param file -
     * 
     * + default value Is ``null``.
     * @param mzdiff the mass tolerance width for extract the feature ions
     * 
     * + default value Is ``0.001``.
     * @param q the frequence threshold for filter the feature ions, this 
     *  value range of this parameter should be inside [0,1] which
     *  means percentage cutoff.
     * 
     * + default value Is ``0.01``.
     * @param fast_bin 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return This function returns a logical value TRUE if the 
     *  given **`file`** stream buffer is not missing,
     *  otherwise the matrix object itself will be returns from 
     *  the function.
   */
   function pixelMatrix(raw: object, file?: any, mzdiff?: number, q?: number, fast_bin?: boolean, env?: object): boolean|object;
   /**
    * get pixels size from the raw data file
    * 
    * 
     * @param file imML/mzPack
     * @param count 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return this function will returns the pixels in dimension size(a tuple list data with slot keys w and h) 
     *  if the count is set to FALSE, by default; otherwise this function will return an integer value for
     *  indicates the real pixel counts number if the count parameter is set to TRUE.
   */
   function pixels(file: any, count?: boolean, env?: object): object;
   module row {
      /**
       * each raw data file is a row scan data
       * 
       * 
        * @param raw a file list of mzpack data files
        * @param y this function will returns the pixel summary data if the ``y`` parameter greater than ZERO.
        * 
        * + default value Is ``0``.
        * @param correction used for data summary, when the ``y`` parameter is greater than ZERO, 
        *  this parameter will works.
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function scans(raw: string, y?: object, correction?: object, env?: object): any;
   }
   /**
     * @param n default value Is ``32``.
     * @param coverage default value Is ``0.3``.
   */
   function sample_bootstraping(layer: object, tissue: object, n?: object, coverage?: number): any;
   /**
    * scale the spatial matrix by column
    * 
    * 
     * @param m -
     * @param factor the size of this numeric vector should be equals to the 
     *  ncol of the given dataframe input **`m`**.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function scale(m: object, factor: any, env?: object): any;
   /**
    * combine each row scan summary vector as the pixels 2D matrix
    * 
    * 
     * @param rowScans -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function scanMatrix(rowScans: any, env?: object): object;
   /**
    * combine each row scan raw data files as the pixels 2D matrix
    * 
    * 
     * @param rowScans data result comes from the function ``row.scans``.
     * @param correction 
     * + default value Is ``null``.
     * @param intocutoff 
     * + default value Is ``0.05``.
     * @param yscale apply for mapping smooth MS1 to ms2 scans
     * 
     * + default value Is ``1``.
     * @param env 
     * + default value Is ``null``.
   */
   function scans2D(rowScans: any, correction?: object, intocutoff?: number, yscale?: number, env?: object): any;
   module spatial {
      /**
       * sum pixels for create pixel spot convolution
       * 
       * 
        * @param mat A matrix liked dataframe object that contains the 
        *  molecule expression data on each spatial spots, data object should 
        *  in format of spatial spot in columns and molecule feature in rows.
        * @param win_size 
        * + default value Is ``2``.
        * @param steps 
        * + default value Is ``1``.
      */
      function convolution(mat: object, win_size?: object, steps?: object): object;
   }
   /**
    * split the raw 2D MSI data into multiple parts with given parts
    * 
    * 
     * @param raw -
     * @param partition -
     * 
     * + default value Is ``5``.
   */
   function splice(raw: object, partition?: object): object;
   module write {
      /**
       * Save and write the given ms-imaging mzpack object as imzML file
       * 
       * 
        * @param mzpack -
        * @param file -
        * @param res the spatial resolution value
        * 
        * + default value Is ``17``.
        * @param ionMode the ion polarity mode value
        * 
        * + default value Is ``null``.
      */
      function imzML(mzpack: object, file: string, res?: number, ionMode?: object): any;
   }
}
