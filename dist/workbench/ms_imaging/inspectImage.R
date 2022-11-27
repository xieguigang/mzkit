require(HDS);

HDS::openStream(`${@dir}/HR2MSI_mouse_urinary_bladder-S096.mzImage`, readonly = TRUE)
|> tree()
|> writeLines(
	con = `${@dir}/mzImage.txt`
);