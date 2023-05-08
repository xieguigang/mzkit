// export R# package module type define for javascript/typescript language
//
// ref=mzkit.ChemicalDraw@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace ChemicalDraw {
   /**
   */
   function KCFDraw(molecule:object): object;
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function kcf(chemical:any, env?:object): object;
   }
}
