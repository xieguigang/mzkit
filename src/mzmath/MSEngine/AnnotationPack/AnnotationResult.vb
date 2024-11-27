#Region "Microsoft.VisualBasic::e303bae2cac2017a9e84ee1c4981055b, mzmath\MSEngine\AnnotationPack\AnnotationResult.vb"

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

    '   Total Lines: 70
    '    Code Lines: 49 (70.00%)
    ' Comment Lines: 14 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (10.00%)
    '     File Size: 2.10 KB


    ' Class AlignmentHit
    ' 
    '     Properties: adducts, biodeep_id, exact_mass, formula, libname
    '                 mz, name, npeaks, occurrences, ppm
    '                 pvalue, RI, rt, samplefiles, theoretical_mz
    '                 xcms_id
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' the annotation result of a specific peak
''' </summary>
Public Class AlignmentHit

    Public Property xcms_id As String
    Public Property libname As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property RI As Double
    Public Property theoretical_mz As Double
    Public Property exact_mass As Double
    Public Property adducts As String
    Public Property ppm As Double
    Public Property occurrences As Integer
    Public Property biodeep_id As String
    Public Property name As String
    Public Property formula As String
    Public Property npeaks As Integer
    Public Property pvalue As Double

    ''' <summary>
    ''' sample hits of current library reference
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' null or empty means annotation in ms1 level
    ''' </remarks>
    Public Property samplefiles As Dictionary(Of String, Ms2Score)

    Default Public Property SampleAlignment(sampleName As String) As Ms2Score
        Get
            Return samplefiles.TryGetValue(sampleName)
        End Get
        Set
            samplefiles(sampleName) = Value
        End Set
    End Property

    Sub New()
    End Sub

    ''' <summary>
    ''' make a copy of the alignment hit result data
    ''' </summary>
    ''' <param name="copy"></param>
    Sub New(copy As AlignmentHit)
        xcms_id = copy.xcms_id
        libname = copy.libname
        mz = copy.mz
        rt = copy.rt
        RI = copy.RI
        theoretical_mz = copy.theoretical_mz
        exact_mass = copy.exact_mass
        adducts = copy.adducts
        ppm = copy.ppm
        occurrences = copy.occurrences
        biodeep_id = copy.biodeep_id
        name = copy.name
        formula = copy.formula
        npeaks = copy.npeaks
        pvalue = copy.pvalue
        samplefiles = New Dictionary(Of String, Ms2Score)(copy.samplefiles)
    End Sub

    Public Overrides Function ToString() As String
        Return name & "_" & adducts
    End Function

End Class
