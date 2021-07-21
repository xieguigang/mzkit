imports "task" from "Mzkit_win32.Task";

# title: row scans to mzPack
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "a temp file path that its content contains selected raw data file path for each row scans"]
const input as string = ?"--files" || stop("a file list is required!");
[@info "a file path for export mzPack data file."]
const save as string  = ?"--save"  || stop("a file location for save mzPack data is required!");

sleep(1);
task::MSI_rowbind(readLines(input), save);