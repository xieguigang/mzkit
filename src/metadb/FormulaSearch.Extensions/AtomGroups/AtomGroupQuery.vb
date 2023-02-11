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

    Public Class AnnotationQueryResult

        Public Property Annotation As FragmentAnnotationHolder
        Public Property delta As Double

        Sub New(annotation As FragmentAnnotationHolder, delta As Double)
            Me.delta = delta
            Me.Annotation = annotation
        End Sub

    End Class

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
        Public Iterator Function FilterByMass() As IEnumerable(Of AnnotationQueryResult)
            Dim delta As Double

            For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
                delta = stdNum.Abs(group.exactMass - mass)

                If delta <= da Then
                    Yield New AnnotationQueryResult(group, delta)
                ElseIf Not adducts Is Nothing Then
                    ' test on adducts
                    For Each type As MzCalculator In adducts
                        Dim mz As Double = type.CalcMZ(group.exactMass)

                        delta = stdNum.Abs(mz - mass)

                        If delta <= da Then
                            Yield New AnnotationQueryResult(
                                annotation:=New FragmentAnnotationHolder(MassGroup.CreateAdducts(group, adducts:=type)),
                                delta:=delta
                            )
                        End If
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' match on min delta mass hit
        ''' </summary>
        ''' <returns></returns>
        Public Function GetByMass() As FragmentAnnotationHolder
            Dim all = FilterByMass.OrderBy(Function(a) a.delta).ToArray

            If all.Length = 0 Then
                Return Nothing
            Else
                Return all.First.Annotation
            End If
        End Function

        ''' <summary>
        ''' Found atom groups by the mass delta between two fragment mz value
        ''' </summary>
        ''' <param name="mz1"></param>
        ''' <param name="mz2"></param>
        ''' <param name="delta"></param>
        ''' <returns></returns>
        Public Shared Function FindDelta(mz1 As Double, mz2 As Double,
                                         Optional ByRef delta As Integer = 0,
                                         Optional da As Double = 0.1,
                                         Optional adducts As MzCalculator() = Nothing) As FragmentAnnotationHolder

            Dim all = CreateQuery(mz1, mz2, delta, da, adducts).FilterByMass.ToArray

            If all.Length = 0 Then
                Return Nothing
            Else
                Return all.OrderBy(Function(a) a.delta).First.Annotation
            End If
        End Function

        Private Shared Function CreateQuery(mz1 As Double, mz2 As Double,
                                            Optional ByRef delta As Integer = 0,
                                            Optional da As Double = 0.1,
                                            Optional adducts As MzCalculator() = Nothing) As AtomGroupQuery
            Dim d As Double = mz1 - mz2
            Dim dmass As Double = stdNum.Abs(d)

            If dmass <= 0.1 Then
                Return Nothing
            Else
                delta = stdNum.Sign(d)
            End If

            Return New AtomGroupQuery(dmass, da, adducts)
        End Function

        Public Shared Function ListDelta(mz1 As Double, mz2 As Double,
                                         Optional ByRef delta As Integer = 0,
                                         Optional da As Double = 0.1,
                                         Optional adducts As MzCalculator() = Nothing) As IEnumerable(Of AnnotationQueryResult)

            Return CreateQuery(mz1, mz2, delta, da, adducts).FilterByMass
        End Function
    End Class
End Namespace
