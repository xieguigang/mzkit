# Build mysql ORM model by using mysql reflector tool for VB.NET
reflector --reflects /sql "./FooDB.sql" -o "./Massbank/Public/TMIC/FooDB/mysql/" /namespace "TMIC.FooDB.mysql" --language visualbasic /split /auto_increment.disable

# Creates the table schema document
reflector /MySQL.Markdown /sql "./FooDB.sql" > ./FooDB.dev.md 