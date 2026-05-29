#Region "Microsoft.VisualBasic::43d91b8c310c5d7bd1515a6969c5e660, metadb\Chemoinformatics\InChI\Models\InchiFlag.vb"

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

    '   Total Lines: 243
    '    Code Lines: 126 (51.85%)
    ' Comment Lines: 76 (31.28%)
    '    - Xml Docs: 78.95%
    ' 
    '   Blank Lines: 41 (16.87%)
    '     File Size: 9.97 KB


    '     Class InchiFlag
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Enum InnerEnum
    ' 
    '             AuxNone, ChiralFlagOFF, ChiralFlagON, DoNotAddH, FixedH
    '             FoldCRU, KET, LargeMolecules, LooseTSACheck, NEWPSOFF
    '             NoEdits, NoFrameShift, NoWarnings, NPZz, OneFiveT
    '             OutErrInChI, Polymers, Polymers105, RecMet, SAbs
    '             SAtZz, SaveOpt, SLUUD, SNon, SRac
    '             SRel, SUCF, SUU, WarnOnEmptyStructure
    ' 
    ' 
    ' 
    '  
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ordinal, ToString, valueOf, values
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public NotInheritable Class InchiFlag

        ''' <summary>
        ''' Both ends of wedge point to stereocenters [compatible with standard InChI] </summary>
        Public Shared ReadOnly NEWPSOFF As InchiFlag = New InchiFlag("NEWPSOFF", InnerEnum.NEWPSOFF)

        ''' <summary>
        ''' All hydrogens in input structure are explicit [compatible with standard InChI] </summary>
        Public Shared ReadOnly DoNotAddH As InchiFlag = New InchiFlag("DoNotAddH", InnerEnum.DoNotAddH)

        ''' <summary>
        ''' Ignore stereo [compatible with standard InChI] </summary>
        Public Shared ReadOnly SNon As InchiFlag = New InchiFlag("SNon", InnerEnum.SNon)

        ''' <summary>
        ''' Use relative stereo </summary>
        Public Shared ReadOnly SRel As InchiFlag = New InchiFlag("SRel", InnerEnum.SRel)

        ''' <summary>
        ''' Use racemic stereo </summary>
        Public Shared ReadOnly SRac As InchiFlag = New InchiFlag("SRac", InnerEnum.SRac)

        ''' <summary>
        ''' Use Chiral Flag in MOL/SD file record: if On – use Absolute stereo, Off – use Relative stereo </summary>
        Public Shared ReadOnly SUCF As InchiFlag = New InchiFlag("SUCF", InnerEnum.SUCF)

        ''' <summary>
        ''' Set chiral flag ON </summary>
        Public Shared ReadOnly ChiralFlagON As InchiFlag = New InchiFlag("ChiralFlagON", InnerEnum.ChiralFlagON)

        ''' <summary>
        ''' Set chiral flag OFF </summary>
        Public Shared ReadOnly ChiralFlagOFF As InchiFlag = New InchiFlag("ChiralFlagOFF", InnerEnum.ChiralFlagOFF)

        ''' <summary>
        ''' Allows input of molecules up to 32767 atoms [Produces 'InChI=1B' indicating beta status of resulting identifiers] </summary>
        Public Shared ReadOnly LargeMolecules As InchiFlag = New InchiFlag("LargeMolecules", InnerEnum.LargeMolecules)

        ''' <summary>
        ''' Always indicate unknown/undefined stereo </summary>
        Public Shared ReadOnly SUU As InchiFlag = New InchiFlag("SUU", InnerEnum.SUU)

        ''' <summary>
        ''' Stereo labels for "unknown" and "undefined" are different, 'u' and '?', resp. </summary>
        Public Shared ReadOnly SLUUD As InchiFlag = New InchiFlag("SLUUD", InnerEnum.SLUUD)

        ''' <summary>
        ''' Include Fixed H layer </summary>
        Public Shared ReadOnly FixedH As InchiFlag = New InchiFlag("FixedH", InnerEnum.FixedH)

        ''' <summary>
        ''' Include reconnected metals results </summary>
        Public Shared ReadOnly RecMet As InchiFlag = New InchiFlag("RecMet", InnerEnum.RecMet)

        ''' <summary>
        ''' Account for keto-enol tautomerism (experimental) </summary>
        Public Shared ReadOnly KET As InchiFlag = New InchiFlag("KET", InnerEnum.KET)

        ''' <summary>
        ''' Account for 1,5-tautomerism (experimental) </summary>
        Public Shared ReadOnly OneFiveT As InchiFlag = New InchiFlag("OneFiveT", InnerEnum.OneFiveT)

        ''' <summary>
        ''' Omit auxiliary information </summary>
        Public Shared ReadOnly AuxNone As InchiFlag = New InchiFlag("AuxNone", InnerEnum.AuxNone)

        ''' <summary>
        ''' Warn and produce empty InChI for empty structure
        ''' NOTE: This option doesn't currently work due to an InChI library bug
        ''' </summary>
        Public Shared ReadOnly WarnOnEmptyStructure As InchiFlag = New InchiFlag("WarnOnEmptyStructure", InnerEnum.WarnOnEmptyStructure)

        ''' <summary>
        ''' Save custom InChI creation options (non-standard InChI) </summary>
        Public Shared ReadOnly SaveOpt As InchiFlag = New InchiFlag("SaveOpt", InnerEnum.SaveOpt)

        ''' <summary>
        ''' Suppress all warning messages </summary>
        Public Shared ReadOnly NoWarnings As InchiFlag = New InchiFlag("NoWarnings", InnerEnum.NoWarnings)

        ''' <summary>
        ''' Relax criteria of ambiguous drawing for in-ring tetrahedral stereo </summary>
        Public Shared ReadOnly LooseTSACheck As InchiFlag = New InchiFlag("LooseTSACheck", InnerEnum.LooseTSACheck)

        ''' <summary>
        ''' Allow processing of polymers (experimental) </summary>
        Public Shared ReadOnly Polymers As InchiFlag = New InchiFlag("Polymers", InnerEnum.Polymers)

        ''' <summary>
        ''' Allow processing of polymers (experimental, legacy mode of v. 1.05) </summary>
        Public Shared ReadOnly Polymers105 As InchiFlag = New InchiFlag("Polymers105", InnerEnum.Polymers105)

        ''' <summary>
        ''' Remove repeats within constitutional repeating units (CRU/SRU) </summary>
        Public Shared ReadOnly FoldCRU As InchiFlag = New InchiFlag("FoldCRU", InnerEnum.FoldCRU)

        ''' <summary>
        ''' Disable polymer CRU frame shift </summary>
        Public Shared ReadOnly NoFrameShift As InchiFlag = New InchiFlag("NoFrameShift", InnerEnum.NoFrameShift)

        ''' <summary>
        ''' Disable polymer CRU frame shift and folding </summary>
        Public Shared ReadOnly NoEdits As InchiFlag = New InchiFlag("NoEdits", InnerEnum.NoEdits)

        ''' <summary>
        ''' Allow non-polymer-related Zz atoms (pseudo element placeholders) </summary>
        Public Shared ReadOnly NPZz As InchiFlag = New InchiFlag("NPZz", InnerEnum.NPZz)

        ''' <summary>
        ''' Allow stereo at atoms connected to Zz </summary>
        Public Shared ReadOnly SAtZz As InchiFlag = New InchiFlag("SAtZz", InnerEnum.SAtZz)

        ''' <summary>
        ''' Use absolute stereo (this is the default, so this flag is typically redundant) </summary>
        Public Shared ReadOnly SAbs As InchiFlag = New InchiFlag("SAbs", InnerEnum.SAbs)

        ''' <summary>
        ''' Output an empty InChI ("InChI=1//" or "InChI=1S//") on error </summary>
        Public Shared ReadOnly OutErrInChI As InchiFlag = New InchiFlag("OutErrInChI", InnerEnum.OutErrInChI)

        Private Shared ReadOnly valueList As IList(Of InchiFlag) = New List(Of InchiFlag)()

        Shared Sub New()
            valueList.Add(NEWPSOFF)
            valueList.Add(DoNotAddH)
            valueList.Add(SNon)
            valueList.Add(SRel)
            valueList.Add(SRac)
            valueList.Add(SUCF)
            valueList.Add(ChiralFlagON)
            valueList.Add(ChiralFlagOFF)
            valueList.Add(LargeMolecules)
            valueList.Add(SUU)
            valueList.Add(SLUUD)
            valueList.Add(FixedH)
            valueList.Add(RecMet)
            valueList.Add(KET)
            valueList.Add(OneFiveT)
            valueList.Add(AuxNone)
            valueList.Add(WarnOnEmptyStructure)
            valueList.Add(SaveOpt)
            valueList.Add(NoWarnings)
            valueList.Add(LooseTSACheck)
            valueList.Add(Polymers)
            valueList.Add(Polymers105)
            valueList.Add(FoldCRU)
            valueList.Add(NoFrameShift)
            valueList.Add(NoEdits)
            valueList.Add(NPZz)
            valueList.Add(SAtZz)
            valueList.Add(SAbs)
            valueList.Add(OutErrInChI)
        End Sub

        Public Enum InnerEnum
            NEWPSOFF
            DoNotAddH
            SNon
            SRel
            SRac
            SUCF
            ChiralFlagON
            ChiralFlagOFF
            LargeMolecules
            SUU
            SLUUD
            FixedH
            RecMet
            KET
            OneFiveT
            AuxNone
            WarnOnEmptyStructure
            SaveOpt
            NoWarnings
            LooseTSACheck
            Polymers
            Polymers105
            FoldCRU
            NoFrameShift
            NoEdits
            NPZz
            SAtZz
            SAbs
            OutErrInChI
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Private Sub New(name As String, innerEnum As InnerEnum)
            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Public Overrides Function ToString() As String
            If Me Is OneFiveT Then
                'Java doesn't allow enums to start with digits
                Return "15T"
            Else
                Return MyBase.ToString()
            End If
        End Function

        Public Shared Function values() As IList(Of InchiFlag)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Shared Function valueOf(name As String) As InchiFlag
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

End Namespace
