$PDA::Page::Main = 0;

$PDA::Action::Close = 2;

$PDA::Action::BaseSeed::Deploy = 3;

function ShapeBase::renderStatus(%this, %obj, %client)
{
    // Team?
    %team = getTargetSensorGroup(%obj.getTarget());

    %relation = "<color:00FF00>Friendly";
    if (%team == 0)
        %relation = "<color:FFFFFF>Neutral";
    else if (%team != %client.team)
        %relation = "<color:FF0000>Hostile";

    %client.sendScoreLine("<just:center>Team Relation:" SPC %relation);

    // What is the integrity?
    %integrity = 100 - (%obj.getDamagePercent() * 100);

    %color = "00FF00";
    if (%integrity >= 50 && %integrity < 80)
        %color = "FFFF00";
    else if (%integrity < 50)
        %color = "FF0000";

    %client.sendScoreLine("<just:center>Hull Integrity: <color:" @ %color @ ">" @ %integrity @ "%");

    // Energy information
    %charge = %obj.getEnergyLevel();
    %maxCharge = %obj.getDatablock().maxEnergy;
    %chargeRate = %obj.getRechargeRate();
    %specChargeRate = %obj.getDatablock().rechargeRate;

    // Since this is in per tick (32ms), we convert it to seconds
    %chargeRateSec = %chargeRate * 31.25;
    %specChargeRateSec = %specChargeRate * 31.25;

    %client.sendScoreLine("<just:center>Capacitor Charge: " @ %obj.getEnergyLevel() @ "w /" SPC %maxCharge @ "w");

    %color = "<color:00FF00>";
    if (%chargeRateSec < %specChargeRateSec)
        %color = "<color:FF0000>";

    %client.sendScoreLine("<just:center>Capacitor Recharge: " @ %color @ %chargeRateSec SPC "w/sec out of" SPC %specChargeRateSec SPC "w/sec spec");
}

function VehicleData::renderStatus(%this, %obj, %client)
{
    ShapeBase::renderStatus(-1, %obj, %client);

    // Tell us who the occupant is
    %occupant = %obj.getControllingClient();
    if (!isObject(%occupant))
        %client.sendScoreLine("<just:center>Occupant: Nobody.");
    else
        %client.sendScoreLine("<just:center>Occupant: " @ %occupant.namebase);
}

function DeployedBaseSeed::renderStatus(%this, %obj, %client)
{
    ShapeBase::renderStatus(-1, %obj, %client);
}

function DeployedBaseSeed::renderActions(%this, %obj, %client)
{
    %team = getTargetSensorGroup(%obj.getTarget());

    // FIXME: Hacks regarding sending custom F2 commands when this menu is
    // displayed.
    if (%team != %client.team)
    {
        %client.sendScoreLine("<just:center>No actions available.");
        return;
    }

    %client.sendScoreLine("<just:center><a:gamelink\t" @ $PDA::Action::BaseSeed::Deploy @ "\t1>Undeploy Base Seed</a>");
}

function DeployedBaseSeed::processCommand(%this, %obj, %client, %command)
{
    switch(%command)
    {
        case $PDA::Action::BaseSeed::Deploy:
            BaseSeed.deploy(%obj.vehicle);
    }
}

function BaseSeed::renderActions(%this, %obj, %client, %command)
{
    %team = getTargetSensorGroup(%obj.getTarget());

    // FIXME: Hacks regarding sending custom F2 commands when this menu is
    // displayed.
    if (%team != %client.team)
    {
        %client.sendScoreLine("<just:center>No actions available.");
        return;
    }

    %client.sendScoreLine("<just:center><a:gamelink\t" @ $PDA::Action::BaseSeed::Deploy @ "\t1>Deploy Base Seed</a>");
}

function BaseSeed::processCommand(%this, %obj, %client, %command)
{
    %team = getTargetSensorGroup(%obj.getTarget());

    switch(%command)
    {
        case $PDA::Action::BaseSeed::Deploy:
            BaseSeed.deploy(%obj);
    }
}

function VehicleData::renderActions(%this, %obj, %client)
{
    %client.sendScoreLine("<just:center>No actions available.");
}

function GameConnection::setScoreHudSubHeader(%this, %value)
{
    messageClient(%this, 'SetScoreHudSubheader', "", %value);
}

function GameConnection::setScoreHudHeader(%this, %value)
{
    messageClient( %this, 'SetScoreHudHeader', "", %value);
}

function GameConnection::sendScoreLine(%this, %line)
{
    messageClient(%this, 'SetLineHud', "", 'scoreScreen', %this.lastScoreIndex, %line);
    %this.lastScoreIndex++;
}

function GameConnection::beginPage(%this)
{
    %this.lastScoreIndex = 0;
}

function GameConnection::clearScoreHud(%this)
{
    messageClient(%this, 'ClearHud', "", 'scoreScreen', 0 );
}

function GameConnection::closeScoreHud(%this)
{
    %this.PDAPage = $PDA::Page::Main;
    serverCmdHideHud(%this, 'scoreScreen');
    commandToClient(%this, 'DisplayHuds');
}

function BOLGame::updateScoreHud(%game, %client, %tag)
{
    if (%client.PDAPage == $PDA::Page::Main || %client.PDAPage == $PDA::Page::Interact)
        Game.processGameLink(%client, %client.PDAPage);
}

function BOLGame::processGameLink(%game, %client, %arg1, %arg2, %arg3, %arg4, %arg5)
{
    if (%arg1 != $PDA::Action::Close)
        %client.PDAPage = %arg1;

    %client.clearScoreHud();

    %client.beginPage();
    %client.SetScoreHudSubheader("<just:center>Object interaction menu.");
    %client.setScoreHudHeader("<just:center>Interaction Menu<just:right><a:gamelink\t" @ $PDA::Action::Close @ "\t1>Close</a>");

    if (!isObject(%client.player) || (%client.player.getState() !$= "move" && %client.player.getState() !$= "Mounted"))
    {
        %client.sendScoreLine("<just:center>You're dead, get out of here.");
        return;
    }

    switch (%arg1)
    {
        case $PDA::Action::Close:
            %client.closeScoreHud();
            return;

        case $PDA::Page::Main:
            // Drop a raycast from the player or their vehicle
            %mount = %client.player.getObjectMount();
            %controlled = isObject(%mount) ? %mount : %client.player;

            %start = %controlled.getMuzzlePoint($WeaponSlot);
            %dir = %controlled.getMuzzleVector($WeaponSlot);

            %dist = 200;
            %result = containerRayCast(%start, vectorAdd(%start, vectorScale(%dir, %dist)), $TypeMasks::StaticShapeObjectType | $TypeMasks::VehicleObjectType, -1);
            %resultPos = getWords(%result, 1, 3);
            %obj = getWord(%result, 0);

            if (!isObject(%obj) || (%obj.getType() & $TypeMasks::StaticShapeObjectType && !%obj.getDatablock().interactive))
            {
                %client.sendScoreLine("<just:center>There is nothing here. Try looking at something.");
                return;
            }

            %datablock = %obj.getDataBlock();
            %name = getTaggedString(getTargetName(%obj.getTarget()));
            %type = getTaggedString(getTargetType(%obj.getTarget()));

            %client.sendScoreLine("<just:center>-- " @ %name SPC %type SPC "--");
            %datablock.renderStatus(%obj, %client);

            %client.sendScoreLine("");
            %client.sendSCoreLine("<just:center>-- Actions --");
            %datablock.renderActions(%obj, %client);

            %client.interactedObject = %obj;
            return;
    }

    if (!isObject(%client.interactedObject))
    {
        BOLGame::processGameLink(%game, %client, $PDA::Page::Main);
        return;
    }

    %client.interactedObject.getDatablock().processCommand(%client.interactedObject, %client, %arg1);
    %client.interactedObject = -1;
    %client.closeScoreHud();

}
