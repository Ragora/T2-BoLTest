function reload(%script)
{
	compile(%script); // Added by JackTL - Duh!!
    exec(%script);
    %count = ClientGroup.getCount();

    for(%i = 0; %i < %count; %i++)
    {
        %cl = ClientGroup.getObject(%i);

        if(!%cl.isAIControlled()) // no sending bots datablocks.. LOL
            %cl.transmitDataBlocks(0); // all clients on server
    }
}

exec("scripts/modscripts/shared/shared.cs");
exec("scripts/modscripts/server/helpers.cs");
exec("scripts/modscripts/server/scoremenu.cs");
exec("scripts/modscripts/server/command.cs");
exec("scripts/modscripts/server/power.cs");
exec("scripts/modscripts/server/aipilot.cs");
exec("scripts/modscripts/server/repulsor.cs");
