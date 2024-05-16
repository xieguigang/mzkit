#Region "Microsoft.VisualBasic::d93fcd0537c1b7fa22852dc64e1af7b1, metadb\Chemoinformatics\Formula\Isotopic\IsotopicPeak.vb"

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

    '   Total Lines: 33
    '    Code Lines: 18
    ' Comment Lines: 5
    '   Blank Lines: 10
    '     File Size: 1.07 KB


    '     Class IsotopicPeak
    ' 
    '         Properties: AbsoluteAbundance, Comment, Mass, MassDifferenceFromMonoisotopicIon, RelativeAbundance
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula.IsotopicPatterns

    ''' <summary>
    ''' 'IsotopicPeak.cs' and 'Isotope.cs' are the storage of isotope calculation result for a fomula.
    ''' The detail of the properties such as M+1, M+2, etc is stored in this class.
    ''' Relative intensity is standardized to 100 as the maximum
    ''' </summary>
    Public Class IsotopicPeak

        Public Sub New()

        End Sub

        Public Sub New(source As IsotopicPeak)
            RelativeAbundance = source.RelativeAbundance
            AbsoluteAbundance = source.AbsoluteAbundance
            Mass = source.Mass
            MassDifferenceFromMonoisotopicIon = source.MassDifferenceFromMonoisotopicIon
            Comment = source.Comment
        End Sub


        Public Property RelativeAbundance As Double

        Public Property AbsoluteAbundance As Double

        Public Property Mass As Double

        Public Property MassDifferenceFromMonoisotopicIon As Double

        Public Property Comment As String = String.Empty
    End Class
End Namespace
