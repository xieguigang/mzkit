#Region "Microsoft.VisualBasic::65fa889bca6b790972c1144a3cd2bbc3, G:/mzkit/src/assembly/ThermoRawFileReader//ThermoReaderOptions.vb"

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

    '   Total Lines: 110
    '    Code Lines: 36
    ' Comment Lines: 58
    '   Blank Lines: 16
    '     File Size: 3.54 KB


    ' Class ThermoReaderOptions
    ' 
    ' 
    '     Delegate Sub
    ' 
    '         Properties: IncludeReferenceAndExceptionData, LoadMSMethodInfo, LoadMSTuneInfo, MaxMz, MaxScan
    '                     MinIntensityThreshold, MinMz, MinRelIntensityThresholdRatio, MinScan, SignalToNoiseThreshold
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Thermo reader options
''' </summary>
''' 
<CLSCompliant(True)>
Public Class ThermoReaderOptions
#Region "Events"

    ''' <summary>
    ''' Delegate method for OptionsUpdatedEvent
    ''' </summary>
    ''' <param name="sender"></param>
    Public Delegate Sub OptionsUpdatedEventHandler(sender As Object)

    ''' <summary>
    ''' This event is raised when one of the options tracked by this class is changed
    ''' </summary>
    Public Event OptionsUpdatedEvent As OptionsUpdatedEventHandler

#End Region

#Region "Member variables"

    Private mIncludeReferenceAndExceptionData As Boolean

#End Region

#Region "Properties"

    ''' <summary>
    ''' When true, include reference and exception peaks when obtaining mass spec data
    ''' using GetScanData, GetScanData2D, or GetScanDataSumScans
    ''' </summary>
    ''' <remarks>Reference and exception peaks are internal mass calibration data within a scan</remarks>
    Public Property IncludeReferenceAndExceptionData As Boolean
        Get
            Return mIncludeReferenceAndExceptionData
        End Get
        Set(value As Boolean)
            If mIncludeReferenceAndExceptionData = value Then Return
            mIncludeReferenceAndExceptionData = value
            RaiseEvent OptionsUpdatedEvent(Me)
        End Set
    End Property

    Const DEFAULT_MAX_MZ = 10000000

    ''' <summary>
    ''' Load MS Method Information when calling OpenRawFile
    ''' </summary>
    ''' <remarks>
    ''' Set this to false when using the ThermoRawFileReader on Linux systems;
    ''' CommonCore.RawFileReader raises an exception due to a null value when accessing
    ''' get_StorageDescriptions from get_InstrumentMethodsCount; stack trace:
    '''   ThermoRawFileReader.XRawFileIO.FillFileInfo
    '''   ThermoFisher.CommonCore.RawFileReader.RawFileAccessBase.get_InstrumentMethodsCount
    '''   ThermoFisher.CommonCore.RawFileReader.StructWrappers.Method.get_StorageDescriptions
    ''' </remarks>
    Public Property LoadMSMethodInfo As Boolean = True

    ''' <summary>
    ''' Load MS Tune Information when calling OpenRawFile
    ''' </summary>
    Public Property LoadMSTuneInfo As Boolean = True

    ''' <summary>
    ''' First scan to output
    ''' </summary>
    ''' <returns></returns>
    Public Property MinScan As Integer = -1
    ''' <summary>
    ''' Lowest m/z to output
    ''' </summary>
    ''' <returns></returns>
    Public Property MinMz As Double = 0
    ''' <summary>
    ''' Highest m/z to output
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxMz As Double = DEFAULT_MAX_MZ
    ''' <summary>
    ''' Relative intensity threshold (value between 0 and 1)
    ''' </summary>
    ''' <returns></returns>
    Public Property MinRelIntensityThresholdRatio As Double = 0
    ''' <summary>
    ''' Minimum S/N ratio
    ''' </summary>
    ''' <returns></returns>
    Public Property SignalToNoiseThreshold As Double = 0
    ''' <summary>
    ''' Minimum intensity threshold (absolute value)
    ''' </summary>
    ''' <returns></returns>
    Public Property MinIntensityThreshold As Double = 0
    ''' <summary>
    ''' Last scan to output
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxScan As Integer = Short.MaxValue

#End Region

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class
