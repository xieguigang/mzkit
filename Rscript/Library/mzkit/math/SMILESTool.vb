
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Embedding
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

<Package("SMILES", Category:=APICategories.UtilityTools)>
Module SMILESTool

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(ChemicalFormula), AddressOf atoms_table)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function atoms_table(smiles As ChemicalFormula, args As List, env As Environment) As RDataframe
        Return atomGroups(smiles)
    End Function


    ''' <summary>
    ''' Parse the SMILES molecule structre string
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <param name="strict"></param>
    ''' <returns>
    ''' A chemical graph object that could be used for build formula or structure analysis
    ''' </returns>
    ''' <remarks>
    ''' SMILES denotes a molecular structure as a graph with optional chiral 
    ''' indications. This is essentially the two-dimensional picture chemists
    ''' draw to describe a molecule. SMILES describing only the labeled
    ''' molecular graph (i.e. atoms and bonds, but no chiral or isotopic 
    ''' information) are known as generic SMILES.
    ''' </remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("parse")>
    Public Function parseSMILES(SMILES As String, Optional strict As Boolean = True) As ChemicalFormula
        Return ParseChain.ParseGraph(SMILES, strict)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.formula")>
    Public Function asFormula(SMILES As ChemicalFormula, Optional canonical As Boolean = True) As Formula
        Return SMILES.GetFormula(canonical)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.graph")>
    Public Function asGraph(smiles As ChemicalFormula) As NetworkGraph
        Return smiles.AsGraph
    End Function

    ''' <summary>
    ''' get atoms table from the SMILES structure data
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <returns></returns>
    <ExportAPI("atoms")>
    Public Function atomGroups(SMILES As ChemicalFormula) As RDataframe
        Dim elements As SmilesAtom() = SMILES.GetAtomTable.ToArray
        Dim rowKeys As String() = elements.Select(Function(a) a.id).ToArray
        Dim atoms As String() = elements.Select(Function(a) a.atom).ToArray
        Dim groups As String() = elements.Select(Function(a) a.group).ToArray
        Dim ionCharge As Integer() = elements.Select(Function(a) a.ion_charge).ToArray
        Dim links As Integer() = elements.Select(Function(a) a.links).ToArray
        Dim partners As String() = elements.Select(Function(a) a.connected.JoinBy("; ")).ToArray

        Return New RDataframe With {
            .rownames = rowKeys,
            .columns = New Dictionary(Of String, Array) From {
                {"atom", atoms},
                {"group", groups},
                {"ion_charge", ionCharge},
                {"links", links},
                {"connected", partners}
            }
        }
    End Function
End Module
