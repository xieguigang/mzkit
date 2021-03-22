#Region "Microsoft.VisualBasic::d12b25d23ae8fa3ba4c1fe907da5fc29, src\metadb\MoNA\SpectraSection.vb"

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

    ' Class SpectraSection
    ' 
    '     Properties: Comment, MetaDB, MetaReader, SpectraInfo
    ' 
    ' Class SpectraInfo
    ' 
    '     Properties: collision_energy, column, flow_gradient, flow_rate, fragmentation_mode
    '                 instrument, instrument_type, ion_mode, ionization, MassPeaks
    '                 MsLevel, mz, precursor_type, resolution, retention_time
    '                 solvent_a, solvent_b
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models

Public Class SpectraSection : Inherits MetaInfo

    Public Property SpectraInfo As SpectraInfo
    Public Property Comment As NameValueCollection

    ''' <summary>
    ''' MoNA里面都主要是讲注释的信息放在<see cref="Comment"/>字段里面的。
    ''' 物质的注释信息主要是放在这个结构体之中，这个属性是对<see cref="Comment"/>
    ''' 属性的解析结果
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MetaDB As MetaData
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Comment.FillData
        End Get
    End Property

    Public ReadOnly Property MetaReader As UnionReader
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New UnionReader(MetaDB, Me)
        End Get
    End Property
End Class

Public Class SpectraInfo

    Public Property MsLevel As String
    Public Property mz As Double
    Public Property precursor_type As String
    Public Property instrument_type As String
    Public Property instrument As String
    Public Property collision_energy As String
    Public Property ion_mode As String
    Public Property ionization As String
    Public Property fragmentation_mode As String
    Public Property resolution As String
    Public Property column As String
    Public Property flow_gradient As String
    Public Property flow_rate As String
    Public Property retention_time As String
    Public Property solvent_a As String
    Public Property solvent_b As String

    Public Property MassPeaks As ms2()
End Class
