// export R# package module type define for javascript/typescript language
//
//    imports "visualPlots" from "mz_quantify";
//
// ref=mzkit.Visual@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Visual plot of the quantification data
 * 
*/
declare namespace visualPlots {
   module chromatogram {
      /**
       * plot MRM chromatogram overlaps in a speicifc rawdata file
       * 
       * 
        * @param mzML a specific MRM rawdata file
        * @param ions -
        * @param labelLayoutTicks -
        * 
        * + default value Is ``2000``.
      */
      function plot(mzML: string, ions: object, labelLayoutTicks?: object): object;
   }
   module MRM {
      module chromatogramPeaks {
         /**
          * Visualization plot of the MRM chromatogram peaks data.
          * 
          * 
           * @param chromatogram the extract MRM chromatogram peaks data.
           * @param title the plot title
           * 
           * + default value Is ``'MRM Chromatogram Peak Plot'``.
           * @param size the size of the output image
           * 
           * + default value Is ``'2200,1600'``.
           * @param padding 
           * + default value Is ``'padding: 200px 80px 150px 200px'``.
           * @param fill fill polygon of the TIC plot?
           * 
           * + default value Is ``true``.
           * @param gridFill color value for fill the grid background
           * 
           * + default value Is ``'rgb(250,250,250)'``.
           * @param lineStyle Css value for adjust the plot style of the curve line of the chromatogram peaks data.
           * 
           * + default value Is ``'stroke: black; stroke-width: 2px; stroke-dash: solid;'``.
           * @param ROI 
           * + default value Is ``null``.
           * @param relativeTimeScale -
           * 
           * + default value Is ``null``.
           * @param showAccumulateLine 
           * + default value Is ``false``.
           * @param parallel 
           * + default value Is ``false``.
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function plot(chromatogram: any, title?: string, size?: any, padding?: any, fill?: boolean, gridFill?: string, lineStyle?: string, ROI?: object, relativeTimeScale?: any, showAccumulateLine?: boolean, parallel?: boolean, env?: object): object;
      }
   }
   /**
    * Draw standard curve
    * 
    * 
     * @param model The linear model of the targeted metabolism model data.
     * @param title The plot title
     * 
     * + default value Is ``''``.
     * @param samples The point data of samples
     * 
     * + default value Is ``null``.
     * @param size 
     * + default value Is ``'1600,1200'``.
     * @param margin 
     * + default value Is ``'padding: 200px 100px 150px 150px'``.
     * @param factorFormat 
     * + default value Is ``'G4'``.
     * @param sampleLabelFont 
     * + default value Is ``'font-style: normal; font-size: 16; font-family: Segoe UI;'``.
     * @param labelerIterations 
     * + default value Is ``1000``.
     * @param gridFill 
     * + default value Is ``'LightGray'``.
     * @param reverse 
     * + default value Is ``false``.
   */
   function standard_curve(model: object, title?: string, samples?: object, size?: string, margin?: string, factorFormat?: string, sampleLabelFont?: string, labelerIterations?: object, gridFill?: string, reverse?: boolean): object;
}
