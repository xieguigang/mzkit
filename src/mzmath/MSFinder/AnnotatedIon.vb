﻿#Region "Microsoft.VisualBasic::1e734f0d9645374e84220116302d7e95, mzmath\MSFinder\AnnotatedIon.vb"

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

    '   Total Lines: 50
    '    Code Lines: 37 (74.00%)
    ' Comment Lines: 6 (12.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (14.00%)
    '     File Size: 1.78 KB


    ' Class AnnotatedIon
    ' 
    '     Properties: AccurateMass, AdductIon, Intensity, IsotopeName, IsotopeWeightNumber
    '                 LinkedAccurateMass, LinkedIntensity, metadata, PeakType
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: SetAdduct, SetIsotope, SetIsotopeC13
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

''' <summary>
''' This class is the storage of adduct or isotope ion assignment used in MS-FINDER program.
''' </summary>
Public Class AnnotatedIon

    Public Property PeakType As AnnotationType
    Public Property AccurateMass As Double
    Public Property Intensity As Double
    Public Property LinkedAccurateMass As Double
    Public Property LinkedIntensity As Double
    Public Property AdductIon As AdductIon
    Public Property IsotopeWeightNumber As Integer

    ''' <summary> 
    ''' C-13, O-18, and something 
    ''' </summary>
    Public Property IsotopeName As String
    Public Property metadata As MetaboliteAnnotation

    Public Sub New()
        PeakType = AnnotationType.Product
    End Sub

    Public Sub SetIsotopeC13(linkedMz As Double)
        LinkedAccurateMass = linkedMz
        PeakType = AnnotationType.Isotope
        IsotopeWeightNumber = 1
        IsotopeName = "C-13"
    End Sub

    Public Sub SetIsotope(linkedMz As Double, intensity As Double, linkedIntensity As Double, isotomeName As String, weightNumber As Integer)
        PeakType = AnnotationType.Isotope
        LinkedAccurateMass = linkedMz
        Me.Intensity = intensity
        Me.LinkedIntensity = linkedIntensity
        IsotopeWeightNumber = weightNumber
        IsotopeName = isotomeName
    End Sub

    Public Sub SetAdduct(linkedMz As Double, intensity As Double, linkedIntensity As Double, adduct As AdductIon)
        PeakType = AnnotationType.Adduct
        Me.Intensity = intensity
        Me.LinkedIntensity = linkedIntensity
        LinkedAccurateMass = linkedMz
        AdductIon = adduct
    End Sub
End Class
