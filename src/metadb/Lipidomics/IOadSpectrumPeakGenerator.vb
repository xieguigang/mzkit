Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports System.Collections.Generic


Public Interface IOadSpectrumPeakGenerator
        Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChain As AcylChain, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal abundance As Double, ByVal oadId As String()) As IEnumerable(Of SpectrumPeak)
        Function GetAlkylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChain As AlkylChain, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal abundance As Double, ByVal oadId As String()) As IEnumerable(Of SpectrumPeak)
        Function GetSphingoDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChain As SphingoChain, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal abundance As Double, ByVal oadId As String()) As IEnumerable(Of SpectrumPeak)
    End Interface

