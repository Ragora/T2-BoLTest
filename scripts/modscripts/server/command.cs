function serverCmdRayAction(%client)
{
    if (!isObject(%client.player) || %client.player.getState() !$= "Move")
        return;

    %start = %client.player.getMuzzlePoint($WeaponSlot);
    %dir = %client.player.getMuzzleVector($WeaponSlot);

    %mountObject = %client.player.getObjectMount();

    %dist = 200;
    %result = containerRayCast(%start, vectorAdd(%start, vectorScale(%dir, %dist)), $TypeMasks::VehicleObjectType | $TypeMasks::TerrainObjectType, %mountObject);
    %resultPos = getWords(%result, 1, 3);
    %result = getWord(%result, 0);

    if (isObject(%result))
        // Struck Terrain
        if (%result.getType() & $TypeMasks::TerrainObjectType && isObject(%client.selection))
        {
            %controllingClient = %client.selection.getControllingClient();
            %controllingClient.setPilotDestination(%resultPos);

            %client.clearObjectBoundingBox(%client.selection);
            %client.deletePrimitive(%client.locationIndicator);

            %client.selection = 0;
        }
        // Struck a vehicle
        else if (%result.getType() & $TypeMasks::VehicleObjectType)
        {
            %controllingClient = %result.getControllingClient();
            if (!isObject(%controllingClient) || getTargetSensorGroup(%result.target) != %client.team || !%controllingClient.isAIControlled())
                return;

            %client.selection = %result;

            // Add a selection reticle on it
            if (%client.getAddress() !$= "local")
                %result = %client.getGhostIndex(%result);

            %client.drawObjectBoundingBox(%result, "0 255 0 255", 10);

            // Create a secondary reticle for use on the terrain and such.
            %client.locationIndicator = %client.setClientSphere(5, "0 0 0", "0 255 0 100", "0 255 0 100", 2);
            %client.bindPrimitiveToRaycast(%client.locationIndicator);
        }
}
