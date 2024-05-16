#Region "Microsoft.VisualBasic::f8d4d98ba40e783cfcb72c1072015636, assembly\NMRFidTool\Math\apodization\AbstractApodizator.vb"

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

    '   Total Lines: 89
    '    Code Lines: 43
    ' Comment Lines: 34
    '   Blank Lines: 12
    '     File Size: 3.89 KB


    '     Class AbstractApodizator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) calculate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

Namespace fidMath.Apodization

    ''' <summary>
    ''' Abstract class that applies a window function to the fid in order to reduce noise or enhance signal.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 11:42
    ''' 
    ''' </summary>
    Public MustInherit Class AbstractApodizator
        Implements Apodizator

        Friend spectrum As Spectrum


        Public Sub New(spectrum As Spectrum)
            Me.spectrum = spectrum
        End Sub

        Public Overridable Function calculate() As Spectrum Implements Apodizator.calculate
            ' perhaps clone the spectrum otherwise one can have more clashes with the acqu and proc objects
            ' because of imutability issues
            Dim fid = New Double(spectrum.Fid.Length - 1) {}
            If spectrum.Acqu.getAcquisitionMode Is Acqu.AcquisitionMode.SEQUENTIAL Then
                Dim i = 0

                While i < spectrum.Proc.TdEffective AndAlso i < spectrum.Fid.Length
                    fid(i) = spectrum.RealChannelData(i) * calculateFactor(i)
                    i += 1
                End While
            Else
                For i As Integer = 0 To spectrum.Fid.Length / 2 - 1
                    ' set real values, i.e. even numbers
                    fid(i * 2) = spectrum.Fid(i * 2) * calculateFactor(i)
                    ' set imaginary values, i.e. odd numbers
                    fid(i * 2 + 1) = spectrum.Fid(i * 2 + 1) * calculateFactor(i)
                Next
            End If
            Return New Spectrum(fid, spectrum.Acqu, spectrum.Proc)
        End Function

        Public Overridable Function calculate(lineBroadning As Double) As Spectrum Implements Apodizator.calculate
            Dim fid = New Double(spectrum.Fid.Length - 1) {}
            ' this creates issues with immutability, need to clone the proc object
            '        spectrum.getProc().setLineBroadening(lineBroadning);
            If spectrum.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.SEQUENTIAL) Then
                Dim i = 0

                While i < spectrum.Proc.TdEffective AndAlso i < spectrum.Fid.Length
                    fid(i) = spectrum.Fid(i) * calculateFactor(i, lineBroadning)
                    i += 1
                End While
            Else
                For i As Integer = 0 To spectrum.Fid.Length / 2 - 1
                    ' set real values, i.e. even numbers
                    fid(i * 2) = spectrum.Fid(i * 2) * calculateFactor(i, lineBroadning)
                    ' set imaginary values, i.e. odd numbers
                    fid(i * 2 + 1) = spectrum.Fid(i * 2 + 1) * calculateFactor(i, lineBroadning)
                Next
            End If
            Return New Spectrum(fid, spectrum.Acqu, spectrum.Proc)
        End Function

        Protected Friend MustOverride Function calculateFactor(i As Integer, lineBroadening As Double) As Double

        Protected Friend MustOverride Function calculateFactor(i As Integer) As Double
    End Class

End Namespace
