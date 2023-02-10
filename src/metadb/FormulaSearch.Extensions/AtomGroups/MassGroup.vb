#Region "Microsoft.VisualBasic::14629ada6ed3ffb1b5da5adf9ce7794b, mzkit\src\metadb\FormulaSearch.Extensions\AtomGroups\MassGroup.vb"

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

    '   Total Lines: 31
    '    Code Lines: 21
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.17 KB


    '     Class MassGroup
    ' 
    '         Properties: exactMass, name
    ' 
    '         Function: CreateAdducts, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace AtomGroups

    ''' <summary>
    ''' A wrapper for an atom group, molecule fragment
    ''' </summary>
    Public Class MassGroup : Implements IExactMassProvider, ICompoundNameProvider

        Public Property exactMass As Double Implements IExactMassProvider.ExactMass
        Public Property name As String Implements ICompoundNameProvider.CommonName

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Shared Function CreateAdducts(anno As FragmentAnnotationHolder, adducts As MzCalculator) As MassGroup
            Dim name As String = $"[{anno.name}{adducts.name}]{adducts.charge}{adducts.mode}"
            Dim mass As Double = adducts.CalcMZ(anno.exactMass)

            Return New MassGroup With {
                .exactMass = mass,
                .name = name
            }
        End Function

    End Class
End Namespace
