#Region "Microsoft.VisualBasic::d7d395e54043e8330c780cdff7c2f091, mzkit\src\mzkit\Task\IonLibrary.vb"

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

    '   Total Lines: 79
    '    Code Lines: 67
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 2.68 KB


    ' Class IonLibrary
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GenericEnumerator, GetDisplay, GetEnumerator, GetIon, GetIonByKey
    '               ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq

Public Class IonLibrary : Implements Enumeration(Of IonPair)

    Dim dadot3 As Tolerance = Tolerance.DeltaMass(0.3)
    Dim ions As IonPair()

    Sub New(ions As IEnumerable(Of IonPair))
        Me.ions = ions.ToArray
    End Sub

    Public Function GetDisplay(ion As IonPair) As String
        Dim namedIon As IonPair = ions _
            .Where(Function(i)
                       Return i.EqualsTo(ion, dadot3)
                   End Function) _
            .FirstOrDefault
        Dim refId As String

        If namedIon Is Nothing Then
            refId = $"Ion [{ion.precursor}/{ion.product}]"
        Else
            refId = namedIon.name
        End If

        Return refId
    End Function

    Public Function GetIon(precursor As Double, product As Double) As IonPair
        Return ions _
            .Where(Function(i)
                       Return dadot3(precursor, i.precursor) AndAlso
                              dadot3(product, i.product)
                   End Function) _
            .FirstOrDefault
    End Function

    Public Function GetIonByKey(key As String) As IonPair
        If key.StringEmpty Then
            Return Nothing
        ElseIf key.IsPattern("Ion \[.+[/].+\]") Then
            Dim tuples = key.GetStackValue("[", "]").Split("/"c).Select(AddressOf Val).ToArray
            Dim ion As IonPair = GetIon(tuples(0), tuples(1))

            If ion Is Nothing Then
                Return New IonPair With {
                    .accession = key,
                    .name = key,
                    .precursor = tuples(0),
                    .product = tuples(1)
                }
            Else
                Return ion
            End If
        Else
            Return ions _
                .Where(Function(i)
                           Return i.accession = key OrElse i.name = key
                       End Function) _
                .FirstOrDefault
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"'{ions.Length}' ions in library"
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of IonPair) Implements Enumeration(Of IonPair).GenericEnumerator
        For Each ion As IonPair In ions
            Yield ion
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of IonPair).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
