#Region "Microsoft.VisualBasic::66c28ed87ef11d52865ddc97e6e544bb, mzkit\src\assembly\NMRFidTool\AbstractFastFourierTransformTool.vb"

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

'   Total Lines: 169
'    Code Lines: 97
' Comment Lines: 47
'   Blank Lines: 25
'     File Size: 6.48 KB


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


    Friend dataField As Double() 'proc_buffer   apodization::transform::do_fft
    'proc_buffer   apodization::transform::do_fft
    Friend fidField As Spectrum


    Public Sub New()
    End Sub

    Friend Overridable Sub applyRedfieldOnSequentialData()
        ' for sequential data apply Redfield trick: multiply data by 1 -1 -1 1
        If fidField.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.SEQUENTIAL) Then
            For i = 0 To fidField.Proc.TdEffective - 1 Step 4
                dataField(i + 1) = -dataField(i + 1)
                dataField(i + 2) = -dataField(i + 2)
            Next
        End If
    End Sub

    Public Overridable Function computeFFT() As Double()
        initDataFormat()
        shiftData()
        applyRedfieldOnSequentialData()
        tdRelatedNonUnderstoodRearrangementForSequential()
        Dim signals As Integer
        fidField.Proc.LineBroadening = 0.3
        ''' applyWindowFunctions //window function need to be applied before FT
        'TODO adapt the FFT to the new object Spectrum
        '        Apodizator apodization = new ExponentialApodizator(data, acquisition.getAcquisitionMode(), processing);
        Dim apodizedData As Double() = Nothing

        '            apodizedData = apodization.calculate();
        Try
        Catch e As Exception
            Console.WriteLine(e.ToString())
            Console.Write(e.StackTrace) 'To change body of catch statement use File | Settings | File Templates.
        End Try
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
            Return dataField
        End Get
        Set(value As Double())
            dataField = value
        End Set
    End Property

    Public Overridable ReadOnly Property Fid As Double()
        Get
            Return fidField.Fid
        End Get
    End Property



    ''' <summary>
    ''' This is where the implementation of each fft package go. </summary>
    ''' <param name="apodizedData">
    ''' @return </param>
    Friend MustOverride Function implementedFFT(apodizedData As Double()) As Double()

    Friend Overridable Sub initDataFormat()
        ' instanciating the array where the fourier transformed spectra will be stored....
        Dim mode = fidField.Acqu.getAcquisitionMode

        If mode Is Acqu.AcquisitionMode.DISP OrElse mode Is Acqu.AcquisitionMode.SIMULTANIOUS Then
            ' allocate space for processing
            dataField = New Double(2 * fidField.Proc.TransformSize - 1) {} ' allocate space for processing
        ElseIf mode Is Acqu.AcquisitionMode.SEQUENTIAL Then
            ' allocate space for processing
            dataField = New Double(4 * fidField.Proc.TransformSize - 1) {} ' allocate space for processing
        Else
        End If
    End Sub


    Public Overridable Sub setData(index As Integer, point As Double)
        dataField(index) = point
    End Sub

    Friend Overridable Sub shiftData()
        ' perform a left or right shift of the data (ignoring the corresponding portion of the data)
        ' the code from cuteNMR was simplified
        For i As Integer = 0 To fidField.Proc.TdEffective - std.Abs(fidField.Proc.Shift) - 1
            Dim dataIndex = If(fidField.Proc.Shift >= 0, i, i - fidField.Proc.Shift) ' or shift the placement of the data to the right
            ' or shift the placement of the data to the right
            Dim fidIndex = If(fidField.Proc.Shift >= 0, i + fidField.Proc.Shift, i) ' start in the correct order
            ' start in the correct order
            Dim type = fidField.Acqu.FidType

            If type Is Acqu.FidData.INT32 Then
                dataField(dataIndex) = fidField.Fid(fidIndex)
            ElseIf type Is Acqu.FidData.[DOUBLE] Then
                dataField(dataIndex) = fidField.Fid(fidIndex)
            ElseIf type Is Acqu.FidData.FLOAT Then
            ElseIf type Is Acqu.FidData.INT16 Then
            Else
            End If
        Next
    End Sub

    Friend Overridable Sub tdRelatedNonUnderstoodRearrangementForSequential()
        ''' try to understand this bit of code!!!!
        'nonsense case if SI is set to be less than TD/2
        Dim td = If(fidField.Proc.TdEffective > 2 * fidField.Proc.TransformSize, 2 * fidField.Proc.TransformSize, fidField.Proc.TdEffective)
        ' move the data from position i to position 2*i, why?
        If fidField.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.SEQUENTIAL) Then
            For i = td - 1 To 1 Step -1
                dataField(2 * i) = dataField(i)
                dataField(i) = 0
            Next
        End If
        ''''''''''''''''''''''''''''''''''''''''''''
    End Sub

    Public Overridable ReadOnly Property Processing As Proc
        Get
            Return fidField.Proc
        End Get
    End Property

    Public Overridable ReadOnly Property Acquisition As Acqu
        Get
            Return fidField.Acqu
        End Get
    End Property
End Class
