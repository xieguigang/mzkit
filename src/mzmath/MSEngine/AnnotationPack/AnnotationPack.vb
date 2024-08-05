
Imports BioNovoGene.Analytical.MassSpectrometry.Math

''' <summary>
''' Result data pack for save the annotation result data
''' </summary>
''' <remarks>
''' data export for internal annotation workflow, handling to customer report and view on mzkit workbench.
''' </remarks>
Public Class AnnotationPack

    ''' <summary>
    ''' the ms2 spectrum alignment search hits
    ''' </summary>
    ''' <returns></returns>
    Public Property libraries As Dictionary(Of String, AlignmentHit())

    ''' <summary>
    ''' the ms1 peaktable
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()

End Class
