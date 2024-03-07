@echo off

set param=%1

if [%param%]==[] (
    set /p param="Input:"
)    

CandidateGenerator.exe %param%
Draw.exe