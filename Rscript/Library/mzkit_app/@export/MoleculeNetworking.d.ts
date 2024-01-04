// export R# package module type define for javascript/typescript language
//
//    imports "MoleculeNetworking" from "mzDIA";
//
// ref=mzkit.MoleculeNetworking@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace MoleculeNetworking {
   module as {
      /**
      */
      function graph(tree: object, ions: object): object;
   }
   /**
     * @param mzdiff1 default value Is ``'da:0.1'``.
     * @param mzdiff2 default value Is ``'da:0.3'``.
     * @param intocutoff default value Is ``0.05``.
     * @param tree_identical default value Is ``0.8``.
     * @param tree_right default value Is ``0.01``.
     * @param env default value Is ``null``.
   */
   function clustering(ions: any, mzdiff1?: any, mzdiff2?: any, intocutoff?: number, tree_identical?: number, tree_right?: number, env?: object): object;
   /**
   */
   function msBin(tree: object, ions: object): object;
   /**
     * @param mzdiff default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function representative(tree: object, mzdiff?: any, env?: object): object;
   /**
     * @param rtwin default value Is ``30``.
     * @param wrap_peaks default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function splitClusterRT(clusters: any, rtwin?: number, wrap_peaks?: boolean, env?: object): object|object;
   /**
     * @param mzdiff default value Is ``0.3``.
     * @param intocutoff default value Is ``0.05``.
     * @param equals default value Is ``0.85``.
   */
   function tree(ions: object, mzdiff?: number, intocutoff?: number, equals?: number): object;
   /**
   */
   function uniqueNames(ions: object): object;
}
