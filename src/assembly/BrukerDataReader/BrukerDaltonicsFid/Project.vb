Imports System.Xml.Serialization

Public Class Project

    Public Property sptype As String
    Public Property pdata As pdata()
    Public Property AnalysisParameter As AnalysisParameter

    Public Shared Function FromResultFolder(dir As String) As Project

    End Function
End Class

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

''' <summary>
''' AnalysisParameter.xml
''' </summary>
Public Class AnalysisParameter
    <XmlAttribute> Public Property cid As String
    Public Property BuildingBlockName As String
    Public Property FontFaceName As String
    Public Property FontPointSize As Double
    Public Property FontOrientation As Double
    Public Property SearchTolerance As Double
    Public Property SearchToleranceUnit As String
    Public Property StringType As String
    Public Property PreviewFlag As Boolean
    Public Property ShowMassDifferenceFlag As Boolean
End Class

Public Class pdata

    Public Property peaklist As pklist

    Public Shared Function LoadFolder(dir As String) As pdata

    End Function

End Class

Public Class pklist
    <XmlAttribute> Public Property version As String
    <XmlAttribute> Public Property creator As String
    <XmlAttribute> Public Property shots As Integer
    <XmlAttribute> Public Property [date] As String
    <XmlAttribute> Public Property spectrumid As String
    <XmlElement> Public Property pk As pk()
End Class

''' <summary>
''' A peak
''' </summary>
Public Class pk

    Public Property absi As Double
    Public Property area As Double
    Public Property chisq As Double
    Public Property goodn As Double
    Public Property goodn2 As Double
    Public Property ind As Double
    Public Property lind As Double
    Public Property lmass As Double
    Public Property mass As Double
    Public Property massemg As Double
    Public Property massgaussian As Double
    Public Property meth As Double
    Public Property reso As Double
    Public Property rind As Double
    Public Property rmass As Double
    Public Property s2n As Double
    Public Property sigmaemg As Double
    Public Property tauemg As Double
    Public Property type As Double

End Class