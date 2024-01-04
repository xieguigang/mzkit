// export R# package module type define for javascript/typescript language
//
//    imports "UVSpectroscopy" from "mzkit";
//
// ref=mzkit.UVSpectroscopy@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace UVSpectroscopy {
   module as {
      /**
        * @param rawfile default value Is ``'UVraw'``.
        * @param env default value Is ``null``.
      */
      function UVtime_signals(rawscans: any, rawfile?: string, env?: object): any;
   }
   /**
     * @param env default value Is ``null``.
   */
   function extract_UVsignals(rawscans: any, instrumentId: string, env?: object): object;
   /**
   */
   function get_instrument(mzml: string): string;
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function UVsignals(file: any, env?: object): any;
   }
   module write {
      /**
        * @param enable_CDFextension default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function UVsignals(signals: any, file: string, enable_CDFextension?: boolean, env?: object): boolean;
   }
}
