# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## geolite_city


|field|type|attributes|description|
|-----|----|----------|-----------|
|locID|Int64 (11)|``NN``||
|country|VarChar (8)|``NN``||
|region|VarChar (45)|||
|city|Text|||
|postalCode|VarChar (45)|||
|latitude|Double|||
|longitude|Double|||
|metroCode|VarChar (45)|||
|areaCode|VarChar (45)|||

```SQL
CREATE TABLE `geolite_city` (
  `locID` int(11) NOT NULL,
  `country` varchar(8) NOT NULL,
  `region` varchar(45) DEFAULT NULL,
  `city` tinytext,
  `postalCode` varchar(45) DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  `longitude` double DEFAULT NULL,
  `metroCode` varchar(45) DEFAULT NULL,
  `areaCode` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`locID`),
  UNIQUE KEY `locID_UNIQUE` (`locID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



