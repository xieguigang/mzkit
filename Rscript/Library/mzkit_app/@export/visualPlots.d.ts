// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Visual@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace visualPlots {
   module as {
      /**
        * @param time default value Is ``'Time'``.
        * @param into default value Is ``'Intensity'``.
        * @param env default value Is ``null``.
      */
      function chromatogram(data: any, time?: string, into?: string, env?: object): object;
   }
   /**
     * @param title default value Is ``''``.
     * @param samples default value Is ``null``.
     * @param size default value Is ``'1600,1200'``.
     * @param margin default value Is ``'padding: 200px 100px 150px 150px'``.
     * @param factorFormat default value Is ``'G4'``.
     * @param sampleLabelFont default value Is ``'font-style: normal; font-size: 16; font-family: Segoe UI;'``.
     * @param labelerIterations default value Is ``1000``.
     * @param gridFill default value Is ``'LightGray'``.
   */
   function standard_curve(model: object, title?: string, samples?: object, size?: string, margin?: string, factorFormat?: string, sampleLabelFont?: string, labelerIterations?: object, gridFill?: string): object;
   module chromatogram {
      /**
        * @param labelLayoutTicks default value Is ``2000``.
      */
      function plot(mzML: string, ions: object, labelLayoutTicks?: object): object;
   }
   module MRM {
      module chromatogramPeaks {
         /**
           * @param title default value Is ``'MRM Chromatogram Peak Plot'``.
           * @param size default value Is ``'2200,1600'``.
           * @param padding default value Is ``'padding: 200px 80px 150px 200px'``.
           * @param fill default value Is ``true``.
           * @param gridFill default value Is ``'rgb(250,250,250)'``.
           * @param lineStyle default value Is ``'stroke: black; stroke-width: 2px; stroke-dash: solid;'``.
           * @param relativeTimeScale default value Is ``null``.
           * @param parallel default value Is ``false``.
           * @param env default value Is ``null``.
         */
         function plot(chromatogram: any, title?: string, size?: any, padding?: any, fill?: boolean, gridFill?: string, lineStyle?: string, relativeTimeScale?: any, parallel?: boolean, env?: object): object;
      }
   }
}
