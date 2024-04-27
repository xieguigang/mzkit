#Region "Microsoft.VisualBasic::d1024fea7a4fb641096029c3d34356ce, G:/mzkit/src/assembly/BrukerDataReader//SCiLSLab/SpotSite.vb"

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

    '   Total Lines: 64
    '    Code Lines: 48
    ' Comment Lines: 2
    '   Blank Lines: 14
    '     File Size: 1.99 KB


    '     Class SpotPack
    ' 
    '         Properties: index
    ' 
    '         Function: ParseFile, X, Y
    ' 
    '     Class SpotSite
    ' 
    '         Properties: index, x, y
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace SCiLSLab

    Public Class SpotPack : Inherits PackFile

        Public Property index As Dictionary(Of String, SpotSite)

        Public Function X() As Double()
            Return (From p As SpotSite
                    In index.Values
                    Let xi As Double = p.x
                    Select xi).ToArray
        End Function

        Public Function Y() As Double()
            Return (From p As SpotSite
                    In index.Values
                    Let yi As Double = p.y
                    Select yi).ToArray
        End Function

        Public Shared Function ParseFile(file As Stream) As SpotPack
            Dim pull As New SpotPack
            Dim spots As SpotSite() = ParseTable(
                file:=file,
                byrefPack:=pull,
                parseLine:=AddressOf SpotSite.Parse,
                println:=Sub(text)
                             ' do nothing
                         End Sub).ToArray

            pull.index = spots.ToDictionary(Function(sp) sp.index)

            Return pull
        End Function
    End Class

    Public Class SpotSite

        Public Property index As String
        Public Property x As Double
        Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"spot_{index} [{x},{y}]"
        End Function

        Friend Shared Function Parse(row As String(), headers As Index(Of String), println As Action(Of String)) As SpotSite
            ' Spot index;x;y
            Dim index As String = row(headers("Spot index"))
            Dim x As Double = Val(row(headers("x")))
            Dim y As Double = Val(row(headers("y")))

            Return New SpotSite With {
                .index = index,
                .x = x,
                .y = y
            }
        End Function

    End Class
End Namespace
