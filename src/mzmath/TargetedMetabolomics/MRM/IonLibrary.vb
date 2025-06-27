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
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq

Namespace MRM

    Public Class IonLibrary : Implements Enumeration(Of IonPair)

        Dim mzErr As Tolerance = Tolerance.DeltaMass(0.3)

        ''' <summary>
        ''' A collection of the MRM ion pairs in current library data
        ''' </summary>
        ReadOnly ions As IonPair()

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return ions.IsNullOrEmpty
            End Get
        End Property

        Sub New(ions As IEnumerable(Of IonPair))
            Me.ions = ions.ToArray
        End Sub

        Public Sub SetError(mzErr As Tolerance)
            If mzErr.Type = MassToleranceType.Da Then
                Me.mzErr = Tolerance.DeltaMass(mzErr.DeltaTolerance)
            Else
                Me.mzErr = Tolerance.PPM(mzErr.DeltaTolerance)
            End If
        End Sub

        Public Function GetDisplay(ion As IonPair) As String
            If ion Is Nothing Then
                Call "the given MRM ion pair data is nothing!".Warning
                Return $"Ion [{ion.precursor}/{ion.product}]"
            ElseIf Not ion.name.StringEmpty(, True) Then
                Return ion.name
            End If

            Dim namedIon As IonPair = ions _
                .Where(Function(i)
                           Return i.EqualsTo(ion, mzErr)
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

        Public Function GetIsomerism() As IEnumerable(Of IsomerismIonPairs)
            Return IonPair.GetIsomerism(ions, mzErr)
        End Function

        Public Function GetIsomerism(q1 As Double, q3 As Double) As IsomerismIonPairs
            Dim iso As IonPair() = ions _
                .Where(Function(i)
                           Return mzErr(q1, i.precursor) AndAlso mzErr(q3, i.product)
                       End Function) _
                .ToArray

            Return New IsomerismIonPairs With {.ions = iso}
        End Function

        Public Function GetIon(precursor As Double, product As Double) As IonPair
            Return ions _
                .Where(Function(i)
                           Return mzErr(precursor, i.precursor) AndAlso
                                  mzErr(product, i.product)
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

        Public Shared Function LoadFile(file As String) As IonLibrary
            Dim ions As IonPair()

            Try
                ions = file.LoadCsv(Of IonPair)
            Catch ex As Exception
                ions = {}
                App.LogException(ex)
            End Try

            Return New IonLibrary(ions)
        End Function
    End Class
End Namespace