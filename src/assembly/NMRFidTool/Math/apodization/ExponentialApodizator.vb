#Region "Microsoft.VisualBasic::2d51b177fdc616bdde87e6042eb4db2a, G:/mzkit/src/assembly/NMRFidTool//Math/apodization/ExponentialApodizator.vb"

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

    '   Total Lines: 65
    '    Code Lines: 15
    ' Comment Lines: 39
    '   Blank Lines: 11
    '     File Size: 2.38 KB


    '     Class ExponentialApodizator
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
    ''' Applies an exponential window function to the fid.
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 03/04/2013
    ''' Time: 11:41
    ''' 
    ''' </summary>
    Public Class ExponentialApodizator
        Inherits AbstractApodizator

        Public Sub New(spectrum As Spectrum)
            MyBase.New(spectrum)
        End Sub

        ''' <summary>
        ''' calculates the weigth for the exponential apodization: W(i)= exp(-i*dw*lb*Pi)
        ''' where i*dw gives the time coordinate in (s) and the line broadening (as defined in the acqu) is in Hz.
        ''' The cuteNMR implementation is W(i) = exp(-(dw*i) * lb * pi)
        ''' In Vogt 2004: W(i)=exp(-lb*(i*dw)^2))
        ''' </summary>
        ''' <param name="i">
        ''' @return </param>
        Protected Friend Overloads Overrides Function calculateFactor(i As Integer) As Double
            Return Math.Exp(-i * spectrum.Proc.DwellTime * spectrum.Proc.LineBroadening * Math.PI)
        End Function

        ''' <summary>
        ''' performs the exponential apodization with specified line broadening.
        ''' </summary>
        ''' <param name="lineBroadening"> </param>
        ''' <exception cref="Exception"> </exception>
        Protected Friend Overloads Overrides Function calculateFactor(i As Integer, lineBroadening As Double) As Double
            Return Math.Exp(-i * spectrum.Proc.DwellTime * lineBroadening * Math.PI)
        End Function




    End Class

End Namespace
