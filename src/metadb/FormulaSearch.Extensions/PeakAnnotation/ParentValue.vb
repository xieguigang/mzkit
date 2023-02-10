#Region "Microsoft.VisualBasic::d798fa6876dfcd4ccb578fae9e4dfd04, mzkit\src\metadb\FormulaSearch.Extensions\PeakAnnotation\ParentValue.vb"

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

    '   Total Lines: 25
    '    Code Lines: 18
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 716 B


    ' Class ParentValue
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetFragment, TestFragments
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.AtomGroups
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Friend Class ParentValue

    Friend ReadOnly parentMz As Double
    Friend ReadOnly formula As Formula

    Sub New(parentMz As Double, formula As String)
        Me.parentMz = parentMz
        Me.formula = FormulaScanner.ScanFormula(formula)
    End Sub

    Public Function TestFragments(frag As String) As Boolean
        If formula Is Nothing Then
            Return True
        End If

        Return True
    End Function

    Public Function GetFragment(candidates As IEnumerable(Of FragmentAnnotationHolder)) As FragmentAnnotationHolder

    End Function
End Class
