#Region "Microsoft.VisualBasic::fe8b871b5ff834526532429cd898cfd0, mzmath\ms2_math-core\Spectra\Models\PeakMs2.vb"

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

    '   Total Lines: 158
    '    Code Lines: 84 (53.16%)
    ' Comment Lines: 53 (33.54%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 21 (13.29%)
    '     File Size: 5.70 KB


    '     Class PeakMs2
    ' 
    '         Properties: activation, collisionEnergy, file, fragments, intensity
    '                     lib_guid, meta, Ms2Intensity, mz, mzInto
    '                     precursor_type, rt, scan
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: AlignMatrix, GetIntensity, GetIons, RtInSecond, ToString
    ' 
    '         Sub: SetIons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports std = System.Math

Namespace Spectra

    ''' <summary>
    ''' 将mzXML文件之中的每一个ms2 scan转换而来
    ''' </summary>
    ''' <remarks>
    ''' A union data model of the ms1 annotation data 
    ''' associated with the ms2 peak scan data.
    ''' </remarks>
    Public Class PeakMs2 : Implements IMs1, IMs1Scan, IMS1Annotation, ISpectrum

        ''' <summary>
        ''' The precursor ``m/z`` value.
        ''' (一级母离子的``m/z``)
        ''' </summary>
        Public Property mz As Double Implements IMs1.mz
        ''' <summary>
        ''' 一级母离子的出峰时间
        ''' </summary>
        Public Property rt As Double Implements IMs1.rt
        ''' <summary>
        ''' 一级母离子的响应强度
        ''' </summary>
        ''' <returns></returns>
        Public Property intensity As Double Implements IMs1Scan.intensity

        ''' <summary>
        ''' 原始数据文件名
        ''' </summary>
        Public Property file As String
        ''' <summary>
        ''' 数据扫描编号
        ''' </summary>
        Public Property scan As String
        Public Property activation As String
        Public Property collisionEnergy As Double

        ''' <summary>
        ''' A unique variable name, meaning could be different with <see cref="LibraryMatrix.Name" />. 
        ''' </summary>
        Public Property lib_guid As String Implements IMS1Annotation.Key

        ''' <summary>
        ''' adducts type of the <see cref="mz"/> value.
        ''' </summary>
        Public Property precursor_type As String Implements IMS1Annotation.precursor_type

        ''' <summary>
        ''' 二级碎片信息
        ''' </summary>
        Public Property mzInto As ms2()

        Public Property meta As Dictionary(Of String, String)

        ''' <summary>
        ''' 获取得到二级碎片的响应强度值的和,这个响应强度值是和其对应的一级母离子的响应强度值是呈正相关的
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public ReadOnly Property Ms2Intensity As Double
            Get
                Return Aggregate mz As ms2
                       In mzInto
                       Into Sum(mz.intensity)
            End Get
        End Property

        ''' <summary>
        ''' the number of the fragments in current spectrum peaks
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public ReadOnly Property fragments As Integer
            Get
                If mzInto Is Nothing Then
                    Return 0
                Else
                    Return mzInto.Length
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(guid As String, spec As IEnumerable(Of ms2))
            lib_guid = guid
            mzInto = spec.SafeQuery.ToArray
        End Sub

        Sub New(guid As String, mz As Double(), into As IVector)
            Call Me.New(guid, into.Data.Select(Function(intensity, i) New ms2(mz(i), intensity)))
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub SetIons(ions As IEnumerable(Of ms2)) Implements ISpectrum.SetIons
            mzInto = ions.ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function GetIons() As IEnumerable(Of ms2) Implements ISpectrum.GetIons
            Return mzInto.AsEnumerable
        End Function

        ''' <summary>
        ''' 将mzXML文件之中的RT文本解析为以秒为单位的rt保留时间数值
        ''' </summary>
        ''' <param name="rt">诸如``PT508.716S``这样格式的表达式字符串</param>
        ''' <returns></returns>
        Public Shared Function RtInSecond(rt As String) As Double
            If rt.StringEmpty Then
                Return 0
            Else
                rt = rt.Substring(2)
                rt = rt.Substring(0, rt.Length - 1)

                Return Double.Parse(rt)
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"M{std.Round(mz)}T{std.Round(rt)} intensity={Ms2Intensity.ToString("G3")} {file}#{scan}"
        End Function

        ''' <summary>
        ''' 当前的这个<see cref="PeakMs2"/>如果在<paramref name="ref"/>找不到对应的``m/z``
        ''' 则对应的部分的into为零
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AlignMatrix(ref As ms2(), tolerance As Tolerance) As ms2()
            Return mzInto.AlignMatrix(ref, tolerance)
        End Function

        Public Function GetIntensity(mz2 As Double, mzdiff As Tolerance) As Double
            Dim mz As ms2 = mzInto _
                .Where(Function(mzi) mzdiff(mzi.mz, mz2)) _
                .OrderByDescending(Function(mzi) mzi.intensity) _
                .FirstOrDefault

            If mz Is Nothing Then
                Return 0
            Else
                Return mz.intensity
            End If
        End Function
    End Class
End Namespace
