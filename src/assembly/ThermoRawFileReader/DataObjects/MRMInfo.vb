Imports System.Runtime.InteropServices

Namespace DataObjects

    ''' <summary>
    ''' Type for MRM Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Class MRMInfo

        '''         <summary>
        '''      List of mass ranges monitored by the first quadrupole
        '''        </summary>
        Public ReadOnly MRMMassList As New List(Of MRMMassRangeType)


        ''' <summary>
        ''' Duplicate the MRM info
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="target"></param>
        Public Shared Sub DuplicateMRMInfo(source As MRMInfo, <Out> ByRef target As MRMInfo)
            target = New MRMInfo()

            For Each item In source.MRMMassList
                target.MRMMassList.Add(item)
            Next
        End Sub
    End Class
End Namespace