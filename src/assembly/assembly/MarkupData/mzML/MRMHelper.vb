Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.IonTargeted
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML

    Public Module MRMHelper

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MRMTargetMz(selector As IMRMSelector) As Double
            Return selector _
                .isolationWindow _
                .cvParams _
                .KeyItem("isolation window target m/z") _
                .value
        End Function
    End Module
End Namespace