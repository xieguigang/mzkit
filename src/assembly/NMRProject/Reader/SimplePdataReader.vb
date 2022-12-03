Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO

' 
'  Copyright (c) 2013 EMBL, European Bioinformatics Institute.
' 
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
' 
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU Lesser General Public License for more details.
' 
'  You should have received a copy of the GNU Lesser General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.
' 

Namespace uk.ac.ebi.nmr.fid.io



    ''' <summary>
    ''' Class that reads the real and imaginary part of Bruker Processed data
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 08/05/2013
    ''' Time: 10:12
    ''' To change this template use File | Settings | File Templates.
    ''' </summary>
    Public Class SimplePdataReader
        Implements FidReader

        Private Shared pdataFolder As String
        Private Shared pdata1r As FileStream
        Private Shared pdata1i As FileStream
        Private Shared processing As Proc
        Private Shared acquisition As Acqu

        Public Sub New(ByVal pdataFolder As File, ByVal acquisition As Acqu, ByVal processing As Proc)
            Me.New(New FileStream(pdataFolder & "/1r", FileMode.Open, FileAccess.Read), New FileStream(pdataFolder & "/1i", FileMode.Open, FileAccess.Read), acquisition, processing)
            Me.pdataFolder = pdataFolder
        End Sub

        Public Sub New(ByVal pdata1r As FileStream, ByVal pdata1i As FileStream, ByVal acquisition As Acqu, ByVal processing As Proc)
            Me.pdata1r = pdata1r
            Me.pdata1i = pdata1i
            Me.acquisition = acquisition
            Me.processing = processing
        End Sub

        Public Overridable Function read() As Spectrum Implements FidReader.read
            Dim realDatapoints = getDatapoints(pdata1r)
            Dim imaginaryDatapoints = getDatapoints(pdata1i)
            Dim fid As Double() = Nothing

            ' trying to get the FID from the file structure as this is a required object to build a Spectrum object
            If pdataFolder IsNot Nothing Then
                'TODO move to java.nio.file
                ' check if one has a Unix or Windows based system
                Dim path As String() = If(System.getProperty("os.name").contains("Win"), pdataFolder.AbsolutePath.Split("\\"), pdataFolder.AbsolutePath.Split("/"))
                ' put path back...
                Dim fidPath As StringBuilder = New StringBuilder()
                If path.Length > 0 Then
                    fidPath.Append(path(0))
                    For i = 1 To path.Length - 2 - 1
                        fidPath.Append(If(System.getProperty(CStr("os.name")).contains("Win"), "\\", "/"))
                        fidPath.Append(path(i))
                    Next
                End If
                fidPath.Append(If(System.getProperty(CStr("os.name")).contains("Win"), "\\", "/"))
                fidPath.Append("fid")
                If Directory.Exists(fidPath.ToString()) OrElse File.Exists(fidPath.ToString()) Then
                    Dim fidReader As FidReader = New Simple1DFidReader(New FileStream(fidPath.ToString(), FileMode.Open, FileAccess.Read), acquisition)
                    Dim spectrum As Spectrum = fidReader.read()
                    fid = spectrum.Fid
                End If
            End If

            Dim spectrum As Spectrum = New Spectrum(fid, acquisition, processing)
            spectrum.ImaginaryChannelData = imaginaryDatapoints
            spectrum.RealChannelData = realDatapoints
            Return spectrum
        End Function

        Protected Friend Overridable Function getDatapoints(ByVal inputStream As FileStream) As Double()
            Dim datapoints As Double() = Nothing
            Dim inChannel As FileChannel = inputStream.Channel
            Dim buffer As ByteBuffer = inChannel.map(FileChannel.MapMode.READ_ONLY, 0, inChannel.size())
            If processing.is32Bit() Then
                Dim result As Integer() = New Integer(CInt(inChannel.size()) / 4 - 1) {}
                Console.WriteLine("Number of points in the processed spectra: " & inChannel.size() / 4)
                ' this has to do with the order of the bytes
                buffer.order(processing.ByteOrder)
                'read the integers
                Dim intBuffer As IntBuffer = buffer.asIntBuffer()
                intBuffer.[get](result)
                ' the number of points in 1r is defined in the procs file
                datapoints = New Double(processing.TdEffective - 1) {}
                ' Bruker only uses half of the points, so I will just pick the even positions
                For i = 0 To result.Length - 1 Step 2
                    datapoints(i / 2) = result(i)
                Next ' its a 64bit file encoding doubles
            Else
                Dim result As Double() = New Double(CInt(inChannel.size()) / 8 - 1) {}

                Console.WriteLine("Number of points in the the processed spectra: " & inChannel.size() / 8)
                ' this has to do with the order of the bytes
                buffer.order(processing.ByteOrder)
                'read the integers
                Dim doubleBuffer As DoubleBuffer = buffer.asDoubleBuffer()
                doubleBuffer.[get](result)
                ' Bruker only uses half of the points, so I will just pick the even positions
                ' making sure I have only the tdeff
                datapoints = New Double(processing.TdEffective - 1) {}
                For i = 0 To processing.TdEffective - 1
                    datapoints(i) = result(i * 2)
                Next
                '            System.arraycopy(result,0,datapoints,0,processing.getTdEffective());
            End If
            Return datapoints
        End Function
    End Class

End Namespace
