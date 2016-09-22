//--------------------------------------------------------------------------------------------------
//  Extended container operations.
//  Extended SimGroup and SimSet operations to implement counter-less iteration as well as
//  supplying a basic ScriptObject superclass for custom-built containers to implement a commandToClient
//  interface with.
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================
function ExtendedContainer::iterationBegin(%this)
{
    %this.currentIterationStack[%this.currentIterationStackCount] = 0;
    %this.currentIterationStackCount++;
}

function ExtendedContainer::getIterationIndex(%this)
{
    return %this.currentIterationStack[%this.currentIterationStackCount - 1];
}

function ExtendedContainer::getIterationDepth(%this)
{
    return %this.currentIterationStackCount;
}

function ExtendedContainer::nextObject(%this)
{
    %currentIndex = %this.currentIterationStack[%this.currentIterationStackCount - 1];
    %this.currentIterationStack[%this.currentIterationStackCount - 1]++;
    %result = %this.getObject(%currentIndex);
    return %result;
}

function ExtendedContainer::iterationEnd(%this)
{
    if (%this.currentIterationStack[%this.currentIterationStackCount - 1] >= %this.getCount())
    {
        %this.currentIterationStackCount--;
        return true;
    }

    return false;
}

function ExtendedContainer::getIndex(%this, %id)
{
    if (!isObject(%id))
        return -1;

    %id = %id.getID();

    // This may be called while we're already in an iteration, do a normal
    // for loop
    for (%iteration = 0; %iteration < %this.getCount(); %iteration++)
        if (%this.getObject(%iteration) == %id)
            return %iteration;

    return -1;
}

function ExtendedContainer::removeIndex(%this, %index)
{
    if (%index < 0 || %index >= %this.getCount())
        return;

    %this.remove(%this.getObject(%index));
}

function ExtendedContainer::insertAt(%this, %id, %index)
{
    if (%index < 0 || %index >= %this.getCount())
        return false;

    // The engine only supports adding objects to the end and start of
    // a SimGroup, so we have to coax it into it with this inefficient
    // method
    %arrangedObjectsCount = 0;

    for (%iteration = 0; %iteration < %this.getCount(); %iteration++)
    {
        if (%iteration == %index)
            %arrangedObjects[%arrangedObjectsCount++] = %id;

        %arrangedObjects[%arrangedObjectsCount++] = %this.getObject(%iteration);
    }

    %this.clear();
    for (%iteration = 0; %iteration < %arrangedObjectsCount; %iteration++)
        %this.add(%arrangedObjects[%iteration]);

    return true;
}

function SimSet::iterationBegin(%this)
{
    ExtendedContainer::iterationBegin(%this);
}

function SimSet::nextObject(%this)
{
    return ExtendedContainer::nextObject(%this);
}

function SimSet::iterationEnd(%this)
{
    return ExtendedContainer::iterationEnd(%this);
}

function SimSet::getIndex(%this, %id)
{
    return ExtendedContainer::getIndex(%this, %id);
}

function SimSet::removeIndex(%this, %index)
{
    return ExtendedContainer::removeIndex(%this, %index);
}

function SimSet::insertAt(%this, %id, %index)
{
    return ExtendedContainer::insertAt(%this, %id, %index);
}
