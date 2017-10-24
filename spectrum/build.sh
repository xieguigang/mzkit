# Build mysql ORM model by using mysql reflector tool for VB.NET
reflector --reflects /sql "./FooDB.sql" -o "./Massbank/TMIC/FooDB/" /namespace "TMIC.FooDB" --language visualbasic /split /auto_increment.disable

# Creates the table schema document
reflector /MySQL.Markdown /sql "./FooDB.sql" > ./FooDB.dev.md 