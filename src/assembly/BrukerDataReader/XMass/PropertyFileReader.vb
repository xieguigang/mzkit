#Region "Microsoft.VisualBasic::3f0e3db03d073e20c1ec6d6e0c428aea, E:/mzkit/src/assembly/BrukerDataReader//XMass/PropertyFileReader.vb"

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

    '   Total Lines: 45
    '    Code Lines: 32
    ' Comment Lines: 5
    '   Blank Lines: 8
    '     File Size: 1.46 KB


    '     Class PropertyFileReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CheckFlag, ReadData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Parser

Namespace XMass

    Friend NotInheritable Class PropertyFileReader

        Private Sub New()
        End Sub

        Public Shared Iterator Function ReadData(file As StreamReader) As IEnumerable(Of NamedValue(Of String()))
            For Each block As String() In FormattedParser.FlagSplit(file, AddressOf CheckFlag)
                Dim si As String = block.JoinBy(vbCrLf)
                Dim split = si.TrimStart("#"c, "$"c).GetTagValue("=", trim:=True)

                block = split.Value.Trim _
                    .LineTokens _
                    .Select(Function(sj) Strings.Trim(sj)) _
                    .ToArray

                Yield New NamedValue(Of String()) With {
                    .Name = split.Name,
                    .Value = block
                }
            Next

            Call file.Close()
            Call file.Dispose()
        End Function

        ''' <summary>
        ''' ##$var=value
        ''' </summary>
        ''' <param name="si"></param>
        ''' <returns></returns>
        Private Shared Function CheckFlag(si As String) As Boolean
            If si Is Nothing OrElse si.Length = 0 Then
                Return False
            Else
                Return si.StartsWith("##")
            End If
        End Function
    End Class
End Namespace
