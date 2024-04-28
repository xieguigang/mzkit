#Region "Microsoft.VisualBasic::e0e26cdf546e7ecf5c04c9784a6b49e9, E:/mzkit/src/assembly/NMRFidTool//Reader/SimplePdataReader.vb"

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

    '   Total Lines: 117
    '    Code Lines: 63
    ' Comment Lines: 41
    '   Blank Lines: 13
    '     File Size: 5.44 KB


    '     Class SimplePdataReader
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: getDatapoints, read
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
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

Namespace Reader

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

        Dim pdataFolder As String
        Dim pdata1r As FileStream
        Dim pdata1i As FileStream
        Dim processing As Proc
        Dim acquisition As Acqu

        Public Sub New(pdataFolder As String, acquisition As Acqu, processing As Proc)
            Me.New(New FileStream(pdataFolder & "/1r", FileMode.Open, FileAccess.Read), New FileStream(pdataFolder & "/1i", FileMode.Open, FileAccess.Read), acquisition, processing)
            Me.pdataFolder = pdataFolder
        End Sub

        Public Sub New(pdata1r As FileStream, pdata1i As FileStream, acquisition As Acqu, processing As Proc)
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
                ' put path back...
                Dim fidPath As String = $"{pdataFolder}/fid"

                If Directory.Exists(fidPath) OrElse File.Exists(fidPath) Then
                    Dim fidReader As FidReader = New Simple1DFidReader(New FileStream(fidPath.ToString(), FileMode.Open, FileAccess.Read), acquisition)
                    Dim spectrumAny As Spectrum = fidReader.read()
                    fid = spectrumAny.Fid
                End If
            End If

            Dim spectrum As Spectrum = New Spectrum(fid, acquisition, processing)
            spectrum.ImaginaryChannelData = imaginaryDatapoints
            spectrum.RealChannelData = realDatapoints
            Return spectrum
        End Function

        Protected Friend Overridable Function getDatapoints(inputStream As FileStream) As Double()
            Dim datapoints As Double() = Nothing
            Dim buffer As New ByteBuffer(inputStream)
            If processing.is32Bit() Then
                Dim result As Integer() = New Integer(CInt(buffer.byteLength / 4) - 1) {}
                Console.WriteLine("Number of points in the processed spectra: " & buffer.byteLength / 4)
                ' this has to do with the order of the bytes
                buffer.order(processing.ByteOrder)
                'read the integers
                ' Dim intBuffer As IntBuffer = buffer.asIntBuffer()
                buffer.[get](result)
                ' the number of points in 1r is defined in the procs file
                datapoints = New Double(processing.TdEffective - 1) {}
                ' Bruker only uses half of the points, so I will just pick the even positions
                For i As Integer = 0 To result.Length - 1 Step 2
                    datapoints(i / 2) = result(i)
                Next ' its a 64bit file encoding doubles
            Else
                Dim result As Double() = New Double(CInt(buffer.byteLength / 8) - 1) {}

                Console.WriteLine("Number of points in the the processed spectra: " & buffer.byteLength / 8)
                ' this has to do with the order of the bytes
                buffer.order(processing.ByteOrder)
                'read the integers
                ' Dim doubleBuffer As DoubleBuffer = buffer.asDoubleBuffer()
                buffer.[get](result)
                ' Bruker only uses half of the points, so I will just pick the even positions
                ' making sure I have only the tdeff
                datapoints = New Double(processing.TdEffective - 1) {}
                For i As Integer = 0 To processing.TdEffective - 1
                    datapoints(i) = result(i * 2)
                Next
                '            System.arraycopy(result,0,datapoints,0,processing.getTdEffective());
            End If
            Return datapoints
        End Function
    End Class

End Namespace
