// export R# package module type define for javascript/typescript language
//
//    imports "MsImaging" from "mzplot";
//
// ref=mzkit.MsImaging@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ### Visual MS imaging data(*.imzML)
 *  
 *  Mass spec imaging (MSI) is a technique measuring chemical composition and 
 *  linking it to spatial coordinates on a surface.  The chemical composition
 *  is determined by mass spectrometry, which measures the mass-to-charge ratios
 *  (m/z's) of any ions that can be generated from the surface.  Most commonly,
 *  the surface is a tissue section on a microscope slide; however, any flat 
 *  surface could be analyzed given it has suitable dimensions and is properly 
 *  prepared.
 * 
*/
declare namespace MsImaging {
   module as {
      /**
       * extract the pixel [x,y] information for all of
       *  the points in the target **`layer`**
       * 
       * 
        * @param layer -
        * @param character the function will returns the character vector 
        *  when this parameter value is set to TRUE
        * 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function pixels(layer: any, character?: boolean, env?: object): string|object;
   }
   /**
    * test of a given MSI layer is target?
    * 
    * 
     * @param layer -
     * @param xy a character vector of ``x,y``
     * @param cutoff 
     * + default value Is ``0.8``.
     * @param samplingRegion 
     * + default value Is ``true``.
   */
   function assert(layer: object, xy: object, cutoff?: number, samplingRegion?: boolean): boolean;
   /**
    * get the default ms-imaging filter pipeline
    * 
    * 
   */
   function defaultFilter(): object;
   /**
    * Extract a spectrum matrix object from MSI data by a given set of m/z values
    * 
    * 
     * @param viewer -
     * @param mz -
     * @param tolerance -
     * 
     * + default value Is ``'ppm:20'``.
     * @param title -
     * 
     * + default value Is ``'FilterMz'``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A spectrum matrix data of m/z value assocated with the intensity value
   */
   function FilterMz(viewer: object, mz: number, tolerance?: any, title?: string, env?: object): object;
   /**
    * Get intensity data vector from a given MS-imaging layer
    * 
    * 
     * @param layer -
     * @param summary -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function intensity(layer: any, summary?: object, env?: object): number;
   /**
    * trim the intensity data value in a pixels of a ion MS-Imaging layer
    * 
    * 
     * @param data -
     * @param max -
     * @param min -
     * 
     * + default value Is ``0``.
   */
   function intensityLimits(data: object, max: number, min?: number): object;
   /**
    * load the raw pixels data from imzML file
    * 
    * 
     * @param mz a collection of ion m/z value for rendering on one image
     * @param tolerance m/z tolerance error for get layer data
     * 
     * + default value Is ``'da:0.1'``.
     * @param skip_zero -
     * 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function ionLayers(imzML: any, mz: number, tolerance?: any, skip_zero?: boolean, env?: object): object;
   /**
    * do pixel interpolation for run MS-imaging
    * 
    * 
     * @param layer -
     * @param gridSize knn size
     * 
     * + default value Is ``3``.
     * @param q -
     * 
     * + default value Is ``0.8``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function knnFill(layer: any, gridSize?: object, q?: number, env?: object): object|object;
   /**
    * render a ms-imaging layer by a specific ``m/z`` scan.
    * 
    * 
     * @param viewer -
     * @param mz -
     * @param pixelSize -
     * 
     * + default value Is ``'5,5'``.
     * @param tolerance -
     * 
     * + default value Is ``'da:0.1'``.
     * @param color 
     * + default value Is ``'viridis:turbo'``.
     * @param levels 
     * + default value Is ``30``.
     * @param cutoff 
     * + default value Is ``[0.1,0.75]``.
     * @param background 
     * + default value Is ``'Transparent'``.
     * @param env 
     * + default value Is ``null``.
   */
   function layer(viewer: object, mz: number, pixelSize?: any, tolerance?: any, color?: string, levels?: object, cutoff?: any, background?: any, env?: object): object;
   /**
    * 
    * 
     * @param raw -
     * @param gridSize -
     * 
     * + default value Is ``6``.
     * @param mzdiff -
     * 
     * + default value Is ``'da:0.1'``.
     * @param keepsLayer if the options is set to false, then this function just returns the ions mz vector.
     *  otherwise, returns a dataframe that contains m/z, density value and ion layer 
     *  objects.
     * 
     * + default value Is ``false``.
     * @param densityCut -
     * 
     * + default value Is ``0.1``.
     * @param qcut 
     * + default value Is ``0.01``.
     * @param intoCut 
     * + default value Is ``0``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function MeasureMSIions(raw: object, gridSize?: object, mzdiff?: any, keepsLayer?: boolean, densityCut?: number, qcut?: number, intoCut?: number, env?: object): number|object;
   /**
    * get the ms1 spectrum data in a specific pixel position
    * 
    * 
     * @param viewer -
     * @param x -
     * @param y -
     * @param tolerance -
     * 
     * + default value Is ``'da:0.1'``.
     * @param threshold -
     * 
     * + default value Is ``0.01``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function MS1(viewer: object, x: object, y: object, tolerance?: any, threshold?: number, env?: object): object;
   /**
     * @param samplingRegion default value Is ``true``.
   */
   function MSI_coverage(layer: object, xy: object, samplingRegion?: boolean): number;
   module MSI_summary {
      /**
        * @param qcut default value Is ``0.75``.
        * @param TrIQ default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function scaleMax(data: object, intensity: object, qcut?: number, TrIQ?: boolean, env?: object): number;
   }
   /**
    * get MSI pixels layer via given ``m/z`` value.
    * 
    * 
     * @param viewer -
     * @param mz -
     * @param tolerance -
     * 
     * + default value Is ``'da:0.1'``.
     * @param split returns a single layer object for multiple input m/z
     *  vector if not split by default, otherwise returns 
     *  multiple layer objects in a list for each corresponding 
     *  ion m/z if split parameter value is set to TRUE.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function MSIlayer(viewer: object, mz: number, tolerance?: any, split?: boolean, env?: object): object;
   /**
    * get a pixel data
    * 
    * 
     * @param data -
     * @param x -
     * @param y -
     * @param env 
     * + default value Is ``null``.
   */
   function pixel(data: any, x: object, y: object, env?: object): object;
   module read {
      /**
       * open the existed mzImage cache file
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzImage(file: any, env?: object): object;
   }
   /**
    * MS-imaging of the MSI summary data result.
    * 
    * 
     * @param data 1. @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.MSISummary``
     *  2. @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.SingleIonLayer``
     * @param intensity -
     * 
     * + default value Is ``null``.
     * @param colorSet @``T:Microsoft.VisualBasic.Imaging.Drawing2D.Colors.ScalerPalette``
     * 
     * + default value Is ``'viridis:turbo'``.
     * @param defaultFill -
     * 
     * + default value Is ``'Transparent'``.
     * @param pixelSize -
     * 
     * + default value Is ``'6,6'``.
     * @param filter 
     * + default value Is ``null``.
     * @param background all of the pixels in this index parameter data value will 
     *  be treated as background pixels and removed from the MSI 
     *  rendering.
     * 
     * + default value Is ``null``.
     * @param size do size overrides, default parameter value nothing means the
     *  size is evaluated based on the dimension **`dims`** 
     *  of the ms-imaging raw data and the **`pixelSize`**
     * 
     * + default value Is ``null``.
     * @param colorLevels 
     * + default value Is ``255``.
     * @param dims 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function render(data: any, intensity?: object, colorSet?: string, defaultFill?: string, pixelSize?: any, filter?: object, background?: string, size?: any, colorLevels?: object, dims?: any, env?: object): object;
   /**
    * rendering ions MSI in (R,G,B) color channels
    * 
    * 
     * @param viewer -
     * @param r -
     * @param g -
     * @param b -
     * @param background 
     * + default value Is ``'black'``.
     * @param tolerance -
     * 
     * + default value Is ``'da:0.1'``.
     * @param maxCut 
     * + default value Is ``0.75``.
     * @param TrIQ 
     * + default value Is ``true``.
     * @param pixelSize 
     * + default value Is ``[5,5]``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function rgb(viewer: object, r: number, g: number, b: number, background?: string, tolerance?: any, maxCut?: number, TrIQ?: boolean, pixelSize?: any, env?: object): object;
   module split {
      /**
       * split the ms-imaging layer into multiple parts
       * 
       * 
        * @param x -
        * @param args default is split layer into multiple sample source
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function layer(x: any, args: object, env?: object): any;
   }
   /**
    * merge multiple layers via intensity sum
    * 
    * 
     * @param layers -
     * @param tolerance 
     * + default value Is ``'da:0.1'``.
     * @param intocutoff 
     * + default value Is ``0.3``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function sum_layers(layers: any, tolerance?: any, intocutoff?: number, env?: object): any;
   /**
    * set cluster tags to the pixel tag property data
    * 
    * 
     * @param layer -
     * @param segments -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function tag_layers(layer: object, segments: any, env?: object): any;
   /**
    * Contrast optimization of mass
    *  spectrometry imaging(MSI) data
    *  visualization by threshold intensity
    *  quantization (TrIQ)
    * 
    * 
     * @param data A ms-imaging ion layer data or a numeric vector of the intensity data.
     * @param q cutoff threshold of the intensity numeric vector
     * 
     * + default value Is ``0.6``.
     * @param levels 
     * + default value Is ``100``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function TrIQ(data: any, q?: number, levels?: object, env?: object): number;
   /**
    * load imzML data into the ms-imaging render
    * 
    * > this function will load entire MSI matrix raw data into memory.
    * 
     * @param file *.imzML;*.mzPack
     * @param env 
     * + default value Is ``null``.
   */
   function viewer(file: any, env?: object): object;
   module write {
      /**
       * write mzImage data file
       * 
       * 
        * @param pixels -
        * @param file -
        * @param da 
        * + default value Is ``0.01``.
        * @param spares 
        * + default value Is ``0.2``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzImage(pixels: any, file: any, da?: number, spares?: number, env?: object): boolean;
   }
}
