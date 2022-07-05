
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Namespace mzData.mzWebCache

    Public Module Serialization

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ms1"></param>
        ''' <param name="file">should be in little endian byte order</param>
        Public Sub ReadScan1(ms1 As ScanMS1, file As BinaryDataReader, Optional readmeta As Boolean = False)
            If readmeta Then
                file.ReadInt32()
                ms1.scan_id = file.ReadString(BinaryStringFormat.ZeroTerminated)
            End If

            ms1.rt = file.ReadDouble
            ms1.BPC = file.ReadDouble
            ms1.TIC = file.ReadDouble

            Dim nsize As Integer = file.ReadInt32
            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim into As Double() = file.ReadDoubles(nsize)

            ms1.mz = mz
            ms1.into = into
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <param name="file">should be in little endian byte order</param>
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <param name="file">should be in little endian byte order</param>
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file">should be in little endian byte order</param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadScanMs2(file As BinaryDataReader) As ScanMS2
            Dim ms2 As New ScanMS2 With {
                .scan_id = file.ReadString(BinaryStringFormat.ZeroTerminated),
                .parentMz = file.ReadDouble,
                .rt = file.ReadDouble,
                .intensity = file.ReadDouble,
                .polarity = file.ReadInt32,
                .charge = file.ReadInt32,
                .activationMethod = file.ReadByte,
                .collisionEnergy = file.ReadDouble,
                .centroided = file.ReadByte = 1
            }
            Dim productSize As Integer = file.ReadInt32

            ms2.mz = file.ReadDoubles(productSize)
            ms2.into = file.ReadDoubles(productSize)

            Return ms2
        End Function
    End Module
End Namespace