#Region "Microsoft.VisualBasic::324ac7c0a84df388fd8934eb63d933f0, E:/mzkit/src/assembly/BrukerDataReader//SCiLSLab/SpotMs.vb"

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

    '   Total Lines: 53
    '    Code Lines: 39
    ' Comment Lines: 1
    '   Blank Lines: 13
    '     File Size: 1.65 KB


    '     Class MsPack
    ' 
    '         Properties: matrix, mz
    ' 
    '         Function: ParseFile
    ' 
    '     Class SpotMs
    ' 
    '         Properties: intensity, spot_id
    ' 
    '         Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SCiLSLab

    Public Class MsPack : Inherits PackFile

        Public Property matrix As SpotMs()
        Public Property mz As Double()

        Public Shared Function ParseFile(file As Stream, Optional println As Action(Of String) = Nothing) As MsPack
            Dim pull As New MsPack

            If println Is Nothing Then
                println = Sub()
                              ' do nothing
                          End Sub
            End If

            Dim spots As SpotMs() = ParseTable(file, pull, AddressOf SpotMs.Parse, println).ToArray
            Dim headerLine As String = pull.metadata(".header")
            Dim mz As Double() = headerLine _
                .Split(";"c) _
                .Skip(1) _
                .Select(AddressOf Conversion.Val) _
                .ToArray

            pull.mz = mz
            pull.matrix = spots

            Return pull
        End Function
    End Class

    Public Class SpotMs

        Public Property spot_id As String
        Public Property intensity As Double()

        Friend Shared Function Parse(row As String(), headers As Index(Of String), println As Action(Of String)) As SpotMs
            Call println($"read spot vector: {row(Scan0)}")

            Return New SpotMs With {
                .spot_id = row(Scan0),
                .intensity = row _
                    .Skip(1) _
                    .Select(AddressOf Conversion.Val) _
                    .ToArray
            }
        End Function

    End Class
End Namespace
