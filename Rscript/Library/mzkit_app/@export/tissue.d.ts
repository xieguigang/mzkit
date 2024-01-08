// export R# package module type define for javascript/typescript language
//
//    imports "tissue" from "mzplot";
//
// ref=mzkit.Tissue@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for HE-stain image analysis
 *  
 *  Pathology is practiced by visual inspection of histochemically stained tissue slides. 
 *  While the hematoxylin and eosin (H&E) stain is most commonly used, special stains can
 *  provide additional contrast to different tissue components.
 *  
 *  Histological analysis of stained human tissue samples is the gold standard for evaluation 
 *  of many diseases, as the fundamental basis of any pathologic evaluation is the examination
 *  of histologically stained tissue affixed on a glass slide using either a microscope or 
 *  a digitized version of the histologic image following the image capture by a whole slide
 *  image (WSI) scanner. The histological staining step is a critical part of the pathology
 *  workflow and is required to provide contrast and color to tissue by facilitating a chromatic 
 *  distinction among different tissue constituents. The most common stain (otherwise referred 
 *  to as the routine stain) is the hematoxylin and eosin (H&E), which is applied to nearly 
 *  all clinical cases, covering ~80% of all the human tissue staining performed globally1. 
 *  The H&E stain is relatively easy to perform and is widely used across the industry. 
 *  In addition to H&E, there are a variety of other histological stains with different
 *  properties which are used by pathologists to better highlight different tissue 
 *  constituents.
 * 
*/
declare namespace tissue {
   /**
    * generates heatmap value
    *  
    *  convert a specific @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap.Layers`` channel inside the tissue 
    *  image scanning result as spatial heatmap matrix data.
    * 
    * 
     * @param tissue -
     * @param heatmap the @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap.Layers`` channel scan for the target colors 
     *  for heatmap rendering.
     * 
     * + default value Is ``null``.
     * @param target the target color channel
     * 
     * + default value Is ``'black'``.
   */
   function heatmap_layer(tissue: object, heatmap?: object, target?: string): object;
   /**
    * evaluate the spatial RSD of a specific channel
    * 
    * 
     * @param layer -
     * @param nbags -
     * 
     * + default value Is ``300``.
     * @param nsamples -
     * 
     * + default value Is ``32``.
     * @return a numeric vector of the rsd value.
   */
   function RSD(layer: object, nbags?: object, nsamples?: object): number;
   /**
    * analysis the HE-stain image by blocks
    * 
    * 
     * @param tissue the image of the HE-stain tissue image
     * @param colors target colors for run scaning
     * 
     * + default value Is ``null``.
     * @param grid_size grid size for divid the image into blocks
     * 
     * + default value Is ``25``.
     * @param tolerance the color tolerance value
     * 
     * + default value Is ``15``.
     * @param density_grid the grid size for evaluate the color density
     * 
     * + default value Is ``5``.
     * @return the collection of the image blocks analysis result
   */
   function scan_tissue(tissue: object, colors?: string, grid_size?: object, tolerance?: object, density_grid?: object): object;
}
