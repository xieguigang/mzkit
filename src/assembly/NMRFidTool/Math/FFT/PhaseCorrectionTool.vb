#Region "Microsoft.VisualBasic::166b79c283f87421be3d585aa04ecf74, assembly\NMRFidTool\Math\FFT\PhaseCorrectionTool.vb"

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

    '   Total Lines: 63
    '    Code Lines: 25
    ' Comment Lines: 26
    '   Blank Lines: 12
    '     File Size: 2.15 KB


    '     Class PhaseCorrectionTool
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: firstOrderPhasing, zeroOrderPhasing
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
