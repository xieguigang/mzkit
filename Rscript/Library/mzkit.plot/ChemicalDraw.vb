
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Drawing
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("ChemicalDraw")>
Module ChemicalDraw

    <ExportAPI("KCFDraw")>
    Public Function DrawKCF(molecule As KCF) As GraphicsData
        Return molecule.Draw
    End Function

    <ExportAPI("as.kcf")>
    <RApiReturn(GetType(KCF))>
    Public Function AsKCF(chemical As Object, Optional env As Environment = Nothing) As Object
        If chemical Is Nothing Then
            Return Internal.debug.stop("the given chemical data is nothing!", env)
        ElseIf TypeOf chemical Is ChemicalFormula Then
            Return DirectCast(chemical, ChemicalFormula).ToKCF
        Else
            Return Internal.debug.stop(New NotImplementedException(chemical.GetType.FullName), env)
        End If
    End Function
End Module
