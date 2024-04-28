#Region "Microsoft.VisualBasic::3ec65b25fbbdd9575705e2e8495316c4, G:/mzkit/src/assembly/ThermoRawFileReader//DataObjects/RawFileInfo.vb"

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

    '   Total Lines: 200
    '    Code Lines: 79
    ' Comment Lines: 94
    '   Blank Lines: 27
    '     File Size: 6.39 KB


    '     Class RawFileInfo
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Clear
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports ThermoFisher.CommonCore.Data.Business

Namespace DataObjects

    ''' <summary>
    ''' Type for File Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Class RawFileInfo
        ''' <summary>
        ''' Date of Acquisition
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public AcquisitionDate As String

        ''' <summary>
        ''' Acquisition Filename
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public AcquisitionFilename As String

        ''' <summary>
        ''' First Comment
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public Comment1 As String

        ''' <summary>
        ''' Second Comment
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public Comment2 As String

        ''' <summary>
        ''' Sample Name
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public SampleName As String

        ''' <summary>
        ''' Sample Comment
        ''' </summary>
        ''' <remarks>Will often be blank</remarks>
        Public SampleComment As String

        ''' <summary>
        ''' Creation date
        ''' </summary>
        Public CreationDate As Date

        ''' <summary>
        ''' Username of the user when the file was created
        ''' </summary>
        Public CreatorID As String

        ''' <summary>
        ''' Dictionary tracking the device data stored in the .raw file
        ''' Keys are Device type, values are the number of devices of this type
        ''' </summary>
        ''' <remarks>
        ''' Typically a .raw file has a single device, of type Device.MS
        ''' Some .raw files also have LC information, stored as Device.Analog or Device.UV
        ''' </remarks>
        Public ReadOnly Devices As Dictionary(Of Device, Integer)

        ''' <summary>
        ''' Instrument Flags
        ''' </summary>
        ''' <remarks>Values should be one of the constants in the InstFlags class (file Enums.cs)</remarks>
        Public InstFlags As String

        ''' <summary>
        ''' Instrument Hardware Version
        ''' </summary>
        Public InstHardwareVersion As String

        ''' <summary>
        ''' Instrument Software Version
        ''' </summary>
        Public InstSoftwareVersion As String

        ''' <summary>
        ''' Instrument Methods
        ''' </summary>
        ''' <remarks>Typically only have one instrument method; the length of this array defines the number of instrument methods</remarks>
        Public ReadOnly InstMethods As List(Of String)

        ''' <summary>
        ''' Instrument Model
        ''' </summary>
        Public InstModel As String

        ''' <summary>
        ''' Instrument Name
        ''' </summary>
        Public InstName As String

        ''' <summary>
        ''' Instrument Description
        ''' </summary>
        ''' <remarks>Typically only defined for instruments converted from other formats</remarks>
        Public InstrumentDescription As String

        ''' <summary>
        ''' Instrument Serial Number
        ''' </summary>
        Public InstSerialNumber As String

        ''' <summary>
        ''' Tune Methods
        ''' </summary>
        ''' <remarks>Typically have one or two tune methods; the length of this array defines the number of tune methods defined</remarks>
        Public ReadOnly TuneMethods As List(Of TuneMethod)

        ''' <summary>
        ''' File format Version Number
        ''' </summary>
        Public VersionNumber As Integer

        ''' <summary>
        ''' Mass Resolution
        ''' </summary>
        Public MassResolution As Double

        ''' <summary>
        ''' First scan number
        ''' </summary>
        Public ScanStart As Integer

        ''' <summary>
        ''' Last scan number
        ''' </summary>
        Public ScanEnd As Integer

        ''' <summary>
        ''' Flag for corrupt files
        ''' </summary>
        ''' <remarks>Auto-set to true if the file is corrupt / has no data</remarks>
        Public CorruptFile As Boolean

        ''' <summary>
        ''' Reset all data in the struct
        ''' </summary>
        Public Sub Clear()
            AcquisitionDate = String.Empty
            AcquisitionFilename = String.Empty
            Comment1 = String.Empty
            Comment2 = String.Empty
            SampleName = String.Empty
            SampleComment = String.Empty
            CreationDate = Date.MinValue
            CreatorID = String.Empty
            Devices.Clear()
            InstFlags = String.Empty
            InstHardwareVersion = String.Empty
            InstSoftwareVersion = String.Empty
            InstMethods.Clear()
            InstModel = String.Empty
            InstName = String.Empty
            InstrumentDescription = String.Empty
            InstSerialNumber = String.Empty
            TuneMethods.Clear()
            VersionNumber = 0
            MassResolution = 0
            ScanStart = 0
            ScanEnd = 0
            CorruptFile = False
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        Public Sub New()
            AcquisitionDate = String.Empty
            AcquisitionFilename = String.Empty
            Comment1 = String.Empty
            Comment2 = String.Empty
            SampleName = String.Empty
            SampleComment = String.Empty
            CreationDate = Date.MinValue
            CreatorID = String.Empty
            Devices = New Dictionary(Of Device, Integer)()
            InstFlags = String.Empty
            InstHardwareVersion = String.Empty
            InstSoftwareVersion = String.Empty
            InstMethods = New List(Of String)()
            InstModel = String.Empty
            InstName = String.Empty
            InstrumentDescription = String.Empty
            InstSerialNumber = String.Empty
            TuneMethods = New List(Of TuneMethod)()
            VersionNumber = 0
            MassResolution = 0
            ScanStart = 0
            ScanEnd = 0
            CorruptFile = False
        End Sub
    End Class

End Namespace
