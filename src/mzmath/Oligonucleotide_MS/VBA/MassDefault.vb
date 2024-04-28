#Region "Microsoft.VisualBasic::208623632655cf857a4cdf86cadb7cb0, E:/mzkit/src/mzmath/Oligonucleotide_MS//VBA/MassDefault.vb"

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

    '   Total Lines: 90
    '    Code Lines: 65
    ' Comment Lines: 13
    '   Blank Lines: 12
    '     File Size: 3.48 KB


    ' Module MassDefault
    ' 
    '     Function: AverageMassBases, AverageMassElements, AverageMassModifications, GetBases, GetElements
    '               GetGroupMass, GetModifications, MonoisotopicBases, MonoisotopicElements, MonoisotopicModifications
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Module MassDefault

    Friend ReadOnly zero As New Element("", 0)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetGroupMass(monoisotopic As Boolean) As IEnumerable(Of GroupMass)
        Return If(monoisotopic, GroupMass.MonoisotopicMass, GroupMass.AverageMass)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetElements(monoisotopic As Boolean) As IEnumerable(Of Element)
        Return If(monoisotopic, MonoisotopicElements(), AverageMassElements())
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetModifications(monoisotopic As Boolean) As IEnumerable(Of Element)
        Return If(monoisotopic, MonoisotopicModifications(), AverageMassModifications())
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="monoisotopic"></param>
    ''' <returns>
    ''' + Adenosine monophosphate
    ''' + Guanosine monophosphate
    ''' + Cytidine monophosphate
    ''' + N1-Me-Pseudo-UMP
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetBases(monoisotopic As Boolean) As IEnumerable(Of Element)
        Return If(monoisotopic, MonoisotopicBases(), AverageMassBases())
    End Function

    Private Iterator Function MonoisotopicBases() As IEnumerable(Of Element)
        Yield New Element("A", 329.0525)
        Yield New Element("G", 345.0474)
        Yield New Element("C", 305.0413)
        Yield New Element("T", 320.041)
    End Function

    Private Iterator Function AverageMassBases() As IEnumerable(Of Element)
        Yield New Element("A", 329.2091)
        Yield New Element("G", 345.2085)
        Yield New Element("C", 305.1841)
        Yield New Element("T", 320.1957)
    End Function

    Private Iterator Function MonoisotopicElements() As IEnumerable(Of Element)
        Yield New Element("C", 12)
        Yield New Element("H", 1.007825)
        Yield New Element("N", 14.003074)
        Yield New Element("O", 15.994915)
        Yield New Element("P", 30.973762)
        Yield New Element("S", 31.972071)
        Yield New Element("Water", 18.010565)
        Yield New Element("Proton", 1.0072765)
    End Function

    Private Iterator Function AverageMassElements() As IEnumerable(Of Element)
        Yield New Element("C", 12.011)
        Yield New Element("H", 1.00794)
        Yield New Element("N", 14.00674)
        Yield New Element("O", 15.9994)
        Yield New Element("P", 30.973762)
        Yield New Element("S", 32.066)
        Yield New Element("Water", 18.01528)
        Yield New Element("Proton", 1)
    End Function

    Private Iterator Function MonoisotopicModifications() As IEnumerable(Of Element)
        ' first element is zero, means no modification
        Yield zero
        Yield New Element("minus p", -79.9663)
        Yield New Element("plus p", 79.9663)
        Yield New Element("cp", -18.0106)
    End Function

    Private Iterator Function AverageMassModifications() As IEnumerable(Of Element)
        ' first element is zero, means no modification
        Yield zero
        Yield New Element("minus p", -79.9799)
        Yield New Element("plus p", 79.9799)
        Yield New Element("cp", -18.0153)
    End Function
End Module
