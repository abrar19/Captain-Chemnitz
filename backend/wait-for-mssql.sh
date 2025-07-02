# wait-for-sql.sh
#!/bin/bash
echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd -S mssql_db,1433 -U sa -P 'Passw0rd' -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 1
done
echo "SQL Server is ready!"
exec "$@"