Imports System
Imports System.Text

Namespace BrukerDataReader
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
