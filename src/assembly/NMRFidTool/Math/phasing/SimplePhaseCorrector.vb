#Region "Microsoft.VisualBasic::5b05356a6da8a837c29d151978b43d9e, assembly\NMRFidTool\Math\phasing\SimplePhaseCorrector.vb"

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

    '   Total Lines: 71
    '    Code Lines: 29 (40.85%)
    ' Comment Lines: 34 (47.89%)
    '    - Xml Docs: 44.12%
    ' 
    '   Blank Lines: 8 (11.27%)
    '     File Size: 3.20 KB


    '     Class SimplePhaseCorrector
    ' 
    '         Function: (+2 Overloads) phaseCorrection
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Public Overridable Function phaseCorrection(spectrum As Spectrum, zeroPhase As Double, firstOrderPhase As Double) As Spectrum Implements PhaseCorrector.phaseCorrection
            Return phaseCorrection(spectrum, zeroPhase, firstOrderPhase, 0)
        End Function

        ''' <summary>
        ''' Implementation based in the definitions from paper {Chen:2012vj}
        ''' </summary>
        ''' <param name="spectrum"> </param>
        ''' <param name="zeroPhase"> </param>
        ''' <param name="firstOrderPhase"> </param>
        ''' <param name="pivot">
        ''' @return </param>
        Public Overridable Function phaseCorrection(spectrum As Spectrum, zeroPhase As Double, firstOrderPhase As Double, pivot As Integer) As Spectrum Implements PhaseCorrector.phaseCorrection
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
