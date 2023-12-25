// export R# package module type define for javascript/typescript language
//
//    imports "MSI" from "mzkit";
//
// ref=mzkit.MSI@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * MS-Imaging data handler
 *  
 *  Mass spectrometry imaging (MSI) is a technique used in mass spectrometry
 *  to visualize the spatial distribution of molecules, as biomarkers, 
 *  metabolites, peptides or proteins by their molecular masses.
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
        * @param strict if the input ``**`dims`**`` produce invalid dimension size
        *  value, example as dimension size is equals to ZERO [0,0], then in strict 
        *  mode, the dimension value will be evaluated from the input raw data
        *  automatically for ensure that the dimension size of the generated layer 
        *  data object is not empty.
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A ms-imaging layer object that could be used for run ms-imaging rendering.
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
        * @param mzdiff the mass tolerance error in @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.DAmethod``
        * 
        * + default value Is ``0.01``.
        * @param dims the dimension size of the ms-imaging spatial data
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function spatial_layers(x: object, mzdiff?: number, dims?: any, env?: object): object;
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
    * get or set the dimension size of the ms-imaging mzpack raw data object
    * 
    * 
     * @param raw -
     * @param dims -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function dimension_size(raw: object, dims?: any, env?: object): any;
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
   module levels {
      /**
       * sum pixels for create pixel spot convolution
       * 
       * 
        * @param mat A matrix liked dataframe object that contains the 
        *  molecule expression data on each spatial spots, data object should 
        *  in format of spatial spot in columns and molecule feature in rows.
        * @param clusters 
        * + default value Is ``6``.
        * @param win_size 
        * + default value Is ``3``.
      */
      function convolution(mat: object, clusters?: object, win_size?: object): object;
   }
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
    * > the input raw data object could be also a file path to the ms-imaging mzpack 
    * >  rawdata, but only version 2 mzPack data file will be supported for load 
    * >  metadata.
    * 
     * @param raw should be a mzPack rawdata object which is used for the ms-imaging application.
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
        * @param file the file path to the specific imzML metadata file for load 
        *  for run ms-imaging analysis.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return this function returns a tuple list object that contains 2 slot elements inside:
        *  
        *  1. scans: is the [x,y] spatial scans data: @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.ScanData``.
        *  2. ibd: is the binary data reader wrapper object for the corresponding 
        *        ``ibd`` file of the given input imzML file: @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.ibdReader``.
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
     * @param noise_cutoff 
     * + default value Is ``1``.
     * @param source_tag 
     * + default value Is ``'pack_matrix'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pack_matrix(file: any, dims?: any, res?: number, noise_cutoff?: number, source_tag?: string, env?: object): any;
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
     * @return the data format of the two kind of the output data result is keeps the same:
     *  
     *  + for a raw matrix object, the column is the ion features and the rows is the spatial spots.
     *  + for a dataset collection vector, the column is also the ion features and the 
     *    rows is the spatial spots.
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
     * @param raw a ms-imaging rawdata object in mzpack format.
     * @param mz a m/z numeric value
     * @param tolerance the mass tolerance error for match the ion in the rawdata.
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
     * @param verbose 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return This function returns a logical value TRUE if the 
     *  given **`file`** stream buffer is not missing,
     *  otherwise the matrix object itself will be returns from 
     *  the function.
   */
   function pixelMatrix(raw: object, file?: any, mzdiff?: number, q?: number, fast_bin?: boolean, verbose?: boolean, env?: object): boolean|object;
   /**
    * get pixels size from the raw data file
    * 
    * 
     * @param file imML/mzPack, or a single ion layer of the ms-imaging rawdata
     * @param count get the pixel count number instead of get the canvas dimension size of the ms-imaging.
     * 
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
      function scans(raw: string, y?: object, correction?: object, env?: object): object;
   }
   /**
    * make expression bootstrapping of current ion layer
    * 
    * > Bootstrapping is a statistical procedure that resamples a single dataset to create
    * >  many simulated samples. This process allows you to calculate standard errors, 
    * >  construct confidence intervals, and perform hypothesis testing for numerous types of
    * >  sample statistics. Bootstrap methods are alternative approaches to traditional 
    * >  hypothesis testing and are notable for being easier to understand and valid for more 
    * >  conditions.
    * 
     * @param layer The target ion layer to run expression bootstraping
     * @param tissue A collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.TissueRegion`` object.
     * @param n Get n sample points for each tissue region
     * 
     * + default value Is ``32``.
     * @param coverage The region area coverage for the bootstrapping.
     * 
     * + default value Is ``0.3``.
     * @return A tuple list object that contains the expression data for each @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.TissueRegion``:
     *  
     *  1. the tuple key is the label of the tissue region data,
     *  2. the tuple value is the numeric expression vector that sampling from 
     *     the corrisponding tissue region, the vector size is equals to the 
     *     parameter ``n``.
   */
   function sample_bootstraping(layer: object, tissue: object, n?: object, coverage?: number): any;
   /**
    * scale the spatial matrix by column
    * 
    * 
     * @param m a dataframe object that contains the spot expression data. 
     *  should be in format of: spot in column and ion features in rows.
     * @param factor the size of this numeric vector should be equals to the 
     *  ncol of the given dataframe input **`m`**.
     * @param bpc scle by bpc or scale by tic?
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A new dataframe data after scaled
   */
   function scale(m: object, factor: any, bpc?: boolean, env?: object): object;
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
     * @param raw the mzpack rawdata object that used for run ms-imaging data analysis.
     * @param partition this parameter indicates that how many blocks that will be splice 
     *  into parts on width dimension and column dimension. the number of the pixels in each 
     *  partition block will be evaluated from this parameter.
     * 
     * + default value Is ``5``.
     * @return A collection of the ms-imaging mzpack object that split from multiple 
     *  parts based on the input **`raw`** data mzpack inputs.
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
        * @param dims an integer vector for set the size of the ms-imaging canvas dimension
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function imzML(mzpack: object, file: string, res?: number, ionMode?: object, dims?: any, env?: object): boolean;
   }
}
