#Region "Microsoft.VisualBasic::7c327cdaa390e3c7b1c771a388566034, mzkit\Rscript\Library\mzkit\math\Formula.vb"

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

'   Total Lines: 575
'    Code Lines: 390
' Comment Lines: 102
'   Blank Lines: 83
'     File Size: 22.15 KB


' Module FormulaTools
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: (+5 Overloads) add, asFormula, atomGroups, canonicalFormula, CreateGraph
'               divide, EvalFormula, FormulaCompositionString, FormulaFinder, FormulaString
'               getElementCount, getFormulaResult, IsotopeDistributionSearch, LoadChemicalDescriptorsMatrix, (+6 Overloads) minus
'               openChemicalDescriptorDatabase, parseSMILES, PeakAnnotation, printFormulas, readKCF
'               readSDF, registerAnnotations, removeElement, (+2 Overloads) repeats, ScanFormula
'               SDF2KCF
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports stdNum = System.Math

''' <summary>
''' The chemical formulae toolkit
''' 
''' In chemistry, a chemical formula is a way of presenting information about the chemical proportions of 
''' atoms that constitute a particular chemical compound or molecule, using chemical element symbols, numbers,
''' and sometimes also other symbols, such as parentheses, dashes, brackets, commas and plus (+) and minus (−) 
''' signs. These are limited to a single typographic line of symbols, which may include subscripts and 
''' superscripts. A chemical formula is not a chemical name since it does not contain any words. Although a 
''' chemical formula may imply certain simple chemical structures, it is not the same as a full chemical 
''' structural formula. Chemical formulae can fully specify the structure of only the simplest of molecules 
''' and chemical substances, and are generally more limited in power than chemical names and structural formulae.
''' </summary>
''' <remarks>
''' The simplest types of chemical formulae are called empirical formulae, which use letters and numbers indicating
''' the numerical proportions of atoms of each type. Molecular formulae indicate the simple numbers of each type 
''' of atom in a molecule, with no information on structure. For example, the empirical formula for glucose is
''' CH2O (twice as many hydrogen atoms as carbon and oxygen), while its molecular formula is C6H12O6 (12 hydrogen 
''' atoms, six carbon and oxygen atoms).
''' 
''' Sometimes a chemical formula is complicated by being written as a condensed formula (or condensed molecular 
''' formula, occasionally called a "semi-structural formula"), which conveys additional information about the
''' particular ways in which the atoms are chemically bonded together, either in covalent bonds, ionic bonds, or 
''' various combinations of these types. This is possible if the relevant bonding is easy to show in one dimension. 
''' An example is the condensed molecular/chemical formula for ethanol, which is CH3−CH2−OH or CH3CH2OH. However, 
''' even a condensed chemical formula is necessarily limited in its ability to show complex bonding relationships 
''' between atoms, especially atoms that have bonds to four or more different substituents.
''' 
''' Since a chemical formula must be expressed as a single line of chemical element symbols, it often cannot be as
''' informative as a true structural formula, which is a graphical representation of the spatial relationship 
''' between atoms in chemical compounds (see for example the figure for butane structural and chemical formulae,
''' at right). For reasons of structural complexity, a single condensed chemical formula (or semi-structural formula)
''' may correspond to different molecules, known as isomers. For example, glucose shares its molecular formula C6H12O6
''' with a number of other sugars, including fructose, galactose and mannose. Linear equivalent chemical names exist 
''' that can and do specify uniquely any complex structural formula (see chemical nomenclature), but such names must
''' use many terms (words), rather than the simple element symbols, numbers, and simple typographical symbols that 
''' define a chemical formula.
''' 
''' Chemical formulae may be used in chemical equations to describe chemical reactions and other chemical transformations, 
''' such as the dissolving of ionic compounds into solution. While, as noted, chemical formulae do not have the full 
''' power of structural formulae to show chemical relationships between atoms, they are sufficient to keep track of 
''' numbers of atoms and numbers of electrical charges in chemical reactions, thus balancing chemical equations so that
''' these equations can be used in chemical problems involving conservation of atoms, and conservation of electric 
''' charge.
''' </remarks>
<Package("formula", Category:=APICategories.UtilityTools)>
Module FormulaTools

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
        Call REnv.AttachConsoleFormatter(Of Formula)(AddressOf FormulaString)
        Call REnv.AttachConsoleFormatter(Of FormulaComposition())(AddressOf printFormulas)

        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(FormulaComposition()), AddressOf getFormulaResult)
    End Sub

    Private Function getFormulaResult(formulas As FormulaComposition(), args As list, env As Environment) As RDataframe
        Dim candidates As New RDataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call candidates.add("formula", formulas.Select(Function(f) f.EmpiricalFormula))
        Call candidates.add("exact_mass", formulas.Select(Function(f) f.ExactMass))
        Call candidates.add("mass_diff", formulas.Select(Function(f) f.massdiff))
        Call candidates.add("ppm", formulas.Select(Function(f) f.ppm))
        Call candidates.add("H/C", formulas.Select(Function(f) f.HCRatio))

        Return candidates
    End Function

    Private Function printFormulas(formulas As FormulaComposition()) As String
        Dim table As New List(Of String())

        table += {"formula", "mass", "da", "ppm", "charge", "m/z"}

        For Each formula As FormulaComposition In formulas
            table += New String() {
                formula.EmpiricalFormula,
                formula.ExactMass,
                formula.ppm,
                formula.ppm,
                formula.charge,
                formula.ExactMass / formula.charge
            }
        Next

        Return table.Print(addBorder:=False)
    End Function

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
    End Function

    Private Function FormulaString(formula As Formula) As String
        Return formula.ExactMass.ToString("F7") & $" ({formula.CountsByElement.Select(Function(e) $"{e.Key}:{e.Value}").JoinBy(", ")})"
    End Function

    <ExportAPI("registerAnnotations")>
    Public Function registerAnnotations(annotation As RDataframe,
                                        Optional debug As Boolean = True,
                                        Optional env As Environment = Nothing) As Object

        Dim items = annotation.forEachRow({"annotation", "formula"}).ToArray

        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' do peak annotation for the ms2 fragments
    ''' </summary>
    ''' <param name="library">
    ''' A ms2 matrix object
    ''' </param>
    ''' <param name="massDiff"></param>
    ''' <param name="isotopeFirst"></param>
    ''' <param name="adducts"></param>
    ''' <returns></returns>
    <ExportAPI("peakAnnotations")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function peakAnnotation_f(library As Object, formula As Object, <RRawVectorArgument> adducts As Object,
                                     Optional massDiff As Double = 0.1,
                                     Optional isotopeFirst As Boolean = True,
                                     Optional as_list As Boolean = True,
                                     Optional env As Environment = Nothing) As Object
        Dim parentMz As Double
        Dim centroid As Boolean
        Dim name As String
        Dim peaksData As ms2()
        Dim adductList = Math.GetPrecursorTypes(adducts, env)

        If library Is Nothing Then
            Return Nothing
        ElseIf TypeOf library Is LibraryMatrix Then
            Dim mat As LibraryMatrix = DirectCast(library, LibraryMatrix)

            parentMz = mat.parentMz
            centroid = mat.centroid
            name = mat.name
            peaksData = mat.ms2
        ElseIf TypeOf library Is PeakMs2 Then
            Dim peak As PeakMs2 = DirectCast(library, PeakMs2)

            parentMz = peak.mz
            centroid = True
            name = peak.lib_guid
            peaksData = peak.mzInto
        Else
            Return Message.InCompatibleType(GetType(LibraryMatrix), library.GetType, env)
        End If

        Dim results As New list With {.slots = New Dictionary(Of String, Object)}
        Dim f As Formula
        Dim spec As New LibraryMatrix(peaksData)
        Dim anno As PeakAnnotation

        If TypeOf formula Is String Then
            f = FormulaScanner.ScanFormula(DirectCast(formula, String))
        ElseIf TypeOf formula Is Formula Then
            f = formula
        Else
            Return Message.InCompatibleType(GetType(Formula), formula.GetType, env)
        End If

        For Each adduct As MzCalculator In adductList
            anno = PeakAnnotation.DoPeakAnnotation(spec, parentMz, adduct, f)

            If as_list Then
                results.slots(adduct.ToString) = New list With {
                   .slots = New Dictionary(Of String, Object) From {
                       {"products", anno.products},
                       {"formula", f},
                       {"adduct", adduct.ToString},
                       {"charge", anno.formula.charge},
                       {"ppm", anno.formula.ppm},
                       {"massdiff", anno.formula.massdiff}
                   }
                }
            Else
                results.slots(adduct.ToString) = anno
            End If
        Next

        If adductList.Length = 1 Then
            Return results.data.First
        Else
            Return results
        End If
    End Function

    ''' <summary>
    ''' find all of the candidate chemical formulas by a 
    ''' specific exact mass value and a specific mass 
    ''' tolerance value in ppm
    ''' </summary>
    ''' <param name="mass">the exact mass value</param>
    ''' <param name="ppm">the mass tolerance value in ppm</param>
    ''' <param name="candidateElements">
    ''' a list configuration of the formula candidates
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("candidates")>
    <RApiReturn(GetType(FormulaComposition))>
    Public Function FormulaFinder(mass#,
                                  Optional ppm# = 5,
                                  <RListObjectArgument>
                                  Optional candidateElements As list = Nothing,
                                  Optional env As Environment = Nothing) As Object

        Dim opts As New SearchOption(-9999, 9999, ppm)

        For Each element As String In candidateElements.getNames
            Dim value As Object = candidateElements.getByName(element)

            If Formula.AllAtomElements.ContainsKey(element) Then
                Dim range = SMRUCC.Rsharp.GetDoubleRange(value, env, [default]:="0,1")

                If range Like GetType(Message) Then
                    Call env.AddMessage(range.TryCast(Of Message).message, MSG_TYPES.WRN)
                    Call opts.AddElement(element, 0, 1)
                Else
                    With range.TryCast(Of DoubleRange)
                        Call opts.AddElement(element, .Min, .Max)
                    End With
                End If
            End If
        Next

        Dim oMwtWin As New FormulaSearch(opts)
        Dim results As FormulaComposition() = oMwtWin.SearchByExactMass(mass).ToArray

        Return results
    End Function

    ''' <summary>
    ''' ### Evaluate formula exact mass
    ''' 
    ''' evaluate exact mass for the given formula strings.
    ''' </summary>
    ''' <param name="formula">
    ''' a vector of the character formulas.
    ''' </param>
    ''' <returns>
    ''' the result data type is keeps the same as the given data type of the
    ''' <paramref name="formula"/> parameter: this function returns a numeric 
    ''' vector if the given <paramref name="formula"/> is a character vector,
    ''' or this function returns a key-value pair tuple list if the given
    ''' <paramref name="formula"/> is a list.
    ''' </returns>
    ''' <remarks>
    ''' -1 will be return if the given formula is empty or error/invalid
    ''' </remarks>
    <ExportAPI("eval")>
    <RApiReturn(GetType(Double))>
    Public Function EvalFormula(<RRawVectorArgument> formula As Object, Optional env As Environment = Nothing) As Object
        If TypeOf formula Is Formula Then
            Return CDbl(DirectCast(formula, Formula))
        End If

        Return env.EvaluateFramework(Of String, Double)(
            x:=formula,
            eval:=Function(str)
                      Return CDbl(FormulaScanner.ScanFormula(str))
                  End Function)
    End Function

    ''' <summary>
    ''' Get atom composition from a formula string
    ''' </summary>
    ''' <param name="formula">The input formula string text.</param>
    ''' <returns></returns>
    <ExportAPI("scan")>
    <RSymbolLanguageMask(FormulaScanner.Pattern, True, Test:=GetType(TestFormulaSymbolLang))>
    <RApiReturn(GetType(Formula))>
    Public Function ScanFormula(formula$, Optional env As Environment = Nothing) As Formula
        Dim globalEnv As GlobalEnvironment = env.globalEnvironment
        Dim n As Integer = globalEnv.options.getOption("formula.polymers_n", 999)
        Dim formulaObj As Formula = FormulaScanner.ScanFormula(formula, n)

        Return formulaObj
    End Function

    ''' <summary>
    ''' Make the given formula string into canonical format
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    <ExportAPI("canonical_formula")>
    Public Function canonicalFormula(formula As Formula) As String
        Return Canonical.BuildCanonicalFormula(formula.CountsByElement)
    End Function

#Region "formula operators"

    <ExportAPI("getElementCount")>
    Public Function getElementCount(formula As Formula, element As String) As Integer
        Return formula(element)
    End Function

    <ExportAPI("removeElement")>
    Public Function removeElement(formula As Formula, element As String) As Formula
        formula = New Formula(formula.CountsByElement, formula.EmpiricalFormula)

        If formula.CountsByElement.ContainsKey(element) Then
            formula.CountsByElement.Remove(element)
        End If

        Return formula
    End Function

    <ROperator("+")>
    Public Function add(part1 As Formula, part2 As Formula) As Formula
        Return part1 + part2
    End Function

    ''' <summary>
    ''' Removes the precrusor ion groups
    ''' </summary>
    ''' <param name="ionFormula"></param>
    ''' <param name="precursor"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ROperator("-")>
    Public Function minus(ionFormula As Formula, precursor As MzCalculator, Optional env As Environment = Nothing) As Formula
        Dim ionName As String = precursor.name
        Dim ion = Parser.Formula(precursor.name)

        If ion Like GetType(String) Then
            Throw New InvalidExpressionException(ion.TryCast(Of String))
        Else
            For Each part In ion.TryCast(Of IEnumerable(Of (sign As Integer, expr As String)))
                Dim subIon As Formula = FormulaScanner.ScanFormula(part.expr)

                subIon *= stdNum.Abs(part.sign)

                If part.sign > 0 Then
                    ' delete part
                    ionFormula -= subIon
                Else
                    ' add part
                    ionFormula += subIon
                End If
            Next
        End If

        Return ionFormula
    End Function

    <ROperator("-")>
    Public Function minus(total As Formula, part As Formula, Optional env As Environment = Nothing) As Formula
        Dim delta = total - part
        Dim negative = delta.CountsByElement.Where(Function(c) c.Value < 0).ToDictionary

        If Not negative.IsNullOrEmpty Then
            Call env.AddMessage({
                $"formula math results negative composition result: {negative.GetJson}",
                $"total: {total.EmpiricalFormula}",
                $"part: {part.EmpiricalFormula}"
            }, MSG_TYPES.WRN)
        End If

        Return delta
    End Function

    <ROperator("*")>
    Public Function repeats(part As Formula, n As Integer) As Formula
        Return part * n
    End Function

    <ROperator("*")>
    Public Function repeats(n As Integer, part As Formula) As Formula
        Return part * n
    End Function

    <ROperator("/")>
    Public Function divide(total As Formula, n As Integer) As Formula
        Return total / n
    End Function

    <ROperator("-")>
    Public Function minus(f As Formula, mass As Double) As Double
        Return f.ExactMass - mass
    End Function

    <ROperator("-")>
    Public Function minus(mass As Double, f As Formula) As Double
        Return mass - f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(mass As Double, f As Formula) As Double
        Return mass + f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(f As Formula, mass As Double) As Double
        Return f.ExactMass + mass
    End Function

    <ROperator("-")>
    Public Function minus(f As Formula, mass As Integer) As Double
        Return f.ExactMass - mass
    End Function

    <ROperator("-")>
    Public Function minus(mass As Integer, f As Formula) As Double
        Return mass - f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(mass As Integer, f As Formula) As Double
        Return mass + f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(f As Formula, mass As Integer) As Double
        Return f.ExactMass + mass
    End Function

#End Region

    ''' <summary>
    ''' Read KCF model data
    ''' </summary>
    ''' <param name="data">The text data or file path</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("read.KCF")>
    Public Function readKCF(data As String) As Model.KCF
        Return Model.IO.LoadKCF(data)
    End Function

    ''' <summary>
    ''' Create molecular network graph model
    ''' </summary>
    ''' <param name="kcf">The KCF molecule model</param>
    ''' <returns></returns>
    <ExportAPI("KCF.graph")>
    Public Function CreateGraph(kcf As Model.KCF) As NetworkGraph
        Return kcf.CreateGraph
    End Function

    ''' <summary>
    ''' parse a single sdf text block
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="parseStruct"></param>
    ''' <returns></returns>
    <ExportAPI("parse.SDF")>
    <RApiReturn(GetType(SDF))>
    Public Function readSDF(data As String, Optional parseStruct As Boolean = True) As SDF
        Return SDF.ParseSDF(data.SolveStream, parseStruct)
    End Function

    <ExportAPI("SDF.convertKCF")>
    Public Function SDF2KCF(sdfModel As SDF) As Model.KCF
        Return sdfModel.ToKCF
    End Function

    '<ExportAPI("download.kcf")>
    'Public Function DownloadKCF(keggcompoundIDs As String(), save$) As Object
    '    Dim result As New List(Of Object)
    '    Dim KCF$

    '    For Each id As String In keggcompoundIDs.SafeQuery
    '        KCF = MetaboliteWebApi.DownloadKCF(id, saveDIR:=save)

    '        Call result.Add(KCF)
    '        Call Thread.Sleep(1000)
    '    Next

    '    Return result.ToArray
    'End Function

    ''' <summary>
    ''' open the file handles of the chemical descriptor database. 
    ''' </summary>
    ''' <param name="dbFile">
    ''' A directory path which contains the multiple database file of the 
    ''' chemical descriptors.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open.descriptor.db")>
    Public Function openChemicalDescriptorDatabase(dbFile As String) As PubChemDescriptorRepo
        Return New PubChemDescriptorRepo(dir:=dbFile)
    End Function

    <ExportAPI("descriptor.matrix")>
    <RApiReturn(GetType(DataSet()))>
    Public Function LoadChemicalDescriptorsMatrix(repo As PubChemDescriptorRepo, cid As Long(), Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return Internal.debug.stop("no chemical descriptor database was provided!", env)
        ElseIf cid.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim matrix As New List(Of DataSet)
        Dim descriptor As ChemicalDescriptor
        Dim row As DataSet

        For Each id As Long In cid
            descriptor = repo.GetDescriptor(cid:=id)
            row = New DataSet With {
                .ID = id,
                .Properties = New Dictionary(Of String, Double) From {
                    {NameOf(descriptor.AtomDefStereoCount), descriptor.AtomDefStereoCount},
                    {NameOf(descriptor.AtomUdefStereoCount), descriptor.AtomUdefStereoCount},
                    {NameOf(descriptor.BondDefStereoCount), descriptor.BondDefStereoCount},
                    {NameOf(descriptor.BondUdefStereoCount), descriptor.BondUdefStereoCount},
                    {NameOf(descriptor.Complexity), descriptor.Complexity},
                    {NameOf(descriptor.ComponentCount), descriptor.ComponentCount},
                    {NameOf(descriptor.ExactMass), descriptor.ExactMass},
                    {NameOf(descriptor.FormalCharge), descriptor.FormalCharge},
                    {NameOf(descriptor.HeavyAtoms), descriptor.HeavyAtoms},
                    {NameOf(descriptor.HydrogenAcceptor), descriptor.HydrogenAcceptor},
                    {NameOf(descriptor.HydrogenDonors), descriptor.HydrogenDonors},
                    {NameOf(descriptor.IsotopicAtomCount), descriptor.IsotopicAtomCount},
                    {NameOf(descriptor.RotatableBonds), descriptor.RotatableBonds},
                    {NameOf(descriptor.TautoCount), descriptor.TautoCount},
                    {NameOf(descriptor.TopologicalPolarSurfaceArea), descriptor.TopologicalPolarSurfaceArea},
                    {NameOf(descriptor.XLogP3), descriptor.XLogP3},
                    {NameOf(descriptor.XLogP3_AA), descriptor.XLogP3_AA}
                }
            }

            If Not row.Properties.Values.All(Function(x) x = 0.0) Then
                ' is not empty
                matrix += row
            End If
        Next

        Return matrix.ToArray
    End Function

    <ExportAPI("isotope_distribution")>
    <RApiReturn(GetType(IsotopeDistribution))>
    Public Function IsotopeDistributionSearch(formula As Object,
                                              Optional prob_threshold As Double = 0.001,
                                              Optional fwhm As Double = 0.1,
                                              Optional pad_left As Double = 3,
                                              Optional pad_right As Double = 3,
                                              Optional interpolate_grid As Double = 0.005) As IsotopeDistribution

        If Not TypeOf formula Is Formula Then
            formula = FormulaScanner.ScanFormula(any.ToString(formula))
        End If

        Return IsotopicPatterns.Calculator.GenerateDistribution(
            formula:=formula,
            prob_threshold:=prob_threshold,
            fwhm:=fwhm,
            pad_left:=pad_left,
            pad_right:=pad_right,
            interpolate_grid:=interpolate_grid
        )
    End Function
End Module
