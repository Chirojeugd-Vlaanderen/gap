CREATE PROCEDURE [Insert_PostNrNaarGemeente]
@NisGemeenteCode decimal(5,0),
@PostNr int
AS
INSERT into [adr].[PostNrNaarGemeente]
(GemeenteID, PostNr)
select GemeenteID, @PostNr
FROM Adr.Gemeente
WHERE (NisGemeenteCode = @NisGemeenteCode)
GO 

EXEC Insert_PostNrNaarGemeente '11001', '2630'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2000'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2018'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2020'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2030'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2040'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2050'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2060'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2100'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2140'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2170'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2180'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2600'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2610'; 
GO
EXEC Insert_PostNrNaarGemeente '11002', '2660'; 
GO
EXEC Insert_PostNrNaarGemeente '11004', '2530'; 
GO
EXEC Insert_PostNrNaarGemeente '11004', '2531'; 
GO
EXEC Insert_PostNrNaarGemeente '11005', '2850'; 
GO
EXEC Insert_PostNrNaarGemeente '11007', '2150'; 
GO
EXEC Insert_PostNrNaarGemeente '11008', '2930'; 
GO
EXEC Insert_PostNrNaarGemeente '11009', '2960'; 
GO
EXEC Insert_PostNrNaarGemeente '11013', '2650'; 
GO
EXEC Insert_PostNrNaarGemeente '11016', '2910'; 
GO
EXEC Insert_PostNrNaarGemeente '11018', '2620'; 
GO
EXEC Insert_PostNrNaarGemeente '11021', '2540'; 
GO
EXEC Insert_PostNrNaarGemeente '11022', '2920'; 
GO
EXEC Insert_PostNrNaarGemeente '11023', '2950'; 
GO
EXEC Insert_PostNrNaarGemeente '11024', '2550'; 
GO
EXEC Insert_PostNrNaarGemeente '11025', '2547'; 
GO
EXEC Insert_PostNrNaarGemeente '11029', '2640'; 
GO
EXEC Insert_PostNrNaarGemeente '11030', '2845'; 
GO
EXEC Insert_PostNrNaarGemeente '11035', '2520'; 
GO
EXEC Insert_PostNrNaarGemeente '11037', '2840'; 
GO
EXEC Insert_PostNrNaarGemeente '11038', '2627'; 
GO
EXEC Insert_PostNrNaarGemeente '11039', '2970'; 
GO
EXEC Insert_PostNrNaarGemeente '11040', '2900'; 
GO
EXEC Insert_PostNrNaarGemeente '11044', '2940'; 
GO
EXEC Insert_PostNrNaarGemeente '11050', '2110'; 
GO
EXEC Insert_PostNrNaarGemeente '11052', '2160'; 
GO
EXEC Insert_PostNrNaarGemeente '11053', '2990'; 
GO
EXEC Insert_PostNrNaarGemeente '11054', '2240'; 
GO
EXEC Insert_PostNrNaarGemeente '11054', '2242'; 
GO
EXEC Insert_PostNrNaarGemeente '11054', '2243'; 
GO
EXEC Insert_PostNrNaarGemeente '11055', '2980'; 
GO
EXEC Insert_PostNrNaarGemeente '11056', '2070'; 
GO
EXEC Insert_PostNrNaarGemeente '11057', '2390'; 
GO
EXEC Insert_PostNrNaarGemeente '12002', '2590'; 
GO
EXEC Insert_PostNrNaarGemeente '12005', '2820'; 
GO
EXEC Insert_PostNrNaarGemeente '12007', '2880'; 
GO
EXEC Insert_PostNrNaarGemeente '12009', '2570'; 
GO
EXEC Insert_PostNrNaarGemeente '12014', '2220'; 
GO
EXEC Insert_PostNrNaarGemeente '12014', '2221'; 
GO
EXEC Insert_PostNrNaarGemeente '12014', '2222'; 
GO
EXEC Insert_PostNrNaarGemeente '12014', '2223'; 
GO
EXEC Insert_PostNrNaarGemeente '12021', '2500'; 
GO
EXEC Insert_PostNrNaarGemeente '12025', '2800'; 
GO
EXEC Insert_PostNrNaarGemeente '12025', '2801'; 
GO
EXEC Insert_PostNrNaarGemeente '12025', '2811'; 
GO
EXEC Insert_PostNrNaarGemeente '12025', '2812'; 
GO
EXEC Insert_PostNrNaarGemeente '12026', '2560'; 
GO
EXEC Insert_PostNrNaarGemeente '12029', '2580'; 
GO
EXEC Insert_PostNrNaarGemeente '12030', '2870'; 
GO
EXEC Insert_PostNrNaarGemeente '12034', '2890'; 
GO
EXEC Insert_PostNrNaarGemeente '12035', '2860'; 
GO
EXEC Insert_PostNrNaarGemeente '12035', '2861'; 
GO
EXEC Insert_PostNrNaarGemeente '12040', '2830'; 
GO
EXEC Insert_PostNrNaarGemeente '13001', '2370'; 
GO
EXEC Insert_PostNrNaarGemeente '13002', '2387'; 
GO
EXEC Insert_PostNrNaarGemeente '13003', '2490'; 
GO
EXEC Insert_PostNrNaarGemeente '13003', '2491'; 
GO
EXEC Insert_PostNrNaarGemeente '13004', '2340'; 
GO
EXEC Insert_PostNrNaarGemeente '13006', '2480'; 
GO
EXEC Insert_PostNrNaarGemeente '13008', '2440'; 
GO
EXEC Insert_PostNrNaarGemeente '13010', '2280'; 
GO
EXEC Insert_PostNrNaarGemeente '13010', '2288'; 
GO
EXEC Insert_PostNrNaarGemeente '13011', '2200'; 
GO
EXEC Insert_PostNrNaarGemeente '13012', '2270'; 
GO
EXEC Insert_PostNrNaarGemeente '13013', '2230'; 
GO
EXEC Insert_PostNrNaarGemeente '13014', '2320'; 
GO
EXEC Insert_PostNrNaarGemeente '13014', '2321'; 
GO
EXEC Insert_PostNrNaarGemeente '13014', '2322'; 
GO
EXEC Insert_PostNrNaarGemeente '13014', '2323'; 
GO
EXEC Insert_PostNrNaarGemeente '13014', '2328'; 
GO
EXEC Insert_PostNrNaarGemeente '13016', '2235'; 
GO
EXEC Insert_PostNrNaarGemeente '13017', '2460'; 
GO
EXEC Insert_PostNrNaarGemeente '13019', '2275'; 
GO
EXEC Insert_PostNrNaarGemeente '13021', '2450'; 
GO
EXEC Insert_PostNrNaarGemeente '13023', '2330'; 
GO
EXEC Insert_PostNrNaarGemeente '13025', '2400'; 
GO
EXEC Insert_PostNrNaarGemeente '13029', '2250'; 
GO
EXEC Insert_PostNrNaarGemeente '13031', '2360'; 
GO
EXEC Insert_PostNrNaarGemeente '13035', '2380'; 
GO
EXEC Insert_PostNrNaarGemeente '13035', '2381'; 
GO
EXEC Insert_PostNrNaarGemeente '13035', '2382'; 
GO
EXEC Insert_PostNrNaarGemeente '13036', '2470'; 
GO
EXEC Insert_PostNrNaarGemeente '13037', '2310'; 
GO
EXEC Insert_PostNrNaarGemeente '13040', '2300'; 
GO
EXEC Insert_PostNrNaarGemeente '13044', '2290'; 
GO
EXEC Insert_PostNrNaarGemeente '13046', '2350'; 
GO
EXEC Insert_PostNrNaarGemeente '13049', '2260'; 
GO
EXEC Insert_PostNrNaarGemeente '13053', '2430'; 
GO
EXEC Insert_PostNrNaarGemeente '13053', '2431'; 
GO
EXEC Insert_PostNrNaarGemeente '21001', '1070'; 
GO
EXEC Insert_PostNrNaarGemeente '21002', '1160'; 
GO
EXEC Insert_PostNrNaarGemeente '21003', '1082'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1000'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1020'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1030'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1040'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1050'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1070'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1120'; 
GO
EXEC Insert_PostNrNaarGemeente '21004', '1130'; 
GO
EXEC Insert_PostNrNaarGemeente '21005', '1040'; 
GO
EXEC Insert_PostNrNaarGemeente '21006', '1140'; 
GO
EXEC Insert_PostNrNaarGemeente '21007', '1190'; 
GO
EXEC Insert_PostNrNaarGemeente '21008', '1083'; 
GO
EXEC Insert_PostNrNaarGemeente '21009', '1050'; 
GO
EXEC Insert_PostNrNaarGemeente '21010', '1090'; 
GO
EXEC Insert_PostNrNaarGemeente '21011', '1081'; 
GO
EXEC Insert_PostNrNaarGemeente '21012', '1080'; 
GO
EXEC Insert_PostNrNaarGemeente '21013', '1060'; 
GO
EXEC Insert_PostNrNaarGemeente '21014', '1210'; 
GO
EXEC Insert_PostNrNaarGemeente '21015', '1030'; 
GO
EXEC Insert_PostNrNaarGemeente '21016', '1180'; 
GO
EXEC Insert_PostNrNaarGemeente '21017', '1170'; 
GO
EXEC Insert_PostNrNaarGemeente '21018', '1200'; 
GO
EXEC Insert_PostNrNaarGemeente '21019', '1150'; 
GO
EXEC Insert_PostNrNaarGemeente '23002', '1730'; 
GO
EXEC Insert_PostNrNaarGemeente '23002', '1731'; 
GO
EXEC Insert_PostNrNaarGemeente '23003', '1650'; 
GO
EXEC Insert_PostNrNaarGemeente '23003', '1651'; 
GO
EXEC Insert_PostNrNaarGemeente '23003', '1652'; 
GO
EXEC Insert_PostNrNaarGemeente '23003', '1653'; 
GO
EXEC Insert_PostNrNaarGemeente '23003', '1654'; 
GO
EXEC Insert_PostNrNaarGemeente '23009', '1547'; 
GO
EXEC Insert_PostNrNaarGemeente '23016', '1700'; 
GO
EXEC Insert_PostNrNaarGemeente '23016', '1701'; 
GO
EXEC Insert_PostNrNaarGemeente '23016', '1702'; 
GO
EXEC Insert_PostNrNaarGemeente '23016', '1703'; 
GO
EXEC Insert_PostNrNaarGemeente '23023', '1570'; 
GO
EXEC Insert_PostNrNaarGemeente '23024', '1755'; 
GO
EXEC Insert_PostNrNaarGemeente '23025', '1850'; 
GO
EXEC Insert_PostNrNaarGemeente '23025', '1851'; 
GO
EXEC Insert_PostNrNaarGemeente '23025', '1852'; 
GO
EXEC Insert_PostNrNaarGemeente '23025', '1853'; 
GO
EXEC Insert_PostNrNaarGemeente '23027', '1500'; 
GO
EXEC Insert_PostNrNaarGemeente '23027', '1501'; 
GO
EXEC Insert_PostNrNaarGemeente '23027', '1502'; 
GO
EXEC Insert_PostNrNaarGemeente '23032', '1540'; 
GO
EXEC Insert_PostNrNaarGemeente '23032', '1541'; 
GO
EXEC Insert_PostNrNaarGemeente '23033', '1560'; 
GO
EXEC Insert_PostNrNaarGemeente '23038', '1910'; 
GO
EXEC Insert_PostNrNaarGemeente '23039', '1880'; 
GO
EXEC Insert_PostNrNaarGemeente '23044', '1770'; 
GO
EXEC Insert_PostNrNaarGemeente '23045', '1840'; 
GO
EXEC Insert_PostNrNaarGemeente '23047', '1830'; 
GO
EXEC Insert_PostNrNaarGemeente '23047', '1831'; 
GO
EXEC Insert_PostNrNaarGemeente '23050', '1860'; 
GO
EXEC Insert_PostNrNaarGemeente '23050', '1861'; 
GO
EXEC Insert_PostNrNaarGemeente '23052', '1785'; 
GO
EXEC Insert_PostNrNaarGemeente '23060', '1745'; 
GO
EXEC Insert_PostNrNaarGemeente '23062', '3090'; 
GO
EXEC Insert_PostNrNaarGemeente '23064', '1670'; 
GO
EXEC Insert_PostNrNaarGemeente '23064', '1671'; 
GO
EXEC Insert_PostNrNaarGemeente '23064', '1673'; 
GO
EXEC Insert_PostNrNaarGemeente '23064', '1674'; 
GO
EXEC Insert_PostNrNaarGemeente '23077', '1600'; 
GO
EXEC Insert_PostNrNaarGemeente '23077', '1601'; 
GO
EXEC Insert_PostNrNaarGemeente '23077', '1602'; 
GO
EXEC Insert_PostNrNaarGemeente '23081', '1820'; 
GO
EXEC Insert_PostNrNaarGemeente '23086', '1740'; 
GO
EXEC Insert_PostNrNaarGemeente '23086', '1741'; 
GO
EXEC Insert_PostNrNaarGemeente '23086', '1742'; 
GO
EXEC Insert_PostNrNaarGemeente '23088', '1800'; 
GO
EXEC Insert_PostNrNaarGemeente '23094', '1930'; 
GO
EXEC Insert_PostNrNaarGemeente '23094', '1932'; 
GO
EXEC Insert_PostNrNaarGemeente '23094', '1933'; 
GO
EXEC Insert_PostNrNaarGemeente '23096', '1980'; 
GO
EXEC Insert_PostNrNaarGemeente '23096', '1981'; 
GO
EXEC Insert_PostNrNaarGemeente '23096', '1982'; 
GO
EXEC Insert_PostNrNaarGemeente '23097', '1760'; 
GO
EXEC Insert_PostNrNaarGemeente '23097', '1761'; 
GO
EXEC Insert_PostNrNaarGemeente '23098', '1620'; 
GO
EXEC Insert_PostNrNaarGemeente '23099', '1950'; 
GO
EXEC Insert_PostNrNaarGemeente '23100', '1630'; 
GO
EXEC Insert_PostNrNaarGemeente '23101', '1640'; 
GO
EXEC Insert_PostNrNaarGemeente '23102', '1780'; 
GO
EXEC Insert_PostNrNaarGemeente '23103', '1970'; 
GO
EXEC Insert_PostNrNaarGemeente '23104', '1750'; 
GO
EXEC Insert_PostNrNaarGemeente '23105', '1790'; 
GO
EXEC Insert_PostNrNaarGemeente '24001', '3200'; 
GO
EXEC Insert_PostNrNaarGemeente '24001', '3201'; 
GO
EXEC Insert_PostNrNaarGemeente '24001', '3202'; 
GO
EXEC Insert_PostNrNaarGemeente '24007', '3130'; 
GO
EXEC Insert_PostNrNaarGemeente '24008', '3460'; 
GO
EXEC Insert_PostNrNaarGemeente '24008', '3461'; 
GO
EXEC Insert_PostNrNaarGemeente '24009', '3060'; 
GO
EXEC Insert_PostNrNaarGemeente '24009', '3061'; 
GO
EXEC Insert_PostNrNaarGemeente '24011', '3360'; 
GO
EXEC Insert_PostNrNaarGemeente '24014', '3190'; 
GO
EXEC Insert_PostNrNaarGemeente '24014', '3191'; 
GO
EXEC Insert_PostNrNaarGemeente '24016', '3370'; 
GO
EXEC Insert_PostNrNaarGemeente '24020', '3290'; 
GO
EXEC Insert_PostNrNaarGemeente '24020', '3293'; 
GO
EXEC Insert_PostNrNaarGemeente '24020', '3294'; 
GO
EXEC Insert_PostNrNaarGemeente '24028', '3450'; 
GO
EXEC Insert_PostNrNaarGemeente '24028', '3454'; 
GO
EXEC Insert_PostNrNaarGemeente '24033', '3150'; 
GO
EXEC Insert_PostNrNaarGemeente '24038', '3020'; 
GO
EXEC Insert_PostNrNaarGemeente '24041', '3320'; 
GO
EXEC Insert_PostNrNaarGemeente '24041', '3321'; 
GO
EXEC Insert_PostNrNaarGemeente '24043', '3220'; 
GO
EXEC Insert_PostNrNaarGemeente '24043', '3221'; 
GO
EXEC Insert_PostNrNaarGemeente '24045', '3040'; 
GO
EXEC Insert_PostNrNaarGemeente '24048', '3140'; 
GO
EXEC Insert_PostNrNaarGemeente '24054', '3470'; 
GO
EXEC Insert_PostNrNaarGemeente '24054', '3471'; 
GO
EXEC Insert_PostNrNaarGemeente '24054', '3472'; 
GO
EXEC Insert_PostNrNaarGemeente '24054', '3473'; 
GO
EXEC Insert_PostNrNaarGemeente '24055', '3070'; 
GO
EXEC Insert_PostNrNaarGemeente '24055', '3071'; 
GO
EXEC Insert_PostNrNaarGemeente '24055', '3078'; 
GO
EXEC Insert_PostNrNaarGemeente '24059', '3400'; 
GO
EXEC Insert_PostNrNaarGemeente '24059', '3401'; 
GO
EXEC Insert_PostNrNaarGemeente '24059', '3404'; 
GO
EXEC Insert_PostNrNaarGemeente '24062', '3000'; 
GO
EXEC Insert_PostNrNaarGemeente '24062', '3001'; 
GO
EXEC Insert_PostNrNaarGemeente '24062', '3010'; 
GO
EXEC Insert_PostNrNaarGemeente '24062', '3012'; 
GO
EXEC Insert_PostNrNaarGemeente '24062', '3018'; 
GO
EXEC Insert_PostNrNaarGemeente '24066', '3210'; 
GO
EXEC Insert_PostNrNaarGemeente '24066', '3211'; 
GO
EXEC Insert_PostNrNaarGemeente '24066', '3212'; 
GO
EXEC Insert_PostNrNaarGemeente '24086', '3050'; 
GO
EXEC Insert_PostNrNaarGemeente '24086', '3051'; 
GO
EXEC Insert_PostNrNaarGemeente '24086', '3052'; 
GO
EXEC Insert_PostNrNaarGemeente '24086', '3053'; 
GO
EXEC Insert_PostNrNaarGemeente '24086', '3054'; 
GO
EXEC Insert_PostNrNaarGemeente '24094', '3110'; 
GO
EXEC Insert_PostNrNaarGemeente '24094', '3111'; 
GO
EXEC Insert_PostNrNaarGemeente '24094', '3118'; 
GO
EXEC Insert_PostNrNaarGemeente '24104', '3080'; 
GO
EXEC Insert_PostNrNaarGemeente '24107', '3300'; 
GO
EXEC Insert_PostNrNaarGemeente '24109', '3120'; 
GO
EXEC Insert_PostNrNaarGemeente '24109', '3128'; 
GO
EXEC Insert_PostNrNaarGemeente '24130', '3440'; 
GO
EXEC Insert_PostNrNaarGemeente '24133', '3350'; 
GO
EXEC Insert_PostNrNaarGemeente '24134', '3270'; 
GO
EXEC Insert_PostNrNaarGemeente '24134', '3271'; 
GO
EXEC Insert_PostNrNaarGemeente '24134', '3272'; 
GO
EXEC Insert_PostNrNaarGemeente '24135', '3390'; 
GO
EXEC Insert_PostNrNaarGemeente '24135', '3391'; 
GO
EXEC Insert_PostNrNaarGemeente '24137', '3380'; 
GO
EXEC Insert_PostNrNaarGemeente '24137', '3381'; 
GO
EXEC Insert_PostNrNaarGemeente '24137', '3384'; 
GO
EXEC Insert_PostNrNaarGemeente '25005', '1320'; 
GO
EXEC Insert_PostNrNaarGemeente '25014', '1420'; 
GO
EXEC Insert_PostNrNaarGemeente '25014', '1421'; 
GO
EXEC Insert_PostNrNaarGemeente '25014', '1428'; 
GO
EXEC Insert_PostNrNaarGemeente '25015', '1440'; 
GO
EXEC Insert_PostNrNaarGemeente '25018', '1325'; 
GO
EXEC Insert_PostNrNaarGemeente '25023', '1490'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1470'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1471'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1472'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1473'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1474'; 
GO
EXEC Insert_PostNrNaarGemeente '25031', '1476'; 
GO
EXEC Insert_PostNrNaarGemeente '25037', '1390'; 
GO
EXEC Insert_PostNrNaarGemeente '25043', '1315'; 
GO
EXEC Insert_PostNrNaarGemeente '25044', '1460'; 
GO
EXEC Insert_PostNrNaarGemeente '25044', '1461'; 
GO
EXEC Insert_PostNrNaarGemeente '25048', '1370'; 
GO
EXEC Insert_PostNrNaarGemeente '25050', '1310'; 
GO
EXEC Insert_PostNrNaarGemeente '25068', '1435'; 
GO
EXEC Insert_PostNrNaarGemeente '25072', '1400'; 
GO
EXEC Insert_PostNrNaarGemeente '25072', '1401'; 
GO
EXEC Insert_PostNrNaarGemeente '25072', '1402'; 
GO
EXEC Insert_PostNrNaarGemeente '25072', '1404'; 
GO
EXEC Insert_PostNrNaarGemeente '25084', '1360'; 
GO
EXEC Insert_PostNrNaarGemeente '25091', '1330'; 
GO
EXEC Insert_PostNrNaarGemeente '25091', '1331'; 
GO
EXEC Insert_PostNrNaarGemeente '25091', '1332'; 
GO
EXEC Insert_PostNrNaarGemeente '25105', '1480'; 
GO
EXEC Insert_PostNrNaarGemeente '25107', '1495'; 
GO
EXEC Insert_PostNrNaarGemeente '25110', '1410'; 
GO
EXEC Insert_PostNrNaarGemeente '25112', '1300'; 
GO
EXEC Insert_PostNrNaarGemeente '25112', '1301'; 
GO
EXEC Insert_PostNrNaarGemeente '25117', '1450'; 
GO
EXEC Insert_PostNrNaarGemeente '25118', '1357'; 
GO
EXEC Insert_PostNrNaarGemeente '25119', '1380'; 
GO
EXEC Insert_PostNrNaarGemeente '25120', '1350'; 
GO
EXEC Insert_PostNrNaarGemeente '25121', '1340'; 
GO
EXEC Insert_PostNrNaarGemeente '25121', '1341'; 
GO
EXEC Insert_PostNrNaarGemeente '25121', '1342'; 
GO
EXEC Insert_PostNrNaarGemeente '25121', '1348'; 
GO
EXEC Insert_PostNrNaarGemeente '25121', '1400'; 
GO
EXEC Insert_PostNrNaarGemeente '25122', '1367'; 
GO
EXEC Insert_PostNrNaarGemeente '25123', '1430'; 
GO
EXEC Insert_PostNrNaarGemeente '25124', '1457'; 
GO
EXEC Insert_PostNrNaarGemeente '31003', '8730'; 
GO
EXEC Insert_PostNrNaarGemeente '31004', '8370'; 
GO
EXEC Insert_PostNrNaarGemeente '31005', '8000'; 
GO
EXEC Insert_PostNrNaarGemeente '31005', '8200'; 
GO
EXEC Insert_PostNrNaarGemeente '31005', '8310'; 
GO
EXEC Insert_PostNrNaarGemeente '31005', '8380'; 
GO
EXEC Insert_PostNrNaarGemeente '31006', '8340'; 
GO
EXEC Insert_PostNrNaarGemeente '31012', '8490'; 
GO
EXEC Insert_PostNrNaarGemeente '31022', '8020'; 
GO
EXEC Insert_PostNrNaarGemeente '31033', '8820'; 
GO
EXEC Insert_PostNrNaarGemeente '31040', '8210'; 
GO
EXEC Insert_PostNrNaarGemeente '31040', '8211'; 
GO
EXEC Insert_PostNrNaarGemeente '31042', '8377'; 
GO
EXEC Insert_PostNrNaarGemeente '31043', '8300'; 
GO
EXEC Insert_PostNrNaarGemeente '31043', '8301'; 
GO
EXEC Insert_PostNrNaarGemeente '32003', '8600'; 
GO
EXEC Insert_PostNrNaarGemeente '32006', '8650'; 
GO
EXEC Insert_PostNrNaarGemeente '32010', '8680'; 
GO
EXEC Insert_PostNrNaarGemeente '32011', '8610'; 
GO
EXEC Insert_PostNrNaarGemeente '32030', '8647'; 
GO
EXEC Insert_PostNrNaarGemeente '33011', '8900'; 
GO
EXEC Insert_PostNrNaarGemeente '33011', '8902'; 
GO
EXEC Insert_PostNrNaarGemeente '33011', '8904'; 
GO
EXEC Insert_PostNrNaarGemeente '33011', '8906'; 
GO
EXEC Insert_PostNrNaarGemeente '33011', '8908'; 
GO
EXEC Insert_PostNrNaarGemeente '33016', '8957'; 
GO
EXEC Insert_PostNrNaarGemeente '33021', '8970'; 
GO
EXEC Insert_PostNrNaarGemeente '33021', '8972'; 
GO
EXEC Insert_PostNrNaarGemeente '33021', '8978'; 
GO
EXEC Insert_PostNrNaarGemeente '33029', '8940'; 
GO
EXEC Insert_PostNrNaarGemeente '33037', '8980'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8950'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8951'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8952'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8953'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8954'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8956'; 
GO
EXEC Insert_PostNrNaarGemeente '33039', '8958'; 
GO
EXEC Insert_PostNrNaarGemeente '33040', '8920'; 
GO
EXEC Insert_PostNrNaarGemeente '33041', '8640'; 
GO
EXEC Insert_PostNrNaarGemeente '34002', '8570'; 
GO
EXEC Insert_PostNrNaarGemeente '34002', '8572'; 
GO
EXEC Insert_PostNrNaarGemeente '34002', '8573'; 
GO
EXEC Insert_PostNrNaarGemeente '34003', '8580'; 
GO
EXEC Insert_PostNrNaarGemeente '34003', '8581'; 
GO
EXEC Insert_PostNrNaarGemeente '34003', '8582'; 
GO
EXEC Insert_PostNrNaarGemeente '34003', '8583'; 
GO
EXEC Insert_PostNrNaarGemeente '34009', '8540'; 
GO
EXEC Insert_PostNrNaarGemeente '34013', '8530'; 
GO
EXEC Insert_PostNrNaarGemeente '34013', '8531'; 
GO
EXEC Insert_PostNrNaarGemeente '34022', '8500'; 
GO
EXEC Insert_PostNrNaarGemeente '34022', '8501'; 
GO
EXEC Insert_PostNrNaarGemeente '34022', '8510'; 
GO
EXEC Insert_PostNrNaarGemeente '34022', '8511'; 
GO
EXEC Insert_PostNrNaarGemeente '34023', '8520'; 
GO
EXEC Insert_PostNrNaarGemeente '34025', '8860'; 
GO
EXEC Insert_PostNrNaarGemeente '34027', '8930'; 
GO
EXEC Insert_PostNrNaarGemeente '34040', '8790'; 
GO
EXEC Insert_PostNrNaarGemeente '34040', '8791'; 
GO
EXEC Insert_PostNrNaarGemeente '34040', '8792'; 
GO
EXEC Insert_PostNrNaarGemeente '34040', '8793'; 
GO
EXEC Insert_PostNrNaarGemeente '34041', '8560'; 
GO
EXEC Insert_PostNrNaarGemeente '34042', '8550'; 
GO
EXEC Insert_PostNrNaarGemeente '34042', '8551'; 
GO
EXEC Insert_PostNrNaarGemeente '34042', '8552'; 
GO
EXEC Insert_PostNrNaarGemeente '34042', '8553'; 
GO
EXEC Insert_PostNrNaarGemeente '34042', '8554'; 
GO
EXEC Insert_PostNrNaarGemeente '34043', '8587'; 
GO
EXEC Insert_PostNrNaarGemeente '35002', '8450'; 
GO
EXEC Insert_PostNrNaarGemeente '35005', '8470'; 
GO
EXEC Insert_PostNrNaarGemeente '35006', '8480'; 
GO
EXEC Insert_PostNrNaarGemeente '35011', '8430'; 
GO
EXEC Insert_PostNrNaarGemeente '35011', '8431'; 
GO
EXEC Insert_PostNrNaarGemeente '35011', '8432'; 
GO
EXEC Insert_PostNrNaarGemeente '35011', '8433'; 
GO
EXEC Insert_PostNrNaarGemeente '35011', '8434'; 
GO
EXEC Insert_PostNrNaarGemeente '35013', '8400'; 
GO
EXEC Insert_PostNrNaarGemeente '35014', '8460'; 
GO
EXEC Insert_PostNrNaarGemeente '35029', '8420'; 
GO
EXEC Insert_PostNrNaarGemeente '35029', '8421'; 
GO
EXEC Insert_PostNrNaarGemeente '36006', '8830'; 
GO
EXEC Insert_PostNrNaarGemeente '36007', '8770'; 
GO
EXEC Insert_PostNrNaarGemeente '36008', '8870'; 
GO
EXEC Insert_PostNrNaarGemeente '36010', '8880'; 
GO
EXEC Insert_PostNrNaarGemeente '36011', '8810'; 
GO
EXEC Insert_PostNrNaarGemeente '36012', '8890'; 
GO
EXEC Insert_PostNrNaarGemeente '36015', '8800'; 
GO
EXEC Insert_PostNrNaarGemeente '36019', '8840'; 
GO
EXEC Insert_PostNrNaarGemeente '37002', '8720'; 
GO
EXEC Insert_PostNrNaarGemeente '37007', '8760'; 
GO
EXEC Insert_PostNrNaarGemeente '37010', '8780'; 
GO
EXEC Insert_PostNrNaarGemeente '37011', '8740'; 
GO
EXEC Insert_PostNrNaarGemeente '37012', '8755'; 
GO
EXEC Insert_PostNrNaarGemeente '37015', '8700'; 
GO
EXEC Insert_PostNrNaarGemeente '37017', '8710'; 
GO
EXEC Insert_PostNrNaarGemeente '37018', '8750'; 
GO
EXEC Insert_PostNrNaarGemeente '37020', '8850'; 
GO
EXEC Insert_PostNrNaarGemeente '37020', '8851'; 
GO
EXEC Insert_PostNrNaarGemeente '38002', '8690'; 
GO
EXEC Insert_PostNrNaarGemeente '38002', '8691'; 
GO
EXEC Insert_PostNrNaarGemeente '38008', '8660'; 
GO
EXEC Insert_PostNrNaarGemeente '38014', '8670'; 
GO
EXEC Insert_PostNrNaarGemeente '38016', '8620'; 
GO
EXEC Insert_PostNrNaarGemeente '38025', '8630'; 
GO
EXEC Insert_PostNrNaarGemeente '41002', '9300'; 
GO
EXEC Insert_PostNrNaarGemeente '41002', '9308'; 
GO
EXEC Insert_PostNrNaarGemeente '41002', '9310'; 
GO
EXEC Insert_PostNrNaarGemeente '41002', '9320'; 
GO
EXEC Insert_PostNrNaarGemeente '41011', '9470'; 
GO
EXEC Insert_PostNrNaarGemeente '41011', '9472'; 
GO
EXEC Insert_PostNrNaarGemeente '41011', '9473'; 
GO
EXEC Insert_PostNrNaarGemeente '41018', '9500'; 
GO
EXEC Insert_PostNrNaarGemeente '41018', '9506'; 
GO
EXEC Insert_PostNrNaarGemeente '41024', '9450'; 
GO
EXEC Insert_PostNrNaarGemeente '41024', '9451'; 
GO
EXEC Insert_PostNrNaarGemeente '41027', '9550'; 
GO
EXEC Insert_PostNrNaarGemeente '41027', '9551'; 
GO
EXEC Insert_PostNrNaarGemeente '41027', '9552'; 
GO
EXEC Insert_PostNrNaarGemeente '41034', '9340'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9400'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9401'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9402'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9403'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9404'; 
GO
EXEC Insert_PostNrNaarGemeente '41048', '9406'; 
GO
EXEC Insert_PostNrNaarGemeente '41063', '9520'; 
GO
EXEC Insert_PostNrNaarGemeente '41063', '9521'; 
GO
EXEC Insert_PostNrNaarGemeente '41081', '9620'; 
GO
EXEC Insert_PostNrNaarGemeente '41082', '9420'; 
GO
EXEC Insert_PostNrNaarGemeente '42003', '9290'; 
GO
EXEC Insert_PostNrNaarGemeente '42004', '9255'; 
GO
EXEC Insert_PostNrNaarGemeente '42006', '9200'; 
GO
EXEC Insert_PostNrNaarGemeente '42008', '9220'; 
GO
EXEC Insert_PostNrNaarGemeente '42010', '9270'; 
GO
EXEC Insert_PostNrNaarGemeente '42011', '9280'; 
GO
EXEC Insert_PostNrNaarGemeente '42023', '9250'; 
GO
EXEC Insert_PostNrNaarGemeente '42025', '9230'; 
GO
EXEC Insert_PostNrNaarGemeente '42026', '9260'; 
GO
EXEC Insert_PostNrNaarGemeente '42028', '9240'; 
GO
EXEC Insert_PostNrNaarGemeente '43002', '9960'; 
GO
EXEC Insert_PostNrNaarGemeente '43002', '9961'; 
GO
EXEC Insert_PostNrNaarGemeente '43002', '9968'; 
GO
EXEC Insert_PostNrNaarGemeente '43005', '9900'; 
GO
EXEC Insert_PostNrNaarGemeente '43007', '9970'; 
GO
EXEC Insert_PostNrNaarGemeente '43007', '9971'; 
GO
EXEC Insert_PostNrNaarGemeente '43010', '9990'; 
GO
EXEC Insert_PostNrNaarGemeente '43010', '9991'; 
GO
EXEC Insert_PostNrNaarGemeente '43010', '9992'; 
GO
EXEC Insert_PostNrNaarGemeente '43014', '9980'; 
GO
EXEC Insert_PostNrNaarGemeente '43014', '9981'; 
GO
EXEC Insert_PostNrNaarGemeente '43014', '9982'; 
GO
EXEC Insert_PostNrNaarGemeente '43014', '9988'; 
GO
EXEC Insert_PostNrNaarGemeente '43018', '9060'; 
GO
EXEC Insert_PostNrNaarGemeente '44001', '9880'; 
GO
EXEC Insert_PostNrNaarGemeente '44001', '9881'; 
GO
EXEC Insert_PostNrNaarGemeente '44011', '9800'; 
GO
EXEC Insert_PostNrNaarGemeente '44012', '9840'; 
GO
EXEC Insert_PostNrNaarGemeente '44013', '9070'; 
GO
EXEC Insert_PostNrNaarGemeente '44019', '9940'; 
GO
EXEC Insert_PostNrNaarGemeente '44020', '9890'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9000'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9030'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9031'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9032'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9040'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9041'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9042'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9050'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9051'; 
GO
EXEC Insert_PostNrNaarGemeente '44021', '9052'; 
GO
EXEC Insert_PostNrNaarGemeente '44029', '9910'; 
GO
EXEC Insert_PostNrNaarGemeente '44034', '9080'; 
GO
EXEC Insert_PostNrNaarGemeente '44036', '9920'; 
GO
EXEC Insert_PostNrNaarGemeente '44036', '9921'; 
GO
EXEC Insert_PostNrNaarGemeente '44040', '9090'; 
GO
EXEC Insert_PostNrNaarGemeente '44043', '9820'; 
GO
EXEC Insert_PostNrNaarGemeente '44045', '9180'; 
GO
EXEC Insert_PostNrNaarGemeente '44048', '9810'; 
GO
EXEC Insert_PostNrNaarGemeente '44049', '9850'; 
GO
EXEC Insert_PostNrNaarGemeente '44052', '9860'; 
GO
EXEC Insert_PostNrNaarGemeente '44064', '9830'; 
GO
EXEC Insert_PostNrNaarGemeente '44064', '9831'; 
GO
EXEC Insert_PostNrNaarGemeente '44072', '9950'; 
GO
EXEC Insert_PostNrNaarGemeente '44073', '9185'; 
GO
EXEC Insert_PostNrNaarGemeente '44080', '9930'; 
GO
EXEC Insert_PostNrNaarGemeente '44080', '9931'; 
GO
EXEC Insert_PostNrNaarGemeente '44080', '9932'; 
GO
EXEC Insert_PostNrNaarGemeente '44081', '9870'; 
GO
EXEC Insert_PostNrNaarGemeente '45017', '9770'; 
GO
EXEC Insert_PostNrNaarGemeente '45017', '9771'; 
GO
EXEC Insert_PostNrNaarGemeente '45017', '9772'; 
GO
EXEC Insert_PostNrNaarGemeente '45035', '9700'; 
GO
EXEC Insert_PostNrNaarGemeente '45041', '9600'; 
GO
EXEC Insert_PostNrNaarGemeente '45057', '9750'; 
GO
EXEC Insert_PostNrNaarGemeente '45059', '9660'; 
GO
EXEC Insert_PostNrNaarGemeente '45059', '9661'; 
GO
EXEC Insert_PostNrNaarGemeente '45060', '9690'; 
GO
EXEC Insert_PostNrNaarGemeente '45061', '9790'; 
GO
EXEC Insert_PostNrNaarGemeente '45062', '9667'; 
GO
EXEC Insert_PostNrNaarGemeente '45063', '9570'; 
GO
EXEC Insert_PostNrNaarGemeente '45063', '9571'; 
GO
EXEC Insert_PostNrNaarGemeente '45063', '9572'; 
GO
EXEC Insert_PostNrNaarGemeente '45064', '9680'; 
GO
EXEC Insert_PostNrNaarGemeente '45064', '9681'; 
GO
EXEC Insert_PostNrNaarGemeente '45064', '9688'; 
GO
EXEC Insert_PostNrNaarGemeente '45065', '9630'; 
GO
EXEC Insert_PostNrNaarGemeente '45065', '9636'; 
GO
EXEC Insert_PostNrNaarGemeente '46003', '9120'; 
GO
EXEC Insert_PostNrNaarGemeente '46003', '9130'; 
GO
EXEC Insert_PostNrNaarGemeente '46013', '9150'; 
GO
EXEC Insert_PostNrNaarGemeente '46014', '9160'; 
GO
EXEC Insert_PostNrNaarGemeente '46020', '9170'; 
GO
EXEC Insert_PostNrNaarGemeente '46021', '9100'; 
GO
EXEC Insert_PostNrNaarGemeente '46021', '9111'; 
GO
EXEC Insert_PostNrNaarGemeente '46021', '9112'; 
GO
EXEC Insert_PostNrNaarGemeente '46024', '9190'; 
GO
EXEC Insert_PostNrNaarGemeente '46025', '9140'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7800'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7801'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7802'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7803'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7804'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7810'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7811'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7812'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7822'; 
GO
EXEC Insert_PostNrNaarGemeente '51004', '7823'; 
GO
EXEC Insert_PostNrNaarGemeente '51008', '7970'; 
GO
EXEC Insert_PostNrNaarGemeente '51008', '7971'; 
GO
EXEC Insert_PostNrNaarGemeente '51008', '7972'; 
GO
EXEC Insert_PostNrNaarGemeente '51008', '7973'; 
GO
EXEC Insert_PostNrNaarGemeente '51009', '7320'; 
GO
EXEC Insert_PostNrNaarGemeente '51009', '7321'; 
GO
EXEC Insert_PostNrNaarGemeente '51009', '7322'; 
GO
EXEC Insert_PostNrNaarGemeente '51012', '7940'; 
GO
EXEC Insert_PostNrNaarGemeente '51012', '7941'; 
GO
EXEC Insert_PostNrNaarGemeente '51012', '7942'; 
GO
EXEC Insert_PostNrNaarGemeente '51012', '7943'; 
GO
EXEC Insert_PostNrNaarGemeente '51014', '7950'; 
GO
EXEC Insert_PostNrNaarGemeente '51014', '7951'; 
GO
EXEC Insert_PostNrNaarGemeente '51017', '7890'; 
GO
EXEC Insert_PostNrNaarGemeente '51019', '7880'; 
GO
EXEC Insert_PostNrNaarGemeente '51065', '7910'; 
GO
EXEC Insert_PostNrNaarGemeente '51065', '7911'; 
GO
EXEC Insert_PostNrNaarGemeente '51065', '7912'; 
GO
EXEC Insert_PostNrNaarGemeente '52010', '7160'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6000'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6001'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6010'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6020'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6030'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6031'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6032'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6040'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6041'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6042'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6043'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6044'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6060'; 
GO
EXEC Insert_PostNrNaarGemeente '52011', '6061'; 
GO
EXEC Insert_PostNrNaarGemeente '52012', '6200'; 
GO
EXEC Insert_PostNrNaarGemeente '52015', '6180'; 
GO
EXEC Insert_PostNrNaarGemeente '52015', '6181'; 
GO
EXEC Insert_PostNrNaarGemeente '52015', '6182'; 
GO
EXEC Insert_PostNrNaarGemeente '52015', '6183'; 
GO
EXEC Insert_PostNrNaarGemeente '52018', '6240'; 
GO
EXEC Insert_PostNrNaarGemeente '52021', '6220'; 
GO
EXEC Insert_PostNrNaarGemeente '52021', '6221'; 
GO
EXEC Insert_PostNrNaarGemeente '52021', '6222'; 
GO
EXEC Insert_PostNrNaarGemeente '52021', '6223'; 
GO
EXEC Insert_PostNrNaarGemeente '52021', '6224'; 
GO
EXEC Insert_PostNrNaarGemeente '52022', '6140'; 
GO
EXEC Insert_PostNrNaarGemeente '52022', '6141'; 
GO
EXEC Insert_PostNrNaarGemeente '52022', '6142'; 
GO
EXEC Insert_PostNrNaarGemeente '52025', '6280'; 
GO
EXEC Insert_PostNrNaarGemeente '52043', '7170'; 
GO
EXEC Insert_PostNrNaarGemeente '52048', '6110'; 
GO
EXEC Insert_PostNrNaarGemeente '52048', '6111'; 
GO
EXEC Insert_PostNrNaarGemeente '52055', '6230'; 
GO
EXEC Insert_PostNrNaarGemeente '52055', '6238'; 
GO
EXEC Insert_PostNrNaarGemeente '52063', '7180'; 
GO
EXEC Insert_PostNrNaarGemeente '52063', '7181'; 
GO
EXEC Insert_PostNrNaarGemeente '52074', '6250'; 
GO
EXEC Insert_PostNrNaarGemeente '52075', '6210'; 
GO
EXEC Insert_PostNrNaarGemeente '52075', '6211'; 
GO
EXEC Insert_PostNrNaarGemeente '53014', '7300'; 
GO
EXEC Insert_PostNrNaarGemeente '53014', '7301'; 
GO
EXEC Insert_PostNrNaarGemeente '53020', '7370'; 
GO
EXEC Insert_PostNrNaarGemeente '53028', '7080'; 
GO
EXEC Insert_PostNrNaarGemeente '53039', '7350'; 
GO
EXEC Insert_PostNrNaarGemeente '53044', '7050'; 
GO
EXEC Insert_PostNrNaarGemeente '53046', '7870'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7000'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7011'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7012'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7020'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7021'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7022'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7024'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7030'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7031'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7032'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7033'; 
GO
EXEC Insert_PostNrNaarGemeente '53053', '7034'; 
GO
EXEC Insert_PostNrNaarGemeente '53065', '7390'; 
GO
EXEC Insert_PostNrNaarGemeente '53068', '7380'; 
GO
EXEC Insert_PostNrNaarGemeente '53068', '7382'; 
GO
EXEC Insert_PostNrNaarGemeente '53070', '7330'; 
GO
EXEC Insert_PostNrNaarGemeente '53070', '7331'; 
GO
EXEC Insert_PostNrNaarGemeente '53070', '7332'; 
GO
EXEC Insert_PostNrNaarGemeente '53070', '7333'; 
GO
EXEC Insert_PostNrNaarGemeente '53070', '7334'; 
GO
EXEC Insert_PostNrNaarGemeente '53082', '7340'; 
GO
EXEC Insert_PostNrNaarGemeente '53083', '7387'; 
GO
EXEC Insert_PostNrNaarGemeente '53084', '7040'; 
GO
EXEC Insert_PostNrNaarGemeente '53084', '7041'; 
GO
EXEC Insert_PostNrNaarGemeente '54007', '7700'; 
GO
EXEC Insert_PostNrNaarGemeente '54007', '7711'; 
GO
EXEC Insert_PostNrNaarGemeente '54007', '7712'; 
GO
EXEC Insert_PostNrNaarGemeente '54010', '7780'; 
GO
EXEC Insert_PostNrNaarGemeente '54010', '7781'; 
GO
EXEC Insert_PostNrNaarGemeente '54010', '7782'; 
GO
EXEC Insert_PostNrNaarGemeente '54010', '7783'; 
GO
EXEC Insert_PostNrNaarGemeente '54010', '7784'; 
GO
EXEC Insert_PostNrNaarGemeente '55004', '7090'; 
GO
EXEC Insert_PostNrNaarGemeente '55010', '7850'; 
GO
EXEC Insert_PostNrNaarGemeente '55022', '7100'; 
GO
EXEC Insert_PostNrNaarGemeente '55022', '7110'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7860'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7861'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7862'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7863'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7864'; 
GO
EXEC Insert_PostNrNaarGemeente '55023', '7866'; 
GO
EXEC Insert_PostNrNaarGemeente '55035', '7070'; 
GO
EXEC Insert_PostNrNaarGemeente '55039', '7830'; 
GO
EXEC Insert_PostNrNaarGemeente '55040', '7060'; 
GO
EXEC Insert_PostNrNaarGemeente '55040', '7061'; 
GO
EXEC Insert_PostNrNaarGemeente '55040', '7062'; 
GO
EXEC Insert_PostNrNaarGemeente '55040', '7063'; 
GO
EXEC Insert_PostNrNaarGemeente '55050', '7190'; 
GO
EXEC Insert_PostNrNaarGemeente '55050', '7191'; 
GO
EXEC Insert_PostNrNaarGemeente '56001', '6150'; 
GO
EXEC Insert_PostNrNaarGemeente '56005', '6500'; 
GO
EXEC Insert_PostNrNaarGemeente '56005', '6511'; 
GO
EXEC Insert_PostNrNaarGemeente '56011', '7130'; 
GO
EXEC Insert_PostNrNaarGemeente '56011', '7131'; 
GO
EXEC Insert_PostNrNaarGemeente '56011', '7133'; 
GO
EXEC Insert_PostNrNaarGemeente '56011', '7134'; 
GO
EXEC Insert_PostNrNaarGemeente '56016', '6460'; 
GO
EXEC Insert_PostNrNaarGemeente '56016', '6461'; 
GO
EXEC Insert_PostNrNaarGemeente '56016', '6462'; 
GO
EXEC Insert_PostNrNaarGemeente '56016', '6463'; 
GO
EXEC Insert_PostNrNaarGemeente '56016', '6464'; 
GO
EXEC Insert_PostNrNaarGemeente '56022', '6560'; 
GO
EXEC Insert_PostNrNaarGemeente '56029', '6440'; 
GO
EXEC Insert_PostNrNaarGemeente '56029', '6441'; 
GO
EXEC Insert_PostNrNaarGemeente '56044', '6540'; 
GO
EXEC Insert_PostNrNaarGemeente '56044', '6542'; 
GO
EXEC Insert_PostNrNaarGemeente '56044', '6543'; 
GO
EXEC Insert_PostNrNaarGemeente '56049', '6567'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6590'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6591'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6592'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6593'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6594'; 
GO
EXEC Insert_PostNrNaarGemeente '56051', '6596'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6530'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6531'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6532'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6533'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6534'; 
GO
EXEC Insert_PostNrNaarGemeente '56078', '6536'; 
GO
EXEC Insert_PostNrNaarGemeente '56085', '7120'; 
GO
EXEC Insert_PostNrNaarGemeente '56086', '6120'; 
GO
EXEC Insert_PostNrNaarGemeente '56087', '7140'; 
GO
EXEC Insert_PostNrNaarGemeente '56087', '7141'; 
GO
EXEC Insert_PostNrNaarGemeente '56088', '6470'; 
GO
EXEC Insert_PostNrNaarGemeente '57003', '7640'; 
GO
EXEC Insert_PostNrNaarGemeente '57003', '7641'; 
GO
EXEC Insert_PostNrNaarGemeente '57003', '7642'; 
GO
EXEC Insert_PostNrNaarGemeente '57003', '7643'; 
GO
EXEC Insert_PostNrNaarGemeente '57018', '7760'; 
GO
EXEC Insert_PostNrNaarGemeente '57027', '7730'; 
GO
EXEC Insert_PostNrNaarGemeente '57062', '7740'; 
GO
EXEC Insert_PostNrNaarGemeente '57062', '7742'; 
GO
EXEC Insert_PostNrNaarGemeente '57062', '7743'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7600'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7601'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7602'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7603'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7604'; 
GO
EXEC Insert_PostNrNaarGemeente '57064', '7608'; 
GO
EXEC Insert_PostNrNaarGemeente '57072', '7610'; 
GO
EXEC Insert_PostNrNaarGemeente '57072', '7611'; 
GO
EXEC Insert_PostNrNaarGemeente '57072', '7618'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7500'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7501'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7502'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7503'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7504'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7506'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7520'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7521'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7522'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7530'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7531'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7532'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7533'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7534'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7536'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7538'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7540'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7542'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7543'; 
GO
EXEC Insert_PostNrNaarGemeente '57081', '7548'; 
GO
EXEC Insert_PostNrNaarGemeente '57093', '7620'; 
GO
EXEC Insert_PostNrNaarGemeente '57093', '7621'; 
GO
EXEC Insert_PostNrNaarGemeente '57093', '7622'; 
GO
EXEC Insert_PostNrNaarGemeente '57093', '7623'; 
GO
EXEC Insert_PostNrNaarGemeente '57093', '7624'; 
GO
EXEC Insert_PostNrNaarGemeente '57094', '7900'; 
GO
EXEC Insert_PostNrNaarGemeente '57094', '7901'; 
GO
EXEC Insert_PostNrNaarGemeente '57094', '7903'; 
GO
EXEC Insert_PostNrNaarGemeente '57094', '7904'; 
GO
EXEC Insert_PostNrNaarGemeente '57094', '7906'; 
GO
EXEC Insert_PostNrNaarGemeente '57095', '7750'; 
GO
EXEC Insert_PostNrNaarGemeente '61003', '4540'; 
GO
EXEC Insert_PostNrNaarGemeente '61010', '4210'; 
GO
EXEC Insert_PostNrNaarGemeente '61012', '4560'; 
GO
EXEC Insert_PostNrNaarGemeente '61019', '4190'; 
GO
EXEC Insert_PostNrNaarGemeente '61024', '4180'; 
GO
EXEC Insert_PostNrNaarGemeente '61024', '4181'; 
GO
EXEC Insert_PostNrNaarGemeente '61028', '4217'; 
GO
EXEC Insert_PostNrNaarGemeente '61028', '4218'; 
GO
EXEC Insert_PostNrNaarGemeente '61031', '4500'; 
GO
EXEC Insert_PostNrNaarGemeente '61039', '4570'; 
GO
EXEC Insert_PostNrNaarGemeente '61041', '4577'; 
GO
EXEC Insert_PostNrNaarGemeente '61043', '4550'; 
GO
EXEC Insert_PostNrNaarGemeente '61048', '4590'; 
GO
EXEC Insert_PostNrNaarGemeente '61063', '4537'; 
GO
EXEC Insert_PostNrNaarGemeente '61068', '4530'; 
GO
EXEC Insert_PostNrNaarGemeente '61072', '4520'; 
GO
EXEC Insert_PostNrNaarGemeente '61079', '4160'; 
GO
EXEC Insert_PostNrNaarGemeente '61079', '4161'; 
GO
EXEC Insert_PostNrNaarGemeente '61079', '4162'; 
GO
EXEC Insert_PostNrNaarGemeente '61079', '4163'; 
GO
EXEC Insert_PostNrNaarGemeente '61080', '4480'; 
GO
EXEC Insert_PostNrNaarGemeente '61081', '4557'; 
GO
EXEC Insert_PostNrNaarGemeente '62003', '4430'; 
GO
EXEC Insert_PostNrNaarGemeente '62003', '4431'; 
GO
EXEC Insert_PostNrNaarGemeente '62003', '4432'; 
GO
EXEC Insert_PostNrNaarGemeente '62006', '4340'; 
GO
EXEC Insert_PostNrNaarGemeente '62006', '4342'; 
GO
EXEC Insert_PostNrNaarGemeente '62009', '4920'; 
GO
EXEC Insert_PostNrNaarGemeente '62011', '4690'; 
GO
EXEC Insert_PostNrNaarGemeente '62015', '4610'; 
GO
EXEC Insert_PostNrNaarGemeente '62022', '4050'; 
GO
EXEC Insert_PostNrNaarGemeente '62022', '4051'; 
GO
EXEC Insert_PostNrNaarGemeente '62022', '4052'; 
GO
EXEC Insert_PostNrNaarGemeente '62022', '4053'; 
GO
EXEC Insert_PostNrNaarGemeente '62026', '4170'; 
GO
EXEC Insert_PostNrNaarGemeente '62026', '4171'; 
GO
EXEC Insert_PostNrNaarGemeente '62027', '4606'; 
GO
EXEC Insert_PostNrNaarGemeente '62027', '4607'; 
GO
EXEC Insert_PostNrNaarGemeente '62027', '4608'; 
GO
EXEC Insert_PostNrNaarGemeente '62032', '4130'; 
GO
EXEC Insert_PostNrNaarGemeente '62038', '4620'; 
GO
EXEC Insert_PostNrNaarGemeente '62038', '4621'; 
GO
EXEC Insert_PostNrNaarGemeente '62038', '4623'; 
GO
EXEC Insert_PostNrNaarGemeente '62038', '4624'; 
GO
EXEC Insert_PostNrNaarGemeente '62051', '4040'; 
GO
EXEC Insert_PostNrNaarGemeente '62051', '4041'; 
GO
EXEC Insert_PostNrNaarGemeente '62051', '4042'; 
GO
EXEC Insert_PostNrNaarGemeente '62060', '4450'; 
GO
EXEC Insert_PostNrNaarGemeente '62060', '4451'; 
GO
EXEC Insert_PostNrNaarGemeente '62060', '4452'; 
GO
EXEC Insert_PostNrNaarGemeente '62060', '4453'; 
GO
EXEC Insert_PostNrNaarGemeente '62060', '4458'; 
GO
EXEC Insert_PostNrNaarGemeente '62063', '4000'; 
GO
EXEC Insert_PostNrNaarGemeente '62063', '4020'; 
GO
EXEC Insert_PostNrNaarGemeente '62063', '4030'; 
GO
EXEC Insert_PostNrNaarGemeente '62063', '4031'; 
GO
EXEC Insert_PostNrNaarGemeente '62063', '4032'; 
GO
EXEC Insert_PostNrNaarGemeente '62079', '4680'; 
GO
EXEC Insert_PostNrNaarGemeente '62079', '4681'; 
GO
EXEC Insert_PostNrNaarGemeente '62079', '4682'; 
GO
EXEC Insert_PostNrNaarGemeente '62079', '4683'; 
GO
EXEC Insert_PostNrNaarGemeente '62079', '4684'; 
GO
EXEC Insert_PostNrNaarGemeente '62093', '4420'; 
GO
EXEC Insert_PostNrNaarGemeente '62096', '4100'; 
GO
EXEC Insert_PostNrNaarGemeente '62096', '4101'; 
GO
EXEC Insert_PostNrNaarGemeente '62096', '4102'; 
GO
EXEC Insert_PostNrNaarGemeente '62099', '4630'; 
GO
EXEC Insert_PostNrNaarGemeente '62099', '4631'; 
GO
EXEC Insert_PostNrNaarGemeente '62099', '4632'; 
GO
EXEC Insert_PostNrNaarGemeente '62099', '4633'; 
GO
EXEC Insert_PostNrNaarGemeente '62100', '4140'; 
GO
EXEC Insert_PostNrNaarGemeente '62100', '4141'; 
GO
EXEC Insert_PostNrNaarGemeente '62108', '4600'; 
GO
EXEC Insert_PostNrNaarGemeente '62108', '4601'; 
GO
EXEC Insert_PostNrNaarGemeente '62108', '4602'; 
GO
EXEC Insert_PostNrNaarGemeente '62118', '4460'; 
GO
EXEC Insert_PostNrNaarGemeente '62119', '4670'; 
GO
EXEC Insert_PostNrNaarGemeente '62119', '4671'; 
GO
EXEC Insert_PostNrNaarGemeente '62119', '4672'; 
GO
EXEC Insert_PostNrNaarGemeente '62120', '4400'; 
GO
EXEC Insert_PostNrNaarGemeente '62121', '4120'; 
GO
EXEC Insert_PostNrNaarGemeente '62121', '4121'; 
GO
EXEC Insert_PostNrNaarGemeente '62121', '4122'; 
GO
EXEC Insert_PostNrNaarGemeente '62122', '4870'; 
GO
EXEC Insert_PostNrNaarGemeente '63001', '4770'; 
GO
EXEC Insert_PostNrNaarGemeente '63001', '4771'; 
GO
EXEC Insert_PostNrNaarGemeente '63003', '4880'; 
GO
EXEC Insert_PostNrNaarGemeente '63004', '4837'; 
GO
EXEC Insert_PostNrNaarGemeente '63012', '4760'; 
GO
EXEC Insert_PostNrNaarGemeente '63012', '4761'; 
GO
EXEC Insert_PostNrNaarGemeente '63013', '4750'; 
GO
EXEC Insert_PostNrNaarGemeente '63020', '4820'; 
GO
EXEC Insert_PostNrNaarGemeente '63020', '4821'; 
GO
EXEC Insert_PostNrNaarGemeente '63023', '4700'; 
GO
EXEC Insert_PostNrNaarGemeente '63023', '4701'; 
GO
EXEC Insert_PostNrNaarGemeente '63035', '4650'; 
GO
EXEC Insert_PostNrNaarGemeente '63035', '4651'; 
GO
EXEC Insert_PostNrNaarGemeente '63035', '4652'; 
GO
EXEC Insert_PostNrNaarGemeente '63035', '4653'; 
GO
EXEC Insert_PostNrNaarGemeente '63035', '4654'; 
GO
EXEC Insert_PostNrNaarGemeente '63038', '4845'; 
GO
EXEC Insert_PostNrNaarGemeente '63040', '4720'; 
GO
EXEC Insert_PostNrNaarGemeente '63040', '4721'; 
GO
EXEC Insert_PostNrNaarGemeente '63040', '4728'; 
GO
EXEC Insert_PostNrNaarGemeente '63045', '4990'; 
GO
EXEC Insert_PostNrNaarGemeente '63046', '4830'; 
GO
EXEC Insert_PostNrNaarGemeente '63046', '4831'; 
GO
EXEC Insert_PostNrNaarGemeente '63046', '4834'; 
GO
EXEC Insert_PostNrNaarGemeente '63048', '4710'; 
GO
EXEC Insert_PostNrNaarGemeente '63048', '4711'; 
GO
EXEC Insert_PostNrNaarGemeente '63049', '4960'; 
GO
EXEC Insert_PostNrNaarGemeente '63057', '4877'; 
GO
EXEC Insert_PostNrNaarGemeente '63058', '4860'; 
GO
EXEC Insert_PostNrNaarGemeente '63058', '4861'; 
GO
EXEC Insert_PostNrNaarGemeente '63061', '4730'; 
GO
EXEC Insert_PostNrNaarGemeente '63061', '4731'; 
GO
EXEC Insert_PostNrNaarGemeente '63067', '4780'; 
GO
EXEC Insert_PostNrNaarGemeente '63067', '4782'; 
GO
EXEC Insert_PostNrNaarGemeente '63067', '4783'; 
GO
EXEC Insert_PostNrNaarGemeente '63067', '4784'; 
GO
EXEC Insert_PostNrNaarGemeente '63072', '4900'; 
GO
EXEC Insert_PostNrNaarGemeente '63073', '4970'; 
GO
EXEC Insert_PostNrNaarGemeente '63075', '4987'; 
GO
EXEC Insert_PostNrNaarGemeente '63076', '4910'; 
GO
EXEC Insert_PostNrNaarGemeente '63079', '4800'; 
GO
EXEC Insert_PostNrNaarGemeente '63079', '4801'; 
GO
EXEC Insert_PostNrNaarGemeente '63079', '4802'; 
GO
EXEC Insert_PostNrNaarGemeente '63080', '4950'; 
GO
EXEC Insert_PostNrNaarGemeente '63084', '4840'; 
GO
EXEC Insert_PostNrNaarGemeente '63084', '4841'; 
GO
EXEC Insert_PostNrNaarGemeente '63086', '4980'; 
GO
EXEC Insert_PostNrNaarGemeente '63086', '4983'; 
GO
EXEC Insert_PostNrNaarGemeente '63087', '4790'; 
GO
EXEC Insert_PostNrNaarGemeente '63087', '4791'; 
GO
EXEC Insert_PostNrNaarGemeente '63088', '4850'; 
GO
EXEC Insert_PostNrNaarGemeente '63088', '4851'; 
GO
EXEC Insert_PostNrNaarGemeente '63088', '4852'; 
GO
EXEC Insert_PostNrNaarGemeente '63089', '4890'; 
GO
EXEC Insert_PostNrNaarGemeente '64008', '4257'; 
GO
EXEC Insert_PostNrNaarGemeente '64015', '4260'; 
GO
EXEC Insert_PostNrNaarGemeente '64015', '4261'; 
GO
EXEC Insert_PostNrNaarGemeente '64015', '4263'; 
GO
EXEC Insert_PostNrNaarGemeente '64021', '4367'; 
GO
EXEC Insert_PostNrNaarGemeente '64023', '4357'; 
GO
EXEC Insert_PostNrNaarGemeente '64025', '4347'; 
GO
EXEC Insert_PostNrNaarGemeente '64029', '4250'; 
GO
EXEC Insert_PostNrNaarGemeente '64029', '4252'; 
GO
EXEC Insert_PostNrNaarGemeente '64029', '4253'; 
GO
EXEC Insert_PostNrNaarGemeente '64029', '4254'; 
GO
EXEC Insert_PostNrNaarGemeente '64034', '4280'; 
GO
EXEC Insert_PostNrNaarGemeente '64047', '4287'; 
GO
EXEC Insert_PostNrNaarGemeente '64056', '4360'; 
GO
EXEC Insert_PostNrNaarGemeente '64063', '4350'; 
GO
EXEC Insert_PostNrNaarGemeente '64063', '4351'; 
GO
EXEC Insert_PostNrNaarGemeente '64065', '4470'; 
GO
EXEC Insert_PostNrNaarGemeente '64074', '4300'; 
GO
EXEC Insert_PostNrNaarGemeente '64075', '4219'; 
GO
EXEC Insert_PostNrNaarGemeente '64076', '4317'; 
GO
EXEC Insert_PostNrNaarGemeente '71002', '3665'; 
GO
EXEC Insert_PostNrNaarGemeente '71004', '3580'; 
GO
EXEC Insert_PostNrNaarGemeente '71004', '3581'; 
GO
EXEC Insert_PostNrNaarGemeente '71004', '3582'; 
GO
EXEC Insert_PostNrNaarGemeente '71004', '3583'; 
GO
EXEC Insert_PostNrNaarGemeente '71011', '3590'; 
GO
EXEC Insert_PostNrNaarGemeente '71016', '3600'; 
GO
EXEC Insert_PostNrNaarGemeente '71017', '3890'; 
GO
EXEC Insert_PostNrNaarGemeente '71017', '3891'; 
GO
EXEC Insert_PostNrNaarGemeente '71020', '3545'; 
GO
EXEC Insert_PostNrNaarGemeente '71022', '3500'; 
GO
EXEC Insert_PostNrNaarGemeente '71022', '3501'; 
GO
EXEC Insert_PostNrNaarGemeente '71022', '3510'; 
GO
EXEC Insert_PostNrNaarGemeente '71022', '3511'; 
GO
EXEC Insert_PostNrNaarGemeente '71022', '3512'; 
GO
EXEC Insert_PostNrNaarGemeente '71024', '3540'; 
GO
EXEC Insert_PostNrNaarGemeente '71034', '3970'; 
GO
EXEC Insert_PostNrNaarGemeente '71034', '3971'; 
GO
EXEC Insert_PostNrNaarGemeente '71037', '3560'; 
GO
EXEC Insert_PostNrNaarGemeente '71045', '3850'; 
GO
EXEC Insert_PostNrNaarGemeente '71047', '3660'; 
GO
EXEC Insert_PostNrNaarGemeente '71053', '3800'; 
GO
EXEC Insert_PostNrNaarGemeente '71053', '3803'; 
GO
EXEC Insert_PostNrNaarGemeente '71053', '3806'; 
GO
EXEC Insert_PostNrNaarGemeente '71057', '3980'; 
GO
EXEC Insert_PostNrNaarGemeente '71066', '3520'; 
GO
EXEC Insert_PostNrNaarGemeente '71067', '3690'; 
GO
EXEC Insert_PostNrNaarGemeente '71069', '3945'; 
GO
EXEC Insert_PostNrNaarGemeente '71070', '3550'; 
GO
EXEC Insert_PostNrNaarGemeente '72003', '3950'; 
GO
EXEC Insert_PostNrNaarGemeente '72004', '3960'; 
GO
EXEC Insert_PostNrNaarGemeente '72018', '3640'; 
GO
EXEC Insert_PostNrNaarGemeente '72020', '3920'; 
GO
EXEC Insert_PostNrNaarGemeente '72021', '3680'; 
GO
EXEC Insert_PostNrNaarGemeente '72025', '3910'; 
GO
EXEC Insert_PostNrNaarGemeente '72029', '3900'; 
GO
EXEC Insert_PostNrNaarGemeente '72030', '3990'; 
GO
EXEC Insert_PostNrNaarGemeente '72037', '3930'; 
GO
EXEC Insert_PostNrNaarGemeente '72038', '3940'; 
GO
EXEC Insert_PostNrNaarGemeente '72039', '3530'; 
GO
EXEC Insert_PostNrNaarGemeente '72040', '3670'; 
GO
EXEC Insert_PostNrNaarGemeente '72041', '3650'; 
GO
EXEC Insert_PostNrNaarGemeente '73001', '3570'; 
GO
EXEC Insert_PostNrNaarGemeente '73006', '3740'; 
GO
EXEC Insert_PostNrNaarGemeente '73006', '3742'; 
GO
EXEC Insert_PostNrNaarGemeente '73006', '3746'; 
GO
EXEC Insert_PostNrNaarGemeente '73009', '3840'; 
GO
EXEC Insert_PostNrNaarGemeente '73022', '3870'; 
GO
EXEC Insert_PostNrNaarGemeente '73028', '3717'; 
GO
EXEC Insert_PostNrNaarGemeente '73032', '3730'; 
GO
EXEC Insert_PostNrNaarGemeente '73032', '3732'; 
GO
EXEC Insert_PostNrNaarGemeente '73040', '3720'; 
GO
EXEC Insert_PostNrNaarGemeente '73040', '3721'; 
GO
EXEC Insert_PostNrNaarGemeente '73040', '3722'; 
GO
EXEC Insert_PostNrNaarGemeente '73040', '3723'; 
GO
EXEC Insert_PostNrNaarGemeente '73040', '3724'; 
GO
EXEC Insert_PostNrNaarGemeente '73042', '3620'; 
GO
EXEC Insert_PostNrNaarGemeente '73042', '3621'; 
GO
EXEC Insert_PostNrNaarGemeente '73066', '3770'; 
GO
EXEC Insert_PostNrNaarGemeente '73083', '3700'; 
GO
EXEC Insert_PostNrNaarGemeente '73098', '3830'; 
GO
EXEC Insert_PostNrNaarGemeente '73098', '3832'; 
GO
EXEC Insert_PostNrNaarGemeente '73107', '3630'; 
GO
EXEC Insert_PostNrNaarGemeente '73107', '3631'; 
GO
EXEC Insert_PostNrNaarGemeente '73109', '3790'; 
GO
EXEC Insert_PostNrNaarGemeente '73109', '3791'; 
GO
EXEC Insert_PostNrNaarGemeente '73109', '3792'; 
GO
EXEC Insert_PostNrNaarGemeente '73109', '3793'; 
GO
EXEC Insert_PostNrNaarGemeente '73109', '3798'; 
GO
EXEC Insert_PostNrNaarGemeente '81001', '6700'; 
GO
EXEC Insert_PostNrNaarGemeente '81003', '6717'; 
GO
EXEC Insert_PostNrNaarGemeente '81004', '6790'; 
GO
EXEC Insert_PostNrNaarGemeente '81004', '6791'; 
GO
EXEC Insert_PostNrNaarGemeente '81004', '6792'; 
GO
EXEC Insert_PostNrNaarGemeente '81013', '6630'; 
GO
EXEC Insert_PostNrNaarGemeente '81015', '6780'; 
GO
EXEC Insert_PostNrNaarGemeente '81015', '6781'; 
GO
EXEC Insert_PostNrNaarGemeente '81015', '6782'; 
GO
EXEC Insert_PostNrNaarGemeente '82003', '6600'; 
GO
EXEC Insert_PostNrNaarGemeente '82005', '6686'; 
GO
EXEC Insert_PostNrNaarGemeente '82005', '6687'; 
GO
EXEC Insert_PostNrNaarGemeente '82005', '6688'; 
GO
EXEC Insert_PostNrNaarGemeente '82009', '6637'; 
GO
EXEC Insert_PostNrNaarGemeente '82014', '6660'; 
GO
EXEC Insert_PostNrNaarGemeente '82014', '6661'; 
GO
EXEC Insert_PostNrNaarGemeente '82014', '6662'; 
GO
EXEC Insert_PostNrNaarGemeente '82014', '6663'; 
GO
EXEC Insert_PostNrNaarGemeente '82014', '6666'; 
GO
EXEC Insert_PostNrNaarGemeente '82032', '6690'; 
GO
EXEC Insert_PostNrNaarGemeente '82032', '6692'; 
GO
EXEC Insert_PostNrNaarGemeente '82032', '6698'; 
GO
EXEC Insert_PostNrNaarGemeente '82036', '6640'; 
GO
EXEC Insert_PostNrNaarGemeente '82036', '6642'; 
GO
EXEC Insert_PostNrNaarGemeente '82037', '6670'; 
GO
EXEC Insert_PostNrNaarGemeente '82037', '6671'; 
GO
EXEC Insert_PostNrNaarGemeente '82037', '6672'; 
GO
EXEC Insert_PostNrNaarGemeente '82037', '6673'; 
GO
EXEC Insert_PostNrNaarGemeente '82037', '6674'; 
GO
EXEC Insert_PostNrNaarGemeente '82038', '6680'; 
GO
EXEC Insert_PostNrNaarGemeente '82038', '6681'; 
GO
EXEC Insert_PostNrNaarGemeente '83012', '6940'; 
GO
EXEC Insert_PostNrNaarGemeente '83012', '6941'; 
GO
EXEC Insert_PostNrNaarGemeente '83013', '6997'; 
GO
EXEC Insert_PostNrNaarGemeente '83028', '6990'; 
GO
EXEC Insert_PostNrNaarGemeente '83031', '6980'; 
GO
EXEC Insert_PostNrNaarGemeente '83031', '6982'; 
GO
EXEC Insert_PostNrNaarGemeente '83031', '6983'; 
GO
EXEC Insert_PostNrNaarGemeente '83031', '6984'; 
GO
EXEC Insert_PostNrNaarGemeente '83031', '6986'; 
GO
EXEC Insert_PostNrNaarGemeente '83034', '6900'; 
GO
EXEC Insert_PostNrNaarGemeente '83040', '6950'; 
GO
EXEC Insert_PostNrNaarGemeente '83040', '6951'; 
GO
EXEC Insert_PostNrNaarGemeente '83040', '6952'; 
GO
EXEC Insert_PostNrNaarGemeente '83040', '6953'; 
GO
EXEC Insert_PostNrNaarGemeente '83044', '6987'; 
GO
EXEC Insert_PostNrNaarGemeente '83049', '6970'; 
GO
EXEC Insert_PostNrNaarGemeente '83049', '6971'; 
GO
EXEC Insert_PostNrNaarGemeente '83049', '6972'; 
GO
EXEC Insert_PostNrNaarGemeente '83055', '6960'; 
GO
EXEC Insert_PostNrNaarGemeente '84009', '6880'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6830'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6831'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6832'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6833'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6834'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6836'; 
GO
EXEC Insert_PostNrNaarGemeente '84010', '6838'; 
GO
EXEC Insert_PostNrNaarGemeente '84016', '6929'; 
GO
EXEC Insert_PostNrNaarGemeente '84029', '6887'; 
GO
EXEC Insert_PostNrNaarGemeente '84033', '6860'; 
GO
EXEC Insert_PostNrNaarGemeente '84035', '6890'; 
GO
EXEC Insert_PostNrNaarGemeente '84043', '6840'; 
GO
EXEC Insert_PostNrNaarGemeente '84050', '6850'; 
GO
EXEC Insert_PostNrNaarGemeente '84050', '6851'; 
GO
EXEC Insert_PostNrNaarGemeente '84050', '6852'; 
GO
EXEC Insert_PostNrNaarGemeente '84050', '6853'; 
GO
EXEC Insert_PostNrNaarGemeente '84050', '6856'; 
GO
EXEC Insert_PostNrNaarGemeente '84059', '6870'; 
GO
EXEC Insert_PostNrNaarGemeente '84068', '6927'; 
GO
EXEC Insert_PostNrNaarGemeente '84075', '6920'; 
GO
EXEC Insert_PostNrNaarGemeente '84075', '6921'; 
GO
EXEC Insert_PostNrNaarGemeente '84075', '6922'; 
GO
EXEC Insert_PostNrNaarGemeente '84075', '6924'; 
GO
EXEC Insert_PostNrNaarGemeente '84077', '6800'; 
GO
EXEC Insert_PostNrNaarGemeente '85007', '6810'; 
GO
EXEC Insert_PostNrNaarGemeente '85007', '6811'; 
GO
EXEC Insert_PostNrNaarGemeente '85007', '6812'; 
GO
EXEC Insert_PostNrNaarGemeente '85007', '6813'; 
GO
EXEC Insert_PostNrNaarGemeente '85009', '6740'; 
GO
EXEC Insert_PostNrNaarGemeente '85009', '6741'; 
GO
EXEC Insert_PostNrNaarGemeente '85009', '6742'; 
GO
EXEC Insert_PostNrNaarGemeente '85009', '6743'; 
GO
EXEC Insert_PostNrNaarGemeente '85011', '6820'; 
GO
EXEC Insert_PostNrNaarGemeente '85011', '6821'; 
GO
EXEC Insert_PostNrNaarGemeente '85011', '6823'; 
GO
EXEC Insert_PostNrNaarGemeente '85011', '6824'; 
GO
EXEC Insert_PostNrNaarGemeente '85024', '6769'; 
GO
EXEC Insert_PostNrNaarGemeente '85026', '6750'; 
GO
EXEC Insert_PostNrNaarGemeente '85034', '6747'; 
GO
EXEC Insert_PostNrNaarGemeente '85039', '6730'; 
GO
EXEC Insert_PostNrNaarGemeente '85045', '6760'; 
GO
EXEC Insert_PostNrNaarGemeente '85045', '6761'; 
GO
EXEC Insert_PostNrNaarGemeente '85045', '6762'; 
GO
EXEC Insert_PostNrNaarGemeente '85046', '6720'; 
GO
EXEC Insert_PostNrNaarGemeente '85046', '6721'; 
GO
EXEC Insert_PostNrNaarGemeente '85046', '6723'; 
GO
EXEC Insert_PostNrNaarGemeente '85046', '6724'; 
GO
EXEC Insert_PostNrNaarGemeente '85047', '6767'; 
GO
EXEC Insert_PostNrNaarGemeente '91005', '5537'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5570'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5571'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5572'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5573'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5574'; 
GO
EXEC Insert_PostNrNaarGemeente '91013', '5576'; 
GO
EXEC Insert_PostNrNaarGemeente '91015', '5555'; 
GO
EXEC Insert_PostNrNaarGemeente '91030', '5590'; 
GO
EXEC Insert_PostNrNaarGemeente '91034', '5500'; 
GO
EXEC Insert_PostNrNaarGemeente '91034', '5501'; 
GO
EXEC Insert_PostNrNaarGemeente '91034', '5502'; 
GO
EXEC Insert_PostNrNaarGemeente '91034', '5503'; 
GO
EXEC Insert_PostNrNaarGemeente '91034', '5504'; 
GO
EXEC Insert_PostNrNaarGemeente '91054', '5575'; 
GO
EXEC Insert_PostNrNaarGemeente '91059', '5360'; 
GO
EXEC Insert_PostNrNaarGemeente '91059', '5361'; 
GO
EXEC Insert_PostNrNaarGemeente '91059', '5362'; 
GO
EXEC Insert_PostNrNaarGemeente '91059', '5363'; 
GO
EXEC Insert_PostNrNaarGemeente '91059', '5364'; 
GO
EXEC Insert_PostNrNaarGemeente '91064', '5370'; 
GO
EXEC Insert_PostNrNaarGemeente '91064', '5372'; 
GO
EXEC Insert_PostNrNaarGemeente '91064', '5374'; 
GO
EXEC Insert_PostNrNaarGemeente '91064', '5376'; 
GO
EXEC Insert_PostNrNaarGemeente '91072', '5560'; 
GO
EXEC Insert_PostNrNaarGemeente '91072', '5561'; 
GO
EXEC Insert_PostNrNaarGemeente '91072', '5562'; 
GO
EXEC Insert_PostNrNaarGemeente '91072', '5563'; 
GO
EXEC Insert_PostNrNaarGemeente '91072', '5564'; 
GO
EXEC Insert_PostNrNaarGemeente '91103', '5520'; 
GO
EXEC Insert_PostNrNaarGemeente '91103', '5521'; 
GO
EXEC Insert_PostNrNaarGemeente '91103', '5522'; 
GO
EXEC Insert_PostNrNaarGemeente '91103', '5523'; 
GO
EXEC Insert_PostNrNaarGemeente '91103', '5524'; 
GO
EXEC Insert_PostNrNaarGemeente '91114', '5580'; 
GO
EXEC Insert_PostNrNaarGemeente '91120', '5377'; 
GO
EXEC Insert_PostNrNaarGemeente '91141', '5530'; 
GO
EXEC Insert_PostNrNaarGemeente '91142', '5540'; 
GO
EXEC Insert_PostNrNaarGemeente '91142', '5541'; 
GO
EXEC Insert_PostNrNaarGemeente '91142', '5542'; 
GO
EXEC Insert_PostNrNaarGemeente '91142', '5543'; 
GO
EXEC Insert_PostNrNaarGemeente '91142', '5544'; 
GO
EXEC Insert_PostNrNaarGemeente '91143', '5550'; 
GO
EXEC Insert_PostNrNaarGemeente '92003', '5300'; 
GO
EXEC Insert_PostNrNaarGemeente '92006', '5330'; 
GO
EXEC Insert_PostNrNaarGemeente '92006', '5332'; 
GO
EXEC Insert_PostNrNaarGemeente '92006', '5333'; 
GO
EXEC Insert_PostNrNaarGemeente '92006', '5334'; 
GO
EXEC Insert_PostNrNaarGemeente '92006', '5336'; 
GO
EXEC Insert_PostNrNaarGemeente '92035', '5310'; 
GO
EXEC Insert_PostNrNaarGemeente '92045', '5150'; 
GO
EXEC Insert_PostNrNaarGemeente '92048', '5070'; 
GO
EXEC Insert_PostNrNaarGemeente '92054', '5340'; 
GO
EXEC Insert_PostNrNaarGemeente '92087', '5640'; 
GO
EXEC Insert_PostNrNaarGemeente '92087', '5641'; 
GO
EXEC Insert_PostNrNaarGemeente '92087', '5644'; 
GO
EXEC Insert_PostNrNaarGemeente '92087', '5646'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5000'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5001'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5002'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5003'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5004'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5020'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5021'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5022'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5024'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5100'; 
GO
EXEC Insert_PostNrNaarGemeente '92094', '5101'; 
GO
EXEC Insert_PostNrNaarGemeente '92097', '5350'; 
GO
EXEC Insert_PostNrNaarGemeente '92097', '5351'; 
GO
EXEC Insert_PostNrNaarGemeente '92097', '5352'; 
GO
EXEC Insert_PostNrNaarGemeente '92097', '5353'; 
GO
EXEC Insert_PostNrNaarGemeente '92097', '5354'; 
GO
EXEC Insert_PostNrNaarGemeente '92101', '5170'; 
GO
EXEC Insert_PostNrNaarGemeente '92114', '5140'; 
GO
EXEC Insert_PostNrNaarGemeente '92137', '5060'; 
GO
EXEC Insert_PostNrNaarGemeente '92138', '5380'; 
GO
EXEC Insert_PostNrNaarGemeente '92140', '5190'; 
GO
EXEC Insert_PostNrNaarGemeente '92141', '5080'; 
GO
EXEC Insert_PostNrNaarGemeente '92141', '5081'; 
GO
EXEC Insert_PostNrNaarGemeente '92142', '5030'; 
GO
EXEC Insert_PostNrNaarGemeente '92142', '5031'; 
GO
EXEC Insert_PostNrNaarGemeente '92142', '5032'; 
GO
EXEC Insert_PostNrNaarGemeente '93010', '5630'; 
GO
EXEC Insert_PostNrNaarGemeente '93014', '5660'; 
GO
EXEC Insert_PostNrNaarGemeente '93018', '5680'; 
GO
EXEC Insert_PostNrNaarGemeente '93022', '5620'; 
GO
EXEC Insert_PostNrNaarGemeente '93056', '5600'; 
GO
EXEC Insert_PostNrNaarGemeente '93088', '5650'; 
GO
EXEC Insert_PostNrNaarGemeente '93088', '5651'; 
GO
EXEC Insert_PostNrNaarGemeente '93090', '5670'; 
GO
DROP PROCEDURE [dbo].[Insert_PostNrNaarGemeente]
GO