Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    Public Class ScanData

        Public Property totalIon As Double
        Public Property x As Integer
        Public Property y As Integer

        Public Property MzPtr As ibdPtr
        Public Property IntPtr As ibdPtr

        Sub New(scan As spectrum)
            totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
            x = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position x")?.value)
            y = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position y")?.value)
            MzPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(Scan0))
            IntPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(1))
        End Sub
    End Class

    Public Class ibdPtr

        Public Property offset As Long
        Public Property arrayLength As Integer
        Public Property encodedLength As Integer

        Public ReadOnly Property UnderlyingType As Type
            Get
                Dim sizeof As Integer = encodedLength / arrayLength

                If sizeof = 4 Then
                    Return GetType(Single)
                Else
                    Return GetType(Double)
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[&{offset.ToHexString}] new {UnderlyingType.Name.ToLower}({arrayLength} - 1){{}}"
        End Function

        Friend Shared Function ParsePtr(bin As binaryDataArray) As ibdPtr
            Dim arrayLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external array length").value)
            Dim offset As Long = Long.Parse(bin.cvParams.KeyItem("external offset").value)
            Dim encodedLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external encoded length").value)

            Return New ibdPtr With {
                .offset = offset,
                .arrayLength = arrayLength,
                .encodedLength = encodedLength
            }
        End Function
    End Class
End Namespace