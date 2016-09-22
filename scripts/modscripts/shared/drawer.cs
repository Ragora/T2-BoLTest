//--------------------------------------------------------------------------------------------------
//  clientDraw.cs
//
//  Clientside, server-controlled geometry drawing implementation using the PlayGUI hack.
//
//  This works with a system of client commands that may be issued by the server end:
//      clientCmdSetClientSphere(%radius, %position, %lineColor, %fillColor, %tessellation [, %id])
//      clientCmdSetClientCircle(%radius, %normal, %position, %lineColor, %fillColor, %segments [, %id])
//      clientCmdSetClientPointTriangle(%p1, %p2, %p3, %lineColor, %fillColor [, %id])
//      clientCmdSetClientLine(%start, %end, %width, %lineColor [, %id])
//      clientCmdRemoveClientDrawer(%id)
//      clientCmdClearDrawers()
//
//  There is one exception to this which is a function to bind circles and spheres to clientside entities.
//  Note, however, that is requires the latest TSExtension to be active on both the client and server ends
//  to work.
//      GameConnection::bindRounded(%this, %roundedID, %object)
//
//  All of the above set functions will automatically execute on the client end and no response is
//  returned from the client to server because the id generated on the client end is easily computed
//  by the server end: ID's will always start numbering at 1 and will only increment regardless until
//  mission switches or if the client just connected to your server. Furthermore, the client will
//  use the regular PlayGUI by default upon connecting to your server and between mission switches
//  until told otherwise:
//      clientCmdUseClientDrawGUI()
//      clientCmdUseRegularPlayGUI()
//
//  The optional %id parameter in all client commands is used to specify an ID of an existing client
//  render object to modify, which means that instead of creating a new one, the client will simply
//  apply all parameters before the %id parameter to the specified object. These identifiers are typed
//  based on the client command originally used to create them, so you must use the appropriate client
//  command to perform modifications. Otherwise, your request will be a no-op with an error on the client.
//
//  This software is licensed under the MIT license. See LICENSE.txt for more information.
//  Copyright (c) 2015 Robert MacGregor
//==================================================================================================

// Execute these for both client and server
exec("stringext.cs");
exec("extendedcontainer.cs");
exec("array.cs");

// Execute for server only, though it shouldn't really matter
exec("drawer_serverapi.cs");

// Execute for client only, though it shouldn't really matter
exec("drawer_clientside.cs");
exec("drawer_clientprotocol.cs");
exec("drawer_clientapi.cs");

// Only activate the package if necessary.
if (!isActivePackage(ClientHooks))
    activatePackage(ClientHooks);
if (!isActivePackage(DrawSubsystemHooks))
    activatePackage(DrawSubsystemHooks);

// Print an error if TSExtension is inaccessible.
if (!$TSExtension::isActive)
    error("!!! ClientDraw: TSExtension is required for some portions of the ClientDraw script to work.");
