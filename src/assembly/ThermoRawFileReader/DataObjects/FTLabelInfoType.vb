#Region "Microsoft.VisualBasic::b2c41273fe2a89c5b1634f97c84c6111, assembly\ThermoRawFileReader\DataObjects\FTLabelInfoType.vb"

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

    '   Total Lines: 59
    '    Code Lines: 20
    ' Comment Lines: 31
    '   Blank Lines: 8
    '     File Size: 1.75 KB


    '     Structure FTLabelInfoType
    ' 
    '         Properties: SignalToNoise
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DataObjects

    ''' <summary>
    ''' Type for storing FT Label Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure FTLabelInfoType
        ''' <summary>
        ''' Peak m/z
        ''' </summary>
        ''' <remarks>This is observed m/z; it is not monoisotopic mass</remarks>
        Public Mass As Double

        ''' <summary>
        ''' Peak Intensity
        ''' </summary>
        Public Intensity As Double

        ''' <summary>
        ''' Peak Resolution
        ''' </summary>
        Public Resolution As Single

        ''' <summary>
        ''' Peak Baseline
        ''' </summary>
        Public Baseline As Single

        ''' <summary>
        ''' Peak Noise
        ''' </summary>
        ''' <remarks>For signal/noise ratio, see SignalToNoise</remarks>
        Public Noise As Single

        ''' <summary>
        ''' Peak Charge
        ''' </summary>
        ''' <remarks>Will be 0 if the charge could not be determined</remarks>
        Public Charge As Integer

        ''' <summary>
        ''' Signal to noise ratio
        ''' </summary>
        ''' <returns>Intensity divided by noise, or 0 if Noise is 0</returns>
        Public ReadOnly Property SignalToNoise As Double
            Get
                If Noise > 0 Then Return Intensity / Noise
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Return a summary of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format("m/z {0,9:F3}, S/N {1,7:F1}, intensity {2,12:F0}", Mass, SignalToNoise, Intensity)
        End Function
    End Structure
End Namespace
