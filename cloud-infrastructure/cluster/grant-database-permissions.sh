RESOURCE_GROUP_NAME="$ENVIRONMENT-$LOCATION_PREFIX"
MANAGED_IDENTITY="$1-$RESOURCE_GROUP_NAME"
SQL_DATABASE=$1
SQL_SERVER="$SQL_SERVER_NAME.database.windows.net"

cd "$(dirname "${BASH_SOURCE[0]}")"
. ./firewall.sh open

echo Granting $MANAGED_IDENTITY permissions on $SQL_SERVER/$SQL_DATABASE database

# Execute the SQL script using mssql-scripter. Pass the script as a heredoc to sqlcmd to allow for complex SQL.
sqlcmd -S $SQL_SERVER -d $SQL_DATABASE --authentication-method=ActiveDirectoryDefault << EOF
IF NOT EXISTS (SELECT [name] FROM [sys].[database_principals] WHERE [name] = '$MANAGED_IDENTITY' AND [type] = 'E')
BEGIN
    CREATE USER [$MANAGED_IDENTITY] FROM EXTERNAL PROVIDER;
    ALTER ROLE db_datareader ADD MEMBER [$MANAGED_IDENTITY];
    ALTER ROLE db_datawriter ADD MEMBER [$MANAGED_IDENTITY];
    ALTER ROLE db_ddladmin ADD MEMBER [$MANAGED_IDENTITY];
END
GO
EOF

. ./firewall.sh close
