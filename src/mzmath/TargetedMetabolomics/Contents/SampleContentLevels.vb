#Region "Microsoft.VisualBasic::bfd9c1e59f6657ac42f3c4a9aba11ffa, TargetedMetabolomics\Contents\SampleContentLevels.vb"

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

    '     Class SampleContentLevels
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: levelKey, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Content

    Public Class SampleContentLevels

        ReadOnly levels As Dictionary(Of String, Double)
        ReadOnly directMap As Boolean

        Default Public ReadOnly Property Content(sampleLevel As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return levels(If(directMap, sampleLevel, levelKey(sampleLevel)))
            End Get
        End Property

        Sub New(levels As Dictionary(Of String, Double), Optional directMap As Boolean = False)
            Me.directMap = directMap
            Me.levels = levels _
                .OrderBy(Function(L) L.Value) _
                .ToDictionary(Function(L) If(directMap, L.Key, levelKey(L.Key)),
                              Function(L)
                                  Return L.Value
                              End Function)
        End Sub

        Private Function levelKey(sampleLevel As String) As String
            Return "L" & sampleLevel.Match("\d+").ParseInteger
        End Function

        Public Overrides Function ToString() As String
            Return levels.GetJson
        End Function

    End Class
End Namespace
