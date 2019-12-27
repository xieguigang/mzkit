Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
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
End Module
