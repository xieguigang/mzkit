#Region "Microsoft.VisualBasic::4eff9f04e506667e7a6bbb1e2b675089, E:/mzkit/src/assembly/BrukerDataReader//XMass/Xml/AnalysisMethod.vb"

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

    '   Total Lines: 76
    '    Code Lines: 60
    ' Comment Lines: 3
    '   Blank Lines: 13
    '     File Size: 2.87 KB


    '     Class AnalysisMethod
    ' 
    '         Properties: creator, MethodParameter, name, script, type
    '                     version
    ' 
    '     Class script
    ' 
    '         Properties: language, text
    ' 
    '     Class MethodParameter
    ' 
    '         Properties: DisplayParameter, MassListLayoutParameter, MSRecalibrationParameter, PrpMethod, SmartFormulaResultListLayoutParameter
    ' 
    '     Class MSRecalibrationParameter
    ' 
    '         Properties: CalibrationMode, CalibrationStrategy, PeakAssignmentToleranceSourceManual, PeakAssignmentToleranceSourceStatisticalPeptide, PeakAssignmentToleranceUserDefinedPPM
    '                     PeakAssignmentToleranceUserDefinedStatisticalPPM, SelectedMassControlListNameManual, SelectedMassControlListNameSmart, ZoomRange, ZoomRangeUnitPercent
    ' 
    '     Class LayoutParameters
    ' 
    '         Properties: LayoutParameter
    ' 
    '     Class ColumnLayout
    ' 
    '         Properties: ColumnID, ColumnWidth
    ' 
    '     Class ColumnLayout2
    ' 
    '         Properties: ColumnID, ColumnLongDesc, ColumnShortDesc, ColumnWidth
    ' 
    '     Class LayoutParameter
    ' 
    '         Properties: ColumnLayout, ColumnLayout2
    ' 
    '     Class DisplayParameter
    ' 
    '         Properties: MassPrecision, ReplaceOriginalSpectrum
    ' 
    '     Class PrpMethod
    ' 
    '         Properties: PrpMethodName, PrpMethodXML
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XMass

    ''' <summary>
    ''' Analysis.FAmethod.xml
    ''' </summary>
    <XmlRoot("method")>
    Public Class AnalysisMethod
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property creator As String

        Public Property MethodParameter As MethodParameter
        Public Property script As script
    End Class

    Public Class script
        <XmlAttribute> Public Property language As String
        Public Property text As String
    End Class

    Public Class MethodParameter
        Public Property DisplayParameter As DisplayParameter
        Public Property PrpMethod As PrpMethod
        Public Property MassListLayoutParameter As LayoutParameter
        Public Property SmartFormulaResultListLayoutParameter As LayoutParameter
        Public Property MSRecalibrationParameter As MSRecalibrationParameter
    End Class

    Public Class MSRecalibrationParameter
        Public Property CalibrationStrategy As Integer
        Public Property PeakAssignmentToleranceSourceManual As Integer
        Public Property PeakAssignmentToleranceSourceStatisticalPeptide As Integer
        Public Property SelectedMassControlListNameManual As String
        Public Property SelectedMassControlListNameSmart As String
        Public Property CalibrationMode As String
        Public Property PeakAssignmentToleranceUserDefinedPPM As Single
        Public Property PeakAssignmentToleranceUserDefinedStatisticalPPM As Single
        Public Property ZoomRange As Single
        Public Property ZoomRangeUnitPercent As Boolean
    End Class

    Public Class LayoutParameters
        Public Property LayoutParameter As LayoutParameter
    End Class

    Public Class ColumnLayout
        <XmlAttribute> Public Property ColumnID As Integer
        <XmlAttribute> Public Property ColumnWidth As Single
    End Class

    Public Class ColumnLayout2
        <XmlAttribute> Public Property ColumnID As String
        <XmlAttribute> Public Property ColumnShortDesc As String
        <XmlAttribute> Public Property ColumnLongDesc As String
        <XmlAttribute> Public Property ColumnWidth As Single
    End Class

    Public Class LayoutParameter
        <XmlElement> Public Property ColumnLayout As ColumnLayout()
        <XmlElement> Public Property ColumnLayout2 As ColumnLayout2()
    End Class

    Public Class DisplayParameter
        Public Property MassPrecision As Integer
        Public Property ReplaceOriginalSpectrum As Boolean
    End Class

    Public Class PrpMethod
        Public Property PrpMethodName As String
        Public Property PrpMethodXML As String

    End Class
End Namespace
