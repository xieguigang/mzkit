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

Namespace fidMath.Apodization

    ''' 
    ''' <summary>
    ''' Applies a Sine Squared window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 15:22
    ''' </summary>
    Public Class SineSquaredApodizator
        Inherits AbstractApodizator

        Protected Friend Sub New(spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub

        Public Overrides Function calculate() As Spectrum

            Dim factor As Double

            ' check the loop conditions in the original code...
            Dim i = 0

            While i < spectrum.Proc.TdEffective - 1 AndAlso i >= spectrum.Proc.TdEffective - 1 - spectrum.RealChannelData.Length * 2
                Try
                    spectrum.setImaginaryChannelData(spectrum.Proc.TdEffective - 2 - i, spectrum.RealChannelData(spectrum.Proc.TdEffective - 2 - i) * calculateFactor(i))
                    spectrum.setRealChannelData(spectrum.Proc.TdEffective - 1 - i, spectrum.RealChannelData(spectrum.Proc.TdEffective - 1 - i) * calculateFactor(i))
                Catch e As Exception
                    Console.WriteLine(e.ToString())
                    Console.Write(e.StackTrace) 'To change body of catch statement use File | Settings | File Templates.
                End Try

                i += 2

            End While
            Return spectrum
        End Function

        Protected Friend Overloads Overrides Function calculateFactor(i As Integer, lineBroadening As Double) As Double
            Return calculateFactor(i)
        End Function

        Protected Friend Overloads Overrides Function calculateFactor(i As Integer) As Double
            Dim offset = (180.0 - spectrum.Proc.SsbSineSquared) / 180.0
            Return Math.Pow(Math.Sin(i / spectrum.Proc.TdEffective * Math.PI * offset), 2)
        End Function
    End Class

End Namespace
