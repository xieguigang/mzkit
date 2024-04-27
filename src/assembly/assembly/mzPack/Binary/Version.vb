#Region "Microsoft.VisualBasic::4c366d1a47feece02a6b7bfb5281783b, G:/mzkit/src/assembly/assembly//mzPack/Binary/Version.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 40
    '    Code Lines: 22
    ' Comment Lines: 10
    '   Blank Lines: 8
    '     File Size: 1.28 KB


    '     Enum Version
    ' 
    '         Version1
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module VersionHelper
    ' 
    '         Function: DecodeVersion, GetVersion
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
