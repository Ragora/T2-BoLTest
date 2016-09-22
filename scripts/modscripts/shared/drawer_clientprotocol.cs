//--------------------------------------------------------------------------------------------------
//  Client protocol.
//  From here downwards is the network protocol that the client sports and in general you shouldn't
//  have to know what goes on down here for the magic to happen.
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================

// Draw a Sphere
$ClientDrawer::Type::Sphere = 1;
// Draw a Triangle that is represented by point 1, point 2, point 3.
$ClientDrawer::Type::PointTriangle = 2;
// Draw a Line
$ClientDrawer::Type::Line = 4;
// Draw a Circle
$ClientDrawer::Type::Circle = 8;
// Draw the bounding box of some object
$ClientDrawer::Type::BoundingBox = 16;

//--------------------------------------------------------------------------------------------------
//  Client command used to register a sphere drawer on the client.
//
//  Parameters:
//      %roundedID - The ID of the rounded (sphere, circle) drawer ID to bind to some
//      arbitrary object.
//      %ghostID - The clientside ghost ID (relevant to the client only) to bind the
//      desiginated rounded drawer to. The drawer then belongs to this object and will delete
//      itself on the client when this object ceases to exist.
//==================================================================================================
function clientCmdBindPrimitiveToObject(%id, %ghostIndex, %key)
{
    %obj = ClientDrawer::obtainObjectID(%ghostIndex, %key);
    if (!isObject(%obj))
        return;

    ClientDrawer::checkMissionGroup();

    // Do we have a valid drawer ID?
    %drawer = ClientDrawer::lookupPrimitive(%id, %key);

    if (isObject(%drawer) && (%drawer.clientDrawerType == $ClientDrawer::Type::Sphere || %drawer.clientDrawerType == $ClientDrawer::Type::Circle))
        %drawer.attachment = %obj;
}

function clientCmdUnbindPrimitive(%id, %ghostIndex, %key)
{
    %obj = ClientDrawer::obtainObjectID(%ghostIndex, %key);
    if (!isObject(%obj))
        return;

    ClientDrawer::checkMissionGroup();

    // Do we have a valid drawer ID?
    %drawer = ClientDrawer::lookupPrimitive(%id, %key);

    if (isObject(%drawer) && (%drawer.clientDrawerType == $ClientDrawer::Type::Sphere || %drawer.clientDrawerType == $ClientDrawer::Type::Circle))
        %drawer.attachment = -1;
}

function clientCmdBindPrimitiveToRaycast(%id, %key)
{
    ClientDrawer::checkMissionGroup();

    // Do we have a valid drawer ID?
    %drawer = ClientDrawer::lookupPrimitive(%id, %key);

    if (isObject(%drawer) && (%drawer.clientDrawerType == $ClientDrawer::Type::Sphere || %drawer.clientDrawerType == $ClientDrawer::Type::Circle))
        %drawer.attachment = "raycast";
}

//--------------------------------------------------------------------------------------------------
//  Client command used to register a sphere drawer on the client.
//      Parameters:
//          %radius - The radius of the sphere to draw.
//          %position - The center of the sphere to draw.
//          %lineColor - The color of the lines used to draw the triangles of the sphere. This
//          is specified in "R G B" format.
//          %fillColor - The filler color of the triangles used to draw the sphere. This is
//          specified in "R G B A" format.
//          %tessellation - This is an integer that specifies the geometrical complexity of
//          the sphere. The tessellation value starts at 0 and works its way up to some
//          hardcoded maximum tessellation value.
//          %id - If specified, this is the ID of an existing client drawer that is a sphere
//          to modify.
//==================================================================================================
function clientCmdSetClientSphere(%radius, %position, %lineColor, %fillColor, %tessellation, %id, %key)
{
    ClientDrawer::checkMissionGroup();

    if (%id !$= "")
    {
        %target = ClientDrawer::lookupPrimitive(%id, %key);

        if (!isObject(%target) || %target.clientDrawerType !$= $ClientDrawer::Type::Sphere)
            return;

        if (isObject(%target))
        {
            %target.radius = %radius;
            %target.lineColor = %lineColor;
            %target.fillColor = %fillColor;
            %target.tessellation = %tessellation;
            %target.setTransform(%position);
        }

        return;
    }

    %marker = new AudioEmitter()
    {
        isClientDrawer = true;
        radius = %radius;
        lineColor = %lineColor;
        fillColor = %fillColor;
        tessellation = %tessellation;
        clientDrawerType = $ClientDrawer::Type::Sphere;
        position = %position;
    };

    ClientDrawer::registerDrawer(%marker);
}

//--------------------------------------------------------------------------------------------------
//  Client command used to register a circle drawer on the client.
//      Parameters:
//          %radius - The radius of the circle to draw.
//          %normal - A normalized rotation to draw the circle at.
//          %position - The center of the circle to draw.
//          %lineColor - The color of the lines used to draw the lines of the circle. This
//          is specified in "R G B" format.
//          %fillColor - The filler color of the circle. This is specified in "R G B A" format.
//          %segments - This is an integer that specifies the geometrical complexity of
//          the circle. This directly controls the amount of lines used to draw the circle and
//          has a hardcoded minimum value of 6.
//          %id - If specified, this is the ID of an existing client drawer that is a sphere
//          to modify.
//==================================================================================================
function clientCmdSetClientCircle(%radius, %normal, %position, %lineColor, %fillColor, %segments, %id, %key)
{
    ClientDrawer::checkMissionGroup();

    if (%id !$= "")
    {
        %target = ClientDrawer::lookupPrimitive(%id, %key);

        if (!isObject(%target) || %target.clientDrawerType !$= $ClientDrawer::Type::Circle)
            return;

        if (isObject(%target))
        {
            %target.radius = %radius;
            %target.lineColor = %lineColor;
            %target.fillColor = %fillColor;
            %target.segments = %segments;
            %target.normal = %normal;
            %target.setTransform(%position);
        }

        return;
    }

    %marker = new AudioEmitter()
    {
        isClientDrawer = true;
        radius = %radius;
        lineColor = %lineColor;
        fillColor = %fillColor;
        segments = %segments;
        clientDrawerType = $ClientDrawer::Type::Circle;
        normal = %normal;
        position = %position;
    };

    ClientDrawer::registerDrawer(%marker);
}

//--------------------------------------------------------------------------------------------------
//      Client command used to register a triangle drawer on the client.
//      Parameters:
//          %p1 - The first point desiginated by "X Y Z".
//          %p2 - The second point desiginated by "X Y Z".
//          %p3 - The third point desiginated by "X Y Z".
//          %lineColor - The color of the lines used to draw the lines of the triangle. This
//          is specified in "R G B" format.
//          %fillColor - The filler color of the triangle. This is specified in "R G B A" format.
//          %id - If specified, this is the ID of an existing client drawer that is a triangle
//          to modify.
//==================================================================================================
function clientCmdSetClientPointTriangle(%p1, %p2, %p3, %lineColor, %fillColor, %id, %key)
{
    ClientDrawer::checkMissionGroup();

    if (%id !$= "")
    {
        %target = ClientDrawer::lookupPrimitive(%id, %key);

        if (!isObject(%target) || %target.clientDrawerType !$= $ClientDrawer::Type::PointTriangle)
            return;

        if (isObject(%target))
        {
            %target.pointTwo = %p2;
            %target.pointThree = %p3;
            %target.lineColor = %lineColor;
            %target.fillColor = %fillColor;
            %target.setTransform(%position);
        }

        return;
    }

    %marker = new AudioEmitter()
    {
        isClientDrawer = true;
        lineColor = %lineColor;
        fillColor = %fillColor;
        clientDrawerType = $ClientDrawer::Type::PointTriangle;
        position = %position;
        pointTwo = %p2;
        pointThree = %p3;
    };

    MissionGroup.add(%marker);
    MissionGroup.clientRenders[MissionGroup.clientRenderCount++] = %marker;
}

//--------------------------------------------------------------------------------------------------
//  Client command used to register a triangle drawer on the client.
//      Parameters:
//          %start - The start of the line desiginated by "X Y Z".
//          %end - The end of the line desiginated by "X Y Z".
//          %width - The width of the line.
//          %lineColor - The color of the lines used to draw the line. This is specified in "
//          R G B" format.
//          %id - If specified, this is the ID of an existing client drawer that is a line
//          to modify.
//==================================================================================================
function clientCmdSetClientLine(%start, %end, %width, %lineColor, %id, %key)
{
    ClientDrawer::checkMissionGroup();

    if (%id !$= "")
    {
        %target = ClientDrawer::lookupPrimitive(%id, %key);

        if (!isObject(%target) || %target.clientDrawerType !$= $ClientDrawer::Type::Line)
            return;

        if (isObject(%target))
        {
            %target.end = %end;
            %target.width = %width;
            %target.lineColor = %lineColor;
            %target.setTransform(%start);
        }

        return;
    }

    %marker = new AudioEmitter()
    {
        isClientDrawer = true;
        start = %start;
        end = %end;
        width = %width;
        position = %start;
        lineColor = %lineColor;
        clientDrawerType = $ClientDrawer::Type::Line;
    };

    ClientDrawer::registerDrawer(%marker);
}

function clientCmdDrawObjectBoundingBox(%ghostIndex, %color, %width, %key)
{
    ClientDrawer::checkMissionGroup();
    %obj = ClientDrawer::obtainObjectID(%ghostIndex, %key);

    if (!isObject(%obj))
        return;

    // Here, instead of creating a new draw marker for each, we just ensure that one always exists which will deal with
    // draws for commands like this.
    if (!isObject(BoundingBoxDrawMarker))
    {
        new AudioEmitter(BoundingBoxDrawMarker)
        {
            isClientDrawer = true;
            clientDrawerType = $ClientDrawer::Type::BoundingBox;
            targets = Array.Create();
        };
        MissionGroup.add(BoundingBoxDrawMarker);
    }

    $ClientDrawer::BoundingBoxColor[%obj] = %color;
    $ClientDrawer::BoundingBoxWidth[%obj] = %width;
    BoundingBoxDrawMarker.targets.Add(%obj);
}

function clientCmdClearObjectBoundingBox(%ghostIndex, %key)
{
    %obj = ClientDrawer::obtainObjectID(%ghostIndex, %key);

    if (!isObject(%obj))
        return;

    if (isObject(BoundingBoxDrawMarker))
        BoundingBoxDrawMarker.targets.removeValue(%obj);
}

//--------------------------------------------------------------------------------------------------
//  Client command used to remove a specific client drawer from the render. This does not decrement
//  the client's counter.
//      Parameters:
//          %id - The ID of the client drawer to remove, completely disregarding the type.
//==================================================================================================
function clientCmdDeletePrimitive(%id, %key)
{
    ClientDrawer::checkMissionGroup();
    %removed = ClientDrawer::lookupPrimitive(%id, %key);

    if (isObject(%removed))
    {
        MissionGroup.remove(%removed);
        %removed.delete();
    }
}

//--------------------------------------------------------------------------------------------------
//  Client command used to notify a client that they should clear all the currently active client
//  drawers. This merely removes them; it does not reset the counter for this client.
//==================================================================================================
function clientCmdClearPrimitives()
{
    if (!isObject(MissionGroup) || !isObject(MissionGroup.drawerArray))
        return;

    MissionGroup.drawerArray.beginIteration();
    while (!MissionGroup.drawerArray.iterationEnd())
    {
        %obj = MissionGroup.drawerArray.nextObject();
        %obj.delete();
    }

    MissionGroup.drawerArray.clear();
}

//--------------------------------------------------------------------------------------------------
//  Client command used to notify a client to use the client draw GUI required for the clientside
//  geometry drawer to work.
//==================================================================================================
function clientCmdUseClientDrawGUI()
{
    PlayGUIEditor.setVisible(1);
    %extent = PlayGUI.getExtent();
    PlayGUIEditor.setExtent(getWord($pref::Video::resolution, 0), getWord($pref::Video::resolution, 1));

    PlayGUI.bringToFront(PlayGUIEditor);
}

//--------------------------------------------------------------------------------------------------
//  Client command used to notify a client to use the regular PlayGUI which does not support
//  the clientside geometry drawing functionality.
//==================================================================================================
function clientCmdUseRegularPlayGUI()
{
    PlayGUIEditor.setVisible(0);
}
