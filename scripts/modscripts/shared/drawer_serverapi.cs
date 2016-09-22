//--------------------------------------------------------------------------------------------------
//  Serverside Client Drawer API
//  All functions declared in this file are intended to be used directly by programmings wanting
//  to utilize the library to its full potential. The library wraps up the inconsistencies between
//  a listen server client and a remote game client as well as creates a wrapper for the counting
//  ID system that the clients implement for deterministic draw entity labeling.
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================

// Time in milliseconds taken to expire the error report window on the client.
$ClientDrawer::ServerAPI::CommandTimerMS = 128;

//--------------------------------------------------------------------------------------------------
//  Command used to trigger the client draw GUI for the given client. By default, clients do not
//  enable the client draw as it has very minor effects on other game UI elements therefore you
//  should only enable it when necessary.
//
//  Parameters:
//      %this - The client in question we are targetting.
//      %val - A boolean representing whether or not this client should use the client draw GUI.
//
//  Effects:
//      When called, %val is assigned to %this.usingClientDrawGUI which signifies whether or not the
//      client is currently using the client draw GUI.
//==================================================================================================
function GameConnection::useClientDrawGUI(%this, %val)
{
    %this.usingClientDrawGUI = %val;

    if (%val)
        commandToClient(%this, 'UseClientDrawGUI');
    else
        commandToClient(%this, 'UseRegularPlayGUI');
}

//--------------------------------------------------------------------------------------------------
//  Command used to instantiate a new sphere or alter the properties of an existing one if %id
//  is specified.
//
//  Parameters:
//      %this - The target client in question.
//      %radius - The radius of the sphere.
//      %position - The position of the sphere.
//      %lineColor - What color to use when drawing the lines in the form of "R G B A" where each
//      component is an unsigned byte 0 - 255.
//      %fillColor - What color to use when filling in the triangles formed by every line in the form
//      of "R G B A" where each component is an unsigned byte 0 - 255.
//      %tessellation - Controls the amount of polygons in the sphere. The bigger this number is,
//      the more triangles that are used.
//      %id - Optional parameter, if specified and it corresponds to a previously created sphere,
//      all previous parameters are used to alter in instead of creating a new one.
//
//  Return:
//      The ID of the client drawer created if %id is not specified. If %id is specified, then -1
//      is returned.
//==================================================================================================
function GameConnection::setClientSphere(%this, %radius, %position, %lineColor, %fillColor, %tessellation, %id)
{
    %assignedID = %this._incrementClientDrawIndex(%id);
    commandToClient(%this, 'SetClientSphere', %radius, %position, %lineColor, %fillColor, %tessellation, %id, %this._registerInvokedCommand("setClientSphere"));
    return %assignedID;
}

//--------------------------------------------------------------------------------------------------
//  Command used to instantiate a new circle or alter the properties of an existing one if %id
//  is specified.
//
//  Parameters:
//      %this - The target client in question.
//      %radius - The radius of the circle.
//      %position - The position of the circle.
//      %normal - A normalized vector representing the direction the circle is facing.
//      %lineColor - What color to use when drawing the lines in the form of "R G B A" where each
//      component is an unsigned byte 0 - 255.
//      %fillColor - What color to use when filling in the triangles formed by every line in the form
//      of "R G B A" where each component is an unsigned byte 0 - 255.
//      %segments - Controls the amount of lines to draw the circle. This has a minimum value of 6
//      as anything lower is liable to crash the running game client.
//      %id - Optional parameter, if specified and it corresponds to a previously created circle,
//      all previous parameters are used to alter in instead of creating a new one.
//
//  Return:
//      The ID of the client drawer created if %id is not specified. If %id is specified, then -1
//      is returned.
//==================================================================================================
function GameConnection::setClientCircle(%this, %radius, %normal, %position, %lineColor, %fillColor, %segments, %id)
{
    %assignedID = %this._incrementClientDrawIndex(%id);
    commandToClient(%this, 'SetClientCircle', %radius, %normal, %position, %lineColor, %fillColor, %segments, %id, %this._registerInvokedCommand("setClientCircle"));
    return %assignedID;
}

//--------------------------------------------------------------------------------------------------
//  Command used to instantiate a new point trangle or alter the properties of an existing one if %id
//  is specified.
//
//  Parameters:
//      %this - The target client in question.
//      %p1 - A vector representing the first point in the triangle.
//      %p2 - A vector representing the second point in the triangle.
//      %p3 - A vector representing the third point in the triangle.
//      %fillColor - What color to use when filling in the triangles formed by every line in the form
//      of "R G B A" where each component is an unsigned byte 0 - 255.
//      %id - Optional parameter, if specified and it corresponds to a previously created point
//      triangle, all previous parameters are used to alter in instead of creating a new one.
//
//  Return:
//      The ID of the client drawer created if %id is not specified. If %id is specified, then -1
//      is returned.
//==================================================================================================
function GameConnection::setClientPointTriangle(%this, %p1, %p2, %p3, %lineColor, %fillColor, %id)
{
    %assignedID = %this._incrementClientDrawIndex(%id);
    commandToClient(%this, 'SetClientPointTriangle', %p1, %p2, %p3, %lineColor, %fillColor, %id, %this._registerInvokedCommand("setClientPointTriangle"));
    return %assignedID;
}

//---------------------------------------------------------------------------------------------------
//  Command used to instantiate a new line or alter the properties of an existing one if %id
//  is specified.
//
//  Parameters:
//      %this - The target client in question.
//      %start - A vector representing the starting point of the line.
//      %end - A vector representing the ending point of the line.
//      %lineColor - What color to use when drawing the lines in the form of "R G B A" where each
//      component is an unsigned byte 0 - 255.
//      %id - Optional parameter, if specified and it corresponds to a previously created line,
//      all previous parameters are used to alter in instead of creating a new one.
//
//  Return:
//      The ID of the client drawer created if %id is not specified. If %id is specified, then -1
//      is returned.
//==================================================================================================
function GameConnection::setClientLine(%this, %start, %end, %width, %lineColor, %id)
{
    %assignedID = %this._incrementClientDrawIndex(%id);
    commandToClient(%this, 'SetClientLine', %start, %end, %width, %lineColor, %id, %this._registerInvokedCommand("setClientLine"));
    return %assignedID;
}

//--------------------------------------------------------------------------------------------------
//  Command used to bind a rounded (sphere, circle) drawer to a object in the server game
//  world. This should be used in preference to directly using the BindRounded client
//  command because this will resolve the client's ghost ID for you if the server is
//  TSExtension enabled.
//
//  Parameters:
//      %roundedID - The ID of the rounded (sphere, circle) drawer ID to bind to some
//      arbitrary object.
//      %object - The server game world object to bind the specified rounded drawer
//      id against on the client.
//==================================================================================================
function GameConnection::bindPrimitive(%this, %roundedID, %obj)
{
    // NOTE: Do we always want this?
    if (%this.getAddress() !$= "local")
    {
        %obj.scopeAlways();
        %obj.scopeToClient(%this);
    }

    commandToClient(%this, 'BindRounded', %roundedID, %this.getGhostIndex(%obj), %this._registerInvokedCommand("bindRounded"));
}

function GameConnection::unbindPrimitive(%this, %id)
{
    commandToClient(%this, 'UnbindPrimitive', %id, %this._registerInvokedCommand("unbindPrimitive"));
}

function GameConnection::bindPrimitiveToRaycast(%this, %id)
{
    commandToClient(%this, 'BindPrimitiveToRaycast', %id, %this._registerInvokedCommand("bindPrimitiveToRaycast"));
}

//---------------------------------------------------------------------------------------------------
//  Command used to instruct a running game client to draw the bounding box of an arbitrary object
//  in the game world. If this function is used where %obj already has a bounding box draw in place,
//  then the parameters of the draw are merely updated with the new %lineColor and %lineWidth.
//
//  Parameters:
//      %this - The target client in question.
//      %obj - The ID of the object in question.
//      %lineColor - What color to use when drawing the lines in the form of "R G B A" where each
//      component is an unsigned byte 0 - 255.
//      %width - The width of the lines drawn.
//
//  Return:
//      The ID of the client drawer created if %id is not specified. If %id is specified, then -1
//      is returned.
//==================================================================================================
function GameConnection::drawObjectBoundingBox(%this, %obj, %lineColor, %lineWidth)
{
    commandToClient(%this, 'DrawObjectBoundingBox', %this.getGhostIndex(%obj), %lineColor, %lineWidth, %this._registerInvokedCommand("drawObjectBoundingBox"));
}

function GameConnection::clearPrimitives(%this)
{
    commandToClient(%this, 'ClearPrimitives', %this._registerInvokedCommand("clearPrimitives"));
}

function GameConnection::clearObjectBoundingBox(%this, %obj)
{
    commandToClient(%this, 'ClearObjectBoundingBox', %this.getGhostIndex(%obj), %this._registerInvokedCommand("clearObjectBoundingBox"));
}

function GameConnection::deletePrimitive(%this, %id)
{
    commandToClient(%this, 'DeletePrimitive', %id,  %this._registerInvokedCommand("deletePrimitive"));
}

function serverCmdClientDrawerError(%client, %key, %description)
{
    %maxDelta = %client.getPing();
    %keyDelta = mAbs(getRealTime() - %client.keyTimeMap[%key]);

    if (%client.getAddress() $= "local")
        %maxDelta = 9999; // You're the server owner.
    else
        %maxDelta = %maxDelta > $ClientDrawer::ServerAPI::CommandTimerMS ? $ClientDrawer::ServerAPI::CommandTimerMS : %maxDelta;

    if (%keyDelta <= %maxDelta)
    {
        %command = %client.keyNameMap[%key];
        %description = getWords(%description, 0, 32);

        error("!!! ClientDrawer (serverCmdClientDrawerError): Client " SPC %client.namebase SPC "(" @ %client @ ") reports that command '" @ %command @ "' failed! Reason: '" @ %description @ "'");
    }
}

function GameConnection::_registerInvokedCommand(%this, %name)
{
    %key = strRandomString();
    %this.keyTimeMap[%key] = getRealTime();
    %this.keyNameMap[%key] = %name;
    return %key;
}

function GameConnection::_incrementClientDrawIndex(%this, %id)
{
    // Only do the increments if we're requesting a new one
    if (%id $= "")
    {
        %assignedID = %this.currentClientDrawIndex;
        if (%assignedID $= "")
            %assignedID = 0;

        %this.currentClientDrawIndex++;
        return %assignedID;
    }

    return -1;
}
