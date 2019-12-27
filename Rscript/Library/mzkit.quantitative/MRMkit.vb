#Region "Microsoft.VisualBasic::2ee324534ad078e776c957485ad4b497, Rscript\Library\mzkit.quantitative\MRMkit.vb"

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

' Module MRMkit
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: ExtractIonData, ExtractPeakROI, printIonPairs, readIonPairs, ScanStandardCurve
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Data
Imports SMRUCC.MassSpectrum.Math.MRM.Models
Imports REnv = SMRUCC.Rsharp.Runtime.Internal
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

<Package("mzkit.mrm")>
Public Module MRMkit

    Sub New()
        REnv.ConsolePrinter.AttachConsoleFormatter(Of IonPair())(AddressOf printIonPairs)
    End Sub

    Private Function printIonPairs(ions As IonPair()) As String
        Dim csv = ions.ToCsvDoc
        Dim printContent = csv.Print(addBorder:=False)

        Return printContent
    End Function

    ''' <summary>
    ''' Extract ion peaks
    ''' </summary>
    ''' <param name="mzML$"></param>
    ''' <param name="ionpairs"></param>
    ''' <returns></returns>
    <ExportAPI("extract.ions")>
    Public Function ExtractIonData(mzML$, ionpairs As IonPair()) As NamedCollection(Of ChromatogramTick)()
        Return MRMSamples.ExtractIonData(ionpairs, mzML, Function(i) i.accession)
    End Function

    <ExportAPI("extract.peakROI")>
    Public Function ExtractPeakROI(mzML$, ionpairs As IonPair(),
                                   Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                   Optional baselineQuantile# = 0.65,
                                   Optional integratorTicks% = 5000,
                                   Optional peakAreaMethod As Integer = 1) As IonTPA()

        Dim method As PeakArea.Methods = CType(peakAreaMethod, PeakArea.Methods)

        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Return ScanOfTPA.ScanTPA(
            mzML, ionpairs, TPAFactors, baselineQuantile, integratorTicks, method
        )
    End Function

    ''' <summary>
    ''' Get ion pair definition data from a given table file.
    ''' </summary>
    ''' <param name="file">A csv file or xlsx Excel data sheet</param>
    ''' <param name="sheetName">The sheet name in excel tables.</param>
    ''' <returns></returns>
    <ExportAPI("read.ion_pairs")>
    Public Function readIonPairs(file$, Optional sheetName$ = "Sheet1") As IonPair()
        If file.ExtensionSuffix("xlsx") Then
            Return Xlsx.Open(path:=file) _
                .GetTable(sheetName) _
                .AsDataSource(Of IonPair) _
                .ToArray
        Else
            Return file.LoadCsv(Of IonPair)
        End If
    End Function

    <ExportAPI("read.reference")>
    Public Function readCompoundReference(file$, Optional sheetName$ = "Sheet1") As Standards()
        Dim reference As Standards()

        If file.ExtensionSuffix("xlsx") Then
            reference = Xlsx.Open(path:=file) _
                .GetTable(sheetName) _
                .AsDataSource(Of Standards) _
                .ToArray
        Else
            reference = file.LoadCsv(Of Standards)
        End If

        For i As Integer = 0 To reference.Length - 1
            reference(i).C = reference(i).C.ToLower
        Next

        Return reference
    End Function

    <ExportAPI("read.IS")>
    Public Function readIS(file$, Optional sheetName$ = "Sheet1") As [IS]()
        If file.ExtensionSuffix("xlsx") Then
            Return Xlsx.Open(path:=file) _
                .GetTable(sheetName) _
                .AsDataSource(Of [IS]) _
                .ToArray
        Else
            Return file.LoadCsv(Of [IS])
        End If
    End Function

    <ExportAPI("MRM.peaks")>
    Public Function ScanPeakTable(mzML$, ions As IonPair(),
                                  Optional peakAreaMethod% = 1,
                                  Optional TPAFactors As Dictionary(Of String, Double) = Nothing) As DataSet()

        Dim method As PeakArea.Methods = CType(peakAreaMethod, PeakArea.Methods)

        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Return WiffRaw.ScanPeakTable(mzML, ions, method, TPAFactors)
    End Function

    ''' <summary>
    ''' Scan the raw file data
    ''' </summary>
    ''' <param name="wiffConverts">A directory that contains the mzML files which are converts from the given wiff raw file.</param>
    ''' <param name="ions">Ion pairs definition data.</param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="TPAFactors"></param>
    ''' <param name="removesWiffName"></param>
    ''' <returns></returns>
    <ExportAPI("wiff.scans")>
    Public Function ScanStandardCurve(wiffConverts$(), ions As IonPair(),
                                      Optional peakAreaMethod% = 1,
                                      Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                      Optional removesWiffName As Boolean = True) As DataSet()

        Dim method As PeakArea.Methods = CType(peakAreaMethod, PeakArea.Methods)

        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        If wiffConverts.IsNullOrEmpty Then
            Throw New ArgumentNullException(NameOf(wiffConverts))
        Else
            Return WiffRaw.Scan(
                mzMLRawFiles:=wiffConverts,
                ions:=ions,
                peakAreaMethod:=method,
                TPAFactors:=TPAFactors,
                refName:=Nothing,
                removesWiffName:=removesWiffName
            )
        End If
    End Function

    ''' <summary>
    ''' Create linear fitting based on the wiff raw scan data.
    ''' </summary>
    ''' <param name="rawScan">The wiff raw scan data which are extract by function: ``wiff.scans``.</param>
    ''' <param name="calibrates"></param>
    ''' <param name="[ISvector]"></param>
    ''' <param name="autoWeighted">
    ''' If the unweighted R2 value of target standard curve is less than 0.99, 
    ''' then the quantify program will try weighted linear fitting. 
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("linears")>
    Public Function Linears(rawScan As DataSet(), calibrates As Standards(), [ISvector] As [IS](), Optional autoWeighted As Boolean = True) As StandardCurve()
        Return rawScan.ToDictionary.Regression(calibrates, ISvector, weighted:=autoWeighted).ToArray
    End Function

    <ExportAPI("models")>
    Public Function Models(detections As StandardCurve()) As FitModel()
        Return detections.DoCall(AddressOf StandardCurve.GetFitModels)
    End Function

    <ExportAPI("points")>
    Public Function GetLinearPoints(linears As StandardCurve(), name$) As MRMStandards()
        Dim line As StandardCurve = linears _
            .Where(Function(l)
                       Return l.name = name
                   End Function) _
            .FirstOrDefault

        If line Is Nothing Then
            Return Nothing
        Else
            Return line.points
        End If
    End Function
End Module

