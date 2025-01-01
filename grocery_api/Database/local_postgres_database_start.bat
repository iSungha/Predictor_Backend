echo off

REM Script to create local postgres database

REM Tests done: 
REM		1. if postgres image not pulled, it will pull then create the Container
REM     2. if container is already up, it does nothing
REM     3. if container is down, but exists, it starts it up
REM     4. if container does not exist, it creates it

set CONTAINER_NAME=Grocery_postgresDB
set CONTAINER_pwd=admin
set CONTAINER_user=superuser
set CONTAINER_dbname=GroceryDB
set CONTAINER_host=GroceryDBHost
set CONTAINER_port=5432
set CONTAINER_baseImg=postgres

REM local variables need to be reset incase we run commands in the same shell
set containerId=
set containerIdAll=

REM local function gets status of container
REM check if container is running
for /f %%i in ('docker ps -qf "name=^%CONTAINER_NAME%"') do set containerId=%%i	
REM check if container exists
for /f %%i in ('docker ps -aqf "name=^%CONTAINER_NAME%"') do set containerIdAll=%%i		

REM Check if container is running
If "%containerId%" NEQ "" (

	echo Container %CONTAINER_NAME% is running already - %containerId%	

) else (

	echo Container %CONTAINER_NAME% is not running.

	REM Check if container exists but not running
	If "%containerIdAll%" EQU "" (

		echo Container %CONTAINER_NAME% does not exist. Creating and starting container...

		docker run -d --name %CONTAINER_NAME% -e POSTGRES_PASSWORD=%CONTAINER_pwd% -e POSTGRES_USER=%CONTAINER_user% -e POSTGRES_DB=%CONTAINER_dbname% -e POSTGRES_HOST=%CONTAINER_host% -p %CONTAINER_port%:%CONTAINER_port% %CONTAINER_baseImg%

	) else (
		echo Container %CONTAINER_NAME% exists already. Starting container... - %containerIdAll%	

		docker start %CONTAINER_NAME%
	)

)