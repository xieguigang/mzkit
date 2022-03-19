#Region "Microsoft.VisualBasic::b1362433f6fc80fbad324fd9b2431866, mzkit\src\mzkit\mzkit\pages\Settings\RawFileViewer.vb"

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

    '   Total Lines: 48
    '    Code Lines: 38
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.94 KB


    ' Class RawFileViewer
    ' 
    '     Sub: ComboBox1_SelectedIndexChanged, LoadSettings, SaveSettings, ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My

Public Class RawFileViewer : Implements ISaveSettings, IPageSettings

    Dim wiffLicense As LicenseFile

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If

        NumericUpDown1.Value = Globals.Settings.viewer.XIC_ppm

        If Globals.Settings.viewer.method = TrimmingMethods.RelativeIntensity Then
            NumericUpDown2.Value = Globals.Settings.viewer.intoCutoff
        Else
            NumericUpDown2.Value = Globals.Settings.viewer.quantile
        End If

        ComboBox1.SelectedIndex = Globals.Settings.viewer.method
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.viewer.XIC_ppm = Val(NumericUpDown1.Value)
        Globals.Settings.viewer.method = ComboBox1.SelectedIndex

        If Globals.Settings.viewer.method = TrimmingMethods.RelativeIntensity Then
            Globals.Settings.viewer.intoCutoff = NumericUpDown2.Value
        Else
            Globals.Settings.viewer.quantile = NumericUpDown2.Value
        End If
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = TrimmingMethods.RelativeIntensity Then
            NumericUpDown2.Value = Globals.Settings.viewer.intoCutoff
        Else
            NumericUpDown2.Value = Globals.Settings.viewer.quantile
        End If
    End Sub
End Class
