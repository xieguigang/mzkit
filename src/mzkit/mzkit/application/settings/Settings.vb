#Region "Microsoft.VisualBasic::a5af5dbc5055cbfd84acae02c705bdfc, src\mzkit\mzkit\settings\Settings.vb"

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

' Class Settings
' 
'     Properties: configFile, formula_search, network, precursor_search, recentFiles
'                 ui, viewer
' 
'     Function: DefaultProfile, GetConfiguration, Save
' 
' Class RawFileViewerSettings
' 
'     Properties: colorSet, intoCutoff, method, quantile, XIC_ppm
' 
'     Function: GetMethod
' 
' Enum TrimmingMethods
' 
'     Quantile, RelativeIntensity
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Configuration

    Public Class Settings

        Public Property precursor_search As PrecursorSearchSettings
        Public Property formula_search As FormulaSearchProfile
        Public Property ui As UISettings
        Public Property viewer As RawFileViewerSettings
        Public Property network As NetworkArguments

        Public Property recentFiles As String()

        Public Property workspaceFile As String

        Public Shared ReadOnly Property configFile As String = App.LocalData & "/settings.json"

        Public Shared Function DefaultProfile() As Settings
            Return New Settings With {
                .precursor_search = New PrecursorSearchSettings With {
                    .ppm = 5, .precursor_types = {"M", "M+H", "M-H", "M+H-H2O", "M-H2O-H"}
                }
            }
        End Function

        Public Shared Function GetConfiguration() As Settings
            Dim config As Settings = configFile.LoadJsonFile(Of Settings)

            If config Is Nothing Then
                config = DefaultProfile()
            End If

            Return config
        End Function

        Public Function Save() As Boolean
            Return Me.GetJson.SaveTo(configFile)
        End Function
    End Class

    Public Class RawFileViewerSettings

        Public Property XIC_ppm As Double = 20
        Public Property colorSet As String()

        Public Property method As TrimmingMethods = TrimmingMethods.RelativeIntensity
        Public Property intoCutoff As Double = 0.05
        Public Property quantile As Double = 0.65
        Public Property fill As Boolean

        Public Function GetMethod() As LowAbundanceTrimming
            If method = TrimmingMethods.RelativeIntensity Then
                Return New RelativeIntensityCutoff(intoCutoff)
            Else
                Return New QuantileIntensityCutoff(quantile)
            End If
        End Function

    End Class

    Public Enum TrimmingMethods
        RelativeIntensity
        Quantile
    End Enum
End Namespace