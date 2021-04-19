Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports ProteoWizard.Interop.filters

<Assembly: InternalsVisibleTo("mzkit")>

Friend Class PWConvertProcessor

    Public filetype As OutFileTypes
    Public filters As Filter()
    Public output As String
    Public bin As ProteoWizardCLI

    Public Function runConvert(file As SeqValue(Of String)) As SeqValue(Of Boolean)
        Dim outputfile = $"{output}/{file.value.FileName}"
        Dim stdOut As String = bin.Convert2mzML(file.value, output, filetype, filters)

        Call stdOut.SaveTo(outputfile.ChangeSuffix("log"))

        If outputfile.FileExists(ZERO_Nonexists:=True) Then
            Return New SeqValue(Of Boolean)(file, True)
        Else
            Return New SeqValue(Of Boolean)(file, False)
        End If
    End Function
End Class