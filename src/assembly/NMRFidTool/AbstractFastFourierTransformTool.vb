#Region "Microsoft.VisualBasic::5cd1426f924d71c156fab95a65aa3004, assembly\NMRFidTool\AbstractFastFourierTransformTool.vb"

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

'   Total Lines: 171
'    Code Lines: 98 (57.31%)
' Comment Lines: 47 (27.49%)
'    - Xml Docs: 27.66%
' 
'   Blank Lines: 26 (15.20%)
'     File Size: 6.67 KB


' Class AbstractFastFourierTransformTool
' 
'     Properties: Acquisition, Data, Fid, Processing
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: computeFFT, getRealPart
' 
'     Sub: applyRedfieldOnSequentialData, initDataFormat, setData, shiftData, tdRelatedNonUnderstoodRearrangementForSequential
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.NMRFidTool.fidMath.Apodization
Imports std = System.Math

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

''' <summary>
''' @name    AbstractFastFourierTransformTool
''' @date    2013.01.31
''' @version $Rev$ : Last Changed $Date$
''' @author  Luis F. de Figueiredo
''' @author  pmoreno
''' @author  $Author$ (this version)
''' @brief   Abstract Fast Fourier Transform Tool class to be extended by all FFT implementations.
''' 
''' </summary>
Public MustInherit Class AbstractFastFourierTransformTool

    ''' <summary>
    ''' proc_buffer   apodization::transform::do_fft
    ''' </summary>
    Friend m_data As Double()
    ''' <summary>
    ''' proc_buffer   apodization::transform::do_fft
    ''' </summary>
    Friend m_fid As Spectrum


    Public Sub New()
    End Sub

    Friend Overridable Sub applyRedfieldOnSequentialData()
        ' for sequential data apply Redfield trick: multiply data by 1 -1 -1 1
        If m_fid.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.SEQUENTIAL) Then
            For i = 0 To m_fid.Proc.TdEffective - 1 Step 4
                m_data(i + 1) = -m_data(i + 1)
                m_data(i + 2) = -m_data(i + 2)
            Next
        End If
    End Sub

    Public Overridable Function computeFFT() As Double()
        initDataFormat()
        shiftData()
        applyRedfieldOnSequentialData()
        tdRelatedNonUnderstoodRearrangementForSequential()

        m_fid.Proc.LineBroadening = 0.3
        ' applyWindowFunctions //window function need to be applied before FT
        'TODO adapt the FFT to the new object Spectrum
        'Dim apodization As New ExponentialApodizator(Data, Acquisition.getAcquisitionMode(), Processing)
        Dim apodizedData As Double() ' = apodization.calculate()

        Return implementedFFT(apodizedData)
    End Function

    Friend Overridable Function getRealPart(data As Double()) As Double()
        Dim realPart = New Double(data.Length / 2 - 1) {}
        Dim realIndex = 0
        For i = 0 To data.Length - 1 Step 2
            realPart(Math.Min(Threading.Interlocked.Increment(realIndex), realIndex - 1)) = data(i)
        Next
        Return realPart
    End Function



    Public Overridable Property Data As Double()
        Get
            Return m_data
        End Get
        Set(value As Double())
            m_data = value
        End Set
    End Property

    Public Overridable ReadOnly Property Fid As Double()
        Get
            Return m_fid.Fid
        End Get
    End Property



    ''' <summary>
    ''' This is where the implementation of each fft package go. </summary>
    ''' <param name="apodizedData">
    ''' @return </param>
    Friend MustOverride Function implementedFFT(apodizedData As Double()) As Double()

    Friend Overridable Sub initDataFormat()
        ' instanciating the array where the fourier transformed spectra will be stored....
        Dim mode = m_fid.Acqu.getAcquisitionMode

        If mode Is Acqu.AcquisitionMode.DISP OrElse mode Is Acqu.AcquisitionMode.SIMULTANIOUS Then
            ' allocate space for processing
            m_data = New Double(2 * m_fid.Proc.TransformSize - 1) {} ' allocate space for processing
        ElseIf mode Is Acqu.AcquisitionMode.SEQUENTIAL Then
            ' allocate space for processing
            m_data = New Double(4 * m_fid.Proc.TransformSize - 1) {} ' allocate space for processing
        Else
        End If
    End Sub


    Public Overridable Sub setData(index As Integer, point As Double)
        m_data(index) = point
    End Sub

    Friend Overridable Sub shiftData()
        ' perform a left or right shift of the data (ignoring the corresponding portion of the data)
        ' the code from cuteNMR was simplified
        For i As Integer = 0 To m_fid.Proc.TdEffective - std.Abs(m_fid.Proc.Shift) - 1
            Dim dataIndex = If(m_fid.Proc.Shift >= 0, i, i - m_fid.Proc.Shift) ' or shift the placement of the data to the right
            ' or shift the placement of the data to the right
            Dim fidIndex = If(m_fid.Proc.Shift >= 0, i + m_fid.Proc.Shift, i) ' start in the correct order
            ' start in the correct order
            Dim type = m_fid.Acqu.FidType

            If type Is Acqu.FidData.INT32 Then
                m_data(dataIndex) = m_fid.Fid(fidIndex)
            ElseIf type Is Acqu.FidData.[DOUBLE] Then
                m_data(dataIndex) = m_fid.Fid(fidIndex)
            ElseIf type Is Acqu.FidData.FLOAT Then
            ElseIf type Is Acqu.FidData.INT16 Then
            Else
            End If
        Next
    End Sub

    Friend Overridable Sub tdRelatedNonUnderstoodRearrangementForSequential()
        ' try to understand this bit of code!!!!
        'nonsense case if SI is set to be less than TD/2
        Dim td = If(m_fid.Proc.TdEffective > 2 * m_fid.Proc.TransformSize, 2 * m_fid.Proc.TransformSize, m_fid.Proc.TdEffective)
        ' move the data from position i to position 2*i, why?
        If m_fid.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.SEQUENTIAL) Then
            For i = td - 1 To 1 Step -1
                m_data(2 * i) = m_data(i)
                m_data(i) = 0
            Next
        End If
        ''''''''''''''''''''''''''''''''''''''''''''
    End Sub

    Public Overridable ReadOnly Property Processing As Proc
        Get
            Return m_fid.Proc
        End Get
    End Property

    Public Overridable ReadOnly Property Acquisition As Acqu
        Get
            Return m_fid.Acqu
        End Get
    End Property
End Class
