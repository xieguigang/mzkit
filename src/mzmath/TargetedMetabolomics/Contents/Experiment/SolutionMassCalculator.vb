Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Content

    ''' <summary>
    ''' Provides functionality to calculate the required masses of chemical reagents 
    ''' to prepare solution mixtures at specified concentrations and volumes. 
    ''' Supports both exact molecular mass and average molecular mass calculations.
    ''' </summary>
    Public Class SolutionMassCalculator

        ReadOnly Chemical_reagents As Dictionary(Of String, Double)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Chemical_reagents">
        ''' Dictionary containing chemical reagent names as keys and their molecular formulas as string values.
        ''' Formula strings must be compatible with FormulaScanner parsing methods.
        ''' </param>
        ''' <param name="useExactMass">
        ''' use the exact mass for the calculation? default false means use the average molecular mass for the calculation.
        ''' 
        ''' ## Average vs. Exact Molecular Weight in Solution Preparation
        ''' 
        ''' In the solution preparation program, the molecular weights stored in the `Chemical_reagents` dictionary
        ''' are typically **average molecular weights** (i.e., weighted averages based on the natural isotopic abundance
        ''' of elements), rather than the exact molecular weights of a single isotope. Below is a detailed analysis:
        ''' 
        ''' ### ⚖️ 1. **Definition of Average Molecular Weight vs. Exact Molecular Weight**
        ''' 
        '''    - **Average Molecular Weight**  
        '''      Calculated based on the relative atomic masses in the periodic table, these values represent the 
        '''      weighted average of the masses of all naturally occurring isotopes (e.g., the atomic mass of carbon
        '''      is 12.011, accounting for the abundance contributions of ¹²C and ¹³C).  
        '''      *Formula Example*: If a compound contains carbon (98.93% ¹²C + 1.07% ¹³C), its average molecular
        '''      weight = (12.0000 × 0.9893) + (13.0034 × 0.0107) = 12.01.
        '''      
        '''    - **Exact Molecular Weight**  
        '''      Refers to the theoretical mass of a molecule with a specific isotope composition (e.g., the exact
        '''      mass of a water molecule H₂O containing only ¹²C and ¹H is 18.0106, while its average molecular 
        '''      weight is 18.015). Such data is primarily used for high-resolution mass spectrometry analysis 
        '''      rather than routine solution preparation.
        '''      
        ''' ### 🧪 2. **Why Average Molecular Weight is Used in the Program?**
        ''' 
        '''    - **Practical Application Compatibility**:  
        '''      Solutions prepared in laboratories typically use commercial reagents, which are products of naturally 
        '''      occurring isotopic mixtures and thus conform to the definition of average molecular weight.
        '''      
        '''    - **Database Sources**:  
        '''      Common chemical databases (e.g., PubChem, ChemSpider) provide molecular weights as average molecular 
        '''      weights. For example:  
        '''      
        '''      - Sodium chloride (NaCl): 58.44 (average weight)  
        '''      - Water (H₂O): 18.015 (average weight).
        '''      
        '''    - **Calculation Consistency**:  
        '''      Concentration calculation formulas (e.g., `mass = concentration × molecular weight × volume`) rely on
        '''      average molecular weights to match literature recipes.
        '''      
        ''' ### ⚠️ 3. **Special Scenarios for Using Exact Molecular Weight**
        ''' 
        '''    If the program is used for **mass spectrometry calibration** or **isotope labeling experiments**, it may 
        '''    be necessary to switch to exact molecular weights (e.g., the exact mass of a ¹³C-labeled compound must be 
        '''    entered separately). However, such cases require additional annotations and are non-standard requirements.
        '''    
        ''' ### 📊 4. **Comparison of the Two Molecular Weights**
        ''' 
        ''' | **Characteristic**       | Average Molecular Weight                                 | Exact Molecular Weight                                    |
        ''' |--------------------------|----------------------------------------------------------|-----------------------------------------------------------|
        ''' | **Definition**           | Weighted average of isotopic abundance                   | Theoretical mass of a single isotope composition          |
        ''' | **Applicable Scenarios** | Routine solution preparation, concentration calculations | High-resolution mass spectrometry, isotope experiments    |
        ''' | **Database Sources**     | PubChem, ChemSpider, etc.                                | NIST Mass Spectral Library, specialized isotope databases |
        ''' | **Example (Water H₂O)**  | 18.015 g/mol                                             | 18.0106 g/mol (¹H₂¹⁶O)                                    |
        ''' 
        ''' ### 💡 5. **Program Implementation Suggestions**
        ''' 
        '''    - **Default to Average Molecular Weight**:  
        '''      Ensure that the data source for the `Chemical_reagents` dictionary comes from authoritative databases 
        '''      (e.g., PubChem), where all values are average molecular weights.
        '''      
        '''    - **Optional Extended Functionality**:  
        '''      If support for exact mass calculations is required, an isotope labeling field (e.g., `IsExactMass As Boolean`) 
        '''      can be added, allowing users to customize the exact mass of specific isotopes.
        '''      
        ''' ### ✅ Summary
        ''' 
        ''' In your solution preparation program, the molecular weights stored in `Chemical_reagents` should be 
        ''' **average molecular weights** to ensure compatibility with routine experimental recipes and calculation accuracy. 
        ''' Only in cases involving special requirements such as mass spectrometry or isotope experiments should the 
        ''' program switch to exact molecular weight mode.
        ''' </param>
        Sub New(Chemical_reagents As Dictionary(Of String, String), Optional useExactMass As Boolean = False)
            Dim eval As Func(Of String, Double) = MassEvaluate(useExactMass)

            Me.Chemical_reagents = Chemical_reagents _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return eval(a.Value)
                              End Function)
        End Sub

        Private Function MassEvaluate(useExactMass As Boolean) As Func(Of String, Double)
            If useExactMass Then
                Return AddressOf FormulaScanner.EvaluateExactMass
            Else
                Return AddressOf FormulaScanner.EvaluateAverageMolecularMass
            End If
        End Function

        ''' <summary>
        ''' Calculates the required masses of chemical components to prepare a target solution with specified volume and concentration.
        ''' </summary>
        ''' <param name="chemicals">Dictionary containing chemical component names as keys and their target concentrations as values</param>
        ''' <param name="VL">Target volume of solution to be prepared, expressed in milliliters (ml)</param>
        ''' <returns>An enumerable sequence of <see cref="NamedValue(Of Double)"/> structures,
        ''' where each entry contains a chemical component name and its calculated mass in grams</returns>
        ''' <exception cref="KeyNotFoundException">Thrown when a chemical component in the target dictionary 
        ''' is not found in the global Chemical_reagents database</exception>
        ''' <remarks>
        ''' <para>Concentration conversion logic:</para>
        ''' <list type="bullet">
        '''   <item>
        '''       <term>mol/L → g</term>
        '''       <description>mass = concentration × molecular weight × volume(L)</description>
        '''   </item>
        '''   <item>
        '''       <term>g/L → g</term>
        '''       <description>mass = concentration × volume(L)</description>
        '''   </item>
        ''' </list>
        ''' <para>Internal volume conversion: Automatically converts input volume from milliliters (ml) to liters (L) using VL/1000</para>
        ''' </remarks>
        Public Iterator Function CalculateSolutionMasses(chemicals As IEnumerable(Of SolutionChemical), VL As Double) As IEnumerable(Of SolutionChemical)
            For Each chem As SolutionChemical In chemicals
                If Not Chemical_reagents.ContainsKey(chem.name) Then
                    Throw New ArgumentException($"试剂 {chem.name} 的分子量未在Chemical_reagents中定义")
                End If

                Dim molecularWeight = Chemical_reagents(chem.name)
                Dim mass As Double = 0

                ' 根据浓度类型执行不同计算
                Select Case chem.type
                    Case ConcentrationType.molL  ' mol/L 摩尔浓度
                        ' 质量(g) = 浓度(mol/L) * 体积(L) * 分子量(g/mol)
                        ' VL/1000 将毫升转换为升
                        mass = chem.content * (VL / 1000) * molecularWeight

                    Case ConcentrationType.gL    ' g/L 质量浓度
                        ' 质量(g) = 浓度(g/L) * 体积(L)
                        mass = chem.content * (VL / 1000)

                    Case ConcentrationType.percentage  ' % 质量百分比浓度
                        ' 质量(g) = 浓度(%) * 目标溶液总质量 / 100
                        ' 假设密度≈1g/ml，则总质量≈VL(g)
                        mass = chem.content * VL / 100

                    Case Else
                        Throw New ArgumentException($"不支持的浓度类型: {chem.type}")
                End Select

                chem.mass = mass

                Yield chem
            Next
        End Function

        Public Shared Function ParseConcentrationType(desc As String, Optional [default] As ConcentrationType = ConcentrationType.molL) As ConcentrationType
            Static str_reps As Dictionary(Of String, ConcentrationType) = Enums(Of ConcentrationType) _
                .Select(Function(c) (c.Description, c)) _
                .JoinIterates(Enums(Of ConcentrationType).Select(Function(c) (c.ToString, c))) _
                .ToDictionary(Function(c) c.Item1.ToLower,
                              Function(c)
                                  Return c.Item2
                              End Function)

            Return str_reps.TryGetValue(Strings.Trim(desc).ToLower, [default])
        End Function
    End Class
End Namespace