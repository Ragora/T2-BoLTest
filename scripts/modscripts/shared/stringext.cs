//--------------------------------------------------------------------------------------------------
//  String helper functions.
//  This file contains various string helper functions that the clientside drawing system utilizes
//  in its operations.
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================

function strHash(%string)
{
    %file = new FileObject();
    %file.OpenForWrite("hash_temp.txt");
    %file.writeLine(%string);
    %file.close();

    %result = getFileCRC("hash_temp.txt");
    deleteFile("hash_temp");
    return %result;
}

$StringExt::DefaultAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
function strRandomString(%minLength, %laxLength, %alphabet)
{
    if (%minLenth $= "")
        %minLength = 5;
    if (%maxLength $= "")
        %maxLength = 6;
    if (%alphabet $= "")
        %alphabet = $StringExt::DefaultAlphabet;

    %length = getRandom(%minLength, %maLength);
    %result = getWord(%alphabet, getRandom(0, strLen(%alphabet)));
    for (%iteration = 1; %iteration < %length; %iteration++)
        %result = %result @ getWord(%alphabet, getRandom(0, strLen(%alphabet)));

    return %result;
}
