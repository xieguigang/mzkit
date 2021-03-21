#Region "Microsoft.VisualBasic::8cad86751bcc452b861ffa1e4c16eff2, src\metadb\Chemoinformatics\Formula\ElementNumType.vb"

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

        Sub New(formula As Formula)
            H = formula("H")
            C = formula("C")
            Si = formula("Si")
            N = formula("N")
            P = formula("P")
            O = formula("O")
            S = formula("S")
            Cl = formula("Cl")
            I = formula("I")
            F = formula("F")
            Br = formula("Br")

            Dim counts As New Dictionary(Of String, Integer)(formula.CountsByElement)

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
        End Sub
    End Structure
End Namespace
