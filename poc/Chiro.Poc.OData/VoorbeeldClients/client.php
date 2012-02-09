<?php
// client.php - eenvoudig script dat de OData-service 'consumeert'
// In principe bestaat er een OData-SDK voor PHP, maar ik denk 
// dat die tamelijk omslachtig en hoogdrempelig is voor de gebruiker.

// DISCLAIMER: Ik heb al vrij lang niets serieus meer geprogrammeerd met PHP


// wijzig onderstaande, zodat het wijst naar een instantie van de service.
DEFINE('SERVICE_URL', 'http://lap-jve2.chiro.lokaal/ODataPoc/DataService.svc');


// get_data - haalt JSON-data op van de service, en parst het
// resultaat als php-object.
function get_data($url)
{
	$context = stream_context_create(
		array('http'=>array('header'=>'accept: application/json')));
	return json_decode(file_get_contents($url, false, $context))->d;
}

?>
<!DOCTYPE html>
<html lang="en">
	<head>
		<title>API-test</title>
	</head>

	<body>
	<table>

<?php
$leden = get_data(SERVICE_URL."/Leden");

foreach ($leden as $lid)
{
?> 
	<tr>
	<td><?=$lid->ID?></td>
	<td><?=$lid->Naam?></td>
	<td><?=$lid->Afdeling?></td>
	</tr>
<?php
}
?>
	</table>

	</body>
</html>
