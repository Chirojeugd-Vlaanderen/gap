#!/bin/bash

echo "$ACCEPT_EULA
$SA_PASSWORD
$SA_PASSWORD
n
n" | /opt/mssql/bin/sqlservr-setup

