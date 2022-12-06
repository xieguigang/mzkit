Imports System

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

Namespace fidMath.FFT

    ''' <summary>
    ''' Performs phase correction to the transformed spectrum
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 18/02/2013
    ''' Time: 05:29
    ''' 
    ''' </summary>
    Public Class PhaseCorrectionTool

        Friend spectrum As Double()
        Friend processing As Proc
        Friend data As Double()

        Public Sub New(spectrum As Double(), processing As Proc)
            Me.spectrum = spectrum
            Me.processing = processing
            data = New Double(processing.TdEffective / 2 + 1 - 1) {}
        End Sub

        Public Overridable Function zeroOrderPhasing(angle As Double) As Double()
            For i = 0 To processing.TdEffective - 1 - 1 Step 2
                data(i / 2) = spectrum(i) * Math.Cos(angle) - spectrum(i + 1) * Math.Sin(angle)
            Next
            Return data
        End Function

        Public Overridable Function firstOrderPhasing(angle As Double) As Double()
            For i = 0 To processing.TdEffective - 1 - 1 Step 2
                data(i / 2) = spectrum(i) * Math.Cos(i / 2 * angle / (processing.TdEffective / 2)) + spectrum(i + 1) * Math.Sin(i / 2 * angle / (processing.TdEffective / 2))
            Next
            Return data
        End Function




    End Class

End Namespace
