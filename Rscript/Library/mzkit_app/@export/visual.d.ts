// export R# package module type define for javascript/typescript language
//
//    imports "visual" from "mzplot";
//
// ref=mzkit.Visual@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace visual {
   /**
     * @param rt_width default value Is ``'60,0.5'``.
     * @param space default value Is ``'5,5'``.
     * @param size default value Is ``'3600,2100'``.
     * @param padding default value Is ``'padding: 200px 600px 250px 250px;'``.
     * @param colorSet default value Is ``'viridis:turbo'``.
     * @param mapLevels default value Is ``64``.
     * @param labelStyle default value Is ``'font-style: normal; font-size: 16; font-family: Bookman Old Style;'``.
     * @param env default value Is ``null``.
   */
   function gcxgc_heatmap(GCxGC: object, metabolites: object, rt_width?: any, space?: any, size?: any, padding?: any, colorSet?: string, mapLevels?: object, labelStyle?: string, env?: object): any;
   module mass_spectrum {
      /**
        * @param alignment default value Is ``null``.
        * @param title default value Is ``'Mass Spectrum Plot'``.
        * @param showLegend default value Is ``true``.
        * @param showGrid default value Is ``true``.
        * @param tagXFormat default value Is ``'F2'``.
        * @param intoCutoff default value Is ``0.3``.
        * @param bar_width default value Is ``8``.
        * @param legend_layout default value Is ``["top-right","title","bottom"]``.
        * @param env default value Is ``null``.
      */
      function plot(spectrum: any, alignment?: any, title?: string, showLegend?: boolean, showGrid?: boolean, tagXFormat?: string, intoCutoff?: number, bar_width?: number, legend_layout?: any, env?: object): object;
   }
   module plot {
      /**
        * @param is_spectrum default value Is ``false``.
        * @param size default value Is ``'1600,1200'``.
        * @param padding default value Is ``'padding:100px 100px 100px 100px;'``.
        * @param colorSet default value Is ``'Set1:c8'``.
        * @param pt_size default value Is ``8``.
        * @param line_width default value Is ``5``.
        * @param env default value Is ``null``.
      */
      function UV_signals(timeSignals: any, is_spectrum?: boolean, size?: any, padding?: any, colorSet?: string, pt_size?: number, line_width?: number, env?: object): object;
   }
   /**
     * @param size default value Is ``'2700,2000'``.
     * @param padding default value Is ``'padding:100px 300px 200px 200px;'``.
     * @param dpi default value Is ``300``.
     * @param env default value Is ``null``.
   */
   function plotNetworkClusterHistogram(mn: object, size?: any, padding?: any, dpi?: object, env?: object): any;
   /**
     * @param colorSet default value Is ``'darkblue,blue,skyblue,green,orange,red,darkred'``.
     * @param contour default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function raw_scatter(ms1_scans: any, colorSet?: any, contour?: boolean, env?: object): any;
   /**
     * @param mzwidth default value Is ``'da:0.3'``.
     * @param noise_cutoff default value Is ``0.5``.
     * @param size default value Is ``'1600,1200'``.
     * @param env default value Is ``null``.
   */
   function raw_snapshot3D(ms1_scans: any, mzwidth?: any, noise_cutoff?: number, size?: any, env?: object): any;
}
