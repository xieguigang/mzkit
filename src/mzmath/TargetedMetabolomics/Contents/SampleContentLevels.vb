#Region "Microsoft.VisualBasic::5a80063536c25179a7c2729c81e5f11a, src\mzmath\TargetedMetabolomics\Contents\SampleContentLevels.vb"

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

    ''' <summary>
    ''' a helper model for get concentration gradient value of given level
    ''' </summary>
    Public Class SampleContentLevels

        Friend ReadOnly levels As IReadOnlyDictionary(Of String, Double)
        Friend ReadOnly directMap As Boolean

        ''' <summary>
        ''' get concentration gradient value of given level
        ''' </summary>
        ''' <param name="sampleLevel"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Content(sampleLevel As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return levels(If(directMap, sampleLevel, levelKey(sampleLevel)))
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="levels"></param>
        ''' <param name="directMap">
        ''' if this parameter is set to TRUE, then it means do not parse the level name into integer number level
        ''' use the original name as the level key.
        ''' </param>
        Sub New(levels As Dictionary(Of String, Double), Optional directMap As Boolean = False)
            Me.directMap = directMap
            Me.levels = levels _
                .OrderBy(Function(L) L.Value) _
                .ToDictionary(Function(L) If(directMap, L.Key, levelKey(L.Key)),
                              Function(L)
                                  Return L.Value
                              End Function)
        End Sub

        ''' <summary>
        ''' parse the original level key name into a integer number
        ''' </summary>
        ''' <param name="sampleLevel"></param>
        ''' <returns></returns>
        Private Function levelKey(sampleLevel As String) As String
            Return "L" & sampleLevel.Match("\d+").ParseInteger
        End Function

        Public Overrides Function ToString() As String
            Return DirectCast(levels, Dictionary(Of String, Double)).GetJson
        End Function

    End Class
End Namespace
