#Region "Microsoft.VisualBasic::8a4adba733346c4cc150ff1348c4e620, G:/mzkit/src/assembly/NMRFidTool//Math/FFT/FastFourierTransform.vb"

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

    '   Total Lines: 38
    '    Code Lines: 6
    ' Comment Lines: 26
    '   Blank Lines: 6
    '     File Size: 1.25 KB


    '     Interface FastFourierTransform
    ' 
    '         Function: (+2 Overloads) computeFFT
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

Namespace fidMath.FFT

    ''' <summary>
    ''' @name    FastFourierTransform
    ''' @date    2013.01.31
    ''' @version $Rev$ : Last Changed $Date$
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' @author  pmoreno
    ''' @brief   interface for the FFT calss
    ''' 
    ''' </summary>
    Public Interface FastFourierTransform

        Function computeFFT() As Spectrum

        Function computeFFT(offset As Integer) As Spectrum

    End Interface

End Namespace
