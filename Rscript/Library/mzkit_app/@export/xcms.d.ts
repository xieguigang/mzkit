﻿// export R# package module type define for javascript/typescript language
//
//    imports "xcms" from "mz_quantify";
//
// ref=mzkit.xcms@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the xcms interop and data handler
 * 
*/
declare namespace xcms {
   /**
     * @param group_features default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function parse_xcms_samples(file: any, group_features?: boolean, env?: object): object|object;
}
