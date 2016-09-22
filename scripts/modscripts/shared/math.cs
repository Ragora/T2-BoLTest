function getWordMax(%sequence)
{
    %result = getWord(%sequence, 0);
    for (%iteration = 1; %iteration < getWordCount(%sequence); %iteration++)
    {
        %current = getWord(%sequence, %iteration);
        %result = %current > %result ? %current : %result;
    }

    return %result;
}

function getWordMin(%sequence)
{
    %result = getWord(%sequence, 0);
    for (%iteration = 1; %iteration < getWordCount(%sequence); %iteration++)
    {
        %current = getWord(%sequence, %iteration);
        %result = %current < %result ? %current : %result;
    }

    return %result;
}

function vectorRandomDir()
{
    %result = getRandom(-9999, 9999) SPC getRandom(-9999, 9999) SPC getRandom(-9999, 9999);
    return vectorNormalize(%result);
}

function vectorRandom(%minDist, %maxDist, %rel)
{
    %dir = vectorRandomDir();
    %mag = getRandom(%minDist, %maxDist);
    return vectorAdd(vectorScale(%dir, %mag), %rel);
}

function vectorRandomOnTerrain(%minDist, %maxDist, %rel)
{
    %result = vectorRandom(%minDist, %maxDist, %rel);
    return setWord(%result, 2, getTerrainHeight(%result));
}

function vectorRandomOn(%minDist, %maxDist, %rel, %mask)
{
    %end = vectorRandomOnTerrain(%minDist, %maxDist, %rel);

    // Just assume 1000m above terrain is good enough
    %start = vectorAdd(%end, "0 0 1000");

    %ray = ContainerRayCast(%start, %end, %mask, -1);
    %rayHit = getWord(%ray);

    if (isObject(%rayHit))
        return getWords(%ray, 1, 3);

    // If we hit nothing, just return the end
    return %end;
}

function boxContains(%box, %point)
{
    %pX = getWord(%point, 0);
    %pY = getWord(%point, 1);
    %pZ = getWord(%point, 2);

    return %pX >= getWord(%box, 0) && %pX <= getWord(%box, 3) &&
    %pY >= getWord(%box, 1) && %pY <= getWord(%box, 4) &&
    %pZ >= getWord(%box, 2) && %pZ <= getWord(%box, 5);
}

function _boxIntersects(%box1, %box2)
{
    // These are diagonally across from each other
    %b2lowerRearLeftPoint = getWords(%box2, 0, 2);
    %b2upperFrontRightPoint = getWords(%box2, 3, 5);

    %b2upperFrontRightX = getWord(%b2upperFrontRightPoint, 0);
    %b2upperFrontRightY = getWord(%b2upperFrontRightPoint, 1);
    %b2upperFrontRightZ = getWord(%b2upperFrontRightPoint, 2);
    %b2lowerRearLeftX = getWord(%b2lowerRearLeftPoint, 0);
    %b2lowerRearLeftY = getWord(%b2lowerRearLeftPoint, 1);
    %b2lowerRearLeftZ = getWord(%b2lowerRearLeftPoint, 2);

    // Calculate remaining rear points
    %b2lowerRearRightPoint = %b2upperFrontRightX SPC %b2lowerRearLeftY SPC %b2lowerRearLeftZ;
    %b2upperRearRightPoint = %b2upperFrontRightX SPC %b2lowerRearLeftY SPC %b2upperFrontRightZ;
    %b2upperRearLeftPoint = %b2lowerRearLeftX SPC %b2lowerRearLeftY SPC %b2upperFrontRightZ;

    // Calculate remaining front points
    %b2upperFrontLeftPoint = %b2lowerRearLeftX SPC %b2upperFrontRightY SPC %b2upperFrontRightZ;
    %b2lowerFrontRightPoint = %b2upperFrontRightX SPC %b2upperFrontRightY SPC %b2lowerRearLeftZ;
    %b2lowerFrontLeftPoint = %b2lowerRearLeftX SPC %b2upperFrontRightY SPC %b2lowerRearLeftZ;

    // End this madness...
    return boxContains(%box1, %b2lowerRearLeftPoint) || boxContains(%box1, %b2upperFrontRightPoint) ||
    boxContains(%box1, %b2lowerRearRightPoint) || boxContains(%box1, %b2upperRearRightPoint) ||
    boxContains(%box1, %b2upperRearLeftPoint) || boxContains(%box1, %b2upperFrontLeftPoint) ||
    boxContains(%box1, %b2lowerFrontRightPoint) || boxContains(%box1, %b2lowerFrontLeftPoint);
}

function boxIntersects(%box1, %box2)
{
    return _boxIntersects(%box1, %box2) || _boxIntersects(%box2, %box1);
}
