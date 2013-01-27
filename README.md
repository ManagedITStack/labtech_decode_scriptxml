labtech_decode_scriptxml
========================

A command-line tool to decode packed Labtech script XML files.

NOTES:
==========

I am not affiliated with Labtech Software. Please use this software at your own risk. 
The unpacked scripts are not readable by Labtech and should not be distributed. This
tool is intended purely for educational purposes to enable study of the command mappings
of Labtech's exported XML scripts. Please publish any script step mappings that you learn
to Mappings.md in this repository.

Enjoy.

USAGE: 
===========

labtech_decode_scriptxml.exe
                 --input=input.xml
                 --output=output.xml
                [--overwrite=false|true]

EXIT CODES
===========
      0: Success
      1: ShowedHelp
      2: InputFileNotFound
      3: InputFileInvalid
      4: OutputFileExists
    100: UnknownError