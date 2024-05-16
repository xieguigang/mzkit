#Region "Microsoft.VisualBasic::019d0eafcf49a1644e7688dda7707bc9, metadb\Lipidomics\LipidParser\LipidParsers.vb"

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

    '   Total Lines: 907
    '    Code Lines: 734
    ' Comment Lines: 4
    '   Blank Lines: 169
    '     File Size: 42.76 KB


    ' Class PCLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PELipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PALipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PGLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PILipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PSLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPCLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPELipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPGLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPILipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPSLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class TGLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class MGLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class DGLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class CARLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class DGTALipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class DGTSLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LDGTALipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LDGTSLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class EtherLPCLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class EtherPCLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class EtherLPELipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class EtherPELipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class BMPLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class GM3LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class SHexCerLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class SMLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PCd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PEd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PGd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PId5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class PSd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPCd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPEd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPGd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPId5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class LPSd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class TGd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class DGd5LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class SMd9LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class CELipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' Class CEd7LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass

' <auto-generated>
' THIS (.cs) FILE IS GENERATED. DO NOT CHANGE IT.
' CHANGE THE .tt FILE INSTEAD.
' </auto-generated>

Public Class PCLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PC" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PC\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * 18, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxPC, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.PC, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PELipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PE" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PE\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * 12, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxPE, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.PE, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PALipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PA" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PA\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * 7, OxygenMass * 6, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PA, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PGLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PG" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PG\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxPG, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.PG, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PILipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PI" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PI\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 9, HydrogenMass * 17, OxygenMass * 11, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxPI, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.PI, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PSLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PS" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PS\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 12, OxygenMass * 8, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxPS, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.PS, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPCLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPC" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPC\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * 18, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPC, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPELipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPE" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPE\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * 12, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPE, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPGLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPG" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPG\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPG, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPILipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPI" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPI\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 9, HydrogenMass * 17, OxygenMass * 11, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPI, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPSLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPS" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPS\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 12, OxygenMass * 8, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPS, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class TGLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "TG" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(3)
    Public Shared ReadOnly Pattern As String = $"^TG\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * 5, OxygenMass * 3}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            If chains.OxidizedCount > 0 Then
                Return New Lipid(LbmClass.OxTG, Skelton + chains.Mass, chains)
            End If
            Return New Lipid(LbmClass.TG, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class MGLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "MG" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^MG\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 3}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.MG, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class DGLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "DG" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^DG\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 3}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.DG, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class CARLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "CAR" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(1)
    Public Shared ReadOnly Pattern As String = $"^CAR\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 7, HydrogenMass * 14, OxygenMass * 3, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.CAR, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class DGTALipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "DGTA" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^DGTA\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 10, HydrogenMass * 19, OxygenMass * 5, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.DGTA, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class DGTSLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "DGTS" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^DGTS\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 10, HydrogenMass * 19, OxygenMass * 5, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.DGTS, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LDGTALipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LDGTA" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LDGTA\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 10, HydrogenMass * 19, OxygenMass * 5, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LDGTA, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LDGTSLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LDGTS" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LDGTS\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 10, HydrogenMass * 19, OxygenMass * 5, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LDGTS, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class EtherLPCLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPC" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildLysoEtherParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPC\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * 18, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.EtherLPC, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class EtherPCLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PC" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildEtherParser(2)
    Public Shared ReadOnly Pattern As String = $"^PC\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * 18, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.EtherPC, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class EtherLPELipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPE" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildLysoEtherParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPE\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * 12, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.EtherLPE, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class EtherPELipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PE" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildEtherParser(2)
    Public Shared ReadOnly Pattern As String = $"^PE\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * 12, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.EtherPE, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class BMPLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "BMP" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^BMP\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.BMP, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class GM3LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "GM3" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^GM3\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 23, HydrogenMass * 37, OxygenMass * 18, NitrogenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.GM3, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class SHexCerLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "SHexCer" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^SHexCer\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 8, SulfurMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.SHexCer, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class SMLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "SM" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^SM\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * 12, OxygenMass * 3, NitrogenMass * 1, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.SM, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PCd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PC_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PC_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * (18 - 5), Hydrogen2Mass * 5, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PC_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PEd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PE_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PE_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * (12 - 5), Hydrogen2Mass * 5, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PE_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PGd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PG_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PG_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * (13 - 5), Hydrogen2Mass * 5, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PG_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PId5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PI_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PI_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 9, HydrogenMass * (17 - 5), Hydrogen2Mass * 5, OxygenMass * 11, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PI_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class PSd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "PS_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^PS_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * (12 - 5), Hydrogen2Mass * 5, OxygenMass * 8, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.PS_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPCd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPC_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPC_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 8, HydrogenMass * (18 - 5), Hydrogen2Mass * 5, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPC_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPEd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPE_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPE_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * (12 - 5), Hydrogen2Mass * 5, OxygenMass * 6, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPE_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPGd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPG_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPG_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * (13 - 5), Hydrogen2Mass * 5, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPG_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPId5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPI_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPI_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 9, HydrogenMass * (17 - 5), Hydrogen2Mass * 5, OxygenMass * 11, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPI_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class LPSd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "LPS_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 2)
    Public Shared ReadOnly Pattern As String = $"^LPS_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * (12 - 5), Hydrogen2Mass * 5, OxygenMass * 8, NitrogenMass, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.LPS_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class TGd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "TG_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(3)
    Public Shared ReadOnly Pattern As String = $"^TG_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * (5 - 5), Hydrogen2Mass * 5, OxygenMass * 3}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.TG_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class DGd5LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "DG_d5" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly Pattern As String = $"^DG_d5\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 3, HydrogenMass * (6 - 5), Hydrogen2Mass * 5, OxygenMass * 3}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.DG_d5, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class SMd9LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "SM_d9" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^SM_d9\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 5, HydrogenMass * (12 - 9), Hydrogen2Mass * 9, OxygenMass * 3, NitrogenMass * 1, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.SM_d9, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class CELipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "CE" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 1)
    Public Shared ReadOnly Pattern As String = $"^CE\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 27, HydrogenMass * 45, OxygenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.CE, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class

Public Class CEd7LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "CE_d7" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildSpeciesLevelParser(1, 1)
    Public Shared ReadOnly Pattern As String = $"^CE_d7\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 27, HydrogenMass * (45 - 7), Hydrogen2Mass * 7, OxygenMass * 1}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.CE_d7, Skelton + chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class
