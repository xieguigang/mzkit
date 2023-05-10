// export R# package module type define for javascript/typescript language
//
// ref=mzkit.ms2_simulator@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mzkit.simulator {
   module read {
      /**
      */
      function kcf(file: string): object;
   }
   module molecular {
      /**
        * @param verbose default value Is ``false``.
      */
      function graph(mol: object, verbose?: boolean): object;
   }
   module energy {
      /**
      */
      function range(mol: object): object;
      /**
        * @param max default value Is ``1000``.
      */
      function normal(mu: number, delta: number, max?: number): object;
      /**
        * @param env default value Is ``null``.
      */
      function custom(fun: object, max: number, env?: object): object;
   }
   /**
     * @param nIntervals default value Is ``1000``.
     * @param precision default value Is ``4``.
   */
   function fragmentation(mol: object, energy: object, nIntervals?: object, precision?: object): object;
   module write {
      /**
      */
      function mgf(fragments: object, file: string): boolean;
   }
}
