Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData

Namespace mzData.mzWebCache

    Public Class mzMLScans : Inherits ScanPopulator(Of mzML.spectrum)

        Public Sub New(Optional mzErr$ = "da:0.1")
            MyBase.New(mzErr)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function loadScans(rawfile As String) As IEnumerable(Of mzML.spectrum)
            Return mzML.Xml.LoadScans(rawfile)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function dataReader() As MsDataReader(Of mzML.spectrum)
            Return New mzMLScan()
        End Function
    End Class
End Namespace