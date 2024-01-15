Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports Microsoft.VisualBasic.Language

Public Class Compression

#Region "metadb As IMetaDb"
    ReadOnly annoData As Func(Of String, (name As String, formula As String))
    ReadOnly xrefData As Func(Of String, Dictionary(Of String, String))
#End Region

    Dim println As Action(Of String)

    Sub New(annoData As Func(Of String, (name As String, formula As String)),
            xrefData As Func(Of String, Dictionary(Of String, String)),
            Optional println As Action(Of String) = Nothing)

        Me.annoData = annoData
        Me.xrefData = xrefData
        Me.println = If(println, New Action(Of String)(AddressOf VBDebugger.EchoLine))
    End Sub

    Public Sub SpectrumCompression(spectrumLib As SpectrumReader, newPool As SpectrumPack,
                                   Optional nspec As Integer = 5,
                                   Optional xrefDb As String = Nothing,
                                   Optional test As Integer = -1)

        Dim pullAll = spectrumLib.LoadMass.ToArray
        Dim nsize As Integer = 0

        For Each metabo As MassIndex In pullAll
            Try
                Call CompressSingle(metabo, spectrumLib, newPool, nspec, xrefDb)

                nsize += 1

                ' take part of the data for run test
                If test > 0 Then
                    If nsize >= test Then
                        Exit For
                    End If
                End If
            Catch ex As Exception

            End Try
        Next
    End Sub

    Private Sub CompressSingle(metabo As MassIndex, spectrumLib As SpectrumReader, newPool As SpectrumPack,
                               nspec As Integer,
                               xrefDb As String)

        Dim allspec = spectrumLib.GetSpectrum(metabo).ToArray
        Dim i As i32 = 1

        If allspec.Length > nspec Then
            allspec = Cleanup.Compress(allspec, n:=nspec).ToArray
        End If

        Dim annoData = Me.annoData(metabo.name)
        Dim xrefs = Me.xrefData(metabo.name)
        Dim uuid As String
        Dim xref_id As String

        If annoData.name.StringEmpty AndAlso annoData.formula.StringEmpty Then
            uuid = metabo.name
            xref_id = uuid
        Else
            If Not xrefDb.StringEmpty Then
                uuid = xrefs.TryGetValue(xrefDb)
            Else
                uuid = metabo.name.Split.First
            End If

            If Not uuid.StringEmpty Then
                xref_id = uuid
            Else
                xref_id = metabo.name.Split.First
            End If

            uuid = $"{uuid}|{SpectrumPack.PathName(annoData.name)}|{SpectrumPack.PathName(annoData.formula)}"
        End If

        Call println(uuid)

        For Each spectrum As PeakMs2 In allspec
            spectrum.lib_guid = $"{uuid}#{++i}"
            spectrum.scan = xref_id
            spectrum.file = uuid
            newPool.Push(uuid, If(annoData.formula, metabo.formula), spectrum)
        Next
    End Sub
End Class
