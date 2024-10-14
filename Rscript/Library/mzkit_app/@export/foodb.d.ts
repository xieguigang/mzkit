// export R# package module type define for javascript/typescript language
//
//    imports "foodb" from "mzkit";
//
// ref=mzkit.foodbTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace foodb {
   /**
     * @param env default value Is ``null``.
   */
   function loadFoods(dir: string, env?: object): any;
}
