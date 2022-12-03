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

Namespace fidMath.Phasing

    ''' <summary>
    ''' Method to perform phase correction
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/05/2013
    ''' Time: 14:47
    ''' 
    ''' </summary>
    Public Class SimplePhaseCorrector
        Implements PhaseCorrector

        Public Overridable Function phaseCorrection(ByVal spectrum As Spectrum, ByVal zeroPhase As Double, ByVal firstOrderPhase As Double) As Spectrum Implements PhaseCorrector.phaseCorrection
            Return phaseCorrection(spectrum, zeroPhase, firstOrderPhase, 0)
        End Function

        ''' <summary>
        ''' Implementation based in the definitions from paper {Chen:2012vj}
        ''' </summary>
        ''' <paramname="spectrum"> </param>
        ''' <paramname="zeroPhase"> </param>
        ''' <paramname="firstOrderPhase"> </param>
        ''' <paramname="pivot">
        ''' @return </param>
        Public Overridable Function phaseCorrection(ByVal spectrum As Spectrum, ByVal zeroPhase As Double, ByVal firstOrderPhase As Double, ByVal pivot As Integer) As Spectrum Implements PhaseCorrector.phaseCorrection
            Dim realChannel = New Double(spectrum.Acqu.AquiredPoints / 2 - 1) {}
            Dim imaginaryChannel = New Double(spectrum.Acqu.AquiredPoints / 2 - 1) {}

            If pivot > spectrum.Acqu.AquiredPoints / 2 OrElse pivot < 0 Then
                Try
                    Throw New Exception(" pivot is incorrectly set")
                Catch e As Exception
                    Console.WriteLine(e.ToString())
                    Console.Write(e.StackTrace) 'To change body of catch statement use File | Settings | File Templates.
                End Try
            End If

            For i As Integer = 0 To spectrum.Acqu.AquiredPoints / 2 - 1
                Dim phaseAngle = 2 * Math.PI / 360 * (i / (spectrum.Fid.Length / 2) * firstOrderPhase + zeroPhase)
                realChannel(i) = spectrum.RealChannelData(i) * Math.Cos(phaseAngle) - spectrum.ImaginaryChannelData(i) * Math.Sin(phaseAngle)
                imaginaryChannel(i) = spectrum.RealChannelData(i) * Math.Sin(phaseAngle) + spectrum.ImaginaryChannelData(i) * Math.Cos(phaseAngle)
            Next

            spectrum.RealChannelData = realChannel
            spectrum.ImaginaryChannelData = imaginaryChannel
            Return spectrum
        End Function
    End Class
End Namespace
