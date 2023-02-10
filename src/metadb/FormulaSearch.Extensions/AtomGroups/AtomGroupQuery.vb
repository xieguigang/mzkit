#Region "Microsoft.VisualBasic::09b5b1ed99e05910a54168dfadfed14b, mzkit\src\metadb\FormulaSearch.Extensions\AtomGroups\AtomGroupQuery.vb"

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

    '   Total Lines: 68
    '    Code Lines: 46
    ' Comment Lines: 12
    '   Blank Lines: 10
    '     File Size: 2.65 KB


    '     Class AtomGroupQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FilterByMass, GetByMass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.My
Imports stdNum = System.Math

Namespace AtomGroups

    Public Class AtomGroupQuery

        ReadOnly mass As Double
        ReadOnly da As Double
        ReadOnly adducts As MzCalculator()

        Sub New(mass As Double,
                Optional da As Double = 0.1,
                Optional adducts As MzCalculator() = Nothing)

            Me.mass = mass
            Me.da = da
            Me.adducts = adducts
        End Sub

        ''' <summary>
        ''' populate all candidate hits list by a specific mass tolerance hits
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function FilterByMass() As IEnumerable(Of FragmentAnnotationHolder)
            For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
                ' returns the first value if matched with mass delta
                If stdNum.Abs(group.exactMass - mass) <= da Then
                    Yield group
                ElseIf Not adducts Is Nothing Then
                    ' test on adducts
                    For Each type As MzCalculator In adducts
                        Dim mz As Double = type.CalcMZ(group.exactMass)

                        If stdNum.Abs(mz - mass) <= da Then
                            Yield New FragmentAnnotationHolder(MassGroup.CreateAdducts(group, adducts:=type))
                        End If
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' match on first hit
        ''' </summary>
        ''' <returns></returns>
        Public Function GetByMass() As FragmentAnnotationHolder
            For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
                ' returns the first value if matched with mass delta
                If stdNum.Abs(group.exactMass - mass) <= da Then
                    Return group
                ElseIf Not adducts Is Nothing Then
                    ' test on adducts
                    For Each type As MzCalculator In adducts
                        Dim mz As Double = type.CalcMZ(group.exactMass)

                        If stdNum.Abs(mz - mass) <= da Then
                            Return New FragmentAnnotationHolder(MassGroup.CreateAdducts(group, adducts:=type))
                        End If
                    Next
                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace
