// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Tissue@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace tissue {
   /**
     * @param colors default value Is ``null``.
     * @param gridSize default value Is ``25``.
     * @param tolerance default value Is ``15``.
     * @param densityGrid default value Is ``5``.
   */
   function scan_tissue(tissue: object, colors?: string, gridSize?: object, tolerance?: object, densityGrid?: object): object;
   /**
     * @param heatmap default value Is ``null``.
     * @param target default value Is ``'black'``.
   */
   function heatmap_layer(tissue: object, heatmap?: object, target?: string): object;
   /**
     * @param nbags default value Is ``300``.
     * @param nsamples default value Is ``32``.
   */
   function RSD(layer: object, nbags?: object, nsamples?: object): number;
}
