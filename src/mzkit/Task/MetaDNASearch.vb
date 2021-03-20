Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.Linq

Public Module MetaDNASearch

    <Extension>
    Public Sub RunDIA(raw As Raw, println As Action(Of String), ByRef output As MetaDNAResult())
        Dim metaDNA As New Algorithm(Tolerance.PPM(20), 0.4, Tolerance.DeltaMass(0.3))
        Dim mzpack As mzPack

        If Not raw.isLoaded Then
            raw = raw.LoadMzpack
        End If

        mzpack = raw.loaded

        Dim ionMode As Integer = mzpack.MS.Select(Function(a) a.products).IteratesALL.First.polarity
        Dim range As String()

        If ionMode = 1 Then
            range = {"[M]+", "[M+H]+"}
        Else
            range = {"[M]-", "[M-H]-"}
        End If

        Dim infer = metaDNA _
            .SetKeggLibrary(KEGGRepo.RequestKEGGcompounds) _
            .SetNetwork(KEGGRepo.RequestKEGGReactions) _
            .SetSearchRange(range) _
            .SetSamples(mzpack.GetMs2Peaks, autoROIid:=True) _
            .SetReportHandler(println) _
            .DIASearch() _
            .ToArray

        output = metaDNA.ExportTable(infer, unique:=True)
    End Sub
End Module
