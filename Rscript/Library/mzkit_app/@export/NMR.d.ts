// export R# package module type define for javascript/typescript language
//
//    imports "NMR" from "mzplot";
//
// ref=mzkit.plotNMR@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace NMR {
   /**
     * @param size default value Is ``'3600,2400'``.
     * @param padding default value Is ``'padding: 200px 400px 300px 100px'``.
     * @param env default value Is ``null``.
   */
   function plot_nmr(nmr: object, size?: any, padding?: any, env?: object): any;
}
