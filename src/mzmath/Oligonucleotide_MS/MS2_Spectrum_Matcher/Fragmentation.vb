#Region "Microsoft.VisualBasic::6e70adad4a28762bb54bd6f4da81ca50, E:/mzkit/src/mzmath/Oligonucleotide_MS//MS2_Spectrum_Matcher/Fragmentation.vb"

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

    '   Total Lines: 48
    '    Code Lines: 40
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 1.79 KB


    ' Class Fragmentation
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: LoadFragmentation
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' An excellent reference is: Timar, Z. in Handbook of Analysis of Oligonucleotides and Related Products. (eds. J.V. Bonilla & G.S. Srivatsa) 167-218 (CRC Press, 2011)
''' </summary>
Public Class Fragmentation

    Public Cut As String
    Public AdjustMassFromWhichEnd As String
    Public C As String
    Public H As String
    Public N As String
    Public O As String
    Public P As String
    Public S As String
    Public SelectAllowedCuts As Boolean

    Sub New(Cut As String, AdjustMassFromWhichEnd As String,
            C As String,
            H As String,
            N As String,
            O As String,
            P As String,
            S As String,
            SelectAllowedCuts As Boolean)

        Me.Cut = Cut
        Me.AdjustMassFromWhichEnd = AdjustMassFromWhichEnd
        Me.C = C
        Me.H = H
        Me.N = N
        Me.O = O
        Me.P = P
        Me.S = S
        Me.SelectAllowedCuts = SelectAllowedCuts
    End Sub

    Public Iterator Function LoadFragmentation() As IEnumerable(Of Fragmentation)
        Yield New Fragmentation("w", "5'", 0, 2, 0, 4, 1, 0, True)
        Yield New Fragmentation("x", "5'", 0, 0, 0, 3, 1, 0, True)
        Yield New Fragmentation("y", "5'", 0, 1, 0, 1, 0, 0, True)
        Yield New Fragmentation("z", "5'", 0, -1, 0, 0, 0, 0, True)
        Yield New Fragmentation("a-B", "3'", "special", "special", "special", "special", "special", "special", True)
        Yield New Fragmentation("a", "3'", 0, -2, 0, -4, -1, 0, True)
        Yield New Fragmentation("b", "3'", 0, 0, 0, -3, -1, 0, True)
        Yield New Fragmentation("c", "3'", 0, -1, 0, -1, 0, 0, True)
        Yield New Fragmentation("d", "3'", 0, 1, 0, 0, 0, 0, True)
    End Function

End Class
