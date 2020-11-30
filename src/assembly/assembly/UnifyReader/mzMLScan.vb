Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace DataReader

    Public Class mzMLScan : Inherits MsDataReader(Of spectrum)

        Public Overrides Function GetScanTime(scan As spectrum) As Double
            Return scan.scan_time
        End Function

        Public Overrides Function GetScanId(scan As spectrum) As Double
            Dim scanType As String = scan.scanList.scans(0).cvParams.KeyItem("filter string")?.value
            Dim polarity As String = GetPolarity(scan)

            If scan.ms_level = 1 Then
                Return $"[MS1] {scanType}, ({polarity}) retentionTime={CInt(scan.scan_time)}"
            Else
                Return $"[MS/MS] {scanType}, ({polarity}) M{CInt(scan.selectedIon.mz)}T{CInt(scan.scan_time)}"
            End If
        End Function

        Public Overrides Function IsEmpty(scan As spectrum) As Boolean
            Return True
        End Function

        Public Overrides Function GetMsMs(scan As spectrum) As ms2()
            Dim mz As Double() = scan.ByteArray("m/z array").Base64Decode
            Dim intensity = scan.ByteArray("intensity array").Base64Decode
            Dim msms As ms2() = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = intensity(i),
                                .quantity = .intensity
                            }
                        End Function) _
                .ToArray

            Return msms
        End Function

        Public Overrides Function GetMsLevel(scan As spectrum) As Integer
            Return CInt(Val(scan.ms_level))
        End Function

        Public Overrides Function GetBPC(scan As spectrum) As Double
            Return Val(scan.cvParams.KeyItem("base peak intensity")?.value)
        End Function

        Public Overrides Function GetTIC(scan As spectrum) As Double
            Return Val(scan.cvParams.KeyItem("total ion current")?.value)
        End Function

        Public Overrides Function GetParentMz(scan As spectrum) As Double
            Return scan.selectedIon.mz
        End Function

        Public Overrides Function GetPolarity(scan As spectrum) As String
            If Not scan.cvParams.KeyItem("positive scan") Is Nothing Then
                Return "+"
            Else
                Return "-"
            End If
        End Function
    End Class
End Namespace