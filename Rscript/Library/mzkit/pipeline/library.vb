#Region "Microsoft.VisualBasic::f33f9eae48f381a95f137203ec79dd95, mzkit\Rscript\Library\mzkit\annotations\library.vb"

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

'   Total Lines: 235
'    Code Lines: 192
' Comment Lines: 5
'   Blank Lines: 38
'     File Size: 8.72 KB


' Module library
' 
'     Function: AddReference, asSpectrum, createAnnotation, createLibraryIO, createMetabolite
'               createPrecursorIons, PopulateIonData, queryByMz, SaveResult
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the metabolite annotation toolkit
''' </summary>
<Package("annotation")>
<RTypeExport("xref", GetType(xref))>
Module library

    <ExportAPI("assert.adducts")>
    <RApiReturn(GetType(MzCalculator))>
    Public Function assertAdducts(formula As String,
                                  <RRawVectorArgument>
                                  adducts As Object,
                                  Optional ion_mode As Object = "+",
                                  Optional env As Environment = Nothing) As Object

        Static asserts As New Dictionary(Of IonModes, PrecursorAdductsAssignRuler) From {
            {IonModes.Positive, New PrecursorAdductsAssignRuler(IonModes.Positive)},
            {IonModes.Negative, New PrecursorAdductsAssignRuler(IonModes.Negative)}
        }

        Dim ionVal = Math.GetIonMode(ion_mode, env)
        Dim ruler = asserts(ionVal)
        Dim precursors As MzCalculator() = Math.GetPrecursorTypes(adducts, env)

        Return ruler.AssertAdducts(formula, precursors).ToArray
    End Function

    ''' <summary>
    ''' a shortcut method for populate the peak ms2 data from a mzpack raw data file
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("populateIonData")>
    <Extension>
    <RApiReturn(GetType(PeakMs2))>
    Public Function PopulateIonData(raw As mzPack,
                                    Optional mzdiff As Object = "da:0.3",
                                    Optional env As Environment = Nothing) As Object

        Dim tolerance = Math.getTolerance(mzdiff, env)

        If tolerance Like GetType(Message) Then
            Return tolerance.TryCast(Of Message)
        End If

        Dim mzErr As Tolerance = tolerance.TryCast(Of Tolerance)
        Dim ions As New List(Of PeakMs2)

        For Each ms1 In raw.MS
            For Each ms2 In ms1.products.SafeQuery
                For Each mzi As Double In ms1.mz
                    If mzErr(mzi, ms2.parentMz) Then
                        Dim ion2 As New PeakMs2 With {
                            .mz = mzi,
                            .rt = ms1.rt,
                            .file = raw.source,
                            .lib_guid = ms2.scan_id,
                            .activation = ms2.activationMethod.Description,
                            .collisionEnergy = ms2.collisionEnergy,
                            .intensity = ms2.intensity,
                            .scan = ms2.scan_id,
                            .mzInto = ms2.GetMs.ToArray
                        }

                        Call ions.Add(ion2)
                    End If
                Next
            Next
        Next

        Return ions.ToArray
    End Function

    ''' <summary>
    ''' create a new metabolite annotation information
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="formula"></param>
    ''' <param name="name"></param>
    ''' <param name="synonym"></param>
    ''' <param name="xref"></param>
    ''' <returns></returns>
    <ExportAPI("annotation")>
    Public Function createAnnotation(id As String,
                                     formula As String,
                                     name As String,
                                     Optional synonym As String() = Nothing,
                                     Optional xref As xref = Nothing) As MetaInfo

        Return New MetaInfo With {
            .xref = If(xref, New xref),
            .formula = formula,
            .ID = id,
            .name = name,
            .synonym = synonym,
            .exact_mass = CDbl(FormulaScanner.ScanFormula(formula))
        }
    End Function
End Module
