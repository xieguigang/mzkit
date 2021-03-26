#Region "Microsoft.VisualBasic::8554b709db5c8562cce57f1cd3a336a7, src\mzmath\TargetedMetabolomics\Contents\ContentTable.vb"

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

    '     Class ContentTable
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetIS, GetStandards, hasDefined, hasISDefined, StripMaxCommonNames
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text

Namespace Content

    Public Class ContentTable

        ''' <summary>
        ''' [ion -> [sample, content]]
        ''' </summary>
        ReadOnly matrix As Dictionary(Of String, SampleContentLevels)
        ''' <summary>
        ''' [ion -> standards]
        ''' </summary>
        ReadOnly standards As Dictionary(Of String, Standards)
        ReadOnly [IS] As Dictionary(Of String, [IS])

        Default Public ReadOnly Property Content(sampleLevel As String, ion As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return matrix(ion)(sampleLevel)
            End Get
        End Property

        Sub New(matrix As Dictionary(Of String, SampleContentLevels), standards As Dictionary(Of String, Standards), [IS] As Dictionary(Of String, [IS]))
            Me.matrix = matrix
            Me.standards = standards
            Me.IS = [IS]
        End Sub

        ''' <summary>
        ''' check if the target <paramref name="ion"/> is exists or not?
        ''' </summary>
        ''' <param name="ion"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function hasDefined(ion As String) As Boolean
            Return standards.ContainsKey(ion)
        End Function

        ''' <summary>
        ''' check if the target <paramref name="is"/> is exists or not?
        ''' </summary>
        ''' <param name="[is]"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function hasISDefined([is] As String) As Boolean
            Return Me.IS.ContainsKey([is])
        End Function

        ''' <summary>
        ''' get IS definition model.
        ''' </summary>
        ''' <param name="ion"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIS(ion As String) As [IS]
            If [IS] Is Nothing Then
                Return New [IS]
            Else
                Return [IS].TryGetValue(ion, [default]:=New [IS])
            End If
        End Function

        ''' <summary>
        ''' get target ion data model object
        ''' </summary>
        ''' <param name="ion"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetStandards(ion As String) As Standards
            Return standards(ion)
        End Function

        ''' <summary>
        ''' 这个函数主要是用于删除wiff文件转换为mzML文件之后留存的文件名前缀
        ''' </summary>
        ''' <param name="files">
        ''' a list of *.mzML file path that convert from wiff raw data file
        ''' </param>
        ''' <returns></returns>
        Public Shared Function StripMaxCommonNames(files As String()) As NamedValue(Of String)()
            Dim names As String() = files.Select(AddressOf BaseName).ToArray
            Dim minName As String = names.MinLengthString
            Dim index As Integer

            For i As Integer = 0 To minName.Length - 1
                index = i

                If names.Select(Function(str) str(index)).Distinct.Count > 1 Then
                    names = names _
                        .Select(Function(str)
                                    Return str.Substring(index).Trim(" "c, ASCII.TAB, "-"c, "_"c)
                                End Function) _
                        .ToArray

                    Exit For
                End If
            Next

            If names.All(Function(str) Char.IsDigit(str.First)) Then
                names = names _
                    .Select(Function(str) $"L{str}") _
                    .ToArray
            End If

            Return names _
                .Select(Function(nameStr, i)
                            Return New NamedValue(Of String) With {
                                .Name = nameStr,
                                .Value = files(i)
                            }
                        End Function) _
                .OrderBy(Function(file)
                             Return file.Name.Match("\d+").ParseInteger
                         End Function) _
                .ToArray
        End Function
    End Class
End Namespace
