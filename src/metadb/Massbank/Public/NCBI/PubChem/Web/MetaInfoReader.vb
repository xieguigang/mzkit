#Region "Microsoft.VisualBasic::010356209f2376e27bfa3522fb41cecd, mzkit\src\metadb\Massbank\Public\NCBI\PubChem\Web\MetaInfoReader.vb"

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

    '   Total Lines: 335
    '    Code Lines: 282
    ' Comment Lines: 18
    '   Blank Lines: 35
    '     File Size: 14.97 KB


    '     Module MetaInfoReader
    ' 
    '         Function: GetInform, GetInformList, GetMetaInfo, getSynonyms, getXrefId
    '                   navigateView, parseChemical, removesDbEntry, safeProject, stripMarkupString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports any = Microsoft.VisualBasic.Scripting
Imports MetaInfo = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace NCBI.PubChem

    <HideModuleName>
    Public Module MetaInfoReader

        ''' <summary>
        ''' 如果<paramref name="path"/>的末端是使用索引语法,则索引的起始下标是从零开始的
        ''' </summary>
        ''' <param name="view"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetInform(view As PugViewRecord, path$) As Information
            Dim parts = path.Trim("/"c).Split("/"c)
            Dim section = view.navigateView(parts)

            Return section.GetInformation(parts.Last, multipleInfo:=False)
        End Function

        <Extension>
        Private Function navigateView(view As PugViewRecord, parts As String()) As Section
            If parts.Length = 1 OrElse view Is Nothing Then
                Return Nothing
            End If

            Dim sec As Section = view(parts(Scan0))

            For Each part As String In parts.Skip(1).Take(parts.Length - 2)
                If sec Is Nothing Then
                    Return Nothing
                Else
                    sec = sec(part)
                End If
            Next

            Return sec
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="view"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetInformList(view As PugViewRecord, path$) As Information()
            Dim parts = path.Trim("/"c).Split("/"c)
            Dim section = view.navigateView(parts)

            Return section.GetInformation(parts.Last, multipleInfo:=True)
        End Function

        ReadOnly nameDatabase As Index(Of String) = {
            "Human Metabolome Database (HMDB)",
            "ChEBI",
            "DrugBank",
            "European Chemicals Agency (ECHA)",
            "MassBank of North America (MoNA)"
        }

        <Extension>
        Private Iterator Function getSynonyms(names As Section) As IEnumerable(Of String)
            If names Is Nothing Then
                Return
            End If

            Dim depositor = names("Depositor-Supplied Synonyms")
            Dim mesh = names("MeSH Entry Terms")

            If Not depositor Is Nothing Then
                For Each info As Information In depositor.Information
                    If info.InfoType Is GetType(String) Then
                        Yield info.InfoValue
                    Else
                        For Each value As String In DirectCast(info.InfoValue, String())
                            Yield value
                        Next
                    End If
                Next
            End If

            If Not mesh Is Nothing Then
                For Each info As Information In mesh.Information
                    If info.InfoType Is GetType(String) Then
                        Yield info.InfoValue
                    Else
                        For Each value As String In DirectCast(info.InfoValue, String())
                            Yield value
                        Next
                    End If
                Next
            End If
        End Function

        ''' <summary>
        ''' 从pubchem数据库之中提取注释所需要的必须基本信息
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetMetaInfo(view As PugViewRecord) As MetaInfo
            If view Is Nothing Then
                Return Nothing
            End If

            Dim identifier As Section = view("Names and Identifiers")
            Dim formula = view.GetInform("/Names and Identifiers/Molecular Formula/#0")
            Dim descriptors = identifier("Computed Descriptors")
            Dim SMILES As String = any.ToString(view.GetInform("/Names and Identifiers/Computed Descriptors/Canonical SMILES/#0").InfoValue).stripMarkupString
            Dim IUPAC As String = any.ToString(view.GetInform("/Names and Identifiers/Computed Descriptors/IUPAC Name/#0").InfoValue).stripMarkupString
            Dim desc As String() = view _
                .GetInformList("/Names and Identifiers/Record Description/*") _
                .Select(Function(i)
                            Return any.ToString(i.InfoValue).stripMarkupString
                        End Function) _
                .ToArray
            Dim InChIKey = descriptors("InChI Key").GetInformationString("#0").stripMarkupString
            Dim InChI = descriptors("InChI").GetInformationString("#0").stripMarkupString
            Dim otherNames = identifier("Other Identifiers")
            Dim synonyms = identifier("Synonyms") _
                .getSynonyms _
                .Distinct _
                .Select(Function(name) name.stripMarkupString) _
                .OrderBy(Function(s) s) _
                .ToArray
            Dim computedProperties As Section = view("Chemical and Physical Properties")("Computed Properties")
            Dim experimentProperties As Section = view("Chemical and Physical Properties")("Experimental Properties")
            Dim otherId = view _
                .GetInformList("/Names and Identifiers/Synonyms/Depositor-Supplied Synonyms/*") _
                .Select(Function(a) any.ToString(a.InfoValue).stripMarkupString) _
                .ToArray
            Dim tissues = view _
                 .GetInform("/Pharmacology and Biochemistry/Human Metabolite Information/Tissue Locations/*") _
                ?.Value _
                ?.StringWithMarkup _
                 .SafeQuery _
                 .Select(Function(a) a.String.stripMarkupString) _
                 .ToArray
            Dim taxon = view("Taxonomy") _
                .GetInformation("*", multipleInfo:=True) _
                .TryCast(Of Information()) _
                .Where(Function(i) Not i.URL.StringEmpty) _
                .Select(Function(i)
                            Return any.ToString(i.InfoValue).stripMarkupString
                        End Function) _
                .ToArray
            Dim CASNumber$()
            Dim wikipedia As String
            Dim referenceList As Reference() = view.Reference

            If referenceList Is Nothing Then
                referenceList = {}
            End If
            If synonyms Is Nothing Then
                synonyms = {}
            End If

            If otherNames Is Nothing Then
                CASNumber = synonyms _
                    .Where(Function(id) id.IsPattern("\d+([-]\d+)+")) _
                    .ToArray
                wikipedia = referenceList.GetReferenceID("Wikipedia")
            Else
                CASNumber = otherNames("CAS")?.GetInformationStrings("CAS", True)
                wikipedia = otherNames("Wikipedia")?.GetInformationString("Wikipedia")
            End If

            Dim exact_mass# = computedProperties("Exact Mass").GetInformationNumber(Nothing)
            Dim xref As New xref With {
                .InChI = InChI,
                .CAS = CASNumber.Distinct.ToArray,
                .InChIkey = InChIKey,
                .pubchem = view.RecordNumber,
                .chebi = getXrefId(synonyms, otherId, Function(id) id.IsPattern("CHEBI[:]\d+")),
                .KEGG = getXrefId(synonyms, otherId, Function(id) id.IsPattern("C\d{5}", RegexOptions.Singleline)), ' KEGG编号是C开头,后面跟随5个数字
                .HMDB = referenceList.GetReferenceID(PugViewRecord.HMDB),
                .SMILES = SMILES,
                .DrugBank = referenceList.GetReferenceID(PugViewRecord.DrugBank),
                .ChEMBL = getXrefId(synonyms, otherId, Function(id) id.StartsWith("ChEMBL")),
                .Wikipedia = wikipedia,
                .lipidmaps = referenceList.GetReferenceID("LIPID MAPS"),
                .MeSH = referenceList.GetReferenceID("Medical Subject Headings (MeSH)", name:=True),
                .ChemIDplus = referenceList.GetReferenceID("ChemIDplus")
            }
            Dim commonName$ = view.RecordTitle

            If commonName.StringEmpty Then
                commonName = view _
                    .Reference _
                    .FirstOrDefault(Function(r) r.SourceName Like nameDatabase) _
                   ?.Name
            End If

            ' 20190618 formula可能会存在多个的情况
            Dim formulaStr As String = ""

            If Not formula Is Nothing Then
                If formula.InfoType Is GetType(String) Then
                    formulaStr = formula.InfoValue
                Else
                    formulaStr = DirectCast(formula.InfoValue, String()).FirstOrDefault
                End If
            End If

            Return New MetaInfo With {
                .formula = formulaStr,
                .xref = xref,
                .name = commonName,
                .exact_mass = exact_mass,
                .ID = view.RecordNumber,
                .synonym = synonyms.removesDbEntry.ToArray,
                .organism = taxon,
                .chemical = computedProperties.parseChemical().parseExperimentals(experimentProperties),
                .samples = tissues,
                .IUPACName = IUPAC,
                .description = desc.JoinBy(vbCrLf)
            }
        End Function

        <Extension>
        Public Function getXrefId(synonyms As String(), otherId As String(), getId As Func(Of String, Boolean)) As String
            Dim id As String = synonyms.Where(getId).FirstOrDefault

            If id.StringEmpty Then
                id = otherId.Where(getId).FirstOrDefault
            End If

            Return id
        End Function

        <Extension>
        Private Function stripMarkupString(str As String) As String
            Return str.TrimNewLine.Trim.StringReplace("\s{2,}", " ")
        End Function

        <Extension>
        Private Iterator Function removesDbEntry(synonyms As String()) As IEnumerable(Of String)
            For Each name As String In synonyms
                If name.IsPattern("\d+") Then
                    Continue For
                End If
                If name.Match("\d+").Length > 2 Then
                    Continue For
                End If
                If xref.IsCASNumber(name) Then
                    Continue For
                End If

                Yield name.stripMarkupString
            Next
        End Function

        <Extension>
        Private Function parseExperimentals(desc As ChemicalDescriptor, experiments As Section)
            If Not experiments Is Nothing Then
                desc.LogP = experiments.safeProject(key:="LogP",
                                                    Function(info)
                                                        Return info.Select(Function(c)
                                                                               Return New Chemoinformatics.Value With {
                                                                                  .value = c.GetInformationNumber,
                                                                                  .reference = c.Reference
                                                                               }
                                                                           End Function).ToArray
                                                    End Function)
                desc.CCS = experiments.safeProject(
                    key:="Collision Cross Section",
                     Function(info)
                         Return info _
                            .Select(Function(c)
                                        Return New CCS With {
                                            .value = any.ToString(c.InfoValue).stripMarkupString,
                                            .reference = c.Reference.stripMarkupString
                                        }
                                    End Function) _
                            .ToArray
                     End Function)
                desc.Solubility = experiments.safeProject(
                    key:="Solubility",
                     Function(info)
                         Return info _
                            .Where(Function(a)
                                       Return Not a.UnitValue Is Nothing
                                   End Function) _
                            .Select(Function(a) a.UnitValue) _
                            .ToArray
                     End Function)
                desc.MeltingPoint = experiments.safeProject(
                    key:="Melting Point",
                     Function(info)
                         Return info _
                            .Where(Function(a)
                                       Return Not a.UnitValue Is Nothing
                                   End Function) _
                            .Select(Function(a) a.UnitValue) _
                            .ToArray
                     End Function)
            End If

            Return desc
        End Function

        <Extension>
        Private Function parseChemical(computedProperties As Section) As ChemicalDescriptor
            Dim desc As New ChemicalDescriptor With {
                .XLogP3 = computedProperties("XLogP3").GetInformationNumber("*"),
                .AtomDefStereoCount = computedProperties("Defined Atom Stereocenter Count").GetInformationNumber("*"),
                .AtomUdefStereoCount = computedProperties("Undefined Atom Stereocenter Count").GetInformationNumber("*"),
                .BondDefStereoCount = computedProperties("Defined Bond Stereocenter Count").GetInformationNumber("*"),
                .BondUdefStereoCount = computedProperties("Undefined Bond Stereocenter Count").GetInformationNumber("*"),
                .Complexity = computedProperties("Complexity").GetInformationNumber("*"),
                .ComponentCount = computedProperties("").GetInformationNumber("*"),
                .ExactMass = computedProperties("Exact Mass").GetInformationNumber("*"),
                .FormalCharge = computedProperties("Formal Charge").GetInformationNumber("*"),
                .HeavyAtoms = computedProperties("Heavy Atom Count").GetInformationNumber("*"),
                .HydrogenAcceptor = computedProperties("Hydrogen Bond Acceptor Count").GetInformationNumber("*"),
                .HydrogenDonors = computedProperties("Hydrogen Bond Donor Count").GetInformationNumber("*"),
                .IsotopicAtomCount = computedProperties("Isotope Atom Count").GetInformationNumber("*"),
                .RotatableBonds = computedProperties("Rotatable Bond Count").GetInformationNumber("*"),
                .TautoCount = computedProperties("").GetInformationNumber("*"),
                .TopologicalPolarSurfaceArea = computedProperties("Topological Polar Surface Area").GetInformationNumber("*"),
                .XLogP3_AA = computedProperties("").GetInformationNumber("*"),
                .CovalentlyBonded = computedProperties("Covalently-Bonded Unit Count").GetInformationNumber("*")
            }

            Return desc
        End Function

        <Extension>
        Private Function safeProject(Of T)(info As Section, key As String, project As Func(Of Information(), T)) As T
            Dim raw As Section = info(key)

            If raw Is Nothing Then
                Return Nothing
            Else
                Return project(raw.Information)
            End If
        End Function
    End Module
End Namespace
