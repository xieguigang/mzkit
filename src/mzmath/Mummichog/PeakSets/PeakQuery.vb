#Region "Microsoft.VisualBasic::600911f61596f6f3b338d3be7a55cfee, E:/mzkit/src/mzmath/Mummichog//PeakSets/PeakQuery.vb"

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

    '   Total Lines: 50
    '    Code Lines: 38
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 1.44 KB


    ' Class PeakQuery
    ' 
    '     Properties: adducts, exactMass, id_group, peaks, rt
    '                 size
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' a collection of the spectrum items that matched with the same exact mass value.
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class PeakQuery(Of T As IMS1Annotation)

    ''' <summary>
    ''' the exact mass value that multiple <see cref="peaks"/> spectrum data matched.
    ''' </summary>
    ''' <returns></returns>
    Public Property exactMass As Double
    ''' <summary>
    ''' a collection of the spectrum object, the precursor m/z value of 
    ''' these spectrum object maybe different, but has the same exact 
    ''' mass value in different precursor type computation result. 
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As T()

    ''' <summary>
    ''' the number of the <see cref="peaks"/> collection.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return peaks.TryCount
        End Get
    End Property

    ''' <summary>
    ''' a collection of the matched precursor type of the precursors
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property adducts As String()
        Get
            Return peaks _
                .Select(Function(p) p.precursor_type) _
                .Distinct _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' a group of unqiue reference id of <see cref="peaks"/>.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property id_group As String()
        Get
            Return peaks _
                .Select(Function(p) p.Key) _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' returns the rt range in [rtmin, rtmax]
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property rt As DoubleRange
        Get
            Return New DoubleRange(From peak As T
                                   In peaks
                                   Let t = peak.rt
                                   Select t)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{exactMass.ToString("F4")}@{rt}s has {peaks.Length} peaks: {adducts.JoinBy("; ")}"
    End Function

End Class
