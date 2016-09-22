//------------------------------------------------------------------------------
// Array.cs
// Array object you can pass around. This is helpful for storing sets of data
// in a clientside environment where SimGroups or SimSets are not available
// (because they don't recognize the existence of some script visible objects)
// or when not applicable (Because the data stored is not a SimObject). This
// would be a cleaner implementation than enforcing the sequential array logic
// in your own coding in most cases at the expense of one SimObject for the
// ArrayFactory instance called "Array" and one for each array instantiated.
//
// Please note that this is not a true array as Torque Script does not natively
// support such a data structure as of Tribes 2's engine. All assignments are
// done using a key, value assignment where array indices are translated to
// special field names.
//
// Arrays have two properties that are used in conjunction with the
// functions below:
//  .length - The number of indices currently in the array. This may be altered
//  at any point in time with no real negative consequences. Assign it to 0 for
//  clearing functionality.
//  .element[%index] - The value that resides at %index. There is no special
//  read function for doing the same thing because Torque cannot propertly
//  represent an erroneous read, so please use ::isValidIndex if this is a
//  concern.
// Copyright (c) 2016 Robert MacGregor
//==============================================================================

//------------------------------------------------------------------------------
// Description: Helper function to instantiate a new array, to avoid the
// standard script object instantiation which is slightly tedious.
// Parameter %name: The name of the new array to create. If something exists
// with the given name already, no name is used.
// Note: Do not use this directly, please invoke it against the ArrayFactory
// instance which is conveniently named "Array".
//==============================================================================
function ArrayFactory::create(%this, %name)
{
	if (isObject(%name))
		%name = "";

	%object = new ScriptObject(%name) { class = "ArrayObject"; superclass = "ExtendedContainer"; length = 0; };
	return %object;
}

//------------------------------------------------------------------------------
// Description: Adds a new value to the array, placing it at the end.
// Parameter %value: The value to insert.
//==============================================================================
function ArrayObject::add(%this, %value)
{
    %this.element[%this.length] = %value;
    %this.length++;
}

//------------------------------------------------------------------------------
// Description: Sets a new value at %index of the array.
// Parameter %index: The index to assign to.
// Parameter %value: The value to write.
// Returns: False When %index is < 0 or >= the total number of indices in the
// array, true when the write is successful.
//==============================================================================
function ArrayObject::setElement(%this, %index, %value)
{
    if (%index >= %this.length || %index < 0)
        return false;

    %this.element[%index] = %value;
    return true;
}

function ArrayObject::remove(%this, %value)
{
	return %this.removeValue(%value);
}

//------------------------------------------------------------------------------
// Description: Removes an element from the array at the specified %index,
// pushing all elements ahead back one element to compensate.
// Parameter %index: The index of the element to remove.
// Returns: False When %index is < 0 or >= the total number of indices in the
// array, true when the removal is successful.
//==============================================================================
function ArrayObject::removeIndex(%this, %index)
{
    if (%index >= %this.length || %index < 0)
        return false;

    %this.element[%index] = "";
    for (%i = %index; %i < %this.length; %i++)
        %this.element[%i] = %this.element[%i + 1];

	%this.length--;
	return true;
}

//------------------------------------------------------------------------------
// Description: Determines whether or not the array contains %value.
// Parameteter %value: The value to search for.
// Returns: True when %value is found, false when not.
//==============================================================================
function ArrayObject::contains(%this, %value)
{
    return %this.valueLocation(%value) != -1;
}

function ArrayObject::valueLocation(%this, %value)
{
    for (%i = 0; %i < %this.length; %i++)
		if (%this.element[%i] == %value)
			return %i;

	return -1;
}

function ArrayObject::removeValue(%this, %value)
{
    %location = %this.valueLocation(%value);

    if (%location == -1)
        return false;

    %this.removeIndex(%location);
    return true;
}

//------------------------------------------------------------------------------
// Description: Determines whether or not %index is a valid index for this
// array.
// Returns: False When %index is < 0 or >= the total number of indices in the
// array, true when a valid index.
//==============================================================================
function ArrayObject::isValidIndex(%this, %index)
{
    return %index >= 0 && %index < %this.length;
}

//------------------------------------------------------------------------------
// Description: Clears the array's data by assigning length = 0. This is here
// for compatability with the extended container interface.
//==============================================================================
function ArrayObject::clear(%this)
{
    %this.length = 0;
}

//------------------------------------------------------------------------------
// Description: Returns the array's .length value. This is here for
// compatability with the extended container interface.
//==============================================================================
function ArrayObject::getCount(%this)
{
    return %this.length;
}

//------------------------------------------------------------------------------
// Description: Returns the object at array index %index. This is here for
// compatability with the extended container interface.
//==============================================================================
function ArrayObject::getObject(%this, %index)
{
    return %this.element[%index];
}

// Instantiate the factory instance.
if (!IsObject(Array))
	new ScriptObject(Array) { class = "ArrayFactory"; superclass = "ExtendedContainer"; };
