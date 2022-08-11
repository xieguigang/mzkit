require(mzkit);

imports "flexImaging" from "mzkit";

`${@dir}/data.mcf_idx`
|> flexImaging::read.metadata()
|> as.list(byrow = TRUE,  names = 'metadata')
|> str()
;