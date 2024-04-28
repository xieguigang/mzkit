#Region "Microsoft.VisualBasic::472863564ce36c6b9c533a3d55bde3ec, E:/mzkit/src/assembly/NMRFidTool//Math/apodization/GaussianApodizator.vb"

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
    '    Code Lines: 20
    ' Comment Lines: 40
    '   Blank Lines: 11
    '     File Size: 2.74 KB


    '     Class GaussianApodizator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) calculateFactor
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

Namespace fidMath.Apodization

    ''' <summary>
    ''' Applies a gaussian window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 12:32
    ''' 
    ''' </summary>
    Public Class GaussianApodizator
        Inherits AbstractApodizator

        Public Sub New(spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub


        ''' <summary>
        ''' calculates the weigth for the guassian apodization: W(i)=exp(-2(i*dw*lb)^2)
        ''' where i*dw gives the time coordinate in (s) and sigma is the line broadening with default value 0.1 Hz.
        ''' The cuteNMR implementation is de facto W(i)=exp(-2(i*dw*lb)^2))
        ''' In Vogt 2004: W(i)=exp(-lb*(i*dw)^2))
        ''' In Wikipedia: W(i)=exp(-1/2*(((n-(N-1)/2))/(sigma * (N-1)/2))^2))
        ''' </summary>
        ''' <exception cref="Exception"> </exception>
        Protected Friend Overloads Overrides Function calculateFactor(i As Integer) As Double
            spectrum.Proc.LineBroadening = 0.1
            Return calculateFactor(i, 0.1)
        End Function

        ''' <summary>
        ''' performs the guassian apodization with specified line broadning. </summary>
        ''' <param name="lbGauss"> </param>
        ''' <exception cref="Exception"> </exception>
        Protected Friend Overloads Overrides Function calculateFactor(i As Integer, lbGauss As Double) As Double

            If lbGauss = 0 Then
                Throw New Exception("line broadening cannot be zero")
            End If
            ' original expression: (1.0/std::sqrt(2))/par->lbGauss * (1.0/std::sqrt(2))/par->lbGauss;
            '        double sigmaFactor=0.5*Math.pow(1/lbGauss,2);

            Dim time = i * spectrum.Proc.DwellTime
            Return Math.Exp(-2 * Math.Pow(time * lbGauss, 2))
        End Function

    End Class

End Namespace
