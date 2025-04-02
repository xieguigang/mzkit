# spectrumTree

Provides R language interface for mass spectrometry data processing and metabolite annotation using spectrum tree-based reference libraries.
> This module enables cross-language interoperability between VB.NET mass spectrometry algorithms and R scripting environments.
>  
>  Key features include:
>  
>  1. Reference library management (Pack/Binary/Tree formats)
>  2. Spectrum similarity searches with multiple algorithms
>  3. Metabolite annotation pipeline integration
>  4. Library compression and optimization
>  5. Test dataset generation
>  6. Embedding generation for machine learning applications
>  
>  Supported data types:
>  
>  - mzPack containers
>  - PeakMs2 spectra
>  - LibraryMatrix objects
>  - BioDeep metabolite metadata
>  
>  Search algorithms implemented:
>  
>  - Cosine similarity (dot product)
>  - Jaccard index
>  - Entropy-based scoring
>  - Forward/reverse match validation

+ [new](spectrumTree/new.1) Creates a new reference spectrum database file for storing spectral data.
+ [get_testSample](spectrumTree/get_testSample.1) Extract the test sample data for run evaluation of the annotation workflow
+ [readpack](spectrumTree/readpack.1) open the spectrum pack reference database file
+ [open](spectrumTree/open.1) ### open the spectrum reference database
+ [export_spectrum](spectrumTree/export_spectrum.1) export all reference spectrum from the given library object
+ [dotcutoff](spectrumTree/dotcutoff.1) set dot cutoff parameter for the cos score similarity algorithm
+ [parallel](spectrumTree/parallel.1) Enables or disables parallel processing for spectral searches.
+ [jaccardSet](spectrumTree/jaccardSet.1) construct a fragment set library for run spectrum search in jaccard index matches method
+ [top_candidates](spectrumTree/top_candidates.1) Retrieves the top candidate matches from a metabolite library search.
+ [candidate_ids](spectrumTree/candidate_ids.1) Extract all reference id from a set of spectrum annotation candidate result
+ [as.annotation_result](spectrumTree/as.annotation_result.1) Create metabolite annotation result dataset for a set of the spectrum annotation candidates result.
+ [query](spectrumTree/query.1) do spectrum family alignment via cos similarity
+ [addBucket](spectrumTree/addBucket.1) push the reference spectrum data into the spectrum reference tree library
+ [compress](spectrumTree/compress.1) Compresses and optimizes a spectrum library, removing redundant entries.
+ [embedding](spectrumTree/embedding.1) Generates vector embeddings for spectral data (e.g., for machine learning).
