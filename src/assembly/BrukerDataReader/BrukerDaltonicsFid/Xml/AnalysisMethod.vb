
Imports System.Xml.Serialization

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
