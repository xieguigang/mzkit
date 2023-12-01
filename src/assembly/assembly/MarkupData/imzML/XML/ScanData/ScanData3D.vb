Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    Public Class ScanData3D : Implements ImageScan

        ''' <summary>
        ''' TIC
        ''' </summary>
        ''' <returns></returns>
        Public Property totalIon As Double
        Public Property x As Double
        Public Property y As Double
        Public Property z As Double
        Public Property MzPtr As ibdPtr Implements ImageScan.MzPtr
        Public Property IntPtr As ibdPtr Implements ImageScan.IntPtr

        Sub New(scan As spectrum)
            MzPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(Scan0))
            IntPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(1))

            If Not scan.cvParams Is Nothing Then
                totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
            End If

            Call XML.Get3DPositionXYZ(scan, x, y, z)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}, {z}] {totalIon.ToString("F3")}"
        End Function
    End Class

End Namespace