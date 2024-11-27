#Region "Microsoft.VisualBasic::e10ad172ad06cd6104d352844dde3df0, metadb\Massbank\Public\HERB\HerbCompoundObject.vb"

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

    '   Total Lines: 31
    '    Code Lines: 23 (74.19%)
    ' Comment Lines: 3 (9.68%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 5 (16.13%)
    '     File Size: 948 B


    '     Class HerbCompoundObject
    ' 
    '         Properties: reference
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace HERB

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class HerbCompoundObject : Inherits HERB_ingredient_info

        Public Property reference As HERB_reference_info()

        Sub New()
        End Sub

        Sub New(base As HERB_ingredient_info)
            Ingredient_id = base.Ingredient_id
            Ingredient_name = base.Ingredient_name
            [Alias] = base.Alias
            Ingredient_formula = base.Ingredient_formula
            Ingredient_Smile = base.Ingredient_Smile
            Ingredient_weight = base.Ingredient_weight
            OB_score = base.OB_score
            CAS_id = base.CAS_id
            SymMap_id = base.SymMap_id
            TCMID_id = base.TCMID_id
            TCMSP_id = base.TCMSP_id
            TCM_ID_id = base.TCM_ID_id
            PubChem_id = base.PubChem_id
            DrugBank_id = base.DrugBank_id
        End Sub

    End Class
End Namespace
