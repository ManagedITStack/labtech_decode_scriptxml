labtech_decode_scriptxml
========================

A command-line tool to decode packed Labtech script XML files.

ABOUT:
==========

Use this to decrypt Labtech Packed
XML files to a json file.

USAGE: 
===========

DecryptLTXML.exe --input=input.xml
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