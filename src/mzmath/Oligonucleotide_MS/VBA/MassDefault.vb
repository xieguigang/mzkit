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
