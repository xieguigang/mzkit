#Region "Microsoft.VisualBasic::84e1403d2f7ee43f7ad3a11af77b7765, G:/mzkit/src/assembly/NMRFidTool//Reader/Simple1DFidReader.vb"

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

    '   Total Lines: 83
    '    Code Lines: 41
    ' Comment Lines: 30
    '   Blank Lines: 12
    '     File Size: 3.23 KB


    '     Class Simple1DFidReader
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: read
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

' 
'  Copyright (c) 2013. EMBL, European Bioinformatics Institute
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
    ''' Reader for Bruker's fid file.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' </summary>
    Public Class Simple1DFidReader
        Implements FidReader

        Private acquisition As Acqu
        Private processing As Proc
        Private fidInput As FileStream
        Private zeroFrequency As Double = 0


        Public Sub New(fidInputS As FileStream, acquisition As Acqu)
            Me.acquisition = acquisition
            fidInput = fidInputS
        End Sub

        Public Sub New(fidInputS As FileStream, acquisition As Acqu, processing As Proc)
            Me.acquisition = acquisition
            fidInput = fidInputS
            Me.processing = processing
        End Sub

        Public Overridable Function read() As Spectrum Implements FidReader.read
            Dim fid As Double() = Nothing
            '  Dim inChannel As FileChannel = fidInput.Channel
            Dim buffer As New ByteBuffer(fidInput)
            If acquisition.is32Bit() Then
                Dim result As Integer() = New Integer(CInt(fidInput.Length / 4) - 1) {}
                Console.WriteLine("Number of points in the fid: " & fidInput.Length / 4)
                ' this has to do with the order of the bytes
                buffer.order(acquisition.ByteOrder)
                'read the integers
                ' Dim intBuffer As IntBuffer = buffer.asIntBuffer()
                ' intBuffer.[get](result)
                buffer.get(result)
                fid = New Double(acquisition.AquiredPoints - 1) {}
                For i = 0 To fid.Length - 1
                    fid(i) = result(i)
                Next ' its a 64bit file encoding doubles
            Else
                Dim result As Double() = New Double(CInt(fidInput.Length / 8) - 1) {}
                Console.WriteLine("Number of points in the fid: " & fidInput.Length / 8)
                ' this has to do with the order of the bytes
                buffer.order(acquisition.ByteOrder)
                'read the integers
                ' Dim doubleBuffer As DoubleBuffer = buffer.asDoubleBuffer()
                buffer.[get](result)
                Array.Copy(result, 0, fid, 0, acquisition.AquiredPoints)
            End If

            Return New Spectrum(fid, acquisition)

        End Function
    End Class

End Namespace
