Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Blender

    Public Class CMYKConfigs

        Public Property C As MzAnnotation
        Public Property M As MzAnnotation
        Public Property Y As MzAnnotation
        Public Property K As MzAnnotation

        Public Function GetJSON() As String
            Dim json As New Dictionary(Of String, Dictionary(Of String, String))

            json!mode = New Dictionary(Of String, String) From {{"blender", "cmyk"}}
            json!c = RGBConfigs.ToJson(C)
            json!m = RGBConfigs.ToJson(M)
            json!y = RGBConfigs.ToJson(Y)
            json!k = RGBConfigs.ToJson(K)

            Return json.GetJson
        End Function

        Public Shared Function ParseJSON(json_str As String) As CMYKConfigs
            Dim vals = json_str.LoadJSON(Of Dictionary(Of String, Dictionary(Of String, String)))
            Dim cfgs As New CMYKConfigs With {
                .C = RGBConfigs.ParseVal(vals!c),
                .M = RGBConfigs.ParseVal(vals!m),
                .Y = RGBConfigs.ParseVal(vals!y),
                .K = RGBConfigs.ParseVal(vals!k)
            }

            Return cfgs
        End Function

    End Class
End Namespace