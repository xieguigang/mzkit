// export R# package module type define for javascript/typescript language
//
// ref=mzkit.ProteoWizard@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ProteoWizard helper
 *  
 *  You should config the bin path of ProteoWizard at first 
 *  by using ``options`` function
 * 
*/
declare namespace ProteoWizard {
   module convert {
      module thermo {
         /**
          * Convert thermo raw files to mzXML files
          * 
          * 
           * @param raw -
           * @param output The output directory
           * @param filetype the file type of the raw data output.
           * 
           * + default value Is ``null``.
           * @param filters data filters
           * 
           * + default value Is ``null``.
           * @param parallel 
           * + default value Is ``false``.
           * @param env 
           * + default value Is ``null``.
         */
         function raw(raw: string, output: string, filetype?: object, filters?: object, parallel?: any, env?: object): any;
      }
   }
   module filter {
      /**
      */
      function msLevel(level: string): object;
      /**
       * 
       * 
        * @param start Start time in time unit of seconds
        * @param stop Stop time in time unit of seconds
      */
      function scanTime(start: number, stop: number): object;
   }
   module MRM {
      /**
       * Convert MRM wiff file to mzMl files
       * 
       * 
        * @param wiff The file path of the wiff file
        * @param output 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
        * @return File path collection of the converted mzML files.
      */
      function mzML(wiff: string, output?: string, env?: object): any;
   }
   module msconvert {
      /**
       * Is the ``ProteoWizard`` program ready to use?
       * 
       * 
        * @param env 
        * + default value Is ``null``.
      */
      function ready(env?: object): boolean;
   }
}
