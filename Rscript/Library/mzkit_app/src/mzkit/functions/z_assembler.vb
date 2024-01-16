Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

Public Class z_assembler_func : Inherits RDefaultFunction

    Public Property assembler As ZAssembler

    <RDefaultFunction>
    Public Function addLayer(layer As IMZPack, z As Integer, Optional env As Environment = Nothing) As Object
        Call assembler.Write2DLayer(layer, z)
        Return Nothing
    End Function

End Class
