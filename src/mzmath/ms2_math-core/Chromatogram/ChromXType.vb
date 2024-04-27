#Region "Microsoft.VisualBasic::606e862455f2ec47fa03ee08be3e0ecf, G:/mzkit/src/mzmath/ms2_math-core//Chromatogram/ChromXType.vb"

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

    '   Total Lines: 261
    '    Code Lines: 201
    ' Comment Lines: 24
    '   Blank Lines: 36
    '     File Size: 9.33 KB


    '     Enum ChromXType
    ' 
    '         Drift, Mz, RI, RT
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class RetentionTime
    ' 
    '         Properties: [Default], Type, Unit, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetUnitString, ToString
    ' 
    '     Class RetentionIndex
    ' 
    '         Properties: [Default], Type, Unit, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetUnitString, ToString
    ' 
    '     Class DriftTime
    ' 
    '         Properties: [Default], Type, Unit, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetUnitString, ToString
    ' 
    '     Class MzValue
    ' 
    '         Properties: [Default], Type, Unit, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetUnitString, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Chromatogram

    Public Enum ChromXType
        RT
        RI
        Drift
        Mz
    End Enum

    Public NotInheritable Class RetentionTime
        Implements IChromX

        Public ReadOnly Property Value As Double Implements IChromX.Value

        Public ReadOnly Property Type As ChromXType Implements IChromX.Type

        Public ReadOnly Property Unit As ChromXUnit Implements IChromX.Unit

        Public Sub New(retentionTime As Double, Optional unit As ChromXUnit = ChromXUnit.Min)
            Value = retentionTime
            Type = ChromXType.RT
            Me.Unit = unit
        End Sub

        ''' <summary>
        ''' Only MessagePack for C# use this constructor. Use ctor(double, ChromXUnit) instead.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="_type"></param>
        ''' <param name="unit"></param>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Sub New(value As Double, _type As ChromXType, unit As ChromXUnit)
            Me.Value = value
            Type = ChromXType.RT
            Me.Unit = unit
        End Sub

        Public Overrides Function ToString() As String
            Select Case Type
                Case ChromXType.RT
                    Return $"RT: {Value:F3} {GetUnitString()}"
                Case ChromXType.RI
                    Return $"RI: {Value:F3} {GetUnitString()}"
                Case ChromXType.Drift
                    Return $"Drift: {Value:F3} {GetUnitString()}"
                Case ChromXType.Mz
                    Return $"Mz: {Value:F3} {GetUnitString()}"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GetUnitString() As String
            Select Case Unit
                Case ChromXUnit.Min
                    Return "min"
                Case ChromXUnit.Sec
                    Return "sec"
                Case ChromXUnit.None
                    Return ""
                Case ChromXUnit.Mz
                    Return "m/z"
                Case ChromXUnit.Msec
                    Return "msec"
                Case Else
                    Return ""
            End Select
        End Function

        Friend Shared ReadOnly Property [Default] As RetentionTime = New RetentionTime(-1, ChromXUnit.Min)
    End Class

    Public NotInheritable Class RetentionIndex
        Implements IChromX

        Public ReadOnly Property Value As Double Implements IChromX.Value

        Public ReadOnly Property Type As ChromXType Implements IChromX.Type

        Public ReadOnly Property Unit As ChromXUnit Implements IChromX.Unit

        Public Sub New(retentionIndex As Double, Optional unit As ChromXUnit = ChromXUnit.None)
            Value = retentionIndex
            Type = ChromXType.RI
            Me.Unit = unit
        End Sub

        ''' <summary>
        ''' Only MessagePack for C# use this constructor. Use ctor(double, ChromXUnit) instead.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="_type"></param>
        ''' <param name="unit"></param>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Sub New(value As Double, _type As ChromXType, unit As ChromXUnit)
            Me.Value = value
            Type = ChromXType.RI
            Me.Unit = unit
        End Sub

        Public Overrides Function ToString() As String
            Select Case Type
                Case ChromXType.RT
                    Return $"RT: {Value:F3} {GetUnitString()}"
                Case ChromXType.RI
                    Return $"RI: {Value:F3} {GetUnitString()}"
                Case ChromXType.Drift
                    Return $"Drift: {Value:F3} {GetUnitString()}"
                Case ChromXType.Mz
                    Return $"Mz: {Value:F3} {GetUnitString()}"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GetUnitString() As String
            Select Case Unit
                Case ChromXUnit.Min
                    Return "min"
                Case ChromXUnit.Sec
                    Return "sec"
                Case ChromXUnit.None
                    Return ""
                Case ChromXUnit.Mz
                    Return "m/z"
                Case ChromXUnit.Msec
                    Return "msec"
                Case Else
                    Return ""
            End Select
        End Function

        Friend Shared ReadOnly Property [Default] As RetentionIndex = New RetentionIndex(-1, ChromXUnit.None)
    End Class

    Public NotInheritable Class DriftTime
        Implements IChromX

        Public ReadOnly Property Value As Double Implements IChromX.Value

        Public ReadOnly Property Type As ChromXType Implements IChromX.Type

        Public ReadOnly Property Unit As ChromXUnit Implements IChromX.Unit

        Public Sub New(driftTime As Double, Optional unit As ChromXUnit = ChromXUnit.Msec)
            Value = driftTime
            Type = ChromXType.Drift
            Me.Unit = unit
        End Sub

        ''' <summary>
        ''' Only MessagePack for C# use this constructor. Use ctor(double, ChromXUnit) instead.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="_type"></param>
        ''' <param name="unit"></param>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Sub New(value As Double, _type As ChromXType, unit As ChromXUnit)
            Me.Value = value
            Type = ChromXType.Drift
            Me.Unit = unit
        End Sub

        Public Overrides Function ToString() As String
            Select Case Type
                Case ChromXType.RT
                    Return $"RT: {Value:F3} {GetUnitString()}"
                Case ChromXType.RI
                    Return $"RI: {Value:F3} {GetUnitString()}"
                Case ChromXType.Drift
                    Return $"Drift: {Value:F3} {GetUnitString()}"
                Case ChromXType.Mz
                    Return $"Mz: {Value:F3} {GetUnitString()}"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GetUnitString() As String
            Select Case Unit
                Case ChromXUnit.Min
                    Return "min"
                Case ChromXUnit.Sec
                    Return "sec"
                Case ChromXUnit.None
                    Return ""
                Case ChromXUnit.Mz
                    Return "m/z"
                Case ChromXUnit.Msec
                    Return "msec"
                Case Else
                    Return ""
            End Select
        End Function

        Friend Shared ReadOnly Property [Default] As DriftTime = New DriftTime(-1, ChromXUnit.Msec)
    End Class


    Public NotInheritable Class MzValue
        Implements IChromX

        Public ReadOnly Property Value As Double Implements IChromX.Value

        Public ReadOnly Property Type As ChromXType Implements IChromX.Type

        Public ReadOnly Property Unit As ChromXUnit Implements IChromX.Unit

        Public Sub New(mz As Double, Optional unit As ChromXUnit = ChromXUnit.Mz)
            Value = mz
            Type = ChromXType.Mz
            Me.Unit = unit
        End Sub

        ''' <summary>
        ''' Only MessagePack for C# use this constructor. Use ctor(double, ChromXUnit) instead.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="_type"></param>
        ''' <param name="unit"></param>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Sub New(value As Double, _type As ChromXType, unit As ChromXUnit)
            Me.Value = value
            Type = ChromXType.Mz
            Me.Unit = unit
        End Sub

        Public Overrides Function ToString() As String
            Select Case Type
                Case ChromXType.RT
                    Return $"RT: {Value:F3} {GetUnitString()}"
                Case ChromXType.RI
                    Return $"RI: {Value:F3} {GetUnitString()}"
                Case ChromXType.Drift
                    Return $"Drift: {Value:F3} {GetUnitString()}"
                Case ChromXType.Mz
                    Return $"Mz: {Value:F3} {GetUnitString()}"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GetUnitString() As String
            Select Case Unit
                Case ChromXUnit.Min
                    Return "min"
                Case ChromXUnit.Sec
                    Return "sec"
                Case ChromXUnit.None
                    Return ""
                Case ChromXUnit.Mz
                    Return "m/z"
                Case ChromXUnit.Msec
                    Return "msec"
                Case Else
                    Return ""
            End Select
        End Function

        Friend Shared ReadOnly Property [Default] As MzValue = New MzValue(-1, ChromXUnit.Mz)
    End Class

End Namespace
