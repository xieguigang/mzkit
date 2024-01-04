# spectrumPool

Spectrum clustering/inference via molecule networking method, 
 this api module is working with the biodeep public cloud service

+ [createPool](spectrumPool/createPool.1) create a new spectrum clustering data pool
+ [model_id](spectrumPool/model_id.1) get model id from the spectrum cluster graph model
+ [load_infer](spectrumPool/load_infer.1) Create a spectrum inference protocol workflow
+ [infer](spectrumPool/infer.1) Infer and make annotation to a specific cluster
+ [openPool](spectrumPool/openPool.1) open the spectrum pool from a given resource link
+ [closePool](spectrumPool/closePool.1) close the connection to the spectrum pool
+ [getClusterInfo](spectrumPool/getClusterInfo.1) get metadata dataframe in a given cluster tree
+ [conservedGuid](spectrumPool/conservedGuid.1) generates the guid for the spectrum with unknown annotation
+ [set_conservedGuid](spectrumPool/set_conservedGuid.1) generate and set conserved guid for each spectrum data
+ [addPool](spectrumPool/addPool.1) add sample peaks data to spectrum pool
+ [commit](spectrumPool/commit.1) commit data to the spectrum pool database
