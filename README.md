labtech_decode_scriptxml
========================

A command-line tool to decode packed Labtech script XML files and to extract embedded files.

DOWNLOAD:
==========

Download from [SourceForge](https://sourceforge.net/projects/ltdecodescpxml/files/labtech_decode_scriptxml.exe/download).

NOTES:
==========

I am not affiliated with Labtech Software. Please use this software at your own risk. 
The unpacked scripts are not readable by Labtech and should not be distributed. This
tool is intended purely for educational purposes to enable study of the command mappings
of Labtech's exported XML scripts. Please publish any script step mappings that you learn
to Mappings.md in this repository.

Enjoy.

EXAMPLE:
===========

For an example of both a packed and an unpacked Labtech script. See [CGauss' Current Date Script](https://github.com/ManagedITStack/labtech_create_current_date_variable_script)

The .xml file is the packed file, the .unpacked.xml is the unpacked file. The files extracted during the unpacking process will be extracted to the Transfer directory. Once unpacked, the embedded file is removed from the unpacked XML file.

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
