// export R# package module type define for javascript/typescript language
//
//    imports "SingleCells" from "mzkit";
//
// ref=mzkit.SingleCells@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace SingleCells {
   module apply {
      /**
        * @param env default value Is ``null``.
      */
      function scale(x: any, scaler: object, env?: object): object;
   }
   module as {
      /**
        * @param single_cell default value Is ``false``.
      */
      function expression(x: object, single_cell?: boolean): object;
   }
   /**
     * @param mzdiff default value Is ``0.005``.
     * @param freq default value Is ``0.001``.
     * @param env default value Is ``null``.
   */
   function cell_matrix(raw: object, mzdiff?: number, freq?: number, env?: object): object;
   module df {
      /**
      */
      function mz_matrix(x: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function mz_matrix(x: any, args: object, env?: object): object;
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function matrix(file: any, env?: object): object;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function mz_matrix(file: any, env?: object): object;
   }
   /**
     * @param da default value Is ``0.01``.
     * @param parallel default value Is ``true``.
   */
   function SCM_ionStat(raw: object, da?: number, parallel?: boolean): object;
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function matrix(x: object, file: any, env?: object): boolean;
   }
}
