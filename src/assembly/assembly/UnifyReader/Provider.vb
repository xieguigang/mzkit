Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Module Provider

        Public Function GetMsMs(Of Scan)() As Func(Of Scan, ms2())
            Dim reader = MsDataReader(Of Scan).ScanProvider

            Select Case GetType(Scan)
                Case GetType(mzXML.scan)
                    Return CObj(New Func(Of mzXML.scan, ms2())(AddressOf DirectCast(reader, mzXMLScan).GetMsMs))
                Case GetType(mzML.spectrum)
                    Return CObj(New Func(Of mzML.spectrum, ms2())(AddressOf DirectCast(reader, mzMLScan).GetMsMs))
                Case Else
                    Throw New NotImplementedException(GetType(Scan).FullName)
            End Select
        End Function

    End Module
End Namespace