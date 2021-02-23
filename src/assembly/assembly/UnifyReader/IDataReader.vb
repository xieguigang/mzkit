Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Interface IDataReader

        Function GetScanTime(scan As Object) As Double
        Function GetScanId(scan As Object) As String
        Function IsEmpty(scan As Object) As Boolean

        Function GetMsMs(scan As Object) As ms2()
        Function GetMsLevel(scan As Object) As Integer
        Function GetBPC(scan As Object) As Double
        Function GetTIC(scan As Object) As Double
        Function GetParentMz(scan As Object) As Double
        Function GetPolarity(scan As Object) As String

    End Interface
End Namespace