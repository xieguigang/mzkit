// export R# package module type define for javascript/typescript language
//
//    imports "ThermoRaw" from "mzkit";
//
// ref=mzkit.ThermoRaw@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace ThermoRaw {
   /**
   */
   function events(scan: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function load_MSI(raw: object, pixels: any, env?: object): object;
   /**
   */
   function logs(scan: object): object;
   /**
   */
   function MSI_pixels(mzpack: object): object;
   module open {
      /**
       * open a Thermo raw file
       * 
       * 
        * @param rawfile the file path of the ``*.raw``.
      */
      function raw(rawfile: string): object;
   }
   module read {
      /**
      */
      function rawscan(raw: object, scanId: object): object;
   }
}
