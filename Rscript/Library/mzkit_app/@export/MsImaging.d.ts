// export R# package module type define for javascript/typescript language
//
// ref=mzkit.MsImaging@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace MsImaging {
   /**
     * @param q default value Is ``0.6``.
     * @param levels default value Is ``100``.
     * @param env default value Is ``null``.
   */
   function TrIQ(data: any, q?: number, levels?: object, env?: object): number;
   /**
     * @param min default value Is ``0``.
   */
   function intensityLimits(data: object, max: number, min?: number): object;
   module write {
      /**
        * @param da default value Is ``0.01``.
        * @param spares default value Is ``0.2``.
        * @param env default value Is ``null``.
      */
      function mzImage(pixels: any, file: any, da?: number, spares?: number, env?: object): boolean;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function mzImage(file: any, env?: object): object;
   }
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param title default value Is ``'FilterMz'``.
     * @param env default value Is ``null``.
   */
   function FilterMz(viewer: object, mz: number, tolerance?: any, title?: string, env?: object): object;
   /**
     * @param tolerance default value Is ``'da:0.1'``.
     * @param threshold default value Is ``0.01``.
     * @param env default value Is ``null``.
   */
   function MS1(viewer: object, x: object, y: object, tolerance?: any, threshold?: number, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function viewer(file: any, env?: object): object;
   /**
   */
   function pixel(data: object, x: object, y: object): object;
   /**
     * @param tolerance default value Is ``'da:0.1'``.
     * @param skip_zero default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function ionLayers(imzML: any, mz: number, tolerance?: any, skip_zero?: boolean, env?: object): object;
   /**
     * @param background default value Is ``'black'``.
     * @param tolerance default value Is ``'da:0.1'``.
     * @param maxCut default value Is ``0.75``.
     * @param TrIQ default value Is ``true``.
     * @param pixelSize default value Is ``[5,5]``.
     * @param env default value Is ``null``.
   */
   function rgb(viewer: object, r: number, g: number, b: number, background?: string, tolerance?: any, maxCut?: number, TrIQ?: boolean, pixelSize?: any, env?: object): object;
   /**
     * @param tolerance default value Is ``'da:0.1'``.
     * @param env default value Is ``null``.
   */
   function MSIlayer(viewer: object, mz: number, tolerance?: any, env?: object): object;
   /**
     * @param summary default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function intensity(layer: any, summary?: object, env?: object): number;
   /**
     * @param gridSize default value Is ``3``.
     * @param q default value Is ``0.8``.
     * @param env default value Is ``null``.
   */
   function knnFill(layer: any, gridSize?: object, q?: number, env?: object): object|object;
   /**
     * @param samplingRegion default value Is ``true``.
   */
   function MSI_coverage(layer: object, xy: object, samplingRegion?: boolean): number;
   /**
     * @param cutoff default value Is ``0.8``.
     * @param samplingRegion default value Is ``true``.
   */
   function assert(layer: object, xy: object, cutoff?: number, samplingRegion?: boolean): boolean;
   /**
     * @param pixelSize default value Is ``'5,5'``.
     * @param tolerance default value Is ``'da:0.1'``.
     * @param color default value Is ``'viridis:turbo'``.
     * @param levels default value Is ``30``.
     * @param cutoff default value Is ``[0.1,0.75]``.
     * @param background default value Is ``'Transparent'``.
     * @param env default value Is ``null``.
   */
   function layer(viewer: object, mz: number, pixelSize?: any, tolerance?: any, color?: string, levels?: object, cutoff?: any, background?: any, env?: object): object;
   module MSI_summary {
      /**
        * @param qcut default value Is ``0.75``.
        * @param TrIQ default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function scaleMax(data: object, intensity: object, qcut?: number, TrIQ?: boolean, env?: object): number;
   }
   /**
   */
   function defaultFilter(): object;
   /**
     * @param intensity default value Is ``null``.
     * @param colorSet default value Is ``'viridis:turbo'``.
     * @param defaultFill default value Is ``'Transparent'``.
     * @param pixelSize default value Is ``'6,6'``.
     * @param filter default value Is ``null``.
     * @param background default value Is ``null``.
     * @param size default value Is ``null``.
     * @param colorLevels default value Is ``255``.
     * @param dims default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function render(data: any, intensity?: object, colorSet?: string, defaultFill?: string, pixelSize?: any, filter?: object, background?: string, size?: any, colorLevels?: object, dims?: any, env?: object): object;
   module as {
      /**
        * @param character default value Is ``true``.
      */
      function pixels(layer: object, character?: boolean): string|object;
   }
   /**
     * @param gridSize default value Is ``6``.
     * @param mzdiff default value Is ``'da:0.1'``.
     * @param keepsLayer default value Is ``false``.
     * @param densityCut default value Is ``0.1``.
     * @param qcut default value Is ``0.01``.
     * @param intoCut default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function MeasureMSIions(raw: object, gridSize?: object, mzdiff?: any, keepsLayer?: boolean, densityCut?: number, qcut?: number, intoCut?: number, env?: object): number|object;
}
