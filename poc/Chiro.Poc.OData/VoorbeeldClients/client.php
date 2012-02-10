<?php
// client.php - eenvoudig script dat de OData-service 'consumeert'
// In principe bestaat er een OData-SDK voor PHP, maar ik denk 
// dat die tamelijk omslachtig en hoogdrempelig is voor de gebruiker.

// DISCLAIMER: Ik heb al vrij lang niets serieus meer geprogrammeerd met PHP


// wijzig onderstaande, zodat het wijst naar een instantie van de service.
DEFINE('SERVICE_URL', 'http://lap-jve2.chiro.lokaal/ODataPoc/DataService.svc');

// gebruikersnaam en wachtwoord voor basic authenticatie.  
// Gebruik de lege string als USER als er geen authenticatie nodig is.
DEFINE('USER','apitest');
DEFINE('PASS','PubliekGeheim.');


// get_data - haalt JSON-data op van de service, en parst het
// resultaat als php-object.
function get_data($url)
{
	$header="accept: application/json\r\n";
	if (USER != '')
	{
		$header .= 'Authorization: Basic '.base64_encode(USER.':'.PASS);
	}

	$context = stream_context_create(
		array('http'=>array(
			'header'=>"accept: application/json\r\n".
			          'Authorization: Basic '.base64_encode(USER.':'.PASS))));

	$result=file_get_contents(SERVICE_URL."\\$url", false, $context);
	return json_decode($result)->d;
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
$leden = get_data("Leden");

foreach ($leden->results as $lid)
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
