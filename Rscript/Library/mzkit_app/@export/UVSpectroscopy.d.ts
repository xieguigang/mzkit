// export R# package module type define for javascript/typescript language
//
// ref=mzkit.UVSpectroscopy@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * helper package module for read ``electromagnetic radiation spectrum`` data
 * 
*/
declare namespace UVSpectroscopy {
   /**
    * Get photodiode array detector instrument configuration id
    * 
    * 
     * @param mzml -
   */
   function get_instrument(mzml:string): string;
   /**
     * @param env default value Is ``null``.
   */
   function extract_UVsignals(rawscans:any, instrumentId:string, env?:object): object;
   module as {
      /**
        * @param rawfile default value Is ``'UVraw'``.
        * @param env default value Is ``null``.
      */
      function UVtime_signals(rawscans:any, rawfile?:string, env?:object): any;
   }
   module write {
      /**
       * write UV signal data into a text file or netCDF4 data file
       * 
       * 
        * @param signals a vector or pipeline of @``T:Microsoft.VisualBasic.Math.SignalProcessing.GeneralSignal``
        * @param file the file path of the data file that will be write signal data to it.
        * @param enable_CDFextension only available for sciBASIC.NET product when this option is set to ``TRUE``. not supports for the 
        *  standard netcdf library on linux platform or other cdf file reader software like NASA Panoply, etc.
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function UVsignals(signals:any, file:string, enable_CDFextension?:boolean, env?:object): boolean;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function UVsignals(file:any, env?:object): any;
   }
}
