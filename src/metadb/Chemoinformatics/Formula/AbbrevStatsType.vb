' Molecular Weight Calculator routines with ActiveX Class interfaces: MWElementAndMassRoutines

' -------------------------------------------------------------------------------
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2003
' E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
' Website: http://panomics.pnnl.gov/ or http://www.sysbio.org/resources/staff/
' -------------------------------------------------------------------------------
' 
' Licensed under the Apache License, Version 2.0; you may not use this file except
' in compliance with the License.  You may obtain a copy of the License at 
' http://www.apache.org/licenses/LICENSE-2.0
'
' Notice: This computer software was prepared by Battelle Memorial Institute, 
' hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
' Department of Energy (DOE).  All rights in the computer software are reserved 
' by DOE on behalf of the United States Government and the Contractor as 
' provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
' WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
' SOFTWARE.  This notice including this sentence must appear on any copies of 
' this computer software.

Public Class AbbrevStatsType

    ''' <summary>
    ''' The symbol for the abbreviation, e.g. Ph for 
    ''' the phenyl group or Ala for alanine (3 letter 
    ''' codes for amino acids)
    ''' </summary>
    ''' <returns></returns>
    Public Property Symbol As String
    ''' <summary>
    ''' Cannot contain other abbreviations
    ''' </summary>
    ''' <returns></returns>
    Public Property Formula As String
    ''' <summary>
    ''' Computed mass for quick reference
    ''' </summary>
    ''' <returns></returns>
    Public Property Mass As Double
    Public Property Charge As Single
    ''' <summary>
    ''' True if an amino acid
    ''' </summary>
    ''' <returns></returns>
    Public Property IsAminoAcid As Boolean
    ''' <summary>
    ''' Only used for amino acids
    ''' </summary>
    ''' <returns></returns>
    Public Property OneLetterSymbol As String
    ''' <summary>
    ''' Description of the abbreviation
    ''' </summary>
    ''' <returns></returns>
    Public Property Comment As String
    Public Property InvalidSymbolOrFormula As Boolean

    Public Overrides Function ToString() As String
        Return Symbol & ": " & Formula
    End Function
End Class