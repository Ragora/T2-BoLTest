package AIPilot
{
    function AIConnection::setPilotDestination(%this, %target)
    {
        %this.pilotDestination = %target;
        parent::setPilotDestination(%this, %target);
    }

    function AIConnection::setControlObject(%this, %object)
    {
        if (%obj.getType() & $TypeMasks::VehicleObjectType)
        {
            %this.clearStep();
            %this.clearTasks();

            // TODO: Does the task subsystem even run on AI's using vehicles?
            %this.addTask(AIPilotTask);
            %this.pilottedVehicle = %object;
        }
        else
        {
            %this.pilottedVehicle = -1;
        }

        parent::setControlObject(%this, %object);
    }
};


function AIPilotTask::assume(%task, %client)
{

}

function AIPilotTask::weight(%task, %client)
{
    if (vectorDist(%client.pilottedVehicle.getWorldBoxCenter(), %client.pilotDestination) < 5)
    {
        echo("WAT");
    }
}

function AIPilotTask::monitor(%task, %client)
{

}


if (!isActivePackage(AIPilot))
{
    activatePackage(AIPilot);
    deactivatePackage(AIPilot);
}
