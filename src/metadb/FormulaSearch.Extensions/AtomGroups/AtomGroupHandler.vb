#Region "Microsoft.VisualBasic::336f884666a379c1b0f8d9111cd53f85, mzkit\src\metadb\FormulaSearch.Extensions\AtomGroups\AtomGroupHandler.vb"

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
'    Code Lines: 42
' Comment Lines: 0
'   Blank Lines: 11
'     File Size: 2.07 KB


' Class AtomGroupHandler
' 
'     Function: FindDelta, GetByMass, loadGroup
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.My
Imports stdNum = System.Math

Public Class AtomGroupHandler

    Shared ReadOnly alkyl As Dictionary(Of String, Formula) = loadGroup(Of Alkyl)()
    Shared ReadOnly ketones As Dictionary(Of String, Formula) = loadGroup(Of Ketones)()
    Shared ReadOnly amines As Dictionary(Of String, Formula) = loadGroup(Of Amines)()
    Shared ReadOnly alkenyl As Dictionary(Of String, Formula) = loadGroup(Of Alkenyl)()
    Shared ReadOnly others As Dictionary(Of String, Formula) = loadGroup(Of Others)()

    Private Shared Function loadGroup(Of T As Class)() As Dictionary(Of String, Formula)
        Return DataFramework.Schema(Of T)(
            flag:=PropertyAccess.Readable,
            nonIndex:=True,
            binds:=BindingFlags.Static Or BindingFlags.Public
        ) _
        .ToDictionary(Function(p) p.Key,
                        Function(p)
                            Return DirectCast(p.Value.GetValue(Nothing, Nothing), Formula)
                        End Function)
    End Function

    Public Shared ReadOnly Property AllAnnotations As FragmentAnnotationHolder()
        Get
            Return SingletonList(Of FragmentAnnotationHolder).ForEach.ToArray
        End Get
    End Property

    Shared Sub New()
        Call SingletonList(Of FragmentAnnotationHolder).Add(From i In alkyl Select New FragmentAnnotationHolder(i.Value))
        Call SingletonList(Of FragmentAnnotationHolder).Add(From i In ketones Select New FragmentAnnotationHolder(i.Value))
        Call SingletonList(Of FragmentAnnotationHolder).Add(From i In amines Select New FragmentAnnotationHolder(i.Value))
        Call SingletonList(Of FragmentAnnotationHolder).Add(From i In alkenyl Select New FragmentAnnotationHolder(i.Value))
        Call SingletonList(Of FragmentAnnotationHolder).Add(From i In others Select New FragmentAnnotationHolder(i.Value))

        Call Multiple(SingletonList(Of FragmentAnnotationHolder).ForEach.ToArray)
        Call MixAll(SingletonList(Of FragmentAnnotationHolder).ForEach.ToArray)
    End Sub

    ''' <summary>
    ''' x2
    ''' </summary>
    Private Shared Sub Multiple(all As FragmentAnnotationHolder())
        For Each item In all
            SingletonList(Of FragmentAnnotationHolder).Add(item * 2)
        Next
    End Sub

    Private Shared Sub MixAll(all As FragmentAnnotationHolder())
        Dim mix As FragmentAnnotationHolder

        ' a + b
        For Each a In all
            For Each b In From i In all Where i.name <> a.name
                SingletonList(Of FragmentAnnotationHolder).Add(a + b)
            Next
        Next

        ' a - b
        For Each a In all
            For Each b In From i In all Where i.name <> a.name
                mix = a - b

                If mix.exactMass > 0 Then
                    SingletonList(Of FragmentAnnotationHolder).Add(mix)
                End If
            Next
        Next
    End Sub

    Public Shared Sub Register(annotations As IEnumerable(Of FragmentAnnotationHolder))
        Dim list As FragmentAnnotationHolder() = annotations.ToArray

        Call Multiple(list)
        Call MixAll(list)
    End Sub

    Public Shared Function CreateModel(name As String, formula As String) As FragmentAnnotationHolder
        Dim chemical As Formula = FormulaScanner.ScanFormula(formula)
        Dim anno As New FragmentAnnotationHolder(chemical, name)

        Return anno
    End Function

    Public Shared Function CreateModel(name As String, exactMass As Double) As FragmentAnnotationHolder
        Dim group As New MassGroup With {
            .name = name,
            .exactMass = exactMass
        }

        Return New FragmentAnnotationHolder(group)
    End Function

    Public Shared Function GetByMass(mass As Double, Optional da As Double = 0.1) As FragmentAnnotationHolder
        For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
            If stdNum.Abs(group.exactMass - mass) <= da Then
                Return group
            End If
        Next

        Return Nothing
    End Function

    Public Shared Function FindDelta(mz1 As Double, mz2 As Double,
                                     Optional ByRef delta As Integer = 0,
                                     Optional da As Double = 0.1) As FragmentAnnotationHolder
        Dim d As Double = mz1 - mz2
        Dim dmass As Double = stdNum.Abs(d)

        If dmass <= 0.1 Then
            Return Nothing
        Else
            delta = stdNum.Sign(d)
        End If

        Dim group As FragmentAnnotationHolder = GetByMass(dmass, da)
        Return group
    End Function
End Class
