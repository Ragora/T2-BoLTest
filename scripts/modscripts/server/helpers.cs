function ShapeBase::getWorldBoxSize(%this)
{
    %box = %this.getWorldBox();

    %lowerRearLeftPoint = getWords(%box, 0, 2);
    %upperFrontRightPoint = getWords(%box, 3, 5);

    return getWord(%upperFrontRightPoint, 0) - getWord(%lowerRearLeftPoint, 0) SPC
    getWord(%upperFrontRightPoint, 1) - getWord(%lowerRearLeftPoint, 1) SPC
    getWord(%upperFrontRightPoint, 2) - getWord(%lowerRearLeftPoint, 2);
}

function ShapeBase::worldBoxIntersects(%this, %test)
{
    if (!isObject(%test))
        return -1;

    return boxIntersects(%this.getWorldBox(), %test.getWorldBox());
}

function ShapeBase::getRotation(%this)
{
    return getWords(%this.getTransform(), 3, 6);
}

function ShapeBase::getPosition(%this)
{
    return getWords(%this.getTransform(), 0, 2);
}

function ShapeBase::setPosition(%this, %pos)
{
    %this.setTransform(%pos SPC %this.getRotation());
}

function ShapeBase::setRotation(%this, %rot)
{
    %this.setTransform(%this.getPosition() SPC %rot);
}
