#Region "Microsoft.VisualBasic::7e0b7fd2a1d749b1be11703e486bc4a3, G:/mzkit/src/metadb/Chemoinformatics//InChI/Extensions.vb"

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

    '   Total Lines: 150
    '    Code Lines: 112
    ' Comment Lines: 11
    '   Blank Lines: 27
    '     File Size: 4.59 KB


    '     Module Extensions
    ' 
    '         Function: evalSHA256, InchiKey, MakeHashCode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Security.Cryptography
Imports System.Text
Imports BioNovoGene.BioDeep.Chemoinformatics.IUPAC.InChILayers
Imports Microsoft.VisualBasic.Linq

Namespace IUPAC

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' https://github.com/mnowotka/chembl_ikey
    ''' </remarks>
    <HideModuleName>
    Public Module Extensions

        Const INCHI_STRING_PREFIX = "InChI="

#Disable Warning
        ReadOnly LEN_INCHI_STRING_PREFIX As Integer = Len(INCHI_STRING_PREFIX)
        ReadOnly hash As New SHA256Managed()
#Enable Warning

        ''' <summary>
        ''' function for generates the inchikey based on the inchi string data
        ''' </summary>
        ''' <param name="inchi"></param>
        ''' <returns></returns>
        Public Function MakeHashCode(inchi As String) As InChIKey
            If inchi.StringEmpty Then
                Return Nothing
            End If

            Dim slen = inchi.Length

            If slen < LEN_INCHI_STRING_PREFIX + 3 Then
                Return Nothing
            End If

            If Not inchi.StartsWith(INCHI_STRING_PREFIX) Then
                Return Nothing
            End If

            Return InchiKey(inchi)
        End Function

        Private Function InchiKey(inchi As String) As InChIKey
            Dim bStdFormat As Integer
            Dim pos_slash1 = LEN_INCHI_STRING_PREFIX + 1
            Dim flagstd = "S"
            Dim flagnonstd = "N"
            Dim flagver = "A"
            Dim flagproto = "N"
            Dim pplus = "OPQRSTUVWXYZ"
            Dim pminus = "MLKJIHGFEDCB"

            If inchi(pos_slash1) = "S"c Then
                bStdFormat = 1
                pos_slash1 += 1
            End If
            If inchi(pos_slash1) <> "/"c Then
                Return Nothing
            End If
            If Not Char.IsLetterOrDigit(inchi(pos_slash1 + 1)) AndAlso inchi(pos_slash1 + 1) <> "/"c Then
                Return Nothing
            End If

            Dim str As String = inchi.Substring(LEN_INCHI_STRING_PREFIX)

            If str.StringEmpty Then
                Return Nothing
            End If

            Dim aux = str.Substring(pos_slash1 - LEN_INCHI_STRING_PREFIX + 1)
            Dim slen = aux.Length
            Dim proto As Integer
            Dim [end] = 0

            For Each ch In aux.SeqIterator
                If ch = "/"c Then
                    Dim cn = aux(ch + 1)

                    If cn = "c"c OrElse cn = "h"c OrElse cn = "q"c Then
                        Continue For
                    End If
                    If cn = "p"c Then
                        proto = ch
                        Continue For
                    End If
                    If cn = "f"c OrElse cn = "r"c Then
                        Return Nothing
                    End If
                    [end] = ch
                    Exit For
                End If
            Next

            If [end] = slen - 1 Then
                [end] += 1
            End If

            Dim smajor As String

            If proto = 0 Then
                smajor = aux.Substring(0, [end])
            Else
                smajor = aux.Substring(0, proto)
            End If

            If proto <> 0 Then
                Dim nprotons As Integer = aux.Substring(proto + 2, [end])

                If nprotons > 0 Then
                    If nprotons > 12 Then
                        flagproto = "A"
                    Else
                        flagproto = pplus(nprotons - 1)
                    End If
                ElseIf nprotons < 0 Then
                    If nprotons < -12 Then
                        flagproto = "A"
                    Else
                        flagproto = pminus(-nprotons - 1)
                    End If
                Else
                    Return Nothing
                End If
            End If

            Dim sminor = ""

            If [end] <> slen Then
                sminor = aux.Substring([end])
            End If
            If Len(sminor) < 255 Then
                sminor += sminor
            End If

            Dim flag = If(bStdFormat, flagstd, flagnonstd)
            Dim digest_major = evalSHA256(smajor)
            Dim digest_minor = evalSHA256(sminor)

        End Function

        Private Function evalSHA256(str As String) As String
            Dim digest = hash.ComputeHash(Encoding.UTF8.GetBytes(str))

        End Function
    End Module
End Namespace
