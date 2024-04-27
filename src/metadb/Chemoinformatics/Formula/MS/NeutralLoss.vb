#Region "Microsoft.VisualBasic::b3dd250c21600ffc53a3ab73d1b2f98a, G:/mzkit/src/metadb/Chemoinformatics//Formula/MS/NeutralLoss.vb"

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

    '   Total Lines: 102
    '    Code Lines: 62
    ' Comment Lines: 6
    '   Blank Lines: 34
    '     File Size: 3.28 KB


    '     Class ProductIon
    ' 
    '         Properties: CandidateInChIKeys, CandidateOntologies, Comment, Formula, Frequency
    '                     Intensity, IonMode, IsotopeDiff, Mass, MassDiff
    '                     Name, ShortName, Smiles
    ' 
    '         Function: GetDefault
    ' 
    '     Class NeutralLoss
    ' 
    '         Properties: CandidateInChIKeys, CandidateOntologies, Comment, Formula, Frequency
    '                     Iontype, MassError, MassLoss, Name, PrecursorIntensity
    '                     PrecursorMz, ProductIntensity, ProductMz, ShortName, Smiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS.AtomGroups
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Formula.MS

    ''' <summary>
    ''' This class is the storage of product ion assignment used in MS-FINDER program.
    ''' </summary>
    Public Class ProductIon

        Public Property Mass As Double

        Public Property Intensity As Double

        Public Property Formula As Formula

        Public Property IonMode As IonModes

        Public Property Smiles As String

        Public Property MassDiff As Double

        Public Property IsotopeDiff As Double

        Public Property Comment As String

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As List(Of String) = New List(Of String)()

        Public Property Frequency As Double

        Public Property CandidateOntologies As List(Of String) = New List(Of String)()

        Public Shared Iterator Function GetDefault() As IEnumerable(Of ProductIon)
            For Each container As Type In {
                GetType(Alkenyl), GetType(Alkyl), GetType(Amines),
                GetType(Glycosides), GetType(Ketones), GetType(Lipids),
                GetType(Others)
            }
                For Each atom_group As PropertyInfo In container.GetProperties(DataFramework.PublicShared)
                    Yield New ProductIon With {
                       .Formula = atom_group.GetValue(Nothing),
                       .IonMode = IonModes.Positive,
                       .Name = atom_group.Name,
                       .ShortName = .Name,
                       .Mass = .Formula.ExactMass
                    }

                    Yield New ProductIon With {
                       .Formula = atom_group.GetValue(Nothing),
                       .IonMode = IonModes.Negative,
                       .Name = atom_group.Name,
                       .ShortName = .Name,
                       .Mass = .Formula.ExactMass
                    }
                Next
            Next
        End Function
    End Class

    ''' <summary>
    ''' This class is the storage of each neutral loss information used in MS-FINDER program.
    ''' </summary>
    Public Class NeutralLoss

        Public Property MassLoss As Double

        Public Property PrecursorMz As Double

        Public Property ProductMz As Double

        Public Property PrecursorIntensity As Double

        Public Property ProductIntensity As Double

        Public Property MassError As Double

        Public Property Formula As Formula = New Formula()

        Public Property Iontype As IonModes

        Public Property Comment As String

        Public Property Smiles As String

        Public Property Frequency As Double

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As List(Of String) = New List(Of String)()

        Public Property CandidateOntologies As List(Of String) = New List(Of String)()

    End Class
End Namespace
