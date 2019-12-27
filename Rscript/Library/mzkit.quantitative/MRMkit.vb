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

    <ExportAPI("wiff.standard_curve")>
    Public Function ScanStandardCurve(wiffConverts$, ions As IonPair(),
                                      Optional peakAreaMethod% = 1,
                                      Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                      Optional calibrationNamedPattern$ = ".+[-]CAL\d+",
                                      Optional levelPattern$ = "[-]CAL\d+") As DataSet()

        Dim method As PeakArea.Methods = CType(peakAreaMethod, PeakArea.Methods)

        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Return StandardCurve.Scan(
            raw:=wiffConverts,
            ions:=ions,
            peakAreaMethod:=method,
            TPAFactors:=TPAFactors,
            refName:=Nothing,
            calibrationNamedPattern:=calibrationNamedPattern,
            levelPattern:=levelPattern
        )
    End Function
End Module
