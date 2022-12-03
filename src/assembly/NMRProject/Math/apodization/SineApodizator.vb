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

Namespace uk.ac.ebi.nmr.fid.tools.apodization

    ''' <summary>
    ''' Applies a Sine Bell window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 15:18
    ''' 
    ''' </summary>
    Public Class SineApodizator
        Inherits AbstractApodizator

        Protected Friend Sub New(ByVal spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub
        'TODO fix this class
        Public Overrides Function calculate() As Spectrum

            Dim factor As Double
            ' ssbSine is defined as 180/ssb
            ' TODO allow to redifine
            Dim offset = (180.0 - spectrum.Proc.SsbSine) / 180.0
            ' check the loop conditions in the original code...
            Dim i = 0

            While i < spectrum.Proc.TdEffective - 1 AndAlso i >= spectrum.Proc.TdEffective - 1 - spectrum.RealChannelData.Length * 2
                factor = Math.Sin(i / spectrum.Proc.TdEffective * Math.PI * offset)
                spectrum.setRealChannelData(spectrum.Proc.TdEffective - 1 - i, spectrum.RealChannelData(spectrum.Proc.TdEffective - 1 - i) * factor)
                spectrum.setImaginaryChannelData(spectrum.Proc.TdEffective - 2 - i, spectrum.ImaginaryChannelData(spectrum.Proc.TdEffective - 2 - i) * factor)
                i += 2
            End While
            Return spectrum
        End Function

        Protected Friend Overrides Overloads Function calculateFactor(ByVal i As Integer, ByVal lineBroadening As Double) As Double
            Return 0 'To change body of implemented methods use File | Settings | File Templates.
        End Function

        Protected Friend Overrides Overloads Function calculateFactor(ByVal i As Integer) As Double
            Dim offset = (180.0 - spectrum.Proc.SsbSine) / 180.0

            Return 0 'To change body of implemented methods use File | Settings | File Templates.
        End Function
    End Class

End Namespace
