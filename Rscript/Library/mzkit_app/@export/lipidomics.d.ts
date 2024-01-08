// export R# package module type define for javascript/typescript language
//
//    imports "lipidomics" from "mzDIA";
//
// ref=mzkit.LCLipidomics@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Lipidomics annotation based on MS-DIAL
 * 
*/
declare namespace lipidomics {
   /**
    * meansrue lipid ions
    * 
    * 
     * @param lipidclass configs of the target lipid @``T:BioNovoGene.Analytical.MassSpectrometry.Lipidomics.LbmClass`` for run spectrum peaks generation
     * @param adduct -
     * @param minCarbonCount -
     * @param maxCarbonCount -
     * @param minDoubleBond -
     * @param maxDoubleBond -
     * @param maxOxygen -
     * @return a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Lipidomics.LipidIon`` data
   */
   function lipid_ions(lipidclass: object, adduct: object, minCarbonCount: object, maxCarbonCount: object, minDoubleBond: object, maxDoubleBond: object, maxOxygen: object): object;
}
