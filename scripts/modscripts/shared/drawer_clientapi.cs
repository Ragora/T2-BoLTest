//--------------------------------------------------------------------------------------------------
//  Clientside Drawer API
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================

$ClientDrawer::ClientAPI::MaxDrawers = 100;

function ClientDrawer::reportError(%key, %function, %message)
{
    if (isObject(ServerConnection))
    {
        commandToServer('ClientDrawerError', %key, %message);
        error("!!! ClientDrawer (" @ %function @  "): " @ %message);
    }

    return -1;
}

function ClientDrawer::deletePrimitive(%id, %key)
{
    if (!isObject(MissionGroup))
        return ClientDrawer::reportError(%key, "::deletePrimitive", "MissionGroup does not exist.");

    %removed = ClientDrawer::lookupPrimitive(%id, %key);

    if (isObject(%removed))
    {
        MissionGroup.remove(%removed);
        MissionGroup.drawerArray.remove(%removed);
        %removed.delete();
    }
    else
        return ClientDrawer::reportError(%key, "::deletePrimitive", "No such primitive by id" SPC %id);
}

function ClientDrawer::lookupPrimitive(%id, %key)
{
    if (!isObject(MissionGroup))
        return ClientDrawer::reportError(%key, "::lookupPrimitive", "MissionGroup does not exist.");

    %result = MissionGroup.drawerIndexMap[%id];
    if (!isObject(%result))
        return ClientDrawer::reportError(%key, "::lookupPrimitive", "No such drawer by id" SPC %id);

    return %result;
}

function ClientDrawer::checkMissionGroup()
{
    if (!isObject(MissionGroup))
    {
        new SimGroup(MissionGroup);
        MissionGroup.nextDrawerID = 0;
        MissionGroup.drawerArray = Array.create();
    }

    if (!isObject(MissionGroup.drawerArray))
        MissionGroup.drawerArray = Array.create();
}

function ClientDrawer::registerDrawer(%obj)
{
    ClientDrawer::checkMissionGroup();

    if (MissionGroup.drawerArray.getCount() >= $ClientDrawer::ClientAPI::MaxDrawers)
        return ClientDrawer::reportError(%key, "::registerDrawer", "Too many active primitives. Max:" SPC $ClientDrawer::ClientAPI::MaxDrawers);

    MissionGroup.add(%obj);

    MissionGroup.drawerIndexMap[MissionGroup.nextDrawerID] = %obj;
    MissionGroup.nextDrawerID++;
    MissionGroup.drawerArray.add(%obj);
}

function ClientDrawer::obtainObjectID(%ghostIndex, %key)
{
    if (ServerConnection.getAddress() !$= "local")
        if ($TSExtension::isActive)
            %obj = ServerConnection.resolveGhost(%ghostIndex);
        else
            return ClientDrawer::reportError(%key, "::obtainObjectID", "TSEXtension is not running or out of date.");
    else // If the server connection address is local, we're running in a listen server
        return %ghostIndex;

    // First off, can we even see the suggested ghost?
    if (!isObject(%obj))
        return ClientDrawer::reportError(%key, "::obtainObjectID", "No such ghost index " @ %ghostIndex);

    // Can we access it?
    %position = %obj.getWorldBoxCenter();
    if (%position $= "")
        return ClientDrawer::reportError(%key, "::obtainObjectID", "Cannot access object " @ %obj SPC ", ghost index " @ %ghostIndex @ ". SI mempatches are probably not active.");

    return %obj;
}
