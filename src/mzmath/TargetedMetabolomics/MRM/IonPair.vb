﻿#Region "Microsoft.VisualBasic::6752693422b268d31864701dbbc7ba67, mzmath\TargetedMetabolomics\MRM\IonPair.vb"

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

'   Total Lines: 125
'    Code Lines: 77 (61.60%)
' Comment Lines: 33 (26.40%)
'    - Xml Docs: 96.97%
' 
'   Blank Lines: 15 (12.00%)
'     File Size: 4.82 KB


'     Class IonPair
' 
'         Properties: accession, name, precursor, product, rt
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: (+2 Overloads) Assert, EqualsTo, GetIsomerism, populateGroupElement, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports chromatogramTicks = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram
Imports std = System.Math

Namespace MRM.Models

    ''' <summary>
    ''' Data model for the MRM ion pair
    ''' </summary>
    Public Class IonPair : Implements INamedValue

        ''' <summary>
        ''' The database accession ID
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="ID")>
        <Description("The unique reference id of current metabolite ion, example as hmdb id or cas number.")>
        <XmlAttribute>
        Public Property accession As String
        ''' <summary>
        ''' The display title name
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Description("The metabolite ion display name")>
        Public Property name As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' Q1 precursor ion m/z
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Description("The Q1 precursor ion m/z")>
        Public Property precursor As Double
        ''' <summary>
        ''' Q3 product ion m/z
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Description("The Q3 product ion m/z")>
        Public Property product As Double
        ''' <summary>
        ''' The retention time of the ion pair
        ''' </summary>
        ''' <returns></returns>
        '''
        <Description("The retention time of the ion pair, should be in time data unit Seconds.")>
        <XmlAttribute>
        Public Property rt As Double?

        Sub New()
        End Sub

        Sub New(q1 As Double, q3 As Double)
            precursor = q1
            product = q3
        End Sub

        Public Overrides Function ToString() As String
            If name.StringEmpty Then
                Return $"{precursor}/{product}"
            Else
                If rt Is Nothing Then
                    Return $"Dim {name} As [{precursor}, {product}]"
                Else
                    Return $"Dim {name} As [{precursor}, {product}], {rt} sec"
                End If
            End If
        End Function

        Public Function EqualsTo(other As IonPair, tolerance As Tolerance) As Boolean
            Dim checkMass = tolerance(other.precursor, precursor) AndAlso tolerance(other.product, product)

            If rt IsNot Nothing AndAlso other.rt IsNot Nothing Then
                Return checkMass AndAlso std.Abs(CDbl(rt) - CDbl(other.rt)) <= 1
            Else
                Return checkMass
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="tolerance">less than 0.3da or 20ppm??</param>
        ''' <returns></returns>
        Public Function Assert(chromatogram As chromatogramTicks, tolerance As Tolerance) As Boolean
            If chromatogram.id = "TIC" OrElse chromatogram.id = "BPC" Then
                Return False
            Else
                Dim pre As Double = chromatogram.precursor.MRMTargetMz
                Dim pro As Double = chromatogram.product.MRMTargetMz

                If tolerance.Equals(Val(pre), precursor) AndAlso tolerance.Equals(Val(pro), product) Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

        Public Function Assert(q1 As Double, q3 As Double, tolerance As Tolerance) As Boolean
            If tolerance.Equals(q1, precursor) AndAlso tolerance.Equals(q3, product) Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Produce an <see cref="IsomerismIonPairs"/> object sequence with 
        ''' element length equals to the input ions data 
        ''' <paramref name="ionpairs"/> 
        ''' </summary>
        ''' <param name="ionpairs"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        Public Shared Function GetIsomerism(ionpairs As IonPair(), tolerance As Tolerance) As IEnumerable(Of IsomerismIonPairs)
            Return populateGroupElement(ionpairs, tolerance) _
                .GroupBy(Function(i) i.groupKey) _
                .IteratesALL
        End Function

        Private Shared Iterator Function populateGroupElement(ionpairs As IonPair(), tolerance As Tolerance) As IEnumerable(Of IsomerismIonPairs)
            Dim iso As New List(Of IonPair)

            For Each ion As IonPair In ionpairs
                For Each can As IonPair In ionpairs.Where(Function(a) a.accession <> ion.accession)
                    If tolerance.Equals(can.precursor, ion.precursor) AndAlso tolerance.Equals(can.product, ion.product) Then
                        iso += can
                    End If
                Next

                Yield New IsomerismIonPairs With {
                    .ions = iso.PopAll,
                    .target = ion
                }
            Next
        End Function
    End Class

End Namespace
