#Region "Microsoft.VisualBasic::848df95b2745986c5e6c76774dbb277f, G:/mzkit/src/metadb/Chemoinformatics//Formula/ElementNumType.vb"

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

    '   Total Lines: 51
    '    Code Lines: 44
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.37 KB


    '     Structure ElementNumType
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula

    Friend Structure ElementNumType

        Public H As Integer
        Public C As Integer
        Public Si As Integer
        Public N As Integer
        Public P As Integer
        Public O As Integer
        Public S As Integer
        Public Cl As Integer
        Public I As Integer
        Public F As Integer
        Public Br As Integer
        Public Other As Integer

        Public Formula As Formula

        Sub New(f As Formula)
            H = f("H")
            C = f("C")
            Si = f("Si")
            N = f("N")
            P = f("P")
            O = f("O")
            S = f("S")
            Cl = f("Cl")
            I = f("I")
            Me.F = f("F")
            Br = f("Br")

            Dim counts As New Dictionary(Of String, Integer)(f.CountsByElement)

            Call counts.Remove("H")
            Call counts.Remove("C")
            Call counts.Remove("Si")
            Call counts.Remove("N")
            Call counts.Remove("P")
            Call counts.Remove("O")
            Call counts.Remove("S")
            Call counts.Remove("Cl")
            Call counts.Remove("I")
            Call counts.Remove("F")
            Call counts.Remove("Br")

            Other = counts.Values.Sum
            Formula = f
        End Sub
    End Structure
End Namespace
