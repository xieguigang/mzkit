#Region "Microsoft.VisualBasic::f6cc28bf1ba6821a7d2d2725148ed45c, mzkit\src\assembly\mzPackExtensions\VendorStream\WiffRawStream.vb"

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

'   Total Lines: 115
'    Code Lines: 92
' Comment Lines: 3
'   Blank Lines: 20
'     File Size: 3.96 KB


' Class WiffRawStream
' 
'     Properties: rawFileName
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: defaultScanId, pullAllScans
' 
'     Sub: RemoveAbNoise, walkScan
' 
' 
' /********************************************************************************/

#End Region

#If NET48 Then

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Clearcore2.Data.DataAccess.SampleData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' wiff raw to mzpack convertor
''' </summary>
Public Class WiffRawStream : Inherits VendorStreamLoader(Of ScanInfo)

    ReadOnly raw As WiffScanFileReader
    ReadOnly checkNoise As Boolean = False

    Public Overrides ReadOnly Property rawFileName As String
        Get
            Return raw.wiffPath
        End Get
    End Property

    Public Overrides ReadOnly Property getExperimentType As FileApplicationClass
        Get
            If raw.experimentType = ExperimentType.MRM Then
                Return FileApplicationClass.LCMSMS
            ElseIf raw.experimentType = ExperimentType.SIM Then
                Return FileApplicationClass.GCMS
            Else
                Return FileApplicationClass.LCMS
            End If
        End Get
    End Property

    Dim sampleName As String
    Dim typeCache As FileApplicationClass

    Public Sub New(raw As WiffScanFileReader,
                   Optional scanIdFunc As Func(Of ScanInfo, Integer, String) = Nothing,
                   Optional checkNoise As Boolean = True)

        MyBase.New(scanIdFunc)

        Me.raw = raw
        Me.checkNoise = checkNoise
        Me.typeCache = getExperimentType

        If raw.experimentType = ExperimentType.MRM Then
            Me.checkNoise = False
            VBDebugger.EchoLine("Disable remove noise for MRM experiment data!")
        End If
    End Sub

    Private Shared Sub RemoveAbNoise(ByRef mz As Double(), ByRef into As Double())
        Dim intensity As Double() = into
        Dim clean As ms2() = mz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = intensity(i)
                        }
                    End Function) _
            .AbSciexBaselineHandling _
            .ToArray

        mz = clean.Select(Function(i) i.mz).ToArray
        into = clean.Select(Function(i) i.intensity).ToArray
    End Sub

    Protected Overrides Sub walkScan(scan As ScanInfo)
        Dim msData As PeakList = If(
            typeCache = FileApplicationClass.LCMSMS,
            raw.GetProfileFromScanNum(scan.ScanNumber),
            raw.GetCentroidFromScanNum(scan.ScanNumber)
        )
        Dim mz As Double() = msData.mz
        Dim into As Double() = msData.into
        Dim scanId As String = scanIdFunc(scan, MSscans.Count)

        If checkNoise Then
            Call RemoveAbNoise(mz, into)
        End If

        If mz.Length = 0 Then
            Return
        End If

        If scan.MSLevel = 1 Then
            If Not MS1 Is Nothing Then
                MS1.products = MS2.PopAll
                MS1.meta.Add(mzStreamWriter.SampleMetaName, sampleName)
                MSscans += MS1
            End If

            scanId = $"{scanId},RT={scan.RetentionTime.ToString("F2")}min; total_ions={scan.TotalIonCurrent.ToString("G4")},basepeak_Mz={mz.ElementAtOrDefault(which.Max(into)).ToString("F4")}"
            MS1 = New ScanMS1 With {
                .BPC = scan.BasePeakIntensity,
                .into = into,
                .mz = mz,
                .rt = scan.RetentionTime * 60,
                .scan_id = scanId,
                .TIC = scan.TotalIonCurrent,
                .meta = New Dictionary(Of String, String)
            }

            If typeCache = FileApplicationClass.LCMSMS Then
                ' MRM ion pair information is save in the scan1 metadata
                For i As Integer = 0 To mz.Length - 1
                    MS1.meta.Add("MRM: " & msData.MRM(i).ToString, CInt(i).ToString)
                Next
            End If
        Else
            MS2 += New ScanMS2 With {
                .activationMethod = ActivationMethods.CID,
                .centroided = True,
                .charge = scan.PrecursorCharge,
                .collisionEnergy = scan.CollisionEnergy,
                .intensity = scan.TotalIonCurrent,
                .parentMz = scan.PrecursorMz,
                .scan_id = scanId,
                .rt = scan.RetentionTime * 60,
                .polarity = scan.Polarity,
                .mz = mz,
                .into = into
            }
        End If
    End Sub

    Protected Overrides Function defaultScanId(scaninfo As ScanInfo, i As Integer) As String
        Return scaninfo.ToString
    End Function

    Protected Overrides Iterator Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of ScanInfo)
        Dim i As i32 = Scan0

        For Each name As String In raw.sampleNames
            Call raw.SetCurrentSample(++i)

            Dim n As Integer = raw.GetLastSpectrumNumber

            sampleName = name

            For scanId As Integer = 0 To n
                Yield raw.GetScan(scanId)
            Next
        Next
    End Function
End Class

#End If
