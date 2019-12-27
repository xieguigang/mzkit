Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

<Package("mzkit.mrm")>
Public Module MRMkit

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
End Module
