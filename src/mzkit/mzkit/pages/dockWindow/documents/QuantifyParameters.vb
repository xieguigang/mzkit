Imports System.ComponentModel

Public Class QuantifyParameters

    <Category("Peak Width")>
    <DisplayName("min")>
    <Description("the min peak width in rt(seconds).")>
    Public Property peakMin As Double
    <Category("Peak Width")>
    <DisplayName("max")>
    <Description("the max peak width in rt(seconds).")>
    Public Property peakMax As Double

    <Category("Peak Finding")>
    <Description("The threshold value of sin(alpha) angle value, value of this parameter should be in range of [0,90]")>
    Public Property angle_threshold As Double = 8

End Class
