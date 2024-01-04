// export R# package module type define for javascript/typescript language
//
//    imports "MSI" from "mzkit";
//
// ref=mzkit.MSI@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace MSI {
   module as {
      /**
        * @param context default value Is ``'MSIlayer'``.
        * @param dims default value Is ``null``.
        * @param strict default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function layer(x: any, context?: any, dims?: any, strict?: boolean, env?: object): object;
   }
   /**
   */
   function basePeakMz(summary: object): object;
   module cast {
      /**
        * @param mzdiff default value Is ``0.01``.
        * @param dims default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function spatial_layers(x: object, mzdiff?: number, dims?: any, env?: object): object;
   }
   /**
     * @param hasMs2 default value Is ``false``.
   */
   function correction(totalTime: number, pixels: object, hasMs2?: boolean): object;
   /**
     * @param dims default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function dimension_size(raw: object, dims?: any, env?: object): any;
   /**
     * @param mzdiff default value Is ``0.001``.
     * @param q default value Is ``0.001``.
     * @param fast_bins default value Is ``true``.
   */
   function getMatrixIons(raw: object, mzdiff?: number, q?: number, fast_bins?: boolean): number;
   /**
     * @param env default value Is ``null``.
   */
   function ions_jointmatrix(raw: object, env?: object): object;
   /**
     * @param grid_size default value Is ``5``.
     * @param da default value Is ``0.01``.
     * @param parallel default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function ionStat(raw: any, grid_size?: object, da?: number, parallel?: boolean, env?: object): object;
   module levels {
      /**
        * @param clusters default value Is ``6``.
        * @param win_size default value Is ``3``.
      */
      function convolution(mat: object, clusters?: object, win_size?: object): object;
   }
   /**
   */
   function moran_I(x: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function msi_metadata(raw: any, env?: object): object;
   /**
     * @param x default value Is ``null``.
     * @param y default value Is ``null``.
     * @param as_vector default value Is ``false``.
     * @param dims default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function MSI_summary(raw: object, x?: object, y?: object, as_vector?: boolean, dims?: any, env?: object): object|object;
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function imzML(file: string, env?: object): object;
   }
   /**
     * @param dims default value Is ``null``.
     * @param res default value Is ``17``.
     * @param noise_cutoff default value Is ``1``.
     * @param source_tag default value Is ``'pack_matrix'``.
     * @param env default value Is ``null``.
   */
   function pack_matrix(file: any, dims?: any, res?: number, noise_cutoff?: number, source_tag?: string, env?: object): any;
   /**
     * @param topN default value Is ``3``.
     * @param mzError default value Is ``'da:0.05'``.
     * @param ionSet default value Is ``null``.
     * @param raw_matrix default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function peakMatrix(raw: object, topN?: object, mzError?: any, ionSet?: any, raw_matrix?: boolean, env?: object): object|object;
   /**
     * @param resolution default value Is ``100``.
     * @param mzError default value Is ``'da:0.05'``.
     * @param cutoff default value Is ``0.05``.
     * @param env default value Is ``null``.
   */
   function peakSamples(raw: object, resolution?: object, mzError?: any, cutoff?: number, env?: object): any;
   /**
     * @param tolerance default value Is ``'da:0.1'``.
     * @param env default value Is ``null``.
   */
   function pixelId(raw: object, mz: number, tolerance?: any, env?: object): string;
   /**
   */
   function pixelIons(raw: object): object;
   /**
     * @param file default value Is ``null``.
     * @param mzdiff default value Is ``0.001``.
     * @param q default value Is ``0.01``.
     * @param fast_bin default value Is ``true``.
     * @param verbose default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function pixelMatrix(raw: object, file?: any, mzdiff?: number, q?: number, fast_bin?: boolean, verbose?: boolean, env?: object): boolean|object;
   /**
     * @param count default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function pixels(file: any, count?: boolean, env?: object): object;
   module row {
      /**
        * @param y default value Is ``0``.
        * @param correction default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function scans(raw: string, y?: object, correction?: object, env?: object): object;
   }
   /**
     * @param n default value Is ``32``.
     * @param coverage default value Is ``0.3``.
   */
   function sample_bootstraping(layer: object, tissue: object, n?: object, coverage?: number): any;
   /**
     * @param bpc default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function scale(m: object, factor: any, bpc?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function scanMatrix(rowScans: any, env?: object): object;
   /**
     * @param correction default value Is ``null``.
     * @param intocutoff default value Is ``0.05``.
     * @param yscale default value Is ``1``.
     * @param env default value Is ``null``.
   */
   function scans2D(rowScans: any, correction?: object, intocutoff?: number, yscale?: number, env?: object): any;
   module spatial {
      /**
        * @param win_size default value Is ``2``.
        * @param steps default value Is ``1``.
      */
      function convolution(mat: object, win_size?: object, steps?: object): object;
   }
   /**
     * @param partition default value Is ``5``.
   */
   function splice(raw: object, partition?: object): object;
   module write {
      /**
        * @param res default value Is ``17``.
        * @param ionMode default value Is ``null``.
        * @param dims default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function imzML(mzpack: object, file: string, res?: number, ionMode?: object, dims?: any, env?: object): boolean;
   }
}
