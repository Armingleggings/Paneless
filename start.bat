@echo off
REM Activate the virtual environment
cd venv
call "Scripts\activate"

REM Set the FLASK_APP environment variable
set FLASK_APP=../paneless

REM Run the Flask application
flask run