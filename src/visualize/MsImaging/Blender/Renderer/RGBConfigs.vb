#Region "Microsoft.VisualBasic::9bf5496e818ffbf0f1de58fef1b61128, visualize\MsImaging\Blender\Renderer\RGBConfigs.vb"

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

'   Total Lines: 37
'    Code Lines: 28 (75.68%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 9 (24.32%)
'     File Size: 1.47 KB


'     Class RGBConfigs
' 
'         Properties: B, G, R
' 
'         Function: GetJSON, ParseJSON, ParseVal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Blender

    ''' <summary>
    ''' config for the rgb ions
    ''' </summary>
    Public Class RGBConfigs : Implements Enumeration(Of MzAnnotation)

        Public Property R As MzAnnotation
        Public Property G As MzAnnotation
        Public Property B As MzAnnotation

        Public Function GetJSON() As String
            Dim json As New Dictionary(Of String, Dictionary(Of String, String))

            json!mode = New Dictionary(Of String, String) From {{"blender", "rgb"}}
            json!r = ToJson(R)
            json!g = ToJson(G)
            json!b = ToJson(B)

            Return json.GetJson
        End Function

        Public Overrides Function ToString() As String
            Dim rgb As New List(Of String)

            If Not R Is Nothing Then Call rgb.Add($"{R.productMz.ToString("F4")}(red)")
            If Not G Is Nothing Then Call rgb.Add($"{G.productMz.ToString("F4")}(green)")
            If Not B Is Nothing Then Call rgb.Add($"{B.productMz.ToString("F4")}(blue)")

            Return rgb.JoinBy("; ")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ToJson(m As MzAnnotation) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"m/z", m.productMz},
                {"annotation", m.annotation}
            }
        End Function

        Public Shared Function ParseJSON(jsonstr As String) As RGBConfigs
            Dim vals = jsonstr.LoadJSON(Of Dictionary(Of String, Dictionary(Of String, String)))
            Dim cfgs As New RGBConfigs With {
                .R = ParseVal(vals!r),
                .G = ParseVal(vals!g),
                .B = ParseVal(vals!b)
            }

            Return cfgs
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function ParseVal(val As Dictionary(Of String, String)) As MzAnnotation
            Return New MzAnnotation With {
                .annotation = val!annotation,
                .productMz = Double.Parse(val("m/z"))
            }
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of MzAnnotation) Implements Enumeration(Of MzAnnotation).GenericEnumerator
            If Not R Is Nothing Then Yield R
            If Not G Is Nothing Then Yield G
            If Not B Is Nothing Then Yield B
        End Function
    End Class
End Namespace
