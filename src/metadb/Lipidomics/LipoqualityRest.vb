Imports System.ComponentModel
Imports CompMs.Common.Components
Imports System.Runtime.InteropServices

Public NotInheritable Class LipoqualityRest

    Private Sub New()
    End Sub

    Public Shared Sub GoToLQDB(ByVal query As MoleculeMsReference, ByVal lipidName As String, <Out> ByRef [error] As String)
        [error] = String.Empty
        Dim lipidinfo = LipoqualityDatabaseManagerUtility.ConvertMsdialLipidnameToLipidAnnotation(query, lipidName)
        If lipidinfo Is Nothing Then
            [error] = "Type 1 error: Lipid name format invalid"
            'MessageBox.Show("Type 1 error: Lipid name format invalid", "Error", MessageBoxButton.OK);
            Return
        End If

        Dim url = getLqUrl(lipidinfo)
        If Equals(url, String.Empty) Then
            [error] = "Type 2 error: Lipid name format invalid"
            'MessageBox.Show("Type 2 error: Lipid name format invalid", "Error", MessageBoxButton.OK);
            Return
        End If

        Try
            Process.Start(url)
        Catch ex As Win32Exception

        End Try
    End Sub

    Private Shared Function getLqUrl(ByVal lipidinfo As LipoqualityAnnotation) As String
        Select Case lipidinfo.LipidClass

                'Glycerolipid
            Case "MAG"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "MAG " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "DAG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DAG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DAG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "TAG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn3AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "TAG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "/" & lipidinfo.Sn3AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn3AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "TAG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Lyso phospholipid
            Case "LPC"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPC " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LPE"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPE " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LPG"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPG " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LPI"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPI " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LPS"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPS " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LPA"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LPA " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "LDGTS"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "LDGTS " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If

                'Phospholipid
            Case "PC"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PC " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PC " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PE"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PE " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PE " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PI"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PI " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PI " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PA"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PA " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PA " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "BMP"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "BMP " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "BMP " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "HBMP"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn3AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "HBMP " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "/" & lipidinfo.Sn3AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn3AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "HBMP " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "CL"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn3AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn4AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "CL " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "/" & lipidinfo.Sn3AcylChain & "/" & lipidinfo.Sn4AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn3AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn4AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "CL " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Ether linked phospholipid
            Case "EtherPC"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PC " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PC " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "EtherPE"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PE " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PE " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Oxidized phospholipid
            Case "OxPC"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPC " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPC " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "OxPE"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPE " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPE " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "OxPG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "OxPI"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPI " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPI " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "OxPS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Oxidized ether linked phospholipid
            Case "EtherOxPC"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPC " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPC " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "EtherOxPE"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPE " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "OxPE " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Oxidized ether linked phospholipid
            Case "PMeOH"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PMeOH " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PMeOH " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PEtOH"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PEtOH " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PEtOH " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "PBtOH"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PBtOH " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "PBtOH " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Plantlipid
            Case "MGDG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "MGDG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "MGDG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "DGDG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DGDG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DGDG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "SQDG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SQDG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SQDG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "DGTS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DGTS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DGTS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "GlcADG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcADG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcADG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "AcylGlcADG"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn3AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "AcylGlcADG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "/" & lipidinfo.Sn3AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn3AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "AcylGlcADG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Others
            Case "CE"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "CE " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "ACar"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "ACar " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "FA", "DMEDFA", "OxFA", "DMEDOxFA"
                If Equals(lipidinfo.Sn1AcylChain, String.Empty) Then
                    Return String.Empty
                Else
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "FA " & lipidinfo.Sn1AcylChain & "&ct=c"
                End If
            Case "FAHFA", "DMEDFAHFA"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DAG " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "DAG " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Sphingomyelin
            Case "SM"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SM " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SM " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

                'Ceramide
            Case "Cer-ADS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-ADS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-ADS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-AS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-AS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-AS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-BDS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-BDS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-BDS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-BS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-BS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-BS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-EODS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-EODS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-EODS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-EOS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-EOS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-EOS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-NDS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NDS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NDS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-NS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-NP"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NP " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-NP " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "HexCer-NS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-NS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-NS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "HexCer-NDS"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-NDS " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-NDS " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "Cer-AP"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-AP " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "Cer-AP " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "HexCer-AP"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-AP " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GlcCer-AP " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

            Case "SHexCer"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SHexCer " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "SHexCer " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If
            Case "GM3"
                If Not Equals(lipidinfo.Sn1AcylChain, String.Empty) AndAlso Not Equals(lipidinfo.Sn2AcylChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GM3 " & lipidinfo.Sn1AcylChain & "/" & lipidinfo.Sn2AcylChain & "&ct=c"
                ElseIf (Equals(lipidinfo.Sn1AcylChain, String.Empty) OrElse Equals(lipidinfo.Sn2AcylChain, String.Empty)) AndAlso Not Equals(lipidinfo.TotalChain, String.Empty) Then
                    Return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" & "GM3 " & lipidinfo.TotalChain & "&ct=c"
                Else
                    Return String.Empty
                End If

            Case Else
                Return String.Empty
        End Select
    End Function
End Class
