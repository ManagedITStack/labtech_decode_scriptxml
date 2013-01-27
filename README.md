labtech_decode_scriptxml
========================

A command-line tool to decode packed Labtech script XML files.

ABOUT:
==========

Use this to decode Labtech Packed XML script files to an XML file. 

USAGE: 
===========

labtech_decode_scriptxml.exe
                 --input=input.xml
                 --output=output.txt
                [--overwrite=false|true]

EXIT CODES
===========
      0: Success
      1: ShowedHelp
      2: InputFileNotFound
      3: InputFileInvalid
      4: OutputFileExists
    100: UnknownError