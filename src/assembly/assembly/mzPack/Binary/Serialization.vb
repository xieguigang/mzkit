
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Namespace mzData.mzWebCache

    Public Module Serialization

        <Extension>
        Public Sub WriteScan1(scan As ScanMS1, file As BinaryDataWriter)
            ' write MS1 scan information
            ' this first zero int32 is a 
            ' placeholder for indicate the byte size
            ' of this ms1 data region
            Call file.Write(0)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.rt)
            Call file.Write(scan.BPC)
            Call file.Write(scan.TIC)
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
            Call file.Flush()
        End Sub

        <Extension>
        Public Sub WriteBuffer(scan As ScanMS2, file As BinaryDataWriter)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.parentMz)
            Call file.Write(scan.rt)
            Call file.Write(scan.intensity)
            Call file.Write(scan.polarity)
            Call file.Write(scan.charge)
            Call file.Write(scan.activationMethod)
            Call file.Write(scan.collisionEnergy)
            Call file.Write(CByte(If(scan.centroided, 1, 0)))
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
        End Sub
    End Module
End Namespace