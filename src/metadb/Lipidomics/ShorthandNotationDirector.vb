#Region "Microsoft.VisualBasic::d46c75ce75450ef8fa5cfcb31cd50cb8, metadb\Lipidomics\ShorthandNotationDirector.vb"

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

    '   Total Lines: 64
    '    Code Lines: 51
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 3.02 KB


    ' Class ShorthandNotationDirector
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Construct, SetDoubleBondNumberLevel, SetDoubleBondPositionLevel, SetMolecularSpeciesLevel, SetOxidizedNumberLevel
    '          SetOxidizedPositionLevel, SetPositionLevel, SetSpeciesLevel
    ' 
    ' /********************************************************************************/

#End Region

Public NotInheritable Class ShorthandNotationDirector
    Private ReadOnly _builder As ILipidomicsVisitorBuilder

    Public Sub New(builder As ILipidomicsVisitorBuilder)
        _builder = builder
        builder.SetAcylOxidized(OxidizedIndeterminateState.Identity)
        builder.SetAcylDoubleBond(DoubleBondIndeterminateState.Identity)
        builder.SetAlkylOxidized(OxidizedIndeterminateState.Identity)
        builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.Identity)
        builder.SetSphingoOxidized(OxidizedIndeterminateState.Identity)
        builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.Identity)
        builder.SetChainsState(ChainsIndeterminateState.PositionLevel)
    End Sub

    Public Sub Construct()
        ' for acyl
        _builder.SetAcylOxidized(OxidizedIndeterminateState.AllPositions)
        _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllPositions)
        ' for alkyl
        _builder.SetAlkylOxidized(OxidizedIndeterminateState.AllPositions)
        _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllPositions.Exclude(1))
        ' for sphingosine
        _builder.SetSphingoOxidized(OxidizedIndeterminateState.AllPositions.Exclude(1))
        _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllPositions)
        ' for chains
        _builder.SetChainsState(ChainsIndeterminateState.SpeciesLevel)
    End Sub

    Public Sub SetSpeciesLevel()
        _builder.SetChainsState(ChainsIndeterminateState.SpeciesLevel)
    End Sub

    Public Sub SetMolecularSpeciesLevel()
        _builder.SetChainsState(ChainsIndeterminateState.MolecularSpeciesLevel)
    End Sub

    Public Sub SetPositionLevel()
        _builder.SetChainsState(ChainsIndeterminateState.PositionLevel)
    End Sub

    Public Sub SetDoubleBondPositionLevel()
        _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
        _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
        _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
    End Sub

    Public Sub SetDoubleBondNumberLevel()
        _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllPositions)
        _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllPositions.Exclude(1))
        _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllPositions)
    End Sub

    Public Sub SetOxidizedPositionLevel()
        _builder.SetAcylOxidized(OxidizedIndeterminateState.Identity)
        _builder.SetAlkylOxidized(OxidizedIndeterminateState.Identity)
        _builder.SetSphingoOxidized(OxidizedIndeterminateState.Identity)
    End Sub

    Public Sub SetOxidizedNumberLevel()
        _builder.SetAcylOxidized(OxidizedIndeterminateState.AllPositions)
        _builder.SetAlkylOxidized(OxidizedIndeterminateState.AllPositions)
        _builder.SetSphingoOxidized(OxidizedIndeterminateState.AllPositions.Exclude(1))
    End Sub
End Class
