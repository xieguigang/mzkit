#!/usr/local/bin/R#

print(as.data.frame(list.raw()));

for (file in list.raw()) {
    # str(as.list(file));
    
    print(head(TIC(file)));
}

list.raw()[1] :> TIC :> write.csv(file = `${!script$dir}/TIC.csv`);