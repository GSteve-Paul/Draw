@echo off

msbuild draw.proj
msbuild candidategenerator.proj
cd . >> ./bin/Release/candidate_data.txt
cd . >> ./bin/Release/prize_amount.txt
cd . >> ./bin/Release/prize_content.txt
cd . >> ./bin/Release/prize_name.txt
cd . >> ./bin/Release/Output.txt
copy src\runner.bat bin\Release\runner.bat