Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.MRM
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Module test

    Sub Main()

        Dim std = "D:\ProteoWizard.d\MRM_Test\MetaCardio_STD-X.csv".LoadCsv(Of Standards)
        Dim [IS] As [IS]() = "D:\ProteoWizard.d\MRM_Test\IS.csv".LoadCsv(Of [IS])
        Dim ion_pairs = "D:\ProteoWizard.d\MRM_Test\ion_pairs.csv".LoadCsv(Of IonPair)

        Dim fits As NamedValue(Of FitResult)() = Nothing
        Dim X As List(Of DataSet) = Nothing
        Dim result = MRMSamples.QuantitativeAnalysis("D:\ProteoWizard.d\Data20180313\Data20180313.wiff", ion_pairs, std, [IS], fits, X, calibrationNamedPattern:=".+M1[-]L\d+").ToArray


        Pause()
    End Sub
End Module
