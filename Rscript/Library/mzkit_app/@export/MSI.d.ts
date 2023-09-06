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
        * @param pixels -
        * @param context -
        * 
        * + default value Is ``'MSIlayer'``.
        * @param dims -
        * 
        * + default value Is ``null``.
        * @param strict -
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function layer(pixels: object, context?: string, dims?: any, strict?: boolean, env?: object): object;
   }
   /**
   */
   function basePeakMz(summary: object): object;
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
     * @param mzdiff default value Is ``0.001``.
     * @param q default value Is ``0.001``.
   */
   function getMatrixIons(raw: object, mzdiff?: number, q?: number): number;
   /**
     * @param env default value Is ``null``.
   */
   function ions_jointmatrix(raw: object, env?: object): object;
   /**
    * count pixels/density/etc for each ions m/z data
    * 
    * 
     * @param raw -
     * @param grid_size -
     * 
     * + default value Is ``5``.
     * @param da -
     * 
     * + default value Is ``0.01``.
     * @param parallel 
     * + default value Is ``true``.
   */
   function ionStat(raw: object, grid_size?: object, da?: number, parallel?: boolean): object;
   /**
    * get ms-imaging metadata
    * 
    * 
     * @param raw -
   */
   function msi_metadata(raw: object): object;
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
      */
      function imzML(file: string): any;
   }
   /**
    * Extract the ion data matrix
    * 
    * 
     * @param raw -
     * @param topN -
     * 
     * + default value Is ``3``.
     * @param mzError -
     * 
     * + default value Is ``'da:0.05'``.
     * @param ionSet A tuple list of the ion dataset range, the tuple list object should 
     *  be in data format of [unique_id => mz]
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakMatrix(raw: object, topN?: object, mzError?: any, ionSet?: object, env?: object): any;
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
     * @param mzdiff 
     * + default value Is ``0.001``.
     * @param q 
     * + default value Is ``0.001``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pixelMatrix(raw: object, file: object, mzdiff?: number, q?: number, env?: object): object;
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
      */
      function imzML(mzpack: object, file: string): any;
   }
}
