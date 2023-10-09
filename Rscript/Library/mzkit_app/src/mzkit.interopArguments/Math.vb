#Region "Microsoft.VisualBasic::5ec4f9d8868af4df78c1e7f9e85c64bb, Rscript\Library\mzkit.interopArguments\Math.vb"

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

' Module Math
' 
'     Function: GetPrecursorTypes, getTolerance
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports PeakMs1 = BioNovoGene.Analytical.MassSpectrometry.Math.Peaktable

Module Math

    Public Function GetIonMode(x As Object, env As Environment) As IonModes
        If x Is Nothing Then
            Return IonModes.Unknown
        End If

        If TypeOf x Is IonModes Then
            Return DirectCast(x, IonModes)
        ElseIf DataFramework.IsNumericType(x.GetType) Then
            Return If(CDbl(x) > 0, IonModes.Positive, IonModes.Negative)
        ElseIf TypeOf x Is String OrElse TypeOf x Is Char Then
            Return Provider.ParseIonMode(CStr(x), allowsUnknown:=True)
        Else
            Return IonModes.Unknown
        End If
    End Function

    ''' <summary>
    ''' this helper function will always returns a <see cref="Tolerance"/> 
    ''' value or message if the type cast failure.
    ''' </summary>
    ''' <param name="val">
    ''' value of this parameter value usually be two kind of default value:
    ''' 
    ''' + da:0.3
    ''' + ppm:20
    ''' </param>
    ''' <param name="env">
    ''' apply for throw exception message
    ''' </param>
    ''' <returns></returns>
    Public Function getTolerance(val As Object, env As Environment, Optional default$ = "ppm:20") As [Variant](Of Tolerance, Message)
        If val Is Nothing Then
            Return Tolerance.DefaultTolerance.DefaultValue
        ElseIf val.GetType.IsInheritsFrom(GetType(Tolerance)) Then
            Return DirectCast(val, Tolerance)
        ElseIf val.GetType Is GetType(String) Then
            Return Tolerance.ParseScript(val)
        ElseIf val.GetType Is GetType(String()) Then
            Return Tolerance.ParseScript(DirectCast(val, String())(Scan0))
        ElseIf val.GetType Is GetType(vector) Then
            Return Tolerance.ParseScript(DirectCast(val, vector).data(Scan0))
        Else
            Dim errMsg As String = $"mzdiff tolerance value can not be '{val.GetType.FullName}', [{val}]!"
            Dim ex As New NotImplementedException(errMsg)

            Return debug.stop(ex, env)
        End If
    End Function

    ''' <summary>
    ''' Get a set of <see cref="MzCalculator"/> based on the parameter value
    ''' </summary>
    ''' <param name="adducts">
    ''' should be a collection of <see cref="MzCalculator"/> or 
    ''' a collection of <see cref="String"/> literal of the 
    ''' adducts
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Function GetPrecursorTypes(adducts As Object, env As Environment) As MzCalculator()
        Dim data As pipeline = pipeline.TryCreatePipeline(Of MzCalculator)(adducts, env, suppress:=True)

        If Not data.isError Then
            Return data.populates(Of MzCalculator)(env).ToArray
        End If

        data = pipeline.TryCreatePipeline(Of String)(adducts, env, suppress:=True)

        If Not data.isError Then
            Return data.populates(Of String)(env) _
                .ToArray _
                .DoCall(AddressOf Provider.Calculators)
        End If

        Throw New NotImplementedException
    End Function

    Public Function GetPeakList(peaktable As Object, env As Environment) As [Variant](Of Message, PeakMs1())
        If TypeOf peaktable Is dataframe Then
            Return PeakListFromDataframe(peaktable)
        Else
            Dim peakList As pipeline = pipeline.TryCreatePipeline(Of PeakMs1)(peaktable, env)

            If peakList.isError Then
                Return peakList.getError
            Else
                Return peakList _
                    .populates(Of PeakMs1)(env) _
                    .ToArray
            End If
        End If
    End Function

    <Extension>
    Public Function PeakListFromDataframe(peaktable As dataframe) As PeakMs1()
        Dim mz As Double() = peaktable.getVector(Of Double)("mz", "m/z")
        Dim rt As Double() = peaktable.getVector(Of Double)("rt", "RT", "retention_time")
        Dim id As String() = peaktable.getVector(Of String)("xcms_id", "id", "ID", "name", "guid")
        Dim scan As Integer() = peaktable.getVector(Of Integer)("scan")
        Dim rtmin As Double() = peaktable.getVector(Of Double)("rtmin")
        Dim rtmax As Double() = peaktable.getVector(Of Double)("rtmax")
        Dim maxinto As Double() = peaktable.getVector(Of Double)("maxInto", "maxinto")
        Dim area As Double() = peaktable.getVector(Of Double)("area", "into")
        Dim mzmin As Double() = peaktable.getVector(Of Double)("mzmin")
        Dim mzmax As Double() = peaktable.getVector(Of Double)("mzmax")

        If scan.IsNullOrEmpty Then
            scan = mz.Sequence.ToArray
        End If

        Return id _
            .Select(Function(uid, i)
                        Return New PeakMs1 With {
                            .mz = mz(i),
                            .rt = rt(i),
                            .name = id(i),
                            .scan = scan(i),
                            .rtmin = rtmin.ElementAtOrDefault(i, .rt),
                            .rtmax = rtmax.ElementAtOrDefault(i, .rt),
                            .into = area.ElementAtOrDefault(i),
                            .maxo = maxinto.ElementAtOrDefault(i),
                            .mzmin = mzmin.ElementAtOrDefault(i, .mz),
                            .mzmax = mzmax.ElementAtOrDefault(i, .mz)
                        }
                    End Function) _
            .ToArray
    End Function
End Module
