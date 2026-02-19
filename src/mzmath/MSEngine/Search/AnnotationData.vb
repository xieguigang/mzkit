#Region "Microsoft.VisualBasic::75d69fe40c811fd7edd699eba86c9964, mzmath\MSEngine\Search\AnnotationData.vb"

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
'    Code Lines: 21 (42.00%)
' Comment Lines: 23 (46.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 6 (12.00%)
'     File Size: 2.00 KB


' Class AnnotationData
' 
'     Properties: [class], Alignment, CommonName, ExactMass, Formula
'                 ID, kingdom, molecular_framework, Score, sub_class
'                 super_class, Xref
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the annotation result dataset
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks>
''' the report table row will be generates from this object
''' </remarks>
Public Class AnnotationData(Of T As ICrossReference)
    Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider
    Implements GenericCompound
    Implements IMetabolite(Of T)

    ''' <summary>
    ''' A unique database reference id of current metabolite data object
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String Implements IReadOnlyId.Identity
    Public Property ExactMass As Double Implements IExactMassProvider.ExactMass
    Public Property CommonName As String Implements ICompoundNameProvider.CommonName
    Public Property Formula As String Implements IFormulaProvider.Formula

    ''' <summary>
    ''' the external database cross reference
    ''' </summary>
    ''' <returns></returns>
    Public Property Xref As T Implements IMetabolite(Of T).CrossReference

    ''' <summary>
    ''' MSDIAL score
    ''' </summary>
    ''' <returns></returns>
    Public Property Score As MsScanMatchResult
    ''' <summary>
    ''' Spectrum alignment result
    ''' </summary>
    ''' <returns></returns>
    Public Property Alignment As AlignmentOutput

    Public Property kingdom As String Implements ICompoundClass.kingdom
    Public Property super_class As String Implements ICompoundClass.super_class
    Public Property [class] As String Implements ICompoundClass.class
    Public Property sub_class As String Implements ICompoundClass.sub_class
    Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

End Class
