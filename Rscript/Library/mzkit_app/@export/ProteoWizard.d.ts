// export R# package module type define for javascript/typescript language
//
//    imports "ProteoWizard" from "mzkit";
//
// ref=mzkit.ProteoWizard@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace ProteoWizard {
   module convert {
      module thermo {
         /**
           * @param filetype default value Is ``null``.
           * @param filters default value Is ``null``.
           * @param parallel default value Is ``false``.
           * @param env default value Is ``null``.
         */
         function raw(raw: string, output: string, filetype?: object, filters?: object, parallel?: any, env?: object): any;
      }
   }
   module filter {
      /**
      */
      function msLevel(level: string): object;
      /**
      */
      function scanTime(start: number, stop: number): object;
   }
   module MRM {
      /**
        * @param output default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function mzML(wiff: string, output?: string, env?: object): any;
   }
   module msconvert {
      /**
        * @param env default value Is ``null``.
      */
      function ready(env?: object): boolean;
   }
}
