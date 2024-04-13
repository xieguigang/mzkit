// export R# package module type define for javascript/typescript language
//
//    imports "mzkit.simulator" from "mzDIA";
//
// ref=mzkit.ms2_simulator@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace mzkit.simulator {
   module embedded {
      /**
       * make the molecular graph embedding to vector
       * 
       * 
        * @param mol -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function graph2vector(mol: any, env?: object): any;
   }
   module molecular {
      /**
       * parse the smiles structure string as molecular network graph
       * 
       * 
        * @param mol -
        * @param verbose -
        * 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function graph(mol: any, verbose?: boolean, env?: object): object;
   }
   module read {
      /**
      */
      function kcf(file: string): object;
   }
}
