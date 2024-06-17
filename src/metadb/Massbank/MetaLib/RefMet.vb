Namespace MetaLib

    ''' <summary>
    ''' RefMet: A Reference set of Metabolite names
    ''' </summary>
    ''' <remarks>
    ''' RefMet Naming Conventions
    ''' 
    ''' The names used in RefMet are generally based on common, officially accepted terms and 
    ''' incorporate notations which are appropriate for the type of analytical technique used.
    ''' In general, high-throughput untargeted MS experiments are not capable of deducing 
    ''' stereochemistry, double bond position/geometry and sn position (for glycerolipids/
    ''' lycerophospholipids). Secondly, the type of MS technique employed, as well as the mass
    ''' accuacy of the instrument will produce identifications at different levels of detail. 
    ''' For example, MS/MS methods are capable of identifying acyl chain substituents in lipids 
    ''' (e.g. PC 16:0/20:4) whereas MS methods only using precursor ion information might report 
    ''' these ions as "sum-composition" species (e.g. PC 36:4). RefMet covers both types of 
    ''' notations in an effort to enable data-sharing and comparative analysis of metabolomics 
    ''' data, using an analytical chemistry-centric approach.
    ''' 
    ''' The "sum-composition" lipid species indicate the number Of carbons And number Of "double
    ''' bond equivalents" (DBE), but Not chain positions Or Double bond regiochemistry And geometry.
    ''' The concept Of a Double bond equivalent unites a range Of chemical functionality which 
    ''' gives rise To isobaric features by mass spectometry. For example a chain containing a ring 
    ''' results In loss Of 2 hydrogen atoms (compared To a linear Structure) And thus has 1 DBE 
    ''' since the mass And molecular formula Is identical To a linear Structure With one Double bond.
    ''' Similarly, conversion Of a hydroxyl group To ketone results In loss Of 2 hydrogen atoms,
    ''' therefore the ketone Is assigned 1 DBE. Where applicable, the number Of oxygen atoms Is added 
    ''' To the abbreviation, separated by a semi-colon. Oxygen atoms In the Class-specific functional
    ''' group (e.g. the carboxylic acid group For fatty acids Or the phospholcholine group For PC) 
    ''' are excluded. In the Case Of sphingolipids, all oxygen atoms apart from the amide oxygen are
    ''' included, In order To discrminate, For example, between 1-deoxyceramides (;O), ceramides (;O2) 
    ''' And phytoceramides (;O3).
    ''' 
    ''' Some notes pertaining To different metabolite classes are outlined below.
    ''' </remarks>
    Public Class RefMet

        Public Property refmet_name As String
        Public Property super_class As String
        Public Property main_class As String
        Public Property sub_class As String
        Public Property formula As String
        Public Property exactmass As String
        Public Property inchi_key As String
        Public Property smiles As String
        Public Property pubchem_cid As String

        Public Overrides Function ToString() As String
            Return refmet_name
        End Function

    End Class
End Namespace