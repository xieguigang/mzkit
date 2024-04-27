#Region "Microsoft.VisualBasic::995ee5e822c95593715f3e8e0182fd77, G:/mzkit/src/mzmath/Oligonucleotide_MS//VBA/Dim9.vb"

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

    '   Total Lines: 27
    '    Code Lines: 21
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 708 B


    ' Class Dim9
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Arp, Crp, List, Vrp
    ' 
    ' /********************************************************************************/

#End Region

Friend Class Dim9 : Inherits VB6DimensionVector(Of Object)

    Public Sub New()
        MyBase.New(9)
    End Sub

    Public Shared Function Arp() As Integer()
        Return {10, 12, 5, 6, 1, 0}
    End Function

    Public Shared Function Crp() As Integer()
        Return {9, 12, 3, 7, 1, 0}
    End Function

    Public Shared Function Vrp() As Integer()
        Return {10, 13, 2, 8, 1, 0}
    End Function

    Public Shared Function List() As Dictionary(Of String, Integer())
        Return New Dictionary(Of String, Integer()) From {
            {NameOf(Arp), Arp()},
            {NameOf(Crp), Crp()},
            {NameOf(Vrp), Vrp()}
        }
    End Function

End Class
