#Region "Microsoft.VisualBasic::40f84b4e8264dc2fa2fd30c59e2c0015, visualize\MsImaging\Blender\Renderer\CMYKConfigs.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 38
    '    Code Lines: 29 (76.32%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.68%)
    '     File Size: 1.31 KB


    '     Class CMYKConfigs
    ' 
    '         Properties: C, K, M, Y
    ' 
    '         Function: GetJSON, ParseJSON
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
