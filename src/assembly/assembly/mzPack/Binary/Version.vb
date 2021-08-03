Imports Microsoft.VisualBasic.Data.IO

Namespace mzData.mzWebCache

    ''' <summary>
    ''' the mzpack file format version code
    ''' </summary>
    ''' <remarks>
    ''' use byte mask as version string,
    ''' integer (4 bytes)
    ''' main, [minor minor minor]
    ''' main [1,255]
    ''' sub  [0, minor1, minor2, minor3] (integer)
    ''' </remarks>
    Public Enum Version As Integer
        Version1
    End Enum

    Public Module VersionHelper

        Public Function GetVersion(main As Byte, minor As Integer) As Integer
            Dim versionBytes As Byte() = BitConverter.GetBytes(minor)
            versionBytes(Scan0) = main
            Return BitConverter.ToInt32(versionBytes, Scan0)
        End Function

        Public Function DecodeVersion(ver As Integer) As System.Version
            Dim bytes As Byte() = BitConverter.GetBytes(ver)

            If ByteOrderHelper.SystemByteOrder = ByteOrder.LittleEndian Then
                Call Array.Reverse(bytes)
            End If

            Dim main As Byte = bytes(Scan0)
            Dim minor As Integer = BitConverter.ToInt32({0, bytes(1), bytes(2), bytes(3)}, Scan0)

            Return New System.Version(main, minor)
        End Function
    End Module
End Namespace