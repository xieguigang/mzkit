#Region "Microsoft.VisualBasic::f9e03bb073bc46798b79f5a4ad75c297, src\metadb\Chemoinformatics\Formula\AbbrevStatsType.vb"

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

    '     Class AbbrevStatsType
    ' 
    '         Properties: Charge, Comment, Formula, InvalidSymbolOrFormula, IsAminoAcid
    '                     Mass, OneLetterSymbol, Symbol
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

Namespace Formula

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
End Namespace
