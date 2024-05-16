#Region "Microsoft.VisualBasic::1cc82a1fa4b530ff922cf66482e2534c, assembly\BrukerDataReader\Raw\GlobalParameters.vb"

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

    '   Total Lines: 85
    '    Code Lines: 42
    ' Comment Lines: 31
    '   Blank Lines: 12
    '     File Size: 2.98 KB


    '     Class GlobalParameters
    ' 
    '         Properties: AcquiredMZMaximum, AcquiredMZMinimum, MaxMZFilter, MinMZFilter, ML1
    '                     ML2, NumValuesInScan, SampleRate
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Sub: Display, SetDefaults
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Text

Namespace Raw
    Public Class GlobalParameters
        ''' <summary>
        ''' Calibration value A
        ''' </summary>
        ''' <remarks>Previously CalA</remarks>
        Public Property ML1 As Double

        ''' <summary>
        ''' Calibration value B
        ''' </summary>
        ''' <remarks>Previously CalB</remarks>
        Public Property ML2 As Double

        ''' <summary>
        ''' The number of individual values in a scan. For FTMS, typically this is a power of 2.
        ''' e.g. if there are 8 values, this translates to 4 XY data points
        ''' </summary>
        ''' <remarks>TD</remarks>
        Public Property NumValuesInScan As Integer

        ''' <summary>
        ''' Sampling rate; not sure of the units.  A key for figuring out m/z values
        ''' </summary>
        ''' <remarks>SW_h * 2</remarks>
        Public Property SampleRate As Double

        ''' <summary>
        ''' Minimum m/z value for the acquired data
        ''' </summary>
        Public Property AcquiredMZMinimum As Double

        ''' <summary>
        ''' Maximum m/z value for the acquired data
        ''' </summary>
        Public Property AcquiredMZMaximum As Double

        ''' <summary>
        ''' Last minimum m/z value specified when calling GetMassSpectrum()
        ''' </summary>
        Public Property MinMZFilter As Single

        ''' <summary>
        ''' Last maximum m/z value specified when calling GetMassSpectrum()
        ''' </summary>
        Public Property MaxMZFilter As Single

        Public Sub New()
            SetDefaults()
        End Sub

        ' ReSharper disable once UnusedMember.Global
        Public Sub New(ml1 As Double, ml2 As Double, sampleRate As Double, numValuesInScan As Integer)
            Me.New()
            Me.ML1 = ml1
            Me.ML2 = ml2
            Me.SampleRate = sampleRate
            Me.NumValuesInScan = numValuesInScan
        End Sub

        ' ReSharper disable once UnusedMember.Global
        Public Sub Display()
            Dim sb = New StringBuilder()
            sb.AppendFormat("ML1 =    {0:F3}{1}", ML1, Environment.NewLine)
            sb.AppendFormat("ML2 =    {0:F3}{1}", ML2, Environment.NewLine)
            sb.AppendFormat("SW_h =   {0:F3}{1}", SampleRate, Environment.NewLine)
            sb.AppendFormat("TD =     {0}{1}", NumValuesInScan, Environment.NewLine)
            sb.AppendFormat("MZ_min = {0:F3}{1}", AcquiredMZMinimum, Environment.NewLine)
            sb.AppendFormat("MZ_max = {0:F3}{1}", AcquiredMZMaximum, Environment.NewLine)
            Console.WriteLine(sb.ToString())
        End Sub

        Private Sub SetDefaults()
            ML1 = -1
            ML2 = -1
            NumValuesInScan = 0
            SampleRate = -1
            MinMZFilter = 300
            MaxMZFilter = 2000
        End Sub
    End Class
End Namespace
