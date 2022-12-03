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
