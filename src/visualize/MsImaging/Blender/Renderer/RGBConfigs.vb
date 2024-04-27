#Region "Microsoft.VisualBasic::9bf5496e818ffbf0f1de58fef1b61128, G:/mzkit/src/visualize/MsImaging//Blender/Renderer/RGBConfigs.vb"

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
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 9
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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Blender

    Public Class RGBConfigs

        Public Property R As NamedValue(Of Double)
        Public Property G As NamedValue(Of Double)
        Public Property B As NamedValue(Of Double)

        Public Function GetJSON() As String
            Dim json As New Dictionary(Of String, Dictionary(Of String, String))

            json!r = New Dictionary(Of String, String) From {{"m/z", R.Value}, {"annotation", R.Name}}
            json!g = New Dictionary(Of String, String) From {{"m/z", G.Value}, {"annotation", G.Name}}
            json!b = New Dictionary(Of String, String) From {{"m/z", B.Value}, {"annotation", B.Name}}

            Return json.GetJson
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

        Private Shared Function ParseVal(val As Dictionary(Of String, String)) As NamedValue(Of Double)
            Return New NamedValue(Of Double)(val!annotation, Double.Parse(val("m/z")))
        End Function
    End Class
End Namespace
