// export R# package module type define for javascript/typescript language
//
//    imports "z_assembler" from "mzkit";
//
// ref=mzkit.z_assembler@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * package for processing the ms-imaging 3D data assembly
 * 
*/
declare namespace z_assembler {
   /**
    * Create mzpack object for ms-imaging in 3D
    *  
    *  this function assembling a collection of the 2D layer in z-axis
    *  order for construct a new 3D volume data.
    * 
    * > a @``T:BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.MzMatrix`` object will be packed as the 3D volumn result.
    * 
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function z_assembler(header: object, file: any, env?: object): object;
   /**
    * create the volume file header data
    * 
    * 
     * @param features the ion features m/z vector
     * @param mzdiff -
     * 
     * + default value Is ``0.001``.
   */
   function z_header(features: any, mzdiff?: number): object;
   /**
    * create a simple 3d volume model for mzkit workbench
    * 
    * > this function works for combine a collection of the 2D layer as the 3d volume @``T:BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.MzMatrix`` data
    * 
     * @param layers should be a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.MsImaging.SingleIonLayer``. 
     *  the layer elements in this collection should be already been re-ordered by 
     *  the z-axis!
     * @param dump -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function z_volume(layers: any, dump: string, env?: object): any;
}
