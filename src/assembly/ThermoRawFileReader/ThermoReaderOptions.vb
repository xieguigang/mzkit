
''' <summary>
''' Thermo reader options
''' </summary>
Public Class ThermoReaderOptions
#Region "Events"

    ''' <summary>
    ''' Delegate method for OptionsUpdatedEvent
    ''' </summary>
    ''' <param name="sender"></param>
    Public Delegate Sub OptionsUpdatedEventHandler(ByVal sender As Object)

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
        Set(ByVal value As Boolean)
            If mIncludeReferenceAndExceptionData = value Then Return
            mIncludeReferenceAndExceptionData = value
            RaiseEvent OptionsUpdatedEvent(Me)
        End Set
    End Property

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

#End Region

End Class
