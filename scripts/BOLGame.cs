// DisplayName = Birth of Legends

//--- GAME RULES BEGIN ---
// Destroy the enemy base seed.
//--- GAME RULES END ---

//exec the AI scripts
exec("scripts/aiBOL.cs");

//-- tracking  ---
function BOLGame::initGameVars(%game)
{
    for (%i = 0; %i < %game.numTeams; %i++)
        %game.teamHasSeed[%i] = true;
}

package BOLGame
{
    function ShapeBaseData::onDestroyed(%data, %obj)
    {
        // Stub for interception of destruction of various assets
    }

    function BaseSeed::onDestroyed(%this, %obj)
    {
        parent::onDestroyed(%this, %obj);

        %game.teamHasSeed[%obj.team] = false;

        // TODO: Evaluate the losers for anything that could lead to a potential
        // recovery anyway.
        Game.gameOver();
    }
};

function BOLGame::getTeamSkin(%game, %team)
{
    if(isDemo() || $host::tournamentMode)
    {
        return $teamSkin[%team];
    }

    else
    {
    if(!$host::useCustomSkins)
    {
        %terrain = MissionGroup.musicTrack;
        //error("Terrain type is: " SPC %terrain);
        switch$(%terrain)
        {
            case "lush":
                if(%team == 1)
                    %skin = 'beagle';
                else if(%team == 2)
                    %skin = 'dsword';
                else %skin = 'base';

            case "badlands":
                if(%team == 1)
                    %skin = 'swolf';
                else if(%team == 2)
                    %skin = 'dsword';
                else %skin = 'base';

            case "ice":
                if(%team == 1)
                    %skin = 'swolf';
                else if(%team == 2)
                    %skin = 'beagle';
                else %skin = 'base';

            case "desert":
                if(%team == 1)
                    %skin = 'cotp';
                else if(%team == 2)
                    %skin = 'beagle';
                else %skin = 'base';

            case "Volcanic":
                if(%team == 1)
                    %skin = 'dsword';
                else if(%team == 2)
                    %skin = 'cotp';
                else %skin = 'base';

            default:
                if(%team == 2)
                    %skin = 'baseb';
                else %skin = 'base';
        }
    }
    else %skin = $teamSkin[%team];

    //error("%skin = " SPC getTaggedString(%skin));
    return %skin;
}
}

function BOLGame::getTeamName(%game, %team)
{
   if ( isDemo() || $host::tournamentMode)
       return $TeamName[%team];

   //error("BOLGame::getTeamName");
   if(!$host::useCustomSkins)
   {
      %terrain = MissionGroup.musicTrack;
      //error("Terrain type is: " SPC %terrain);
      switch$(%terrain)
      {
         case "lush":
            if(%team == 1)
               %name = 'Blood Eagle';
            else if(%team == 2)
               %name = 'Diamond Sword';

         case "badlands":
            if(%team == 1)
               %name = 'Starwolf';
            else if(%team == 2)
               %name = 'Diamond Sword';

            case "ice":
               if(%team == 1)
                  %name = 'Starwolf';
               else if(%team == 2)
                  %name = 'Blood Eagle';

            case "desert":
               if(%team == 1)
                  %name = 'Phoenix';
               else if(%team == 2)
                  %name = 'Blood Eagle';

            case "Volcanic":
               if(%team == 1)
                  %name = 'Diamond Sword';
               else if(%team == 2)
                  %name = 'Phoenix';

            default:
               if(%team == 2)
                  %name = 'Inferno';
               else
                  %name = 'Storm';
      }

      if(%name $= "")
      {
         //error("No team Name =============================");
         %name = $teamName[%team];
      }
   }
   else
     %name = $TeamName[%team];

   //error("%name = " SPC getTaggedString(%name));
   return %name;
}

//--------------------------------------------------------------------------
function BOLGame::missionLoadDone(%game)
{
   //default version sets up teams - must be called first...
   DefaultGame::missionLoadDone(%game);

   for(%i = 1; %i < (%game.numTeams + 1); %i++)
   {
      $teamScore[%i] = 0;

      // Also spawn the base seed for each team except 0
      %group = nameToID("Team" @ %i);
      %foundSpawn = -1;
      for (%s = 0; %s < %group.getCount(); %s++)
      {
          %obj = %group.getObject(%s);
          if (%obj.getName() $= "SeedSpawn")
          {
              %foundSpawn = %obj;
              break;
          }
      }

      if (isObject(%foundSpawn))
      {
        %seed = new HoverVehicle()
        {
            team = %i;
            datablock = "BaseSeed";
            position = %foundSpawn.getPosition();
        };
      }
      else // Otherwise we choose a spawn like normal and hope it works
      {

      }
   }

   // remove
   MissionGroup.clearFlagWaypoints();

   //reset some globals, just in case...
	$dontScoreTimer[1] = false;
	$dontScoreTimer[2] = false;
}

function BOLGame::timeLimitReached(%game)
{
   logEcho("game over (timelimit)");
   %game.gameOver();
   cycleMissions();
}

function BOLGame::scoreLimitReached(%game)
{
   logEcho("game over (scorelimit)");
   %game.gameOver();
   cycleMissions();
}

function BOLGame::gameOver(%game)
{
   //call the default
   DefaultGame::gameOver(%game);

   //send the winner message
   %winner = "";
   if ($teamScore[1] > $teamScore[2])
      %winner = %game.getTeamName(1);
   else if ($teamScore[2] > $teamScore[1])
      %winner = %game.getTeamName(2);

   if (%winner $= 'Storm')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.stowins.wav" );
   else if (%winner $= 'Inferno')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.infwins.wav" );
   else if (%winner $= 'Starwolf')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.swwin.wav" );
   else if (%winner $= 'Blood Eagle')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.bewin.wav" );
   else if (%winner $= 'Diamond Sword')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.dswin.wav" );
   else if (%winner $= 'Phoenix')
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.pxwin.wav" );
   else
      messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.gameover.wav" );

   messageAll('MsgClearObjHud', "");
   for(%i = 0; %i < ClientGroup.getCount(); %i ++)
   {
      %client = ClientGroup.getObject(%i);
      %game.resetScore(%client);
   }
   for(%j = 1; %j <= %game.numTeams; %j++)
      $TeamScore[%j] = 0;
}

function BOLGame::onClientDamaged(%game, %clVictim, %clAttacker, %damageType, %implement, %damageLoc)
{
   //the DefaultGame will set some vars
   DefaultGame::onClientDamaged(%game, %clVictim, %clAttacker, %damageType, %implement, %damageLoc);
}

function BOLGame::updateBaseInfo(%game, %client)
{
    if (%client.team == 0)
    {
        messageClient(%client, 'MsgSPCurrentObjective1', "", "Join a team for Resource Info.");
        messageClient(%client, 'MsgSPCurrentObjective2', "", "");
        return;
    }

    messageClient(%client, 'MsgSPCurrentObjective1', "", "Base Power: 0w    Soldiers: 0");
    messageClient(%client, 'MsgSPCurrentObjective2', "", "Scrap: 0");
}

////////////////////////////////////////////////////////////////////////////////////////
function BOLGame::clientMissionDropReady(%game, %client)
{
   messageClient(%client, 'MsgClientReady',"", "SinglePlayerGame");
   BOLGame::updateBaseInfo(%game, %client);

   // Ensure we're using the custom drawing GUI
   %client.useClientDrawGUI(true);

   %game.resetScore(%client);
   for(%i = 1; %i <= %game.numTeams; %i++)
   {
      $Teams[%i].score = 0;
      messageClient(%client, 'MsgCTFAddTeam', "", %i, %game.getTeamName(%i), $flagStatus[%i], $TeamScore[%i]);
   }

   messageClient(%client, 'MsgMissionDropInfo', '\c0You are in mission %1 (%2).', $MissionDisplayName, $MissionTypeDisplayName, $ServerName );
   DefaultGame::clientMissionDropReady(%game, %client);
}

function BOLGame::assignClientTeam(%game, %client, %respawn)
{
   DefaultGame::assignClientTeam(%game, %client, %respawn);
   // if player's team is not on top of objective hud, switch lines
   messageClient(%client, 'MsgCheckTeamLines', "", %client.team);

   BOLGame::updateBaseInfo(%game, %client);
}

function BOLGame::recalcScore(%game, %cl)
{
   %killValue = %cl.kills * %game.SCORE_PER_KILL;
   %deathValue = %cl.deaths * %game.SCORE_PER_DEATH;

   if (%killValue - %deathValue == 0)
      %killPoints = 0;
   else
      %killPoints = (%killValue * %killValue) / (%killValue - %deathValue);

   if(!isDemo())
   {
        %cl.offenseScore = %killPoints +
                        %cl.suicides            * %game.SCORE_PER_SUICIDE +
                        %cl.escortAssists       * %game.SCORE_PER_ESCORT_ASSIST +
                        %cl.teamKills           * %game.SCORE_PER_TEAMKILL +
                        %cl.scoreHeadshot           * %game.SCORE_PER_HEADSHOT +
                        %cl.flagCaps            * %game.SCORE_PER_PLYR_FLAG_CAP       +
                        %cl.flagGrabs           * %game.SCORE_PER_PLYR_FLAG_TOUCH       +
                        %cl.genDestroys         * %game.SCORE_PER_DESTROY_GEN         +
                        %cl.sensorDestroys     * %game.SCORE_PER_DESTROY_SENSOR      +
                        %cl.turretDestroys     * %game.SCORE_PER_DESTROY_TURRET      +
                        %cl.iStationDestroys   * %game.SCORE_PER_DESTROY_ISTATION    +
                        %cl.vstationDestroys   * %game.SCORE_PER_DESTROY_VSTATION    +
                        %cl.solarDestroys      * %game.SCORE_PER_DESTROY_SOLAR       +
                        %cl.sentryDestroys     * %game.SCORE_PER_DESTROY_SENTRY      +
                        %cl.depSensorDestroys  * %game.SCORE_PER_DESTROY_DEP_SENSOR  +
                        %cl.depTurretDestroys  * %game.SCORE_PER_DESTROY_DEP_TUR     +
                        %cl.depStationDestroys * %game.SCORE_PER_DESTROY_DEP_INV     +
                        %cl.vehicleScore + %cl.vehicleBonus;

        %cl.defenseScore =   %cl.genDefends          * %game.SCORE_PER_GEN_DEFEND +
                        %cl.flagDefends         * %game.SCORE_PER_FLAG_DEFEND +
                        %cl.carrierKills        * %game.SCORE_PER_CARRIER_KILL +
                        %cl.escortAssists       * %game.SCORE_PER_ESCORT_ASSIST +
                        %cl.turretKills         * %game.SCORE_PER_TURRET_KILL_AUTO +
                        %cl.mannedturretKills   * %game.SCORE_PER_TURRET_KILL +
                        %cl.genRepairs          * %game.SCORE_PER_REPAIR_GEN             +
                        %cl.SensorRepairs       * %game.SCORE_PER_REPAIR_SENSOR          +
                        %cl.TurretRepairs       * %game.SCORE_PER_REPAIR_TURRET          +
                        %cl.StationRepairs      * %game.SCORE_PER_REPAIR_ISTATION        +
                        %cl.VStationRepairs     * %game.SCORE_PER_REPAIR_VSTATION        +
                        %cl.solarRepairs        * %game.SCORE_PER_REPAIR_SOLAR           +
                        %cl.sentryRepairs       * %game.SCORE_PER_REPAIR_SENTRY          +
                        %cl.depInvRepairs       * %game.SCORE_PER_REPAIR_DEP_INV         +
                        %cl.depTurretRepairs    * %game.SCORE_PER_REPAIR_DEP_TUR  +
                        %cl.returnPts;
        }

    if( isDemo() )
    {
        %cl.offenseScore = %killPoints +
                            %cl.flagDefends         * %game.SCORE_PER_FLAG_DEFEND +
                            %cl.suicides * %game.SCORE_PER_SUICIDE + //-1
                            %cl.escortAssists * %game.SCORE_PER_ESCORT_ASSIST + // 1
                            %cl.teamKills * %game.SCORE_PER_TEAMKILL + // -1
                            %cl.flagCaps * %game.SCORE_PER_PLYR_FLAG_CAP + // 3
                            %cl.genDestroys * %game.SCORE_PER_GEN_DESTROY; // 2

        %cl.defenseScore =   %cl.genDefends * %game.SCORE_PER_GEN_DEFEND +   // 1
                            %cl.carrierKills * %game.SCORE_PER_CARRIER_KILL +  // 1
                            %cl.escortAssists * %game.SCORE_PER_ESCORT_ASSIST + // 1
                            %cl.turretKills * %game.SCORE_PER_TURRET_KILL +  // 1
                            %cl.flagReturns * %game.SCORE_PER_FLAG_RETURN +  // 1
                            %cl.genRepairs * %game.SCORE_PER_GEN_REPAIR;  // 1
    }

   %cl.score = mFloor(%cl.offenseScore + %cl.defenseScore);

   %game.recalcTeamRanks(%cl);
}

function BOLGame::updateKillScores(%game, %clVictim, %clKiller, %damageType, %implement)
{
   // console error message suppression
   if( isObject( %implement ) )
   {
      if( %implement.getDataBlock().getName() $= "AssaultPlasmaTurret" ||  %implement.getDataBlock().getName() $= "BomberTurret" ) // gunner
           %clKiller = %implement.vehicleMounted.getMountNodeObject(1).client;
      else if(%implement.getDataBlock().catagory $= "Vehicles") // pilot
           %clKiller = %implement.getMountNodeObject(0).client;
   }

   if(%game.testTurretKill(%implement))   //check for turretkill before awarded a non client points for a kill
      %game.awardScoreTurretKill(%clVictim, %implement);
   else if (%game.testKill(%clVictim, %clKiller)) //verify victim was an enemy
   {
      %value = %game.awardScoreKill(%clKiller);
      %game.shareScore(%clKiller, %value);
      %game.awardScoreDeath(%clVictim);

      if (%game.testGenDefend(%clVictim, %clKiller))
         %game.awardScoreGenDefend(%clKiller);

      if(%game.testCarrierKill(%clVictim, %clKiller))
         %game.awardScoreCarrierKill(%clKiller);
      else
      {
         if (%game.testFlagDefend(%clVictim, %clKiller))
            %game.awardScoreFlagDefend(%clKiller);
      }
      if (%game.testEscortAssist(%clVictim, %clKiller))
         %game.awardScoreEscortAssist(%clKiller);
   }
   else
   {
      if (%game.testSuicide(%clVictim, %clKiller, %damageType))  //otherwise test for suicide
      {
         %game.awardScoreSuicide(%clVictim);
      }
      else
      {
         if (%game.testTeamKill(%clVictim, %clKiller)) //otherwise test for a teamkill
            %game.awardScoreTeamKill(%clVictim, %clKiller);
      }
   }
}

function BOLGame::resetScore(%game, %client)
{
   %client.offenseScore = 0;
   %client.kills = 0;
   %client.deaths = 0;
   %client.suicides = 0;
   %client.escortAssists = 0;
   %client.teamKills = 0;
   %client.flagCaps = 0;
   %client.flagGrabs = 0;
   %client.genDestroys = 0;
   %client.sensorDestroys = 0;
   %client.turretDestroys = 0;
   %client.iStationDestroys = 0;
   %client.vstationDestroys = 0;
   %client.solarDestroys = 0;
   %client.sentryDestroys = 0;
   %client.depSensorDestroys = 0;
   %client.depTurretDestroys = 0;
   %client.depStationDestroys = 0;
   %client.vehicleScore = 0;
   %client.vehicleBonus = 0;

    %client.flagDefends = 0;
   %client.defenseScore = 0;
   %client.genDefends = 0;
   %client.carrierKills = 0;
   %client.escortAssists = 0;
   %client.turretKills = 0;
   %client.mannedTurretKills = 0;
   %client.flagReturns = 0;
   %client.genRepairs = 0;
    %client.SensorRepairs   =0;
    %client.TurretRepairs   =0;
    %client.StationRepairs  =0;
    %client.VStationRepairs =0;
    %client.solarRepairs    =0;
    %client.sentryRepairs   =0;
    %client.depInvRepairs   =0;
    %client.depTurretRepairs=0;
    %client.returnPts = 0;
   %client.score = 0;
}

function BOLGame::objectRepaired(%game, %obj, %objName)
{
   %item = %obj.getDataBlock().getName();
   %obj.wasDisabled = false;
}

function BOLGame::enterMissionArea(%game, %playerData, %player)
{
    if(%player.getState() $= "Dead")
    return;
    %player.client.outOfBounds = false;
   messageClient(%player.client, 'EnterMissionArea', '\c1You are back in the mission area.');
   logEcho(%player.client.nameBase@" (pl "@%player@"/cl "@%player.client@") entered mission area");

   //the instant a player leaves the mission boundary, the flag is dropped, and the return is scheduled...
   if (%player.holdingFlag > 0)
   {
      cancel($FlagReturnTimer[%player.holdingFlag]);
      $FlagReturnTimer[%player.holdingFlag] = "";
   }
}

function BOLGame::leaveMissionArea(%game, %playerData, %player)
{
    if(%player.getState() $= "Dead")
    return;
   // maybe we'll do this just in case
   %player.client.outOfBounds = true;
   // if the player is holding a flag, strip it and throw it back into the mission area
   // otherwise, just print a message
   if(%player.holdingFlag > 0)
      %game.boundaryLoseFlag(%player);
   else
      messageClient(%player.client, 'MsgLeaveMissionArea', '\c1You have left the mission area.~wfx/misc/warning_beep.wav');
   logEcho(%player.client.nameBase@" (pl "@%player@"/cl "@%player.client@") left mission area");
}

function BOLGame::applyConcussion(%game, %player)
{
   %game.dropFlag( %player );
}

function BOLGame::vehicleDestroyed(%game, %vehicle, %destroyer)
{
    //vehicle name
    %data = %vehicle.getDataBlock();
    //%vehicleType = getTaggedString(%data.targetNameTag) SPC getTaggedString(%data.targetTypeTag);
    %vehicleType = getTaggedString(%data.targetTypeTag);
    if(%vehicleType !$= "MPB")
        %vehicleType = strlwr(%vehicleType);

    %enemyTeam = ( %destroyer.team == 1 ) ? 2 : 1;

    %scorer = 0;
    %multiplier = 1;

    %passengers = 0;
    for(%i = 0; %i < %data.numMountPoints; %i++)
        if(%vehicle.getMountNodeObject(%i))
            %passengers++;

    //what destroyed this vehicle
    if(%destroyer.client)
    {
        //it was a player, or his mine, satchel, whatever...
        %destroyer = %destroyer.client;
        %scorer = %destroyer;

        // determine if the object used was a mine
        if(%vehicle.lastDamageType == $DamageType::Mine)
            %multiplier = 2;
    }
    else if(%destroyer.getClassName() $= "Turret")
    {
        if(%destroyer.getControllingClient())
        {
            //manned turret
            %destroyer = %destroyer.getControllingClient();
            %scorer = %destroyer;
        }
        else
        {
            %destroyerName = "A turret";
            %multiplier = 0;
        }
    }
    else if(%destroyer.getDataBlock().catagory $= "Vehicles")
    {
        // Vehicle vs vehicle kill!
        if(%name $= "BomberFlyer" || %name $= "AssaultVehicle")
            %gunnerNode = 1;
        else
            %gunnerNode = 0;

        if(%destroyer.getMountNodeObject(%gunnerNode))
        {
            %destroyer = %destroyer.getMountNodeObject(%gunnerNode).client;
            %scorer = %destroyer;
        }
        %multiplier = 3;
    }
    else  // Is there anything else we care about?
        return;


    if(%destroyerName $= "")
        %destroyerName = %destroyer.name;

    if(%vehicle.team == %destroyer.team) // team kill
    {
        %pref = (%vehicleType $= "Assault Tank") ? "an" : "a";
        messageAll( 'msgVehicleTeamDestroy', '\c0%1 TEAMKILLED %3 %2!', %destroyerName, %vehicleType, %pref);
    }

    else // legit kill
    {
        messageTeamExcept(%destroyer, 'msgVehicleDestroy', '\c0%1 destroyed an enemy %2.', %destroyerName, %vehicleType);
        messageTeam(%enemyTeam, 'msgVehicleDestroy', '\c0%1 destroyed your team\'s %2.', %destroyerName, %vehicleType);
        //messageClient(%destroyer, 'msgVehicleDestroy', '\c0You destroyed an enemy %1.', %vehicleType);
    }
}
