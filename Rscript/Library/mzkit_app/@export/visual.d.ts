// export R# package module type define for javascript/typescript language
//
//    imports "visual" from "mzplot";
//
// ref=mzkit.Visual@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace visual {
   /**
    * 
    * 
     * @param GCxGC -
     * @param metabolites [name, rt1, rt2]
     * @param rt_width 
     * + default value Is ``'60,0.5'``.
     * @param space 
     * + default value Is ``'5,5'``.
     * @param size 
     * + default value Is ``'3600,2100'``.
     * @param padding 
     * + default value Is ``'padding: 200px 600px 250px 250px;'``.
     * @param colorSet 
     * + default value Is ``'viridis:turbo'``.
     * @param mapLevels 
     * + default value Is ``64``.
     * @param labelStyle 
     * + default value Is ``'font-style: normal; font-size: 16; font-family: Bookman Old Style;'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function gcxgc_heatmap(GCxGC: object, metabolites: object, rt_width?: any, space?: any, size?: any, padding?: any, colorSet?: string, mapLevels?: object, labelStyle?: string, env?: object): any;
   module mass_spectrum {
      /**
       * Plot of the mass spectrum
       * 
       * 
        * @param spectrum the ms spectrum object, this parameter can be a collection 
        *  of ms2 object model, or else is a library matrix or peak 
        *  ms2 model object, or else is a mgf ion object, or else a 
        *  dataframe with columns ``mz`` and ``into``.
        * @param alignment -
        * 
        * + default value Is ``null``.
        * @param title the main title that display on the chart plot
        * 
        * + default value Is ``'Mass Spectrum Plot'``.
        * @param showLegend 
        * + default value Is ``true``.
        * @param showGrid 
        * + default value Is ``true``.
        * @param tagXFormat 
        * + default value Is ``'F2'``.
        * @param intoCutoff 
        * + default value Is ``0.3``.
        * @param bar_width the column width of the bar plot
        * 
        * + default value Is ``8``.
        * @param legend_layout the layout of the legend plot, this parameter value could affects the plot style
        * 
        * + default value Is ``["top-right","title","bottom"]``.
        * @param env 
        * + default value Is ``null``.
      */
      function plot(spectrum: any, alignment?: any, title?: string, showLegend?: boolean, showGrid?: boolean, tagXFormat?: string, intoCutoff?: number, bar_width?: number, legend_layout?: any, env?: object): object;
   }
   module plot {
      /**
       * visual of the UV spectrum
       * 
       * 
        * @param timeSignals -
        * @param is_spectrum -
        * 
        * + default value Is ``false``.
        * @param size -
        * 
        * + default value Is ``'1600,1200'``.
        * @param padding -
        * 
        * + default value Is ``'padding:100px 100px 100px 100px;'``.
        * @param colorSet -
        * 
        * + default value Is ``'Set1:c8'``.
        * @param pt_size -
        * 
        * + default value Is ``8``.
        * @param line_width -
        * 
        * + default value Is ``5``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function UV_signals(timeSignals: any, is_spectrum?: boolean, size?: any, padding?: any, colorSet?: string, pt_size?: number, line_width?: number, env?: object): object;
   }
   /**
    * Plot cluster size histogram on RT dimension
    * 
    * 
     * @param mn the molecular networking graph result
     * @param size -
     * 
     * + default value Is ``'2700,2000'``.
     * @param padding -
     * 
     * + default value Is ``'padding:100px 300px 200px 200px;'``.
     * @param dpi -
     * 
     * + default value Is ``300``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function plotNetworkClusterHistogram(mn: object, size?: any, padding?: any, dpi?: object, env?: object): any;
   /**
    * plot raw scatter matrix based on a given sequence of ms1 scans data
    * 
    * 
     * @param ms1_scans a sequence of ms1 scan data or a mzpack data object.
     * @param colorSet 
     * + default value Is ``'darkblue,blue,skyblue,green,orange,red,darkred'``.
     * @param contour 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function raw_scatter(ms1_scans: any, colorSet?: any, contour?: boolean, env?: object): any;
   /**
    * plot raw XIC matrix based on a given sequence of ms1 scans data
    * 
    * 
     * @param ms1_scans all ms1 scan point data for create XIC overlaps
     * @param mzwidth mz tolerance for create XIC data
     * 
     * + default value Is ``'da:0.3'``.
     * @param noise_cutoff 
     * + default value Is ``0.5``.
     * @param size 
     * + default value Is ``'1920,1200'``.
     * @param colors 
     * + default value Is ``'paper'``.
     * @param show_legends 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function raw_snapshot3D(ms1_scans: any, mzwidth?: any, noise_cutoff?: number, size?: any, colors?: any, show_legends?: boolean, env?: object): any;
}
